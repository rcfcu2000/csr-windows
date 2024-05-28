using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace csr_windows.Resources.AttachProperties
{
    public class MouseDependencyProperty
    {
        #region 默认背景色
        public static Brush GetBackColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(BackColorProperty);
        }
        public static void SetBackColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(BackColorProperty, value);
        }
        /// <summary>
        /// 默认背景色
        /// </summary>
        public static readonly DependencyProperty BackColorProperty =
            DependencyProperty.RegisterAttached("BackColor", typeof(Brush), typeof(MouseDependencyProperty), new PropertyMetadata(default));
        #endregion

        #region 鼠标按下去背景色
        public static Brush GetPressedColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(PressedColorProperty);
        }

        public static void SetPressedColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(PressedColorProperty, value);
        }

        /// <summary>
        /// 按下去的背景色
        /// </summary>
        public static readonly DependencyProperty PressedColorProperty =
            DependencyProperty.RegisterAttached("PressedColor", typeof(Brush), typeof(MouseDependencyProperty), new PropertyMetadata(default));
        #endregion

        #region 鼠标悬浮背景色
        public static Brush GetOverBackColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(OverBackColorProperty);
        }

        public static void SetOverBackColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(OverBackColorProperty, value);
        }

        /// <summary>
        /// 鼠标悬浮背景色
        /// </summary>
        public static readonly DependencyProperty OverBackColorProperty  =
            DependencyProperty.RegisterAttached("OverBackColor", typeof(Brush), typeof(MouseDependencyProperty), new PropertyMetadata(default));
        #endregion

        #region 鼠标悬浮前景色
        public static Brush GetOverForeground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(OverForegroundProperty);
        }

        public static void SetOverForeground(DependencyObject obj, Brush value)
        {
            obj.SetValue(OverForegroundProperty, value);
        }

        /// <summary>
        /// 鼠标悬浮前景色
        /// </summary>
        public static readonly DependencyProperty OverForegroundProperty =
            DependencyProperty.RegisterAttached("OverForeground", typeof(Brush), typeof(MouseDependencyProperty), new PropertyMetadata(default));
        #endregion

        #region 正常颜色

        public static Brush GetNormalColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(NormalColorProperty);
        }

        public static void SetNormalColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(NormalColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for NormalColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NormalColorProperty =
            DependencyProperty.RegisterAttached("NormalColor", typeof(Brush), typeof(MouseDependencyProperty), new PropertyMetadata(default));



        #endregion

        #region 禁用颜色


        public static Brush GetEnableColor(DependencyObject obj)
        {
            return (Brush)obj.GetValue(EnableColorProperty);
        }

        public static void SetEnableColor(DependencyObject obj, Brush value)
        {
            obj.SetValue(EnableColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for EnableColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableColorProperty =
            DependencyProperty.RegisterAttached("EnableColor", typeof(Brush), typeof(MouseDependencyProperty), new PropertyMetadata(default));


        #endregion
    }
}
