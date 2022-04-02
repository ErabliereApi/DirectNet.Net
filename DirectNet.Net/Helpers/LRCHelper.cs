namespace DirectNet.Net;

public static class LRCHelper
{
    public static byte[] CalculateLRC(byte[] headerBytes, byte endOfBlock)
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

        var half1 = 0x00;
        var half2 = 0x00;

        for (int i = 0; i < 4; i++)
        {
            half1 += (totalOfOnes[i] & 1) << i;
        }

        if (half1 < 10)
        {
            half1 += 0x30;
        }

        for (int i = 4; i < 8; i++)
        {
            half2 += (totalOfOnes[i] & 1) << (i - 4);
        }

        if (half2 < 10)
        {
            half2 += 0x30;
        }

        return new byte[] { (byte)half2, (byte)half1 };
    }
}
