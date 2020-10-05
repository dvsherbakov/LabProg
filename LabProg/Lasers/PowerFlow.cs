using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabProg.Lasers
{
    class PowerFlow
    {
        private readonly List<PowerFlowAtom> f_powers;
        public bool Circular { get; }

        public PowerFlow(bool circular)
        {
            f_powers = new List<PowerFlowAtom>();
            Circular = circular;
        }

        public void GenerateLinearCycle(int maxPower, int xTime, int minPower, int iTime, int cycles)
        {
            for (var i = 0; i < cycles; i++)
            {
                f_powers.Add(new PowerFlowAtom(maxPower, xTime));
                f_powers.Add(new PowerFlowAtom(minPower, iTime));
            }
        }

        public void GenerateHarmonicCycle(int amplitude, int level, double freq, int duration)
        {
            var quantumCount = duration * 20;


            for (var t = 0; t < quantumCount; t++)
            {
                f_powers.Add(new PowerFlowAtom((int)(level+(amplitude / 2) + Math.Round((amplitude / 2) * Math.Cos(2 * 3.1415 * freq * 0.05 * t))), 50));
            }

        }
        public int GetPeriodMilliseconds() => f_powers.Sum(x => x.Interval);

        public int[] GetPoersPoint() => f_powers.Select(x => x.Power).ToArray();

        public PowerFlowAtom[] GetSeries => f_powers.ToArray();
    }
}
