using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
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

        private readonly PumpDriver _pumpDriver;
        private readonly ConfocalDriver _confocalDriver;
        private readonly PwrDriver _pwrDriver;
        private readonly LaserDriver _laserDriver;
        private readonly PyroDriver _pyroDriver;
        private readonly DispenserDriver _dispenserDriver;
        private readonly PressurePumpDriver _pressurePumpDriver;
        private readonly PressureSensorDriver _pressureSensorDriver;
        private readonly LightingDriver _lightingDriver;

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
                    _pumpDriver.ConnectToPorts();
                else
                    _pumpDriver.Disconnect();
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
                _pumpDriver?.ToggleTwoPump(value);
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
                _pumpDriver?.SetRequiredLvl(value);
            }
        }

        private string _fIncomingPumpPortSelected;

        public string IncomingPumpPortSelected
        {
            get => _fIncomingPumpPortSelected;
            set
            {
                _ = Set(ref _fIncomingPumpPortSelected, value);
                if (_pumpDriver != null) _pumpDriver.PortStrInput = value;
            }
        }

        private string _outgoingPumpPortSelected;

        public string OutgoingPumpPortSelected
        {
            get => _outgoingPumpPortSelected;
            set
            {
                Set(ref _outgoingPumpPortSelected, value);
                if (_pumpDriver != null) _pumpDriver.PortStrOutput = value;
            }
        }

        private float _fPumpingSpeedSelected;

        public float PumpingSpeedSelected
        {
            get => _fPumpingSpeedSelected;
            set
            {
                _ = Set(ref _fPumpingSpeedSelected, value);
                if (_pumpDriver != null) _pumpDriver.PumpingSpeed = value;
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

        private string _outgoingPumpQueue;

        public string OutgoingPumpQueue
        {
            get => _outgoingPumpQueue;
            set => Set(ref _outgoingPumpQueue, value);
        }

        private string _incomingPumpQueue;

        public string IncomingPumpQueue
        {
            get => _incomingPumpQueue;
            set => Set(ref _incomingPumpQueue, value);
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

        private int _laserReducePower;

        public int LaserReducePower
        {
            get => _laserReducePower;
            set => Set(ref _laserReducePower, value);
        }

        private int _laserReduceTime;

        public int LaserReduceTime
        {
            get => _laserReduceTime;
            set => Set(ref _laserReduceTime, value);
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

        private Visibility _secondPumpPanelVisibility;

        public Visibility SecondPumpPanelVisibility
        {
            get => _secondPumpPanelVisibility;
            set => Set(ref _secondPumpPanelVisibility, value);
        }

        private bool _isRevereFirstPump;

        public bool IsRevereFirstPump
        {
            get => _isRevereFirstPump;
            set => Set(ref _isRevereFirstPump, value);
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
                    _laserDriver.ConnectToPort();
                    _laserDriver.SetLaserType(LaserTypeSelectedIndex);
                }
                else _laserDriver.Disconnect();
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
                _laserDriver?.EmitOn(value);
            }
        }

        private string _laserPortSelected;

        public string LaserPortSelected
        {
            get => _laserPortSelected;
            set
            {
                _ = Set(ref _laserPortSelected, value);
                if (_laserDriver != null) _laserDriver.PortString = value;
            }
        }

        private string _pyroPortSelected;

        public string PyroPortSelected
        {
            get => _pyroPortSelected;
            set
            {
                _ = Set(ref _pyroPortSelected, value);
                if (_pyroDriver != null) _pyroDriver.PortStr = value;
            }
        }

        private string _dispenserPortSelected;

        public string DispenserPortSelected
        {
            get => _dispenserPortSelected;
            set
            {
                _ = Set(ref _dispenserPortSelected, value);
                if (_dispenserDriver != null) _dispenserDriver.PortStr = value;
            }
        }

        private string _fAirSupportPortSelected;

        public string AirSupportPortSelected
        {
            get => _fAirSupportPortSelected;
            set => Set(ref _fAirSupportPortSelected, value);
        }

        private string _pwrPortSelected;

        public string PwrPortSelected
        {
            get => _pwrPortSelected;
            set
            {
                _ = Set(ref _pwrPortSelected, value);
                if (_pwrDriver != null) _pwrDriver.PortStr = value;
            }
        }

        private string _pressureSensorPortSelected;

        public string PressureSensorPortSelected
        {
            get => _pressureSensorPortSelected;
            set
            {
                _ = Set(ref _pressureSensorPortSelected, value);
                if (_pressureSensorDriver != null) _pressureSensorDriver.PortStr = value;
            }
        }

        private string _lightingPortSelected;

        public string LightingPortSelected
        {
            get => _lightingPortSelected;
            set
            {
                if (!Set(ref _lightingPortSelected, value)) return;
                if (_lightingDriver != null) _lightingDriver.PortStr = value;
            }
        }

        private int _laserTypeSelectedIndex;

        public int LaserTypeSelectedIndex
        {
            get => _laserTypeSelectedIndex;
            set
            {
                _ = Set(ref _laserTypeSelectedIndex, value);
                _laserDriver?.SetLaserType(value);
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
                _pwrDriver.ConnectToPort();
            }
        }

        private bool _pwrAllDiodesSwitch;

        public bool PwrAllDiodesSwitch
        {
            get => _pwrAllDiodesSwitch;
            set
            {
                Set(ref _pwrAllDiodesSwitch, value);
                OnSetAllDiodes(value);
            }
        }

        private int _pwrAllDiodeAmplitude;

        public int PwrAllDiodeAmplitude
        {
            get => _pwrAllDiodeAmplitude;
            set => Set(ref _pwrAllDiodeAmplitude, value);
        }

        private bool _fPwrSwitchCh0;

        public bool PwrSwitchCh0
        {
            get => _fPwrSwitchCh0;
            set
            {
                _ = Set(ref _fPwrSwitchCh0, value);
                if (value) _pwrDriver?.SetChannelOn(0);
                else _pwrDriver?.SetChannelOff(0);
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
                    _pwrDriver?.SetChannelOn(0);
                }
                else
                {
                    LabelPwrChannel0Bias = Resources.LabelOffsetVoltage;
                }

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
                    _pwrDriver?.SetChannelOn(1);
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
                if (value) _pwrDriver?.SetChannelOn(1);
                else _pwrDriver?.SetChannelOff(1);
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
                if (value) _pwrDriver?.SetChannelOn(2);
                else _pwrDriver?.SetChannelOff(2);
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
                    _pwrDriver?.SetChannelOn(2);
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
            set
            {
                Set(ref _fPwrCh2Duty, value);
                _pwrParams[2].Duty = value;
            }
        }

        private int _fPwrCh2Phase;

        public int PwrCh2Phase
        {
            get => _fPwrCh2Phase;
            set
            {
                Set(ref _fPwrCh2Phase, value);
                _pwrParams[2].Phase = value;
            }
        }

        private int _fPwrCh2MaxVoltage;

        public int PwrCh2MaxVoltage
        {
            get => _fPwrCh2MaxVoltage;
            set
            {
                Set(ref _fPwrCh2MaxVoltage, value);
                _pwrParams[2].MaxVolts = value;
            }
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
                if (value) _pwrDriver?.SetChannelOn(3);
                else _pwrDriver?.SetChannelOff(3);
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
                    _pwrDriver?.SetChannelOn(3);
                }
                else
                {
                    LabelPwrChannel3Bias = Resources.LabelOffsetVoltage;
                }

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
                    _pwrDriver?.SetChannelOn(4);
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
                    _pwrDriver?.SetChannelOn(4);
                else
                    _pwrDriver?.SetChannelOff(4);
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
                if (value) _pwrDriver?.SetChannelOn(5);
                else _pwrDriver?.SetChannelOff(5);
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
                    _pwrDriver?.SetChannelOn(5);
                }
                else
                {
                    LabelPwrChannel5Bias = Resources.LabelOffsetVoltage;
                    _pwrDriver?.SetChannelOff(5);
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
                _pumpDriver?.SetPumpActive(value);
            }
        }

        private bool _fIsConfocalActive;

        public bool IsConfocalActive
        {
            get => _fIsConfocalActive;
            set
            {
                Set(ref _fIsConfocalActive, value);
                _confocalDriver?.SetMeasuredActive(value);
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
                    _dispenserDriver.ConnectToPort();
                    CollectDispenserData();
                }
                else _dispenserDriver.Disconnect();
            }
        }

        private int _dispenserTypeSelectedIndex;

        public int DispenserTypeSelectedIndex
        {
            get => _dispenserTypeSelectedIndex;
            set => Set(ref _dispenserTypeSelectedIndex, value);
        }

        private bool _fIsPressureSensorPortConnected;

        public bool IsPressureSensorPortConnected
        {
            get => _fIsPressureSensorPortConnected;
            set
            {
                Set(ref _fIsPressureSensorPortConnected, value);
                _pressureSensorDriver?.ConnectToPort();
            }
        }

        private bool _isPressureSensorActive;

        public bool IsPressureSensorActive
        {
            get => _isPressureSensorActive;
            set
            {
                _ = Set(ref _isPressureSensorActive, value);
                _pressureSensorDriver?.SetMeasuring(value);
            }
        }

        private bool _isPyroPortConnected;

        public bool IsPyroPortConnected
        {
            get => _isPyroPortConnected;
            set
            {
                _ = Set(ref _isPyroPortConnected, value);
                if (value) _pyroDriver.ConnectToPort();
                else _pyroDriver.Disconnect();
            }
        }

        private bool _isPyroActive;

        public bool IsPyroActive
        {
            get => _isPyroActive;
            set
            {
                _ = Set(ref _isPyroActive, value);
                _pyroDriver.SetMeasuring(value);
            }
        }

        private float _fPyroTemperature;

        public float PyroTemperature
        {
            get => _fPyroTemperature;
            set
            {
                _ = Set(ref _fPyroTemperature, value);
                //convert pyro to termo
                if (value > 0)
                    //CurrentTemperature = Math.Round((value * value * (-0.0118)) + (value * 2.9009) - 44.827, 3);
                    CurrentTemperature = Math.Round((value * value * (-0.0173)) + (value * 3.2429) - 36.308, 3);
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
                if (value) _pressurePumpDriver?.Calibrate();
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
                    _dispenserDriver.Start();
                }
                else
                {
                    _dispenserDriver.Stop();
                    _dispenserDriver.Reset();
                }
            }
        }

        private int _fDispenserSignalType;

        public int DispenserSignalType
        {
            get => _fDispenserSignalType;
            set
            {
                _ = Set(ref _fDispenserSignalType, value);
                _dispenserDriver?.SetSignalType(_fDispenserSignalType);
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
                _dispenserDriver?.SetChannel(value);
                Set(ref _fDispenserChannel, value);
            }
        }

        private int _fDispenserFrequency;

        public int DispenserFrequency
        {
            get => _fDispenserFrequency;
            set
            {
                _dispenserDriver?.SetFrequency(value);
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
                _pressurePumpDriver?.SetTrigger(value);
            }
        }

        private int _fAirSupportPressure;

        public int AirSupportPressure
        {
            get => _fAirSupportPressure;
            set
            {
                _ = Set(ref _fAirSupportPressure, value);
                _pressurePumpDriver?.SetPressure(value);
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

        private bool _isLightingPortConnected;

        public bool IsLightingPortConnected
        {
            get => _isLightingPortConnected;
            set
            {
                _ = Set(ref _isLightingPortConnected, value);
                if (value)
                {
                    _lightingDriver?.ConnectToPort();
                    _lightingDriver?.SetUvChannel(LightingUvChannelValue);
                    _lightingDriver?.SetBlueChannel(LightingBlueChannelValue);
                    _lightingDriver?.SetGreenRedChannel(LightingGreenRedChannelValue);
                }
                else _lightingDriver.ClosePort();
            }
        }

        private string _channelManagementTitle;

        public string ChannelManagementTitle
        {
            get => _channelManagementTitle;
            set => Set(ref _channelManagementTitle, value);
        }

        private bool _isLightingOn;

        public bool IsLightingOn
        {
            get => _isLightingOn;
            set
            {
                Set(ref _isLightingOn, value);
                if (value) _lightingDriver?.On();
                else _lightingDriver?.Off();
            }
        }

        private int _lightingUvChannelValue;

        public int LightingUvChannelValue
        {
            get => _lightingUvChannelValue;
            set
            {
                _ = Set(ref _lightingUvChannelValue, value);
                _lightingDriver?.SetUvChannel(value);
            }
        }

        private int _lightingBlueChannelValue;

        public int LightingBlueChannelValue
        {
            get => _lightingBlueChannelValue;
            set
            {
                _ = Set(ref _lightingBlueChannelValue, value);
                _lightingDriver?.SetBlueChannel(value);
            }
        }

        private int _lightingGreenRedChannelValue;

        public int LightingGreenRedChannelValue
        {
            get => _lightingGreenRedChannelValue;
            set
            {
                if (Set(ref _lightingGreenRedChannelValue, value))
                    _lightingDriver?.SetGreenRedChannel(value);
            }
        }

        private int _lightingGeneralChannelValue;

        public int LightingGeneralChannelValue
        {
            get => _lightingGeneralChannelValue;
            set
            {
                if (Set(ref _lightingGeneralChannelValue, value))
                    _lightingDriver.SetGeneralChannel(value);
            }
        }

        private Visibility _multiChannelVisibility;

        public Visibility MultiChannelVisibility
        {
            get => _multiChannelVisibility;
            set => Set(ref _multiChannelVisibility, value);
        }

        private Visibility _allChannelVisibility;

        public Visibility AllChannelVisibility
        {
            get => _allChannelVisibility;
            set => Set(ref _allChannelVisibility, value);
        }

        private bool _isSingleLighting;

        public bool IsSingleLighting
        {
            get => _isSingleLighting;
            set
            {
                _ = Set(ref _isSingleLighting, value);
                if (value)
                {
                    MultiChannelVisibility = Visibility.Collapsed;
                    AllChannelVisibility = Visibility.Visible;
                    ChannelManagementTitle = LabelLightingChannelsAll;
                }
                else
                {
                    MultiChannelVisibility = Visibility.Visible;
                    AllChannelVisibility = Visibility.Collapsed;
                    ChannelManagementTitle = LabelLightingChannels;
                }
            }
        }

        private bool _isDynamicManageChannels;

        public bool IsDynamicManageChannels
        {
            get => _isDynamicManageChannels;
            set
            {
                Set(ref _isDynamicManageChannels, value);
                _lightingDriver.IsDynamicManageChannels = value;
            }
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
        public ObservableCollection<string> DispenserPortCollection { get; set; }
        public ObservableCollection<string> DispenserModeCollection { get; set; }
        public ObservableCollection<float> PumpingSpeedCollection { get; set; }
        public ObservableCollection<int> AirSupportPressureCollection { get; set; }
        public ObservableCollection<string> PressureSensorPortCollection { get; set; }
        public ObservableCollection<string> LightingPortCollection { get; set; }

        #endregion

        #region StaticLabels

        public static string LabelOn => Resources.LabelOn;
        public static string LogMessageHeader => Resources.LogHeaderColumn1Name;
        public static string LabelPumpOperation => Resources.PumpOperationTitle;
        public static string LabelConfocalData => Resources.LabelConfocalData;
        public static string LabelConfocalSetter => Resources.LabelConfocalSetter;
        public static string LabelPumpActive => Resources.LabelPumpActive;
        public static string LabelPortConnection => Resources.LabelPortConnection;
        public static string LabelAllDiodes => Resources.LabelAllDiodes;
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
        public static string DispenserTypeLabel => Resources.DispenserTypeLabel;
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
        public static string PressureOperationTitle => Resources.PressureOperationTitle;
        public static string LabelLightingActive => Resources.LabelLightingActive;
        public static string LabelLightingChannels => Resources.LabelLightingChannels;
        public static string LabelLightingChannelsAll => Resources.LabelLightingChannelsAll;
        public static string LabelDynamicManageChannels => Resources.LabelDynamicManageChannels;

        #endregion

        #region Commands

        public ICommand QuitCommand { get; }
        public ICommand MinimizedCommand { get; }
        public ICommand MaximizedCommand { get; }
        public ICommand StandardSizeCommand { get; }
        public ICommand SetLaserPwrCommand { get; }
        public ICommand SetDiodesParam { get; }
        public ICommand ToggleLaserEmit { get; }
        public ICommand StartPumpCommand { get; }
        public ICommand TogglePumpCommand { get; }
        public ICommand ToggleDispenserCommand { get; }
        public ICommand ReadChannelParamsCommand { get; }
        public ICommand WriteChannelParamsCommand { get; }
        public ICommand StartMaxPressureImpulse { get; }
        public ICommand TestInputStart { get; }
        public ICommand TestOutputStart { get; }
        public ICommand TestInputStop { get; }
        public ICommand TestOutputStop { get; }
        public ICommand TestInputPump { get; }
        public ICommand TestOutputPump { get; }

        #endregion

        public MainModel()
        {
            // Data context
            // _fDbContext = new ApplicationContext();
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
            IncomingPumpPortCollection =
                new ObservableCollection<string>(new PortList().GetPortList(IncomingPumpPortSelected));
            OutgoingPumpPortSelected = Settings.Default.OutloginPumpPortSelected;
            OutcomingPumpPortCollection =
                new ObservableCollection<string>(new PortList().GetPortList(OutgoingPumpPortSelected));
            LaserPortSelected = Settings.Default.LaserPortSelected;
            LaserPortCollection = new ObservableCollection<string>(new PortList().GetPortList(LaserPortSelected));
            PyroPortSelected = Settings.Default.PyroPortSelected;
            DispenserPortSelected = Settings.Default.DispenserPortSelected;
            PressureSensorPortSelected = Settings.Default.PressureSensorPortSelected;
            PyroPortCollection = new ObservableCollection<string>(new PortList().GetPortList(PyroPortSelected));
            PwrPortSelected = Settings.Default.PwrPortSelected;
            PwrPortCollection = new ObservableCollection<string>(new PortList().GetPortList(PyroPortSelected));
            LaserHistoryCollection = new ObservableCollection<int>() { 100, 150, 200, 250, 300, 350, 400 };
            DispenserPortCollection =
                new ObservableCollection<string>(new PortList().GetPortList(DispenserPortSelected));
            PressureSensorPortCollection =
                new ObservableCollection<string>(new PortList().GetPortList(PressureSensorPortSelected));
            LightingPortCollection =
                new ObservableCollection<string>(new PortList().GetPortList(PressureSensorPortSelected));
            LightingPortSelected = Settings.Default.LightingPortSelected;
            // AirSupportPortCollection = new ObservableCollection<string>(new PortList().GetPortList(AirSupportPortSelected));
            DispenserModeCollection = new ObservableCollection<string>(new PortList().GetDispenserModes());
            PumpingSpeedCollection = new ObservableCollection<float>() { 0f, 0.5f, 2.5f, 11f, 25f, 45f, 65f, 150f };
            AirSupportPressureCollection = new ObservableCollection<int> { 0, 1, 2, 5, 10, 20, 50, 150, 200 };

            //Other
            CurWindowState = WindowState.Normal;
            //Exclude
            _dispenserDriver = new DispenserDriver();
            _dispenserDriver.SetLogMessage += AddLogMessage;
            _dispenserDriver.PortStr = Settings.Default.DispenserPortSelected;
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
            DispenserTypeSelectedIndex = Settings.Default.DispenserTypeSelectedIndex;
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
            LightingUvChannelValue = Settings.Default.LightingUvChannelValue;
            LightingBlueChannelValue = Settings.Default.LightingBlueChannelValue;
            LightingGreenRedChannelValue = Settings.Default.LightingGreenRedChannelValue;
            IsSingleLighting = Settings.Default.IsSingleLighting;
            //init command area
            QuitCommand = new LambdaCommand(OnQuitApp);
            MinimizedCommand = new LambdaCommand(OnMinimizedCommandExecute);
            MaximizedCommand = new LambdaCommand(OnMaximizedCommandExecute);
            StandardSizeCommand = new LambdaCommand(OnStandardSizeCommand);
            SetLaserPwrCommand = new LambdaCommand(OnSetLaserPower);
            SetDiodesParam = new LambdaCommand(OnSetDiodesParam);
            ToggleLaserEmit = new LambdaCommand(OnToggleLaserEmit);
            StartPumpCommand = new LambdaCommand(OnStartPump);
            TogglePumpCommand = new LambdaCommand(OnTogglePump);
            ToggleDispenserCommand = new LambdaCommand(OnToggleDispenserActive);
            ReadChannelParamsCommand = new LambdaCommand(OnReadChannelParamsCommand);
            WriteChannelParamsCommand = new LambdaCommand(OnWriteChannelParamsCommand);
            StartMaxPressureImpulse = new LambdaCommand(OnStartMaxPressureImpulse);
            TestInputPump = new LambdaCommand(OnTestInputPump);
            TestOutputPump = new LambdaCommand(OnTestOutputPump);
            TestInputStart = new LambdaCommand(OnTestInputStart);
            TestOutputStart = new LambdaCommand(OnTestOutputStart);
            TestInputStop = new LambdaCommand(OnTestInputStop);
            TestOutputStop = new LambdaCommand(OnTestOutputStop);
            //Drivers area
            _confocalDriver = new ConfocalDriver();
            _confocalDriver.ObtainedDataEvent += SetUpMeasuredLevel;
            _confocalDriver.SetLogMessage += AddLogMessage;

            _pumpDriver = new PumpDriver();
            _pumpDriver.SetLogMessage += AddLogMessage;
            _pumpDriver.PortStrInput = Settings.Default.IncomingPumpPortSelected;
            _pumpDriver.PortStrOutput = Settings.Default.OutloginPumpPortSelected;
            _pumpDriver.ToggleTwoPump(Settings.Default.IsTwoPump);
            _pumpDriver.SetInputSpeed += SetIncomingPumpSpeedLabel;
            _pumpDriver.SetOutputSpeed += SetOutgoingPumpSpeedLabel;
            _pumpDriver.SetInputQueue += SetIncomingPumpQueue;
            _pumpDriver.SetOutputQueue += SetOutgoingPumpQueue;
            _pumpDriver.SetRequiredLvl(ConfocalLevelSetter);

            _pwrDriver = new PwrDriver();
            _pwrDriver.SetLogMessage += AddLogMessage;
            _pwrDriver.SetChannelParameters += ChannelParams;
            _pwrDriver.PortStr = Settings.Default.PwrPortSelected;

            _laserDriver = new LaserDriver();
            _laserDriver.SetLogMessage += AddLogMessage;
            _laserDriver.PortString = Settings.Default.LaserPortSelected;
            LaserTypeSelectedIndex = Settings.Default.LaserTypeSelectedIndex;

            _pyroDriver = new PyroDriver();
            _pyroDriver.SetLogMessage += AddLogMessage;
            _pyroDriver.EventHandler += PyroHandler;
            _pyroDriver.PortStr = Settings.Default.PyroPortSelected;

            _pressurePumpDriver = new PressurePumpDriver();

            _pressureSensorDriver = new PressureSensorDriver(PressureSensorPortSelected);
            _pressureSensorDriver.EventHandler += PressureSensorHandler;

            _lightingDriver = new LightingDriver
            {
                PortStr = Settings.Default.LightingPortSelected
            };

            AddLogMessage("Application Started");
        }

        private void AddLogMessage(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                LogCollection.Insert(0, new LogItem(DateTime.Now, message));
                // _fDbContext.Logs.Add(new Log { Dt = DateTime.Now, Message = message, Code = 0 });
            });
        }

        private void PyroHandler(float temperature)
        {
            Application.Current.Dispatcher.Invoke(() => { PyroTemperature = temperature; });
            //_fDbContext.Temperatures.Add(new Temperature { Dt = DateTime.Now, Tmp = temperature, CurTmp = CurrentTemperature });
        }

        private void PressureSensorHandler(float pressure, float temperature)
        {
            Application.Current.Dispatcher.Invoke(() => { PressureSensorTemperature = temperature; });
            Application.Current.Dispatcher.Invoke(() => { PressureSensorValue = pressure; });
        }

        private void OnQuitApp(object p)
        {
            try
            {
                //_ = _fDbContext.SaveChanges();
            }
            catch (Exception e)
            {
                AddLogMessage(e.Message);
            }

            _pumpDriver?.Disconnect();
            _pwrDriver?.Disconnect();
            _laserDriver?.Disconnect();
            _pyroDriver?.Disconnect();
            _dispenserDriver?.Disconnect();
            _lightingDriver?.ClosePort();

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
            Settings.Default.DispenserTypeSelectedIndex = DispenserTypeSelectedIndex;
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
            Settings.Default.LightingUvChannelValue = LightingUvChannelValue;
            Settings.Default.LightingBlueChannelValue = LightingBlueChannelValue;
            Settings.Default.LightingGreenRedChannelValue = LightingGreenRedChannelValue;
            Settings.Default.IsSingleLighting = IsSingleLighting;
            Settings.Default.LightingPortSelected = LightingPortSelected;
            Settings.Default.Save();

            //Remove this blog later
            //TextWriter tw = new StreamWriter("SavedList.txt");
            //foreach (var s in LogCollection)
            //    tw.WriteLine(s.Message);
            //tw.Close();

            //_fDbContext.Dispose();
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
            _laserDriver.SetPower(LaserPowerSetter);
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

        private void OnSetAllDiodes(bool value)
        {
            PwrSwitchCh0 = value;
            Thread.Sleep(250);
            PwrSwitchCh1 = value;
            Thread.Sleep(250);
            PwrSwitchCh2 = value;
            Thread.Sleep(250);
            PwrSwitchCh3 = value;
        }

        private void OnReadChannelParamsCommand(object sender)
        {
            var ch = (int)sender;
            _pwrDriver?.GetChanelData(Convert.ToByte(ch));
        }

        private void OnWriteChannelParamsCommand(object sender)
        {
            var ch = (int)sender;
            var pi = GetChannelParams(ch);
            _pwrDriver?.WriteChannelData(ch, pi);
        }

        private void OnSetDiodesParam(object sender)
        {
            _pwrDriver?.SetDiodesParam(PwrAllDiodeAmplitude);
        }

        private void OnStartMaxPressureImpulse(object sender)
        {
            _pressurePumpDriver.StartImpulse();
        }

        private void OnTestInputPump(object sender)
        {
            _pumpDriver?.TestInputPump();
        }

        private void OnTestOutputPump(object sender)
        {
            _pumpDriver?.TestOutputPump();
        }

        private void OnTestInputStart(object sender)
        {
            _pumpDriver?.TestInputStart();
        }

        private void OnTestOutputStart(object sender)
        {
            _pumpDriver?.TestOutputStart();
        }

        private void OnTestInputStop(object sender)
        {
            _pumpDriver?.TestInputStop();
        }

        private void OnTestOutputStop(object sender)
        {
            _pumpDriver?.TestOutputStop();
        }

        private void SetUpMeasuredLevel(DistMeasureRes lvl)
        {
            ConfocalLevel = Math.Round(lvl.Dist, 5);
            _pumpDriver?.SetMeasuredLevel(lvl);
            ConfocalLog = _confocalDriver.GetLastFragment();
        }

        private void SetIncomingPumpSpeedLabel(string speed)
        {
            Application.Current.Dispatcher.Invoke(() => IncomingPumpSpeed = speed);
        }

        private void SetOutgoingPumpSpeedLabel(string speed)
        {
            Application.Current.Dispatcher.Invoke(() => OutgoingPumpSpeed = speed);
        }

        private void SetIncomingPumpQueue(string queue)
        {
            Application.Current.Dispatcher.Invoke(() => IncomingPumpQueue = queue);
        }

        private void SetOutgoingPumpQueue(string queue)
        {
            Application.Current.Dispatcher.Invoke(() => OutgoingPumpQueue = queue);
        }

        private void CollectSineData()
        {
            var data = new DispenserSineWaveData()
            { TimeToverall = DispenserHToverall, V0 = DispenserHv0, VPeak = DispenserHVpeak };
            _dispenserDriver?.SetSineWaveData(data);
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
            _dispenserDriver?.SetPulseWaveData(data);
        }

        private void CollectDispenserData()
        {
            CollectSineData();
            CollectPulseData();
            _dispenserDriver?.SetChannel(_fDispenserChannel);
            _dispenserDriver?.SetFrequency(_fDispenserFrequency);
        }

        private void ChannelParams(int channel, DataModels.PwrItem pi)
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