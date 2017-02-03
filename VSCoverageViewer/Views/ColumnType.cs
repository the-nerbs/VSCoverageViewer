using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VSCoverageViewer.Views
{
    /// <summary>
    /// Attached property for setting column tags in XAML.
    /// </summary>
    public static class ColumnType
    {
        public static readonly DependencyProperty ColumnProperty =
            DependencyProperty.RegisterAttached("Tag", typeof(Column), typeof(ColumnType));


        public static Column GetTag(this DataGridColumn column)
        {
            if (column == null)
                throw new ArgumentNullException(nameof(column));

            return (Column)column.GetValue(ColumnProperty);
        }

        public static void SetTag(this DataGridColumn column, Column value)
        {
            if (column == null)
                throw new ArgumentNullException(nameof(column));

            column.SetValue(ColumnProperty, value);
        }
    }
}
