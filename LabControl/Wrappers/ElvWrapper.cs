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

        [DllImport("Elveflow32.dll", EntryPoint = "AF1_Calib",
       ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AF1_Calib(int AF1_ID_in, double[] Calib_array_out, int len);


        [DllImport("Elveflow32.dll", EntryPoint = "Elveflow_Calibration_Load",
       ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Elveflow_Calibration_Load(string Path,  double[] Calib_Array_out, int len);

        [DllImport("Elveflow32.dll", EntryPoint = "AF1_Get_Press",
       ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AF1_Get_Press(int AF1_ID_in, int Integration_time, double[] Calib_array_in, out double Pressure, int len);

        [DllImport("Elveflow32.dll", EntryPoint = "AF1_Set_Press",
       ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AF1_Set_Press(int AF1_ID_in, double Pressure, double[] Calib_array_in, int len);

        [DllImport("Elveflow32.dll", EntryPoint = "Elveflow_Calibration_Save",
       ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Elveflow_Calibration_Save(string Path, double[] Calib_Array_in, int len);

        [DllImport("Elveflow32.dll", EntryPoint = "AF1_Destructor",
       ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AF1_Destructor(int AF1_ID_in);

        [DllImport("Elveflow32.dll", EntryPoint = "AF1_Get_Flow_rate",
       ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AF1_Get_Flow_rate(int AF1_ID_in, out double Flow);

        [DllImport("Elveflow32.dll", EntryPoint = "AF1_Get_Trig",
       ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AF1_Get_Trig(int AF1_ID_in, out int trigger);

        [DllImport("Elveflow32.dll", EntryPoint = "AF1_Set_Trig",
       ExactSpelling = false, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AF1_Set_Trig(int AF1_ID_in, int trigger);
    }
}
