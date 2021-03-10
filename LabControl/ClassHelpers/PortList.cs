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

        private List<string> GetPortList()
        {
            return pList;
        }

        public List<string> GetPortList(string initialPort)
        {
            if (initialPort == null) return GetPortList();
            if (initialPort.Length < 2 || pList.Contains(initialPort)) return pList;
            pList.Add(initialPort);
            return pList;
        }

        public List<string> GetDispenserModes()
        {
            return f_DispModeList;
        }
    }
}
