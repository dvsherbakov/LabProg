using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaserTest
{
    internal abstract class LaserSerial
    {
        public string Name { get; set; }
        public delegate void LogMessage(string msg);


        protected SerialPort _port;


        protected LaserSerial(string name)
        {
            Name = name;
        }

        public abstract void Init();

        public abstract void On();

        public abstract void Off();

        public abstract void SetPowerLevel(int pwr);

        protected static byte[] TrimReceivedData(IEnumerable<byte> src)
        {
            var res = new List<byte>(src);
            while (res.LastOrDefault() == 0)
            {
                res.RemoveAt(res.Count - 1);
            }
            return res.ToArray();
        }


    }

}
