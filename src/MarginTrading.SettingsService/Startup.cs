using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Middleware;
using Lykke.Common.ApiLibrary.Swagger;
using Lykke.Logs;
using Lykke.Logs.MsSql;
using Lykke.Logs.MsSql.Repositories;
using Lykke.SettingsReader;
using Lykke.SettingsReader.ReloadingManager;
using Lykke.SlackNotification.AzureQueue;
using MarginTrading.SettingsService.Core.Domain;
using MarginTrading.SettingsService.Core.Services;
using MarginTrading.SettingsService.Modules;
using MarginTrading.SettingsService.Services;
using MarginTrading.SettingsService.Settings;
using MarginTrading.SettingsService.SqlRepositories.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using LogEntity = Lykke.Logs.LogEntity;

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

                services.AddSwaggerGen(options =>
                {
                    options.DefaultLykkeConfiguration("v1", $"{ServiceName} API");
                    var contractsXmlPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, 
                        "MarginTrading.SettingsService.Contracts.xml");
                    options.IncludeXmlComments(contractsXmlPath);
                });

                var builder = new ContainerBuilder();
                var appSettings = Configuration.LoadSettings<AppSettings>();

                Log = CreateLogWithSlack(services, appSettings);

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

        private static ILog CreateLogWithSlack(IServiceCollection services, IReloadingManager<AppSettings> settings)
        {
            var consoleLogger = new LogToConsole();
            var aggregateLogger = new AggregateLogger();

            aggregateLogger.AddLog(consoleLogger);
            
            if (settings.CurrentValue.MarginTradingSettingsService.Db.StorageMode == StorageMode.SqlServer)
            {
                if (string.IsNullOrEmpty(settings.CurrentValue.MarginTradingSettingsService.Db.LogsConnString))
                {
                    throw new Exception("SqlConnectionString must have a value if StorageMode is SqlServer");
                }
                
                var sqlLogger = new LogToSql(new SqlLogRepository("SettingsServiceLog",
                    settings.CurrentValue.MarginTradingSettingsService.Db.LogsConnString));

                aggregateLogger.AddLog(sqlLogger);
            } 
            else if (settings.CurrentValue.MarginTradingSettingsService.Db.StorageMode == StorageMode.Azure)
            {
                if (string.IsNullOrEmpty(settings.CurrentValue.MarginTradingSettingsService.Db.LogsConnString))
                {
                    throw new Exception("LogsAzureConnString must have a value if StorageMode is Azure");
                }
                
                var dbLogConnectionStringManager =
                    settings.Nested(x => x.MarginTradingSettingsService.Db.LogsConnString);
                var dbLogConnectionString = dbLogConnectionStringManager.CurrentValue;

                if (string.IsNullOrEmpty(dbLogConnectionString))
                {
                    consoleLogger.WriteWarningAsync(nameof(Startup), nameof(CreateLogWithSlack),
                        "Table logger is not initialized").Wait();
                    return aggregateLogger;
                }

                if (dbLogConnectionString.StartsWith("${") && dbLogConnectionString.EndsWith("}"))
                    throw new InvalidOperationException(
                        $"LogsConnString {dbLogConnectionString} is not filled in settings");

                var persistenceManager = new LykkeLogToAzureStoragePersistenceManager(
                    AzureTableStorage<LogEntity>.Create(dbLogConnectionStringManager, "SettingsServiceLog", consoleLogger),
                    consoleLogger);

                LykkeLogToAzureSlackNotificationsManager slackNotificationsManager = null;
                if (settings.CurrentValue.SlackNotifications != null)
                {
                    // Creating slack notification service, which logs own azure queue processing messages to aggregate log
                    var slackService = services.UseSlackNotificationsSenderViaAzureQueue(
                        new Lykke.AzureQueueIntegration.AzureQueueSettings
                        {
                            ConnectionString = settings.CurrentValue.SlackNotifications.AzureQueue.ConnectionString,
                            QueueName = settings.CurrentValue.SlackNotifications.AzureQueue.QueueName
                        }, aggregateLogger);

                    slackNotificationsManager =
                        new LykkeLogToAzureSlackNotificationsManager(slackService, consoleLogger);
                }

                // Creating azure storage logger, which logs own messages to console log
                var azureStorageLogger = new LykkeLogToAzureStorage(
                    persistenceManager,
                    slackNotificationsManager,
                    consoleLogger);

                azureStorageLogger.Start();

                aggregateLogger.AddLog(azureStorageLogger);
            }

            LogLocator.Log = aggregateLogger;

            return aggregateLogger;
        }
    }
}
