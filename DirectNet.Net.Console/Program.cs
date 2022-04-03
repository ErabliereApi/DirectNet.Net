using DirectNet.Net;
using DirectNet.Net.Extensions;
using DirectNet.Net.Helpers;
using Microsoft.Extensions.Logging;

var logger = LoggerFactory.Create(builder =>
{
    builder.SetMinimumLevel(LogLevel.Warning)
           .AddConsole();
});

var directnet = new DirectNetClient("COM4", 5000, logger: logger.CreateLogger<DirectNetClient>());

try
{
    directnet.Open();

    var begin = OctalHelper.FromOctal("4000");

    begin += 1; // Add the offset

    for (int a = 0; a < 24; a++)
    {
        var value = await directnet.ReadVMemoryLocationAsync((begin + a).ToString("X"));

        Console.WriteLine($"Memory Address: {802 + a} Main {a + 1,2}: {value}");
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex);
}
finally
{
    directnet.Close();
}