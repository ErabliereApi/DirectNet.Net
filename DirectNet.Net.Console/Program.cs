using DirectNet.Net;
using DirectNet.Net.Console;
using Microsoft.Extensions.Logging;

var logger = LoggerFactory.Create(builder =>
{
    builder.SetMinimumLevel(LogLevel.Warning)
           .AddConsole();
});

var directnet = new DirectNetClient("COM4", 5000, logger: logger.CreateLogger<DirectNetClient>());

int returnCode = 1;

try
{
    await Examples.ReadAndWriteVMemoryLocation(directnet);

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