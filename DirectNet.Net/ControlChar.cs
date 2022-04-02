namespace DirectNet.Net;

/// <summary>
/// A class containing some constante related to serial communication.
/// </summary>
/// <remarks>
/// These are standard ASCII Control characters and are not unique to the DirectNET protocol.
/// </remarks>
public static class ControlChar {

    /// <summary>
    /// Enquiry to start communications
    /// </summary>
    public const byte ENQ = 0x05;

    /// <summary>
    /// Acknowledge (data received and no errors)
    /// </summary>
    public const byte ACK = 0x06;

    /// <summary>
    /// Negative Acknowledge (data received but there were errors)
    /// </summary>
    public const byte NAK = 0x15;

    /// <summary>
    /// Start of Header
    /// </summary>
    public const byte SOH = 0x01;

    /// <summary>
    /// End of Transmission Block (intermediate block)
    /// </summary>
    public const byte ETB = 0x17;

    /// <summary>
    /// Start of Text (beginning of data block)
    /// </summary>
    public const byte STX = 0x02;

    /// <summary>
    /// End of Text (end of last data block)
    /// </summary>
    public const byte ETX = 0x03;

    /// <summary>
    /// End of Transmission (transaction complete)
    /// </summary>
    public const byte EOT = 0x04;
}