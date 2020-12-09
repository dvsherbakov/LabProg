﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace LabControl.ViewModels
{
    internal class MainModel : ViewModel
    {
        #region ModelFields
        private WindowState f_CurWindowState;
        public WindowState CurWindowState
        {
            get => f_CurWindowState;
            set => Set(ref f_CurWindowState, value);
        }

        public static string WindowTitle => Properties.Resources.MainWindowTitle;

        private int f_WindowHeight;
        public int WindowHeight
        {
            get => f_WindowHeight;
            set => Set(ref f_WindowHeight, value);
        }

        private int f_WindowWidth;
        public int WindowWidth
        {
            get => f_WindowWidth;
            set => Set(ref f_WindowWidth, value);
        }

        private bool f_IsTwoPump;
        public bool IsTwoPump
        {
            get => f_IsTwoPump;
            set
            {
                LabelPumpCount = value ? Properties.Resources.LabelTwoPump : Properties.Resources.LabelOnePump;
                SecondPumpPortHeight = value ? 28 : 0;
                Set(ref f_IsTwoPump, value);
            }
        }

        private string f_LabelPumpCount;
        public string LabelPumpCount
        {
            get => f_LabelPumpCount;
            set => Set(ref f_LabelPumpCount, value);
        }

        private double f_ConfocalLevel;
        public double ConfocalLevel
        {
            get => f_ConfocalLevel;
            set => Set(ref f_ConfocalLevel, value);
        }

        private double f_ConfocalLevelSetter;
        public double ConfocalLevelSetter
        {
            get => f_ConfocalLevelSetter;
            set => Set(ref f_ConfocalLevelSetter, value);
        }

        private string f_IncomingPumpPortSelected;
        public string IncomingPumpPortSelected
        {
            get => f_IncomingPumpPortSelected;
            set => Set(ref f_IncomingPumpPortSelected, value);
        }

        private string f_OutloginPumpPortSelected;
        public string OutloginPumpPortSelected
        {
            get => f_OutloginPumpPortSelected;
            set => Set(ref f_OutloginPumpPortSelected, value);
        }

        private int f_CurrentLaserPower;
        public int CurrentLaserPower
        {
            get => f_CurrentLaserPower;
            set => Set(ref f_CurrentLaserPower, value);
        }

        private int f_LaserPowerSetter;
        public int LaserPowerSetter
        {
            get => f_LaserPowerSetter;
            set => Set(ref f_LaserPowerSetter, value);
        }

        private double f_CurrentTemperature;
        public double CurrentTemperature
        {
            get => f_CurrentTemperature;
            set => Set(ref f_CurrentTemperature, value);
        }

        private int f_SecondPumpPortHeight;
        public int SecondPumpPortHeight
        {
            get => f_SecondPumpPortHeight;
            set => Set(ref f_SecondPumpPortHeight, value);
        }

        private string f_LaserPortSelected;
        public string LaserPortSelected
        {
            get => f_LaserPortSelected;
            set => Set(ref f_LaserPortSelected, value);
        }

        private string f_PiroPortSelected;
        public string PiroPortSelected
        {
            get => f_PiroPortSelected;
            set => Set(ref f_PiroPortSelected, value);
        }

        private int f_LaserTypeSelectedIndex;
        public int LaserTypeSelectedIndex
        {
            get => f_LaserTypeSelectedIndex;
            set => Set(ref f_LaserTypeSelectedIndex, value);
        }

        private bool f_PwrSwitchCh0;
        public bool PwrSwitchCh0
        {
            get => f_PwrSwitchCh0;
            set => Set(ref f_PwrSwitchCh0, value);
        }

        private bool f_PwrSwitchCh1;
        public bool PwrSwitchCh1
        {
            get => f_PwrSwitchCh1;
            set => Set(ref f_PwrSwitchCh1, value);
        }
        private bool f_PwrSwitchCh2;
        public bool PwrSwitchCh2
        {
            get => f_PwrSwitchCh2;
            set => Set(ref f_PwrSwitchCh2, value);
        }
        #endregion

        #region Collections
        public ObservableCollection<ClassHelpers.LogItem> LogCollection { get; }
        public ObservableCollection<string> IncomingPumpPortCollection { get; set; }
        public ObservableCollection<string> PowerSupplyTypes { get; set; }
        public ObservableCollection<string> OutloginPumpPortCollection { get; set; }
        public ObservableCollection<string> LaserPortCollection { get; set; }
        public ObservableCollection<string> PiroPortCollection { get; set; }
        #endregion

        #region StaticLabels
        public static string LogMessageHeader => Properties.Resources.LogHeaderColumn1Name;
        public static string LabelPumpOperation => Properties.Resources.PumpOperationTitle;
        public static string LabelConfocalData => Properties.Resources.LabelConfocalData;
        public static string LabelConfocalSetter => Properties.Resources.LabelConfocalSetter;
        public static string LabelPumpActive => Properties.Resources.LabelPumpActive;
        public static string LabelPortConnection => Properties.Resources.LabelPortConnection;
        public static string LabelSettings => Properties.Resources.LabelSettings;
        public static string LabelInputPumpPort => Properties.Resources.LabelInputPumpPort;
        public static string LabelOutputPumpPort => Properties.Resources.LabelOutputPumpPort;
        public static string LaserOperationTitle => Properties.Resources.LaserOpeprationTitle;
        public static string LabelEmmitLaser => Properties.Resources.LabelEmmitLaser;
        public static string LabelCurrentPower => Properties.Resources.LabelCurrentPower;
        public static string LabelSetterPower => Properties.Resources.LabelSetterPower;
        public static string LabelCurrentTemperature => Properties.Resources.LabelCurrentTemperature;
        public static string LabelLaserPort => Properties.Resources.LabelLaserPort;
        public static string LabelPiroPort => Properties.Resources.LabelPiroPort;
        public static string LabelLaserType => Properties.Resources.LabelLaserType;
        public static string PowerSuplyTitle => Properties.Resources.PowerSuplyTitle;
        public static string LabelUfLed1 => Properties.Resources.LabelUfLed1;
        public static string LabelUfLed2 => Properties.Resources.LabelUfLed2;
        public static string LabelChanelsSwitch => Properties.Resources.LabelChanelsSwitch;
        public static string LabelIkLed1 => Properties.Resources.LabelIkLed1;
        public static string LabelIkLed2 => Properties.Resources.LabelIkLed2;
        #endregion
        //private Timer f_TestTimer;

        #region Commands
        public ICommand QuitCommand { get; }
        public ICommand MinimizedCommand { get; }
        public ICommand MaximizedCommand { get; }
        public ICommand NormalizeCommand { get; }
        public ICommand StandartSizeCommand { get; }
        #endregion
        public MainModel()
        {
            // init collections
            LogCollection = new ObservableCollection<ClassHelpers.LogItem>();
            IncomingPumpPortSelected = Properties.Settings.Default.IncomingPumpPortSelected;
            PowerSupplyTypes = new ObservableCollection<string>(new ClassHelpers.PowerSuplyTupesList().GetTypesList());
        IncomingPumpPortCollection = new ObservableCollection<string>(new ClassHelpers.PortList().GetPortList(IncomingPumpPortSelected));
            OutloginPumpPortSelected = Properties.Settings.Default.OutloginPumpPortSelected;
            OutloginPumpPortCollection = new ObservableCollection<string>(new ClassHelpers.PortList().GetPortList(OutloginPumpPortSelected));
            LaserPortSelected = Properties.Settings.Default.LaserPortSelected;
            LaserPortCollection = new ObservableCollection<string>(new ClassHelpers.PortList().GetPortList(LaserPortSelected));
            PiroPortSelected = Properties.Settings.Default.PiroPortSelected;
            PiroPortCollection = new ObservableCollection<string>(new ClassHelpers.PortList().GetPortList(PiroPortSelected));
            CurWindowState = WindowState.Normal;
            //load params from settings
            WindowHeight = Properties.Settings.Default.WindowHeight == 0 ? 550 : Properties.Settings.Default.WindowHeight;
            WindowWidth = Properties.Settings.Default.WindowWidth == 0 ? 850 : Properties.Settings.Default.WindowWidth;
            IsTwoPump = Properties.Settings.Default.IsTwoPump;
            ConfocalLevelSetter = Properties.Settings.Default.ConfocalLevelSetter;
            LaserPowerSetter = Properties.Settings.Default.LaserPowerSetter;
            LaserTypeSelectedIndex = Properties.Settings.Default.LaserTypeSelectedIndex;
            //init command area
            QuitCommand = new LambdaCommand(OnQuitApp);
            MinimizedCommand = new LambdaCommand(OnMinimizedCommandExecute);
            MaximizedCommand = new LambdaCommand(OnMaximizedCommandExecute);
            NormalizeCommand = new LambdaCommand(OnMaximizedCommandExecute);
            StandartSizeCommand = new LambdaCommand(OnStandartSizeCommand);
            //test area
            //f_TestTimer = new Timer(2000);
            //f_TestTimer.Elapsed += AddMockMessage;
            //f_TestTimer.Start();

            AddLogMessage("Application Started");
        }

        private void AddLogMessage(string message)
        {
            Application.Current.Dispatcher.Invoke(() => LogCollection.Add(new ClassHelpers.LogItem(DateTime.Now, message)));
        }

        private void OnQuitApp(object p)
        {
            Properties.Settings.Default.WindowHeight = WindowHeight;
            Properties.Settings.Default.WindowWidth = WindowWidth;
            Properties.Settings.Default.IsTwoPump = IsTwoPump;
            Properties.Settings.Default.ConfocalLevelSetter = ConfocalLevelSetter;
            Properties.Settings.Default.IncomingPumpPortSelected = IncomingPumpPortSelected;
            Properties.Settings.Default.OutloginPumpPortSelected = OutloginPumpPortSelected;
            Properties.Settings.Default.LaserPowerSetter = LaserPowerSetter;
            Properties.Settings.Default.LaserPortSelected = LaserPortSelected;
            Properties.Settings.Default.PiroPortSelected = PiroPortSelected;
            Properties.Settings.Default.LaserTypeSelectedIndex = LaserTypeSelectedIndex;
            Properties.Settings.Default.Save();
            Application.Current.Shutdown();
        }

        private void OnMinimizedCommandExecute(object p)
        {
            CurWindowState = WindowState.Minimized;
        }

        private void OnMaximizedCommandExecute(object p)
        {
            CurWindowState = CurWindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void OnStandartSizeCommand(object sender)
        {
            WindowHeight = 470;
            WindowWidth = 630;
        }
    }
}
