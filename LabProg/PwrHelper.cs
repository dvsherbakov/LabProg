using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace LabProg
{

    public partial class MainWindow
    {
        private void ReadPwrParam(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Name == "PwrCh0Read") PwrSerial.GetChanellData(0);
            if (((Button)sender).Name == "PwrCh1Read") PwrSerial.GetChanellData(1);
            if (((Button)sender).Name == "PwrCh2Read") PwrSerial.GetChanellData(2);
            if (((Button)sender).Name == "PwrCh3Read") PwrSerial.GetChanellData(3);
            if (((Button)sender).Name == "PwrCh4Read") PwrSerial.GetChanellData(4);
            if (((Button)sender).Name == "PwrCh5Read") PwrSerial.GetChanellData(5);
        }

        public void GetSignal(object sender, EventArgs e)
        {
            //Console.WriteLine(e);
            //PwrSerial.GetChanellData(1);
            var btSignal = sender as PwrSerial;
            var rs = BitConverter.ToString(btSignal._rxdata);
            PwrItem pi = new PwrItem(btSignal._rxdata);
            if (pi.IsCorrect)
            {
                switch (PwrSerial.CurChannel)
                {
                    case 0:
                        Dispatcher.Invoke(() => CbModeCh0.SelectedIndex = pi.Mode);
                        Properties.Settings.Default.PwrCh0Bias = pi.Bias.ToString();
                        Properties.Settings.Default.PwrCh0Amplitude = pi.Amplitude.ToString();
                        Properties.Settings.Default.PwrCh0Frequency = pi.Frequency.ToString();
                        Properties.Settings.Default.PwrCh0Duty = pi.Duty.ToString();
                        Properties.Settings.Default.PwrCh0Phase = pi.Phase.ToString();
                        Properties.Settings.Default.PwrCh0MaxVolts = pi.MaxVolts.ToString();
                        Properties.Settings.Default.PwrCh0MaxAmps = pi.MaxAmps.ToString();
                        break;
                    case 1:
                        Dispatcher.Invoke(() => CbModeCh1.SelectedIndex = pi.Mode);
                        Properties.Settings.Default.PwrCh1Bias = pi.Bias.ToString();
                        Properties.Settings.Default.PwrCh1Amplitude = pi.Amplitude.ToString();
                        Properties.Settings.Default.PwrCh1Frequency = pi.Frequency.ToString();
                        Properties.Settings.Default.PwrCh1Duty = pi.Duty.ToString();
                        Properties.Settings.Default.PwrCh1Phase = pi.Phase.ToString();
                        Properties.Settings.Default.PwrCh1MaxVolts = pi.MaxVolts.ToString();
                        Properties.Settings.Default.PwrCh1MaxAmps = pi.MaxAmps.ToString();
                        break;
                    case 2:
                        Dispatcher.Invoke(() => CbModeCh2.SelectedIndex = pi.Mode);
                        Properties.Settings.Default.PwrCh2Bias = pi.Bias.ToString();
                        Properties.Settings.Default.PwrCh2Amplitude = pi.Amplitude.ToString();
                        Properties.Settings.Default.PwrCh2Frequency = pi.Frequency.ToString();
                        Properties.Settings.Default.PwrCh2Duty = pi.Duty.ToString();
                        Properties.Settings.Default.PwrCh2Phase = pi.Phase.ToString();
                        Properties.Settings.Default.PwrCh2MaxVolts = pi.MaxVolts.ToString();
                        Properties.Settings.Default.PwrCh2MaxAmps = pi.MaxAmps.ToString();
                        break;
                    case 3:
                        Dispatcher.Invoke(() => CbModeCh3.SelectedIndex = pi.Mode);
                        Properties.Settings.Default.PwrCh3Bias = pi.Bias.ToString();
                        Properties.Settings.Default.PwrCh3Amplitude = pi.Amplitude.ToString();
                        Properties.Settings.Default.PwrCh3Frequency = pi.Frequency.ToString();
                        Properties.Settings.Default.PwrCh3Duty = pi.Duty.ToString();
                        Properties.Settings.Default.PwrCh3Phase = pi.Phase.ToString();
                        Properties.Settings.Default.PwrCh3MaxVolts = pi.MaxVolts.ToString();
                        Properties.Settings.Default.PwrCh3MaxAmps = pi.MaxAmps.ToString();
                        break;
                    case 4:
                        Dispatcher.Invoke(() => CbModeCh4.SelectedIndex = pi.Mode);
                        Properties.Settings.Default.PwrCh4Bias = pi.Bias.ToString();
                        Properties.Settings.Default.PwrCh4Amplitude = pi.Amplitude.ToString();
                        Properties.Settings.Default.PwrCh4Frequency = pi.Frequency.ToString();
                        Properties.Settings.Default.PwrCh4Duty = pi.Duty.ToString();
                        Properties.Settings.Default.PwrCh4Phase = pi.Phase.ToString();
                        Properties.Settings.Default.PwrCh4MaxVolts = pi.MaxVolts.ToString();
                        Properties.Settings.Default.PwrCh4MaxAmps = pi.MaxAmps.ToString();
                        break;
                    case 5:
                        Dispatcher.Invoke(() => CbModeCh5.SelectedIndex = pi.Mode);
                        Properties.Settings.Default.PwrCh5Bias = pi.Bias.ToString();
                        Properties.Settings.Default.PwrCh5Amplitude = pi.Amplitude.ToString();
                        Properties.Settings.Default.PwrCh5Frequency = pi.Frequency.ToString();
                        Properties.Settings.Default.PwrCh5Duty = pi.Duty.ToString();
                        Properties.Settings.Default.PwrCh5Phase = pi.Phase.ToString();
                        Properties.Settings.Default.PwrCh5MaxVolts = pi.MaxVolts.ToString();
                        Properties.Settings.Default.PwrCh5MaxAmps = pi.MaxAmps.ToString();
                        break;
                    default:
                        break;
                }
            }
            Dispatcher.Invoke(() => LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText =  $"Блок питания: {rs}" }));
        }

        private void PwrPortOn(object sender, RoutedEventArgs e)
        {
            try
            {
                _pwrSerial.OpenPort();
                _pwrSerial.onRecieve += GetSignal;
            }
            catch (Exception ex)
            {
                LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = ex.Message });
                CbConnectPwrPort.IsChecked = false;
            }
        }

        private void PwrPortOff(object sender, RoutedEventArgs e)
        {
            _pwrSerial.ClosePort();
        }

        private void PwrCanelOn(object sender, RoutedEventArgs e)
        {
            if (LogBox != null)
            {
                switch (((ToggleButton)sender).Name)
                {
                    case "PwrCh0Check":
                        LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Включен канал 0" });
                        PwrSerial.SetChannelOn(0);
                        break;
                    case "PwrCh1Check":
                        PwrSerial.SetChannelOn(1);
                        LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Включен канал 1" });
                        break;
                    case "PwrCh2Check":
                        PwrSerial.SetChannelOn(2);
                        LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Включен канал 2" });
                        break;
                    case "PwrCh3Check":
                        PwrSerial.SetChannelOn(3);
                        LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Включен канал 3" });
                        break;
                    case "PwrCh4Check":
                        PwrSerial.SetChannelOn(4);
                        LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Включен канал 4" });
                        break;
                    case "PwrCh5Check":
                        PwrSerial.SetChannelOn(5);
                        LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Включен канал 5" });
                        break;
                    default:
                        PwrSerial.SetChannelOn(9);
                        break;
                }
            }
        }

        private void PwrChannelOff(object sender, RoutedEventArgs e)
        {
            switch (((ToggleButton)sender).Name)
            {
                case "PwrCh0Check":
                    PwrSerial.SetChannelOff(0);
                    LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Выключен канал 0" });
                    break;
                case "PwrCh1Check":
                    PwrSerial.SetChannelOff(1);
                    LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Выключен канал 1" });
                    break;
                case "PwrCh2Check":
                    PwrSerial.SetChannelOff(2);
                    LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Выключен канал 2" });
                    break;
                case "PwrCh3Check":
                    PwrSerial.SetChannelOff(3);
                    LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Выключен канал 3" });
                    break;
                case "PwrCh4Check":
                    PwrSerial.SetChannelOff(4);
                    LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Выключен канал 4" });
                    break;
                case "PwrCh5Check":
                    PwrSerial.SetChannelOff(5);
                    LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Выключен канал 5" });
                    break;
                default:
                    PwrSerial.SetChannelOff(9);
                    break;
            }
        }

        private void SetChanellBiasTitle(int channel)
        {
            switch (channel)
            {
                case 0:
                    TbChannel0Bias.Text = CbModeCh0.SelectedIndex == 1 ? "Регулируемый ток (мА)" : "Напряжение смещения (мВ) ";
                    if (CbModeCh0.SelectedIndex == 0 || CbModeCh0.SelectedIndex == 1)
                        TbChannel0Amplitude.IsEnabled = false;
                    else TbChannel0Amplitude.IsEnabled = true;
                    break;
                case 1:
                    TbChannel1Bias.Text = CbModeCh1.SelectedIndex == 1 ? "Регулируемый ток (мА)" : "Напряжение смещения (мВ) ";
                    if (CbModeCh1.SelectedIndex == 0 || CbModeCh1.SelectedIndex == 1)
                        TbChannel1Amplitude.IsEnabled = false;
                    else TbChannel1Amplitude.IsEnabled = true;
                    break;
                case 2:
                    TbChannel2Bias.Text = CbModeCh2.SelectedIndex == 1 ? "Регулируемый ток (мА)" : "Напряжение смещения (мВ) ";
                    if (CbModeCh2.SelectedIndex == 0 || CbModeCh2.SelectedIndex == 1)
                        TbChannel2Amplitude.IsEnabled = false;
                    else TbChannel2Amplitude.IsEnabled = true;
                    break;
                case 3:
                    TbChannel3Bias.Text = CbModeCh3.SelectedIndex == 1 ? "Регулируемый ток (мА)" : "Напряжение смещения (мВ) ";
                    if (CbModeCh3.SelectedIndex == 0 || CbModeCh3.SelectedIndex == 1)
                        TbChannel3Amplitude.IsEnabled = false;
                    else TbChannel3Amplitude.IsEnabled = true;
                    break;
                case 4:
                    TbChannel4Bias.Text = CbModeCh4.SelectedIndex == 1 ? "Регулируемый ток (мА)" : "Напряжение смещения (мВ) ";
                    if (CbModeCh4.SelectedIndex == 0 || CbModeCh4.SelectedIndex == 1)
                        TbChannel4Amplitude.IsEnabled = false;
                    else TbChannel4Amplitude.IsEnabled = true;
                    break;
            }
        }

        private void ChanelModeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).Name == "CbModeCh0") SetChanellBiasTitle(0);
            if (((ComboBox)sender).Name == "CbModeCh1") SetChanellBiasTitle(1);
            if (((ComboBox)sender).Name == "CbModeCh2") SetChanellBiasTitle(2);
            if (((ComboBox)sender).Name == "CbModeCh3") SetChanellBiasTitle(3);
            if (((ComboBox)sender).Name == "CbModeCh4") SetChanellBiasTitle(4);
            if (((ComboBox)sender).Name == "CbModeCh5") SetChanellBiasTitle(5);
        }
               
        private void PwrCh0Write_Click(object sender, RoutedEventArgs e)
        {
            var mv = int.Parse(TbCh0MaxVolts.Text);
            if ((mv <= 10000) && (mv>0))_pwrSerial.SetMaxVolts(0, mv);
            var bias = int.Parse(TbCh0Bias.Text);
            if ((bias <= 10000) && (bias>0)) _pwrSerial.SetBias(0, bias);
            
        }

        private void PwrCh1Write_Click(object sender, RoutedEventArgs e)
        {
            int mv = int.Parse(TbCh1MaxVolts.Text);
            if ((mv <= 10000) && (mv > 0)) _pwrSerial.SetMaxVolts(1, mv);
            int bias = int.Parse(TbCh1Bias.Text);
            if ((bias <= 10000) && (bias > 0)) _pwrSerial.SetBias(1, bias);
        }

        private void PwrCh2Write_Click(object sender, RoutedEventArgs e)
        {
            int mv = int.Parse(TbCh2MaxVolts.Text);
            if ((mv <= 3000)&&(mv > 0)) _pwrSerial.SetMaxVolts(2, mv);
            int bias = int.Parse(TbCh2Bias.Text);
            if ((bias <= 10000) && (bias > 0)) _pwrSerial.SetBias(2, bias);
        }

        private void PwrCh3Write_Click(object sender, RoutedEventArgs e)
        {
            int mv = int.Parse(TbCh3MaxVolts.Text);
            if ((mv <= 3000) && (mv > 0)) _pwrSerial.SetMaxVolts(3, mv);
            int bias = int.Parse(TbCh3Bias.Text);
            if ((bias <= 10000) && (bias > 0)) _pwrSerial.SetBias(3, bias);
        }

        private void PwrCh4Write_Click(object sender, RoutedEventArgs e)
        {
            int mv = int.Parse(TbCh4MaxVolts.Text);
            if ((mv <= 3000) && (mv > 0)) _pwrSerial.SetMaxVolts(3, mv);
            int bias = int.Parse(TbCh3Bias.Text);
            if ((bias <= 10000) && (bias > 0)) _pwrSerial.SetBias(3, bias);
        }

        private void PwrCh5Write_Click(object sender, RoutedEventArgs e)
        {
            int mv = int.Parse(TbCh5MaxVolts.Text);
            if ((mv <= 4200) && (mv > 0)) _pwrSerial.SetMaxVolts(5, mv);
            int bias = int.Parse(TbChannel5Bias.Text);
            if (bias > 0) _pwrSerial.SetBias(5, bias);
        }
    }
}