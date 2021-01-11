using LabControl.PortModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabControl.LogicModels
{
    class LaserDriver
    {
        public string PortString { get; set; }
        private LaserSerial f_Port;

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        public void ConnectToPort()
        {
            f_Port = new LaserSerial(PortString);
            f_Port.SetLogMessage += TestLog;
            f_Port.OpenPort();
        }

        public void Disconnect()
        {
            f_Port.ClosePort();
        }

        private void TestLog(string msg)
        {
            SetLogMessage?.Invoke(msg);
        }

        public void SetPower(int pwr)
        {
            f_Port.SetPower(pwr);
        }
    }
}
