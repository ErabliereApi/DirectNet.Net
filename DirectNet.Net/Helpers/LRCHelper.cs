namespace DirectNet.Net;

public static class LRCHelper
{
    public static byte CalculateLRC(byte[] bytes, byte endOfBlock)
    {
        var v = 0;

        for (int i = 1; bytes[i] != endOfBlock && i < bytes.Length; i++)
        {
            v ^= bytes[i];
        }

        return (byte) v;
    }
}
