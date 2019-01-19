using System;
using System.Windows;
using System.Windows.Controls;

namespace LabProg
{
    public partial class MainWindow : Window
    {

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
            }
        }

        private void ChanelModeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).Name == "CbModeCh0") SetChanellBiasTitle(0);
            if (((ComboBox)sender).Name == "CbModeCh1") SetChanellBiasTitle(1);
        }

        private void ReadPwrParam(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Name == "PwrCh1Read") PwrSerial.GetChanellData(1);
        }

        private void PwrCanelOn(object sender, RoutedEventArgs e)
        {
            switch (((CheckBox)sender).Name)
            {
                case "PwrCh0Check":
                    PwrSerial.SetChannelOn(0);
                    break;
                case "PwrCh1Check":
                    PwrSerial.SetChannelOn(1);
                    break;
                case "PwrCh2Check":
                    PwrSerial.SetChannelOn(2);
                    break;
                case "PwrCh3Check":
                    PwrSerial.SetChannelOn(3);
                    break;
                case "PwrCh4Check":
                    PwrSerial.SetChannelOn(4);
                    break;
                case "PwrCh5Check":
                    PwrSerial.SetChannelOn(5);
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
                    break;
                case "PwrCh1Check":
                    PwrSerial.SetChannelOff(1);
                    break;
                case "PwrCh2Check":
                    PwrSerial.SetChannelOff(2);
                    break;
                case "PwrCh3Check":
                    PwrSerial.SetChannelOff(3);
                    break;
                case "PwrCh4Check":
                    PwrSerial.SetChannelOff(4);
                    break;
                case "PwrCh5Check":
                    PwrSerial.SetChannelOff(5);
                    break;
                default:
                    PwrSerial.SetChannelOff(9);
                    break;
            }
        }

        private void PwrCh0Wr(object sender, RoutedEventArgs e)
        {
            byte[] b = { 0x2, 0x3, 0xE8, 0xC7 };

            if (((Button)sender).Name == "PwrCh0Write") SetChanellBiasTitle(0);
        }
    }
}