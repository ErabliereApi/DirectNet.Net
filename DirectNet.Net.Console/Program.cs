using CustomLogic.Common;
using DirectNet.Net;
using DirectNet.Net.Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO.Ports;

var (provider, _) = ServicesSetup.GetServiceProvider();

var comPort = SerialPort.GetPortNames().FirstOrDefault();

if (args.Length > 0)
{
    comPort = args[0];
}

if (comPort == null)
{
    throw new ArgumentException("No comport send as parameter or automatically detected");
}

var directnet = new DirectNetClient(comPort, 5000, logger: provider.GetRequiredService<ILogger<DirectNetClient>>());

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