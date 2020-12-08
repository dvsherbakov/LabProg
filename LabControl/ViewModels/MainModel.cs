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

        public ObservableCollection<ClassHelpers.LogItem> LogCollection { get; }
        public ObservableCollection<string> IncomingPumpPortCollection { get; set; }
        public ObservableCollection<string> OutloginPumpPortCollection { get; set; }

        private bool f_IsTwoPump;
        public bool IsTwoPump
        {
            get => f_IsTwoPump;
            set
            {
                LabelPumpCount = value ? Properties.Resources.LabelTwoPump : Properties.Resources.LabelOnePump;
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
            LogCollection = new ObservableCollection<ClassHelpers.LogItem>();
            IncomingPumpPortSelected = Properties.Settings.Default.IncomingPumpPortSelected;
            IncomingPumpPortCollection = new ObservableCollection<string>((new ClassHelpers.PortList()).GetPortList(IncomingPumpPortSelected));
            OutloginPumpPortSelected = Properties.Settings.Default.OutloginPumpPortSelected;
            OutloginPumpPortCollection = new ObservableCollection<string>((new ClassHelpers.PortList()).GetPortList(OutloginPumpPortSelected));
            CurWindowState = WindowState.Normal;
            //load params from settings
            WindowHeight = Properties.Settings.Default.WindowHeight == 0 ? 550 : Properties.Settings.Default.WindowHeight;
            WindowWidth = Properties.Settings.Default.WindowWidth == 0 ? 850 : Properties.Settings.Default.WindowWidth;
            IsTwoPump = Properties.Settings.Default.IsTwoPump;
            ConfocalLevelSetter = Properties.Settings.Default.ConfocalLevelSetter;
            LaserPowerSetter = Properties.Settings.Default.LaserPowerSetter;
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
