namespace DirectNet.Net;

public interface IDirectNetClient
{
    bool IsOpen { get; }

    void Open();
    void Close();
    Task<byte[]> ReadAsync(string address, int nbAddressRead = 1);
    Task WriteAsync(string address, byte[] data);
    Task EnquiryAsync();
}
