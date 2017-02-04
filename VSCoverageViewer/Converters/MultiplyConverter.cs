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
    /// A value converter used to multiply a numeric value by a given factor. Supports converting
    /// from any <see cref="IConvertible"/> supporting <see cref="IConvertible.ToDouble(IFormatProvider)"/>
    /// to either double or GridLength.
    /// </summary>
    [ValueConversion(typeof(IConvertible), typeof(double))]
    [ValueConversion(typeof(IConvertible), typeof(GridLength))]
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
        /// <param name="culture">The culture to perform the conversion in.</param>
        /// <returns>The given value, multiplied by the factor.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IConvertible)
            {
                double val = ((IConvertible)value).ToDouble(culture);
                double result = val * Factor;

                if (targetType == typeof(double))
                {
                    return result;
                }
                else if (targetType == typeof(GridLength))
                {
                    return new GridLength(result);
                }
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
            if (typeof(IConvertible).IsAssignableFrom(targetType))
            {
                if (value is double)
                {
                    double val = ((IConvertible)value).ToDouble(null);
                    return val / Factor;
                }
                else if (value is GridLength)
                {
                    var gridLength = (GridLength)value;

                    if (gridLength.GridUnitType == GridUnitType.Pixel)
                    {
                        return gridLength.Value / Factor;
                    }
                }
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
