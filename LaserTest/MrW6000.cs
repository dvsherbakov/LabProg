using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace LaserTest
{
    internal class MrW6000 : LaserSerial
    {

        public override void Init()
        {

            try
            {
                if (Name.Length == 0)
                {
                    Name = "COM6";
                }

                Port = new SerialPort("COM16")
                {
                    BaudRate = 115200,
                    Parity = Parity.None,
                    StopBits = StopBits.One,
                    DataBits = 8,
                };
                Port.DataReceived += DataReceivedHandler;
                Port.Open();
            }

            catch (Exception ex)
            {
                OnSetLogMessage(ex.Message);
            }
        }

        public override void On()
        {
            var cmd = new byte[] { 0x5A, 0xA5, 0x06, 0x83, 0x00, 0xA0, 0x01, 0x00, 0x01 };
            Port.Write(cmd, 0, 9);

        }

        public override void Off()
        {
            var cmd = new byte[] { 0x5A, 0xA5, 0x06, 0x83, 0x00, 0xA0, 0x01, 0x00, 0x00 };
            Port.Write(cmd, 0, 9);
        }

        public override void SetPowerLevel(int percent)
        {
            var command = new List<byte> { 0x5A, 0xA5, 0x06, 0x83, 0x00, 0x20, 0x01 };
            var bts = BitConverter.GetBytes(percent);
            command.Add(bts[1]);
            command.Add(bts[0]);


            var cmd = command.ToArray();
            Port.Write(cmd, 0, 9);
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
                   OnSetLogMessage(ex.Message);
                }
                var ascii = Encoding.ASCII;
                var answers = ascii.GetString(TrimReceivedData(mXdata)).Split('\r');
            }
            catch (Exception ex)
            {
                OnSetLogMessage(ex.Message);
            }
        }

        public MrW6000(string name) : base(name)
        {
        }
    }
}
