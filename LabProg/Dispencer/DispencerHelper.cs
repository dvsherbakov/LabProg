﻿using System.Windows;
using System.Windows.Input;

namespace LabProg
{
    public partial class MainWindow
    {
        public ICommand f_StartDispencerCommand;
        public ICommand f_ConnectDispencerPortCommand;
        public ICommand f_DisconnectDispencerPortCommand;
       
        private void DispenserPortConnect(object p)
        {
            dispSerial.OpenPort();
        }

        private void DispenserPortConnect(object sender, RoutedEventArgs e)
        {
            dispSerial.OpenPort();
        }

        private void DispenserStart(object sender, RoutedEventArgs e)
        {
            dispSerial.Start();
        }

        private void DispenserStop(object sender, RoutedEventArgs e)
        {
            dispSerial.Stop();
        }
    }
}