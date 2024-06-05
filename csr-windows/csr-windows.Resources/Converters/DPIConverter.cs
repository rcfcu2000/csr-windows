using csr_windows.Resources.Enumeration;
using csr_windows.Resources.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace csr_windows.Resources.Converters
{

    /// <summary>
    /// 不同的DPI的转换器
    /// 主要是要用在X,Y坐标的转换
    /// parameter 需要传递过来枚举DPIConverterEnum用来判断是X还是Y
    /// </summary>
    public class DPIConverter : IValueConverter
    {
        const int DesignWidth = 2560;
        const int DesignHeight = 1440;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Enum.TryParse(parameter.ToString(), out DPIConverterEnum dpi))
            {
                PropertyInfo propertyInfo;
                if (dpi == DPIConverterEnum.X)
                    propertyInfo = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
                else
                    propertyInfo = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

                if (propertyInfo != null)
                {
                    var dpiValue = (int)propertyInfo.GetValue(null, null);
                    double nWidth = ScreenManager.GetScreenWidth();
                    double nHieght = ScreenManager.GetScreenHeight();
                    double dpiRatio = 96d / dpiValue;

                    if (dpi == DPIConverterEnum.X)
                        dpiRatio = ((double)(nWidth / DesignWidth)) * dpiRatio;
                    else
                        dpiRatio = ((double)(nHieght / DesignHeight)) * dpiRatio;

                    if (Int32.TryParse(value.ToString(), out int intValue))
                        return intValue * dpiRatio;
                }
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
