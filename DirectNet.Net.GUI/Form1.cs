using DirectNet.Net.Extensions;
using System.Diagnostics;
using System.IO.Ports;

namespace DirectNet.Net.GUI;

public partial class Form1 : Form
{
    private IDirectNetClient _client;
    private Task _task;

    public Form1()
    {
        InitializeComponent();
        var ports = SerialPort.GetPortNames();
        toolStripComboBox1.Items.AddRange(ports);
        if (ports.Length > 0)
        {
            toolStripComboBox1.SelectedIndex = 0;
        }
        _client = new DirectNetClient(ports.FirstOrDefault() ?? "COM4");
        _task = Task.Run(async () =>
        {
            var chrono = new Stopwatch();

            while (true)
            {
                try
                {
                    if (!_client.IsOpen)
                    {
                        _client.Open();
                    }

                    chrono.Start();

                    var values = await _client.ReadVMemoryLocationsAsync("V4000", 24);

                    chrono.Stop();

                    toolStripStatusLabel1.Text = $"Scan time: {chrono.ElapsedMilliseconds}ms";

                    chrono.Reset();

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
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);

                    await Task.Delay(1000);
                }
            }
        });
    }

    private void groupBox1_Enter(object sender, EventArgs e)
    {

    }

    private void toolStripStatusLabel1_Click(object sender, EventArgs e)
    {

    }
}
