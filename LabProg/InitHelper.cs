using System.Windows;
using System.IO.Ports;
using System.Windows.Controls;
using Timer = System.Timers.Timer;

namespace LabProg
{
    public partial class MainWindow : Window
    {
        Timer ConfocalTimer;
        private void InitInternalComponents()
        {
            CbPumpPort.Items.Clear();
            CbMirrorPort.Items.Clear();
            foreach (var s in SerialPort.GetPortNames())
            {
                CbPumpPort.Items.Add(s);
                CbMirrorPort.Items.Add(s);
                CbPowerPort.Items.Add(s);
            }
            InitPwrItems();
            InitPumpItems();
        }

        private void InitPwrItems()
        {
            var dl= new PwrModes();
            CbModeCh1.Items.Clear();
            CbModeCh1.ItemsSource = dl.GetValues();
            CbModeCh1.SelectedValuePath = "Key";
            CbModeCh1.DisplayMemberPath = "Value";
            CbModeCh0.SelectedIndex = Properties.Settings.Default.PwrModeCh0;
            CbModeCh1.SelectedIndex = Properties.Settings.Default.PwrModeCh1;
            CbModeCh2.SelectedIndex = Properties.Settings.Default.PwrModeCh2;
            CbModeCh3.SelectedIndex = Properties.Settings.Default.PwrModeCh3;
            CbModeCh4.SelectedIndex = Properties.Settings.Default.PwrModeCh4;
            CbModeCh5.SelectedIndex = Properties.Settings.Default.PwrModeCh5;
            CbPowerPort.SelectedIndex = Properties.Settings.Default.PwrPortIndex;
            CbPumpPort.SelectedIndex = Properties.Settings.Default.LvlPortIndex;
            SetChanellBiasTitle(1);
        }

        private void InitPumpItems()
        {
            ConfocalTimer = new Timer
            {
                Interval = 1000
            };
            ConfocalTimer.Elapsed += PeackInfo;
        }
    }
}