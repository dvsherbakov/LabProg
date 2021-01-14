using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Windows;
using System.Windows.Input;
using LabControl.ClassHelpers;
using LabControl.DataModels;
using LabControl.LogicModels;
using LabControl.Properties;

namespace LabControl.ViewModels
{
    internal class MainModel : ViewModel
    {
        private readonly ApplicationContext f_DbContext;
        #region drivers
        private readonly PumpDriver f_PumpDriver;
        private readonly ConfocalDriver f_ConfocalDriver;
        private readonly PwrDriver f_PwrDriver;
        private readonly LaserDriver f_LaserDriver;
        private readonly PyroDriver f_PyroDriver;
        #endregion

        #region ModelFields
        private WindowState f_CurWindowState;
        public WindowState CurWindowState
        {
            get => f_CurWindowState;
            set => Set(ref f_CurWindowState, value);
        }

        public static string WindowTitle => Resources.MainWindowTitle;

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

        private bool f_IsPumpPortsConnect;
        public bool IsPumpPortsConnect
        {
            get => f_IsPumpPortsConnect;
            set
            {
                Set(ref f_IsPumpPortsConnect, value);
                if (value)
                    f_PumpDriver.ConnectToPorts();
                else
                    f_PumpDriver.Disconnect();
            }
        }

