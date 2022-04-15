using DirectNet.Net.Extensions;

namespace DirectNet.Net.Console;

public static class Examples
{
    public static async Task ReadAndWriteVMemoryLocation(IDirectNetClient directnet)
    {
        directnet.Open();

        var values = await directnet.ReadVMemoryLocationsAsync("V4000", 24);

        for (int i = 0; i < values.Length; i++)
        {
            System.Console.WriteLine($"{i} {values[i]}");
        }

        // This will write value 255 in memory location V4000 and 2 in memory location V4001
        // in the binary format. For BCD there is no helper method for now.
        await directnet.WriteAsync("V4000", new byte[] { 0x00, 0xFF, 0x00, 0x02 });

        values = await directnet.ReadVMemoryLocationsAsync("V4000", 24);

        for (int i = 0; i < values.Length; i++)
        {
            System.Console.WriteLine($"{i} {values[i]}");
        }
    }
}
