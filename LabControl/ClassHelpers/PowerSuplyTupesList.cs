using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabControl.ClassHelpers
{
    class PowerSuplyTupesList
    {
        private readonly List<String> pList;

        public PowerSuplyTupesList()
        {
            pList = new List<string>() { "Регулирование по напряжению", "Регулирование по току", "Синусоидальный сигнал", "ШИМ сигнал" };
        }

        public List<string> GetTypesList()
        {
            return pList;
        }
    }
}
