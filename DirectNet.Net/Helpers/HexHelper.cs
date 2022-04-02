namespace DirectNet.Net.Helpers;

public static class HexHelper
{
    /// <summary>
    /// Convert an integer to hex values ready to send in a DirectNet communication. 
    /// GetHex(10) => [0x31, 0x30]
    /// </summary>
    public static byte[] GetHex(int number, int arrayReturnMinSize = 1)
    {
        var str = number.ToString().PadLeft(arrayReturnMinSize, '0');

        var bytes = new byte[str.Length];

        for (int i = bytes.Length - 1; i >= 0; i--)
        {
            bytes[i] = (byte)(0x30 + byte.Parse(str[i].ToString(), System.Globalization.NumberStyles.HexNumber));
        }

        return bytes;
    }
}
