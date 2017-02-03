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
    sealed class MainWindowViewModel : ObservableObject,
        ICoverageGridViewModel,
        IMainToolbarViewModel,
        IMainMenuViewModel
    {
        // non-bindable:
        private readonly IMessenger _messenger;
        private readonly Dictionary<Column, bool> _columnVisibility = new Dictionary<Column, bool>();

        // bindable:
        private IReadOnlyList<CoverageNodeViewModel> _rowsAsFlatList;
        private CoverageNodeViewModel _selectedRow;
        private ColumnPreset _columnVisibilityPreset;
        private double _coverageThreshold;


        public Window Owner { get; set; }

        public ObservableCollection<CoverageNodeViewModel> CoverageRows { get; }
                = new ObservableCollection<CoverageNodeViewModel>();

        public CoverageNodeViewModel SelectedCoverageRow
        {
            get { return _selectedRow; }
            set { SetIfChanged(ref _selectedRow, value); }
        }

        public IReadOnlyList<CoverageNodeViewModel> RowsAsFlatList
        {
            get { return _rowsAsFlatList; }
            private set { Set(ref _rowsAsFlatList, value); }
        }

        public ColumnPreset ColumnVisibilityPreset
        {
            get { return _columnVisibilityPreset; }
            set
            {
                Set(ref _columnVisibilityPreset, value);
                SetVisibleColumns(value);
            }
        }

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



        public RelayCommand<string> OpenCmd { get; }
        public RelayCommand<string> SaveCmd { get; }
        public RelayCommand ExportCmd { get; }
        public RelayCommand ExitCmd { get; }
        public RelayCommand ShowColumnPropertiesCmd { get; }
        public RelayCommand ShowFilterDesignCmd { get; }

        public RelayCommand ExpandTreeCmd { get; }
        public RelayCommand CollapseTreeCmd { get; }
        public RelayCommand ExpandAllCmd { get; }
        public RelayCommand CollapseAllCmd { get; }

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
            CoverageThreshold = thresh;


            // commands
            OpenCmd = new RelayCommand<string>(OpenFile);
            SaveCmd = new RelayCommand<string>(SaveFile);
            ExportCmd = new RelayCommand(ExportCoverage);
            ExitCmd = new RelayCommand(Exit);

            ExpandTreeCmd = new RelayCommand(ExpandTree, HaveSelectedNode);
            CollapseTreeCmd = new RelayCommand(CollapseTree, HaveSelectedNode);
            ExpandAllCmd = new RelayCommand(ExpandAll, HaveRows);
            CollapseAllCmd = new RelayCommand(CollapseAll, HaveRows);

            CoverageRows.CollectionChanged += CoverageRowsCollectionChanged;
        }


        public bool IsColumnVisible(Column tag)
        {
            return _columnVisibility[tag];
        }


        #region Command Implementations

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
                    ofd.FileOk += CheckFileOK;

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
                    ofd.FileOk -= CheckFileOK;
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

        public void ExportCoverage()
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

        private void Exit()
        {
            Application.Current.Shutdown();
        }

        public void ShowColumnPropertiesDialog()
        {
            // TODO: column visibility.
            throw new NotImplementedException();
        }

        public void ShowFilterDesignerDialog()
        {
            // TODO(filters): show filter dialog.
            throw new NotImplementedException();
        }

        private bool HaveRows()
        {
            return CoverageRows != null &&
                   CoverageRows.Count > 0;
        }

        public void ExpandAll()
        {
            foreach (var row in RowsAsFlatList)
            {
                row.IsExpanded = true;
            }
        }

        public void CollapseAll()
        {
            foreach (var row in RowsAsFlatList)
            {
                row.IsExpanded = false;
            }
        }

        private bool HaveSelectedNode()
        {
            return SelectedCoverageRow != null;
        }

        private void ExpandTree()
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

        private void CollapseTree()
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


        private void SetVisibleColumns(ColumnPreset preset)
        {
            SetVisibleColumns(ColumnPresetLists.GetPreset(preset));
        }

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

        private static void AppendRows(CoverageNodeViewModel vm, List<CoverageNodeViewModel> flatList)
        {
            flatList.Add(vm);

            foreach (var child in vm.Children)
            {
                AppendRows(child, flatList);
            }
        }


        private void CheckFileOK(object sender, CancelEventArgs e)
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

        private void SendThresholdChanged(double newValue)
        {
            _messenger.Send(new ThresholdChangedMessage(newValue));
        }

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
