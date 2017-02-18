using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;
using VSCoverageViewer.Views;

namespace VSCoverageViewer.Interfaces
{
    /// <summary>
    /// Interface for objects which are bindable to an instance of <see cref="MainMenuStrip"/>.
    /// </summary>
    interface IMainMenuViewModel
    {
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
        /// Gets a command which exits the application.
        /// </summary>
        RelayCommand ExitCmd { get; }
    }
}
