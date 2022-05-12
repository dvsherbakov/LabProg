using LabControl.PortModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabControl.LogicModels
{
    internal class LaserDriver
    {
        public string PortString { get; set; }
        private LaserSerial _port;
        private int _laserType;

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        public void ConnectToPort()
        {
            _port = _laserType switch
            {
                0 => new LaserOmicron(PortString),
                1 => new MrlIii660D(PortString),
                2 => new MrW6000(PortString),
                _ => _port
            };

            _port.SetLogMessage += TestLog;
            _port.Init();
        }

        public void Disconnect()
        {
            _port?.ClosePort();
        }

        private void TestLog(string msg)
        {
            SetLogMessage?.Invoke(msg);
        }

        public void SetPower(int pwr)
        {
            _port?.SetPowerLevel(pwr);
        }

        public void SetLaserType(int tp)
        {
            _laserType = tp;
        }

        public void EmitOn(bool isEmit)
        {
            if (isEmit)
            {
                _port?.On();
            }
            else
            {
                _port?.Off();
            }
        }
    }
}
