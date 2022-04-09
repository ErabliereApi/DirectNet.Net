namespace DirectNet.Net.GUI;

public class ErabliereApiOptionsWithSensors : ErabliereAPI.Proxy.ErabliereApiOptions
{
    public Guid[] CapteursIds { get; set; } = Array.Empty<Guid>();

    public double SendIntervalInMinutes { get; set; }
}
