using System;
using System.Windows;
using System.Windows.Controls;

namespace LabProg
{
    public partial class MainWindow : Window
    {
        private void PeackPyroInfo(object source, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() => PyroTemp.Text = _pyroSerial.GetLastRes().ToString("N2"));
            Dispatcher.Invoke(() => TbCurrentLPwr.Text = _laserSerial.GetLasePower().ToString());
        }
    }
}