using ErabliereAPI.Proxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text.Json;

namespace CustomLogic.Common;

public static class ServicesSetup
{
    private static ErabliereApiOptionsWithSensors? _options;

    public static (IServiceProvider, ILogger) GetServiceProvider()
    {
        var logger = new LoggerConfiguration()
            .WriteTo.File("log.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        logger.Information("Starting DirectNet.Net.GUI app.");

        IServiceCollection serviceCollection = new ServiceCollection()
            .AddMemoryCache()
            .AddTransient<AzureADClientCredentialsHandler>()
            .AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddSerilog(logger);
            });
        serviceCollection.AddOptions<ErabliereApiOptionsWithSensors>().Configure(o =>
        {
            if (_options == null)
            {
                var configPath = Environment.GetEnvironmentVariable("ERABLIERE_API_CONFIG") ?? "";
                _options = JsonSerializer.Deserialize<ErabliereApiOptionsWithSensors>(File.ReadAllText(configPath)) ?? throw new InvalidOperationException("Options cannot be null");
            }

            o.BaseUrl = _options.BaseUrl;
            o.ClientId = _options.ClientId;
            o.ClientSecret = _options.ClientSecret;
            o.Scope = _options.Scope;
            o.TenantId = _options.TenantId;
            o.CapteursIds = _options.CapteursIds;
            o.SendIntervalInMinutes = _options.SendIntervalInMinutes;
            o.PLCScanFrequencyInSeconds = _options.PLCScanFrequencyInSeconds;
        });
        serviceCollection.AddOptions<AzureADClientCreadentialsOptions>().Configure<IOptions<ErabliereApiOptionsWithSensors>>((o, d) =>
        {
            o.ClientId = d.Value.ClientId;
            o.ClientSecret = d.Value.ClientSecret;
            o.Scope = d.Value.Scope;
            o.TenantId = d.Value.TenantId;
        });
        serviceCollection.AddHttpClient("ErabliereAPI")
                         .AddHttpMessageHandler<AzureADClientCredentialsHandler>();
        IServiceProvider provider = serviceCollection.BuildServiceProvider();

        logger.Information("ServiceProvider is ready.");

        return (provider, logger);
    }
}
