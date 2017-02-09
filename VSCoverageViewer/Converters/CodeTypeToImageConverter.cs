using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using VSCoverageViewer.Models;

namespace VSCoverageViewer.Converters
{
    /// <summary>
    /// Converts a <see cref="CodeElementType"/> to it's associated image.
    /// </summary>
    [ValueConversion(typeof(CodeElementType), typeof(ImageSource))]
    internal class CodeTypeToImageConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boxed <see cref="CodeElementType"/> to an <see cref="ImageSource"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="culture">Unused.</param>
        /// <returns>The converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null ||
                !(value is CodeElementType) ||
                targetType != typeof(ImageSource))
            {
                return DependencyProperty.UnsetValue;
            }

            string resourceKey;

            switch ((CodeElementType)value)
            {
                case CodeElementType.CoverageFile:
                    resourceKey = AppResourceKeys.TotalsImg;
                    break;

                case CodeElementType.Module:
                    resourceKey = AppResourceKeys.ModuleImg;
                    break;

                case CodeElementType.Namespace:
                    resourceKey = AppResourceKeys.NamespaceImg;
                    break;

                case CodeElementType.Class:
                    resourceKey = AppResourceKeys.ClassImg;
                    break;

                case CodeElementType.Struct:
                    resourceKey = AppResourceKeys.StructImg;
                    break;

                case CodeElementType.Property:
                    resourceKey = AppResourceKeys.PropertyImg;
                    break;

                case CodeElementType.Function:
                    resourceKey = AppResourceKeys.FunctionImg;
                    break;

                default:
                    resourceKey = null;
                    break;
            }

            return Application.Current.TryFindResource(resourceKey);
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
