using DirectNet.Net.Extensions;
using DirectNet.Net.Helpers;

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

    public static async Task<int[]> ReadVMemoryLocationInBCD(IDirectNetClient directnet)
    {
        directnet.Open();

        var values = await directnet.ReadVMemoryLocationsAsync("V4000", 24, Static.FormatType.BCD);

        for (int i = 0; i < values.Length; i++)
        {
            System.Console.WriteLine($"{i} {values[i]}");
        }

        return values;
    }

    public static async Task<byte[]> ReadInputPoints(IDirectNetClient directnet)
    {
        directnet.Open();

        // Work in progress
        var count = OctalHelper.FromOctal("40437") - OctalHelper.FromOctal("40400");

        System.Console.WriteLine($"Count: {count}");

        var values = await directnet.ReadAsync("V40400", count);

        for (int i = 0; i < values.Length; i++)
        {
            System.Console.WriteLine($"{i} {Convert.ToString(values[i], 2).PadLeft(16, '0')}");
        }

        return values;
    }
}
