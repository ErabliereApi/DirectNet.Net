using CustomLogic.Common;
using DirectNet.Net;
using DirectNet.Net.Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var (provider, _) = ServicesSetup.GetServiceProvider();

var directnet = new DirectNetClient(args[0], 5000, logger: provider.GetRequiredService<ILogger<DirectNetClient>>());

int returnCode = 1;

try
{
    var values = await Examples.ReadVMemoryLocationInBCD(directnet);

    await ErabliereApiTasks.Send24ValuesAsync(provider, values, CancellationToken.None);

    returnCode = 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex);
}
finally
{
    directnet.Close();
}

return returnCode;