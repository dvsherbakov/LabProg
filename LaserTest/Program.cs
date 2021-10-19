using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LaserTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var p = new PortModel();
            Console.WriteLine("Start");
            p.SetPower();

            p.On();
            Thread.Sleep(12000);
            Console.WriteLine("Stop");
            p.Off();
            Console.ReadLine();
        }

        
    }
}
