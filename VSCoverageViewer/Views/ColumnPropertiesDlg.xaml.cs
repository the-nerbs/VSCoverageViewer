using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VSCoverageViewer.ViewModels;

namespace VSCoverageViewer.Views
{
    /// <summary>
    /// Interaction logic for ColumnPropertiesDlg.xaml
    /// </summary>
    /// <devdoc>
    /// Unfortunately, WPF's ListBox doesn't support binding the selected items (though I can't
    /// figure out why, even when looking at the reference source...).  Because of this, the
    /// binding is manually emulated here for the selections of both the list boxes on the view.
    /// </devdoc>
    public partial class ColumnPropertiesDlg : Window
    {
        private bool _ignoreSelectionChanges = false;


        /// <summary>
        /// Gets the view model.
        /// </summary>
        internal ColumnPropertiesViewModel ViewModel
        {
            get { return (DataContext as ColumnPropertiesViewModel); }
        }


        /// <summary>
        /// Initializes a new instance of <see cref="ColumnPropertiesDlg"/>.
        /// </summary>
        public ColumnPropertiesDlg()
        {
            InitializeComponent();

            SourceInitialized += HandleSourceInitialized;
        }


        /// <summary>
        /// Performs source initialization.
        /// </summary>
        /// <param name="sender">unused.</param>
        /// <param name="e">unused.</param>
        private void HandleSourceInitialized(object sender, EventArgs e)
        {
            // probably doesn't matter, but unsubscribe just to be safe.
            SourceInitialized -= HandleSourceInitialized;

            NativeMethods.ClearWindowStyles(this, WindowStyles.SysMenu);
        }

        /// <summary>
        /// Handles the accept button being clicked.
        /// </summary>
        /// <param name="sender">unused.</param>
        /// <param name="e">unused.</param>
        private void HandleAcceptClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Handles the selection of available columns changing.
        /// </summary>
        /// <param name="sender">unused.</param>
        /// <param name="e">The event details.</param>
        private void HandleSelectedAvailableColumnsChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_ignoreSelectionChanges)
            {
                var vm = ViewModel;
                if (vm != null)
                {
                    using (new DisallowReentrancy(this))
                    {
                        UpdateSelection(vm.SelectedAvailableColumns, e); 
                    }
                }
            }
        }

        /// <summary>
        /// Handles the VM's selection of available columns changing.
        /// </summary>
        /// <param name="sender">unused.</param>
        /// <param name="e">unused.</param>
        private void HandleVMSelectedAvailableColumnsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!_ignoreSelectionChanges)
            {
                using (new DisallowReentrancy(this))
                {
                    _availableColumns.SelectedItems.Clear();

                    foreach (var col in ViewModel.SelectedAvailableColumns)
                    {
                        _availableColumns.SelectedItems.Add(col);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the selection of visible columns changing.
        /// </summary>
        /// <param name="sender">unused.</param>
        /// <param name="e">The event details.</param>
        private void HandleSelectedVisibleColumnsChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_ignoreSelectionChanges)
            {
                var vm = ViewModel;
                if (vm != null)
                {
                    using (new DisallowReentrancy(this))
                    {
                        UpdateSelection(vm.SelectedVisibleColumns, e);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the VM's selection of visible columns changing.
        /// </summary>
        /// <param name="sender">unused.</param>
        /// <param name="e">unused.</param>
        private void HandleVMSelectedVisibleColumnsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!_ignoreSelectionChanges)
            {
                using (new DisallowReentrancy(this))
                {
                    _visibleColumns.SelectedItems.Clear();

                    foreach (var col in ViewModel.SelectedVisibleColumns)
                    {
                        _visibleColumns.SelectedItems.Add(col);
                    }
                }
            }
        }


        /// <summary>
        /// Updates a collection of columns with a selection change event details.
        /// </summary>
        /// <param name="collection">The collection to update.</param>
        /// <param name="changeDetails">The change details.</param>
        private void UpdateSelection(ICollection<Column> collection, SelectionChangedEventArgs changeDetails)
        {
            foreach (Column col in changeDetails.AddedItems)
            {
                collection.Add(col);
            }

            foreach (Column col in changeDetails.RemovedItems)
            {
                collection.Remove(col);
            }
        }


        /// <summary>
        /// Handles a dependency property changing.
        /// </summary>
        /// <param name="e">The event details.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == DataContextProperty)
            {
                if (e.OldValue != null)
                {
                    var vm = e.OldValue as ColumnPropertiesViewModel;

                    if (vm != null)
                    {
                        vm.SelectedAvailableColumns.CollectionChanged -= HandleVMSelectedAvailableColumnsChanged;
                        vm.SelectedVisibleColumns.CollectionChanged -= HandleVMSelectedVisibleColumnsChanged;
                    }
                }

                if (e.NewValue != null)
                {
                    var vm = e.NewValue as ColumnPropertiesViewModel;

                    if (vm != null)
                    {
                        vm.SelectedAvailableColumns.CollectionChanged += HandleVMSelectedAvailableColumnsChanged;
                        vm.SelectedVisibleColumns.CollectionChanged += HandleVMSelectedVisibleColumnsChanged;
                    }
                }
            }
        }


        /// <summary>
        /// Helper class for disabling/enabling re-entrancy
        /// </summary>
        private class DisallowReentrancy : IDisposable
        {
            private readonly ColumnPropertiesDlg _window;


            public DisallowReentrancy(ColumnPropertiesDlg window)
            {
                Debug.Assert(window != null);
                _window = window;
                _window._ignoreSelectionChanges = true;
            }


            public void Dispose()
            {
                _window._ignoreSelectionChanges = false;
            }
        }
    }
}
