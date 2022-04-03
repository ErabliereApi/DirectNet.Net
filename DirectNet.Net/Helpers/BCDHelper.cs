namespace DirectNet.Net.Helpers;

public static class BCDHelper
{
    public static int FromBCD(byte b1, byte b2)
    {
        var value = FromInternalBCD(b1);
        value += FromInternalBCD(b2) * 100;

        return value;
    }

    private static int FromInternalBCD(byte b1)
    {
        if (b1 > 16)
        {
            return b1;
        }

        int value = b1 & 0b1111;

        if (value == 0 && b1 > 0)
        {
            value += b1;
        }
        else
        {
            value += ((b1 & 0b11110000) >> 4) * 10;
        }

        return value;
    }
}
