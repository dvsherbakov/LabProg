using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;

namespace LabProg
{
    public class PumpSerial
    {
        private readonly SerialPort _mPort;
        private bool _active;
        private readonly List<string> RecievedData;
        private bool f_direction;
        public bool IsDriven { get; set; }

        public PumpSerial(string portStr, bool startDirection)
        {
            RecievedData = new List<string>();
            if (portStr == "") portStr = "COM7";
            _mPort = new SerialPort(portStr)
            {
                BaudRate = int.Parse("9600"),
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true
            };
            _mPort.DataReceived += DataReceivedHandler;
        }

        public void OpenPort()
        {
            _mPort.Open();
            _active = true;
            IsDriven = false;
            if (f_direction) SetClockwiseDirection(); else SetCounterClockwiseDirection();  
        }

        public void ClosePort()
        {
            _mPort.Close();
            _active = false;
            StopPump();
        }

        public bool Active()
        {
            return _active;
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            System.Threading.Thread.Sleep(20);
            var cnt = _mPort.ReadBufferSize;
            var mRxdata = new byte[cnt + 1];
            try
            {
                _mPort.Read(mRxdata, 0, cnt);
            }
            catch
            {
                // ignored
            }
            var ascii = Encoding.ASCII;
            RecievedData.Add(ascii.GetString(mRxdata));
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

        
        public void StartPump()
        {
            if (!IsDriven) _mPort.Write("s");
            System.Threading.Thread.Sleep(20);
            IsDriven = true;
        }

        public void StopPump()
        {
            if (IsDriven) _mPort.Write("t");
            System.Threading.Thread.Sleep(20);
            IsDriven = false;
        }

        public void SetClockwiseDirection()
        {
            _mPort.Write("r");
            System.Threading.Thread.Sleep(20);
        }

        public void SetCounterClockwiseDirection()
        {
            _mPort.Write("l");
            System.Threading.Thread.Sleep(20);
        }

        public void Reverce()
        {
            f_direction = !f_direction;
            if (f_direction) SetClockwiseDirection(); else SetCounterClockwiseDirection();
            System.Threading.Thread.Sleep(20);
        }

        public void SetSpeed(string speed)
        {
            _mPort.Write(speed);
            System.Threading.Thread.Sleep(20);
        }
    }
}