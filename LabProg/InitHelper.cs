﻿using System.Windows;
using System.IO.Ports;
using Timer = System.Timers.Timer;
using System.Threading;
using System.Text.RegularExpressions;
using System.Linq;
using System.Windows.Controls;
using System;
using System.Windows.Input;
using System.Net.Http.Headers;

namespace LabProg
{
    public partial class MainWindow : Window
    {
        private Timer f_ConfocalTimer;
        private Timer f_PyroTimer;
        private Timer f_CameraTimer;

        public ICommand QuitCommand { get; set; }

        //System.Windows.Threading.Dispatcher _dispatcher;
        private void InitInternalComponents()
        {
            f_StartAndorCameraCommand = new LambdaCommand(OnStartAndorCameraCommandExecute);
            CbPumpPort.Items.Clear();
            CbMirrorPort.Items.Clear();
            CbPowerPort.Items.Clear();
            CbPumpSecondPort.Items.Clear();
            CbLaserPort.Items.Clear();
            CbPyroPort.Items.Clear();
            CbArduinoPort.Items.Clear();
            CbDispenserPort.Items.Clear();
            foreach (var s in SerialPort.GetPortNames())
            {
                CbPumpPort.Items.Add(new TextBlock() { Text = s });
                CbMirrorPort.Items.Add(new TextBlock() { Text = s });
                CbPowerPort.Items.Add(new TextBlock() { Text = s });
                CbPumpSecondPort.Items.Add(new TextBlock() { Text = s });
                CbLaserPort.Items.Add(new TextBlock() { Text = s });
                CbPyroPort.Items.Add(new TextBlock() { Text = s });
                CbArduinoPort.Items.Add(new TextBlock() { Text = s });
                CbDispenserPort.Items.Add(new TextBlock() { Text = s });
            }
            CbPixelFormatConv.ItemsSource = PixelFormatsItems.GetList();
            CbPixelFormatConv.SelectedValuePath = "Value";
            CbPixelFormatConv.DisplayMemberPath = "Description";
            CbPixelFormatConv.SelectedIndex = 7;
            InitPwrItems();
            InitPumpItems();
            InitPyroTimer();
            InitCameraTimer();
            lvLaserPowerItems.Items.Clear();
            //_dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
            QuitCommand = new LambdaCommand(p => Application.Current.Shutdown());
            f_ConnectDispencerPortCommand = new LambdaCommand(DispenserPortConnect);
            f_StartDispencerCommand = new LambdaCommand(p => dispSerial.GetVersion());
            f_DisconnectDispencerPortCommand = new LambdaCommand(p => dispSerial.ClosePort());
        }

