using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace VSCoverageViewer.Converters
{
    /// <summary>
    /// Converts a percentage value to a string. This converter can round-trip.
    /// </summary>
    [ValueConversion(typeof(double), typeof(string))]
    internal class PercentageConverter : IValueConverter
    {
        /// <summary>
        /// Converts from a percentage, as a double, to a string.
        /// </summary>
        /// <param name="value">The percentage value.</param>
        /// <param name="targetType">The target type. Must be <see cref="string"/>.</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="culture">The culture to perform the conversion in.</param>
        /// <returns>The percentage, as a string.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null ||
                !(value is double) ||
                targetType != typeof(string))
            {
                return DependencyProperty.UnsetValue;
            }

            double percent = (double)value * 100.0;
            return percent.ToString("0", culture) + " %";
        }


        /// <summary>
        /// Converts a percentage string back to a double.
        /// </summary>
        /// <param name="value">The percentage string to convert.</param>
        /// <param name="targetType">The target type. Must be <see cref="double"/>.</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="culture">The culture to perform the conversion in.</param>
        /// <returns>The percentage, as a double.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value as string;

            if (str != null &&
                targetType == typeof(double))
            {
                str = str.Trim();

                if (str.EndsWith("%"))
                {
                    str = str.Substring(0, str.Length - 1).TrimEnd();
                }

                double percent;
                if (double.TryParse(str, NumberStyles.Float, culture, out percent))
                {
                    return (percent / 100.0);
                }
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
