using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace VSCoverageViewer.Converters
{
    class MultiplyConverter : IValueConverter
    {
        public double Factor { get; set; }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double? val = (value as IConvertible)?.ToDouble(null);

            if (val == null)
            {
                throw new ArgumentException("Invalid value", nameof(value));
            }

            return val * Factor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double? val = (value as IConvertible)?.ToDouble(null);

            if (val == null)
            {
                throw new ArgumentException("Invalid value", nameof(value));
            }

            return val / Factor;
        }
    }
}
