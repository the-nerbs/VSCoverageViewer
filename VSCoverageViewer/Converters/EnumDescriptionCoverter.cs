using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace VSCoverageViewer.Converters
{
    [ValueConversion(typeof(object), typeof(string))]
    class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Contract.RequiresNotNull(value, nameof(value));
            Contract.Requires(value.GetType().IsEnum, "The value must be an enum.", nameof(value));
            Contract.Requires(targetType == typeof(string), "Target type must be System.String.", nameof(targetType));

            return GetDescription(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
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
