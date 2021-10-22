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
        private LaserSerial _port;

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        public void ConnectToPort()
        {
            _port = new LaserSerial(PortString);
            _port.SetLogMessage += TestLog;
            _port.OpenPort();
        }

        public void Disconnect()
        {
            _port?.ClosePort();
        }

        private void TestLog(string msg)
        {
            SetLogMessage?.Invoke(msg);
        }

        public void SetPower(int pwr)
        {
            _port?.SetPower(pwr);
        }

        public void SetLaserType(int tp)
        {
            _port?.SetLaserType(tp);
        }

        public void EmitOn(bool isEmit)
        {
            if (isEmit)
            {
                _port?.SetOn();
            }
            else
            {
                _port?.SetOff();
            }
        }

    }
}
