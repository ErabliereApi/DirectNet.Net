namespace DirectNet.Net.Helpers;

public static class PaquetHelper
{
    public static byte[] GeneratePaquet(byte[] data)
    {
        var paquet = new byte[data.Length + 3];

        paquet[0] = ControlChar.STX;
        
        for (int i = 0; i < data.Length; i++)
        {
            paquet[i + 1] = data[i];
        }

        paquet[^2] = ControlChar.ETX;

        paquet[^1] = LRCHelper.CalculateLRC(paquet, ControlChar.ETX);

        return paquet;
    }
}
