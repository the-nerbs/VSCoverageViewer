using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using VSCoverageViewer.Interfaces;
using VSCoverageViewer.Messaging;
using VSCoverageViewer.Properties;

namespace VSCoverageViewer.ViewModels
{
    /// <summary>
    /// The main view model.
    /// </summary>
    internal sealed class MainWindowViewModel : ObservableObject,
        ICoverageGridViewModel,
        IMainToolbarViewModel,
        IMainMenuViewModel
    {
        // non-bindable:
        private readonly IMessenger _messenger;
        private readonly Dictionary<Column, bool> _columnVisibility = new Dictionary<Column, bool>();

        // bindable - outside of constructor, set through property.
        private IReadOnlyList<CoverageNodeViewModel> _rowsAsFlatList;
        private CoverageNodeViewModel _selectedRow;
        private ColumnPreset _columnVisibilityPreset;
        private double _coverageThreshold;


        /// <summary>
        /// Gets or sets the owning window.
        /// </summary>
        public Window Owner { get; set; }

        /// <inheritdoc />
        public ObservableCollection<CoverageNodeViewModel> CoverageRows { get; }
                = new ObservableCollection<CoverageNodeViewModel>();

        /// <inheritdoc />
        public CoverageNodeViewModel SelectedCoverageRow
        {
            get { return _selectedRow; }
            set { SetIfChanged(ref _selectedRow, value); }
        }

        /// <inheritdoc />
        public IReadOnlyList<CoverageNodeViewModel> RowsAsFlatList
        {
            get { return _rowsAsFlatList; }
            private set { Set(ref _rowsAsFlatList, value); }
        }

        /// <inheritdoc />
        public ColumnPreset ColumnVisibilityPreset
        {
            get { return _columnVisibilityPreset; }
            set
            {
                Set(ref _columnVisibilityPreset, value);
                SetVisibleColumns(value);
            }
        }

        /// <inheritdoc />
        public double CoverageThreshold
        {
            get { return _coverageThreshold; }
            set
            {
                Contract.Requires(0 <= value && value <= 1.0,
                                  "Coverage threshold must be between 0% and 100%",
                                  nameof(value));

                Set(ref _coverageThreshold, value);
                _messenger.Send(new ThresholdChangedMessage(value));
            }
        }


        /// <inheritdoc />
        public RelayCommand<string> OpenCmd { get; }

        /// <inheritdoc />
        public RelayCommand<string> SaveCmd { get; }

        /// <inheritdoc />
        public RelayCommand ExportCmd { get; }

        /// <inheritdoc />
        public RelayCommand ExitCmd { get; }

        /// <inheritdoc />
        public RelayCommand ShowColumnPropertiesCmd { get; }

        /// <inheritdoc />
        public RelayCommand ShowFilterDesignCmd { get; }

        /// <inheritdoc />
        public RelayCommand ExpandTreeCmd { get; }

        /// <inheritdoc />
        public RelayCommand CollapseTreeCmd { get; }

        /// <inheritdoc />
        public RelayCommand ExpandAllCmd { get; }

        /// <inheritdoc />
        public RelayCommand CollapseAllCmd { get; }


        /// <summary>
        /// Initializes a new instance of <see cref="MainWindowViewModel"/>.
        /// </summary>
        public MainWindowViewModel()
        {
            _messenger = Messenger.Default;

            // set default column visibility
            foreach (Column col in Enum.GetValues(typeof(Column)))
            {
                _columnVisibility[col] = true;
            }

            // set default coverage threshold
            double thresh = Settings.Default.ThresholdCoverageRatio;
            if (double.IsNaN(thresh) ||
                double.IsInfinity(thresh) ||
                (thresh < 0 || 1 < thresh))
            {
                thresh = 0.75;
            }
            _coverageThreshold = thresh;


            // commands
            OpenCmd = new RelayCommand<string>(OpenFile);
            SaveCmd = new RelayCommand<string>(SaveFile);
            ExportCmd = new RelayCommand(ExportCoverageSummary);
            ExitCmd = new RelayCommand(Exit);

            ExpandTreeCmd = new RelayCommand(ExpandSelectedTree, HaveSelectedNode);
            CollapseTreeCmd = new RelayCommand(CollapseSelectedTree, HaveSelectedNode);
            ExpandAllCmd = new RelayCommand(ExpandAll, HaveRows);
            CollapseAllCmd = new RelayCommand(CollapseAll, HaveRows);

            CoverageRows.CollectionChanged += CoverageRowsCollectionChanged;
        }


        /// <inheritdoc />
        public bool IsColumnVisible(Column tag)
        {
            return _columnVisibility[tag];
        }


        #region Command Implementations

        /// <summary>
        /// Opens a coverage file..
        /// </summary>
        /// <param name="path">
        /// The path to the file to open. If null or empty, the user is prompted to select a file.
        /// </param>
        public void OpenFile(string path)
        {
            bool canceled = false;

            // if we were not given a path, let the user browse for one.
            if (string.IsNullOrEmpty(path))
            {
                //TODO (testing): hide this behind a service interface
                var ofd = new OpenFileDialog
                {
                    Filter = "Coverage XML File (*.coveragexml)|*.coveragexml",
                    Multiselect = false,
                };

                try
                {
                    ofd.FileOk += CheckFileOpenOK;

                    if (ofd.ShowDialog(Owner) == true)
                    {
                        path = ofd.FileName;
                    }
                    else
                    {
                        // user cancelled
                        canceled = true;
                    }
                }
                finally
                {
                    ofd.FileOk -= CheckFileOpenOK;
                }
            }

            if (!canceled)
            {
                var reader = new CoverageReader();

                var model = reader.ReadCoverageXml(path);
                model.AdditionalData["FullPath"] = path;

                CoverageRows.Add(new CoverageNodeViewModel(model));
            }
        }

        /// <summary>
        /// Saves a coverage file.
        /// </summary>
        /// <param name="path">
        /// The path to save the file to. IF null or empty, the user is prompted to select the file path.
        /// </param>
        public void SaveFile(string path)
        {
            bool canceled = false;

            // if we were not given a path, let the user browse for one.
            if (string.IsNullOrEmpty(path))
            {
                //TODO (testing): hide this behind a service interface
                var ofd = new SaveFileDialog
                {
                    Filter = "Coverage XML File (*.coveragexml)|*.coveragexml",
                };

                if (ofd.ShowDialog(Owner) == true)
                {
                    path = ofd.FileName;
                }
                else
                {
                    // user canceled
                    canceled = true;
                }
            }

            if (!canceled)
            {
                var writer = new CoverageWriter();
                writer.WriteCoverageXml(CoverageRows.Select(vm => vm.Model), path);
            }
        }

        /// <summary>
        /// Exports a summary of the open coverage data, in HTML format.
        /// </summary>
        public void ExportCoverageSummary()
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Coverage Export (*.html)|*.html";
            sfd.OverwritePrompt = true;

            if (sfd.ShowDialog(Owner) == true)
            {
                //TODO(export): project name?
                var writer = new CoverageWriter();
                writer.WriteHtmlSummary(CoverageRows.Select(vm => vm.Model), "TESTING", sfd.FileName);
            }
        }

