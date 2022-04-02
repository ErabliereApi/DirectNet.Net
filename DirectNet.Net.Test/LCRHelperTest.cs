using Xunit;
using static DirectNet.Net.ControlChar;

namespace DirectNet.Net.Test;

public class DirectNetTest
{
    /// <summary>
    /// Test the CalculateLRC method base on the example in the documentation
    /// </summary>
    [Fact]
    public void CalculateLRC_Case1()
    {
        var headerBytes = new byte[] 
        {
            SOH, 0x30, 0x34, 0x31, 0x30, 0x34, 0x31, 0x30, 0x31, 0x30, 0x31, 0x39, 0x30, 0x30, 0x31, ETB, 0x00, 0x00 
        };
        Assert.Equal(18, headerBytes.Length);

        var lrc = LRCHelper.CalculateLRC(headerBytes, ETB);

        Assert.Equal(0x30, lrc[0]);
        Assert.Equal(0x38, lrc[1]);
    }

    /// <summary>
    /// Test the CalculateLRC method base on the example in the documentation
    /// </summary>
    [Fact]
    public void CalculateLRC_Case2()
    {
        var headerBytes = new byte[]
        {
            SOH, 0x30, 0x31, 0x30, 0x31, 0x30, 0x38, 0x30, 0x31, 0x30, 0x30, 0x30, 0x34, 0x30, 0x30, ETB, 0x00
        };
        Assert.Equal(17, headerBytes.Length);

        var lrc = LRCHelper.CalculateLRC(headerBytes, ETB);

        Assert.Equal(0x30, lrc[0]);
        Assert.Equal(0x0D, lrc[1]);
    }
}