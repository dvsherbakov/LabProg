using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabProg.ViewModels
{
    class MainViewModel : ViewModel
    {
        private List<Cams.CamType> CamList;

        public MainViewModel()
        {
            CamList = new List<Cams.CamType> { new Cams.CamType(0, "PCO"), new Cams.CamType(1, "Andor") };
        }
    }
}
