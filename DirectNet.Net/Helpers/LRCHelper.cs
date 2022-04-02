namespace DirectNet.Net;

public static class LRCHelper
{
    public static byte CalculateLRC(byte[] headerBytes, byte endOfBlock)
    {
        int[] totalOfOnes = new int[8];

        for (int i = 1; i < headerBytes.Length && headerBytes[i] != endOfBlock; i++)
        {
            var bytes = headerBytes[i];

            for (int b = 0; b < 8; b++)
            {
                totalOfOnes[b] += (bytes >> b) & 1;
            }
        }

        int half1 = 0x00;

        for (int i = 0; i < 8; i++)
        {
            half1 += (totalOfOnes[i] & 1) << i;
        }

        return (byte)half1;
    }
}
