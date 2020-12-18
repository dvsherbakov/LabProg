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
        private readonly PumpSerial _portInput;
        private readonly PumpSerial _portOutput;
        public string PortStrInput { get; set; }
        public string PortStrOutput { get; set; }
        public bool DirectionInput { get; set; }
        public bool DirectionOutput { get; set; }
        public PumpDriver(Action<string> addLogBoxMessage)
        {
            _portInput = new PumpSerial(PortStrInput, DirectionInput, addLogBoxMessage);
            _portOutput = new PumpSerial(PortStrOutput, DirectionOutput, addLogBoxMessage);
        }
    }
}
