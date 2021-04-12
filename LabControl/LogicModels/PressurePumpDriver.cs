using LabControl.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabControl.LogicModels
{
    class PressurePumpDriver
    {
        private readonly int p_Id = -1;
        private int err;
        private double[] Calibration;

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        public PressurePumpDriver()
        {
            err = ElvWrapper.AF1_Initialization("Dev1", 2, 5, out p_Id);
            double[] Calibration = new double[1000];
            
        }

        public void Calibrate()
        {
            var fName = $"{System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}\\calibrate.arr";
            if (File.Exists(fName))
            {
                err = ElvWrapper.Elveflow_Calibration_Load($"{System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}\\calibrate.arr", Calibration, 1000);
                SetLogMessage?.Invoke("Калибровочный массив загружен");
            }
            else
            {
                SetLogMessage?.Invoke("Производиться калибровки, подождите около 2х минут");
                err = ElvWrapper.AF1_Calib(p_Id, Calibration, 1000);
                err = ElvWrapper.Elveflow_Calibration_Save($"{System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}\\calibrate.arr", Calibration, 1000);
                SetLogMessage?.Invoke("Калибровка окончена");
            }
        }

        ~PressurePumpDriver()
        {
            ElvWrapper.AF1_Destructor(p_Id);
        }

        public void SetPressure(double pressure)
        {
            err = ElvWrapper.AF1_Set_Press(p_Id, pressure*10, Calibration, 1000);
        }

        public double GetPressure()
        {
            err = ElvWrapper.AF1_Get_Press(p_Id, 1000, Calibration, out double pressure, 1000);
            return pressure;
        }

        public void SetTrigger(bool trigger)
        {
            int tr = trigger ? 1 : 0;
            ElvWrapper.AF1_Set_Trig(p_Id, tr);
        }

        public bool GetTrigger()
        {
            ElvWrapper.AF1_Get_Trig(p_Id, out int trigger);
            return trigger == 1;
        }
    }
}
