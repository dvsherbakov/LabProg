using System;
using System.Windows;

namespace LabProg
{

    public partial class MainWindow
    {
        private readonly PwrSerial _pwrSerial;
        private readonly PumpSerial _pumpSerial;
        private readonly PyroSerial _pyroSerial;
        private readonly LaserSerial _laserSerial;

        public MainWindow()
        {
            InitializeComponent();
            LogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Приложение запущено" });
            InitInternalComponents();
            
            _pwrSerial = new PwrSerial(CbPowerPort.Text);
            _pumpSerial = new PumpSerial(CbPumpPort.Text);
            _pyroSerial = new PyroSerial(CbPyroPort.Text);
            _laserSerial = new LaserSerial(CbLaserPort.Text);
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            Close();
        }        
    }
}
