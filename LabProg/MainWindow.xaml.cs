﻿using System;
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
        private readonly PyroSerial _pyroSerial;
        private readonly LaserSerial _laserSerial;
        private readonly Dispencer.DispSerial dispSerial;

        public MainWindow()
        {
            InitializeComponent();
            LogBox.Items.Add(new LogBoxItem { Dt = DateTime.Now, LogText = "Приложение запущено" });
            InitInternalComponents();

            _pwrSerial = new PwrSerial(CbPowerPort.Text);
            _pumpSerial = new PumpSerial(CbPumpPort.Text, Properties.Settings.Default.PumpReverse, AddLogBoxMessage);
            dispSerial = new Dispencer.DispSerial(CbDispenserPort.Text, AddLogBoxMessage, DispathData);
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

        private void BtLaserAddPowerCycle(object sender, RoutedEventArgs e)
        {
            var wa = new LaserPowerAtom();
            if (wa.ShowDialog() != true) return;
            var item = new ListViewItem
            {
                DataContext = wa.LaserPowerAtomResult
            };
            lvLaserPowerItems.Items.Add(wa.LaserPowerAtomResult);
        }
    }
}
