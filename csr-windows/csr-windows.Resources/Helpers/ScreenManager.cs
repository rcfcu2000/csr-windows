using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows;
using System.Drawing;
using System.Windows.Media;

namespace csr_windows.Resources.Helpers
{
    public class ScreenManager
    {
        /// <summary>
        /// 获取DPI百分比
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static double GetDpiRatio(Window window)
        {
            var currentGraphics = Graphics.FromHwnd(new WindowInteropHelper(window).Handle);
            return currentGraphics.DpiX / 96;
        }

        public static double GetDpiRatio()
        {
            var currentGraphics = Graphics.FromHwnd(IntPtr.Zero);
            return currentGraphics.DpiX / 96;
        }

        //public static double GetDpiRatio()
        //{
        //    //return GetDpiRatio(Application.Current.MainWindow);
        //}

        public static double GetScreenHeight()
        {
            return SystemParameters.PrimaryScreenHeight * GetDpiRatio();
        }

        public static double GetScreenActualHeight()
        {
            return SystemParameters.PrimaryScreenHeight;
        }

        public static double GetScreenWidth()
        {
            return SystemParameters.PrimaryScreenWidth * GetDpiRatio();
        }

        public static double GetScreenActualWidth()
        {
            return SystemParameters.PrimaryScreenWidth;
        }
    }
}
