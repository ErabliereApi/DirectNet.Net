namespace DirectNet.Net.Helpers;

public static class HexHelper
{
    /// <summary>
    /// Convert an integer to hex values ready to send in a DirectNet communication. 
    /// GetHex(10) => [0x31, 0x30]
    /// </summary>
    public static byte[] GetDirectNetHex(int number, int arrayReturnMinSize = 1)
    {
        var str = number.ToString().PadLeft(arrayReturnMinSize, '0');

        var bytes = new byte[str.Length];

        for (int i = bytes.Length - 1; i >= 0; i--)
        {
            bytes[i] = (byte)(0x30 + ParseHex(str[i]));
        }

        return bytes;
    }

    public static byte ParseHex(char hex)
    {
        return byte.Parse(hex.ToString(), System.Globalization.NumberStyles.HexNumber);
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
