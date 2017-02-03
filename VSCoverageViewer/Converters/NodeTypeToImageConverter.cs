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
    [ValueConversion(typeof(CoverageNodeType), typeof(ImageSource))]
    class NodeTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Contract.RequiresNotNull(value, nameof(value));

            Contract.Requires(value is CoverageNodeType,
                              "Converted value must be a " + nameof(CoverageNodeType),
                              nameof(value));

            Contract.Requires(typeof(ImageSource).IsAssignableFrom(targetType),
                              "Target type must derive from ImageSource",
                              nameof(targetType));

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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
