using LabProg.Cams;
using System.Windows;
using System.Windows.Input;

namespace LabProg
{
    public partial class MainWindow
    {
        public ICommand StartAndorCameraCommand;

        public CameraAndor camAndor;

        private void OnStartAndorCameraCommandExecute(object p)
        {
            camAndor = new CameraAndor();
        }

        public void ExecuteFromCb(object sender, RoutedEventArgs e)
        {
            OnStartAndorCameraCommandExecute(sender);
        }
    }
}