using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LabControl.LogicModels
{
    class PressureSensorDriver
    {
        public string PortStr { get; set; }
        private PortModels.PressureSensor f_Port;

        private Timer f_Timer = new Timer();

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        public delegate void PressureEventHandler(float pressure, float temperature);
        public event PressureEventHandler EventHandler;

        public PressureSensorDriver(string portName = "COM12")
        {
            f_Timer.Interval = 1000;
            PortStr = portName;
            f_Timer.Elapsed += OnTimedEvent;
        }

        public void ConnectToPort()
        {
            
            f_Port = new PortModels.PressureSensor(PortStr);
            f_Port.Open();
        }

        public void Disconnect()
        {
            f_Port.Close();
        }

        private void TestLog(string msg)
        {
            SetLogMessage?.Invoke(msg);
        }

        
        public void SetMeasuring(bool isMeasuring)
        {
            if (isMeasuring) f_Timer.Start(); else f_Timer.Stop();
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            var pressure = f_Port.GetPressure();
            var temp = f_Port.GetTemperature();
            EventHandler?.Invoke(pressure, temp);
        }

    }
}
