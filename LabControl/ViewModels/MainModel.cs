using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace LabControl.ViewModels
{
    class MainModel : ViewModel
    {
        #region ModelFields
        private WindowState f_curWindowState;
        public WindowState CurWindowState
        {
            get => f_curWindowState;
            set => Set(ref f_curWindowState, value);
        }

        private string f_WindowTitle;
        public string WindowTitle
        {
            get => f_WindowTitle;
            set => Set(ref f_WindowTitle, value);
        }

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
        #endregion

        private Timer f_TestTimer;

        #region Commands
        public ICommand QuitCommand { get; }
        public ICommand MinimizedCommand { get; }
        public ICommand MaximizedCommand { get; }
        public ICommand NormalizeCommand { get; }
        #endregion
        public MainModel()
        {
            LogCollection = new ObservableCollection<ClassHelpers.LogItem>();
            WindowTitle = Properties.Resources.MainWindowTitle;
            CurWindowState = WindowState.Normal;
            WindowHeight = Properties.Settings.Default.WindowHeight == 0 ? 550 : Properties.Settings.Default.WindowHeight;
            WindowWidth = Properties.Settings.Default.WindowWidth == 0 ? 850 : Properties.Settings.Default.WindowWidth;
            //init command area
            QuitCommand = new LambdaCommand(OnQuitApp);
            MinimizedCommand = new LambdaCommand(OnMinimizedCommandExecute);
            MaximizedCommand = new LambdaCommand(OnMaximizedCommandExecute);
            NormalizeCommand = new LambdaCommand(OnMaximizedCommandExecute);
            //test area
            f_TestTimer = new Timer(2000);
            f_TestTimer.Elapsed += AddMockMessage;
            //f_TestTimer.Start();
        }

        private void AddMockMessage(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => LogCollection.Add(new ClassHelpers.LogItem(DateTime.Now, "wsnedfkjkwer;;fklwer")));
        }

        private void OnQuitApp(object p)
        {
            Properties.Settings.Default.WindowHeight = WindowHeight;
            Properties.Settings.Default.WindowWidth = WindowWidth;
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
    }
}
