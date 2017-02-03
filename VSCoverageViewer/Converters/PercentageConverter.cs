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
    [ValueConversion(typeof(double), typeof(string))]
    class PercentageConverter : IValueConverter
    {
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
                if (double.TryParse(str, out percent))
                {
                    return (percent / 100.0);
                }
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
