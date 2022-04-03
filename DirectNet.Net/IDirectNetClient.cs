namespace DirectNet.Net;

public interface IDirectNetClient
{
    bool IsOpen { get; }

    void Open();
    void Close();
    Task EnquiryAsync();
    Task<byte[]> ReadAsync(string address, int nbAddressRead = 1);
}
