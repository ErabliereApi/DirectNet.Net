using DirectNet.Net;
using Microsoft.Extensions.Logging;

var logger = LoggerFactory.Create(builder =>
{
    builder.SetMinimumLevel(LogLevel.Debug)
           .AddConsole();
});

var directnet = new DirectNetClient("COM4", 5000, logger: logger.CreateLogger<DirectNetClient>());

try
{
    directnet.Open();

    for (int a = 0; a < 24; a++)
    {
        var response = await directnet.ReadAsync((801 + a).ToString(), 2);

        if (response[0] != ControlChar.ACK)
        {
            throw new Exception($"Ack invalid. Got {response[0]}");
        }

        if (response[1] != ControlChar.STX)
        {
            throw new Exception($"Excpected start of data block but got : {response[1]}");
        }

        var nextIndex = 2;

        for (int i = 2; i < response.Length && response[i] != ControlChar.ETX;)
        {
            Console.WriteLine(response[i]);

            nextIndex = ++i;
        }

        if (response[nextIndex] != ControlChar.ETX)
        {
            throw new Exception($"Excepted end of block but got : {response[nextIndex]}");
        }

        nextIndex++;

        if (LRCHelper.CalculateLRC(response.Skip(1).ToArray(), ControlChar.ETX)[1] != response[nextIndex])
        {
            throw new Exception($"Chacksum verification failed");
        }
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

