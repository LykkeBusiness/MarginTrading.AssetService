// Copyright (c) 2019 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using JetBrains.Annotations;
using Lykke.AzureQueueIntegration;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Logs;
using Lykke.Logs.MsSql;
using Lykke.Logs.MsSql.Repositories;
using Lykke.Logs.Serilog;
using Lykke.SettingsReader;
using Lykke.SlackNotification.AzureQueue;
using Lykke.SlackNotifications;
using Lykke.Snow.Common.Startup;
using Lykke.Snow.Common.Startup.ApiKey;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.Modules;
using MarginTrading.SettingsService.Services;
using MarginTrading.SettingsService.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace MarginTrading.SettingsService
{
    public class Startup
    {
        public static string ServiceName { get; } = PlatformServices.Default.Application.ApplicationName;
        
        public IHostingEnvironment Environment { get; }
        public IContainer ApplicationContainer { get; private set; }
        public IConfigurationRoot Configuration { get; }
        public ILog Log { get; private set; }
        
        public Startup(IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddSerilogJson(env)
                .AddEnvironmentVariables()
                .Build();

            Environment = env;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddMvc()
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                        options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    });
                
                var appSettings = Configuration.LoadSettings<AppSettings>();
                
                services.AddApiKeyAuth(appSettings.CurrentValue.MarginTradingSettingsServiceClient);

                services.AddSwaggerGen(options =>
                {
                    options.DefaultLykkeConfiguration("v1", $"{ServiceName} API");
                    var contractsXmlPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, 
                        "MarginTrading.SettingsService.Contracts.xml");
                    options.IncludeXmlComments(contractsXmlPath);
                    if (!string.IsNullOrWhiteSpace(appSettings.CurrentValue.MarginTradingSettingsServiceClient?.ApiKey))
                    {
                        options.OperationFilter<ApiKeyHeaderOperationFilter>();
                    }
                });

                var builder = new ContainerBuilder();

                Log = CreateLogWithSlack(Configuration, services, appSettings);

                builder.RegisterModule(new ServiceModule(appSettings.Nested(x => x.MarginTradingSettingsService), Log));
                builder.RegisterModule(new CqrsModule(appSettings.CurrentValue.MarginTradingSettingsService.Cqrs, Log));
                builder.Populate(services);
                ApplicationContainer = builder.Build();

                return new AutofacServiceProvider(ApplicationContainer);
            }
            catch (Exception ex)
            {
                Log?.WriteFatalError(nameof(Startup), nameof(ConfigureServices), ex);
                throw;
            }
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            try
            {
                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseHsts();
                }
                
#if DEBUG
                app.UseLykkeMiddleware(ServiceName, ex => ex.ToString());
#else
                app.UseLykkeMiddleware(ServiceName, ex => new ErrorResponse {ErrorMessage = ex.Message});
