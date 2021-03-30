﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Timers;

namespace LabControl.PortModels
{
    internal class PyroSerial
    {
        private static SerialPort _port;
        private byte[] f_RxData;
        private long f_RxIdx;
        private readonly Dictionary<long, float> f_TempLog = new Dictionary<long, float>();
        private readonly Timer f_ATimer = new Timer();

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        public delegate void PyroEventHandler(float temperature);
        public event PyroEventHandler EventHandler;

        public PyroSerial(string port)
        {
            
            if (string.IsNullOrEmpty(port)) port = "COM4";
            f_RxIdx = 0;
            SetLogMessage?.Invoke($"Try connected Pyro on port {port}");
            _port = new SerialPort(port)
            {
                BaudRate = 115200,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None,
                RtsEnable = true
            };
            _port.DataReceived += DataReceivedHandler;
            f_ATimer.Elapsed += OnTimedEvent;
            f_ATimer.Interval = 1200;
        }

        public static void OpenPort()
        {
            _port.Open();
        }

        public void StartMeasuring()
        {
            f_ATimer.Enabled = true;
        }

        public void StopMeasuring()
        {
            f_ATimer.Enabled = false;
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
                f_RxData = new byte[cnt + 1];
                sp.Read(f_RxData, 0, cnt);
            }
            catch (Exception ex)
            {
                SetLogMessage?.Invoke(ex.Message);
            }
            var t = RcConvert(f_RxData);
            f_TempLog.Add(f_RxIdx, t);
            f_RxIdx++;
            EventHandler?.Invoke(t);
        }

        private static float RcConvert(byte[] rData)
        {
            //while (rData[0] > 10)
            //{
            //    var tLst = new List<byte>(rData);
            //    tLst.Remove(0);
            //    rData = tLst.ToArray();
            //}
            var tmp = new byte[4];
            tmp[0] = 0;
            tmp[1] = 0;
            tmp[2] = rData[0];
            tmp[3] = rData[1];
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
        }

        public float GetLastRes()
        {
            f_TempLog.TryGetValue(f_RxIdx - 1, out var val);
            return val;
        }
    }
}
