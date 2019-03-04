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
using LabProg.Resources;

namespace LabProg
{
    
    public partial class MainWindow : Window
    {
        readonly PwrSerial pwrSerial;
        readonly PumpSerial pumpSerial;
        private PyroSerial _pyroSerial;

        public MainWindow()
        {
            InitializeComponent();
            LogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Приложение запущено" });
            InitInternalComponents();
            
            pwrSerial = new PwrSerial(CbPowerPort.Text);
            pumpSerial = new PumpSerial(CbPumpPort.Text);
            _pyroSerial = new PyroSerial(CbPyroPort.Text);
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Close();
        }

       


        
    }
}
