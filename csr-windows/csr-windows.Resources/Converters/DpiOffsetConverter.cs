using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace csr_windows.Resources.Converters
{
    public class DpiOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double offset && Application.Current.MainWindow != null)
            {
                var visual = Application.Current.MainWindow;
                var source = PresentationSource.FromVisual(visual);
                if (source?.CompositionTarget != null)
                {
                    double dpiScaleFactor = 96.0 / (source.CompositionTarget.TransformToDevice.M11 * 96.0);
                    return offset * dpiScaleFactor;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
