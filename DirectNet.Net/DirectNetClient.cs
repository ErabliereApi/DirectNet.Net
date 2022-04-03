using System.IO.Ports;
using DirectNet.Net.Extensions;
using DirectNet.Net.Helpers;
using Microsoft.Extensions.Logging;

namespace DirectNet.Net;

/// <summary>
/// A implementation of a directnet client using a serial port.
/// </summary>
/// <remarks>
/// Ref: https://support.automationdirect.com/docs/an-misc-029.pdf
/// </remarks>
public class DirectNetClient : IDirectNetClient, IDisposable {
    private readonly SerialPort _serialPort;
    private readonly ILogger<DirectNetClient>? _logger;

    /// <summary>
    /// Create a DirectNetClient
    /// </summary>
    /// <param name="portName">Port name, for com port 4, enter COM4</param>
    /// <param name="timeout">The timout of read and write operation in seconds</param>
    public DirectNetClient(
        string portName, int timeout, Handshake handshake = Handshake.None, 
        ILogger<DirectNetClient>? logger = null) 
    {
        _serialPort = new SerialPort(portName, 9600, Parity.Odd, 8, StopBits.One)
        {
            ReadTimeout = timeout,
            WriteTimeout = timeout,
            Handshake = handshake
        };
        _logger = logger;
    }

    public void Open() {
        _logger?.LogInformation("Opening serial port {portName}", _serialPort.PortName);

        _serialPort.Open();
        
        if (!_serialPort.IsOpen) {
            throw new Exception("Could not open serial port");
        }

        _serialPort.DiscardInBuffer();
        _serialPort.DiscardOutBuffer();

        _logger?.LogInformation("Opening serial port {portName} now completed", _serialPort.PortName);
    }

    private static readonly byte[] AckMessage = new byte[3] { 0x4E, 0x21, 0x05 };

    public async Task EnquiryAsync()
    {
        _logger?.LogInformation("Begin Enquiry");

        _serialPort.Write(AckMessage, 0, 3);

        var response = await _serialPort.ReadAsync(3, logger: _logger);

        if (! (response[0] == AckMessage[0] &&
            response[1] == AckMessage[1] &&
            response[2] == AckMessage[2] + 1))
        {
            throw new InvalidOperationException("Enquiry failed");
        }

        _logger?.LogInformation("End Enquiry");
    }

    public void Close() 
    {
        _logger?.LogInformation("Closing serial port", _serialPort.PortName);
        _serialPort.Close();
        _logger?.LogInformation("Serial port {portName} now close", _serialPort.PortName);
    }

    /// <summary>
    /// Read some data begin at the address desired and the number of address to read
    /// </summary>
    /// <param name="address">The address in hex format. For address reference, see: https://cdn.automationdirect.com/static/manuals/dadnet/appxf.pdf </param>
    /// <param name="nbAddressRead"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<byte[]> ReadAsync(string address, int nbAddressRead = 1)
    {
        _logger?.LogDebug("Begin ReadAsync on address {address}", address);

        await EnquiryAsync();

        var header = HeaderHelper.GenerateHeader(OperationType.Read, address, nbAddressRead);

        _serialPort.Write(header, 0, header.Length);

        const int dataPacketMinSize = 4;

        var response = await _serialPort.ReadAsync(dataPacketMinSize + nbAddressRead,
            (byteRead, responseBuffer) => !(byteRead == 1 && responseBuffer[0] == ControlChar.NAK), _logger);

        if (response[0] == ControlChar.NAK)
        {
            throw new Exception("Recieved NAK (Negative Acknowledge - data received but there were errors)");
        }

        _serialPort.Write(new byte[] { ControlChar.ACK }, 0, 1);

        var eot = await 
            _serialPort.ReadAsync(1, 
            (byteRead, responseBuffer) => !(byteRead == 1 && response[0] == ControlChar.NAK), _logger);

        if (eot[0] != ControlChar.EOT)
        {
            throw new Exception($"Expected EOT(0x04) but got 0x{BitConverter.ToString(eot)[0]}");
        }

        _serialPort.Write(new byte[] { ControlChar.EOT }, 0, 1);

        return response;
    }

    public void Dispose()
    {
        _serialPort?.Dispose();
        GC.SuppressFinalize(this);
    }
}