using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Timers;

namespace LabControl.PortModels
{
    internal class PyroSerial
    {
        private static SerialPort _port;
        private byte[] _fRxData;
        private long _fRxIdx;
        private readonly Dictionary<long, float> _fTempLog = new Dictionary<long, float>();
        private readonly System.Timers.Timer _fATimer = new System.Timers.Timer();

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        public delegate void PyroEventHandler(float temperature);
        public event PyroEventHandler EventHandler;

        public PyroSerial(string port)
        {
            
            if (string.IsNullOrEmpty(port)) port = "COM4";
            _fRxIdx = 0;
            _port = new SerialPort(port)
            {
                BaudRate = 57600,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true
            };
            _port.DataReceived += DataReceivedHandler;
            _fATimer.Elapsed += OnTimedEvent;
            _fATimer.Interval = 1200;
        }

        public static void OpenPort()
        {
            _port.Open();
        }

        public void StartMeasuring()
        {
            _fATimer.Enabled = true;
        }

        public void StopMeasuring()
        {
            _fATimer.Enabled = false;
        }

        public void ClosePort()
        {
            StopMeasuring();
            _port.Close();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var sp = (SerialPort)sender;
            try
            {
                var cnt = sp.ReadBufferSize;
                _fRxData = new byte[cnt + 1];
                sp.Read(_fRxData, 0, cnt);
            }
            catch (Exception ex)
            {
                SetLogMessage?.Invoke(ex.Message);
            }
            var t = RcConvert(_fRxData);
            _fTempLog.Add(_fRxIdx, t);
            _fRxIdx++;
            EventHandler?.Invoke(t);
        }

        private static float RcConvert(IEnumerable<byte> rData)
        {
            var newData = TrimReceivedData(rData);
            if (newData.Length < 2) return 0;
            var tmp = new byte[4];
            tmp[0] = 0;
            tmp[1] = 0;
            tmp[2] = newData[0];
            tmp[3] = newData[1];
            float res = tmp[0] << 24 | tmp[1] << 16 | tmp[2] << 8 | tmp[3];
            return (res - 1000) / 10;
        }

        private void Write(byte[] buf)
        {
            try
            {
                _port.Write(buf, 0, buf.Length);
            }
            catch (Exception ex)
            {
                //ErrList.Add(ex.Message);
                SetLogMessage?.Invoke(ex.Message);
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            byte[] buf = { 01 };
            Write(buf);
            Thread.Sleep(100);
        }

        public float GetLastRes()
        {
            _fTempLog.TryGetValue(_fRxIdx - 1, out var val);
            return val;
        }

        private static byte[] TrimReceivedData(IEnumerable<byte> src)
        {
            var res = new List<byte>(src);
            while (res.LastOrDefault() == 0)
            {
                res.RemoveAt(res.Count - 1);
            }
            return res.ToArray();
        }
    }
}
