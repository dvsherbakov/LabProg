using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YodnTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var ps = new YodnSerial();
            ps.Init();
            
            
            ps.SetUvChannel(22);
            
            ps.SetBlueChannel(19);
            
            ps.SetGreenRedChannel(47);
            
            ps.On();
            Console.ReadKey();
            ps.Off();
            Console.ReadKey();

            ps.On();
            Console.ReadKey();
            ps.Off();
            Console.ReadKey();
        }
    }
}
