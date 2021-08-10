using LabControl.Wrappers;
using System.IO;
using System.Threading;

namespace LabControl.LogicModels
{
    internal class PressurePumpDriver
    {
        private readonly int _pId;
        private readonly double[] _fCalibration;

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        public PressurePumpDriver()
        {
            _ = ElvWrapper.AF1_Initialization("Dev1", 2, 5, out _pId);
            _fCalibration = new double[1000];
            
        }

        public void Calibrate()
        {
            var fName = $"{Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}\\calibrate.arr";
            if (File.Exists(fName))
            {
                _ = ElvWrapper.Elveflow_Calibration_Load($"{System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}\\calibrate.arr", _fCalibration, 1000);
                SetLogMessage?.Invoke("Калибровочный массив загружен");
            }
            else
            {
                SetLogMessage?.Invoke("Производиться калибровки, подождите около 2х минут");
                ElvWrapper.AF1_Calib(_pId, _fCalibration, 1000);
                ElvWrapper.Elveflow_Calibration_Save($"{System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}\\calibrate.arr", _fCalibration, 1000);
                SetLogMessage?.Invoke("Калибровка окончена");
            }
        }

        ~PressurePumpDriver()
        {
            _ = ElvWrapper.AF1_Destructor(_pId);
        }

        public void SetPressure(double pressure)
        {
            ElvWrapper.AF1_Set_Press(_pId, pressure*10, _fCalibration, 1000);
        }

        public void StartImpulse()
        {
            var pressure = GetPressure();
            SetLogMessage?.Invoke("Запуск продувки");
            _ = ElvWrapper.AF1_Set_Press(_pId, 1900, _fCalibration, 1000);
            Thread.Sleep(2000);
            _ = ElvWrapper.AF1_Set_Press(_pId, pressure * 10, _fCalibration, 1000);
        }

        public double GetPressure()
        {
            _ = ElvWrapper.AF1_Get_Press(_pId, 1000, _fCalibration, out double pressure, 1000);
            return pressure;
        }

        public void SetTrigger(bool trigger)
        {
            var tr = trigger ? 1 : 0;
            _ = ElvWrapper.AF1_Set_Trig(_pId, tr);
        }

        public bool GetTrigger()
        {
            _ = ElvWrapper.AF1_Get_Trig(_pId, out var trigger);
            return trigger == 1;
        }
    }
}
