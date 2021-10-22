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
            Console.WriteLine("Start");
            var p = new PortModel();
            //while (true)
            //{

                p.SetPower(50);
                Thread.Sleep(2000);
                //p.On();
                //Thread.Sleep(10000);
                //p.Off();
                //Thread.Sleep(2000);

            //}
            Console.ReadKey();

        }


    }
}
