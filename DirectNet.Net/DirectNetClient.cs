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
public class DirectNetClient : IDirectNetClient
{
    private readonly SerialPort _serialPort;
    private readonly ILogger<DirectNetClient>? _logger;

    /// <summary>
    /// Create a DirectNetClient
    /// </summary>
    /// <param name="portName">Port name, for com port 4, enter COM4</param>
    /// <param name="timeout">The timout of read and write operation in milisenconds</param>
    public DirectNetClient(
        string portName, int timeout = 1000, Handshake handshake = Handshake.None, 
        int baudRate = 9600, Parity parity = Parity.Odd, int dataBits = 8, StopBits stopBits = StopBits.One,
        ILogger<DirectNetClient>? logger = null) 
    {
        _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits)
        {
            ReadTimeout = timeout,
            WriteTimeout = timeout,
            Handshake = handshake
        };
        _logger = logger;
    }

    public bool IsOpen => _serialPort.IsOpen;

    public string PortName => _serialPort.PortName;

    public void Open() 
    {
        _logger?.LogInformation("Opening serial port {portName}", _serialPort.PortName);

        _serialPort.Open();
        
        if (!_serialPort.IsOpen) {
            throw new Exception("Could not open serial port");
        }

        _serialPort.DiscardInBuffer();
        _serialPort.DiscardOutBuffer();

        _logger?.LogInformation("Opening serial port {portName} now completed", _serialPort.PortName);
    }

    public void Close()
    {
        _logger?.LogInformation("Closing serial port {portName}", _serialPort.PortName);
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
    public async Task<byte[]> ReadAsync(string address, 
        int nbAddressRead = 1, int slaveAddress = 1, int masterAddress = 0,
        CancellationToken token = default)
    {
        _logger?.LogDebug("Begin ReadAsync on address {address}", address);

        await EnquiryAsync(slaveAddress, token);

        var header = HeaderHelper.GenerateHeader(OperationType.Read, address, nbAddressRead);

        _serialPort.Write(header);

        await ReadAckAsync(token);        

        byte[] response = await ParseData(nbAddressRead);

        WriteAck();

        await EndTransactionAsync(response, token);

        _logger?.LogDebug("End ReadAsync on address {address}", address);

        return response;
    }

    public async Task WriteAsync(string address, byte[] data, int slaveAddress = 1, int masterAddress = 0, CancellationToken token = default)
    {
        _logger?.LogDebug("Begin write on address {address}", address);

        await EnquiryAsync(slaveAddress, token);

        var header = HeaderHelper.GenerateHeader(OperationType.Write, address, data.Length);

        _serialPort.Write(header);

        await ReadAckAsync(token);

        var dataPaquet = PaquetHelper.GeneratePaquet(data);

        _serialPort.Write(dataPaquet);

        await ReadAckAsync(token);

        _serialPort.Write(ControlChar.EOT);

        _logger?.LogDebug("End write on address {address}", address);
    }

    public async Task EnquiryAsync(int slaveAddress, CancellationToken token = default)
    {
        _logger?.LogInformation("Begin Enquiry");

        var ackMessage = new byte[3] { 0x4E, (byte)(0x20 + slaveAddress), 0x05 };

        _serialPort.Write(ackMessage);

        var response = await _serialPort.ReadAsync(3, logger: _logger, token: token);

        if (!(response[0] == ackMessage[0] &&
            response[1] == ackMessage[1] &&
            response[2] == ackMessage[2] + 1))
        {
            throw new InvalidOperationException("Enquiry failed");
        }

        _logger?.LogInformation("End Enquiry");
    }

    private async Task<byte[]> ParseData(int size)
    {
        const int dataPacketMinSize = 3;

        var bytes = await _serialPort.ReadAsync(size + dataPacketMinSize,
            (byteRead, responseBuffer) => !(byteRead == 1 && responseBuffer[0] == ControlChar.NAK), _logger);

        return bytes.Skip(1).Take(size).ToArray();
    }

    private async Task ReadAckAsync(CancellationToken token)
    {
        var ack = await _serialPort.ReadAsync(1, logger: _logger, token: token);
        if (ack[0] == ControlChar.NAK)
        {
            throw new Exception("Error reading acknowledgement. Recieved NAK. The data command sent to the PLC was formatted incorrectly or the LRC was incorrect.");
        }
        if (ack[0] != ControlChar.ACK)
        {
            throw new Exception($"Recieved 0x{ack[0]:X}. Expected {ControlChar.ACK:X}");
        }
    }

    private void WriteAck() => _serialPort.Write(ControlChar.ACK);

    private async Task EndTransactionAsync(byte[] response, CancellationToken token)
    {
        var eot = await
            _serialPort.ReadAsync(1,
            (byteRead, responseBuffer) => !(byteRead == 1 && response[0] == ControlChar.NAK), logger: _logger, token: token);

        if (eot[0] != ControlChar.EOT)
        {
            throw new Exception($"Expected EOT(0x04) but got 0x{BitConverter.ToString(eot)[0]}");
        }

        _serialPort.Write(ControlChar.EOT);
    }

    public void Dispose()
    {
        _serialPort?.Dispose();
        GC.SuppressFinalize(this);
    }
}