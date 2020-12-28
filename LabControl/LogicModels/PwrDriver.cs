using LabControl.PortModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabControl.LogicModels
{
    class PwrDriver
    {
        private PwrSerial f_pwrSerial;
        public string PortStr { get; set; }

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        public void ConnectToPort()
        {
            f_pwrSerial = new PwrSerial(PortStr);
            f_pwrSerial?.OpenPort();
            f_pwrSerial.SetLogMessage += TestLog;
        }

        public void Disconnect()
        {
            f_pwrSerial?.ClosePort();
        }

        private void TestLog(string msg)
        {
            SetLogMessage?.Invoke(msg);
        }
    }
}
