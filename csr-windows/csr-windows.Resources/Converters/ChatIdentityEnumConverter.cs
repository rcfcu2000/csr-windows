using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace csr_windows.Resources.Converters
{
    public class ChatIdentityEnumConverter : IValueConverter
    {
        public Visibility EqualsVisibility { get; set; }
        public Visibility NotEqualsVisibility { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            if (!Enum.IsDefined(value.GetType(), value))
                return false;

            var enumValue = Enum.Parse(value.GetType(), parameter.ToString());
            if (enumValue.Equals(value))
            {
                return EqualsVisibility;
            }
            else
            {
                return NotEqualsVisibility;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;

            return Enum.Parse(targetType, parameter.ToString());
        }
    }
}
