using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;
using VSCoverageViewer.Views;

namespace VSCoverageViewer.Interfaces
{
    /// <summary>
    /// Interface for objects which are bindable to an instance of <see cref="MainToolStrip"/>.
    /// </summary>
    interface IMainToolbarViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the current column visibility preset.
        /// </summary>
        ColumnPreset ColumnVisibilityPreset { get; set; }

        /// <summary>
        /// Gets or sets the coverage percentage which is considered good.  In the coverage grid,
        /// this is the point at which the cell highlight turns green.
        /// </summary>
        double CoverageThreshold { get; set; }


        /// <summary>
        /// Gets a command which opens a file. If the parameter is null or the empty string, then
        /// the user will be prompted to select a file to open.
        /// </summary>
        RelayCommand<string> OpenCmd { get; }

        /// <summary>
        /// Gets a command which saves a file. If the parameter is null or the empty string, then
        /// the user will be prompted to select a file to save to.
        /// </summary>
        RelayCommand<string> SaveCmd { get; }

        /// <summary>
        /// Gets a command which creates a report of coverage report.
        /// </summary>
        RelayCommand ReportCmd { get; }

        /// <summary>
        /// Gets a command which shows the column display properties dialog.
        /// </summary>
        RelayCommand ShowColumnPropertiesCmd { get; }

        /// <summary>
        /// Gets a command which shows the filter properties dialog.
        /// </summary>
        RelayCommand ShowFilterDesignCmd { get; }
    }
}
