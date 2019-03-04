using System;
using System.Windows;
using System.Windows.Controls;

namespace LabProg
{

    public partial class MainWindow : Window
    {

        private void LaserPortOn(object sender, RoutedEventArgs e)
        {
            _pyroSerial.OpenPort();
        }

        private void LaserPortOff(object sender, RoutedEventArgs e)
        {
            _pyroSerial.ClosePort();
        }

    }
}