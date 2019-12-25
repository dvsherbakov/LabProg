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
            }
            catch (Exception ex)
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

        private void SetLaserPwr(object sender, RoutedEventArgs e)
        {
            var pwrText = TbLaserPwr.Text;
            if (Int32.TryParse(pwrText, out int outPwr))
            {
                //_laserSerial.SetPower(outPwr);
                _laserSerial.SetPowerLevel(outPwr);
                LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = $"Попытка установить мощность лазера в {outPwr}" });
            }
        }

        private void SetLaserOn(object sender, RoutedEventArgs e)
        {
            //_laserSerial.SetOn();
            _laserSerial.Start();
            LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = $"Попытка включения лазера" });
        }

        private void SetLaserOff(object sender, RoutedEventArgs e)
        {
            _laserSerial.SetOff();
            LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = $"Попытка выключения лазера" });
        }
    }
}