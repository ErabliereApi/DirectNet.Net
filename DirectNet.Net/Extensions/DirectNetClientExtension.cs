using DirectNet.Net.Helpers;
using DirectNet.Net.Static;

namespace DirectNet.Net.Extensions;

public static class DirectNetClientExtension
{
    /// <summary>
    /// Read a V memory location
    /// </summary>
    /// <param name="directnet"></param>
    /// <param name="address">The VMemory address with the prefix V. Example: V4000</param>
    /// <returns></returns>
    public static async Task<int> ReadVMemoryLocationAsync(this IDirectNetClient directnet, string address, FormatType formatType = default)
    {
        var response = await directnet.ReadAsync(address, 2);

        return FromFormat(response[0], response[1], formatType);
    }

    /// <summary>
    /// Read many VMemory Location
    /// </summary>
    /// <param name="directnet"></param>
    /// <param name="address">The VMemory address with the prefix V. Example: V4000</param>
    /// <param name="nbVMemoryAddress">The number of VMemory location to read</param>
    /// <returns></returns>
    public static async Task<int[]> ReadVMemoryLocationsAsync(this IDirectNetClient directnet, string address, int nbVMemoryAddress, FormatType format = default, CancellationToken token = default)
    {
        var bytes = await directnet.ReadAsync(address, nbVMemoryAddress * 2, token: token);

        var values = new int[nbVMemoryAddress];

        for (int i = 0; i < bytes.Length; i += 2)
        {
            values[i / 2] = FromFormat(bytes[i], bytes[i + 1], format);
        }

        return values;
    }

    private static int FromFormat(byte b1, byte b2, FormatType formatType)
    {
        return formatType switch
        {
            FormatType.Binary => BinaryHelper.FromBinary(b1, b2),
            FormatType.BCD => BCDHelper.FromBCD(b1, b2),
            _ => throw new NotImplementedException(),
        };
    }
}