        private void InitPwrItems()
        { //LoadSettings
            var dl = new PwrModes();
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
            CbDispSignalType.SelectedIndex = Properties.Settings.Default.DispSignalType;
            SetPortSelection(CbPowerPort, Properties.Settings.Default.PwrPortIndex);
            SetPortSelection(CbPumpPort, Properties.Settings.Default.LvlPortIndex);
            SetPortSelection(CbPumpSecondPort, Properties.Settings.Default.LvlSecondPortIndex);
            SetPortSelection(CbDispenserPort, Properties.Settings.Default.DispencerPortIndex);
            SetPortSelection(CbLaserPort, Properties.Settings.Default.LaserPortIndex);
            CbLaserType.SelectedIndex = Properties.Settings.Default.LaserType;
            CbCamType.SelectedIndex = Properties.Settings.Default.CameraType;
            CbAndorMode.SelectedIndex = Properties.Settings.Default.AndorMode;
            CbDispCurrentChannel.SelectedIndex = Properties.Settings.Default.DispCurrentChannel;
            SetPortSelection(CbPyroPort, Properties.Settings.Default.PyroPortIndex);
            SetPortSelection(CbArduinoPort, Properties.Settings.Default.ArduinoPortIndex);
            SetPortSelection(CbMirrorPort, Properties.Settings.Default.MirrorPortIndex);
            tbSaveCamPath.Text = Properties.Settings.Default.CameraSavePath;
            tbSaveCamPrefix.Text = Properties.Settings.Default.CameraSavePrefix;
            tbFrameCount.Text = Properties.Settings.Default.CameraFrameMaxCount;
            SetChanellBiasTitle(1);
            OnChangeFrameMaxCount(this, null);
            TbTwoPumpToggle();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        { //Save Settings
            Properties.Settings.Default.PwrModeCh0 = CbModeCh0.SelectedIndex;
            Properties.Settings.Default.PwrModeCh1 = CbModeCh1.SelectedIndex;
            Properties.Settings.Default.PwrModeCh2 = CbModeCh2.SelectedIndex;
            Properties.Settings.Default.PwrModeCh3 = CbModeCh3.SelectedIndex;
            Properties.Settings.Default.PwrModeCh4 = CbModeCh4.SelectedIndex;
            Properties.Settings.Default.PwrModeCh5 = CbModeCh5.SelectedIndex;
            Properties.Settings.Default.DispSignalType = CbDispSignalType.SelectedIndex;
            Properties.Settings.Default.PwrPortIndex = GetPortSelection(CbPowerPort);
            Properties.Settings.Default.ArduinoPortIndex = GetPortSelection(CbArduinoPort);
            Properties.Settings.Default.MirrorPortIndex = GetPortSelection(CbMirrorPort);
            Properties.Settings.Default.LvlPortIndex = GetPortSelection(CbPumpPort);
            Properties.Settings.Default.DispencerPortIndex = GetPortSelection(CbDispenserPort);
            Properties.Settings.Default.LvlSecondPortIndex = GetPortSelection(CbPumpSecondPort);
            Properties.Settings.Default.LaserPortIndex = GetPortSelection(CbLaserPort);
            Properties.Settings.Default.LaserType = CbLaserType.SelectedIndex;
            Properties.Settings.Default.CameraType = CbCamType.SelectedIndex;
            Properties.Settings.Default.AndorMode = CbAndorMode.SelectedIndex;
            Properties.Settings.Default.DispCurrentChannel = CbDispCurrentChannel.SelectedIndex;
            //Properties.Settings.Default.PyroPortIndex = 8;
            Properties.Settings.Default.PyroPortIndex = GetPortSelection(CbPyroPort);
            Properties.Settings.Default.CameraSavePath = tbSaveCamPath.Text;
            Properties.Settings.Default.CameraSavePrefix = tbSaveCamPrefix.Text;
            Properties.Settings.Default.CameraFrameMaxCount = tbFrameCount.Text;
            Properties.Settings.Default.Save();
            if (!(_laserSerial is null))
            {
                _laserSerial.SetOff();
                _laserSerial.ClosePort();
            }
            PwrSerial.SetChannelOff(0); Thread.Sleep(100);
            PwrSerial.SetChannelOff(1); Thread.Sleep(100);
            PwrSerial.SetChannelOff(2); Thread.Sleep(100);
            PwrSerial.SetChannelOff(3); Thread.Sleep(100);
            PwrSerial.SetChannelOff(4); Thread.Sleep(100);
            PwrSerial.SetChannelOff(5); Thread.Sleep(100);
        }

        private void InitPumpItems()
        {
            f_ConfocalTimer = new Timer
            {
                Interval = 5000
            };
            f_ConfocalTimer.Elapsed += PeackInfo;
        }

        private void InitPyroTimer()
        {
            f_PyroTimer = new Timer
            {
                Interval = 1000
            };
            f_PyroTimer.Elapsed += PeackPyroInfo;
        }

        private int GetCameraTimerInterval()
        {
            var str = Properties.Settings.Default.FreqCam;
            var br = double.TryParse(str, out double outTime);
            if (br)
                return (int)(outTime * 1000);
            else
                return 1000;
        }

        private void InitCameraTimer()
        {
            f_CameraTimer = new Timer
            {
                Interval = GetCameraTimerInterval()
            };
            f_CameraTimer.Elapsed += OnTimerTeak;
        }

        private int GetPortNumber(string portName)
        {
            string resultString = string.Join(string.Empty, Regex.Matches(portName, @"\d+").OfType<Match>().Select(m => m.Value));
            int.TryParse(resultString, out int result);
            return result;
        }

        private void SetPortSelection(ComboBox cb, int port)
        {
            var items = cb.Items;
            var index = -1;
            var foundIndex = -1;
            foreach (var item in items)
            {
                index++;
                var it = (TextBlock)item;
                if (port == GetPortNumber(it.Text))
                    foundIndex = index;
            }
            if (foundIndex > 0) cb.SelectedIndex = foundIndex;
            else
            {
                var NewItem = new TextBlock() { Text = $"COM{port}", Opacity = 0.2 };
                var newIndex = cb.Items.Add(NewItem);
                cb.SelectedIndex = newIndex;
            }
        }

        private int GetPortSelection(ComboBox cb)
        {
            if (cb == null) return 0;
            var sItem = (TextBlock)cb.SelectedItem;
            return sItem == null ? 0 : GetPortNumber(sItem.Text);
        }

        public void AddLogBoxMessage(string message)
        {
            Dispatcher.Invoke(() => LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = message }));
        }


    }
}