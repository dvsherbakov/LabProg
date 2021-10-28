using LabControl.PortModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabControl.LogicModels
{
    class LaserDriver
    {
        public string PortString { get; set; }
        private LaserSerial _port;
        private int _laserType;

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        public void ConnectToPort()
        {
            switch (_laserType)
            {
                case 0:  
                    _port = new LaserOmicron(PortString);
                    break;
                case 1:
                    _port = new MrlIii660D(PortString);
                    break;
                case 2:
                    _port = new MrW6000(PortString);
                    break;
            }

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
