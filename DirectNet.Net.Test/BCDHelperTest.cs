using DirectNet.Net.Helpers;
using Xunit;

namespace DirectNet.Net.Test;

public class BCDHelperTest
{
    [Fact]
    public void BcdHelper_244_Resding268()
    {
        Assert.Equal(244, BCDHelper.FromBCD(68, 2));
    }
}