        /// <summary>
        /// Exits the application.
        /// </summary>
        private void Exit()
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Shows the column properties dialog.
        /// </summary>
        public void ShowColumnPropertiesDialog()
        {
            // TODO: column visibility.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Shows the filter designer dialog.
        /// </summary>
        public void ShowFilterDesignerDialog()
        {
            // TODO(filters): show filter dialog.
            throw new NotImplementedException();
        }


        /// <summary>
        /// Command predicate indicating if there are any coverage rows.
        /// </summary>
        /// <returns>True if there are any coverage rows, or false if not.</returns>
        private bool HaveRows()
        {
            return CoverageRows != null &&
                   CoverageRows.Count > 0;
        }

        /// <summary>
        /// Expands all coverage data rows.
        /// </summary>
        public void ExpandAll()
        {
            foreach (var row in RowsAsFlatList)
            {
                row.IsExpanded = true;
            }
        }

        /// <summary>
        /// Collapses all coverage data rows.
        /// </summary>
        public void CollapseAll()
        {
            foreach (var row in RowsAsFlatList)
            {
                row.IsExpanded = false;
            }
        }


        /// <summary>
        /// Command predicate indicating if there is a selected coverage node.
        /// </summary>
        /// <returns>True if there is a selected coverage node, or false if not.</returns>
        private bool HaveSelectedNode()
        {
            return SelectedCoverageRow != null;
        }

        /// <summary>
        /// Expands the selected node and all of its children.
        /// </summary>
        private void ExpandSelectedTree()
        {
            Action<CoverageNodeViewModel> expandNode = null;
            expandNode = 
                (vm) =>
                {
                    vm.IsExpanded = true;

                    foreach (var item in vm.Children)
                    {
                        expandNode(item);
                    }
                };

            expandNode(SelectedCoverageRow);
        }

        /// <summary>
        /// Collapses the selected node and all of its children.
        /// </summary>
        private void CollapseSelectedTree()
        {
            Action<CoverageNodeViewModel> collapseNode = null;
            collapseNode =
                (vm) =>
                {
                    vm.IsExpanded = false;

                    foreach (var item in vm.Children)
                    {
                        collapseNode(item);
                    }
                };

            collapseNode(SelectedCoverageRow);
        }

        #endregion


        /// <summary>
        /// Sets the visible columns based on a preset.
        /// </summary>
        /// <param name="preset">The column visibility preset.</param>
        private void SetVisibleColumns(ColumnPreset preset)
        {
            SetVisibleColumns(ColumnPresetLists.GetPreset(preset));
        }

        /// <summary>
        /// Sets the visible columns.
        /// </summary>
        /// <param name="columns">A collection of the columns to make visible.</param>
        private void SetVisibleColumns(IEnumerable<Column> columns)
        {
            foreach (Column col in _columnVisibility.Keys.ToArray())
            {
                _columnVisibility[col] = false;
            }

            foreach (Column col in columns)
            {
                _columnVisibility[col] = true;
            }

            SendColumnVisibilityChanged();
        }



        /// <summary>
        /// Handles the coverage collection changing.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void CoverageRowsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var flatList = new List<CoverageNodeViewModel>();

            foreach (var vm in CoverageRows)
            {
                AppendRows(vm, flatList);
            }

            RowsAsFlatList = flatList;
            SendThresholdChanged(CoverageThreshold);
        }

