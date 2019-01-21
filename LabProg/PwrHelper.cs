using System;
using System.Windows;
using System.Windows.Controls;

namespace LabProg
{

    public partial class MainWindow : Window
    {
        private void ReadPwrParam(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Name == "PwrCh1Read") PwrSerial.GetChanellData(1);
        }

        private void PwrPortOn(object sender, RoutedEventArgs e)
        {
            pwrSerial.OpenPort();
        }

        private void PwrPortOff(object sender, RoutedEventArgs e)
        {
            pwrSerial.ClosePort();
        }

        private void PwrCanelOn(object sender, RoutedEventArgs e)
        {
            switch (((CheckBox)sender).Name)
            {
                case "PwrCh0Check":
                    PwrSerial.SetChannelOn(0);
                   // LogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Включен канал 0" });
                    break;
                case "PwrCh1Check":
                    PwrSerial.SetChannelOn(1);
                   // LogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Включен канал 1" });
                    break;
                case "PwrCh2Check":
                    PwrSerial.SetChannelOn(2);
                   // LogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Включен канал 2" });
                    break;
                case "PwrCh3Check":
                    PwrSerial.SetChannelOn(3);
                   // LogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Включен канал 3" });
                    break;
                case "PwrCh4Check":
                    PwrSerial.SetChannelOn(4);
                    //LogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Включен канал 4" });
                    break;
                case "PwrCh5Check":
                    PwrSerial.SetChannelOn(5);
                    //LogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Включен канал 5" });
                    break;
                default:
                    PwrSerial.SetChannelOn(9);
                    break;
            }
        }

        private void PwrChannelOff(object sender, RoutedEventArgs e)
        {
            switch (((CheckBox)sender).Name)
            {
                case "PwrCh0Check":
                    PwrSerial.SetChannelOff(0);
                    LogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Выключен канал 0" });
                    break;
                case "PwrCh1Check":
                    PwrSerial.SetChannelOff(1);
                    LogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Выключен канал 1" });
                    break;
                case "PwrCh2Check":
                    PwrSerial.SetChannelOff(2);
                    LogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Выключен канал 2" });
                    break;
                case "PwrCh3Check":
                    PwrSerial.SetChannelOff(3);
                    LogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Выключен канал 3" });
                    break;
                case "PwrCh4Check":
                    PwrSerial.SetChannelOff(4);
                    LogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Выключен канал 4" });
                    break;
                case "PwrCh5Check":
                    PwrSerial.SetChannelOff(5);
                    LogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Выключен канал 5" });
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
            int mv = int.Parse(TbCh0MaxVolts.Text);
            if (mv <= 10000) pwrSerial.SetMaxVolts(0, mv);
            int bias = int.Parse(TbCh0Bias.Text);
            if (bias <= 10000) pwrSerial.SetBias(0, bias);
            
        }

        private void PwrCh1Write_Click(object sender, RoutedEventArgs e)
        {
            int mv = int.Parse(TbCh1MaxVolts.Text);
            if (mv <= 10000) pwrSerial.SetMaxVolts(1, mv);
            int bias = int.Parse(TbCh1Bias.Text);
            if (bias <= 10000) pwrSerial.SetBias(1, bias);
        }

        private void PwrCh2Write_Click(object sender, RoutedEventArgs e)
        {
            int mv = int.Parse(TbCh2MaxVolts.Text);
            if (mv <= 3000) pwrSerial.SetMaxVolts(2, mv);
            int bias = int.Parse(TbCh2Bias.Text);
            if (bias <= 3000) pwrSerial.SetBias(2, bias);
        }

        private void PwrCh3Write_Click(object sender, RoutedEventArgs e)
        {
            int mv = int.Parse(TbCh3MaxVolts.Text);
            if (mv <= 3000) pwrSerial.SetMaxVolts(3, mv);
            int bias = int.Parse(TbCh3Bias.Text);
            if (bias <= 3000) pwrSerial.SetBias(3, bias);
        }
    }
}