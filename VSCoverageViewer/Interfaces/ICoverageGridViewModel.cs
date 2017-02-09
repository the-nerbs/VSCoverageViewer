using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;
using VSCoverageViewer.ViewModels;
using VSCoverageViewer.Views;

namespace VSCoverageViewer.Interfaces
{
    /// <summary>
    /// Interface for objects which can be bound to an instance of <see cref="CoverageGrid"/>.
    /// </summary>
    internal interface ICoverageGridViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the coverage data as a flattened list.
        /// </summary>
        IReadOnlyList<CoverageNodeViewModel> RowsAsFlatList { get; }

        /// <summary>
        /// Gets or sets the selected coverage row.
        /// </summary>
        CoverageNodeViewModel SelectedCoverageRow { get; set; }

        /// <summary>
        /// Gets or sets the coverage percentage which is considered good. This is the point at
        /// which the cell highlight turns green.
        /// </summary>
        double CoverageThreshold { get; set; }


        /// <summary>
        /// Gets a command which expands a coverage node and its children.
        /// </summary>
        RelayCommand ExpandTreeCmd { get; }

        /// <summary>
        /// Gets a command which collapsed a coverage node and its children.
        /// </summary>
        RelayCommand CollapseTreeCmd { get; }

        /// <summary>
        /// Gets a command which expands all coverage nodes.
        /// </summary>
        RelayCommand ExpandAllCmd { get; }

        /// <summary>
        /// Gets a command which collapses all coverage nodes.
        /// </summary>
        RelayCommand CollapseAllCmd { get; }

        /// <summary>
        /// Gets a command which removes the selected coverage node.
        /// </summary>
        RelayCommand RemoveNodeCmd { get; }

        /// <summary>
        /// Gets a command which allows the user to select an assembly to read metadata from.
        /// </summary>
        RelayCommand ReadMetadataCmd { get; }


        /// <summary>
        /// Determines the visibility of a column.
        /// </summary>
        /// <param name="tag">The column tag.</param>
        /// <returns>True if the column is visible, or false if not.</returns>
        bool IsColumnVisible(Column tag);
    }
}
