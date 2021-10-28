using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace LabControl.PortModels
{
    internal class MrlIii660D : LaserSerial
    {
        public MrlIii660D(string name) : base(name)
        {
        }

        public override void Init()
        {
            try
            {
                if (Name.Length == 0)
                {
                    Name = "COM6";
                }

                Port = new SerialPort(Name)
                {
                    BaudRate = 9600,
                    Parity = Parity.None,
                    StopBits = StopBits.One,
                    DataBits = 8,
                    Handshake = Handshake.None,
                    RtsEnable = true
                };

                Port.DataReceived += DataReceivedHandler;
            }
            catch (Exception ex)
            {
                OnSetLogMessage(ex.Message);
            }
        }

        public override void On()
        {
            if (!Port.IsOpen) return;
            var cmd = new byte[] { 0x53, 0x08, 0x06, 0x01, 0x00, 0x01, 0x63, 0x0D };
            Port.Write(cmd, 0, 8);
        }

        public override void Off()
        {
            if (!Port.IsOpen) return;
            var cmd = new byte[] { 0x53, 0x08, 0x06, 0x01, 0x00, 0x02, 0x64, 0x0D };
            Port.Write(cmd, 0, 8);
        }

        public override void SetPowerLevel(int pwr)
        {
            var command = new List<byte> { 0x53, 0x08, 0x04, 0x01 };
            var bts = BitConverter.GetBytes(pwr);
            command.Add(bts[1]);
            command.Add(bts[0]);
            var sm = command.Aggregate<byte, short>(0, (current, x) => (short)(current + x));
            var cb = BitConverter.GetBytes(sm);
            command.Add(cb[0]);
            command.Add(0x0D);

            var cmd = command.ToArray();
            Port.Write(cmd, 0, 8);
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
    }
}
