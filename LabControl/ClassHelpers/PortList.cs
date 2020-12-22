using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace LabControl.ClassHelpers
{
    class PortList
    {
        private readonly List<String> pList;
        public PortList()
        {
            pList = QueryPortList();
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
    }
}
