using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabProg.Lasers
{
    class PowerFlowAtom
    {
        public int Power { get; set; }
        public int Interval { get; set; }

        public PowerFlowAtom(int pwr, int tm)
        {
            Power = pwr;
            Interval = tm;
        }

        public PowerFlowAtom()
        {
            Power = 100;
            Interval = 100;
        }
    }
}
