using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LabProg
{
    public partial class MainWindow
    {
        public ICommand f_StartDispencerCommand;
        public ICommand f_ConnectDispencerPortCommand;
        public ICommand f_DisconnectDispencerPortCommand;
       
        private void DispenserPortConnect(object p)
        {
            dispSerial.OpenPort();
        }

        private void DispenserPortConnect(object sender, RoutedEventArgs e)
        {
            dispSerial.OpenPort();
        }

        private void DispenserPortChange(object sender, RoutedEventArgs e)
        {
            if (dispSerial!=null)
                dispSerial.PortName = CbDispenserPort.Text;
        }

        private void DispenserStart(object sender, RoutedEventArgs e)
        {
            dispSerial.Start();
        }

        private void DispenserStop(object sender, RoutedEventArgs e)
        {
            dispSerial.Stop();
        }

        private void DispenserGetChannels(object sender, RoutedEventArgs e)
        {
            dispSerial.GetNumberOfChannels();
        }

        private void DispenserDump(object sender, RoutedEventArgs e)
        {
            dispSerial.Dump();
        }

        private void DispenserReset(object sender, RoutedEventArgs e)
        {
            dispSerial.SoftReset();
        }

        private void DispenserVersion(object sender, RoutedEventArgs e)
        {
            dispSerial.GetVersion();
        }

        private void DispenserChangeSignalType(object sender, RoutedEventArgs e)
        {
            var type = ((ComboBox)sender).SelectedIndex;
            switch (type)
            {
                case 0:
                    if (grSingleWave!=null) grSingleWave.Visibility = Visibility.Visible;
                    if (grHarmonycWave!=null) grHarmonycWave.Visibility = Visibility.Collapsed;
                    if (spAdvancedWave!=null) spAdvancedWave.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    if (grSingleWave != null) grSingleWave.Visibility = Visibility.Collapsed;
                    if (grHarmonycWave != null) grHarmonycWave.Visibility = Visibility.Visible;
                    if (spAdvancedWave != null) spAdvancedWave.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    if (grSingleWave != null) grSingleWave.Visibility = Visibility.Collapsed;
                    if (grHarmonycWave != null) grHarmonycWave.Visibility = Visibility.Collapsed;
                    if (spAdvancedWave != null) spAdvancedWave.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void SetDispOptions(byte[] data)
        {
            var v0 = BitConverter.ToInt16(data, 1);
            Properties.Settings.Default.DispV0 = v0.ToString();
            var rt1 = BitConverter.ToInt16(data, 2);
            Properties.Settings.Default.DispRiseTm = rt1.ToString();
            var v1 = BitConverter.ToInt16(data, 3);
            Properties.Settings.Default.DispV1 = v1.ToString();
            var t1 = BitConverter.ToInt16(data, 4);
            Properties.Settings.Default.DispKeepTm = t1.ToString();
            var ft = BitConverter.ToInt16(data, 5);
            Properties.Settings.Default.DispFallTm = ft.ToString();
            var v2 = BitConverter.ToInt16(data, 6);
            Properties.Settings.Default.DispV2 = v2.ToString();
            var t2 = BitConverter.ToInt16(data, 7);
            Properties.Settings.Default.DispLowTm = t2.ToString();
            var rt2 = BitConverter.ToInt16(data, 8);
            Properties.Settings.Default.DispRise2Tm = rt2.ToString();
        }

        public void DispathData(byte[] data)
        {
            Debug.WriteLine(data);
            if (data.Length > 2 & data[0]==0x06)
            {
                switch (data[1]){
                    case 60:
                        SetDispOptions(data);
                        break;
                }
            }
        }
    }
}