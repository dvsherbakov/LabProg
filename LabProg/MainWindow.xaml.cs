using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace LabProg
{
    
    public partial class MainWindow : Window
    {
        readonly PwrSerial pwrSerial;
        readonly PumpSerial pumpSerial;

        public MainWindow()
        {
            InitializeComponent();
            LogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Приложение запущено" });
            InitInternalComponents();
            
            pwrSerial = new PwrSerial(CbPowerPort.Text);
            pumpSerial = new PumpSerial(CbPumpPort.Text);
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.PwrModeCh0 = CbModeCh0.SelectedIndex;
            Properties.Settings.Default.PwrModeCh1 = CbModeCh1.SelectedIndex;
            Properties.Settings.Default.PwrModeCh2 = CbModeCh2.SelectedIndex;
            Properties.Settings.Default.PwrModeCh3 = CbModeCh3.SelectedIndex;
            Properties.Settings.Default.PwrModeCh4 = CbModeCh4.SelectedIndex;
            Properties.Settings.Default.PwrModeCh5 = CbModeCh5.SelectedIndex;
            Properties.Settings.Default.PwrPortIndex = CbPowerPort.SelectedIndex;
            Properties.Settings.Default.Save();
        }

        
    }
}
