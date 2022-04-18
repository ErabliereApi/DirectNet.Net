using ErabliereAPI.Proxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CustomLogic.Common;

public static class ErabliereApiTasks
{
    public static async Task Send24ValuesAsync(IServiceProvider _provider, int[] values, CancellationToken token)
    {
        var options = _provider.GetRequiredService<IOptions<ErabliereApiOptionsWithSensors>>().Value;

        using var scope = _provider.CreateScope();

        var httpClientfactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

        var httpClient = httpClientfactory.CreateClient("ErabliereAPI");

        var erabliereApi = new ErabliereApiProxy(options.BaseUrl, httpClient);
        
        for (int i = 0; i < values.Length; i++)
        {
            await erabliereApi.DonneesCapteurPOSTAsync(options.CapteursIds[i], new PostDonneeCapteur
            {
                IdCapteur = options.CapteursIds[i],
                V = values[i]
            }, token);
        }
    }
}
