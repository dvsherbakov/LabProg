using System.Windows;
using System.IO.Ports;
using Timer = System.Timers.Timer;
using System.Threading;

namespace LabProg
{
    public partial class MainWindow : Window
    {
        private Timer _confocalTimer;
        private Timer _pyroTimer;
        private Timer _cameraTimer;

        System.Windows.Threading.Dispatcher _dispatcher;
        private void InitInternalComponents()
        {
            //CbPumpPort.Items.Clear();
            //CbMirrorPort.Items.Clear();
            foreach (var s in SerialPort.GetPortNames())
            {
                CbPumpPort.Items.Add(s);
                CbMirrorPort.Items.Add(s);
                CbPowerPort.Items.Add(s);
            }
            InitPwrItems();
            InitPumpItems();
            InitPyroTimer();

            _dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
        }

        private void InitPwrItems()
        { //LoadSettings
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
            CbLaserPort.SelectedIndex = Properties.Settings.Default.LaserPortIndex;
            CbPyroPort.SelectedIndex = Properties.Settings.Default.PyroPortIndex;
            SetChanellBiasTitle(1);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        { //Save Settings
            Properties.Settings.Default.PwrModeCh0 = CbModeCh0.SelectedIndex;
            Properties.Settings.Default.PwrModeCh1 = CbModeCh1.SelectedIndex;
            Properties.Settings.Default.PwrModeCh2 = CbModeCh2.SelectedIndex;
            Properties.Settings.Default.PwrModeCh3 = CbModeCh3.SelectedIndex;
            Properties.Settings.Default.PwrModeCh4 = CbModeCh4.SelectedIndex;
            Properties.Settings.Default.PwrModeCh5 = CbModeCh5.SelectedIndex;
            Properties.Settings.Default.PwrPortIndex = CbPowerPort.SelectedIndex;
            Properties.Settings.Default.LvlPortIndex = CbPumpPort.SelectedIndex;
            Properties.Settings.Default.LaserPortIndex = CbLaserPort.SelectedIndex;
            Properties.Settings.Default.PyroPortIndex = CbPyroPort.SelectedIndex;
            Properties.Settings.Default.Save();
            PwrSerial.SetChannelOff(0); Thread.Sleep(300);
            PwrSerial.SetChannelOff(1); Thread.Sleep(300);
            PwrSerial.SetChannelOff(2); Thread.Sleep(300);
            PwrSerial.SetChannelOff(3); Thread.Sleep(300);
            PwrSerial.SetChannelOff(4); Thread.Sleep(300);
            PwrSerial.SetChannelOff(5); Thread.Sleep(300);
        }

        private void InitPumpItems()
        {
            _confocalTimer = new Timer
            {
                Interval = 2000
            };
            _confocalTimer.Elapsed += PeackInfo;
        }

        private void InitPyroTimer()
        {
            _pyroTimer = new Timer
            {
                Interval = 1000
            };
            _pyroTimer.Elapsed += PeackPyroInfo;
        }

        private void InitCameraTimer()
        {
            _cameraTimer = new Timer
            {
                Interval = 1000
            };
        }
    }
}