using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace MicrodropDesktop
{
    internal class PortController
    {
        private readonly SerialPort _port;
        private byte[] RxData { get; set; }

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        public PortController(string portName)
        {
            if (string.IsNullOrEmpty(portName)) portName = "COM16";
            _port = new SerialPort(portName)
            {
                BaudRate = 19200,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true
            };
            _port.DataReceived += DataReceivedHandler;
        }


        public void OpenPort()
        {
            _port?.Open();
        }

        public void ClosePort()
        {
            _port?.Close();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var sp = (SerialPort)sender;
            var ascii = Encoding.ASCII;
            try
            {
                var cnt = sp.ReadBufferSize;
                RxData = new byte[cnt + 1];
                var rc = sp.Read(RxData, 0, cnt);
                RxData = TrimReceivedData(RxData);
                SetLogMessage?.Invoke(
                    $"{ascii.GetString(RxData)}");
            }
            catch (Exception ex)
            {
                SetLogMessage?.Invoke(ex.Message);
                //ErrList.Add(ex.Message);
            }
        }

        private static byte[] TrimReceivedData(IEnumerable<byte> src)
        {
            var res = new List<byte>(src);
            while (res.LastOrDefault() == 0)
            {
                res.RemoveAt(res.Count - 1);
            }
            return res.ToArray();
        }

        public void SendRequest()
        {
            if (!_port.IsOpen) return;
            _port.Write("\r");
        }

        public void GetFrequency(int head)
        {
            if (!_port.IsOpen) return;
            _port.Write($"GetFrequency(${head})\r");
        }

        public void SetFrequency(int head, int freq)
        {
            if (!_port.IsOpen) return;
            _port.Write($"SetFrequency(${head}, ${freq})\r");
        }

        public void GetStrobe(int head)
        {
            if (!_port.IsOpen) return;
            _port.Write($"GetStrobe(${head})\r");
        }

        public void GetPulseLength(int head, int pulse)
        {
            if (!_port.IsOpen) return;
            _port.Write($"GetPulseLength(${head}, ${pulse})\r");
        }

        public void SetPulseLength(int head, int pulse, int len)
        {
            if (!_port.IsOpen) return;
            _port.Write($"SetPulseLength(${head}, ${pulse}, ${len})\r");
        }

        public void GetPulseDelay(int head, int pulse)
        {
            if (!_port.IsOpen) return;
            _port.Write($"GetPulseDelay(${head}, ${pulse})\r");
        }

        public void SetPulseDelay(int head, int pulse, int len)
        {
            if (!_port.IsOpen) return;
            _port.Write($"SetPulseDelay(${head}, ${pulse}, ${len})\r");
        }

        public void GetPulseVoltage(int head, int pulse)
        {
            if (!_port.IsOpen) return;
            _port.Write($"GetPulseVoltage(${head}, ${pulse})\r");
        }

        public void SetPulseVoltage(int head, int pulse, int value)
        {
            if (!_port.IsOpen) return;
            _port.Write($"SetPulseVoltage(${head}, ${pulse}, ${value})\r");
        }

        public void SetActive(int head, int on)
        {

            if (!_port.IsOpen) return;
            _port.Write($"SetActive(${head}, ${on})\r");
        }

    }
}
