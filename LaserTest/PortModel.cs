using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace LaserTest
{
    internal class PortModel
    {
        private readonly SerialPort _port;

        public PortModel()
        {
            _port = new SerialPort("COM16")
            {
                BaudRate = 115200,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
            };
            _port.DataReceived += DataReceivedHandler;
            _port.Open();
        }

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
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
            var cmd = new byte[] { 0x5A, 0xA5, 0x06, 0x83, 0x00, 0xA0, 0x01, 0x00, 0x01 };
            _port.Write(cmd, 0, 9);
            Console.WriteLine("Send: ", BitConverter.ToString(cmd));
        }

        public void Off()
        {
            var cmd = new byte[] { 0x5A, 0xA5, 0x06, 0x83, 0x00, 0xA0, 0x01, 0x00, 0x00 };
            _port.Write(cmd, 0, 9);
            Console.WriteLine("Send: ", BitConverter.ToString(cmd));
        }

        public void SetPower(byte percent)
        {

            var command = new List<byte> { 0x5A, 0xA5, 0x06, 0x83, 0x00, 0x20, 0x01 };
            var bts = BitConverter.GetBytes(percent);
            command.Add(bts[1]);
            command.Add(bts[0]);


            var cmd = command.ToArray();
            _port.Write(cmd, 0, 9);
            Console.WriteLine($"Send power: {BitConverter.ToString(cmd)}, Length: {command.Count}");
        }
    }
}
