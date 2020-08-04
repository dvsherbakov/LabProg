using LabProg.Lasers;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
                _laserSerial.SetPower(outPwr);
                //_laserSerial.SetPowerLevel(outPwr);
                LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = $"Попытка установить мощность лазера в {outPwr}" });
            }
        }

        private void SetLaserOn(object sender, RoutedEventArgs e)
        {
            _laserSerial.SetOn();
            LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = $"Попытка включения лазера" });
        }


        private async void SetLaserAutoAsync(object sender, RoutedEventArgs e)
        {
            _laserSerial.SetOn();
            await AutomaticLaserPowerAsync();
            LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = $"Попытка запуска серии" });
        }

        private void SetLaserOff(object sender, RoutedEventArgs e)
        {
            _laserSerial.SetOff();
            LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = $"Попытка выключения лазера" });
        }

        private void CbLaserType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_laserSerial != null)
            {
                _laserSerial.SetLaserType(((ComboBox)sender).SelectedIndex);
            }
        }

        private async System.Threading.Tasks.Task AutomaticLaserPowerAsync()
        {
            _laserSerial.SetOn();
            foreach (var item in lvLaserPowerItems.Items)
            {
                var lItem = (LaserPowerAtomResult)item;
                if (lItem.Type == LaserPowerAtomType.Linear)
                {
                    for (int i = 0; i < lItem.CyclesCount; i++)
                    {
                        _laserSerial.SetPower(lItem.HiPower);
                        await Task.Delay(lItem.HiDuration);
                        _laserSerial.SetPower(lItem.LowPower);
                        await Task.Delay(lItem.LowDuration);
                    }
                } else
                {
                    var pf = new PowerFlow(true);
                    pf.GenerateHarmonicCycle(lItem.Amplitude, lItem.Freq, lItem.HarmonicalDuration);
                    var series = pf.GetSeries;
                    for (int i = 0; i < series.Length; i++)
                    {
                        _laserSerial.SetPower(series[i].Power);
                        await Task.Delay(series[i].Interval);
                    }
                }
            }
            _laserSerial.SetOff();
        }
    }
}