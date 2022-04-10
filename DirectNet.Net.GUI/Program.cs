using ErabliereAPI.Proxy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text.Json;

namespace DirectNet.Net.GUI;

internal static class Program
{
    private static ErabliereApiOptionsWithSensors? _options;

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        var logger = new LoggerConfiguration()
            .WriteTo.File("log.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

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
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1(provider));
    }
}