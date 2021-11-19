using LabControl.PortModels;

namespace LabControl.LogicModels
{
    internal class PyroDriver
    {
        public string PortStr { get; set; }
        private PyroSerial _fPort;

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        // public delegate void PyroEventHandler(float temperature);
        public event PyroSerial.PyroEventHandler EventHandler;

        public void ConnectToPort()
        {
            _fPort = new PyroSerial(PortStr);
            _fPort.SetLogMessage += TestLog;
            _fPort.EventHandler += HandleEvent;
            PyroSerial.OpenPort();
        }

        public void Disconnect()
        {
            _fPort?.ClosePort();
        }

        private void TestLog(string msg)
        {
            SetLogMessage?.Invoke(msg);
        }

        private void HandleEvent(float t)
        {
            EventHandler?.Invoke(t);
        }

        public void SetMeasuring(bool isMeasuring)
        {
            if (isMeasuring) _fPort.StartMeasuring(); else _fPort.StopMeasuring();
        }

    }
}
