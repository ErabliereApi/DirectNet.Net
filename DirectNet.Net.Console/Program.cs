using DirectNet.Net;
using DirectNet.Net.Extensions;
using DirectNet.Net.Helpers;
using Microsoft.Extensions.Logging;
using System.Text;

var logger = LoggerFactory.Create(builder =>
{
    builder.SetMinimumLevel(LogLevel.Warning)
           .AddConsole();
});

var directnet = new DirectNetClient("COM4", 5000, logger: logger.CreateLogger<DirectNetClient>());

try
{
    directnet.Open();

    var begin = OctalHelper.FromOctal("4000");

    begin += 1; // Add the offset

    var csv = new StringBuilder();

    csv.Append("Expected value");
    csv.Append(';');
    csv.Append("byte 1");
    csv.Append(';');
    csv.Append("byte 2");
    csv.Append(';');
    csv.Append("After conversion");

    for (int a = 0; a < 299; a++)
    {
        var response = await directnet.ReadAsync((begin + a).ToString("X"), 2);

        var value = BCDHelper.FromBCD(response[0], response[1]);

        csv.Append(a);
        csv.Append(';');
        csv.Append(response[0]);
        csv.Append(';');
        csv.Append(response[1]);
        csv.Append(';');
        csv.AppendLine(value.ToString());
    }

    File.WriteAllText("TestCase.csv", csv.ToString());
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex);
}
finally
{
    directnet.Close();
}