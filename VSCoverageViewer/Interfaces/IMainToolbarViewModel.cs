using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;

namespace VSCoverageViewer.Interfaces
{
    interface IMainToolbarViewModel : INotifyPropertyChanged
    {
        ColumnPreset ColumnVisibilityPreset { get; set; }
        double CoverageThreshold { get; set; }

        RelayCommand<string> OpenCmd { get; }
        RelayCommand ShowColumnPropertiesCmd { get; }
        RelayCommand ShowFilterDesignCmd { get; }
    }
}
