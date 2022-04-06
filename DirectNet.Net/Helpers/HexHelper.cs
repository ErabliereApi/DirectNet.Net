namespace DirectNet.Net.Helpers;

public static class HexHelper
{
    /// <summary>
    /// Convert an integer to hex values ready to send in a DirectNet communication. 
    /// GetDirectNetHex(01) => [0x30, 0x31]
    /// GetDirectNetHex(10) => [0x30, 0x3A]
    /// </summary>
    public static byte[] GetDirectNetHex(int number, int arrayReturnMinSize = 1)
    {
        var str = number.ToString("X").PadLeft(arrayReturnMinSize, '0');

        var bytes = new byte[str.Length];

        for (int i = bytes.Length - 1; i >= 0; i--)
        {
            bytes[i] = (byte)str[i];
        }

        return bytes;
    }

    public static byte PrepareForHeader(int value)
    {
        if (value > 20)
        {
            return (byte)value;
        }

        return (byte)(0x30 + value);
    }
}
