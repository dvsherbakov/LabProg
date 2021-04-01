using LabControl.DataModels;
using LabControl.PortModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace LabControl.LogicModels
{
    class PwrDriver
    {
        private PwrSerial f_pwrSerial;
        public string PortStr { get; set; }

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        public delegate void SetChannel(int channel, PwrItem pi);
        public event SetChannel SetChannelParameters;

        public void ConnectToPort()
        {
            f_pwrSerial = new PwrSerial(PortStr);
            PwrSerial.OpenPort();
            f_pwrSerial.SetLogMessage += TestLog;
            f_pwrSerial.OnRecieve += GetSignal;
        }

        public void Disconnect()
        {
            PwrSerial.ClosePort();
        }

        private void TestLog(string msg)
        {
            SetLogMessage?.Invoke(msg);
        }

        public void GetSignal(byte[] rxData)
        {
           
            PwrItem pi = new PwrItem(rxData);
            if (pi.IsCorrect)
            {
                SetChannelParameters.Invoke(PwrSerial.CurChannel, pi);
                
                SetLogMessage?.Invoke($"Чтение из бп, канал {PwrSerial.CurChannel}, успешно");
            }
            else
            {
                SetLogMessage?.Invoke("Нераспознаный ответ БП");   
            }

        }

        public void SetChannelOn(int channel)
        {
            f_pwrSerial?.SetChannelOn(channel);
        }

        public void SetChannelOff(int channel)
        {
            f_pwrSerial?.SetChannelOff(channel);
        }

        public void GetChanelData(byte channel)
        {
            f_pwrSerial?.GetChanelData(channel);
        }

    }
}
