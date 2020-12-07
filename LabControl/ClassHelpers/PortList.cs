﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (pList.Contains(initialport)) return pList;
            pList.Add(initialport);
            return pList;
        }
    }
}