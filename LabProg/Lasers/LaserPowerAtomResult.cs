using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabProg
{
    public class LaserPowerAtomResult
    {
        public LaserPowerAtomType Type { get; set; }
        //Linear
        public int HiPower { get; set; }
        public int LowPower { get; set; }
        public int HiDuration { get; set; }
        public int LowDuration { get; set; }
        public int CyclesCount { get; set; }
        //Harmonical
        public int Amplitude { get; set; }
        public double Freq { get; set; }
        public int HarmonicalDuration { get; set; }

        public string AtomType => Type == LaserPowerAtomType.Linear ? "Линейный" : "Гармоничный";

        public string Duration => Type == LaserPowerAtomType.Harmonical ? HarmonicalDuration.ToString() : (((float)(HiDuration + LowDuration)) / 1000 * CyclesCount).ToString();

        public string Description
        {
            get
            {
                string v = $"ВУ Мощн:{HiPower} Длит:{HiDuration} НУ Мощн:{LowPower} Длит:{LowDuration}, {CyclesCount} цик";
                return Type == LaserPowerAtomType.Linear ? v :
                    $"Амплитуда:{Amplitude} частота:{Freq} длительность:{HarmonicalDuration}";
            }
        }
    }

    public enum LaserPowerAtomType
    {
        Linear,
        Harmonical
    }
}
