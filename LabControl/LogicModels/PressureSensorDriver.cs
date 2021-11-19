using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LabControl.LogicModels
{
    internal class PressureSensorDriver
    {
        public string PortStr { get; set; }
        private PortModels.PressureSensor _port;

        private readonly Timer _timer = new Timer();

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        public delegate void PressureEventHandler(float pressure, float temperature);
        public event PressureEventHandler EventHandler;

        public PressureSensorDriver(string portName = "COM12")
        {
            _timer.Interval = 1000;
            PortStr = portName;
            _timer.Elapsed += OnTimedEvent;
        }

        public void ConnectToPort()
        {
            
            _port = new PortModels.PressureSensor(PortStr);
            _port.Open();
        }

        public void Disconnect()
        {
            _port.Close();
        }

        private void TestLog(string msg)
        {
            SetLogMessage?.Invoke(msg);
        }

        
        public void SetMeasuring(bool isMeasuring)
        {
            if (isMeasuring) _timer.Start(); else _timer.Stop();
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (!_port.IsOpen) return;
            var pressure = _port.GetPressure();
            var temp = _port.GetTemperature();
            EventHandler?.Invoke(pressure, temp);
        }

    }
}
