﻿using LabProg.Cams;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LabProg
{
    public partial class MainWindow
    {
        private ICommand f_StartAndorCameraCommand;

        private CameraAndor camAndor;

        private void OnStartAndorCameraCommandExecute(object p)
        {
            camAndor = new CameraAndor(AddLogBoxMessage);
        }

        private void ExecuteFromCb(object sender, RoutedEventArgs e)
        {
            OnStartAndorCameraCommandExecute(sender);
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
    }
}