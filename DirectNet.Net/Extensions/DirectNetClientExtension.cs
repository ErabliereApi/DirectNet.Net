using DirectNet.Net.Helpers;

namespace DirectNet.Net.Extensions;

public static class DirectNetClientExtension
{
    public static async Task<int> ReadVMemoryLocationAsync(this IDirectNetClient directnet, string vMemoryAddressInHex)
    {
        var response = await directnet.ReadAsync(vMemoryAddressInHex, 2);

        var value = BCDHelper.FromBCD(response[0], response[1]);

        return value;
    }
}
