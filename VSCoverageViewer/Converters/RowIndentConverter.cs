using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace VSCoverageViewer.Converters
{
    /// <summary>
    /// Converts a row's depth to a margin value.
    /// </summary>
    [ValueConversion(typeof(int), typeof(Thickness))]
    internal class RowIndentConverter : IValueConverter
    {
        /// <summary>
        /// The depth of a single indentation level.
        /// </summary>
        public double IndentDepth { get; set; }


        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type to convert to. Must be double.</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="culture">The culture to perform the conversion in.</param>
        /// <returns>The given value, multiplied by the factor.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var rowDepth = value as int?;

            if (rowDepth != null &&
                rowDepth >= 0 &&
                targetType == typeof(Thickness))
            {
                return new Thickness(rowDepth.Value * IndentDepth, 0, 0, 0);
            }

            return DependencyProperty.UnsetValue;
        }


        /// <summary>
        /// Unsupported.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
