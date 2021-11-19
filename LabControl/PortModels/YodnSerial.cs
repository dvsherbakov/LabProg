using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace LabControl.PortModels
{
    internal class YodnSerial
    {
        private string _portName;
        private SerialPort _port;

        public void Init(string portName = "COM17")
        {
            _portName = portName;
            _port = new SerialPort(_portName)
            {
                BaudRate = 9600,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One,
                Handshake = Handshake.None,
                ReadBufferSize = 2048,
                WriteBufferSize = 2048,
                ReceivedBytesThreshold = 1,
                ReadTimeout = 1000,
                WriteTimeout = 1000,
                Encoding = Encoding.GetEncoding("iso-8859-1")
            };
            _port.DataReceived += DataReceivedHandler;
            _port.Open();
            _port.DiscardInBuffer();
            _port.DiscardOutBuffer();
            _port.Write("p");
            Thread.Sleep(50);

            _port.Write("P");
            Thread.Sleep(50);
            _port.Write("Q");
            Thread.Sleep(50);
            _port.Write("R");
            Thread.Sleep(50);

            _port.Write(new byte[] { 0x53, 0x01 }, 0, 2);
            Thread.Sleep(50);
            _port.Write(new byte[] { 0x53, 0x02 }, 0, 2);
            Thread.Sleep(50);
            _port.Write(new byte[] { 0x53, 0x03 }, 0, 2);
            Thread.Sleep(50);
            _port.Write(new byte[] { 0x55, 0x01 }, 0, 2);
            Thread.Sleep(50);
            _port.Write(new byte[] { 0x55, 0x02 }, 0, 2);
            Thread.Sleep(50);
            _port.Write(new byte[] { 0x55, 0x03 }, 0, 2);
            Thread.Sleep(50);
            _port.Write(new byte[] { 0x57, 0x00 }, 0, 2);
            Thread.Sleep(50);
            Off();
        }

        public void Close()
        {
            Off();
            _port.DiscardInBuffer();
            _port.DiscardOutBuffer();
            _port.Close();
        }

        public void SetUvChannel(int value)
        {
            if (_port.IsOpen)
                _port.Write(new byte[] { 0x61, 0x01, (byte)value }, 0, 3); //Set led 1
        }

        public void SetBlueChannel(int value)
        {
            if (_port.IsOpen)
                _port.Write(new byte[] { 0x61, 0x02, (byte)value }, 0, 3); //Set led 2
        }

        public void SetGreenRedChannel(int value)
        {
            if (_port.IsOpen)
                _port.Write(new byte[] { 0x61, 0x03, (byte)value }, 0, 3); //Set Led 3
        }

        public void On()
        {
            if (!_port.IsOpen) return;
            _port.Write("p");
            Thread.Sleep(50);
            _port.Write(new byte[] { 0x60, 0x00, 0x01 }, 0, 3); //Lamp On
        }

        public void Off()
        {
            if (_port.IsOpen)
                _port.Write(new byte[] { 60, 00, 00 }, 0, 3); //Lamp Off

        }

        public void SendR()
        {
            if (_port.IsOpen)
                _port.Write("R");
        }

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var sp = (SerialPort)sender;
            try
            {
                var inputData = sp.ReadExisting();
                Debug.Write("Data Received:");
                Debug.WriteLine(inputData);
            }
            catch (Exception ex)
            {
                Debug.Write("catch");
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
