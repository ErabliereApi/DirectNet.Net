using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO.Ports;
namespace DirectNet.Net.Extensions;

public static class SerialClientExtension
{
    public static async Task<byte[]> ReadAsync(this SerialPort serialPort, 
        int nbByteToRead, Func<int, byte[], bool>? additionnalPredicat = null, ILogger? logger = null)
    {
        logger?.LogDebug("Begin ReadAsync. NbBytesRead: {nbByteToRead}", nbByteToRead);
        var watch = logger?.IsEnabled(LogLevel.Debug) == true ? Stopwatch.StartNew() : null;

        var bytes = new byte[nbByteToRead];

        var nbBytesRead = 0;

        using var tokenSource = new CancellationTokenSource();

        tokenSource.CancelAfter(serialPort.ReadTimeout);

        while (nbBytesRead < nbByteToRead && (additionnalPredicat == null || additionnalPredicat.Invoke(nbBytesRead, bytes)))
        {
            nbBytesRead += 
                await serialPort.BaseStream.ReadAsync(bytes.AsMemory(nbBytesRead, nbByteToRead - nbBytesRead), tokenSource.Token);
        }

        if (nbBytesRead != nbByteToRead)
        {
            throw new InvalidOperationException($"Expected to read {nbByteToRead} bytes but got {nbBytesRead}");
        }

        logger?.LogDebug(
            "End ReadAsync. NbBytesRead: {nbByteToRead}. Duration: {ElapsedMilliseconds}ms", nbByteToRead, watch?.ElapsedMilliseconds);

        return bytes;
    }

    public static void Write(this SerialPort serialPort, byte[] bytes)
    {
        serialPort.Write(bytes, 0, bytes.Length);
    }

    public static void Write(this SerialPort serialPort, byte c)
    {
        serialPort.Write(new byte[] { c });
    }
}
