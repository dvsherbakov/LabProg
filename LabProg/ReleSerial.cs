using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace LabProg
{
    class ReleSerial
    {
        private static SerialPort Port;

        public ReleSerial(string port)
        {
            if (port == "") port = "COM4";
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
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
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
