using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace LabProg
{
    public class PumpSerial
    {
        private readonly SerialPort _mPort;
        private bool _active;
        private readonly List<string> RecievedData;

        public PumpSerial(string portStr)
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
            _mPort.Write("s");
        }

        public void StopPump()
        {
            _mPort.Write("t");
        }

        public void SetClockwiseDirection()
        {
            _mPort.Write("r");
        }

        public void SetCounterClockwiseDirection()
        {
            _mPort.Write("l");
        }

        public void SetSpeed(string speed)
        {
            _mPort.Write(speed);
        }
    }
}