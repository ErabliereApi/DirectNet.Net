namespace DirectNet.Net.Extensions;

public static class DirectNetClientExtension
{
    public static async Task<int> ReadVMemoryLocationAsync(this DirectNetClient directnet, string vMemoryAddressInHex)
    {
        var response = await directnet.ReadAsync(vMemoryAddressInHex, 2);

        var i = 0;

        if (response[i] != ControlChar.ACK)
        {
            throw new Exception($"Ack invalid. Got {response[i]}");
        }

        if (response[++i] != ControlChar.STX)
        {
            throw new Exception($"Excpected start of data block but got : {response[i]}");
        }

        i++;
        var value = 0;

        value += response[i++];
        value += response[i++] * 100;

        if (response[i] != ControlChar.ETX)
        {
            throw new Exception($"Excepted end of block but got : {response[i]}");
        }

        i++;

        if (LRCHelper.CalculateLRC(response.Skip(1).ToArray(), ControlChar.ETX) != response[i])
        {
            throw new Exception($"Chacksum verification failed");
        }

        return value;
    }
}
