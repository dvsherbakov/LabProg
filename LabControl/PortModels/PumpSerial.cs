﻿using System;
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
        private readonly SerialPort f_MPort;
        //private readonly List<string> f_RecievedData;
        //private bool f_Direction;
        private readonly string f_ComId;
        public bool PumpReverse { private get; set; }
        private readonly ObservableCollection<string> f_CmdQueue;
        private readonly Timer f_QueueTimer;

        public bool Active { get; private set; }

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        public PumpSerial(string portStr, bool startDirection)
        {
            //f_RecievedData = new List<string>();
            if (portStr == "") portStr = "COM7";
            PumpReverse = startDirection;
            f_ComId = portStr;
            f_MPort = new SerialPort(portStr)
            {
                BaudRate = int.Parse("9600"),
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true
            };
            f_MPort.DataReceived += DataReceivedHandler;

            f_CmdQueue = new ObservableCollection<string>();
            f_CmdQueue.CollectionChanged += StartQueue;

            f_QueueTimer = new Timer
            {
                Interval = 1000,
                Enabled = false,
            };
            f_QueueTimer.Elapsed += TimerEvent;
        }

        private async void StartQueue(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add) return;
            await Task.Delay(1150);
            f_QueueTimer.Enabled = true;
        }

        void TimerEvent(object source, ElapsedEventArgs e)
        {
            if (f_CmdQueue.Count > 0)
            {
                var itm = f_CmdQueue.FirstOrDefault();
                if (f_CmdQueue.Count > 0) { f_CmdQueue.Remove(itm); }
                WriteAnyCommand(itm);
            }

            if (f_CmdQueue.Count == 0) f_QueueTimer.Enabled = false;
        }

        private void WriteAnyCommand(string cmd)
        {
            if (f_MPort.IsOpen)
            {
                try
                {
                    f_MPort.Write(cmd);
                    //SetLogMessage?.Invoke($"Pump start command {cmd}");
                }
                catch (Exception ex)
                {
                    SetLogMessage?.Invoke($"Port {f_ComId} is occuped");
                    SetLogMessage?.Invoke(ex.Message);
                }
            }
            else
            {
                SetLogMessage?.Invoke($"Pump port {f_ComId} is closed");
                f_CmdQueue.Clear();
            }
        }

        public void OpenPort()
        {
            SetLogMessage?.Invoke($"Try connect pump on port {f_ComId}");
            try
            {
                f_MPort.Open();
            }
            catch (Exception ex)
            {
                SetLogMessage?.Invoke(ex.Message);
            }
            Active = true;
        }

        public bool IsOpen => f_MPort.IsOpen;

        public void ClosePort()
        {
            f_QueueTimer.Enabled = false;
            f_CmdQueue.Clear();
            StopPump();
            f_MPort.Close();
            Active = false;
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            System.Threading.Thread.Sleep(30);
            var cnt = f_MPort.ReadBufferSize;
            var mRxData = new byte[cnt + 1];
            try
            {
                f_MPort.Read(mRxData, 0, cnt);
                mRxData = TrimReceivedData(mRxData);
            }
            catch (Exception ex)
            {
                SetLogMessage?.Invoke(ex.Message);
            }
            var ascii = Encoding.ASCII;
            if (mRxData.Length > 0)
            {
                //f_RecievedData.Add(ascii.GetString(mRxData));
                // Debug.WriteLine($"{f_ComId}:{ascii.GetString(mRxData)}");
            }
        }


        public void AddStartPump()
        {
            if (!f_MPort.IsOpen) return;
            f_CmdQueue.Add("s");
        }

        private void StopPump()
        {
            if (f_MPort.IsOpen)
            {
                f_MPort.Write("t");
                SetLogMessage?.Invoke($"Pump  stop: {f_ComId}");
            }
            else
            {
                SetLogMessage?.Invoke($"Pump port {f_ComId} is closed");
            }
        }

        public void AddStopPump()
        {
            if (f_MPort.IsOpen)
                f_CmdQueue.Add("t");
        }

        public void AddClockwiseDirection()
        {
            if (!f_MPort.IsOpen) return;
            if (PumpReverse) f_CmdQueue.Add("l");
            else f_CmdQueue.Add("r");
        }

        public void SetCounterClockwiseDirection()
        {
            if (f_MPort.IsOpen)
            {
                if (PumpReverse) f_MPort.Write("r");
                else f_MPort.Write("l");
                System.Threading.Thread.Sleep(130);
            }
            else
            {
                SetLogMessage?.Invoke($"Pump port {f_ComId} is closed");
            }
        }

        public void AddCounterClockwiseDirection()
        {
            if (!f_MPort.IsOpen) return;
            if (PumpReverse) f_CmdQueue.Add("r");
            else f_CmdQueue.Add("l");
        }

        public void AddSpeed(string speed)
        {
            if (!f_MPort.IsOpen) return;
            f_CmdQueue.Add(speed);
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
