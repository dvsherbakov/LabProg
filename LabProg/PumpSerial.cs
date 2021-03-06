﻿using System;
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
        private readonly SerialPort f_MPort;
        private readonly List<string> f_RecievedData;
        private bool f_Direction;
        //private bool IsDriven { get; set; }
        private readonly string f_ComId;
        public bool PumpReverse { private get; set; }
        private readonly Action<string> f_AddLogBoxMessage;
        private readonly ObservableCollection<string> f_CmdQueue;
        private readonly Timer f_QueueTimer;

        //private string prevSpeed = "";

        public PumpSerial(string portStr, bool startDirection, Action<string> addLogBoxMessage)
        {
            f_RecievedData = new List<string>();
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
            this.f_AddLogBoxMessage = addLogBoxMessage;

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
            f_QueueTimer.Enabled = true; ;
        }

        void TimerEvent(object source, ElapsedEventArgs e)
        {
            if (f_CmdQueue.Count > 0)
            {
                var itm = f_CmdQueue.FirstOrDefault();
                f_CmdQueue.Remove(itm);
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
                    f_AddLogBoxMessage($"Pump start command {cmd}");
                } catch (Exception ex)
                {
                    f_AddLogBoxMessage($"Port {f_ComId} is occuped");
                    f_AddLogBoxMessage(ex.Message);
                }
            }
            else
            {
                f_AddLogBoxMessage($"Pump port {f_ComId} is closed");
            }
        }

        public void OpenPort()
        {
            f_MPort.Open();
            Active = true;
            //IsDriven = false;
            
        }

        public bool IsOpen => f_MPort.IsOpen;

        public void ClosePort()
        {
            f_MPort.Close();
            Active = false;
            StopPump();
        }

        public bool Active { get; private set; }


        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            System.Threading.Thread.Sleep(30);
            var cnt = f_MPort.ReadBufferSize;
            var mRxData = new byte[cnt + 1];
            try
            {
                f_MPort.Read(mRxData, 0, cnt);
            }
            catch (Exception ex)
            {
                f_AddLogBoxMessage(ex.Message);
            }
            var ascii = Encoding.ASCII;
            f_RecievedData.Add(ascii.GetString(mRxData));
        }

        public string ReadPortData()
        {
            var cnt = f_MPort.ReadBufferSize;
            var mRxData = new byte[cnt + 1];
            try
            {
                f_MPort.Read(mRxData, 0, cnt);
            }
            catch (Exception ex)
            {
                f_AddLogBoxMessage(ex.Message);
            }
            var ascii = Encoding.ASCII;
            return ascii.GetString(mRxData);
        }


        public void StartPump()
        {
            if (f_MPort.IsOpen)
            {
                f_MPort.Write("s");
                f_AddLogBoxMessage("Pump  start");
            }
            else
            {
                f_AddLogBoxMessage($"Pump port {f_ComId} is closed");
            }
        }

        public void AddStartPump()
        {
            f_CmdQueue.Add("s");
        }

        private void StopPump()
        {
            if (f_MPort.IsOpen)
            {
               f_MPort.Write("t");
                f_AddLogBoxMessage("Pump  stop");
            }
            else
            {
                f_AddLogBoxMessage($"Pump port {f_ComId} is closed");
            }
        }

        public void AddStopPump()
        {
            f_CmdQueue.Add("t");
        }

        public void SetClockwiseDirection()
        {
            if (f_MPort.IsOpen)
            {
                if (PumpReverse) f_MPort.Write("l");
                else f_MPort.Write("r");
            }
            else
            {
                f_AddLogBoxMessage($"Pump port {f_ComId} is closed");
            }
        }

        public void AddClockwiseDirection()
        {
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
                f_AddLogBoxMessage($"Pump port {f_ComId} is closed");
            }
        }

        public void AddCounterClockwiseDirection()
        {
            if (PumpReverse) f_CmdQueue.Add("r");
            else f_CmdQueue.Add("l");
        }

        public void Reverce()
        {
            f_Direction = !f_Direction;
            if (f_Direction) SetClockwiseDirection(); else SetCounterClockwiseDirection();
            System.Threading.Thread.Sleep(30);
        }

        public void SetSpeed(string speed)
        {
            if (f_MPort.IsOpen)
            {
                f_MPort.Write(speed);
                System.Threading.Thread.Sleep(130);
                f_AddLogBoxMessage($"Порт насоса {f_ComId}, меняем скорость: '{speed}'");
            }
            else
            {
                f_AddLogBoxMessage($"Pump port {f_ComId} is closed");
            }
        }

        public void AddSpeed(string speed)
        {
            f_CmdQueue.Add(speed);
        }
    }

}