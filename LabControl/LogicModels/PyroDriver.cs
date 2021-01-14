using LabControl.PortModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabControl.LogicModels
{
    internal class PyroDriver
    {
        public string PortStr { get; set; }
        private PyroSerial f_Port;

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        public delegate void PyroEventHandler(float temperature);
        public event PyroSerial.PyroEventHandler EventHandler;

        public void ConnectToPort()
        {
            f_Port = new PyroSerial(PortStr);
            f_Port.SetLogMessage += TestLog;
            f_Port.EventHandler += HandleEvent;
            PyroSerial.OpenPort();
        }

        public void Disconnect()
        {
            f_Port.ClosePort();
        }

        private void TestLog(string msg)
        {
            SetLogMessage?.Invoke(msg);
        }

        private void HandleEvent(float t)
        {
            EventHandler?.Invoke(t);
        }

        public void SetMeasuring(bool isMeasuring)
        {
            if (isMeasuring) f_Port.StartMeasuring(); else f_Port.StopMeasuring();
        }

    }
}
