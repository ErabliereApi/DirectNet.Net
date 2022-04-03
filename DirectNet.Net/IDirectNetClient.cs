namespace DirectNet.Net;

public interface IDirectNetClient
{
    void Open();
    void Close();
    Task EnquiryAsync();
    Task<byte[]> ReadAsync(string address, int nbAddressRead = 1);
}
