using Kathe;
using Kathe.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;


var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
{
    { LogConfigurationExtensions.Conventions.Configuration.BrokerId, "test-broker" },
    { LogConfigurationExtensions.Conventions.Environment.AspnetCore, "local" },
    { LogConfigurationExtensions.Conventions.Environment.SeqUrl, "http://138.201.190.36:5341" },
    { LogConfigurationExtensions.Conventions.Environment.SeqApiKey, "Rj83s0Ao4y18iwI0b726" },
    { LogConfigurationExtensions.Conventions.Environment.Nova, "LOCAL-MISHA" },
}).Build();

var logger = LogConfiguration.BuildSerilogLogger(configuration, "LocalTestingConsole");
new HostBuilder()
    .ConfigureServices(
        services =>
        {
            services.AddEnrichedLogging();
            services.AddSingleton((new KatheLoggingOptions()).All());
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
                                context.RequestServices.GetService<ILogger<Program>>().LogTrace("Trace message");
                                context.RequestServices.GetService<ILogger<Program>>().LogDebug("Debug message");
                                context.RequestServices.GetService<ILogger<Program>>().LogInformation("Information message");
                                context.RequestServices.GetService<ILogger<Program>>().LogWarning("Warning message");
                                context.RequestServices.GetService<ILogger<Program>>().LogError("Error message");
                                context.RequestServices.GetService<ILogger<Program>>().LogCritical("Critical message");
                                try
                                {
                                    throw new Exception("Something goes very wrong");
                                }
                                catch (Exception e)
                                {
                                    context.RequestServices.GetService<ILogger<Program>>().LogError(e, "Error happened");
                                }
                                await context.Response.WriteAsync("hi");
                                
                                throw new Exception("Critical fail.");
                            });
                    })
                .Build()
                .Run();
        });
    