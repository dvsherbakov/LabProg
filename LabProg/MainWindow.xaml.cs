using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace LabProg
{

    public partial class MainWindow
    {
        readonly PwrSerial _pwrSerial;
        readonly PumpSerial _pumpSerial;
        private PumpSerial _pumpSecondSerial;
        private PyroSerial _pyroSerial;
        private LaserSerial _laserSerial;
        private Dispencer.DispSerial dispSerial;

        public MainWindow()
        {
            InitializeComponent();
            LogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Приложение запущено" });
            InitInternalComponents();

            _pwrSerial = new PwrSerial(CbPowerPort.Text);
            _pumpSerial = new PumpSerial(CbPumpPort.Text, Properties.Settings.Default.PumpReverse, AddLogBoxMessage);
            dispSerial = new Dispencer.DispSerial(CbDispenserPort.Text, AddLogBoxMessage);
            _pyroSerial = new PyroSerial(CbPyroPort.Text);
            _laserSerial = new LaserSerial(CbLaserPort.Text);
            _laserSerial.SetLaserType(Properties.Settings.Default.LaserType);
            DataContext = this;
        }

        private void CloseApp(object sender, RoutedEventArgs e)
        {
            //PwrChannelOff(sender, e);
            //PwrSerial.SetChannelOff(0);
            Close();
        }

        

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            lvLaserPowerItems.Items.Remove(lvLaserPowerItems.SelectedItem);
        }

        private void btLaserAddPowerCycle(object sender, RoutedEventArgs e)
        {
            LaserPowerAtom wa = new LaserPowerAtom();
            if (wa.ShowDialog() == true)
            {
                ListViewItem item = new ListViewItem
                {
                    DataContext = wa.LaserPowerAtomResult
                };
                lvLaserPowerItems.Items.Add(wa.LaserPowerAtomResult);
            }
        }
    }
}
