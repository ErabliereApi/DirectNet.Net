using DirectNet.Net;
using DirectNet.Net.Extensions;
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

    var values = await directnet.ReadVMemoryLocationsAsync("V4000", 24);

    for (int i = 0; i < values.Length; i++)
    {
        Console.WriteLine($"{i} {values[i]}");
    }

    // This will write value 255 in memory location V4000 and 2 in memory location V4001
    await directnet.WriteAsync("V4000", new byte[] { 0x00, 0xFF, 0x00, 0x02 });

    values = await directnet.ReadVMemoryLocationsAsync("V4000", 24);

    for (int i = 0; i < values.Length; i++)
    {
        Console.WriteLine($"{i} {values[i]}");
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