using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LabControl.PortModels
{
    public class PumpSerial
    {
        private readonly SerialPort _port;
        private readonly string _comId;
        public bool PumpReverse { private get; set; }
        private readonly ObservableCollection<string> _cmdQueue;
        private readonly Timer _queueTimer;

        public bool Active { get; private set; }

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        public PumpSerial(string portStr, bool startDirection)
        {
            if (portStr == "")
            {
                portStr = "COM7";
            }

            PumpReverse = startDirection;
            _comId = portStr;
            _port = new SerialPort(portStr)
            {
                BaudRate = int.Parse("9600"),
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true
            };
            _port.DataReceived += DataReceivedHandler;

            _cmdQueue = new ObservableCollection<string>();
            _cmdQueue.CollectionChanged += StartQueue;

            _queueTimer = new Timer
            {
                Interval = 1000,
                Enabled = false,
            };
            _queueTimer.Elapsed += TimerEvent;
        }

        private async void StartQueue(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
            {
                return;
            }

            await Task.Delay(1150);
            _queueTimer.Enabled = true;
        }

        private void TimerEvent(object source, ElapsedEventArgs e)
        {
            if (_cmdQueue.Count > 0)
            {
                if (!IsOpen)
                {
                    try
                    {
                        _port.Open();
                    }
                    catch (Exception ex)
                    {
                        SetLogMessage?.Invoke(ex.Message);
                        return;
                    }
                }

                var itm = _cmdQueue.FirstOrDefault();
                if (_cmdQueue.Count > 0) { _ = _cmdQueue.Remove(itm); }
                WriteAnyCommand(itm);
            }

            if (_cmdQueue.Count != 0)
            {
                return;
            }

            _queueTimer.Enabled = false;
            _port.Close();
        }

        private void WriteAnyCommand(string cmd)
        {
            if (IsOpen)
            {
                try
                {
                    _port.Write(cmd);
                }
                catch (Exception ex)
                {
                    SetLogMessage?.Invoke($"Port {_comId} is busy");
                    SetLogMessage?.Invoke(ex.Message);
                }
            }
            else
            {
                SetLogMessage?.Invoke($"Pump port {_comId} is closed");
                _cmdQueue.Clear();
            }
        }

        public void OpenPort()
        {
            SetLogMessage?.Invoke($"Try connect pump on port {_comId}");
            try
            {
                if (!IsOpen)
                {
                    _port.Open();
                }
            }
            catch (Exception ex)
            {
                SetLogMessage?.Invoke(ex.Message);
            }
            Active = true;
        }

        public bool IsOpen => _port.IsOpen;

        public void ClosePort()
        {
            _queueTimer.Enabled = false;
            _cmdQueue.Clear();
            StopPump();
            _port.Close();
            Active = false;
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            System.Threading.Thread.Sleep(30);
            var cnt = _port.ReadBufferSize;
            var mRxData = new byte[cnt + 1];
            try
            {
                _port.Read(mRxData, 0, cnt);
                mRxData = TrimReceivedData(mRxData);
            }
            catch (Exception ex)
            {
                SetLogMessage?.Invoke(ex.Message);
            }
            var ascii = Encoding.ASCII;
            if (mRxData.Length > 0)
            {
                Debug.WriteLine($"{_comId}:{ascii.GetString(mRxData)}");
            }
        }


        public void AddStartPump()
        {
            if (!IsOpen)
            {
                return;
            }

            _cmdQueue.Add("s");
        }

        private void StopPump()
        {
            if (IsOpen)
            {
                _port.Write("t");
                SetLogMessage?.Invoke($"Pump  stop: {_comId}");
            }
            else
            {
                SetLogMessage?.Invoke($"Pump port {_comId} is closed");
            }
        }

        public void AddStopPump()
        {
            _cmdQueue.Add("t");
        }

        public void AddClockwiseDirection()
        {
            _cmdQueue.Add(PumpReverse ? "l" : "r");
        }

        public void AddCounterClockwiseDirection()
        {

            _cmdQueue.Add(PumpReverse ? "r" : "l");
        }

        public void AddSpeed(string speed)
        {
            _cmdQueue.Add(speed);
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

        public void TestPump()
        {
            if (!IsOpen) return;
            _port.Write("l");
            Task.Delay(100);
            _port.Write("s");
            Task.Delay(500);
            _port.Write("t");
            Task.Delay(100);
            _port.Write("r");
            Task.Delay(100);
            _port.Write("s");
            Task.Delay(500);
            _port.Write("t");
        }
    }
}
