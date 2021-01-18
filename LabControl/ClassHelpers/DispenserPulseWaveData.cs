using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabControl.ClassHelpers
{
    public class DispenserPulseWaveData
    {
        public int TimeRise1 { get; set; }
        public int TimeT1 { get; set; }
        public int TimeFall { get; set; }
        public int TimeT2 { get; set; }
        public int TimeRise2 { get; set; }
        //Volts
        public int V0 { get; set; }
        public int V1 { get; set; }
        public int V2 { get; set; }
    }
}
