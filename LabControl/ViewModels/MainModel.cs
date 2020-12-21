using LabControl.ClassHelpers;
using LabControl.LogicModels;
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
        #region drivers
        private PumpDriver f_PumpDriver;
        private ConfocalDriver f_ConfocalDriver;
        #endregion

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
                SecondPumpPanelVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                LabelPump = value ? Properties.Resources.LabelPumpIn : Properties.Resources.LabelPump;
                Set(ref f_IsTwoPump, value);
            }
        }

        private string f_LabelPump;
        public string LabelPump
        {
            get => f_LabelPump;
            set => Set(ref f_LabelPump, value);
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

        private Visibility f_SecondPumpPanelVisibility;
        public Visibility SecondPumpPanelVisibility
        {
            get => f_SecondPumpPanelVisibility;
            set => Set(ref f_SecondPumpPanelVisibility, value);
        }

        private bool f_IsRevereFirstPump;
        public bool IsRevereFirstPump
        {
            get => f_IsRevereFirstPump;
            set => Set(ref f_IsRevereFirstPump, value);
        }

        private bool f_IsRevereSecondPump;
        public bool IsRevereSecondPump
        {
            get => f_IsRevereSecondPump;
            set => Set(ref f_IsRevereSecondPump, value);
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

        private string f_PwrPortSelected;
        public string PwrPortSelected
        {
            get => f_PwrPortSelected;
            set => Set(ref f_PwrPortSelected, value);
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

        private string f_LabelPwrChannel0Bias;
        public string LabelPwrChannel0Bias
        {
            get => f_LabelPwrChannel0Bias;
            set => Set(ref f_LabelPwrChannel0Bias, value);
        }

        private int f_PwrCh0Mode;
        public int PwrCh0Mode
        {
            get => f_PwrCh0Mode;
            set
            {
                Set(ref f_PwrCh0Mode, value);
                LabelPwrChannel0Bias = value == 1 ? Properties.Resources.LabelElectricFlow : Properties.Resources.LabelOffsetVoltage;
            }
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

        private string f_LabelPwrChannel1Bias;
        public string LabelPwrChannel1Bias
        {
            get => f_LabelPwrChannel1Bias;
            set => Set(ref f_LabelPwrChannel1Bias, value);
        }

        private int f_PwrCh1Mode;
        public int PwrCh1Mode
        {
            get => f_PwrCh1Mode;
            set
            {
                Set(ref f_PwrCh1Mode, value);
                LabelPwrChannel1Bias = value == 1 ? Properties.Resources.LabelElectricFlow : Properties.Resources.LabelOffsetVoltage;
            }
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

        private string f_LabelPwrChannel2Bias;
        public string LabelPwrChannel2Bias
        {
            get => f_LabelPwrChannel2Bias;
            set => Set(ref f_LabelPwrChannel2Bias, value);
        }

        private int f_PwrCh2Mode;
        public int PwrCh2Mode
        {
            get => f_PwrCh2Mode;
            set
            {
                Set(ref f_PwrCh2Mode, value);
                LabelPwrChannel2Bias = value == 1 ? Properties.Resources.LabelElectricFlow : Properties.Resources.LabelOffsetVoltage;
            }
        }

        private int f_PwrCh2Bias;
        public int PwrCh2Bias
        {
            get => f_PwrCh2Bias;
            set => Set(ref f_PwrCh2Bias, value);
        }

        private int f_PwrCh2Amplitude;
        public int PwrCh2Amplitude
        {
            get => f_PwrCh2Amplitude;
            set => Set(ref f_PwrCh2Amplitude, value);
        }

        private int f_PwrCh2Freq;
        public int PwrCh2Freq
        {
            get => f_PwrCh2Freq;
            set => Set(ref f_PwrCh2Freq, value);
        }

        private int f_PwrCh2Duty;
        public int PwrCh2Duty
        {
            get => f_PwrCh2Duty;
            set => Set(ref f_PwrCh2Duty, value);
        }

        private int f_PwrCh2Phase;
        public int PwrCh2Phase
        {
            get => f_PwrCh2Phase;
            set => Set(ref f_PwrCh2Phase, value);
        }

        private int f_PwrCh2MaxVoltage;
        public int PwrCh2MaxVoltage
        {
            get => f_PwrCh2MaxVoltage;
            set => Set(ref f_PwrCh2MaxVoltage, value);
        }

        private int f_PwrCh2MaxAmps;
        public int PwrCh2MaxAmps
        {
            get => f_PwrCh2MaxAmps;
            set => Set(ref f_PwrCh2MaxAmps, value);
        }

        private bool f_PwrSwitchCh3;
        public bool PwrSwitchCh3
        {
            get => f_PwrSwitchCh3;
            set => Set(ref f_PwrSwitchCh3, value);
        }

        private string f_LabelPwrChannel3Bias;
        public string LabelPwrChannel3Bias
        {
            get => f_LabelPwrChannel3Bias;
            set => Set(ref f_LabelPwrChannel3Bias, value);
        }

        private int f_PwrCh3Mode;
        public int PwrCh3Mode
        {
            get => f_PwrCh3Mode;
            set
            {
                Set(ref f_PwrCh3Mode, value);
                LabelPwrChannel3Bias = value == 1 ? Properties.Resources.LabelElectricFlow : Properties.Resources.LabelOffsetVoltage;
            }
        }

        private string f_LabelPwrChannel4Bias;
        public string LabelPwrChannel4Bias
        {
            get => f_LabelPwrChannel4Bias;
            set => Set(ref f_LabelPwrChannel4Bias, value);
        }

        private int f_PwrCh4Mode;
        public int PwrCh4Mode
        {
            get => f_PwrCh4Mode;
            set
            {
                Set(ref f_PwrCh4Mode, value);
                LabelPwrChannel4Bias = value == 1 ? Properties.Resources.LabelElectricFlow : Properties.Resources.LabelOffsetVoltage;
            }
        }

        private int f_PwrCh3Bias;
        public int PwrCh3Bias
        {
            get => f_PwrCh3Bias;
            set => Set(ref f_PwrCh3Bias, value);
        }

        private int f_PwrCh3Amplitude;
        public int PwrCh3Amplitude
        {
            get => f_PwrCh3Amplitude;
            set => Set(ref f_PwrCh3Amplitude, value);
        }

        private int f_PwrCh3Freq;
        public int PwrCh3Freq
        {
            get => f_PwrCh3Freq;
            set => Set(ref f_PwrCh3Freq, value);
        }

        private int f_PwrCh3Duty;
        public int PwrCh3Duty
        {
            get => f_PwrCh3Duty;
            set => Set(ref f_PwrCh3Duty, value);
        }

        private int f_PwrCh3Phase;
        public int PwrCh3Phase
        {
            get => f_PwrCh3Phase;
            set => Set(ref f_PwrCh3Phase, value);
        }

        private int f_PwrCh3MaxVoltage;
        public int PwrCh3MaxVoltage
        {
            get => f_PwrCh3MaxVoltage;
            set => Set(ref f_PwrCh3MaxVoltage, value);
        }

        private int f_PwrCh3MaxAmps;
        public int PwrCh3MaxAmps
        {
            get => f_PwrCh3MaxAmps;
            set => Set(ref f_PwrCh3MaxAmps, value);
        }

        private bool f_PwrSwitchCh4;
        public bool PwrSwitchCh4
        {
            get => f_PwrSwitchCh4;
            set => Set(ref f_PwrSwitchCh4, value);
        }

        private int f_PwrCh4Bias;
        public int PwrCh4Bias
        {
            get => f_PwrCh4Bias;
            set => Set(ref f_PwrCh4Bias, value);
        }

        private int f_PwrCh4Amplitude;
        public int PwrCh4Amplitude
        {
            get => f_PwrCh4Amplitude;
            set => Set(ref f_PwrCh4Amplitude, value);
        }

        private int f_PwrCh4Freq;
        public int PwrCh4Freq
        {
            get => f_PwrCh4Freq;
            set => Set(ref f_PwrCh4Freq, value);
        }

        private int f_PwrCh4Duty;
        public int PwrCh4Duty
        {
            get => f_PwrCh4Duty;
            set => Set(ref f_PwrCh4Duty, value);
        }

        private int f_PwrCh4Phase;
        public int PwrCh4Phase
        {
            get => f_PwrCh4Phase;
            set => Set(ref f_PwrCh4Phase, value);
        }

        private int f_PwrCh4MaxVoltage;
        public int PwrCh4MaxVoltage
        {
            get => f_PwrCh4MaxVoltage;
            set => Set(ref f_PwrCh4MaxVoltage, value);
        }

        private int f_PwrCh4MaxAmps;
        public int PwrCh4MaxAmps
        {
            get => f_PwrCh4MaxAmps;
            set => Set(ref f_PwrCh4MaxAmps, value);
        }

        private int f_PwrSwitchCh5;
        public int PwrSwitchCh5
        {
            get => f_PwrSwitchCh5;
            set => Set(ref f_PwrSwitchCh5, value);
        }

        private string f_LabelPwrChannel5Bias;
        public string LabelPwrChannel5Bias
        {
            get => f_LabelPwrChannel5Bias;
            set => Set(ref f_LabelPwrChannel5Bias, value);
        }

        private int f_PwrCh5Mode;
        public int PwrCh5Mode
        {
            get => f_PwrCh5Mode;
            set { 
                Set(ref f_PwrCh5Mode, value);
                LabelPwrChannel5Bias = value == 1 ? Properties.Resources.LabelElectricFlow : Properties.Resources.LabelOffsetVoltage;
            }
        }

        private int f_PwrCh5Bias;
        public int PwrCh5Bias
        {
            get => f_PwrCh5Bias;
            set => Set(ref f_PwrCh5Bias, value);
        }

        private int f_PwrCh5Amplitude;
        public int PwrCh5Amplitude
        {
            get => f_PwrCh5Amplitude;
            set => Set(ref f_PwrCh5Amplitude, value);
        }

        private int f_PwrCh5Freq;
        public int PwrCh5Freq
        {
            get => f_PwrCh5Freq;
            set => Set(ref f_PwrCh5Freq, value);
        }

        private int f_PwrCh5Duty;
        public int PwrCh5Duty
        {
            get => f_PwrCh5Duty;
            set => Set(ref f_PwrCh5Duty, value);
        }

        private int f_PwrCh5Phase;
        public int PwrCh5Phase
        {
            get => f_PwrCh5Phase;
            set => Set(ref f_PwrCh5Phase, value);
        }

        private int f_PwrCh5MaxVoltage;
        public int PwrCh5MaxVoltage
        {
            get => f_PwrCh5MaxVoltage;
            set => Set(ref f_PwrCh5MaxVoltage, value);
        }

        private int f_PwrCh5MaxAmps;
        public int PwrCh5MaxAmps
        {
            get => f_PwrCh5MaxAmps;
            set => Set(ref f_PwrCh5MaxAmps, value);
        }

        private bool f_IsPumpsActive;
        public bool IsPumpsActive
        {
            get => f_IsPumpsActive;
            set
            {
                Set(ref f_IsPumpsActive, value);
                if (value) IsConfocalActive = value;
            }
        }

        private bool f_IsConfocalActive;
        public bool IsConfocalActive
        {
            get => f_IsConfocalActive;
            set => Set(ref f_IsConfocalActive, value);
        }
        #endregion

        #region Collections
        public ObservableCollection<ClassHelpers.LogItem> LogCollection { get; }
        public ObservableCollection<string> IncomingPumpPortCollection { get; set; }
        public ObservableCollection<string> PowerSupplyTypes { get; set; }
        public ObservableCollection<string> OutloginPumpPortCollection { get; set; }
        public ObservableCollection<string> LaserPortCollection { get; set; }
        public ObservableCollection<string> PiroPortCollection { get; set; }
        public ObservableCollection<string> PwrPortCollection { get; set; }
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
        public static string LabelChannel3 => Properties.Resources.LabelChannel3;
        public static string LabelChannel4 => Properties.Resources.LabelChannel4;
        public static string LabelChannel5 => Properties.Resources.LabelChannel5;
        public static string LabelPwrPort => Properties.Resources.LabelPwrPort;
        public static string LabelPumpOut => Properties.Resources.LabelPumpOut;
        public static string LabelConfocalActive => Properties.Resources.LabelConfocalActive;
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
            PiroPortSelected = Properties.Settings.Default.PwrPortSelected;
            PwrPortCollection = new ObservableCollection<string>(new ClassHelpers.PortList().GetPortList(PiroPortSelected));
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
            PwrCh2Amplitude = Properties.Settings.Default.PwrCh2Amplitude;
            PwrCh2Freq = Properties.Settings.Default.PwrCh2Freq;
            PwrCh2Duty = Properties.Settings.Default.PwrCh2Duty;
            PwrCh2Phase = Properties.Settings.Default.PwrCh2Phase;
            PwrCh2MaxVoltage = Properties.Settings.Default.PwrCh2MaxVoltage;
            PwrCh2MaxAmps = Properties.Settings.Default.PwrCh2MaxAmps;
            PwrCh3Bias = Properties.Settings.Default.PwrCh3Bias;
            PwrCh3Amplitude = Properties.Settings.Default.PwrCh3Amplitude;
            PwrCh3Freq = Properties.Settings.Default.PwrCh3Freq;
            PwrCh3Duty = Properties.Settings.Default.PwrCh3Duty;
            PwrCh3Phase = Properties.Settings.Default.PwrCh3Phase;
            PwrCh3MaxVoltage = Properties.Settings.Default.PwrCh3MaxVoltage;
            PwrCh3MaxAmps = Properties.Settings.Default.PwrCh3MaxAmps;
            PwrCh3Mode = Properties.Settings.Default.PwrCh3Mode;
            PwrCh4Mode = Properties.Settings.Default.PwrCh4Mode;
            PwrCh4Bias = Properties.Settings.Default.PwrCh4Bias;
            PwrCh4Amplitude = Properties.Settings.Default.PwrCh4Amplitude;
            PwrCh4Freq = Properties.Settings.Default.PwrCh4Freq;
            PwrCh4Duty = Properties.Settings.Default.PwrCh4Duty;
            PwrCh4Phase = Properties.Settings.Default.PwrCh4Phase;
            PwrCh4MaxVoltage = Properties.Settings.Default.PwrCh4MaxVoltage;
            PwrCh4MaxAmps = Properties.Settings.Default.PwrCh4MaxAmps;
            PwrCh5Mode = Properties.Settings.Default.PwrCh5Mode;
            PwrCh5Bias = Properties.Settings.Default.PwrCh5Bias;
            PwrCh5Amplitude = Properties.Settings.Default.PwrCh5Amplitude;
            PwrCh5Freq = Properties.Settings.Default.PwrCh5Freq;
            PwrCh5Duty = Properties.Settings.Default.PwrCh5Duty;
            PwrCh5Phase = Properties.Settings.Default.PwrCh5Phase;
            PwrCh5MaxVoltage = Properties.Settings.Default.PwrCh5MaxVoltage;
            PwrCh5MaxAmps = Properties.Settings.Default.PwrCh5MaxAmps;
            IsRevereFirstPump = Properties.Settings.Default.IsRevereFirstPump;
            IsRevereSecondPump = Properties.Settings.Default.IsRevereSecondPump;
            //init command area
            QuitCommand = new LambdaCommand(OnQuitApp);
            MinimizedCommand = new LambdaCommand(OnMinimizedCommandExecute);
            MaximizedCommand = new LambdaCommand(OnMaximizedCommandExecute);
            NormalizeCommand = new LambdaCommand(OnMaximizedCommandExecute);
            StandartSizeCommand = new LambdaCommand(OnStandartSizeCommand);
            //Drivers area
            f_ConfocalDriver = new ConfocalDriver();
            f_ConfocalDriver.ResievedDataEvent += (DistMeasureRes xData) => { ConfocalLevel = xData.Dist; };
            f_ConfocalDriver.SetLogMessage += AddLogMessage;

            f_PumpDriver = new PumpDriver();
            f_PumpDriver.SetLogMessage += AddLogMessage;

            AddLogMessage("Application Started");
            //port init
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
            Properties.Settings.Default.PwrCh2Amplitude = PwrCh2Amplitude;
            Properties.Settings.Default.PwrCh2Freq = PwrCh2Freq;
            Properties.Settings.Default.PwrCh2Duty = PwrCh2Duty;
            Properties.Settings.Default.PwrCh2Phase = PwrCh2Phase;
            Properties.Settings.Default.PwrCh2MaxVoltage = PwrCh2MaxVoltage;
            Properties.Settings.Default.PwrCh2MaxAmps = PwrCh2MaxAmps;
            Properties.Settings.Default.PwrCh3Bias = PwrCh3Bias;
            Properties.Settings.Default.PwrCh3Amplitude = PwrCh3Amplitude;
            Properties.Settings.Default.PwrCh3Freq = PwrCh3Freq;
            Properties.Settings.Default.PwrCh3Duty = PwrCh3Duty;
            Properties.Settings.Default.PwrCh3Phase = PwrCh3Phase;
            Properties.Settings.Default.PwrCh3MaxVoltage = PwrCh3MaxVoltage;
            Properties.Settings.Default.PwrCh3MaxAmps = PwrCh3MaxAmps;
            Properties.Settings.Default.PwrCh3Mode = PwrCh3Mode;
            Properties.Settings.Default.PwrCh4Mode = PwrCh4Mode;
            Properties.Settings.Default.PwrCh4Bias = PwrCh4Bias;
            Properties.Settings.Default.PwrCh4Amplitude = PwrCh4Amplitude;
            Properties.Settings.Default.PwrCh4Freq = PwrCh4Freq;
            Properties.Settings.Default.PwrCh4Duty = PwrCh4Duty;
            Properties.Settings.Default.PwrCh4Phase = PwrCh4Phase;
            Properties.Settings.Default.PwrCh4MaxVoltage = PwrCh4MaxVoltage;
            Properties.Settings.Default.PwrCh4MaxAmps = PwrCh4MaxAmps;
            Properties.Settings.Default.PwrCh5Mode = PwrCh5Mode;
            Properties.Settings.Default.PwrCh5Bias = PwrCh5Bias;
            Properties.Settings.Default.PwrCh5Amplitude = PwrCh5Amplitude;
            Properties.Settings.Default.PwrCh5Freq = PwrCh5Freq;
            Properties.Settings.Default.PwrCh5Duty = PwrCh5Duty;
            Properties.Settings.Default.PwrCh5Phase = PwrCh5Phase;
            Properties.Settings.Default.PwrCh5MaxVoltage = PwrCh5MaxVoltage;
            Properties.Settings.Default.PwrCh5MaxAmps = PwrCh5MaxAmps;
            Properties.Settings.Default.IsRevereFirstPump = IsRevereFirstPump;
            Properties.Settings.Default.IsRevereSecondPump = IsRevereSecondPump;
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
