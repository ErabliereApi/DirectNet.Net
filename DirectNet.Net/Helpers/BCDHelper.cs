namespace DirectNet.Net.Helpers;

public class BCDHelper
{
    public static int FromBCD(byte b1, byte b2)
    {
        return FromBCD(b1) + (FromBCD(b2) * 100);
    }

    public static int FromBCD(byte b1)
    {
        var value = b1 & 0b1111;
        value += ((b1 & 0b11110000) >> 4) * 10;
        return value;
    }
}
