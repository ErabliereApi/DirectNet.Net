// This program connect to a PLC throut the COM4 port and read V memory location enter by a user.

// Create the Serial client
var serialPort = new System.IO.Ports.SerialPort("COM4", 9600, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
var directnet = new DirectNetClient(serialPort);

directnet.Open();

directnet.Enquiry();

Console.WriteLine("Enqury completed");

directnet.Close();