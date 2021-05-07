using LabControl.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabControl.LogicModels
{
    internal class PressurePumpDriver
    {
        private readonly int f_PId;
        private double[] f_Calibration;

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        public PressurePumpDriver()
        {
            ElvWrapper.AF1_Initialization("Dev1", 2, 5, out f_PId);
            f_Calibration = new double[1000];
            
        }

        public void Calibrate()
        {
            var fName = $"{System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}\\calibrate.arr";
            if (File.Exists(fName))
            {
                ElvWrapper.Elveflow_Calibration_Load($"{System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}\\calibrate.arr", f_Calibration, 1000);
                SetLogMessage?.Invoke("Калибровочный массив загружен");
            }
            else
            {
                SetLogMessage?.Invoke("Производиться калибровки, подождите около 2х минут");
                ElvWrapper.AF1_Calib(f_PId, f_Calibration, 1000);
                ElvWrapper.Elveflow_Calibration_Save($"{System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}\\calibrate.arr", f_Calibration, 1000);
                SetLogMessage?.Invoke("Калибровка окончена");
            }
        }

        ~PressurePumpDriver()
        {
            ElvWrapper.AF1_Destructor(f_PId);
        }

        public void SetPressure(double pressure)
        {
            ElvWrapper.AF1_Set_Press(f_PId, pressure*10, f_Calibration, 1000);
        }

        public double GetPressure()
        {
            ElvWrapper.AF1_Get_Press(f_PId, 1000, f_Calibration, out double pressure, 1000);
            return pressure;
        }

        public void SetTrigger(bool trigger)
        {
            int tr = trigger ? 1 : 0;
            ElvWrapper.AF1_Set_Trig(f_PId, tr);
        }

        public bool GetTrigger()
        {
            ElvWrapper.AF1_Get_Trig(f_PId, out int trigger);
            return trigger == 1;
        }
    }
}
