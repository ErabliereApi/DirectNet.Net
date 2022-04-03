using DirectNet.Net.Extensions;
using DirectNet.Net.Helpers;

namespace DirectNet.Net.GUI;

public partial class Form1 : Form
{
    private IDirectNetClient _client;
    private Task _task;

    public Form1()
    {
        InitializeComponent();
        _client = new DirectNetClient("COM4");
        _task = Task.Run(async () =>
        {
            while (true)
            {
                try
                {
                    if (!_client.IsOpen)
                    {
                        _client.Open();
                    }

                    var begin = OctalHelper.FromOctal("4000");

                    begin += 1; // Add the offset

                    for (int a = 0; a < 24; a++)
                    {
                        var value = await _client.ReadVMemoryLocationAsync((begin + a).ToString("X"));

                        switch (a)
                        {
                            case 0:
                                Invoke(() =>
                                {
                                    label1.Text = value.ToString();
                                });
                                break;
                            case 1:
                                Invoke(() =>
                                {
                                    label2.Text = value.ToString();
                                });
                                break;
                            case 2:
                                Invoke(() =>
                                {
                                    label3.Text = value.ToString();
                                });
                                break;
                            case 3:
                                Invoke(() =>
                                {
                                    label4.Text = value.ToString();
                                });
                                break;
                            case 4:
                                Invoke(() =>
                                {
                                    label5.Text = value.ToString();
                                });
                                break;
                            case 5:
                                Invoke(() =>
                                {
                                    label6.Text = value.ToString();
                                });
                                break;
                            case 6:
                                Invoke(() =>
                                {
                                    label7.Text = value.ToString();
                                });
                                break;
                            case 7:
                                Invoke(() =>
                                {
                                    label8.Text = value.ToString();
                                });
                                break;
                            case 8:
                                Invoke(() =>
                                {
                                    label9.Text = value.ToString();
                                });
                                break;
                            case 9:
                                Invoke(() =>
                                {
                                    label10.Text = value.ToString();
                                });
                                break;
                            case 10:
                                Invoke(() =>
                                {
                                    label11.Text = value.ToString();
                                });
                                break;
                            case 11:
                                Invoke(() =>
                                {
                                    label12.Text = value.ToString();
                                });
                                break;
                            case 12:
                                Invoke(() =>
                                {
                                    label13.Text = value.ToString();
                                });
                                break;
                            case 13:
                                Invoke(() =>
                                {
                                    label14.Text = value.ToString();
                                });
                                break;
                            case 14:
                                Invoke(() =>
                                {
                                    label15.Text = value.ToString();
                                });
                                break;
                            case 15:
                                Invoke(() =>
                                {
                                    label16.Text = value.ToString();
                                });
                                break;
                            case 16:
                                Invoke(() =>
                                {
                                    label17.Text = value.ToString();
                                });
                                break;
                            case 17:
                                Invoke(() =>
                                {
                                    label18.Text = value.ToString();
                                });
                                break;
                            case 18:
                                Invoke(() =>
                                {
                                    label19.Text = value.ToString();
                                });
                                break;
                            case 19:
                                Invoke(() =>
                                {
                                    label20.Text = value.ToString();
                                });
                                break;
                            case 20:
                                Invoke(() =>
                                {
                                    label21.Text = value.ToString();
                                });
                                break;
                            case 21:
                                Invoke(() =>
                                {
                                    label22.Text = value.ToString();
                                });
                                break;
                            case 22:
                                Invoke(() =>
                                {
                                    label23.Text = value.ToString();
                                });
                                break;
                            case 23:
                                Invoke(() =>
                                {
                                    label24.Text = value.ToString();
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
}
