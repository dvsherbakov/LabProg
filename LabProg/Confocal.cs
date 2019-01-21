using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace LabProg
{
    class Confocal
    {
        private readonly PumpSerial pumpSerial;
        private bool Reverce { get; set; }
        private int PrevRev { get; set; }
        private bool Active { get; set; }
        private bool FDirection { get; set; }
        private bool AutoStop { get; set; }
        private string _prevSpeed;

        private static string ToFourStr(double src)
        {
            var res = src.ToString(src < 10.0 ? "#.##" : "##.#").Replace(',', '.');
            if (res.Length == 3)
                res = "0" + res;
            return res;
        }
    }
}
