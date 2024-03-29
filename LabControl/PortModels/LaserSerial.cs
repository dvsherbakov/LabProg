﻿using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace LabControl.PortModels
{
    internal abstract class LaserSerial
    {
        protected string Name { get; set; }
        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        protected SerialPort Port;


        protected LaserSerial(string name)
        {
            Name = name;
        }

        public abstract void Init();

        public abstract void On();

        public abstract void Off();

        public abstract void SetPowerLevel(int pwr);

        public abstract void PowerLevelReduce(int pwr, int time);

        internal void ClosePort()
        {
            Port.Close();
        }

        protected static byte[] TrimReceivedData(IEnumerable<byte> src)
        {
            var res = new List<byte>(src);
            while (res.LastOrDefault() == 0)
            {
                res.RemoveAt(res.Count - 1);
            }
            return res.ToArray();
        }


        protected virtual void OnSetLogMessage(string msg)
        {
            SetLogMessage?.Invoke(msg);
        }
    }
}
