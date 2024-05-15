namespace CustomLogic.Common;

public class ErabliereApiOptionsWithSensors : ErabliereAPI.Proxy.ErabliereApiOptions
{
    public Guid ErabliereId { get; set; }

    public Guid[] CapteursIds { get; set; } = Array.Empty<Guid>();

    /// <summary>
    /// The interval set to send data to ErabliereAPI. When set to zero, no data is sent.
    /// </summary>
    public double SendIntervalInMinutes { get; set; }

    /// <summary>
    /// The number of seconds to wait for the next PLC scan. If set to zero (the default) 
    /// no wait will be made and the PLC will be scan whenever the previous scan is over.
    /// </summary>
    public double PLCScanFrequencyInSeconds { get; set; }
}