        private bool f_IsTwoPump;
        public bool IsTwoPump
        {
            get => f_IsTwoPump;
            set
            {
                LabelPumpCount = value ? Resources.LabelTwoPump : Resources.LabelOnePump;
                SecondPumpPanelVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                LabelPump = value ? Resources.LabelPumpIn : Resources.LabelPump;
                f_PumpDriver?.TogleTwoPump(value);
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
            set
            {
                Set(ref f_ConfocalLevelSetter, value);
                if (f_PumpDriver != null) f_PumpDriver.SetRequiredLvl(value);
            }
        }

        private string f_IncomingPumpPortSelected;
        public string IncomingPumpPortSelected
        {
            get => f_IncomingPumpPortSelected;
            set
            {
                Set(ref f_IncomingPumpPortSelected, value);
                if (f_PumpDriver != null) f_PumpDriver.PortStrInput = value;
            }
        }

        private string f_OutcomingPumpPortSelected;
        public string OutloginPumpPortSelected
        {
            get => f_OutcomingPumpPortSelected;
            set
            {
                Set(ref f_OutcomingPumpPortSelected, value);
                if (f_PumpDriver != null) f_PumpDriver.PortStrOutput = value;
            }
        }

        private string f_IncomingPumpSpeed;
        public string IncomingPumpSpeed
        {
            get => f_IncomingPumpSpeed;
            set => Set(ref f_IncomingPumpSpeed, value);
        }

        private string f_OutcomingPumpSpeed;
        public string OutcomingPumpSpeed
        {
            get => f_OutcomingPumpSpeed;
            set => Set(ref f_OutcomingPumpSpeed, value);
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

        private int f_LaserPowerHistorySelectedItem;
        public int LaserPowerHistorySelectedItem
        {
            get => f_LaserPowerHistorySelectedItem;
            set
            {
                Set(ref f_LaserPowerHistorySelectedItem, value);
                LaserPowerSetter = value;
            }
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

        private bool f_IsLaserPortConnected;
        public bool IsLaserPortConnected
        {
            get => f_IsLaserPortConnected;
            set
            {
                Set(ref f_IsLaserPortConnected, value);
                if (value)
                {
                    f_LaserDriver.ConnectToPort();
                    f_LaserDriver.SetLaserType(LaserTypeSelectedIndex);

                }
                else f_LaserDriver.Disconnect();
            }
        }

        private bool f_IsLaserEmit;
        public bool IsLaserEmit
        {
            get => f_IsLaserEmit;
            set
            {
                if (!IsLaserPortConnected) IsLaserPortConnected = true;
                Set(ref f_IsLaserEmit, value);
                if (f_LaserDriver != null) f_LaserDriver.EmitOn(value);
            }
        }

        private string f_LaserPortSelected;
        public string LaserPortSelected
        {
            get => f_LaserPortSelected;
            set
            {
                Set(ref f_LaserPortSelected, value);
                if (f_LaserDriver != null) f_LaserDriver.PortString = value;
            }
        }

        private string f_PyroPortSelected;
        public string PyroPortSelected
        {
            get => f_PyroPortSelected;
            set
            {
                Set(ref f_PyroPortSelected, value);
                if (f_PyroDriver != null) f_PyroDriver.PortStr = value;
            }
        }

        private string f_PwrPortSelected;
        public string PwrPortSelected
        {
            get => f_PwrPortSelected;
            set
            {
                Set(ref f_PwrPortSelected, value);
                if (f_PwrDriver != null) f_PwrDriver.PortStr = value;
            }
        }

        private int f_LaserTypeSelectedIndex;
        public int LaserTypeSelectedIndex
        {
            get => f_LaserTypeSelectedIndex;
            set
            {
                Set(ref f_LaserTypeSelectedIndex, value);
                if (f_LaserDriver != null) f_LaserDriver.SetLaserType(value);
            }
        }

        private bool f_IsPwrPortConnect;
        public bool IsPwrPortConnect
        {
            get => f_IsPwrPortConnect;
            set
            {
                Set(ref f_IsPwrPortConnect, value);
                f_PwrDriver.ConnectToPort();
            }
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
                LabelPwrChannel0Bias = value == 1 ? Resources.LabelElectricFlow : Resources.LabelOffsetVoltage;
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
                LabelPwrChannel1Bias = value == 1 ? Resources.LabelElectricFlow : Resources.LabelOffsetVoltage;
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
                LabelPwrChannel2Bias = value == 1 ? Resources.LabelElectricFlow : Resources.LabelOffsetVoltage;
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
                LabelPwrChannel3Bias = value == 1 ? Resources.LabelElectricFlow : Resources.LabelOffsetVoltage;
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
                LabelPwrChannel4Bias = value == 1 ? Resources.LabelElectricFlow : Resources.LabelOffsetVoltage;
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
            set
            {
                Set(ref f_PwrCh5Mode, value);
                LabelPwrChannel5Bias = value == 1 ? Resources.LabelElectricFlow : Resources.LabelOffsetVoltage;
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
                if (value) IsConfocalActive = true;
                f_PumpDriver?.SetPumpActive(value);
            }
        }

        private bool f_IsConfocalActive;
        public bool IsConfocalActive
        {
            get => f_IsConfocalActive;
            set
            {
                Set(ref f_IsConfocalActive, value);
                f_ConfocalDriver?.SetMeasuredActive(value);
            }
        }

        private double[] f_ConfocalLog;
        public double[] ConfocalLog
        {
            get => f_ConfocalLog;
            set => Set(ref f_ConfocalLog, value);
        }

        private bool f_IsPyroPortConnected;
        public bool IsPyroPortConnected
        {
            get => f_IsPyroPortConnected;
            set
            {
                Set(ref f_IsPyroPortConnected, value);
                if (value) f_PyroDriver.ConnectToPort();
                else f_PyroDriver.Disconnect();
            }
        }

        private bool f_IsPyroActive;
        public bool IsPyroActive
        {
            get => f_IsPyroActive;
            set => Set(ref f_IsPyroActive, value);
        }
        #endregion

        #region Collections
        public ObservableCollection<LogItem> LogCollection { get; }
        public ObservableCollection<string> IncomingPumpPortCollection { get; set; }
        public ObservableCollection<string> PowerSupplyTypes { get; set; }
        public ObservableCollection<string> OutcomingPumpPortCollection { get; set; }
        public ObservableCollection<string> LaserPortCollection { get; set; }
        public ObservableCollection<string> PyroPortCollection { get; set; }
        public ObservableCollection<string> PwrPortCollection { get; set; }
        public ObservableCollection<int> LaserHistoryCollection { get; set; }
        #endregion

        #region StaticLabels
        public static string LabelOn => Resources.LabelOn;
        public static string LogMessageHeader => Resources.LogHeaderColumn1Name;
        public static string LabelPumpOperation => Resources.PumpOperationTitle;
        public static string LabelConfocalData => Resources.LabelConfocalData;
        public static string LabelConfocalSetter => Resources.LabelConfocalSetter;
        public static string LabelPumpActive => Resources.LabelPumpActive;
        public static string LabelPortConnection => Resources.LabelPortConnection;
        public static string LabelSettings => Resources.LabelSettings;
        public static string LabelInputPumpPort => Resources.LabelInputPumpPort;
        public static string LabelOutputPumpPort => Resources.LabelOutputPumpPort;
        public static string LaserOperationTitle => Resources.LaserOpeprationTitle;
        public static string LabelEmitLaser => Resources.LabelEmmitLaser;
        public static string LabelCurrentPower => Resources.LabelCurrentPower;
        public static string LabelSetterPower => Resources.LabelSetterPower;
        public static string LabelCurrentTemperature => Resources.LabelCurrentTemperature;
        public static string LabelLaserPort => Resources.LabelLaserPort;
        public static string LabelPyroPort => Resources.LabelPyroPort;
        public static string LabelLaserType => Resources.LabelLaserType;
        public static string PowerSupplyTitle => Resources.PowerSuplyTitle;
        public static string PyroOperationTitle => Resources.PyroOperationTitle;
        public static string LabelUfLed1 => Resources.LabelUfLed1;
        public static string LabelUfLed2 => Resources.LabelUfLed2;
        public static string LabelChannelsSwitch => Resources.LabelChanelsSwitch;
        public static string LabelPwrChannelMode => Resources.LabelPwrChannelMode;
        // public static string LabelPwrChannelBias => Resources.LabelPwrChannelBias;
        public static string LabelPwrChannelAmplitude => Resources.LabelPwrChannelAmplitude;
        public static string LabelPwrChannelFreq => Resources.LabelPwrChannelFreq;
        public static string LabelPwrChannelPhase => Resources.LabelPwrChannelPhase;
        public static string LabelChannel0 => Resources.LabelChannel0;
        public static string LabelChannel1 => Resources.LabelChannel1;
        public static string LabelIkLed1 => Resources.LabelIkLed1;
        public static string LabelIkLed2 => Resources.LabelIkLed2;
        public static string LabelPwrChannelDuty => Resources.LabelPwrChannelDuty;
        public static string LabelPwrMaxVoltage => Resources.LabelPwrMaxVoltage;
        public static string LabelPwrMaxAmps => Resources.LabelPwrMaxAmps;
        public static string LabelRead => Resources.LabelRead;
        public static string LabelWrite => Resources.LabelWrite;
        public static string LabelChannel2 => Resources.LabelChannel2;
        public static string LabelChannel3 => Resources.LabelChannel3;
        public static string LabelChannel4 => Resources.LabelChannel4;
        public static string LabelChannel5 => Resources.LabelChannel5;
        public static string LabelPwrPort => Resources.LabelPwrPort;
        public static string LabelPumpOut => Resources.LabelPumpOut;
        public static string LabelConfocalActive => Resources.LabelConfocalActive;
        public static string LabelMonitoring => Resources.LabelMonitoring;
        public static string LabelReverse => Resources.LabelReverse;
        public static string LabelMicroCompressor => Resources.LabelMicroCompressor;
        public static string LabelGlassHeating => Resources.LabelGlassHeating;
        public static string LaserPowerHistory => Resources.LaserPowerHistory;

        #endregion


        #region Commands
        public ICommand QuitCommand { get; }
        public ICommand MinimizedCommand { get; }
        public ICommand MaximizedCommand { get; }
        public ICommand StandardSizeCommand { get; }
        public ICommand SetLaserPwrCommand { get; }
        #endregion
        public MainModel()
        {
            // Data context
            f_DbContext = new ApplicationContext();
            f_DbContext.Logs.Load();
            // init collections
            ConfocalLog = new[] { 0d, .40d, .3d };
            LogCollection = new ObservableCollection<LogItem>();
            IncomingPumpPortSelected = Settings.Default.IncomingPumpPortSelected;
            PowerSupplyTypes = new ObservableCollection<string>(new PowerSuplyTupesList().GetTypesList());
            IncomingPumpPortCollection = new ObservableCollection<string>(new PortList().GetPortList(IncomingPumpPortSelected));
            OutloginPumpPortSelected = Settings.Default.OutloginPumpPortSelected;
            OutcomingPumpPortCollection = new ObservableCollection<string>(new PortList().GetPortList(OutloginPumpPortSelected));
            LaserPortSelected = Settings.Default.LaserPortSelected;
            LaserPortCollection = new ObservableCollection<string>(new PortList().GetPortList(LaserPortSelected));
            PyroPortSelected = Settings.Default.PyroPortSelected;
            PyroPortCollection = new ObservableCollection<string>(new PortList().GetPortList(PyroPortSelected));
            PwrPortSelected = Settings.Default.PwrPortSelected;
            PwrPortCollection = new ObservableCollection<string>(new PortList().GetPortList(PyroPortSelected));
            LaserHistoryCollection = new ObservableCollection<int>() { 100, 200, 300 };
            CurWindowState = WindowState.Normal;
            //load params from settings
            WindowHeight = Settings.Default.WindowHeight == 0 ? 550 : Settings.Default.WindowHeight;
            WindowWidth = Settings.Default.WindowWidth == 0 ? 850 : Settings.Default.WindowWidth;
            IsTwoPump = Settings.Default.IsTwoPump;
            ConfocalLevelSetter = Settings.Default.ConfocalLevelSetter;
            LaserPowerSetter = Settings.Default.LaserPowerSetter;

            PwrCh0Mode = Settings.Default.PwrCh0Mode;
            PwrCh0Bias = Settings.Default.PwrCh0Bias;
            PwrCh0Amplitude = Settings.Default.PwrCh0Amplitude;
            PwrCh0Freq = Settings.Default.PwrCh0Freq;
            PwrCh0Duty = Settings.Default.PwrCh0Duty;
            PwrCh0Phase = Settings.Default.PwrCh0Phase;
            PwrCh0MaxVoltage = Settings.Default.PwrCh0MaxVoltage;
            PwrCh0MaxAmps = Settings.Default.PwrCh0MaxAmps;
            PwrCh1Mode = Settings.Default.PwrCh1Mode;
            PwrCh1Mode = Settings.Default.PwrCh1Mode;
            PwrCh1Bias = Settings.Default.PwrCh1Bias;
            PwrCh1Amplitude = Settings.Default.PwrCh1Amplitude;
            PwrCh1Freq = Settings.Default.PwrCh1Freq;
            PwrCh1Duty = Settings.Default.PwrCh1Duty;
            PwrCh1Phase = Settings.Default.PwrCh1Phase;
            PwrCh1MaxVoltage = Settings.Default.PwrCh1MaxVoltage;
            PwrCh1MaxAmps = Settings.Default.PwrCh1MaxAmps;
            PwrCh2Mode = Settings.Default.PwrCh2Mode;
            PwrCh2Bias = Settings.Default.PwrCh2Bias;
            PwrCh2Amplitude = Settings.Default.PwrCh2Amplitude;
            PwrCh2Freq = Settings.Default.PwrCh2Freq;
            PwrCh2Duty = Settings.Default.PwrCh2Duty;
            PwrCh2Phase = Settings.Default.PwrCh2Phase;
            PwrCh2MaxVoltage = Settings.Default.PwrCh2MaxVoltage;
            PwrCh2MaxAmps = Settings.Default.PwrCh2MaxAmps;
            PwrCh3Bias = Settings.Default.PwrCh3Bias;
            PwrCh3Amplitude = Settings.Default.PwrCh3Amplitude;
            PwrCh3Freq = Settings.Default.PwrCh3Freq;
            PwrCh3Duty = Settings.Default.PwrCh3Duty;
            PwrCh3Phase = Settings.Default.PwrCh3Phase;
            PwrCh3MaxVoltage = Settings.Default.PwrCh3MaxVoltage;
            PwrCh3MaxAmps = Settings.Default.PwrCh3MaxAmps;
            PwrCh3Mode = Settings.Default.PwrCh3Mode;
            PwrCh4Mode = Settings.Default.PwrCh4Mode;
            PwrCh4Bias = Settings.Default.PwrCh4Bias;
            PwrCh4Amplitude = Settings.Default.PwrCh4Amplitude;
            PwrCh4Freq = Settings.Default.PwrCh4Freq;
            PwrCh4Duty = Settings.Default.PwrCh4Duty;
            PwrCh4Phase = Settings.Default.PwrCh4Phase;
            PwrCh4MaxVoltage = Settings.Default.PwrCh4MaxVoltage;
            PwrCh4MaxAmps = Settings.Default.PwrCh4MaxAmps;
            PwrCh5Mode = Settings.Default.PwrCh5Mode;
            PwrCh5Bias = Settings.Default.PwrCh5Bias;
            PwrCh5Amplitude = Settings.Default.PwrCh5Amplitude;
            PwrCh5Freq = Settings.Default.PwrCh5Freq;
            PwrCh5Duty = Settings.Default.PwrCh5Duty;
            PwrCh5Phase = Settings.Default.PwrCh5Phase;
            PwrCh5MaxVoltage = Settings.Default.PwrCh5MaxVoltage;
            PwrCh5MaxAmps = Settings.Default.PwrCh5MaxAmps;
            IsRevereFirstPump = Settings.Default.IsRevereFirstPump;
            IsRevereSecondPump = Settings.Default.IsRevereSecondPump;
            //init command area
            QuitCommand = new LambdaCommand(OnQuitApp);
            MinimizedCommand = new LambdaCommand(OnMinimizedCommandExecute);
            MaximizedCommand = new LambdaCommand(OnMaximizedCommandExecute);
            StandardSizeCommand = new LambdaCommand(OnStandardSizeCommand);
            SetLaserPwrCommand = new LambdaCommand(OnSetLaserPower);
            //Drivers area
            f_ConfocalDriver = new ConfocalDriver();
            f_ConfocalDriver.ObtainedDataEvent += SetUpMeasuredLevel;
            f_ConfocalDriver.SetLogMessage += AddLogMessage;

            f_PumpDriver = new PumpDriver();
            f_PumpDriver.SetLogMessage += AddLogMessage;
            f_PumpDriver.PortStrInput = Settings.Default.IncomingPumpPortSelected;
            f_PumpDriver.PortStrOutput = Settings.Default.OutloginPumpPortSelected;
            f_PumpDriver.TogleTwoPump(Settings.Default.IsTwoPump);
            f_PumpDriver.SetInputSpeed += SetIncomingPumpSpeedLabel;
            f_PumpDriver.SetOutputSpeed += SetOutcomingPumpSpeedLabel;
            f_PumpDriver.SetRequiredLvl(ConfocalLevelSetter);

            f_PwrDriver = new PwrDriver();
            f_PwrDriver.SetLogMessage += AddLogMessage;
            f_PwrDriver.PortStr = Settings.Default.PwrPortSelected;

            f_LaserDriver = new LaserDriver();
            f_LaserDriver.SetLogMessage += AddLogMessage;
            f_LaserDriver.PortString = Settings.Default.LaserPortSelected;
            LaserTypeSelectedIndex = Settings.Default.LaserTypeSelectedIndex;

            f_PyroDriver = new PyroDriver();
            f_PyroDriver.SetLogMessage += AddLogMessage;
            f_PyroDriver.PortStr = Settings.Default.PyroPortSelected;

            AddLogMessage("Application Started");
        }

        private void AddLogMessage(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LogCollection.Insert(0, new LogItem(DateTime.Now, message));
                f_DbContext.Logs.Add(new Log { Dt = DateTime.Now, Message = message, Code = 0 });
            });
        }

        private void OnQuitApp(object p)
        {
            f_DbContext.SaveChanges();
            Settings.Default.WindowHeight = WindowHeight;
            Settings.Default.WindowWidth = WindowWidth;
            Settings.Default.IsTwoPump = IsTwoPump;
            Settings.Default.ConfocalLevelSetter = ConfocalLevelSetter;
            Settings.Default.IncomingPumpPortSelected = IncomingPumpPortSelected;
            Settings.Default.OutloginPumpPortSelected = OutloginPumpPortSelected;
            Settings.Default.LaserPowerSetter = LaserPowerSetter;
            Settings.Default.LaserPortSelected = LaserPortSelected;
            Settings.Default.PyroPortSelected = PyroPortSelected;
            Settings.Default.PwrPortSelected = PwrPortSelected;
            Settings.Default.LaserTypeSelectedIndex = LaserTypeSelectedIndex;
            Settings.Default.PwrCh0Mode = PwrCh0Mode;
            Settings.Default.PwrCh0Bias = PwrCh0Bias;
            Settings.Default.PwrCh0Amplitude = PwrCh0Amplitude;
            Settings.Default.PwrCh0Freq = PwrCh0Freq;
            Settings.Default.PwrCh0Duty = PwrCh0Duty;
            Settings.Default.PwrCh0Phase = PwrCh0Phase;
            Settings.Default.PwrCh0MaxVoltage = PwrCh0MaxVoltage;
            Settings.Default.PwrCh0MaxAmps = PwrCh0MaxAmps;
            Settings.Default.PwrCh1Mode = PwrCh1Mode;
            Settings.Default.PwrCh1Bias = PwrCh1Bias;
            Settings.Default.PwrCh1Amplitude = PwrCh1Amplitude;
            Settings.Default.PwrCh1Freq = PwrCh1Freq;
            Settings.Default.PwrCh1Duty = PwrCh1Duty;
            Settings.Default.PwrCh1Phase = PwrCh1Phase;
            Settings.Default.PwrCh1MaxVoltage = PwrCh1MaxVoltage;
            Settings.Default.PwrCh1MaxAmps = PwrCh1MaxAmps;
            Settings.Default.PwrCh2Mode = PwrCh2Mode;
            Settings.Default.PwrCh2Bias = PwrCh2Bias;
            Settings.Default.PwrCh2Amplitude = PwrCh2Amplitude;
            Settings.Default.PwrCh2Freq = PwrCh2Freq;
            Settings.Default.PwrCh2Duty = PwrCh2Duty;
            Settings.Default.PwrCh2Phase = PwrCh2Phase;
            Settings.Default.PwrCh2MaxVoltage = PwrCh2MaxVoltage;
            Settings.Default.PwrCh2MaxAmps = PwrCh2MaxAmps;
            Settings.Default.PwrCh3Bias = PwrCh3Bias;
            Settings.Default.PwrCh3Amplitude = PwrCh3Amplitude;
            Settings.Default.PwrCh3Freq = PwrCh3Freq;
            Settings.Default.PwrCh3Duty = PwrCh3Duty;
            Settings.Default.PwrCh3Phase = PwrCh3Phase;
            Settings.Default.PwrCh3MaxVoltage = PwrCh3MaxVoltage;
            Settings.Default.PwrCh3MaxAmps = PwrCh3MaxAmps;
            Settings.Default.PwrCh3Mode = PwrCh3Mode;
            Settings.Default.PwrCh4Mode = PwrCh4Mode;
            Settings.Default.PwrCh4Bias = PwrCh4Bias;
            Settings.Default.PwrCh4Amplitude = PwrCh4Amplitude;
            Settings.Default.PwrCh4Freq = PwrCh4Freq;
            Settings.Default.PwrCh4Duty = PwrCh4Duty;
            Settings.Default.PwrCh4Phase = PwrCh4Phase;
            Settings.Default.PwrCh4MaxVoltage = PwrCh4MaxVoltage;
            Settings.Default.PwrCh4MaxAmps = PwrCh4MaxAmps;
            Settings.Default.PwrCh5Mode = PwrCh5Mode;
            Settings.Default.PwrCh5Bias = PwrCh5Bias;
            Settings.Default.PwrCh5Amplitude = PwrCh5Amplitude;
            Settings.Default.PwrCh5Freq = PwrCh5Freq;
            Settings.Default.PwrCh5Duty = PwrCh5Duty;
            Settings.Default.PwrCh5Phase = PwrCh5Phase;
            Settings.Default.PwrCh5MaxVoltage = PwrCh5MaxVoltage;
            Settings.Default.PwrCh5MaxAmps = PwrCh5MaxAmps;
            Settings.Default.IsRevereFirstPump = IsRevereFirstPump;
            Settings.Default.IsRevereSecondPump = IsRevereSecondPump;
            Settings.Default.Save();
            f_DbContext.Dispose();
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

        private void OnStandardSizeCommand(object sender)
        {
            WindowHeight = 470;
            WindowWidth = 630;
        }

        private void OnSetLaserPower(object sender)
        {
            f_LaserDriver.SetPower(LaserPowerSetter);
            if (!LaserHistoryCollection.Contains(LaserPowerSetter))
                LaserHistoryCollection.Insert(0, LaserPowerSetter);
        }

        private void SetUpMeasuredLevel(DistMeasureRes lvl)
        {
            ConfocalLevel = Math.Round(lvl.Dist, 5);
            f_PumpDriver?.SetMeasuredLevel(lvl);
            ConfocalLog = f_ConfocalDriver.GetLastFragment();
        }

        private void SetIncomingPumpSpeedLabel(string speed)
        {
            Application.Current.Dispatcher.Invoke(() => IncomingPumpSpeed = speed);
        }
        private void SetOutcomingPumpSpeedLabel(string speed)
        {
            Application.Current.Dispatcher.Invoke(() => OutcomingPumpSpeed = speed);
        }
    }
}
