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
    /// Converts a <see cref="CoverageNodeType"/> to it's associated image.
    /// </summary>
    [ValueConversion(typeof(CoverageNodeType), typeof(ImageSource))]
    internal class NodeTypeToImageConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boxed <see cref="CoverageNodeType"/> to an <see cref="ImageSource"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Unused.</param>
        /// <param name="culture">Unused.</param>
        /// <returns>The converted value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null ||
                !(value is CoverageNodeType) ||
                targetType != typeof(ImageSource))
            {
                return DependencyProperty.UnsetValue;
            }

            string resourceKey;

            switch ((CoverageNodeType)value)
            {
                case CoverageNodeType.CoverageFile:
                    resourceKey = AppResourceKeys.TotalsImg;
                    break;

                case CoverageNodeType.Module:
                    resourceKey = AppResourceKeys.ModuleImg;
                    break;

                case CoverageNodeType.Namespace:
                    resourceKey = AppResourceKeys.NamespaceImg;
                    break;

                case CoverageNodeType.Class:
                    resourceKey = AppResourceKeys.ClassImg;
                    break;

                case CoverageNodeType.Function:
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
