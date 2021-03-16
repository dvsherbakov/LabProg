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

        public void ConnectToPort()
        {
            f_pwrSerial = new PwrSerial(PortStr);
            PwrSerial.OpenPort();
            f_pwrSerial.SetLogMessage += TestLog;
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
                switch (PwrSerial.CurChannel)
                {
                    case 0:
                        //Dispatcher.Invoke(() => CbModeCh0.SelectedIndex = pi.Mode);
                        //Properties.Settings.Default.PwrCh0Bias = pi.Bias.ToString();
                        //Properties.Settings.Default.PwrCh0Amplitude = pi.Amplitude.ToString();
                        //Properties.Settings.Default.PwrCh0Frequency = pi.Frequency.ToString();
                        //Properties.Settings.Default.PwrCh0Duty = pi.Duty.ToString();
                        //Properties.Settings.Default.PwrCh0Phase = pi.Phase.ToString();
                        //Properties.Settings.Default.PwrCh0MaxVolts = pi.MaxVolts.ToString();
                        //Properties.Settings.Default.PwrCh0MaxAmps = pi.MaxAmps.ToString();
                        break;
                    case 1:
                        //Dispatcher.Invoke(() => CbModeCh1.SelectedIndex = pi.Mode);
                        //Properties.Settings.Default.PwrCh1Bias = pi.Bias.ToString();
                        //Properties.Settings.Default.PwrCh1Amplitude = pi.Amplitude.ToString();
                        //Properties.Settings.Default.PwrCh1Frequency = pi.Frequency.ToString();
                        //Properties.Settings.Default.PwrCh1Duty = pi.Duty.ToString();
                        //Properties.Settings.Default.PwrCh1Phase = pi.Phase.ToString();
                        //Properties.Settings.Default.PwrCh1MaxVolts = pi.MaxVolts.ToString();
                        //Properties.Settings.Default.PwrCh1MaxAmps = pi.MaxAmps.ToString();
                        break;
                    case 2:
                        //Dispatcher.Invoke(() => CbModeCh2.SelectedIndex = pi.Mode);
                        //Properties.Settings.Default.PwrCh2Bias = pi.Bias.ToString();
                        //Properties.Settings.Default.PwrCh2Amplitude = pi.Amplitude.ToString();
                        //Properties.Settings.Default.PwrCh2Frequency = pi.Frequency.ToString();
                        //Properties.Settings.Default.PwrCh2Duty = pi.Duty.ToString();
                        //Properties.Settings.Default.PwrCh2Phase = pi.Phase.ToString();
                        //Properties.Settings.Default.PwrCh2MaxVolts = pi.MaxVolts.ToString();
                        //Properties.Settings.Default.PwrCh2MaxAmps = pi.MaxAmps.ToString();
                        break;
                    case 3:
                        //Dispatcher.Invoke(() => CbModeCh3.SelectedIndex = pi.Mode);
                        //Properties.Settings.Default.PwrCh3Bias = pi.Bias.ToString();
                        //Properties.Settings.Default.PwrCh3Amplitude = pi.Amplitude.ToString();
                        //Properties.Settings.Default.PwrCh3Frequency = pi.Frequency.ToString();
                        //Properties.Settings.Default.PwrCh3Duty = pi.Duty.ToString();
                        //Properties.Settings.Default.PwrCh3Phase = pi.Phase.ToString();
                        //Properties.Settings.Default.PwrCh3MaxVolts = pi.MaxVolts.ToString();
                        //Properties.Settings.Default.PwrCh3MaxAmps = pi.MaxAmps.ToString();
                        break;
                    case 4:
                        //Dispatcher.Invoke(() => CbModeCh4.SelectedIndex = pi.Mode);
                        //Properties.Settings.Default.PwrCh4Bias = pi.Bias.ToString();
                        //Properties.Settings.Default.PwrCh4Amplitude = pi.Amplitude.ToString();
                        //Properties.Settings.Default.PwrCh4Frequency = pi.Frequency.ToString();
                        //Properties.Settings.Default.PwrCh4Duty = pi.Duty.ToString();
                        //Properties.Settings.Default.PwrCh4Phase = pi.Phase.ToString();
                        //Properties.Settings.Default.PwrCh4MaxVolts = pi.MaxVolts.ToString();
                        //Properties.Settings.Default.PwrCh4MaxAmps = pi.MaxAmps.ToString();
                        break;
                    case 5:
                        //Dispatcher.Invoke(() => CbModeCh5.SelectedIndex = pi.Mode);
                        //Properties.Settings.Default.PwrCh5Bias = pi.Bias.ToString();
                        //Properties.Settings.Default.PwrCh5Amplitude = pi.Amplitude.ToString();
                        //Properties.Settings.Default.PwrCh5Frequency = pi.Frequency.ToString();
                        //Properties.Settings.Default.PwrCh5Duty = pi.Duty.ToString();
                        //Properties.Settings.Default.PwrCh5Phase = pi.Phase.ToString();
                        //Properties.Settings.Default.PwrCh5MaxVolts = pi.MaxVolts.ToString();
                        //Properties.Settings.Default.PwrCh5MaxAmps = pi.MaxAmps.ToString();
                        break;
                    default:
                        break;
                }
                SetLogMessage?.Invoke($"Чтение из бп, канал {PwrSerial.CurChannel}, успешно");
            }
            else
            {
                SetLogMessage?.Invoke("Нераспознаный ответ БП");   
            }

        }

        public void SetChannelOn(int channel)
        {
            f_pwrSerial.SetChannelOn(channel);
        }
    }
}
