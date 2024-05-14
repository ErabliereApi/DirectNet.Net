using CustomLogic.Common;
using DirectNet.Net.Extensions;
using DirectNet.Net.Static;
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
        string[] ports = SetPortListCombobox();
        if (ports.Length > 0)
        {
            toolStripComboBox1.SelectedIndex = 0;
            toolStripComboBox1.SelectedIndexChanged += ToolStripComboBox1_SelectedIndexChanged;
            RunAndManageBackgroundTask(ports[0]);
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
        _client = new DirectNetClient(port);
        var token = _cst.Token;
        _task = Task.Run(async () =>
        {
            var chrono = new Stopwatch();

            int exceptionInRow = 0;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    await OpenSerialClient(token);

                    chrono.Reset();
                    chrono.Start();

                    var values = await _client.ReadVMemoryLocationsAsync("V4000", 24, format: FormatType.BCD, token: token);

                    var msSerialRead = chrono.ElapsedMilliseconds;

                    if (!string.IsNullOrWhiteSpace(toolStripStatusLabelError.Text))
                    {
                        toolStripStatusLabelError.Text = "";
                    }

                    Invoke(() =>
                    {
                        toolStripStatusLabel1.Text = $"Scan time: {msSerialRead}ms";
                    });

                    UpdateUI(values);

                    try
                    {
                        await UpdateErabliereAPI(values, token);
                        if (toolStripStatusLabel5.Text != "ErabliereAPI: Disabled")
                        {
                            Invoke(() =>
                            {
                                toolStripStatusLabel5.Text = $"ErabliereAPI: Last send {_lastSend}";
                            });
                        }
                    }
                    catch (Exception ez)
                    {
                        _logger.LogError(ez, "Error updating ErabliereAPI");
                        Invoke(() =>
                        {
                            toolStripStatusLabel5.Text = $"ErabliereAPI: {ez.Message.ReplaceLineEndings(" ")}";
                        });
                    }

                    chrono.Stop();

                    if (_options.Value.PLCScanFrequencyInSeconds > 0)
                    {
                        var delay = (int)((_options.Value.PLCScanFrequencyInSeconds * 1000) - chrono.ElapsedMilliseconds);

                        if (delay > 0)
                        {
                            await Task.Delay(delay, token);
                        }
                    }

                    exceptionInRow = 0;
                }
                catch (Exception e)
                {
                    exceptionInRow++;

                    if (exceptionInRow >= 3)
                    {
                        try
                        {
                            _client.Close();
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

    private void UpdateUI(int[] values)
    {
        for (int a = 0; a < values.Length; a++)
        {
            var value = values[a] / 10.0;

            switch (a)
            {
                case 0:
                    Invoke(() =>
                    {
                        label_main1.Text = value.ToString();
                    });
                    break;
                case 1:
                    Invoke(() =>
                    {
                        label_main2.Text = value.ToString();
                    });
                    break;
                case 2:
                    Invoke(() =>
                    {
                        label_main3.Text = value.ToString();
                    });
                    break;
                case 3:
                    Invoke(() =>
                    {
                        label_main4.Text = value.ToString();
                    });
                    break;
                case 4:
                    Invoke(() =>
                    {
                        label_main5.Text = value.ToString();
                    });
                    break;
                case 5:
                    Invoke(() =>
                    {
                        label_main6.Text = value.ToString();
                    });
                    break;
                case 6:
                    Invoke(() =>
                    {
                        label_main7.Text = value.ToString();
                    });
                    break;
                case 7:
                    Invoke(() =>
                    {
                        label_main8.Text = value.ToString();
                    });
                    break;
                case 8:
                    Invoke(() =>
                    {
                        label_main9.Text = value.ToString();
                    });
                    break;
                case 9:
                    Invoke(() =>
                    {
                        label_main10.Text = value.ToString();
                    });
                    break;
                case 10:
                    Invoke(() =>
                    {
                        label_main11.Text = value.ToString();
                    });
                    break;
                case 11:
                    Invoke(() =>
                    {
                        label_main12.Text = value.ToString();
                    });
                    break;
                case 12:
                    Invoke(() =>
                    {
                        label_main13.Text = value.ToString();
                    });
                    break;
                case 13:
                    Invoke(() =>
                    {
                        label_main14.Text = value.ToString();
                    });
                    break;
                case 14:
                    Invoke(() =>
                    {
                        label_main15.Text = value.ToString();
                    });
                    break;
                case 15:
                    Invoke(() =>
                    {
                        label_main16.Text = value.ToString();
                    });
                    break;
                case 16:
                    Invoke(() =>
                    {
                        label_main17.Text = value.ToString();
                    });
                    break;
                case 17:
                    Invoke(() =>
                    {
                        label_main18.Text = value.ToString();
                    });
                    break;
                case 18:
                    Invoke(() =>
                    {
                        label_main19.Text = value.ToString();
                    });
                    break;
                case 19:
                    Invoke(() =>
                    {
                        label_main20.Text = value.ToString();
                    });
                    break;
                case 20:
                    Invoke(() =>
                    {
                        label_main21.Text = value.ToString();
                    });
                    break;
                case 21:
                    Invoke(() =>
                    {
                        label_main22.Text = value.ToString();
                    });
                    break;
                case 22:
                    Invoke(() =>
                    {
                        label_main23.Text = value.ToString();
                    });
                    break;
                case 23:
                    Invoke(() =>
                    {
                        label_main24.Text = value.ToString();
                    });
                    break;
                default:
                    break;
            }
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
}
