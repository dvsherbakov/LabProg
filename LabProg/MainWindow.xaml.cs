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

        public MainWindow()
        {
            InitializeComponent();
            LogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Приложение запущено" });
            InitInternalComponents();

            _pwrSerial = new PwrSerial(CbPowerPort.Text);
            _pumpSerial = new PumpSerial(CbPumpPort.Text);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CbConfocalLevel.Text = ConfocalLb.Text;
        }

        private void CbLaserType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_laserSerial != null)
            {
                _laserSerial.SetLaserType(((ComboBox)sender).SelectedIndex);
            }
        }

        private void CbCamType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = ((ComboBox)sender).SelectedIndex;
            if (index == 0)
            {
                PcoCommands.Visibility = Visibility.Visible;
                AndorCommands.Visibility = Visibility.Collapsed;
            }
            else
            {
                PcoCommands.Visibility = Visibility.Collapsed;
                AndorCommands.Visibility = Visibility.Visible;
            }
        }

        private void TbTwoPump_Checked(object sender, RoutedEventArgs e)
        {
            TbTwoPump.Text = "Два насоса";
            if (TbFirstPump!=null) TbFirstPump.Text = "Порт притока";
            if (TbSecondPump != null) TbSecondPump.Visibility = Visibility.Visible;
            if (CbPumpSecondPort != null) CbPumpSecondPort.Visibility = Visibility.Visible;
            if (_pumpSecondSerial == null) _pumpSecondSerial = new PumpSerial(CbPumpSecondPort.Text);
        }

        private void TbTwoPump_UnChecked(object sender, RoutedEventArgs e)
        {
            TbTwoPump.Text = "Один насос";
            if (TbFirstPump != null) TbFirstPump.Text = "Порт насоса";
            if (TbSecondPump != null) TbSecondPump.Visibility = Visibility.Collapsed;
            if (CbPumpSecondPort != null) CbPumpSecondPort.Visibility = Visibility.Collapsed;
        }
    }
}
