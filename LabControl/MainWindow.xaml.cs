using System.Windows;
using LabControl.DataModels;

namespace LabControl
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var dt = new DataInit();
            InitializeComponent();
        }
    }
}
