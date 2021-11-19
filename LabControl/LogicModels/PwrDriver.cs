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
    internal class PwrDriver
    {
        private PwrSerial _pwrSerial;
        public string PortStr { get; set; }

        public delegate void LogMessage(string msg);
        public event LogMessage SetLogMessage;

        public delegate void SetChannel(int channel, PwrItem pi);
        public event SetChannel SetChannelParameters;

        public void ConnectToPort()
        {
            _pwrSerial = new PwrSerial(PortStr);
            PwrSerial.OpenPort();
            _pwrSerial.SetLogMessage += TestLog;
            _pwrSerial.OnRecieve += GetSignal;
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

            var pi = new PwrItem(rxData);
            if (pi.IsCorrect)
            {
                SetChannelParameters?.Invoke(_pwrSerial.CurChannel, pi);

                SetLogMessage?.Invoke($"Чтение из бп, канал {_pwrSerial.CurChannel}, успешно");
            }
            else
            {
                SetLogMessage?.Invoke("Нераспознаный ответ БП");
            }

        }

        public void SetChannelOn(int channel)
        {
            _pwrSerial?.SetChannelOn(channel);
        }

        public void SetChannelOff(int channel)
        {
            _pwrSerial?.SetChannelOff(channel);
        }

        public void GetChanelData(byte channel)
        {
            _pwrSerial.CurChannel = channel;
            _pwrSerial?.GetChanelData(channel);
        }

        public void WriteChannelData(int channel, PwrItem pi)
        {
            _pwrSerial?.SetMode(channel, pi.Mode);
            if (pi.Amplitude > 0) _pwrSerial?.SetAmplitude(channel, pi.Amplitude);
            if ((pi.Bias <= 10000) && (pi.Bias > 0)) _pwrSerial?.SetBias(channel, pi.Bias);
            if (pi.Frequency > 0) _pwrSerial?.SetFreq(channel, pi.Frequency);
            if (pi.Duty > 0) _pwrSerial?.SetDuty(channel, pi.Duty);
            if (pi.Phase > 0) _pwrSerial?.SetPhase(channel, pi.Phase);
            if ((pi.MaxVolts <= 10000) && (pi.MaxVolts > 0)) _pwrSerial?.SetMaxVolts(channel, pi.MaxVolts);
            if (pi.MaxAmps > 0) _pwrSerial?.SetMaxAmps(channel, pi.MaxAmps);
        }

    }
}
