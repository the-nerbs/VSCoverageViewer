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
    /// A value converter used to multiply a numeric value by a given factor.
    /// </summary>
    [ValueConversion(typeof(IConvertible), typeof(double))]
    internal class MultiplyConverter : IValueConverter
    {
        /// <summary>
        /// The factor to multiply the converted value by.
        /// </summary>
        public double Factor { get; set; }


        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type to convert to. Must be double.</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="culture">Unused.</param>
        /// <returns>The given value, multiplied by the factor.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IConvertible &&
                targetType == typeof(double))
            {
                double val = ((IConvertible)value).ToDouble(null);
                return val * Factor;
            }

            return DependencyProperty.UnsetValue;
        }


        /// <summary>
        /// Converts a value back.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="culture">Unused.</param>
        /// <returns>The given value, multiplied by the factor.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IConvertible &&
                typeof(IConvertible).IsAssignableFrom(targetType))
            {
                double val = ((IConvertible)value).ToDouble(null);
                return val / Factor;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