        /// <summary>
        /// Appends a coverage node and all children in such a way as to flatten the coverage data tree.
        /// </summary>
        /// <param name="vm">The coverage data node.</param>
        /// <param name="flatList">The list to add rows to.</param>
        private static void AppendRows(CoverageNodeViewModel vm, List<CoverageNodeViewModel> flatList)
        {
            flatList.Add(vm);

            foreach (var child in vm.Children)
            {
                AppendRows(child, flatList);
            }
        }


        /// <summary>
        /// Handles an attempt to open a file, and verifies that the file is OK.
        /// </summary>
        /// <param name="sender">The <see cref="FileDialog"/>.</param>
        /// <param name="e">The event data.</param>
        private void CheckFileOpenOK(object sender, CancelEventArgs e)
        {
            var dialog = sender as FileDialog;
            string filePath = dialog.FileName;

            foreach (var vm in CoverageRows)
            {
                object data;
                if (vm.Model.AdditionalData.TryGetValue("FullPath", out data) &&
                    StringComparer.InvariantCultureIgnoreCase.Equals(filePath, data as string))
                {
                    // we already have this file open.
                    e.Cancel = true;
                    break;
                }
            }
        }


        #region Messaging

        /// <summary>
        /// Sends an application message indicating that the coverage threshold has changed.
        /// </summary>
        /// <param name="newValue">The new threshold.</param>
        private void SendThresholdChanged(double newValue)
        {
            _messenger.Send(new ThresholdChangedMessage(newValue));
        }

        /// <summary>
        /// Sends an application message indicating that the column visibility has changed.
        /// </summary>
        private void SendColumnVisibilityChanged()
        {
            _messenger.Send(new ColumnVisibilityChangedMessage());
        }

        //TODO(filters): 'filter changed' messages
        // private void SendFilterChanged(IFilter newFilter)
        // {
        //     _messenger.Send(new FilterChangedMessage(newFilter));
        // }

        #endregion
    }
}
