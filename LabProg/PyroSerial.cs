using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Timers;

namespace LabProg.Resources
{
    class PyroSerial
    {
        private static SerialPort _port;
        private byte[] _rxdata;
        private long _rxidx;
        private static readonly List<string> ErrList = new List<string>();
        private readonly Dictionary<long, float> _tempLog = new Dictionary<long, float>();
        private readonly Timer _aTimer = new Timer();

        public PyroSerial(string port)
        {
            if (port == "") port = "COM5";
            _rxidx = 0;
            _port = new System.IO.Ports.SerialPort(port)
            {
                BaudRate = 115200,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true
            };
            _port.DataReceived += DataReceivedHandler;
           
            _aTimer.Elapsed += OnTimedEvent;
            _aTimer.Interval = 300;
            
        }

        public void OpenPort()
        {
            _port.Open();
            _aTimer.Enabled = true;
        }

        public void ClosePort()
        {
            _aTimer.Enabled = false;
            _port.Close();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var sp = (SerialPort) sender;
            try
            {
                var cnt = sp.ReadBufferSize;
                _rxdata = new byte[cnt + 1];
               sp.Read(_rxdata, 0, cnt);
            }
            catch (Exception ex)
            {
                ErrList.Add(ex.Message);
            }
            //float t = ;
            _tempLog.Add(_rxidx, RcConvert(_rxdata));
            _rxidx++;
        }

        private static float RcConvert(byte[] rData)
        {
            var tmp = new byte[4];

            tmp[0] = 0;
            tmp[1] = 0;
            tmp[2] = rData[0];
            tmp[3] = rData[1];

            float res = tmp[0] << 24 | tmp[1] << 16 | tmp[2] << 8 | tmp[3];

            return (res - 1000) / 10;
        }

        private static void Write(byte[] buf)
        {
            try
            {
                _port.Write(buf, 0, buf.Length);
            }
            catch (Exception ex)
            {
                ErrList.Add(ex.Message);
            }
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            byte[] buf = {01};
            Write(buf);
        }

        public float GetLastRes()
        {
            _tempLog.TryGetValue(_rxidx - 1, out float val);
            return val;
        }
    }
}
