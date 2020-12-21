using LabControl.PortModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabControl.LogicModels
{
    class PumpDriver
    {
        private PumpSerial _portInput;
        private PumpSerial _portOutput;
        public string PortStrInput { get; set; }
        public string PortStrOutput { get; set; }
        public bool DirectionInput { get; set; }
        public bool DirectionOutput { get; set; }

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;
        
        public PumpDriver()
        {

        }

        public void ConnectToPorts()
        {
            _portInput = new PumpSerial(PortStrInput, DirectionInput);
            _portInput.SetLogMessage += TestLog;
            _portOutput = new PumpSerial(PortStrOutput, DirectionOutput);
            _portOutput.SetLogMessage += TestLog;
        }

        private void TestLog(string msg)
        {
            SetLogMessage?.Invoke(msg);
        }
    }
}
