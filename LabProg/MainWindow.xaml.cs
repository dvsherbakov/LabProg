using System;
using System.Windows;
using LabProg.Resources;

namespace LabProg
{

    public partial class MainWindow : Window
    {
        readonly PwrSerial pwrSerial;
        readonly PumpSerial pumpSerial;
        private PyroSerial _pyroSerial;
        private LaserSerial _laserSerial;

        public MainWindow()
        {
            InitializeComponent();
            LogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Приложение запущено" });
            InitInternalComponents();
            
            pwrSerial = new PwrSerial(CbPowerPort.Text);
            pumpSerial = new PumpSerial(CbPumpPort.Text);
            _pyroSerial = new PyroSerial(CbPyroPort.Text);
            _laserSerial = new LaserSerial(CbLaserPort.Text);
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Close();
        }        
    }
}
