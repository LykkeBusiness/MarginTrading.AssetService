using Kathe;
using Kathe.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;


var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string> { { LogConfigurationExtensions.Conventions.Configuration.BrokerId, "test-broker" } })
    .AddJsonFile("appsettings.json")
    .Build();

var logger = LogConfiguration.BuildSerilogLogger(configuration, "LocalTestingConsole");
new HostBuilder()
    .ConfigureServices(
        services =>
        {
            services.AddEnrichedLogging();
            services.AddHttpClient();
            services.Configure<KatheLoggingOptions>(configuration.GetSection("KatheLogging"));
        })
    .ConfigureWebHost(
        webHost =>
        {
            webHost
                .UseConfiguration(configuration)
                .UseKestrel()
                .UseSerilog(logger)
                .UseUrls("http://localhost:5000")
                .Configure(
                    applicationBuilder =>
                    {
                        applicationBuilder.UseEnrichedLogging();
                        applicationBuilder.Use(
                            next => async context =>
                            {
                                if (context.Request.Path == "/favicon.ico")
                                    return;
                                ILogger<Program> log = context.RequestServices.GetService<ILogger<Program>>();
                                
                                log.LogTrace("Trace message");
                                log.LogDebug("Debug message");
                                log.LogInformation("Information message");
                                log.LogWarning("Warning message");
                                try
                                {
                                    throw new Exception("Something goes very wrong");
                                }
                                catch (Exception e)
                                {
                                    log.LogError(e, "Error happened");
                                }

                                log.LogCritical("Fatal Error message");

                                var response = await context.RequestServices.GetService<HttpClient>().GetAsync("http://google.com");
                                
                                await context.Response.WriteAsync($"google says {response.StatusCode}");
                            });
                    });

        })
    .Build()
    .Run();
    