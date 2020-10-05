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
                f_PyroTimer.Start();
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
            f_PyroTimer.Stop();
            LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Выключен порт пирометра" });
            _laserSerial.ClosePort();
            LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Выключен порт лазера" });
        }

        private void SetLaserPwr(object sender, RoutedEventArgs e)
        {
            var pwrText = TbLaserPwr.Text;
            if (!int.TryParse(pwrText, out var outPwr)) return;
            _laserSerial.SetPower(outPwr);
            //_laserSerial.SetPowerLevel(outPwr);
            LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = $"Попытка установить мощность лазера в {outPwr}" });
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

        private void CbLaserType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _laserSerial?.SetLaserType(((ComboBox)sender).SelectedIndex);
        }

        private async Task AutomaticLaserPowerAsync()
        {
            _laserSerial.SetOn();
            foreach (var item in lvLaserPowerItems.Items)
            {
               
               
                var lItem = (LaserPowerAtomResult)item;
                if (lItem.Type == LaserPowerAtomType.Linear)
                {
                    for (var i = 0; i < lItem.CyclesCount; i++)
                    {
                        _laserSerial.SetPower(lItem.HiPower);
                        await Task.Delay(lItem.HiDuration);
                        _laserSerial.SetPower(lItem.LowPower);
                        await Task.Delay(lItem.LowDuration);
                        LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = $"Цткл № {i}" });
                    }
                }
                else
                {
                    var pf = new PowerFlow(true);
                    pf.GenerateHarmonicCycle(lItem.Amplitude, lItem.Level, lItem.Freq, lItem.HarmonicalDuration);
                    var series = pf.GetSeries;
                    foreach (var seriesAtom in series)
                    {
                        _laserSerial.SetPower(seriesAtom.Power);
                        await Task.Delay(seriesAtom.Interval);
                    }
                }
            }
            LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = $"Закончили автосерию" });
            //_laserSerial.SetOff();
        }
        private void SendDiskretPower(object sender, RoutedEventArgs e)
        {
            var name = ((Button)sender).Name;
            var power = 0;
            switch (name) {
                case "btDiskertFirstButton":
                    Int32.TryParse(tbFirstDiskret.Text, out power);
                    break;
                case "btDiskertSecondButton":
                    Int32.TryParse(tbSecondDiskret.Text, out power);
                    break;
                case "btDiskertThirdButton":
                    Int32.TryParse(tbThirdDiskret.Text, out power);
                    break;
                default:
                    power = 0;
                    break;
            }
            _laserSerial.SetPower(power);
            //_laserSerial.SetPowerLevel(outPwr);
            LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = $"Попытка установить мощность лазера в {power}" });
        }
    }
}