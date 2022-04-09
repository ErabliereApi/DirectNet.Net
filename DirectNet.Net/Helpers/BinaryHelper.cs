namespace DirectNet.Net.Helpers;

public static class BinaryHelper
{
    public static int FromBinary(byte b1, byte b2)
    {
        return b1 + (b2 << 8);
    }
}
