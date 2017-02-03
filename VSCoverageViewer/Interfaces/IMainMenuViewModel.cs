using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;

namespace VSCoverageViewer.Interfaces
{
    interface IMainMenuViewModel
    {
        RelayCommand<string> OpenCmd { get; }
        RelayCommand<string> SaveCmd { get; }
        RelayCommand ExportCmd { get; }
        RelayCommand ExitCmd { get; }
    }
}
