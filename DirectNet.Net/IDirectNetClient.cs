namespace DirectNet.Net;

public interface IDirectNetClient : IDisposable
{
    bool IsOpen { get; }

    void Open();
    void Close();
    Task<byte[]> ReadAsync(string address, int nbAddressRead = 1, int slaveAddress = 1, int masterAddress = 0, CancellationToken token = default);
    Task WriteAsync(string address, byte[] data, int slaveAddress = 1, int masterAddress = 0, CancellationToken token = default);
    Task EnquiryAsync(int slaveAddress, CancellationToken token = default);
}
