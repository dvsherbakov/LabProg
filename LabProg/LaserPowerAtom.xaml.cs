using System.Windows;
using System.Windows.Controls;

namespace LabProg
{
    /// <summary>
    /// Interaction logic for LaserPowerAtom.xaml
    /// </summary>
    public partial class LaserPowerAtom : Window
    {
        LaserPowerAtomResult m_res { get; set; }

        public LaserPowerAtom()
        {
            InitializeComponent();
            m_res = new LaserPowerAtomResult();
            cbLaserSeriesType.SelectedIndex = 0;
            spHarmonicalSeries.Visibility = Visibility.Collapsed;
            spLineralSeries.Visibility = Visibility.Visible;
            this.DataContext = m_res;
        }

        private void cbLaserSeriesType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = ((ComboBox)sender).SelectedIndex;
            if (index==0)
            {
                spHarmonicalSeries.Visibility = Visibility.Collapsed;
                spLineralSeries.Visibility = Visibility.Visible;
                m_res.Type = LaserPowerAtomType.Linear;
                Height = 300;
            } else
            {
                spHarmonicalSeries.Visibility = Visibility.Visible;
                spLineralSeries.Visibility = Visibility.Collapsed;
                m_res.Type = LaserPowerAtomType.Harmonical;
                Height = 250;
            }
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public LaserPowerAtomResult LaserPowerAtomResult => m_res;
    }
}
