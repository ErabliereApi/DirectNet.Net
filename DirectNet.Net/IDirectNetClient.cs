namespace DirectNet.Net;

public interface IDirectNetClient
{
    bool IsOpen { get; }

    void Open();
    void Close();
    Task<byte[]> ReadAsync(string address, int nbAddressRead = 1, int slaveAddress = 1, int masterAddress = 0);
    Task WriteAsync(string address, byte[] data, int slaveAddress = 1, int masterAddress = 0);
    Task EnquiryAsync(int slaveAddress);
}
