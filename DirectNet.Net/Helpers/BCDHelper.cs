namespace DirectNet.Net.Helpers;

public static class BCDHelper
{
    public static int FromBCD(byte b1, byte b2)
    {
        return b1 + (b2 << 8);
    }
}
