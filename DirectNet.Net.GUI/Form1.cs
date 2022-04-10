using DirectNet.Net.Extensions;
using DirectNet.Net.Static;
using ErabliereAPI.Proxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IO.Ports;

namespace DirectNet.Net.GUI;

public partial class Form1 : Form
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<Form1> _logger;
    private IDirectNetClient? _client;
    private CancellationTokenSource? _cst;
    private Task? _task;

    public Form1(IServiceProvider provider)
    {
        _provider = provider;
        _logger = provider.GetRequiredService<ILogger<Form1>>();
        InitializeComponent();
        var ports = SerialPort.GetPortNames();
        toolStripComboBox1.Items.AddRange(ports);
        if (ports.Length > 0)
        {
            toolStripComboBox1.SelectedIndex = 0;
            toolStripComboBox1.SelectedIndexChanged += ToolStripComboBox1_SelectedIndexChanged;
            RunAndManageBackgroundTask(ports[0]);
        }
    }

    private void RunAndManageBackgroundTask(string port)
    {
        if (_cst != null)
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
                    _logger.LogError(e, "Error when diposing background task");
                }
                finally
                {
                    _task = null;
                }
            }
        }

        if (_client != null)
        {
            _client.Close();
            toolStripStatusLabel2.Text = $"State: Close";
            toolStripStatusLabel3.Text = "";
            toolStripStatusLabel4.Text = $"Enquery: n/a";
            _client.Dispose();
        }

        _cst = new CancellationTokenSource();
        _client = new DirectNetClient(port);
        var token = _cst.Token;
        _task = Task.Run(async () =>
        {
            var chrono = new Stopwatch();

            while (!token.IsCancellationRequested)
            {
                try
                {
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

                    chrono.Start();

                    var values = await _client.ReadVMemoryLocationsAsync("V4000", 24, format: FormatType.BCD, token: token);

                    chrono.Stop();

                    Invoke(() =>
                    {
                        toolStripStatusLabel1.Text = $"Scan time: {chrono.ElapsedMilliseconds}ms";
                    });

                    chrono.Reset();

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
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error in the background task main loop");

                    if (!token.IsCancellationRequested)
                    {
                        await Task.Delay(1000);
                    }
                }
            }
        }, token);
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

            using var scope = _provider.CreateScope();

            var httpClientfactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

            var httpClient = httpClientfactory.CreateClient("ErabliereAPI");

            var erabliereApi = new ErabliereApiProxy(options.BaseUrl, httpClient);

            for (int i = 0; i < values.Length; i++)
            {
                await erabliereApi.DonneesCapteurPOSTAsync(options.CapteursIds[i], new PostDonneeCapteur
                {
                    IdCapteur = options.CapteursIds[i],
                    V = values[i]
                }, token);
            }

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

    private void groupBox1_Enter(object sender, EventArgs e)
    {

    }

    private void toolStripStatusLabel1_Click(object sender, EventArgs e)
    {

    }

    private void toolStripStatusLabel2_Click(object sender, EventArgs e)
    {

    }
}
