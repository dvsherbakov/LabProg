using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LabControl.ViewModels
{
    class MainModel : ViewModel
    {
        private WindowState f_curWindowState;
        public WindowState CurWindowState
        {
            get => f_curWindowState;
            set => Set(ref f_curWindowState, value);
        }
    }
}
