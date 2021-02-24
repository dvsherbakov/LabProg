using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace LabControl.ClassHelpers
{
    class PortList
    {
        private readonly List<String> pList;
        private readonly List<String> f_DispModeList;
        public PortList()
        {
            pList = QueryPortList();
            f_DispModeList = new List<string> { "Стандартная волна", "Синусоида", "Однократный импульс" };
        }
        private List<string> QueryPortList()
        {
            return SerialPort.GetPortNames().ToList();
        }

        public List<string> GetPortList()
        {
            return pList;
        }

        public List<string> GetPortList(string initialport)
        {
            if (initialport.Length < 2 || pList.Contains(initialport)) return pList;
            pList.Add(initialport);
            return pList;
        }

        public List<string> GetDispenserModes()
        {
            return f_DispModeList;
        }
    }
}
