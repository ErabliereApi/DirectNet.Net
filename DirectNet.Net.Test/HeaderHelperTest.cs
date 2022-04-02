using DirectNet.Net.Helpers;
using Xunit;

namespace DirectNet.Net.Test;

public class HeaderHelperTest
{
    [Fact]
    public void GenerateHeaderTest()
    {
        var header = HeaderHelper.GenerateHeader(OperationType.Read, "v4000", nbAddressRead: 1, slaveAddress: 1, masterAddress: 0);

        Assert.Equal(17, header.Length);

        // The byte array start with the control character SOH
        Assert.Equal(ControlChar.SOH, header[0]);

        // The slave address is valid base on the parameter that was given
        Assert.Equal(0x30, header[1]);
        Assert.Equal(0x31, header[2]);

        // Assert the operation type is read
        Assert.Equal(0x30, header[3]);

        // Assert the data type is the V memory
        Assert.Equal(0x31, header[4]);

        // Assert the starting address field
        Assert.Equal(0x34, header[5]);
        Assert.Equal(0x30, header[6]);
        Assert.Equal(0x30, header[7]);
        Assert.Equal(0x30, header[8]);

        // Assert number of complete block field
        Assert.Equal(0x30, header[9]);
        Assert.Equal(0x30, header[10]);

        // Assert partial number of complete block field
        Assert.Equal(0x30, header[11]);
        Assert.Equal(0x31, header[12]);

        // Assert the master id
        Assert.Equal(0x30, header[13]);
        Assert.Equal(0x30, header[14]);

        // Assert the end of transaction
        Assert.Equal(ControlChar.ETB, header[15]);
    }
}
