using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;
using VSCoverageViewer.ViewModels;

namespace VSCoverageViewer.Interfaces
{
    interface ICoverageGridViewModel : INotifyPropertyChanged
    {
        IReadOnlyList<CoverageNodeViewModel> RowsAsFlatList { get; }

        CoverageNodeViewModel SelectedCoverageRow { get; }

        RelayCommand ExpandTreeCmd { get; }
        RelayCommand CollapseTreeCmd { get; }
        RelayCommand ExpandAllCmd { get; }
        RelayCommand CollapseAllCmd { get; }

        bool IsColumnVisible(Column tag);
    }
}
