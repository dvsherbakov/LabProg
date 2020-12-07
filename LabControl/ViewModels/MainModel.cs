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
        public ObservableCollection<string> PortCollection { get; set; }

        public static string LogMessageHeader => Properties.Resources.LogHeaderColumn1Name;
        public static string LabelPumpOperation => Properties.Resources.PumpOperationTitle;
        public static string LabelConfocalData => Properties.Resources.LabelConfocalData;
        public static string LabelConfocalSetter => Properties.Resources.LabelConfocalSetter;
        public static string LabelPumpActive => Properties.Resources.LabelPumpActive;
        public static string LabelPortConnection => Properties.Resources.LabelPortConnection;
        public static string LabelSettings => Properties.Resources.LabelSettings;

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
            PortCollection = new ObservableCollection<string>((new ClassHelpers.PortList()).GetPortList()); 
           
            CurWindowState = WindowState.Normal;
            //load params from settings
            WindowHeight = Properties.Settings.Default.WindowHeight == 0 ? 550 : Properties.Settings.Default.WindowHeight;
            WindowWidth = Properties.Settings.Default.WindowWidth == 0 ? 850 : Properties.Settings.Default.WindowWidth;
            IsTwoPump = Properties.Settings.Default.IsTwoPump;
            ConfocalLevelSetter = Properties.Settings.Default.ConfocalLevelSetter;
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
