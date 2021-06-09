using System;
using System.Collections.ObjectModel;
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
        private readonly ApplicationContext _fDbContext;
        #region drivers
        private readonly PumpDriver _fPumpDriver;
        private readonly ConfocalDriver _fConfocalDriver;
        private readonly PwrDriver _fPwrDriver;
        private readonly LaserDriver _fLaserDriver;
        private readonly PyroDriver _fPyroDriver;
        private readonly DispenserDriver _fDispenserDriver;
        private readonly PressurePumpDriver _fPressurePumpDriver;
        private readonly PressureSensorDriver _fPressureSensorDriver;
        #endregion

        #region ModelFields
        private WindowState _curWindowState;
        public WindowState CurWindowState
        {
            get => _curWindowState;
            set => Set(ref _curWindowState, value);
        }

        public static string WindowTitle => Resources.MainWindowTitle;

        private int _windowHeight;
        public int WindowHeight
        {
            get => _windowHeight;
            set => Set(ref _windowHeight, value);
        }

        private int _windowWidth;
        public int WindowWidth
        {
            get => _windowWidth;
            set => Set(ref _windowWidth, value);
        }

        private bool _isPumpPortsConnect;
        public bool IsPumpPortsConnect
        {
            get => _isPumpPortsConnect;
            set
            {
                _ = Set(ref _isPumpPortsConnect, value);
                if (value)
                    _fPumpDriver.ConnectToPorts();
                else
                    _fPumpDriver.Disconnect();
            }
        }

        private bool _isTwoPump;
        public bool IsTwoPump
        {
            get => _isTwoPump;
            set
            {
                LabelPumpCount = value ? Resources.LabelTwoPump : Resources.LabelOnePump;
                SecondPumpPanelVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                LabelPump = value ? Resources.LabelPumpIn : Resources.LabelPump;
                _fPumpDriver?.TogleTwoPump(value);
                _ = Set(ref _isTwoPump, value);
            }
        }

        private string _labelPump;
        public string LabelPump
        {
            get => _labelPump;
            set => Set(ref _labelPump, value);
        }

        private string _labelPumpCount;
        public string LabelPumpCount
        {
            get => _labelPumpCount;
            set => Set(ref _labelPumpCount, value);
        }

        private double _confocalLevel;
        public double ConfocalLevel
        {
            get => _confocalLevel;
            set => Set(ref _confocalLevel, value);
        }

        private double _confocalLevelSetter;
        public double ConfocalLevelSetter
        {
            get => _confocalLevelSetter;
            set
            {
                _ = Set(ref _confocalLevelSetter, value);
                _fPumpDriver?.SetRequiredLvl(value);
            }
        }

        private string _fIncomingPumpPortSelected;
        public string IncomingPumpPortSelected
        {
            get => _fIncomingPumpPortSelected;
            set
            {
                _ = Set(ref _fIncomingPumpPortSelected, value);
                if (_fPumpDriver != null) _fPumpDriver.PortStrInput = value;
            }
        }

        private string _outgoingPumpPortSelected;
        public string OutgoingPumpPortSelected
        {
            get => _outgoingPumpPortSelected;
            set
            {
                Set(ref _outgoingPumpPortSelected, value);
                if (_fPumpDriver != null) _fPumpDriver.PortStrOutput = value;
            }
        }

        private float _fPumpingSpeedSelected;
        public float PumpingSpeedSelected
        {
            get => _fPumpingSpeedSelected;
            set
            {
                _ = Set(ref _fPumpingSpeedSelected, value);
                if (_fPumpDriver != null) _fPumpDriver.PumpingSpeed = value;
            }
        }

        private string _incomingPumpSpeed;
        public string IncomingPumpSpeed
        {
            get => _incomingPumpSpeed;
            set => Set(ref _incomingPumpSpeed, value);
        }

        private string _outgoingPumpSpeed;
        public string OutgoingPumpSpeed
        {
            get => _outgoingPumpSpeed;
            set => Set(ref _outgoingPumpSpeed, value);
        }

        private int _currentLaserPower;
        public int CurrentLaserPower
        {
            get => _currentLaserPower;
            set => Set(ref _currentLaserPower, value);
        }

        private int _laserPowerSetter;
        public int LaserPowerSetter
        {
            get => _laserPowerSetter;
            set => Set(ref _laserPowerSetter, value);
        }

        private int _laserPowerHistorySelectedItem;
        public int LaserPowerHistorySelectedItem
        {
            get => _laserPowerHistorySelectedItem;
            set
            {
                _ = Set(ref _laserPowerHistorySelectedItem, value);
                LaserPowerSetter = value;
                OnSetLaserPower(null);
            }
        }

        private double _fCurrentTemperature;
        public double CurrentTemperature
        {
            get => _fCurrentTemperature;
            set => Set(ref _fCurrentTemperature, value);
        }

        private Visibility _fSecondPumpPanelVisibility;
        public Visibility SecondPumpPanelVisibility
        {
            get => _fSecondPumpPanelVisibility;
            set => Set(ref _fSecondPumpPanelVisibility, value);
        }

        private bool _fIsRevereFirstPump;
        public bool IsRevereFirstPump
        {
            get => _fIsRevereFirstPump;
            set => Set(ref _fIsRevereFirstPump, value);
        }

        private bool _fIsRevereSecondPump;
        public bool IsRevereSecondPump
        {
            get => _fIsRevereSecondPump;
            set => Set(ref _fIsRevereSecondPump, value);
        }

        private bool _isLaserPortConnected;
        public bool IsLaserPortConnected
        {
            get => _isLaserPortConnected;
            set
            {
                _ = Set(ref _isLaserPortConnected, value);
                if (value)
                {
                    _fLaserDriver.ConnectToPort();
                    _fLaserDriver.SetLaserType(LaserTypeSelectedIndex);

                }
                else _fLaserDriver.Disconnect();
            }
        }

        private bool _isLaserEmit;
        public bool IsLaserEmit
        {
            get => _isLaserEmit;
            set
            {
                if (!IsLaserPortConnected) IsLaserPortConnected = true;
                _ = Set(ref _isLaserEmit, value);
                _fLaserDriver?.EmitOn(value);
            }
        }

        private string _fLaserPortSelected;
        public string LaserPortSelected
        {
            get => _fLaserPortSelected;
            set
            {
                _ = Set(ref _fLaserPortSelected, value);
                if (_fLaserDriver != null) _fLaserDriver.PortString = value;
            }
        }

        private string _pyroPortSelected;
        public string PyroPortSelected
        {
            get => _pyroPortSelected;
            set
            {
                _ = Set(ref _pyroPortSelected, value);
                if (_fPyroDriver != null) _fPyroDriver.PortStr = value;
            }
        }

        private string _dispenserPortSelected;
        public string DispenserPortSelected
        {
            get => _dispenserPortSelected;
            set
            {
                _ = Set(ref _dispenserPortSelected, value);
                if (_fDispenserDriver != null) _fDispenserDriver.PortStr = value;
            }
        }

        private string _fAirSupportPortSelected;
        public string AirSupportPortSelected
        {
            get => _fAirSupportPortSelected;
            set => Set(ref _fAirSupportPortSelected, value);
        }

        private string _fPwrPortSelected;
        public string PwrPortSelected
        {
            get => _fPwrPortSelected;
            set
            {
                _ = Set(ref _fPwrPortSelected, value);
                if (_fPwrDriver != null) _fPwrDriver.PortStr = value;
            }
        }

        private string _fPressureSensorPortSelected;
        public string PressureSensorPortSelected
        {
            get => _fPressureSensorPortSelected;
            set
            {
                _ = Set(ref _fPressureSensorPortSelected, value);
                if (_fPressureSensorDriver != null) _fPressureSensorDriver.PortStr = value;
            }
        }

        private int _laserTypeSelectedIndex;
        public int LaserTypeSelectedIndex
        {
            get => _laserTypeSelectedIndex;
            set
            {
                _ = Set(ref _laserTypeSelectedIndex, value);
                _fLaserDriver?.SetLaserType(value);
            }
        }

        private readonly DataModels.PwrItem[] _pwrParams;

        private bool _fIsPwrPortConnect;
        public bool IsPwrPortConnect
        {
            get => _fIsPwrPortConnect;
            set
            {
                _ = Set(ref _fIsPwrPortConnect, value);
                _fPwrDriver.ConnectToPort();
            }
        }

        private bool _fPwrSwitchCh0;
        public bool PwrSwitchCh0
        {
            get => _fPwrSwitchCh0;
            set
            {
                _ = Set(ref _fPwrSwitchCh0, value);
                if (value) _fPwrDriver?.SetChannelOn(0); else _fPwrDriver?.SetChannelOff(0);
            }
        }

        private string _fLabelPwrChannel0Bias;
        public string LabelPwrChannel0Bias
        {
            get => _fLabelPwrChannel0Bias;
            set => Set(ref _fLabelPwrChannel0Bias, value);

        }

        private int _pwrCh0Mode;
        public int PwrCh0Mode
        {
            get => _pwrCh0Mode;
            set
            {
                _ = Set(ref _pwrCh0Mode, value);
                if (value == 1)
                {
                    LabelPwrChannel0Bias = Resources.LabelElectricFlow;
                    _fPwrDriver?.SetChannelOn(0);
                }
                else { LabelPwrChannel0Bias = Resources.LabelOffsetVoltage; }
                _pwrParams[0].Mode = value;
            }
        }

        private int _pwrCh0Bias;
        public int PwrCh0Bias
        {
            get => _pwrCh0Bias;
            set
            {
                _ = Set(ref _pwrCh0Bias, value);
                _pwrParams[0].Bias = value;
            }
        }

        private int _pwrCh0Amplitude;
        public int PwrCh0Amplitude
        {
            get => _pwrCh0Amplitude;
            set
            {
                _ = Set(ref _pwrCh0Amplitude, value);
                _pwrParams[0].Amplitude = value;
            }
        }

        private int _pwrCh0Duty;
        public int PwrCh0Duty
        {
            get => _pwrCh0Duty;
            set
            {
                _ = Set(ref _pwrCh0Duty, value);
                _pwrParams[0].Duty = value;
            }
        }

        private int _fPwrCh0Freq;
        public int PwrCh0Freq
        {
            get => _fPwrCh0Freq;
            set
            {
                Set(ref _fPwrCh0Freq, value);
                _pwrParams[0].Frequency = value;
            }
        }

        private int _fPwrCh0Phase;
        public int PwrCh0Phase
        {
            get => _fPwrCh0Phase;
            set
            {
                Set(ref _fPwrCh0Phase, value);
                _pwrParams[0].Phase = value;
            }
        }

        private int _fPwrCh0MaxAmps;
        public int PwrCh0MaxAmps
        {
            get => _fPwrCh0MaxAmps;
            set
            {
                Set(ref _fPwrCh0MaxAmps, value);
                _pwrParams[0].MaxAmps = value;
            }
        }

        private int _fPwrCh0MaxVoltage;
        public int PwrCh0MaxVoltage
        {
            get => _fPwrCh0MaxVoltage;
            set
            {
                Set(ref _fPwrCh0MaxVoltage, value);
                _pwrParams[0].MaxVolts = value;
            }
        }

        private string _fLabelPwrChannel1Bias;
        public string LabelPwrChannel1Bias
        {
            get => _fLabelPwrChannel1Bias;
            set => Set(ref _fLabelPwrChannel1Bias, value);
        }

        private int _fPwrCh1Mode;
        public int PwrCh1Mode
        {
            get => _fPwrCh1Mode;
            set
            {
                Set(ref _fPwrCh1Mode, value);
                if (value == 1)
                {
                    LabelPwrChannel1Bias = Resources.LabelElectricFlow;
                    _fPwrDriver?.SetChannelOn(1);
                }
                else
                {
                    LabelPwrChannel1Bias = Resources.LabelOffsetVoltage;
                }
                _pwrParams[1].Mode = value;
            }
        }

        private bool _fPwrSwitchCh1;
        public bool PwrSwitchCh1
        {
            get => _fPwrSwitchCh1;
            set
            {
                Set(ref _fPwrSwitchCh1, value);
                if (value) _fPwrDriver?.SetChannelOn(1); else _fPwrDriver?.SetChannelOff(1);
            }
        }

        private int _fPwrCh1Bias;
        public int PwrCh1Bias
        {
            get => _fPwrCh1Bias;
            set
            {
                Set(ref _fPwrCh1Bias, value);
                _pwrParams[1].Bias = value;
            }
        }

        private int _fPwrCh1Amplitude;
        public int PwrCh1Amplitude
        {
            get => _fPwrCh1Amplitude;
            set
            {
                Set(ref _fPwrCh1Amplitude, value);
                _pwrParams[1].Amplitude = value;
            }
        }

        private int _fPwrCh1Freq;
        public int PwrCh1Freq
        {
            get => _fPwrCh1Freq;
            set
            {
                Set(ref _fPwrCh1Freq, value);
                _pwrParams[1].Frequency = value;
            }
        }

        private int _fPwrCh1Duty;
        public int PwrCh1Duty
        {
            get => _fPwrCh1Duty;
            set
            {
                Set(ref _fPwrCh1Duty, value);
                _pwrParams[1].Duty = value;
            }
        }

        private int _fPwrCh1Phase;
        public int PwrCh1Phase
        {
            get => _fPwrCh1Phase;
            set
            {
                Set(ref _fPwrCh1Phase, value);
                _pwrParams[1].Phase = value;
            }
        }
        private int _fPwrCh1MaxVoltage;
        public int PwrCh1MaxVoltage
        {
            get => _fPwrCh1MaxVoltage;
            set
            {
                Set(ref _fPwrCh1MaxVoltage, value);
                _pwrParams[1].MaxVolts = value;
            }
        }

        private int _fPwrCh1MaxAmps;
        public int PwrCh1MaxAmps
        {
            get => _fPwrCh1MaxAmps;
            set
            {
                Set(ref _fPwrCh1MaxAmps, value);
                _pwrParams[1].MaxAmps = value;
            }
        }

        private bool _fPwrSwitchCh2;
        public bool PwrSwitchCh2
        {
            get => _fPwrSwitchCh2;
            set
            {
                Set(ref _fPwrSwitchCh2, value);
                if (value) _fPwrDriver?.SetChannelOn(2); else _fPwrDriver?.SetChannelOff(2);
            }
        }

        private string _fLabelPwrChannel2Bias;
        public string LabelPwrChannel2Bias
        {
            get => _fLabelPwrChannel2Bias;
            set => Set(ref _fLabelPwrChannel2Bias, value);
        }

        private int _fPwrCh2Mode;
        public int PwrCh2Mode
        {
            get => _fPwrCh2Mode;
            set
            {
                Set(ref _fPwrCh2Mode, value);
                if (value == 1)
                {
                    LabelPwrChannel2Bias = Resources.LabelElectricFlow;
                    _fPwrDriver?.SetChannelOn(2);
                }
                else
                {
                    LabelPwrChannel2Bias = Resources.LabelOffsetVoltage;
                }
                _pwrParams[2].Mode = value;
            }
        }

        private int _fPwrCh2Bias;
        public int PwrCh2Bias
        {
            get => _fPwrCh2Bias;
            set
            {
                Set(ref _fPwrCh2Bias, value);
                _pwrParams[2].Bias = value;
            }
        }

        private int _fPwrCh2Amplitude;
        public int PwrCh2Amplitude
        {
            get => _fPwrCh2Amplitude;
            set
            {
                Set(ref _fPwrCh2Amplitude, value);
                _pwrParams[2].Amplitude = value;
            }
        }

        private int _fPwrCh2Freq;
        public int PwrCh2Freq
        {
            get => _fPwrCh2Freq;
            set
            {
                Set(ref _fPwrCh2Freq, value);
                _pwrParams[2].Frequency = value;
            }
        }

        private int _fPwrCh2Duty;
        public int PwrCh2Duty
        {
            get => _fPwrCh2Duty;
            set { Set(ref _fPwrCh2Duty, value); _pwrParams[2].Duty = value; }
        }

        private int _fPwrCh2Phase;
        public int PwrCh2Phase
        {
            get => _fPwrCh2Phase;
            set { Set(ref _fPwrCh2Phase, value); _pwrParams[2].Phase = value; }
        }

        private int _fPwrCh2MaxVoltage;
        public int PwrCh2MaxVoltage
        {
            get => _fPwrCh2MaxVoltage;
            set { Set(ref _fPwrCh2MaxVoltage, value); _pwrParams[2].MaxVolts = value; }
        }

        private int _fPwrCh2MaxAmps;
        public int PwrCh2MaxAmps
        {
            get => _fPwrCh2MaxAmps;
            set
            {
                Set(ref _fPwrCh2MaxAmps, value);
                _pwrParams[2].MaxAmps = value;
            }
        }

        private bool _fPwrSwitchCh3;
        public bool PwrSwitchCh3
        {
            get => _fPwrSwitchCh3;
            set
            {
                Set(ref _fPwrSwitchCh3, value);
                if (value) _fPwrDriver?.SetChannelOn(3); else _fPwrDriver?.SetChannelOff(3);
            }
        }

        private string _fLabelPwrChannel3Bias;
        public string LabelPwrChannel3Bias
        {
            get => _fLabelPwrChannel3Bias;
            set => Set(ref _fLabelPwrChannel3Bias, value);
        }

        private int _fPwrCh3Mode;
        public int PwrCh3Mode
        {
            get => _fPwrCh3Mode;
            set
            {
                Set(ref _fPwrCh3Mode, value);
                if (value == 1)
                {
                    LabelPwrChannel3Bias = Resources.LabelElectricFlow;
                    _fPwrDriver?.SetChannelOn(3);
                }
                else { LabelPwrChannel3Bias = Resources.LabelOffsetVoltage; }
                _pwrParams[3].Mode = value;
            }
        }

        private string _fLabelPwrChannel4Bias;
        public string LabelPwrChannel4Bias
        {
            get => _fLabelPwrChannel4Bias;
            set => Set(ref _fLabelPwrChannel4Bias, value);
        }

        private int _fPwrCh4Mode;
        public int PwrCh4Mode
        {
            get => _fPwrCh4Mode;
            set
            {
                Set(ref _fPwrCh4Mode, value);
                if (value == 1)
                {
                    LabelPwrChannel4Bias = Resources.LabelElectricFlow;
                    _fPwrDriver?.SetChannelOn(4);
                }
                else
                {
                    LabelPwrChannel4Bias = Resources.LabelOffsetVoltage;
                }
                _pwrParams[4].Mode = value;
            }
        }

        private int _fPwrCh3Bias;
        public int PwrCh3Bias
        {
            get => _fPwrCh3Bias;
            set
            {
                Set(ref _fPwrCh3Bias, value);
                _pwrParams[3].Bias = value;
            }
        }

        private int _fPwrCh3Amplitude;
        public int PwrCh3Amplitude
        {
            get => _fPwrCh3Amplitude;
            set
            {
                Set(ref _fPwrCh3Amplitude, value);
                _pwrParams[3].Amplitude = value;
            }
        }

        private int _fPwrCh3Freq;
        public int PwrCh3Freq
        {
            get => _fPwrCh3Freq;
            set
            {
                Set(ref _fPwrCh3Freq, value);
                _pwrParams[3].Frequency = value;
            }
        }

        private int _fPwrCh3Duty;
        public int PwrCh3Duty
        {
            get => _fPwrCh3Duty;
            set
            {
                Set(ref _fPwrCh3Duty, value);
                _pwrParams[3].Duty = value;
            }
        }

        private int _fPwrCh3Phase;
        public int PwrCh3Phase
        {
            get => _fPwrCh3Phase;
            set
            {
                Set(ref _fPwrCh3Phase, value);
                _pwrParams[3].Phase = value;
            }
        }

        private int _fPwrCh3MaxVoltage;
        public int PwrCh3MaxVoltage
        {
            get => _fPwrCh3MaxVoltage;
            set
            {
                Set(ref _fPwrCh3MaxVoltage, value);
                _pwrParams[3].MaxVolts = value;
            }
        }

        private int _fPwrCh3MaxAmps;
        public int PwrCh3MaxAmps
        {
            get => _fPwrCh3MaxAmps;
            set
            {
                Set(ref _fPwrCh3MaxAmps, value);
                _pwrParams[3].MaxAmps = value;
            }
        }

        private bool _fPwrSwitchCh4;
        public bool PwrSwitchCh4
        {
            get => _fPwrSwitchCh4;
            set
            {
                Set(ref _fPwrSwitchCh4, value);
                if (value)
                    _fPwrDriver?.SetChannelOn(4);
                else
                    _fPwrDriver?.SetChannelOff(4);
            }
        }

        private int _fPwrCh4Bias;
        public int PwrCh4Bias
        {
            get => _fPwrCh4Bias;
            set
            {
                Set(ref _fPwrCh4Bias, value);
                _pwrParams[4].Bias = value;
            }
        }

        private int _fPwrCh4Amplitude;
        public int PwrCh4Amplitude
        {
            get => _fPwrCh4Amplitude;
            set
            {
                Set(ref _fPwrCh4Amplitude, value);
                _pwrParams[4].Amplitude = value;
            }
        }

        private int _fPwrCh4Freq;
        public int PwrCh4Freq
        {
            get => _fPwrCh4Freq;
            set
            {
                Set(ref _fPwrCh4Freq, value);
                _pwrParams[4].Frequency = value;
            }
        }

        private int _fPwrCh4Duty;
        public int PwrCh4Duty
        {
            get => _fPwrCh4Duty;
            set
            {
                Set(ref _fPwrCh4Duty, value);
                _pwrParams[4].Duty = value;
            }
        }

        private int _fPwrCh4Phase;
        public int PwrCh4Phase
        {
            get => _fPwrCh4Phase;
            set
            {
                Set(ref _fPwrCh4Phase, value);
                _pwrParams[4].Phase = value;
            }
        }

        private int _fPwrCh4MaxVoltage;
        public int PwrCh4MaxVoltage
        {
            get => _fPwrCh4MaxVoltage;
            set
            {
                Set(ref _fPwrCh4MaxVoltage, value);
                _pwrParams[4].MaxVolts = value;
            }
        }

        private int _fPwrCh4MaxAmps;
        public int PwrCh4MaxAmps
        {
            get => _fPwrCh4MaxAmps;
            set
            {
                Set(ref _fPwrCh4MaxAmps, value);
                _pwrParams[4].MaxAmps = value;
            }
        }

        private bool _fPwrSwitchCh5;
        public bool PwrSwitchCh5
        {
            get => _fPwrSwitchCh5;
            set
            {
                Set(ref _fPwrSwitchCh5, value);
                if (value) _fPwrDriver?.SetChannelOn(5); else _fPwrDriver?.SetChannelOff(5);
            }
        }

        private string _fLabelPwrChannel5Bias;
        public string LabelPwrChannel5Bias
        {
            get => _fLabelPwrChannel5Bias;
            set => Set(ref _fLabelPwrChannel5Bias, value);
        }

        private int _fPwrCh5Mode;
        public int PwrCh5Mode
        {
            get => _fPwrCh5Mode;
            set
            {
                Set(ref _fPwrCh5Mode, value);
                if (value == 1)
                {
                    LabelPwrChannel5Bias = Resources.LabelElectricFlow;
                    _fPwrDriver?.SetChannelOn(5);
                }
                else
                {
                    LabelPwrChannel5Bias = Resources.LabelOffsetVoltage;
                    _fPwrDriver?.SetChannelOff(5);
                }
                _pwrParams[5].Mode = value;
            }
        }

        private int _fPwrCh5Bias;
        public int PwrCh5Bias
        {
            get => _fPwrCh5Bias;
            set
            {
                Set(ref _fPwrCh5Bias, value);
                _pwrParams[5].Bias = value;
            }
        }

        private int _fPwrCh5Amplitude;
        public int PwrCh5Amplitude
        {
            get => _fPwrCh5Amplitude;
            set
            {
                Set(ref _fPwrCh5Amplitude, value);
                _pwrParams[5].Amplitude = value;
            }
        }

        private int _fPwrCh5Freq;
        public int PwrCh5Freq
        {
            get => _fPwrCh5Freq;
            set
            {
                Set(ref _fPwrCh5Freq, value);
                _pwrParams[5].Frequency = value;
            }
        }

        private int _fPwrCh5Duty;
        public int PwrCh5Duty
        {
            get => _fPwrCh5Duty;
            set
            {
                Set(ref _fPwrCh5Duty, value);
                _pwrParams[5].Duty = value;
            }
        }

        private int _fPwrCh5Phase;
        public int PwrCh5Phase
        {
            get => _fPwrCh5Phase;
            set
            {
                Set(ref _fPwrCh5Phase, value);
                _pwrParams[5].Phase = value;
            }
        }

        private int _fPwrCh5MaxVoltage;
        public int PwrCh5MaxVoltage
        {
            get => _fPwrCh5MaxVoltage;
            set
            {
                Set(ref _fPwrCh5MaxVoltage, value);
                _pwrParams[5].MaxVolts = value;
            }
        }

        private int _fPwrCh5MaxAmps;
        public int PwrCh5MaxAmps
        {
            get => _fPwrCh5MaxAmps;
            set
            {
                Set(ref _fPwrCh5MaxAmps, value);
                _pwrParams[5].MaxAmps = value;
            }
        }



        private bool _fIsPumpsActive;
        public bool IsPumpsActive
        {
            get => _fIsPumpsActive;
            set
            {
                Set(ref _fIsPumpsActive, value);
                if (value) IsConfocalActive = true;
                _fPumpDriver?.SetPumpActive(value);
            }
        }

        private bool _fIsConfocalActive;
        public bool IsConfocalActive
        {
            get => _fIsConfocalActive;
            set
            {
                Set(ref _fIsConfocalActive, value);
                _fConfocalDriver?.SetMeasuredActive(value);
            }
        }

        private double[] _fConfocalLog;
        public double[] ConfocalLog
        {
            get => _fConfocalLog;
            set => Set(ref _fConfocalLog, value);
        }

        private bool _isDispenserPortConnected;
        public bool IsDispenserPortConnected
        {
            get => _isDispenserPortConnected;
            set
            {
                Set(ref _isDispenserPortConnected, value);

                if (value)
                {
                    _fDispenserDriver.ConnectToPort();
                    CollectDispenserData();
                }
                else _fDispenserDriver.Disconnect();
            }
        }

        private bool _fIsPressureSensorPortConnected;
        public bool IsPressureSensorPortConnected
        {
            get => _fIsPressureSensorPortConnected;
            set
            {
                Set(ref _fIsPressureSensorPortConnected, value);
                _fPressureSensorDriver?.ConnectToPort();
            }
        }

        private bool _isPressureSensorActive;
        public bool IsPressureSensorActive
        {
            get => _isPressureSensorActive;
            set
            {
                _ = Set(ref _isPressureSensorActive, value);
                _fPressureSensorDriver?.SetMeasuring(value);
            }
        }

        private bool _isPyroPortConnected;
        public bool IsPyroPortConnected
        {
            get => _isPyroPortConnected;
            set
            {
                _ = Set(ref _isPyroPortConnected, value);
                if (value) _fPyroDriver.ConnectToPort();
                else _fPyroDriver.Disconnect();
            }
        }

        private bool _isPyroActive;
        public bool IsPyroActive
        {
            get => _isPyroActive;
            set
            {
                _ = Set(ref _isPyroActive, value);
                _fPyroDriver.SetMeasuring(value);
            }
        }

        private float _fPyroTemperature;
        public float PyroTemperature
        {
            get => _fPyroTemperature;
            set
            {
                Set(ref _fPyroTemperature, value);
                CurrentTemperature = Math.Round((value * value * (-0.007)) + (value * 2.1476) - 21.948, 3);
            }
        }

        private float _pressureSensorValue;
        public float PressureSensorValue
        {
            get => _pressureSensorValue;
            set => Set(ref _pressureSensorValue, value);
        }

        private float _pressureSensorTemperature;
        public float PressureSensorTemperature
        {
            get => _pressureSensorTemperature;
            set => Set(ref _pressureSensorTemperature, value);
        }

        private bool _fIsPressurePumpConnected;
        public bool IsPressurePumpConnected
        {
            get => _fIsPressurePumpConnected;
            set
            {
                _ = Set(ref _fIsPressurePumpConnected, value);
                if (value) _fPressurePumpDriver?.Calibrate();
                //FIX_ME add connect function
            }
        }


        private bool _fIsDispenserActive;
        public bool IsDispenserActive
        {
            get => _fIsDispenserActive;
            set
            {
                Set(ref _fIsDispenserActive, value);
                if (_fIsDispenserActive)
                {
                    if (!IsDispenserPortConnected) IsDispenserPortConnected = true;
                    _fDispenserDriver.Start();
                }
                else _fDispenserDriver.Stop();
            }
        }

        private int _fDispenserSignalType;
        public int DispenserSignalType
        {
            get => _fDispenserSignalType;
            set
            {
                _ = Set(ref _fDispenserSignalType, value);
                _fDispenserDriver?.SetSignalType(_fDispenserSignalType);
                switch (value)
                {
                    case 0:
                        DispenserSingleWaveVisible = Visibility.Visible;
                        DispenserHarmonicallyWaveVisible = Visibility.Collapsed;
                        break;
                    case 1:
                        DispenserHarmonicallyWaveVisible = Visibility.Visible;
                        DispenserSingleWaveVisible = Visibility.Collapsed;
                        break;
                    default:
                        DispenserSingleWaveVisible = Visibility.Collapsed;
                        DispenserHarmonicallyWaveVisible = Visibility.Collapsed;
                        break;
                }
            }
        }

        private int _fDispenserChannel;
        public int DispenserChannel
        {
            get => _fDispenserChannel;
            set
            {
                _fDispenserDriver?.SetChannel(value);
                Set(ref _fDispenserChannel, value);
            }
        }

        private int _fDispenserFrequency;
        public int DispenserFrequency
        {
            get => _fDispenserFrequency;
            set
            {
                _fDispenserDriver?.SetFrequency(value);
                Set(ref _fDispenserFrequency, value);
            }
        }

        private int _fDispenserRiseTime;
        public int DispenserRiseTime
        {
            get => _fDispenserRiseTime;
            set
            {
                Set(ref _fDispenserRiseTime, value);
                CollectPulseData();
            }
        }

        private int _fDispenserKeepTime;
        public int DispenserKeepTime
        {
            get => _fDispenserKeepTime;
            set
            {
                Set(ref _fDispenserKeepTime, value);
                CollectPulseData();
            }
        }

        private int _fDispenserFallTime;
        public int DispenserFallTime
        {
            get => _fDispenserFallTime;
            set
            {
                Set(ref _fDispenserFallTime, value);
                CollectPulseData();
            }
        }

        private int _fDispenserLowTime;
        public int DispenserLowTime
        {
            get => _fDispenserLowTime;
            set
            {
                Set(ref _fDispenserLowTime, value);
                CollectPulseData();
            }
        }

        private int _fDispenserRiseTime2;
        public int DispenserRiseTime2
        {
            get => _fDispenserRiseTime2;
            set
            {
                Set(ref _fDispenserRiseTime2, value);
                CollectPulseData();
            }
        }

        private int _fDispenserV0;
        public int DispenserV0
        {
            get => _fDispenserV0;
            set
            {
                Set(ref _fDispenserV0, value);
                CollectPulseData();
            }
        }

        private int _fDispenserV1;
        public int DispenserV1
        {
            get => _fDispenserV1;
            set
            {
                Set(ref _fDispenserV1, value);
                CollectPulseData();
            }
        }

        private int _fDispenserV2;
        public int DispenserV2
        {
            get => _fDispenserV2;
            set
            {
                Set(ref _fDispenserV2, value);
                CollectPulseData();
            }
        }

        private Visibility _fDispenserSingleWaveVisible;
        public Visibility DispenserSingleWaveVisible
        {
            get => _fDispenserSingleWaveVisible;
            set => Set(ref _fDispenserSingleWaveVisible, value);
        }

        private int _fDispenserHv0;
        public int DispenserHv0
        {
            get => _fDispenserHv0;
            set
            {
                Set(ref _fDispenserHv0, value);
                CollectSineData();
            }
        }

        private int _fDispenserHVpeak;
        public int DispenserHVpeak
        {
            get => _fDispenserHVpeak;
            set
            {
                Set(ref _fDispenserHVpeak, value);
                CollectSineData();
            }
        }

        private int _fDispenserHToverall;
        public int DispenserHToverall
        {
            get => _fDispenserHToverall;
            set
            {
                Set(ref _fDispenserHToverall, value);
                CollectSineData();
            }
        }

        private Visibility _fDispenserHarmonicWaveVisible;
        public Visibility DispenserHarmonicallyWaveVisible
        {
            get => _fDispenserHarmonicWaveVisible;
            set => Set(ref _fDispenserHarmonicWaveVisible, value);
        }

        private bool _fPressurePumpActive;
        public bool PressurePumpActive
        {
            get => _fPressurePumpActive;
            set
            {
                Set(ref _fPressurePumpActive, value);
                _fPressurePumpDriver?.SetTrigger(value);
            }
        }

        private int _fAirSupportPressure;
        public int AirSupportPressure
        {
            get => _fAirSupportPressure;
            set
            {
                Set(ref _fAirSupportPressure, value);
                _fPressurePumpDriver?.SetPressure(value);
            }
        }

        private int _airSupportPressureSelectedItem;
        public int AirSupportPressureSelectedItem
        {
            get => _airSupportPressureSelectedItem;
            set
            {
                _ = Set(ref _airSupportPressureSelectedItem, value);
                AirSupportPressure = value;
            }
        }

        #region PwrTab
        private int _fSelectedPowerPage;
        public int SelectedPowerPage
        {
            get => _fSelectedPowerPage;
            set => Set(ref _fSelectedPowerPage, value);
        }
        #endregion


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
        public ObservableCollection<string> DispenserPortCollection { get; set; }
        public ObservableCollection<string> DispenserModeCollection { get; set; }
        public ObservableCollection<float> PumpingSpeedCollection { get; set; }
        public ObservableCollection<int> AirSupportPressureCollection { get; set; }
        public ObservableCollection<string> PressureSensorPortCollection { get; set; }
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
        public static string LabelPressureSensorPort => Resources.LabelPressureSensorPort;
        public static string LabelAirSupportPort => Resources.LabelAirSupportPort;
        public static string LabelLaserType => Resources.LabelLaserType;
        public static string PowerSupplyTitle => Resources.PowerSuplyTitle;
        public static string PyroOperationTitle => Resources.PyroOperationTitle;
        public static string LabelUfLed1 => Resources.LabelUfLed1;
        public static string LabelUfLed2 => Resources.LabelUfLed2;
        public static string LabelChannelsSwitch => Resources.LabelChanelsSwitch;
        public static string LabelPwrChannelMode => Resources.LabelPwrChannelMode;
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
        public static string LabelTemperature => Resources.LabelTemperature;
        public static string LabelPressure => Resources.LabelPressure;
        public static string DispenserOperationTitle => Resources.DispenserOperationTitle;
        public static string DispenserSignalTypeLabel => Resources.DispenserSignalTypeLabel;
        public static string DispenserCurrentChannelLabel => Resources.DispenserCurrentChannelLabel;
        public static string DispenserFrequencyLabel => Resources.DispenserFrequencyLabel;
        public static string DispenserTimeLabel => Resources.DispenserTimeLabel;
        public static string DispenserRiseTimeLabel => Resources.DispenserRiseTimeLabel;
        public static string DispenserRiseTime2Label => Resources.DispenserRiseTime2Label;
        public static string DispenserKeepTimeLabel => Resources.DispenserKeepTimeLabel;
        public static string DispenserFallTimeLabel => Resources.DispenserFallTimeLabel;
        public static string DispenserLowTimeLabel => Resources.DispenserLowTimeLabel;
        public static string DispenserWaitingLabel => Resources.DispenserWaitingLabel;
        public static string DispenserVoltageLabel => Resources.DispenserVoltageLabel;
        public static string DispenserMaximumLabel => Resources.DispenserMaximumLabel;
        public static string DispenserMinimumLabel => Resources.DispenserMinimumLabel;
        public static string DispenserAverageLabel => Resources.DispenserAverageLabel;
        public static string DispenserPeakLabel => Resources.DispenserPeakLabel;
        public static string DispenserStartLabel => Resources.DispenserStartLabel;
        public static string DispenserPeriodLabel => Resources.DispenserPeriodLabel;
        public static string LabelPressurePump => Resources.LabelPressurePump;
        public static int ChannelTag0 => 0;
        public static int ChannelTag1 => 1;
        public static int ChannelTag2 => 2;
        public static int ChannelTag3 => 3;
        public static int ChannelTag4 => 4;
        public static int ChannelTag5 => 5;
        public static string LabelOverPumpSpeed => Resources.LabelOverPumpSpeed;
        public static string LabelPressurePumpStart => Resources.LabelPressurePumpStart;
        public static string LabelAirSupportPressure => Resources.LabelAirSupportPressure;
        #endregion

        #region Commands
        public ICommand QuitCommand { get; }
        public ICommand MinimizedCommand { get; }
        public ICommand MaximizedCommand { get; }
        public ICommand StandardSizeCommand { get; }
        public ICommand SetLaserPwrCommand { get; }
        public ICommand ToggleLaserEmit { get; }
        public ICommand StartPumpCommand { get; }
        public ICommand TogglePumpCommand { get; }
        public ICommand ToggleDispenserCommand { get; }
        public ICommand ReadChannelParamsCommand { get; }
        public ICommand WriteChannelParamsCommand { get; }

        #endregion

        public MainModel()
        {
            // Data context
            _fDbContext = new ApplicationContext();
            //f_DbContext.Logs.Load();
            // init collections
            ConfocalLog = new[] { 0d, .40d, .3d };
            _pwrParams = new DataModels.PwrItem[6];
            for (byte i = 0; i < 6; i++)
            {
                _pwrParams[i] = new DataModels.PwrItem();
            }
            LogCollection = new ObservableCollection<LogItem>();
            IncomingPumpPortSelected = Settings.Default.IncomingPumpPortSelected;
            PowerSupplyTypes = new ObservableCollection<string>(new PowerSuplyTupesList().GetTypesList());
            IncomingPumpPortCollection = new ObservableCollection<string>(new PortList().GetPortList(IncomingPumpPortSelected));
            OutgoingPumpPortSelected = Settings.Default.OutloginPumpPortSelected;
            OutcomingPumpPortCollection = new ObservableCollection<string>(new PortList().GetPortList(OutgoingPumpPortSelected));
            LaserPortSelected = Settings.Default.LaserPortSelected;
            LaserPortCollection = new ObservableCollection<string>(new PortList().GetPortList(LaserPortSelected));
            PyroPortSelected = Settings.Default.PyroPortSelected;
            DispenserPortSelected = Settings.Default.DispenserPortSelected;
            PressureSensorPortSelected = Settings.Default.PressureSensorPortSelected;
            PyroPortCollection = new ObservableCollection<string>(new PortList().GetPortList(PyroPortSelected));
            PwrPortSelected = Settings.Default.PwrPortSelected;
            PwrPortCollection = new ObservableCollection<string>(new PortList().GetPortList(PyroPortSelected));
            LaserHistoryCollection = new ObservableCollection<int>() { 100, 150, 200, 250, 300, 350, 400 };
            DispenserPortCollection = new ObservableCollection<string>(new PortList().GetPortList(DispenserPortSelected));
            PressureSensorPortCollection = new ObservableCollection<string>(new PortList().GetPortList(PressureSensorPortSelected));
            // AirSupportPortCollection = new ObservableCollection<string>(new PortList().GetPortList(AirSupportPortSelected));
            DispenserModeCollection = new ObservableCollection<string>(new PortList().GetDispenserModes());
            PumpingSpeedCollection = new ObservableCollection<float>() { 0f, 0.5f, 2.5f, 11f, 25f, 45f, 65f, 150f };
            AirSupportPressureCollection = new ObservableCollection<int> { 2, 10, 20, 50, 150, 200 };

            //Other
            CurWindowState = WindowState.Normal;
            //Exclude
            _fDispenserDriver = new DispenserDriver();
            _fDispenserDriver.SetLogMessage += AddLogMessage;
            _fDispenserDriver.PortStr = Settings.Default.DispenserPortSelected;
            //f_PressureSensorPortSelected = Settings.Default.PressureSensorPortSelected;
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
            DispenserSignalType = Settings.Default.DispenserSignalType;
            DispenserChannel = Settings.Default.DispenserChannel;
            DispenserFrequency = Settings.Default.DispenserFrequency;
            DispenserRiseTime = Settings.Default.DispenserRiseTime;
            DispenserKeepTime = Settings.Default.DispenserKeepTime;
            DispenserFallTime = Settings.Default.DispenserFallTime;
            DispenserLowTime = Settings.Default.DispenserLowTime;
            DispenserRiseTime2 = Settings.Default.DispenserRiseTime2;
            DispenserV0 = Settings.Default.DispenserV0;
            DispenserV1 = Settings.Default.DispenserV1;
            DispenserV2 = Settings.Default.DispenserV2;
            DispenserHv0 = Settings.Default.DispenserHV0;
            DispenserHVpeak = Settings.Default.DispenserHVpeak;
            DispenserHToverall = Settings.Default.DispenserHToverall;
            AirSupportPressure = Settings.Default.AirSupportPressure;
            //init command area
            QuitCommand = new LambdaCommand(OnQuitApp);
            MinimizedCommand = new LambdaCommand(OnMinimizedCommandExecute);
            MaximizedCommand = new LambdaCommand(OnMaximizedCommandExecute);
            StandardSizeCommand = new LambdaCommand(OnStandardSizeCommand);
            SetLaserPwrCommand = new LambdaCommand(OnSetLaserPower);
            ToggleLaserEmit = new LambdaCommand(OnToggleLaserEmit);
            StartPumpCommand = new LambdaCommand(OnStartPump);
            TogglePumpCommand = new LambdaCommand(OnTogglePump);
            ToggleDispenserCommand = new LambdaCommand(OnToggleDispenserActive);
            ReadChannelParamsCommand = new LambdaCommand(OnReadChannelParamsCommand);
            WriteChannelParamsCommand = new LambdaCommand(OnWriteChannelParamsCommand);

            //Drivers area
            _fConfocalDriver = new ConfocalDriver();
            _fConfocalDriver.ObtainedDataEvent += SetUpMeasuredLevel;
            _fConfocalDriver.SetLogMessage += AddLogMessage;

            _fPumpDriver = new PumpDriver();
            _fPumpDriver.SetLogMessage += AddLogMessage;
            _fPumpDriver.PortStrInput = Settings.Default.IncomingPumpPortSelected;
            _fPumpDriver.PortStrOutput = Settings.Default.OutloginPumpPortSelected;
            _fPumpDriver.TogleTwoPump(Settings.Default.IsTwoPump);
            _fPumpDriver.SetInputSpeed += SetIncomingPumpSpeedLabel;
            _fPumpDriver.SetOutputSpeed += SetOutcomingPumpSpeedLabel;
            _fPumpDriver.SetRequiredLvl(ConfocalLevelSetter);

            _fPwrDriver = new PwrDriver();
            _fPwrDriver.SetLogMessage += AddLogMessage;
            _fPwrDriver.SetChannelParameters += SetChannelParams;
            _fPwrDriver.PortStr = Settings.Default.PwrPortSelected;

            _fLaserDriver = new LaserDriver();
            _fLaserDriver.SetLogMessage += AddLogMessage;
            _fLaserDriver.PortString = Settings.Default.LaserPortSelected;
            LaserTypeSelectedIndex = Settings.Default.LaserTypeSelectedIndex;

            _fPyroDriver = new PyroDriver();
            _fPyroDriver.SetLogMessage += AddLogMessage;
            _fPyroDriver.EventHandler += PyroHandler;
            _fPyroDriver.PortStr = Settings.Default.PyroPortSelected;

            _fPressurePumpDriver = new PressurePumpDriver();

            _fPressureSensorDriver = new PressureSensorDriver(PressureSensorPortSelected);
            _fPressureSensorDriver.EventHandler += PressureSensorHandler;

            AddLogMessage("Application Started");
        }

        private void AddLogMessage(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LogCollection.Insert(0, new LogItem(DateTime.Now, message));
                _fDbContext.Logs.Add(new Log { Dt = DateTime.Now, Message = message, Code = 0 });
            });
        }

        private void PyroHandler(float temperature)
        {
            Application.Current.Dispatcher.Invoke(() => { PyroTemperature = temperature; });
            _fDbContext.Temperatures.Add(new Temperature { Dt = DateTime.Now, Tmp = temperature });
        }

        private void PressureSensorHandler(float pressure, float temperature)
        {
            Application.Current.Dispatcher.Invoke(() => { PressureSensorTemperature = temperature; });
            Application.Current.Dispatcher.Invoke(() => { PressureSensorValue = pressure; });
        }

        private void OnQuitApp(object p)
        {
            _fDbContext.SaveChanges();
            Settings.Default.WindowHeight = WindowHeight;
            Settings.Default.WindowWidth = WindowWidth;
            Settings.Default.IsTwoPump = IsTwoPump;
            Settings.Default.ConfocalLevelSetter = ConfocalLevelSetter;
            Settings.Default.IncomingPumpPortSelected = IncomingPumpPortSelected;
            Settings.Default.OutloginPumpPortSelected = OutgoingPumpPortSelected;
            Settings.Default.LaserPowerSetter = LaserPowerSetter;
            Settings.Default.LaserPortSelected = LaserPortSelected;
            Settings.Default.PyroPortSelected = PyroPortSelected;
            Settings.Default.DispenserPortSelected = DispenserPortSelected;
            Settings.Default.PressureSensorPortSelected = PressureSensorPortSelected;
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
            Settings.Default.DispenserSignalType = DispenserSignalType;
            Settings.Default.DispenserChannel = DispenserChannel;
            Settings.Default.DispenserFrequency = DispenserFrequency;
            Settings.Default.DispenserRiseTime = DispenserRiseTime;
            Settings.Default.DispenserKeepTime = DispenserKeepTime;
            Settings.Default.DispenserFallTime = DispenserFallTime;
            Settings.Default.DispenserLowTime = DispenserLowTime;
            Settings.Default.DispenserRiseTime2 = DispenserRiseTime2;
            Settings.Default.DispenserV0 = DispenserV0;
            Settings.Default.DispenserV1 = DispenserV1;
            Settings.Default.DispenserV2 = DispenserV2;
            Settings.Default.DispenserHV0 = DispenserHv0;
            Settings.Default.DispenserHVpeak = DispenserHVpeak;
            Settings.Default.DispenserHToverall = DispenserHToverall;
            Settings.Default.AirSupportPressure = AirSupportPressure;
            Settings.Default.Save();
            _fDbContext.Dispose();
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
            _fLaserDriver.SetPower(LaserPowerSetter);
            if (!LaserHistoryCollection.Contains(LaserPowerSetter))
                LaserHistoryCollection.Insert(0, LaserPowerSetter);
        }

        private void OnToggleLaserEmit(object sender)
        {
            IsLaserEmit = !IsLaserEmit;
        }

        private void OnStartPump(object sender)
        {
            if (!IsPumpPortsConnect) IsPumpPortsConnect = true;
            IsConfocalActive = !IsPumpsActive;
            IsPumpsActive = !IsPumpsActive;
        }

        private void OnTogglePump(object sender)
        {
            IsPumpsActive = !IsPumpsActive;
        }

        private void OnToggleDispenserActive(object sender)
        {
            IsDispenserActive = !IsDispenserActive;
        }

        private void OnReadChannelParamsCommand(object sender)
        {
            var ch = (int)sender;
            _fPwrDriver?.GetChanelData(Convert.ToByte(ch));
        }

        private void OnWriteChannelParamsCommand(object sender)
        {
            var ch = (int)sender;
            var pi = GetChannelParams(ch);
            _fPwrDriver?.WriteChannelData(ch, pi);
        }

        private void SetUpMeasuredLevel(DistMeasureRes lvl)
        {
            ConfocalLevel = Math.Round(lvl.Dist, 5);
            _fPumpDriver?.SetMeasuredLevel(lvl);
            ConfocalLog = _fConfocalDriver.GetLastFragment();
        }

        private void SetIncomingPumpSpeedLabel(string speed)
        {
            Application.Current.Dispatcher.Invoke(() => IncomingPumpSpeed = speed);
        }
        private void SetOutcomingPumpSpeedLabel(string speed)
        {
            Application.Current.Dispatcher.Invoke(() => OutgoingPumpSpeed = speed);
        }

        private void CollectSineData()
        {
            var data = new DispenserSineWaveData() { TimeToverall = DispenserHToverall, V0 = DispenserHv0, VPeak = DispenserHVpeak };
            _fDispenserDriver?.SetSineWaveData(data);
        }
        private void CollectPulseData()
        {
            var data = new DispenserPulseWaveData()
            {
                TimeRise1 = DispenserRiseTime,
                TimeT1 = DispenserKeepTime,
                TimeFall = DispenserFallTime,
                TimeT2 = DispenserLowTime,
                TimeRise2 = DispenserRiseTime2,
                V0 = DispenserV0,
                V1 = DispenserV1,
                V2 = DispenserV2
            };
            _fDispenserDriver?.SetPulseWaveData(data);
        }

        private void CollectDispenserData()
        {
            CollectSineData();
            CollectPulseData();
            _fDispenserDriver?.SetChannel(_fDispenserChannel);
            _fDispenserDriver?.SetFrequency(_fDispenserFrequency);
        }

        private void SetChannelParams(int channel, DataModels.PwrItem pi)
        {
            switch (channel)
            {
                case 0:
                    PwrCh0Mode = pi.Mode;
                    PwrCh0Bias = pi.Bias;
                    PwrCh0Amplitude = pi.Amplitude;
                    PwrCh0Freq = pi.Frequency;
                    PwrCh0Duty = pi.Duty;
                    PwrCh0Phase = pi.Phase;
                    PwrCh0MaxVoltage = pi.MaxVolts;
                    PwrCh0MaxAmps = pi.MaxAmps;
                    break;
                case 1:
                    PwrCh1Mode = pi.Mode;
                    PwrCh1Bias = pi.Bias;
                    PwrCh1Amplitude = pi.Amplitude;
                    PwrCh1Freq = pi.Frequency;
                    PwrCh1Duty = pi.Duty;
                    PwrCh1Phase = pi.Phase;
                    PwrCh1MaxVoltage = pi.MaxVolts;
                    PwrCh1MaxAmps = pi.MaxAmps;
                    break;
                case 2:
                    PwrCh2Mode = pi.Mode;
                    PwrCh2Bias = pi.Bias;
                    PwrCh2Amplitude = pi.Amplitude;
                    PwrCh2Freq = pi.Frequency;
                    PwrCh2Duty = pi.Duty;
                    PwrCh2Phase = pi.Phase;
                    PwrCh2MaxVoltage = pi.MaxVolts;
                    PwrCh2MaxAmps = pi.MaxAmps;
                    break;
                case 3:
                    PwrCh3Mode = pi.Mode;
                    PwrCh3Bias = pi.Bias;
                    PwrCh3Amplitude = pi.Amplitude;
                    PwrCh3Freq = pi.Frequency;
                    PwrCh3Duty = pi.Duty;
                    PwrCh3Phase = pi.Phase;
                    PwrCh3MaxVoltage = pi.MaxVolts;
                    PwrCh3MaxAmps = pi.MaxAmps;
                    break;
                case 4:
                    PwrCh4Mode = pi.Mode;
                    PwrCh4Bias = pi.Bias;
                    PwrCh4Amplitude = pi.Amplitude;
                    PwrCh4Freq = pi.Frequency;
                    PwrCh4Duty = pi.Duty;
                    PwrCh4Phase = pi.Phase;
                    PwrCh4MaxVoltage = pi.MaxVolts;
                    PwrCh4MaxAmps = pi.MaxAmps;
                    break;
                case 5:
                    PwrCh5Mode = pi.Mode;
                    PwrCh5Bias = pi.Bias;
                    PwrCh5Amplitude = pi.Amplitude;
                    PwrCh5Freq = pi.Frequency;
                    PwrCh5Duty = pi.Duty;
                    PwrCh5Phase = pi.Phase;
                    PwrCh5MaxVoltage = pi.MaxVolts;
                    PwrCh5MaxAmps = pi.MaxAmps;
                    break;
            }
        }

        private DataModels.PwrItem GetChannelParams(int channel)
        {
            if (channel < 0 || channel >= _pwrParams.Length) return null;
            return _pwrParams[channel];
        }
    }
}
