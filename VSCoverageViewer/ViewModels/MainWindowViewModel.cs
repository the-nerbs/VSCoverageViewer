﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using VSCoverageViewer.Interfaces;
using VSCoverageViewer.Messaging;
using VSCoverageViewer.Models;
using VSCoverageViewer.Properties;
using VSCoverageViewer.Views;

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
        private readonly ObservableCollection<CoverageNodeViewModel> _rowsAsFlatList = new ObservableCollection<CoverageNodeViewModel>();

        // bindable - outside of constructor, set through property.
        private ICollectionView _rowsCollectionView;
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
        public ICollectionView RowsAsFlatList
        {
            get
            {
                if (_rowsCollectionView == null)
                {
                    _rowsCollectionView = CollectionViewSource.GetDefaultView(_rowsAsFlatList);
                    _rowsCollectionView.Filter =
                        (obj) =>
                        {
                            return (obj as CoverageNodeViewModel)?.IsVisible ?? false;
                        };

                    var liveView = _rowsCollectionView as ICollectionViewLiveShaping;
                    if (liveView != null)
                    {
                        liveView.IsLiveFiltering = true;
                        liveView.LiveFilteringProperties.Add(nameof(CoverageNodeViewModel.IsVisible));
                    }
                }

                return _rowsCollectionView;
            }
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
        public RelayCommand ReportCmd { get; }

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

        /// <inheritdoc />
        public RelayCommand RemoveNodeCmd { get; }

        /// <inheritdoc />
        public RelayCommand ReadMetadataCmd { get; }


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
            SaveCmd = new RelayCommand<string>(SaveFile, (param) => HaveRows());
            ReportCmd = new RelayCommand(CreateCoverageReport, HaveRows);
            ExitCmd = new RelayCommand(Exit);

            ShowColumnPropertiesCmd = new RelayCommand(ShowColumnPropertiesDialog);

            ExpandTreeCmd = new RelayCommand(ExpandSelectedTree, HaveSelectedNode);
            CollapseTreeCmd = new RelayCommand(CollapseSelectedTree, HaveSelectedNode);
            ExpandAllCmd = new RelayCommand(ExpandAll, HaveRows);
            CollapseAllCmd = new RelayCommand(CollapseAll, HaveRows);

            RemoveNodeCmd = new RelayCommand(RemoveSelectedNode, HaveSelectedNode);

            ReadMetadataCmd = new RelayCommand(ReadMetadata, CanReadMetadata);


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

                if (!string.IsNullOrEmpty(Settings.Default.CoverageXmlDirectory))
                {
                    ofd.InitialDirectory = Settings.Default.CoverageXmlDirectory;
                }

                try
                {
                    ofd.FileOk += CheckFileOpenOK;

                    if (ofd.ShowDialog(Owner) == true)
                    {
                        path = ofd.FileName;

                        Settings.Default.CoverageXmlDirectory = Path.GetDirectoryName(path);
                        Settings.Default.Save();
                    }
                    else
                    {
                        // user canceled
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
                var sfd = new SaveFileDialog
                {
                    Filter = "Coverage XML File (*.coveragexml)|*.coveragexml",
                };

                if (!string.IsNullOrEmpty(Settings.Default.CoverageXmlDirectory))
                {
                    sfd.InitialDirectory = Settings.Default.CoverageXmlDirectory;
                }

                if (sfd.ShowDialog(Owner) == true)
                {
                    path = sfd.FileName;

                    Settings.Default.CoverageXmlDirectory = Path.GetDirectoryName(path);
                    Settings.Default.Save();
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
        /// Creates a report of the open coverage data.
        /// </summary>
        public void CreateCoverageReport()
        {
            var config = new ReportConfigurationModel();

            // set some defaults.
            config.ProjectName = CoverageRows.FirstOrDefault()?.DisplayName;

            var configDlg = new ReportConfigurationDlg(config);
            configDlg.Owner = Owner;

            // note: nullable bools...
            if (configDlg.ShowDialog() == true)
            {
                var writer = new CoverageWriter();
                writer.WriteReport(CoverageRows.Select(vm => vm.Model), config);


                if (config.OpenWhenDone)
                {
                    Process.Start(
                        new ProcessStartInfo
                        {
                            FileName = config.DestinationPath,
                            UseShellExecute = true,
                        }
                    );
                }
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
            var dlg = new ColumnPropertiesDlg();
            dlg.Owner = Owner;

            dlg.ViewModel.SetVisibleColumns(_columnVisibility);

            // note: nullable bools...
            if (dlg.ShowDialog() == true)
            {
                var newSettings = dlg.ViewModel.GetVisibleColumns();

                SetVisibleColumns(
                    newSettings
                        .Where(kvp => kvp.Value)
                        .Select(kvp => kvp.Key)
                );
            }
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
            foreach (var row in _rowsAsFlatList)
            {
                row.SetExpandedWithoutNotifyingChildren(isExpanded: true);
            }
        }

        /// <summary>
        /// Collapses all coverage data rows.
        /// </summary>
        public void CollapseAll()
        {
            foreach (var row in _rowsAsFlatList)
            {
                row.SetExpandedWithoutNotifyingChildren(isExpanded: false);
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
                    vm.SetExpandedWithoutNotifyingChildren(isExpanded: true);

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
                    vm.SetExpandedWithoutNotifyingChildren(isExpanded: false);

                    foreach (var item in vm.Children)
                    {
                        collapseNode(item);
                    }
                };

            collapseNode(SelectedCoverageRow);
        }


        /// <summary>
        /// Removes the selected node and all of its children.
        /// </summary>
        private void RemoveSelectedNode()
        {
            if (SelectedCoverageRow != null)
            {
                CoverageNodeViewModel parent = SelectedCoverageRow.Parent;

                if (parent != null)
                {
                    // non-root node.
                    parent.RemoveChild(SelectedCoverageRow);

                    // rebuild the row list.
                    RebuildRowList();
                }
                else
                {
                    // removing a root, just remove from CoverageRows and let the
                    // CollectionChanged handler deal with updating the list.
                    CoverageRows.Remove(SelectedCoverageRow);
                }
            }
        }


        /// <summary>
        /// Command predicate indicating if metadata can be read.
        /// </summary>
        /// <returns>True if metadata can be read, false if not.</returns>
        private bool CanReadMetadata()
        {
            // only allow loading metadata if the user selected a node that is, or is under, a module node.
            return SelectedCoverageRow != null &&
                   SelectedCoverageRow.Model != null &&
                   SelectedCoverageRow.Model.ClosestAncestor(CoverageNodeType.Module) != null;
        }

        /// <summary>
        /// Reads metadata for a given coverage node.
        /// </summary>
        private void ReadMetadata()
        {
            CoverageNodeModel node = SelectedCoverageRow?.Model.ClosestAncestor(CoverageNodeType.Module);

            if (node != null)
            {
                //TODO (testing): hide this behind a service interface
                var ofd = new OpenFileDialog
                {
                    Filter = ".NET Assembly (*.exe;*.dll)|*.exe;*.dll",
                    Multiselect = false,
                    FileName = node.Name,
                };

                if (ofd.ShowDialog(Owner) == true)
                {
                    MetadataHelper helper;

                    try
                    {
                        helper = new MetadataHelper(ofd.FileName);
                    }
                    catch (BadImageFormatException)
                    {
                        MessageBox.Show(Owner, "The selected file is not a valid .NET assembly.");
                        return;
                    }

                    helper.LoadMetadataFor(node);
                }
            }
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
            RebuildRowList();
        }

        /// <summary>
        /// Rebuilds the flat list of rows.
        /// </summary>
        private void RebuildRowList()
        {
            var flatList = new List<CoverageNodeViewModel>();

            foreach (var vm in CoverageRows)
            {
                AppendRows(vm, flatList);
            }

            _rowsAsFlatList.Clear();
            foreach (var row in flatList)
            {
                _rowsAsFlatList.Add(row);
            }

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
