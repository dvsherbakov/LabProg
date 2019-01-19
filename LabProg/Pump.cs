using System;
using System.IO.Ports;
using System.Text;

namespace LabProg
{
    public class Pump
    {
        private readonly SerialPort _mPort;

        public Pump(string portStr)
        {
            _mPort = new SerialPort(portStr)
            {
                BaudRate = int.Parse("9600"),
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true
            };
            _mPort.Open();
        }

        public string ReadPortData()
        {
            var cnt = _mPort.ReadBufferSize;
            var mRxdata = new byte[cnt + 1];
            try
            {
                _mPort.Read(mRxdata, 0, cnt);
            }
            catch (Exception)
            {
                // ignored
            }

            var ascii = Encoding.ASCII;
            return ascii.GetString(mRxdata);
        }

        public void ClosePort()
        {
            _mPort.Close();
        }

        public void StartPump()
        {
            _mPort.Write("s");
            //Thread.Sleep(100); 
        }

        public void StopPump()
        {
            _mPort.Write("t");
            //Thread.Sleep(100);
        }

        public void SetClockwiseDirection()
        {
            _mPort.Write("r");
            //Thread.Sleep(100);
        }

        public void SetCounterClockwiseDirection()
        {
            _mPort.Write("l");
            //Thread.Sleep(100);
        }

        public void SetSpeed(string speed)
        {
            //ReadPortData();
            _mPort.Write(speed);
            //Thread.Sleep(100);
        }
    }
}