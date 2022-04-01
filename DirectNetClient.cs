using System.IO.Ports;

/// <summary>
/// A implementation of a directnet client using a serial port.
/// </summary>
/// <remarks>
/// Ref: https://support.automationdirect.com/docs/an-misc-029.pdf
/// </remarks>
class DirectNetClient {
    private readonly SerialPort _serialPort;

    public DirectNetClient(SerialPort serialPort) {
        _serialPort = serialPort;
    }

    public void Open() {
        _serialPort.Open();

        if (!_serialPort.IsOpen) {
            throw new Exception("Could not open serial port");
        }
    }

    public void Enquiry() {
        _serialPort.Write(new byte[] { 0x4E, 21, 0x05}, 0, 3);
    }

    public void Close() {
        _serialPort.Close();
    }

    private byte[] GenerateHeader(OperationType operationType) 
    {
        var bytes = new List<byte>();

// SOH (Start of Header): Never Changes. Always use 0x01 for this field.
        bytes.Add(ControlChar.SOH);

/* Address Field without offset. Simple
Conversion: Take the decimal slave value.
Convert to Hex. (NOTE: DCM address
already in hex format) Example: Decimal 04=
Hex 0x04. Look at ASCII Table: 0=30 4=34.
Enter 3034 into the slave address field.
Another example: Decimal 60= Hex 0x3C.
3=33 C=43. Enter 3343 into slave address
field. */
        bytes.AddRange(new byte[] { 0x30, 0x34 });

// Read or Write Request Field: Enter 30 if Read or 38 if write.
        bytes.Add(operationType == OperationType.Read ? (byte)0x30 : (byte)0x38);

// Data Type Field: Refer to Appendixes D-F for the appropriate PLC mapping. Example: DL205 V-memory is 31.
        bytes.Add(0x31);

/* Starting Address Fields: Refer to Appendixes
D-F for the appropriate 4 digit PLC Reference
address to start read or writing at. The value
that is entered into these fields is a octal to
hex conversion plus an offset of 1. For
example: V40400=octal 40400 -> 0x4100 + 1
= 4101 Reference address. You then convert
this value with the ASCII table: 4=34 1=31
0=30 1=31 to get 3431 3031. */
        bytes.AddRange(new byte[] { 0x34, 0x31, 0x30, 0x31 });

/* Number of Complete Data Blocks: Anytime you need more than 256 bytes
of data, you would use this field. If you place a value of 1 into this field,
you will get 256 bytes of data. If you place a value of 2 into this field, you
will get 512 bytes of data (2 Data Blocks). Everytime you increment this
value, you get 256 more bytes of data. Once you determine how many
complete data blocks you want, you convert the number to hex and then
use the ASCII Table to convert to the value to enter into the field. */
        bytes.AddRange(new byte[] { 0x00, 0x01 });

        

        return bytes.ToArray();
    }
}