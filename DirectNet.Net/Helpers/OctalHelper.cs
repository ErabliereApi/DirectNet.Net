namespace DirectNet.Net.Helpers;

public static class OctalHelper
{
    public static int FromOctal(string octalNumber)
    {
        return Convert.ToInt32(octalNumber, 8);
    }
}
