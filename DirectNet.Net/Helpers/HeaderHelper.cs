using System.Text;

namespace DirectNet.Net.Helpers;

public static class HeaderHelper
{
    public static Dictionary<string, byte> MemoryTypes = new()
    {
        { "V", 0x31 }
    };

    public static byte[] GenerateHeader(OperationType operationType, string address, int nbAddressRead = 1, int slaveAddress = 1, int masterAddress = 0)
    {
        var headerBytes = new byte[17];

        // SOH (Start of Header): Never Changes. Always use 0x01 for this field.
        headerBytes[0] = ControlChar.SOH;

        /* Address Field without offset. Simple
        Conversion: Take the decimal slave value.
        Convert to Hex. (NOTE: DCM address
        already in hex format) Example: Decimal 04=
        Hex 0x04. Look at ASCII Table: 0=30 4=34.
        Enter 3034 into the slave address field.
        Another example: Decimal 60= Hex 0x3C.
        3=33 C=43. Enter 3343 into slave address
        field. */
        // Byte 1+2
        SetSlaveAddressField(headerBytes, slaveAddress);

        // Read or Write Request Field: Enter 30 if Read or 38 if write.
        // Byte 3
        headerBytes[3] = (byte)operationType;

        // Data Type Field: Refer to Appendixes D-F for the appropriate PLC mapping. Example: DL205 V-memory is 31.
        // Byte 4
        headerBytes[4] = GetAddressType(address);

        /* Starting Address Fields: Refer to Appendixes
        D-F for the appropriate 4 digit PLC Reference
        address to start read or writing at. The value
        that is entered into these fields is a octal to
        hex conversion plus an offset of 1. For
        example: V40400=octal 40400 -> 0x4100 + 1
        = 4101 Reference address. You then convert
        this value with the ASCII table: 4=34 1=31
        0=30 1=31 to get 3431 3031. */
        // Byte 5+6+7+8
        SetAddressField(address, headerBytes);

        /* Number of Complete Data Blocks: Anytime you need more than 256 bytes
        of data, you would use this field. If you place a value of 1 into this field,
        you will get 256 bytes of data. If you place a value of 2 into this field, you
        will get 512 bytes of data (2 Data Blocks). Everytime you increment this
        value, you get 256 more bytes of data. Once you determine how many
        complete data blocks you want, you convert the number to hex and then
        use the ASCII Table to convert to the value to enter into the field. */
        // Byte 9+10
        SetCompleteBlockField(nbAddressRead, headerBytes);

        /* Partial Data Blocks: You use this field anytime
        you want less than a complete block of data (<256
        bytes). You do the same conversion as before.
        For example, if you want 72 V memory locations
        (144 bytes), you convert decimal 144= Hex 0x90.
        Then do the ASCII table look up. 9=39 0=30. You
        enter 3930 into the partial data block field.
        Remember in ASCII mode, you have to request 4
        bytes per desired V memory location. So to get
        50 V memory locations (200 bytes), you convert
        200= Hex 0xC8. Then do the ASCII table look up.
        C=43 8=38. You enter 4348 into the partial data
        block field. */
        // Byte 11+12
        SetPartialBlockField(nbAddressRead, headerBytes);

        /* Master ID: This field holds the value of the
        Master ID number. This will almost always be
        either 3030 for 0 or 3031 for 1. If you wanted a
        different value, you do the same conversions
        as before.*/
        // Byte 13+14
        SetMasterAddressField(headerBytes, masterAddress);

        /* End of Transmission: This field always holds
        the value of 0x17 since there is only one
        header for any given transaction in DirectNET.*/
        // Byte 15
        headerBytes[15] = ControlChar.ETB;

        /* LRC Checksum: This field holds the
        checksum. The value in the upper field is the
        DirectNET Hex representation and the value in
        the lower field is the DirectNET ASCII
        representation. Remember that only bytes 2 –
        15 are calculated in the LRC (represented in
        red here). Refer to the next slide for a simple
        chart method of calculating the LRC.*/
        // Byte 16 (+18)
        headerBytes[16] = LRCHelper.CalculateLRC(headerBytes, ControlChar.ETB);

        return headerBytes;
    }

    private static byte GetAddressType(string address)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < address.Length; i++)
        {
            if (char.IsLetter(address[i]))
            {
                sb.Append(char.ToUpper(address[i]));
            }
            else if (sb.Length > 0)
            {
                break;
            }
        }

        return MemoryTypes[sb.ToString()];
    }

    private static void SetAddressField(string address, byte[] headerBytes)
    {
        var addressNumber = new StringBuilder();

        for (int i = 0; i < address.Length; i++)
        {
            if (char.IsDigit(address[i]))
            {
                addressNumber.Append(address[i]);
            }
        }

        const int offset = 1;

        var hexAddr = (OctalHelper.FromOctal(addressNumber.ToString()) + offset).ToString("X");
        var hex = hexAddr.ToString().PadLeft(4, '0');

        headerBytes[5] = HexHelper.PrepareForHeader(hex[0]);
        headerBytes[6] = HexHelper.PrepareForHeader(hex[1]);
        headerBytes[7] = HexHelper.PrepareForHeader(hex[2]);
        headerBytes[8] = HexHelper.PrepareForHeader(hex[3]);
    }

    private static void SetCompleteBlockField(int nbAddressRead, byte[] headerBytes)
    {
        if (nbAddressRead >= 256)
        {
            var nbBlock = nbAddressRead / 256;

            if (nbAddressRead != nbBlock * 256)
            {
                nbBlock++;
            }

            var hex = HexHelper.GetDirectNetHex(nbBlock, 2);

            headerBytes[9] = hex[0];
            headerBytes[10] = hex[1];
        }
        else
        {
            headerBytes[9] = 0x30;
            headerBytes[10] = 0x30;
        }
    }

    private static void SetPartialBlockField(int nbAddressRead, byte[] headerBytes)
    {
        if (nbAddressRead < 256)
        {
            var hex = HexHelper.GetDirectNetHex(nbAddressRead, 2);

            headerBytes[11] = hex[0];
            headerBytes[12] = hex[1];
        }
        else
        {
            headerBytes[11] = 0x30;
            headerBytes[12] = 0x30;
        }
    }

    private static void SetMasterAddressField(byte[] headerBytes, int masterAddress)
    {
        var address = HexHelper.GetDirectNetHex(masterAddress, 2);

        headerBytes[13] = address[0];
        headerBytes[14] = address[1];
    }

    private static void SetSlaveAddressField(byte[] headerBytes, int slaveAddress)
    {
        var address = HexHelper.GetDirectNetHex(slaveAddress, 2);

        headerBytes[1] = address[0];
        headerBytes[2] = address[1];
    }
}
