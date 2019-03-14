using System;
using System.Windows;
using System.Windows.Controls;

namespace LabProg
{

    public partial class MainWindow : Window
    {

        private void LaserPortOn(object sender, RoutedEventArgs e)
        {
            try
            {
                _pyroSerial.OpenPort();
                _pyroTimer.Start();
                LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Включен порт пирометра" });
            } catch (Exception ex)
            {
                LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = ex.Message });
            }
            try
            {
                _laserSerial.OpenPort();
            }
            catch (Exception ex)
            {
                LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Включен порт лазера" });
                LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = ex.Message });
            }
        }

        private void LaserPortOff(object sender, RoutedEventArgs e)
        {
            _pyroSerial.ClosePort();
            _pyroTimer.Stop();
            LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Выключен порт пирометра" });
            _laserSerial.ClosePort();
        }

    }
}