using LabProg.Cams;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LabProg
{
    public partial class MainWindow
    {
        private ICommand f_StartAndorCameraCommand;

        private CameraAndor camAndor;

        private void OnStartAndorCameraCommandExecute(object p)
        {
            camAndor = new CameraAndor(AddLogBoxMessage);
            camAndor.CommonEvent += TransferImage;
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

        public delegate void PutImageDelegate(Bitmap frame);

        
        
        private void PI(Bitmap frame)
        {
            BitmapImage bmpImage = BitmapToImageSource(frame);
            PictureBox1.Source = null;
            PictureBox1.Source = bmpImage;
            
        }


        private void TransferImage(Bitmap frame)
        {
            PutImageDelegate putImage = PI;
            putImage(frame);
            LogBox.Items.Insert(0, new LogBoxItem { Dt = DateTime.Now, LogText = "Получено изображение с камеры" });
        }

        private void StartAndorCamSeries(object sender, RoutedEventArgs e)
        {
            if (camAndor != null)
            {
                camAndor.StartCommonTimer();
            }
            else
            {
                LogBox.Items.Insert(0, new LogBoxItem { Dt=System.DateTime.Now, LogText="Камера не инициализирована"});
            }
        }
    }
}