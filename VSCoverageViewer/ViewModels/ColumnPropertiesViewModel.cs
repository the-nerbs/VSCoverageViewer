using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.CommandWpf;

namespace VSCoverageViewer.ViewModels
{
    /// <summary>
    /// View model for the column properties dialog.
    /// </summary>
    internal class ColumnPropertiesViewModel : ObservableObject
    {
        /// <summary>
        /// Gets the collection of available columns.
        /// </summary>
        public SortedObservableCollection<Column> AvailableColumns { get; }
            = new SortedObservableCollection<Column>();

        /// <summary>
        /// Gets the collection of visible columns.
        /// </summary>
        public ObservableCollection<Column> VisibleColumns { get; }
            = new ObservableCollection<Column>();

        /// <summary>
        /// Gets the collection of selected available columns.
        /// </summary>
        public ObservableCollection<Column> SelectedAvailableColumns { get; }
            = new ObservableCollection<Column>();

        /// <summary>
        /// Gets the collection of selected visible columns.
        /// </summary>
        public ObservableCollection<Column> SelectedVisibleColumns { get; }
            = new ObservableCollection<Column>();


        /// <summary>
        /// Gets a command for showing the selected available columns.
        /// </summary>
        public RelayCommand ShowColumnsCmd { get; }

        /// <summary>
        /// Gets a command for hiding the selected visible columns.
        /// </summary>
        public RelayCommand HideColumnsCmd { get; }

        /// <summary>
        /// Gets a command for moving the selected visible columns up.
        /// </summary>
        public RelayCommand MoveColumnsUpCmd { get; }

        /// <summary>
        /// Gets a command for moving the selected visible columns down.
        /// </summary>
        public RelayCommand MoveColumnsDownCmd { get; }


        /// <summary>
        /// Initializes a new instance of <see cref="ColumnPropertiesViewModel"/>.
        /// </summary>
        public ColumnPropertiesViewModel()
            : this(null)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="ColumnPropertiesViewModel"/>.
        /// </summary>
        /// <param name="columnSettings">The existing column settings to show.</param>
        public ColumnPropertiesViewModel(IReadOnlyDictionary<Column, bool> columnSettings)
        {
            ShowColumnsCmd = new RelayCommand(ShowColumns, CanShowColumns);
            HideColumnsCmd = new RelayCommand(HideColumns, CanHideColumns);
            MoveColumnsUpCmd = new RelayCommand(MoveColumnsUp, CanMoveColumnsUp);
            MoveColumnsDownCmd = new RelayCommand(MoveColumnsDown, CanMoveColumnsDown);

            if (columnSettings != null)
            {
                SetVisibleColumns(columnSettings);
            }
            else
            {
                // no settings given - just say they're all hidden.
                foreach (Column col in Enum.GetValues(typeof(Column)))
                {
                    AvailableColumns.Add(col);
                }
            }
        }


        /// <summary>
        /// Gets a mapping of <see cref="Column"/>s to their visibility state.
        /// </summary>
        /// <returns>A dictionary mapping each <see cref="Column"/> to a boolean indicating if it is visible.</returns>
        public IReadOnlyDictionary<Column, bool> GetVisibleColumns()
        {
            var dict = new Dictionary<Column, bool>();

            foreach (var col in VisibleColumns)
            {
                dict[col] = true;
            }

            foreach (var col in AvailableColumns)
            {
                dict[col] = false;
            }

            foreach (Column col in Enum.GetValues(typeof(Column)))
            {
                if (!dict.ContainsKey(col))
                {
                    dict[col] = false;
                }
            }

            return dict;
        }

        /// <summary>
        /// Sets the displayed column settings to the given settings.
        /// </summary>
        /// <param name="columnSettings">A mapping of <see cref="Column"/>s to their visibility state.</param>
        public void SetVisibleColumns(IReadOnlyDictionary<Column, bool> columnSettings)
        {
            Contract.RequiresNotNull(columnSettings, nameof(columnSettings));

            AvailableColumns.Clear();
            VisibleColumns.Clear();
            SelectedAvailableColumns.Clear();
            SelectedVisibleColumns.Clear();

            foreach (Column col in Enum.GetValues(typeof(Column)))
            {
                bool isVisible;
                if (!columnSettings.TryGetValue(col, out isVisible))
                {
                    isVisible = false;
                }

                if (isVisible)
                {
                    VisibleColumns.Add(col);
                }
                else
                {
                    AvailableColumns.Add(col);
                }
            }
        }


        private bool CanShowColumns()
        {
            return SelectedAvailableColumns.Count > 0;
        }

        private void ShowColumns()
        {
            // ToArray used to detach the selection collection while we change the lists.
            foreach (var col in SelectedAvailableColumns.ToArray())
            {
                AvailableColumns.Remove(col);
                VisibleColumns.Add(col);
            }

            SelectedAvailableColumns.Clear();
        }


        private bool CanHideColumns()
        {
            return SelectedVisibleColumns.Count > 0;
        }

        private void HideColumns()
        {
            // ToArray used to detach the selection collection while we change the lists.
            foreach (var col in SelectedVisibleColumns.ToArray())
            {
                VisibleColumns.Remove(col);
                AvailableColumns.Add(col);
            }

            SelectedVisibleColumns.Clear();
        }


        private bool CanMoveColumnsUp()
        {
            return SelectedVisibleColumns.Count != 0 &&
                   !SelectedVisibleColumns.Contains(VisibleColumns[0]);
        }

        private void MoveColumnsUp()
        {
            for (int i = 0; i < SelectedVisibleColumns.Count; i++)
            {
                int origIdx = VisibleColumns.IndexOf(SelectedVisibleColumns[i]);
                VisibleColumns.Move(origIdx, origIdx - 1);
            }
        }


        private bool CanMoveColumnsDown()
        {
            return SelectedVisibleColumns.Count != 0 &&
                   !SelectedVisibleColumns.Contains(VisibleColumns.Last());
        }

        private void MoveColumnsDown()
        {
            for (int i = SelectedVisibleColumns.Count - 1; i >= 0; i--)
            {
                int origIdx = VisibleColumns.IndexOf(SelectedVisibleColumns[i]);
                VisibleColumns.Move(origIdx, origIdx + 1);
            }
        }
    }
}
