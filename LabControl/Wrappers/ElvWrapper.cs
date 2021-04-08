using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LabControl.Wrappers
{
    class ElvWrapper
    {
        [DllImport("Elveflow32.dll", EntryPoint = "AF1_Initialization",
       ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AF1_Initialization(string Device_Name, ushort Pressure_Regulator, ushort Sensor, out int AF1_ID_out);

        [DllImport("Elveflow32.dll", EntryPoint = "Elveflow_Calibration_Default",
       ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AF1_Calib(int AF1_ID_in, out double[] Calib_array_out, int len);
    }
}
