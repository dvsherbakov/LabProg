using System;
using System.Windows;

namespace LabProg
{

    public partial class MainWindow
    {

        private void LaserPortOn(object sender, RoutedEventArgs e)
        {
            try
            {
                _pyroSerial.OpenPort();
                LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Включен порт пирометра" });
                _pyroTimer.Start();
            } catch (Exception ex)
            {
                LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = ex.Message });
                ChbLaserPort.IsChecked = false;
            }
            try
            {
                _laserSerial.OpenPort();
                LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Включен порт лазера" });
            }
            catch (Exception ex)
            {
                LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = ex.Message });
                ChbLaserPort.IsChecked = false;
            }
        }

        private void LaserPortOff(object sender, RoutedEventArgs e)
        {
            _pyroSerial.ClosePort();
            _pyroTimer.Stop();
            LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Выключен порт пирометра" });
            _laserSerial.ClosePort();
            LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Выключен порт лазера" });
        }

    }
}