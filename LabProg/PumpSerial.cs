using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LabProg
{
    public class PumpSerial
    {
        private readonly SerialPort _mPort;
        private bool _active;
        private readonly List<string> RecievedData;
        private bool f_direction;
        public bool IsDriven { get; set; }
        private readonly string comId;
        public bool PumpReverse { get; set; }
        private readonly Action<string> addLogBoxMessage;
        private readonly ObservableCollection<string> cmdQueue;
        private Timer queueTimer;

        //private string prevSpeed = "";

        public PumpSerial(string portStr, bool startDirection, Action<string> addLogBoxMessage)
        {
            RecievedData = new List<string>();
            if (portStr == "") portStr = "COM7";
            PumpReverse = startDirection;
            comId = portStr;
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
            this.addLogBoxMessage = addLogBoxMessage;

            cmdQueue = new ObservableCollection<string>();
            cmdQueue.CollectionChanged += StartQueue;

            queueTimer = new Timer
            {
                Interval = 1000,
                Enabled = false,
            };
            queueTimer.Elapsed += TimerEvent;
        }

        async void StartQueue(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                await Task.Delay(1150);
                queueTimer.Enabled = true; ;
            }
        }

        void TimerEvent(object source, ElapsedEventArgs e)
        {
            var itm = cmdQueue.FirstOrDefault();
            cmdQueue.Remove(itm);
            WriteAnyCommand(itm);

            if (cmdQueue.Count == 0) queueTimer.Enabled = false;
        }

        private void WriteAnyCommand(string cmd)
        {
            if (_mPort.IsOpen)
            {
                _mPort.Write(cmd);
            }
            else
            {
                addLogBoxMessage($"Pump port {comId} is closed");
            }
        }

        public void OpenPort()
        {
            _mPort.Open();
            _active = true;
            IsDriven = false;
            //if (f_direction) SetClockwiseDirection(); else SetCounterClockwiseDirection();  
        }

        public bool IsOpen => _mPort.IsOpen;

        public void ClosePort()
        {
            _mPort.Close();
            _active = false;
            StopPump();
        }

        public bool Active() => _active;
        

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            System.Threading.Thread.Sleep(30);
            var cnt = _mPort.ReadBufferSize;
            var mRxdata = new byte[cnt + 1];
            try
            {
                _mPort.Read(mRxdata, 0, cnt);
            }
            catch (Exception ex)
            {
                addLogBoxMessage(ex.Message);
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
            catch (Exception ex)
            {
                addLogBoxMessage(ex.Message);
            }
            var ascii = Encoding.ASCII;
            return ascii.GetString(mRxdata);
        }


        public void StartPump()
        {
            if (_mPort.IsOpen)
            {
                if (!IsDriven) _mPort.Write("s");
                System.Threading.Thread.Sleep(130);
                IsDriven = true;
            }
            else
            {
                addLogBoxMessage($"Pump port {comId} is closed");
            }
        }

        public void AddStartPump()
        {
            cmdQueue.Add("s");
        }

        public void StopPump()
        {
            if (_mPort.IsOpen)
            {
                if (IsDriven) _mPort.Write("t");
                System.Threading.Thread.Sleep(130);
                IsDriven = false;
            }
            else
            {
                addLogBoxMessage($"Pump port {comId} is closed");
            }
        }

        public void AddStopPump()
        {
            cmdQueue.Add("t");
        }

        public void SetClockwiseDirection()
        {
            if (_mPort.IsOpen)
            {
                if (PumpReverse) _mPort.Write("l");
                else _mPort.Write("r");
                System.Threading.Thread.Sleep(130);
            }
            else
            {
                addLogBoxMessage($"Pump port {comId} is closed");
            }
        }

        public void AddClockwiseDirection()
        {
            if (PumpReverse) cmdQueue.Add("l");
            else cmdQueue.Add("r");
        }

        public void SetCounterClockwiseDirection()
        {
            if (_mPort.IsOpen)
            {
                if (PumpReverse) _mPort.Write("r");
                else _mPort.Write("l");
                System.Threading.Thread.Sleep(130);
            }
            else
            {
                addLogBoxMessage($"Pump port {comId} is closed");
            }
        }

        public void AddCounterClockwiseDirection()
        {
            if (PumpReverse) cmdQueue.Add("r");
            else cmdQueue.Add("l");
        }

        public void Reverce()
        {
            f_direction = !f_direction;
            if (f_direction) SetClockwiseDirection(); else SetCounterClockwiseDirection();
            System.Threading.Thread.Sleep(30);
        }

        public void SetSpeed(string speed)
        {
            if (_mPort.IsOpen)
            {
                //if (speed != prevSpeed)
                //{
                _mPort.Write(speed);
                System.Threading.Thread.Sleep(130);
                addLogBoxMessage($"Порт насоса {comId}, меняем скорость: '{speed}'");
                //} else addLogBoxMessage($"Порт насоса {comId}, скорость та же");
                //prevSpeed = speed;
            }
            else
            {
                addLogBoxMessage($"Pump port {comId} is closed");
            }
        }

        public void AddSpeed(string speed)
        {
            cmdQueue.Add(speed);
        }
    }

}