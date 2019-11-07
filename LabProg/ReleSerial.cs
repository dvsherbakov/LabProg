using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Threading;

namespace LabProg
{
    class ReleSerial
    {
        private static SerialPort Port;

        public ReleSerial(string port)
        {
            if (port == "") port = "COM8";
            Port = new SerialPort(port)
            {
                BaudRate = 9600,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true
            };
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var sp = (SerialPort)sender;
            try
            {
                var cnt = sp.ReadBufferSize;
                var _rxdata = new byte[cnt + 1];
                var rc = sp.Read(_rxdata, 0, cnt);
                var ascii = Encoding.ASCII;
                var answrs = ascii.GetString(_rxdata).Split('\r');
                foreach (var anw in answrs)
                {
                   // Dispatcher.Invoke(() =>  LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Выключен порт пирометра" }); anw));
                }
            }
            catch (Exception ex)
            {
               // Dispatcher.Invoke(() => lbText.Items.Add(ex.Message));
            }
        }

        public void SendReleOn()
        {
            byte[] buf = { 0x48 };
            Port.Write(buf, 0, buf.Length);
        }

        public void SendBufOff()
        {
            byte[] buf = { 0x4C };
            Port.Write(buf, 0, buf.Length);
        }
    }
}
