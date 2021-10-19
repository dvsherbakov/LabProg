using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserTest
{
    internal class PortModel
    {
        private SerialPort _port;

        public PortModel()
        {
            _port = new SerialPort("COM16")
            {
                BaudRate = 9600,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true
            };
            _port.DataReceived += DataReceivedHandler;
            _port.Open();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var sp = (SerialPort)sender;
            try
            {
                var cnt = sp.ReadBufferSize;
                var mXdata = new byte[cnt + 1];
                try
                {
                    sp.Read(mXdata, 0, cnt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                var ascii = Encoding.ASCII;
                var answers = ascii.GetString(mXdata).Split('\r');
                foreach (var s in answers)
                {
                    Console.WriteLine(s);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void On()
        {
            var cmd = new byte[8] { 0x53, 0x08, 0x06, 0x01, 0x00, 0x01, 0x63, 0x0D };
            _port.Write(cmd, 0, 8);
            _port.WriteLine("on");
        }

        public void Off()
        {
            _port.WriteLine("off");
        }

        public void SetPower()
        {
            var level = 25;
            var command = new List<byte> { 0x53, 0x08, 0x04, 0x01 };
            var bts = BitConverter.GetBytes(level);
            command.Add(bts[1]);
            command.Add(bts[0]);
            var sm = command.Aggregate<byte, short>(0, (current, x) => (short)(current + x));
            var cb = BitConverter.GetBytes(sm);
            command.Add(cb[0]);
            command.Add(0x0D);

            var cmd = command.ToArray();
            _port.Write(cmd, 0, 8);
            _port.WriteLine("65");
        }
    }
}
