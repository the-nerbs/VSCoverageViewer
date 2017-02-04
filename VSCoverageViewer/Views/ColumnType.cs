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
    internal static class ColumnType
    {
        public static readonly DependencyProperty ColumnProperty =
            DependencyProperty.RegisterAttached("Tag", typeof(Column), typeof(ColumnType));


        /// <summary>
        /// Gets the column tag from a <see cref="DataGridColumn"/>.
        /// </summary>
        /// <param name="column">The column to get the tag from.</param>
        /// <returns>The column tag.</returns>
        public static Column GetTag(this DataGridColumn column)
        {
            if (column == null)
                throw new ArgumentNullException(nameof(column));

            return (Column)column.GetValue(ColumnProperty);
        }


        /// <summary>
        /// Sets the column tag on a <see cref="DataGridColumn"/>.
        /// </summary>
        /// <param name="column">The column to set the tag on.</param>
        /// <param name="value">The column tag.</param>
        public static void SetTag(this DataGridColumn column, Column value)
        {
            if (column == null)
                throw new ArgumentNullException(nameof(column));

            column.SetValue(ColumnProperty, value);
        }
    }
}
