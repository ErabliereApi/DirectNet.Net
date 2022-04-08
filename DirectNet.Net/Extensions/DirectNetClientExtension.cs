using DirectNet.Net.Helpers;

namespace DirectNet.Net.Extensions;

public static class DirectNetClientExtension
{
    /// <summary>
    /// Read a V memory location
    /// </summary>
    /// <param name="directnet"></param>
    /// <param name="address">The VMemory address with the prefix V. Example: V4000</param>
    /// <returns></returns>
    public static async Task<int> ReadVMemoryLocationAsync(this IDirectNetClient directnet, string address)
    {
        var response = await directnet.ReadAsync(address, 2);

        var value = BCDHelper.FromBCD(response[0], response[1]);

        return value;
    }

    /// <summary>
    /// Read many VMemory Location
    /// </summary>
    /// <param name="directnet"></param>
    /// <param name="address">The VMemory address with the prefix V. Example: V4000</param>
    /// <param name="nbVMemoryAddress">The number of VMemory location to read</param>
    /// <returns></returns>
    public static async Task<int[]> ReadVMemoryLocationsAsync(this IDirectNetClient directnet, string address, int nbVMemoryAddress, CancellationToken token = default)
    {
        var bytes = await directnet.ReadAsync(address, nbVMemoryAddress * 2, token: token);

        var values = new int[nbVMemoryAddress];

        for (int i = 0; i < bytes.Length; i += 2)
        {
             values[i / 2] = BCDHelper.FromBCD(bytes[i], bytes[i + 1]);
        }

        return values;
    }
}
