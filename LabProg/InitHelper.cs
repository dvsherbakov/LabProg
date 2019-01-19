﻿using System.Windows;
using System.IO.Ports;
using System.Windows.Controls;

namespace LabProg
{
    public partial class MainWindow : Window
    {
        private void InitInternalComponents()
        {
            CbPumpPort.Items.Clear();
            CbMirrorPort.Items.Clear();
            foreach (var s in SerialPort.GetPortNames())
            {
                CbPumpPort.Items.Add(s);
                CbMirrorPort.Items.Add(s);
                CbPowerPort.Items.Add(s);
            }
            InitPwrItems();
        }

        private void InitPwrItems()
        {
            var dl= new PwrModes();
            CbModeCh1.Items.Clear();
            CbModeCh1.ItemsSource = dl.GetValues();
            CbModeCh1.SelectedValuePath = "Key";
            CbModeCh1.DisplayMemberPath = "Value";
            CbModeCh0.SelectedIndex = Properties.Settings.Default.PwrModeCh0;
            CbModeCh1.SelectedIndex = Properties.Settings.Default.PwrModeCh1;
            CbModeCh2.SelectedIndex = Properties.Settings.Default.PwrModeCh2;
            CbModeCh3.SelectedIndex = Properties.Settings.Default.PwrModeCh3;
            CbModeCh4.SelectedIndex = Properties.Settings.Default.PwrModeCh4;
            CbModeCh5.SelectedIndex = Properties.Settings.Default.PwrModeCh5;
            CbPowerPort.SelectedIndex = Properties.Settings.Default.PwrPortIndex;
            SetChanellBiasTitle(1);
        }

        
    }
}