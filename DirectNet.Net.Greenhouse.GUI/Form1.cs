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
    private const int readLength = 8;

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
                toolStripComboBox1.SelectedIndex = ports.ToList().IndexOf("COM4");
                if (toolStripComboBox1.SelectedIndex == -1)
                {
                    toolStripComboBox1.SelectedIndex = 0;
                }
                toolStripComboBox1.SelectedIndexChanged += ToolStripComboBox1_SelectedIndexChanged;
                RunAndManageBackgroundTask(ports[toolStripComboBox1.SelectedIndex]);
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
        var ports = SerialPort.GetPortNames().OrderBy(p => p).ToArray();
        _logger.LogInformation("Available ports: {ports}", ports.Aggregate((a, b) => $"{a}, {b}"));
        toolStripComboBox1.Items.AddRange(ports);
        return ports;
    }

    private void RunAndManageBackgroundTask(string port)
    {
        _logger.LogInformation("RunAndManageBackgroundTask with port {port}", port);
        CleanupBackgroudTask();
        _logger.LogInformation("CleanupBackgroudTask done. Creating new CancellationTokenSource");

        _cst = new CancellationTokenSource();
        var token = _cst.Token;
        _task = Task.Run(async () =>
        {
            _logger.LogInformation("Task.Run started");

            var chrono = new Stopwatch();

            if (_client?.IsOpen == false || _client == null)
            {
                if (_client == null)
                {
                    _logger.LogInformation("Creating new DirectNetClient with port {port}", port);
                    _client = new DirectNetClient(port);
                }
                await OpenSerialClient(token);
            }

            int exceptionInRow = 0;

            var api = _provider.GetRequiredService<IErabliereAPIProxy>();

            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (_client == null)
                    {
                        throw new InvalidOperationException("Client must not be null to run the background task");
                    }

                    var values = await _client.ReadAsync("V4000", readLength, token: token);

                    var lastHour = DateTimeOffset.UtcNow - TimeSpan.FromHours(1);

                    var erablieres = await api.ErablieresAllAsync(
                        my: null,
                        select: null,
                        filter: $"id eq {_options.Value.ErabliereId}",
                        top: null,
                        skip: null,
                        expand: $"capteurs($expand=donneescapteur($filter=d gt {lastHour:yyyy-MM-ddTHH:mm:ss.FFFZ};$top=1;$orderby=d desc))",
                        orderby: null,
                        cancellationToken: token);

                    var erabliere = erablieres.First();

                    if (erabliere.Id == null)
                    {
                        throw new InvalidOperationException("Erabliere Id must not be null");
                    }

                    var precipitations = await api.HourlyAsync(erabliere.Id.Value, "fr-CA", cancellationToken: token);
                    
                    var decisionContext = new DecisionContext();

                    Invoke(() =>
                    {
                        groupBox1.Text = erabliere.Nom;
                        groupBox2.Text = "PLC DL06";

                        groupBox15.Text = "Valve 1";
                        groupBox16.Text = "Valve 2";

                        groupBox14.Text = "Précipitation 12h";
                        label12.Text = $"{precipitations.Sum(f => f.HasPrecipitation == true ? 1 : 0)} heures";
                        decisionContext.PrecepitationNext12h = precipitations.Sum(f => f.HasPrecipitation == true ? 1 : 0);

                        button1.Text = "Ouvrir";
                        button2.Text = "Fermer";
                        button3.Text = "Ouvrir";
                        button4.Text = "Fermer";

                        UpdatePLCUI(values);

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
                                        decisionContext.Temperature = capteur.DonneesCapteur?.FirstOrDefault()?.Valeur;
                                        break;
                                    case 1:
                                        groupBox4.Text = capteur.Nom;
                                        label2.Text = FormatLabelText(capteur);
                                        decisionContext.Humidity = capteur.DonneesCapteur?.FirstOrDefault()?.Valeur;
                                        break;
                                    case 2:
                                        groupBox5.Text = capteur.Nom;
                                        label3.Text = FormatLabelText(capteur);
                                        decisionContext.PressionAtmopherique = capteur.DonneesCapteur?.FirstOrDefault()?.Valeur;
                                        break;
                                    case 3:
                                        groupBox6.Text = capteur.Nom;
                                        label4.Text = FormatLabelText(capteur);
                                        decisionContext.DewPoint = capteur.DonneesCapteur?.FirstOrDefault()?.Valeur;
                                        break;
                                    case 4:
                                        groupBox7.Text = capteur.Nom;
                                        label5.Text = FormatLabelText(capteur);
                                        decisionContext.WindDirection = capteur.DonneesCapteur?.FirstOrDefault()?.Valeur;
                                        break;
                                    case 5:
                                        groupBox8.Text = capteur.Nom;
                                        label6.Text = FormatLabelText(capteur);
                                        decisionContext.WindSpeed = capteur.DonneesCapteur?.FirstOrDefault()?.Valeur;
                                        break;
                                    case 6:
                                        groupBox9.Text = capteur.Nom;
                                        label7.Text = FormatLabelText(capteur);
                                        decisionContext.Precipitations = capteur.DonneesCapteur?.FirstOrDefault()?.Valeur;
                                        break;
                                    case 7:
                                        groupBox10.Text = capteur.Nom;
                                        label8.Text = FormatLabelText(capteur);
                                        decisionContext.RadiationSolair = capteur.DonneesCapteur?.FirstOrDefault()?.Valeur;
                                        break;
                                    case 8:
                                        groupBox11.Text = capteur.Nom;
                                        label9.Text = FormatLabelText(capteur);
                                        decisionContext.HumiditeSolArriere = capteur.DonneesCapteur?.FirstOrDefault()?.Valeur;
                                        break;
                                    case 9:
                                        groupBox12.Text = capteur.Nom;
                                        label10.Text = FormatLabelText(capteur);
                                        decisionContext.HumiditeSolSerre = capteur.DonneesCapteur?.FirstOrDefault()?.Valeur;
                                        break;
                                    case 10:
                                        groupBox13.Text = capteur.Nom;
                                        label11.Text = FormatLabelText(capteur);
                                        decisionContext.TemperatureSerre = capteur.DonneesCapteur?.FirstOrDefault()?.Valeur;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                        toolStripStatusLabel5.Text = $"Last update: {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss}";
                    });

                    if (decisionContext.HumiditeSolSerre < 450) {
                        await _client.WriteAsync("V4000", [0b0, 0b1], token: _cst?.Token ?? default);
                    } else {
                        await _client.WriteAsync("V4000", [0b0, 0b0], token: _cst?.Token ?? default);
                    }

                    if (decisionContext.HumiditeSolArriere < 450) {
                        await _client.WriteAsync("V4002", [0b0, 0b1], token: _cst?.Token ?? default);
                    } else {
                        await _client.WriteAsync("V4002", [0b0, 0b0], token: _cst?.Token ?? default);
                    }

                    values = await _client.ReadAsync("V4000", readLength, token: token);

                    UpdatePLCUI(values);

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

    private void UpdatePLCUI(byte[] values)
    {
        _logger.LogInformation("UpdatePLCUI with values {values}", values.Select(v => v.ToString()).Aggregate((a, b) => $"{a}, {b}"));

        if (_client?.IsOpen == true && values != null)
        {
            label13.Text = values[0] == 1 ? "Ouverte" : "Fermé";
            label14.Text = values[4] == 1 ? "Ouverte" : "Fermé";
        }
        else
        {
            label13.Text = "Non connecté";
            label14.Text = "Non connecté";
        }
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
                if (!_cst.IsCancellationRequested)
                {
                    _logger.LogInformation("CancellationTokenSource.Cancel");
                    _cst.Cancel();
                }
                _cst.Dispose();
                if (_task != null)
                {
                    if (_task.IsCompleted == false)
                    {
                        _task.Wait(1000);
                    }

                    try
                    {
                        if (_task.Status == TaskStatus.WaitingForActivation)
                        {
                            _task.Start();
                        }
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
            try
            {
                _logger.LogInformation("Closing com port {portName}", _client.PortName);
                _client.Close();
                toolStripStatusLabel2.Text = $"State: Close";
                toolStripStatusLabel3.Text = "";
                toolStripStatusLabel4.Text = $"Enquery: n/a";
                _logger.LogInformation("Disposing serial client");
                _client.Dispose();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when closing the serial client");
            }
            finally
            {
                _client = null;
            }
        }
    }

    private void ToolStripResetDriverButton_Click(object sender, EventArgs e)
    {
        CleanupBackgroudTask();

        ChooseComPortAndLaunchTask();
    }

    private async void OuvrirValve1(object sender, EventArgs e)
    {
        try
        {
            if (_client?.IsOpen == true)
            {
                if (_client?.IsOpen == true)
                {
                    await _client.WriteAsync("V4000", [0b0, 0b1], token: _cst?.Token ?? default);

                    var values = await _client.ReadAsync("V4000", readLength, token: _cst?.Token ?? default);

                    Invoke(() =>
                    {
                        UpdatePLCUI(values);
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when opening valve 1");
        }
    }

    private async void FermerVavle1(object sender, EventArgs e)
    {
        try
        {
            if (_client?.IsOpen == true)
            {
                await _client.WriteAsync("V4000", [0b0, 0b0], token: _cst?.Token ?? default);

                var values = await _client.ReadAsync("V4000", readLength, token: _cst?.Token ?? default);

                Invoke(() =>
                {
                    UpdatePLCUI(values);
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when closing valve 1");
        }
    }

    private async void OuvrirValve2(object sender, EventArgs e)
    {
        try {
            if (_client?.IsOpen == true)
            {
                await _client.WriteAsync("V4002",[0b0, 0b1], token: _cst?.Token ?? default);

                var values = await _client.ReadAsync("V4000", readLength, token: _cst?.Token ?? default);

                Invoke(() =>
                {
                    UpdatePLCUI(values);
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when opening valve 2");
        }
    }

    private async void FermerValve2(object sender, EventArgs e)
    {
        try {
            if (_client?.IsOpen == true)
            {
                await _client.WriteAsync("V4002", [0b0, 0b0], token: _cst?.Token ?? default);

                var values = await _client.ReadAsync("V4000", readLength, token: _cst?.Token ?? default);

                Invoke(() =>
                {
                    UpdatePLCUI(values);
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error when closing valve 2");
        }
    }
}

internal class DecisionContext
{
    public DecisionContext()
    {
    }

    public int? Temperature { get; internal set; }
    public int? Humidity { get; internal set; }
    public int? PressionAtmopherique { get; internal set; }
    public int? DewPoint { get; internal set; }
    public int? WindDirection { get; internal set; }
    public int? WindSpeed { get; internal set; }
    public int? Precipitations { get; internal set; }
    public int? RadiationSolair { get; internal set; }
    public int? HumiditeSolArriere { get; internal set; }
    public int? HumiditeSolSerre { get; internal set; }
    public int? TemperatureSerre { get; internal set; }
    public int PrecepitationNext12h { get; internal set; }
}