#endif
      
                app.UseAuthentication();
                app.UseMvc();
                app.UseSwagger();
                app.UseSwaggerUI(a => a.SwaggerEndpoint("/swagger/v1/swagger.json", "Settings Service API Swagger"));

                appLifetime.ApplicationStarted.Register(() => StartApplication().GetAwaiter().GetResult());
                appLifetime.ApplicationStopping.Register(() => StopApplication().GetAwaiter().GetResult());
                appLifetime.ApplicationStopped.Register(() => CleanUp().GetAwaiter().GetResult());
            }
            catch (Exception ex)
            {
                Log?.WriteFatalError(nameof(Startup), nameof(Configure), ex);
                throw;
            }
        }

        private async Task StartApplication()
        {
            try
            {
                // NOTE: Service not yet receive and process requests here

                await ApplicationContainer.Resolve<IStartupManager>().StartAsync();

                await Log.WriteMonitorAsync("", $"Env: {Program.EnvInfo}", "Started");
            }
            catch (Exception ex)
            {
                await Log.WriteFatalErrorAsync(nameof(Startup), nameof(StartApplication), "", ex);
                throw;
            }
        }

        private async Task StopApplication()
        {
            try
            {
                // NOTE: Service still can receive and process requests here, so take care about it if you add logic here.

                await ApplicationContainer.Resolve<IShutdownManager>().StopAsync();
            }
            catch (Exception ex)
            {
                if (Log != null)
                {
                    await Log.WriteFatalErrorAsync(nameof(Startup), nameof(StopApplication), "", ex);
                }
                throw;
            }
        }

        private async Task CleanUp()
        {
            try
            {
                // NOTE: Service can't receive and process requests here, so you can destroy all resources

                if (Log != null)
                {
                    await Log.WriteMonitorAsync("", $"Env: {Program.EnvInfo}", "Terminating");
                }

                ApplicationContainer.Dispose();
            }
            catch (Exception ex)
            {
                if (Log != null)
                {
                    await Log.WriteFatalErrorAsync(nameof(Startup), nameof(CleanUp), "", ex);
                    (Log as IDisposable)?.Dispose();
                }
                throw;
            }
        }

        private static ILog CreateLogWithSlack(IConfiguration configuration, IServiceCollection services, 
            IReloadingManager<AppSettings> settings)
        {
            const string requestsLogName = "SettingsServiceRequestsLog";
            const string logName = "SettingsServiceLog";
            var consoleLogger = new LogToConsole();
            
            #region Logs settings validation

            if (!settings.CurrentValue.MarginTradingSettingsService.UseSerilog 
                && string.IsNullOrWhiteSpace(settings.CurrentValue.MarginTradingSettingsService.Db.LogsConnString))
            {
                throw new Exception("Either UseSerilog must be true or LogsConnString must be set");
            }

            #endregion Logs settings validation
            
            #region Slack registration

            ISlackNotificationsSender slackService = null;

            if (settings.CurrentValue.SlackNotifications != null)
            {
                var azureQueue = new AzureQueueSettings
                {
                    ConnectionString = settings.CurrentValue.SlackNotifications.AzureQueue.ConnectionString,
                    QueueName = settings.CurrentValue.SlackNotifications.AzureQueue.QueueName
                };

                slackService =
                    services.UseSlackNotificationsSenderViaAzureQueue(azureQueue, consoleLogger);
            }

            #endregion Slack registration
            
            if (settings.CurrentValue.MarginTradingSettingsService.UseSerilog)
            {
                var serilogLogger = new SerilogLogger(typeof(Startup).Assembly, configuration);

                LogLocator.RequestsLog = LogLocator.CommonLog = serilogLogger;

                return serilogLogger;
            }

            if (settings.CurrentValue.MarginTradingSettingsService.Db.StorageMode == StorageMode.SqlServer)
            {
                LogLocator.CommonLog = new AggregateLogger(
                    new LogToSql(new SqlLogRepository(logName,
                        settings.CurrentValue.MarginTradingSettingsService.Db.LogsConnString)),
                    new LogToConsole());
                
                LogLocator.RequestsLog = new AggregateLogger(
                    new LogToSql(new SqlLogRepository(requestsLogName,
                        settings.CurrentValue.MarginTradingSettingsService.Db.LogsConnString)),
                    new LogToConsole());

                return LogLocator.CommonLog;
            }

            if (settings.CurrentValue.MarginTradingSettingsService.Db.StorageMode != StorageMode.Azure)
            {
                throw new Exception("Wrong config! Logging must be set either to Serilog, SqlServer or Azure.");
            }

            #region Azure logging

            LogLocator.RequestsLog = services.UseLogToAzureStorage(settings.Nested(s => 
                    s.MarginTradingSettingsService.Db.LogsConnString),
                slackService, requestsLogName, consoleLogger);

            LogLocator.CommonLog = services.UseLogToAzureStorage(settings.Nested(s => 
                    s.MarginTradingSettingsService.Db.LogsConnString),
                slackService, logName, consoleLogger);

            return LogLocator.CommonLog;

            #endregion Azure logging
        }
    }
}
