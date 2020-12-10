using System;
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

        private int f_PwrCh0Mode;
        public int PwrCh0Mode
        {
            get => f_PwrCh0Mode;
            set => Set(ref f_PwrCh0Mode, value);
        }

        private int f_PwrCh0Bias;
        public int PwrCh0Bias
        {
            get => f_PwrCh0Bias;
            set => Set(ref f_PwrCh0Bias, value);
        }

        private int f_PwrCh0Amplitude;
        public int PwrCh0Amplitude
        {
            get => f_PwrCh0Amplitude;
            set => Set(ref f_PwrCh0Amplitude, value);
        }

        private int f_PwrCh0Duty;
        public int PwrCh0Duty
        {
            get => f_PwrCh0Duty;
            set => Set(ref f_PwrCh0Duty, value);
        }

        private int f_PwrCh0Freq;
        public int PwrCh0Freq
        {
            get => f_PwrCh0Freq;
            set => Set(ref f_PwrCh0Freq, value);
        }

        private int f_PwrCh0Phase;
        public int PwrCh0Phase
        {
            get => f_PwrCh0Phase;
            set => Set(ref f_PwrCh0Phase, value);
        }

        private int f_PwrCh0MaxAmps;
        public int PwrCh0MaxAmps
        {
            get => f_PwrCh0MaxAmps;
            set => Set(ref f_PwrCh0MaxAmps, value);
        }

        private int f_PwrCh0MaxVoltage;
        public int PwrCh0MaxVoltage
        {
            get => f_PwrCh0MaxVoltage;
            set => Set(ref f_PwrCh0MaxVoltage, value);
        }

        private int f_PwrCh1Mode;
        public int PwrCh1Mode
        {
            get => f_PwrCh1Mode;
            set => Set(ref f_PwrCh1Mode, value);
        }

        private bool f_PwrSwitchCh1;
        public bool PwrSwitchCh1
        {
            get => f_PwrSwitchCh1;
            set => Set(ref f_PwrSwitchCh1, value);
        }

        private int f_PwrCh1Bias;
        public int PwrCh1Bias
        {
            get => f_PwrCh1Bias;
            set => Set(ref f_PwrCh1Bias, value);
        }

        private int f_PwrCh1Amplitude;
        public int PwrCh1Amplitude
        {
            get => f_PwrCh1Amplitude;
            set => Set(ref f_PwrCh1Amplitude, value);
        }

        private int f_PwrCh1Freq;
        public int PwrCh1Freq
        {
            get => f_PwrCh1Freq;
            set => Set(ref f_PwrCh1Freq, value);
        }

        private int f_PwrCh1Duty;
        public int PwrCh1Duty
        {
            get => f_PwrCh1Duty;
            set => Set(ref f_PwrCh1Duty, value);
        }

        private int f_PwrCh1Phase;
        public int PwrCh1Phase
        {
            get => f_PwrCh1Phase;
            set => Set(ref f_PwrCh1Phase, value);
        }
        private int f_PwrCh1MaxVoltage;
        public int PwrCh1MaxVoltage
        {
            get => f_PwrCh1MaxVoltage;
            set => Set(ref f_PwrCh1MaxVoltage, value);
        }

        private int f_PwrCh1MaxAmps;
        public int PwrCh1MaxAmps
        {
            get => f_PwrCh1MaxAmps;
            set => Set(ref f_PwrCh1MaxAmps, value);
        }

        private bool f_PwrSwitchCh2;
        public bool PwrSwitchCh2
        {
            get => f_PwrSwitchCh2;
            set => Set(ref f_PwrSwitchCh2, value);
        }

        private int f_PwrCh2Mode;
        public int PwrCh2Mode
        {
            get => f_PwrCh2Mode;
            set => Set(ref f_PwrCh2Mode, value);
        }

        private int f_PwrCh2Bias;
        public int PwrCh2Bias
        {
            get => f_PwrCh2Bias;
            set => Set(ref f_PwrCh2Bias, value);
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
        public static string LabelOn => Properties.Resources.LabelOn;
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
        public static string LabelPwrChannelMode => Properties.Resources.LabelPwrChannelMode;
        public static string LabelPwrChannelBias => Properties.Resources.LabelPwrChannelBias;
        public static string LabelPwrChannelAmplitude => Properties.Resources.LabelPwrChannelAmplitude;
        public static string LabelPwrChannelFreq => Properties.Resources.LabelPwrChannelFreq;
        public static string LabelPwrChannelPhase => Properties.Resources.LabelPwrChannelPhase;
        public static string LabelChannel0 => Properties.Resources.LabelChannel0;
        public static string LabelChannel1 => Properties.Resources.LabelChannel1;
        public static string LabelIkLed1 => Properties.Resources.LabelIkLed1;
        public static string LabelIkLed2 => Properties.Resources.LabelIkLed2;
        public static string LabelPwrChannelDuty => Properties.Resources.LabelPwrChannelDuty;
        public static string LabelPwrMaxVoltage => Properties.Resources.LabelPwrMaxVoltage;
        public static string LabelPwrMaxAmps => Properties.Resources.LabelPwrMaxAmps;
        public static string LabelRead => Properties.Resources.LabelRead;
        public static string LabelWrite => Properties.Resources.LabelWrite;
        public static string LabelChannel2 => Properties.Resources.LabelChannel2;
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
            PwrCh0Mode = Properties.Settings.Default.PwrCh0Mode;
            PwrCh0Bias = Properties.Settings.Default.PwrCh0Bias;
            PwrCh0Amplitude = Properties.Settings.Default.PwrCh0Amplitude;
            PwrCh0Freq = Properties.Settings.Default.PwrCh0Freq;
            PwrCh0Duty = Properties.Settings.Default.PwrCh0Duty;
            PwrCh0Phase = Properties.Settings.Default.PwrCh0Phase;
            PwrCh0MaxVoltage = Properties.Settings.Default.PwrCh0MaxVoltage;
            PwrCh0MaxAmps = Properties.Settings.Default.PwrCh0MaxAmps;
            PwrCh1Mode = Properties.Settings.Default.PwrCh1Mode;
            PwrCh1Mode = Properties.Settings.Default.PwrCh1Mode;
            PwrCh1Bias = Properties.Settings.Default.PwrCh1Bias;
            PwrCh1Amplitude = Properties.Settings.Default.PwrCh1Amplitude;
            PwrCh1Freq = Properties.Settings.Default.PwrCh1Freq;
            PwrCh1Duty = Properties.Settings.Default.PwrCh1Duty;
            PwrCh1Phase = Properties.Settings.Default.PwrCh1Phase;
            PwrCh1MaxVoltage = Properties.Settings.Default.PwrCh1MaxVoltage;
            PwrCh1MaxAmps = Properties.Settings.Default.PwrCh1MaxAmps;
            PwrCh2Mode = Properties.Settings.Default.PwrCh2Mode;
            PwrCh2Bias = Properties.Settings.Default.PwrCh2Bias;
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
            Properties.Settings.Default.PwrCh0Mode = PwrCh0Mode;
            Properties.Settings.Default.PwrCh0Bias = PwrCh0Bias;
            Properties.Settings.Default.PwrCh0Amplitude = PwrCh0Amplitude;
            Properties.Settings.Default.PwrCh0Freq = PwrCh0Freq;
            Properties.Settings.Default.PwrCh0Duty = PwrCh0Duty;
            Properties.Settings.Default.PwrCh0Phase = PwrCh0Phase;
            Properties.Settings.Default.PwrCh0MaxVoltage = PwrCh0MaxVoltage;
            Properties.Settings.Default.PwrCh0MaxAmps = PwrCh0MaxAmps;
            Properties.Settings.Default.PwrCh1Mode = PwrCh1Mode;
            Properties.Settings.Default.PwrCh1Bias = PwrCh1Bias;
            Properties.Settings.Default.PwrCh1Amplitude = PwrCh1Amplitude;
            Properties.Settings.Default.PwrCh1Freq = PwrCh1Freq;
            Properties.Settings.Default.PwrCh1Duty = PwrCh1Duty;
            Properties.Settings.Default.PwrCh1Phase = PwrCh1Phase;
            Properties.Settings.Default.PwrCh1MaxVoltage = PwrCh1MaxVoltage;
            Properties.Settings.Default.PwrCh1MaxAmps = PwrCh1MaxAmps;
            Properties.Settings.Default.PwrCh2Mode = PwrCh2Mode;
            Properties.Settings.Default.PwrCh2Bias = PwrCh2Bias;
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
