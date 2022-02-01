using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
        public event LogMessage SetQueue;

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
                Interval = 300,
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
                
                if (_cmdQueue.Count > 0)
                {
                    var itm = _cmdQueue.FirstOrDefault();
                    _cmdQueue.RemoveAt(0);
                    WriteAnyCommand(itm);
                }
            }

            SetQueue?.Invoke(string.Join(":", _cmdQueue));

            if (_cmdQueue.Count != 0)
            {
                return;
            }

            _queueTimer.Enabled = false;
           
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
                    SetLogMessage?.Invoke($"Port {_comId} error: {ex.Message}");

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
                SetLogMessage?.Invoke(($"{_comId}:{ascii.GetString(mRxData)}"));
            }
        }

        private void AddToQueue(string cmd)
        {
            _cmdQueue.Add(cmd);
            SetQueue?.Invoke(string.Join(":", _cmdQueue));
        }

        public void AddStartPump()
        {
            if (!IsOpen)
            {
                SetLogMessage?.Invoke($"Can't start. Pump port {_comId} is closed");
                return;
            }

            AddToQueue("s");
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
            AddToQueue("t");
        }

        public void AddClockwiseDirection()
        {
            AddToQueue(PumpReverse ? "l" : "r");
        }

        public void AddCounterClockwiseDirection()
        {

            AddToQueue(PumpReverse ? "r" : "l");
        }

        public void AddSpeed(string speed)
        {
            AddToQueue(speed);
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

        public async Task TestPumpAsync()
        {
            if (!IsOpen) _port.Open();
            _port.Write("l");
            await Task.Delay(1000);
            _port.Write("s");
            await Task.Delay(5000);
            _port.Write("t");
            await Task.Delay(300);
            _port.Write("r");
            await Task.Delay(1000);
            _port.Write("s");
            await Task.Delay(5000);
            _port.Write("t");
        }

        public void TestStart()
        {
            if (!IsOpen) _port.Open();
            _port.Write("s");
        }

        public void TestStop()
        {
            if (!IsOpen) _port.Open();
            _port.Write("t");
        }
    }
}
