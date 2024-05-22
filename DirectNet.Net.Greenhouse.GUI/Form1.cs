using CustomLogic.Common;
using ErabliereAPI.Proxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IO.Ports;

namespace DirectNet.Net.Greenhouse.GUI;

public partial class Form1 : Form
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<Form1> _logger;
    private readonly IOptions<ErabliereApiOptionsWithSensors> _options;
    private IDirectNetClient? _client;
    private CancellationTokenSource? _cst;
    private Task? _task;

    public Form1(IServiceProvider provider)
    {
        _provider = provider;
        _options = provider.GetRequiredService<IOptions<ErabliereApiOptionsWithSensors>>();
        _logger = provider.GetRequiredService<ILogger<Form1>>();
        InitializeComponent();
        ChooseComPortAndLaunchTask();
    }

    private void ChooseComPortAndLaunchTask()
    {
        try
        {
            string[] ports = SetPortListCombobox();
            if (ports.Length > 0)
            {
                toolStripComboBox1.SelectedIndex = 0;
                toolStripComboBox1.SelectedIndexChanged += ToolStripComboBox1_SelectedIndexChanged;
                RunAndManageBackgroundTask(ports[0]);
            }
            else
            {
                RunAndManageBackgroundTask("");
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
        }
    }

    private string[] SetPortListCombobox()
    {
        toolStripComboBox1.Items.Clear();
        var ports = SerialPort.GetPortNames();
        toolStripComboBox1.Items.AddRange(ports);
        return ports;
    }

    private void RunAndManageBackgroundTask(string port)
    {
        CleanupBackgroudTask();

        _cst = new CancellationTokenSource();
        if (!string.IsNullOrWhiteSpace(port))
        {
            _client = new DirectNetClient(port);
        }
        var token = _cst.Token;
        _task = Task.Run(async () =>
        {
            var chrono = new Stopwatch();

            int exceptionInRow = 0;

            var api = _provider.GetRequiredService<IErabliereAPIProxy>();

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var lastHour = DateTimeOffset.UtcNow - TimeSpan.FromHours(1);

                    var erablieres = await api.ErablieresAllAsync(
                        select: null,
                        filter: $"id eq {_options.Value.ErabliereId}",
                        top: null,
                        skip: null,
                        expand: $"capteurs($expand=donneescapteur($filter=d gt {lastHour:yyyy-MM-ddTHH:mm:ss.FFFZ};$top=1;$orderby=d desc))",
                        orderby: null);

                    var erabliere = erablieres.First();

                    Invoke(() =>
                    {
                        groupBox1.Text = erabliere.Nom;
                        groupBox2.Text = "PLC DL06";

                        groupBox15.Text = "Valve 1";
                        groupBox16.Text = "Valve 2";

                        groupBox14.Hide();
                        label12.Hide();

                        button1.Text = "Ouvrir";
                        button2.Text = "Fermer";
                        button3.Text = "Ouvrir";
                        button4.Text = "Fermer";

                        if (_client?.IsOpen == true)
                        {

                        }
                        else
                        {
                            label13.Text = "Non connecté";
                            label14.Text = "Non connecté";
                        }

                        for (var i = 0; i < _options.Value.CapteursIds.Length; i++)
                        {
                            var capteur = erabliere.Capteurs?.FirstOrDefault(c => c.Id == _options.Value.CapteursIds[i]);

                            if (capteur != null)
                            {
                                switch (i)
                                {
                                    case 0:
                                        groupBox3.Text = capteur.Nom;
                                        label1.Text = FormatLabelText(capteur);
                                        break;
                                    case 1:
                                        groupBox4.Text = capteur.Nom;
                                        label2.Text = FormatLabelText(capteur);
                                        break;
                                    case 2:
                                        groupBox5.Text = capteur.Nom;
                                        label3.Text = FormatLabelText(capteur);
                                        break;
                                    case 3:
                                        groupBox6.Text = capteur.Nom;
                                        label4.Text = FormatLabelText(capteur);
                                        break;
                                    case 4:
                                        groupBox7.Text = capteur.Nom;
                                        label5.Text = FormatLabelText(capteur);
                                        break;
                                    case 5:
                                        groupBox8.Text = capteur.Nom;
                                        label6.Text = FormatLabelText(capteur);
                                        break;
                                    case 6:
                                        groupBox9.Text = capteur.Nom;
                                        label7.Text = FormatLabelText(capteur);
                                        break;
                                    case 7:
                                        groupBox10.Text = capteur.Nom;
                                        label8.Text = FormatLabelText(capteur);
                                        break;
                                    case 8:
                                        groupBox11.Text = capteur.Nom;
                                        label9.Text = FormatLabelText(capteur);
                                        break;
                                    case 9:
                                        groupBox12.Text = capteur.Nom;
                                        label10.Text = FormatLabelText(capteur);
                                        break;
                                    case 10:
                                        groupBox13.Text = capteur.Nom;
                                        label11.Text = FormatLabelText(capteur);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    });

                    await Task.Delay(TimeSpan.FromSeconds(_options.Value.PLCScanFrequencyInSeconds), token);

                    exceptionInRow = 0;
                }
                catch (Exception e)
                {
                    exceptionInRow++;

                    if (exceptionInRow >= 3)
                    {
                        try
                        {
                            _client?.Close();
                        }
                        catch (Exception ec)
                        {
                            _logger.LogError(ec, "Error trying to close the serial client after 3 exceptions");
                        }
                    }

                    _logger.LogError(e, "Error in the background task main loop");

                    try
                    {
                        Invoke(() =>
                        {
                            if (e is TimeoutException)
                            {
                                toolStripStatusLabelError.Text = "Error: Timout reading com port.";
                            }
                            else
                            {
                                toolStripStatusLabelError.Text = e.Message;
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error when settings error message in the UI.");
                    }

                    if (!token.IsCancellationRequested)
                    {
                        await Task.Delay(1000, token);
                    }
                }
            }
        }, token);
    }

    private string FormatLabelText(Capteur capteur)
    {
        var data = capteur.DonneesCapteur?.SingleOrDefault();

        if (data == null)
        {
            return "n/a";
        }

        return $"{data.Valeur / 10} {capteur.Symbole}";
    }

    private async Task OpenSerialClient(CancellationToken token)
    {
        if (_client == null)
        {
            throw new InvalidOperationException("Client must not be null to open the serial connection");
        }

        if (!_client.IsOpen)
        {
            _client.Open();
            Invoke(() =>
            {
                toolStripStatusLabel2.Text = $"State: Open";
                toolStripStatusLabel3.Text = $"PortName: {_client.PortName}";
            });
            try
            {
                await _client.EnquiryAsync(1, token);
                Invoke(() =>
                {
                    toolStripStatusLabel4.Text = $"Enquery: Succeded";
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when checking Enquiry");
                Invoke(() =>
                {
                    toolStripStatusLabel4.Text = $"Enquery: Failed";
                });
            }
        }
    }

    public DateTime _lastSend;

    private async ValueTask UpdateErabliereAPI(int[] values, CancellationToken token)
    {
        var options = _provider.GetRequiredService<IOptions<ErabliereApiOptionsWithSensors>>().Value;

        if (options.SendIntervalInMinutes <= 0)
        {
            Invoke(() =>
            {
                toolStripStatusLabel5.Text = "ErabliereAPI: Disabled";
            });
        }
        else if (DateTime.Now - _lastSend > TimeSpan.FromMinutes(options.SendIntervalInMinutes))
        {
            Invoke(() =>
            {
                toolStripStatusLabel5.Text = $"ErabliereAPI: Sending datas {DateTime.Now}";
            });

            await ErabliereApiTasks.Send24ValuesAsync(_provider, values, token);

            _lastSend = DateTime.Now;
        }
    }

    private void ToolStripComboBox1_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (sender is ToolStripComboBox comboBox)
        {
            RunAndManageBackgroundTask(comboBox.SelectedItem?.ToString() ?? "");
        }
        else
        {
            throw new NotImplementedException("Event is not implemented when the sender is not a combobox.");
        }
    }

    private void GroupBox1_Enter(object sender, EventArgs e)
    {

    }

    private void ToolStripStatusLabel1_Click(object sender, EventArgs e)
    {

    }

    private void ToolStripStatusLabel2_Click(object sender, EventArgs e)
    {

    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        _logger.LogInformation(nameof(Form1_FormClosing));
        if (_cst != null)
        {
            _logger.LogInformation("CancellationTokenSource.Cancel");
            _cst.Cancel();
            _logger.LogInformation("Form1.CleanupBackgroupTask");
            CleanupBackgroudTask();
        }
    }

    private void CleanupBackgroudTask()
    {
        if (_cst != null)
        {
            try
            {
                _cst.Cancel();
                _cst.Dispose();
                if (_task != null)
                {
                    if (_task.IsCompleted == false)
                    {
                        _task.Wait(1000);
                    }

                    try
                    {
                        _task.Dispose();
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error when diposing background task. State was {taskStatus}. Exceptions: {taskExceptions}", _task.Status, _task.Exception);
                    }
                    finally
                    {
                        _task = null;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in the task cancellation process");
            }
        }

        if (_client != null)
        {
            _logger.LogInformation("Closing com port {portName}", _client.PortName);
            _client.Close();
            toolStripStatusLabel2.Text = $"State: Close";
            toolStripStatusLabel3.Text = "";
            toolStripStatusLabel4.Text = $"Enquery: n/a";
            _logger.LogInformation("Disposing serial client");
            _client.Dispose();
        }
    }

    private void ToolStripResetDriverButton_Click(object sender, EventArgs e)
    {
        CleanupBackgroudTask();

        ChooseComPortAndLaunchTask();
    }

    private void OuvrirValve1(object sender, EventArgs e)
    {
        if (_client?.IsOpen == true)
        {
            if (_client?.IsOpen == true)
            {
                _client.WriteAsync("V4000", new byte[] { 0b1 });
            }
        }
    }

    private void FermerVavle1(object sender, EventArgs e)
    {
        if (_client?.IsOpen == true)
        {
            _client.WriteAsync("V4000", new byte[] { 0b0 });
        }
    }

    private void OuvrirValve2(object sender, EventArgs e)
    {
        if (_client?.IsOpen == true)
        {
            _client.WriteAsync("V4002", new byte[] { 0b1 });
        }
    }

    private void FermerValve2(object sender, EventArgs e)
    {
        if (_client?.IsOpen == true)
        {
            _client.WriteAsync("V4002", new byte[] { 0b0 });
        }
    }
}
