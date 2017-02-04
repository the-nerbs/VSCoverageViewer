using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace VSCoverageViewer.Converters
{
    /// <summary>
    /// Value converter used to display a string (specified by <see cref="DescriptionAttribute"/>)
    /// for enum values.
    /// </summary>
    [ValueConversion(typeof(object), typeof(string))]
    internal class EnumDescriptionConverter : IValueConverter
    {
        /// <summary>
        /// Converts from an enum value to the description.
        /// </summary>
        /// <param name="value">The boxed enum value.</param>
        /// <param name="targetType">The target type. Must be <see cref="string"/></param>
        /// <param name="parameter">Not used by this converter.</param>
        /// <param name="culture">Unused.</param>
        /// <returns>
        /// The text specified in the enum's <see cref="DescriptionAttribute"/>. If no
        /// description is provided, the result of <c>value.ToString</c> is returned.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null ||
                !value.GetType().IsEnum ||
                targetType != typeof(string))
            {
                return DependencyProperty.UnsetValue;
            }

            //TODO(localization ?): Add a LocalizableDescriptionAttribute for specifying descriptions as a resource. Same TODO for all converters.

            return GetDescription(value);
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }



        private static string GetDescription(object value)
        {
            DescriptionAttribute attr = value
                .GetType()
                .GetMember(value.ToString())
                .SingleOrDefault()
                ?.GetCustomAttributes(typeof(DescriptionAttribute), false)
                ?.FirstOrDefault() as DescriptionAttribute;

            return attr?.Description ?? value.ToString();
        }
    }
}
