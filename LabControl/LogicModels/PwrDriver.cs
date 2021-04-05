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
                SetChannelParameters?.Invoke(f_pwrSerial.CurChannel, pi);
                
                SetLogMessage?.Invoke($"Чтение из бп, канал {f_pwrSerial.CurChannel}, успешно");
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
            f_pwrSerial.CurChannel = channel;
            f_pwrSerial?.GetChanelData(channel);
        }

        public void WriteChannelData(int channel, PwrItem pi)
        {
            f_pwrSerial?.SetMode(channel, pi.Mode);
            if (pi.Amplitude> 0) f_pwrSerial.SetAmplitude(channel, pi.Amplitude);
            if ((pi.Bias <= 10000) && (pi.Bias > 0)) f_pwrSerial.SetBias(channel, pi.Bias);
            if (pi.Frequency> 0) f_pwrSerial.SetFreq(channel, pi.Frequency);
            if (pi.Duty > 0) f_pwrSerial.SetDuty(channel, pi.Duty);
            if (pi.Phase> 0) f_pwrSerial.SetPhase(channel, pi.Phase);
            if ((pi.MaxVolts <= 10000) && (pi.MaxVolts > 0)) f_pwrSerial.SetMaxVolts(channel, pi.MaxVolts);
            if (pi.MaxAmps > 0) f_pwrSerial.SetMaxAmps(channel, pi.MaxAmps);
        }

    }
}
