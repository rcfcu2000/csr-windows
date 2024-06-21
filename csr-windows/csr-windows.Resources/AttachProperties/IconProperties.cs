using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace csr_windows.Resources.AttachProperties
{
    public class IconProperties
    {
        #region 边距


        public static Thickness GetIconThickness(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(IconThicknessProperty);
        }

        public static void SetIconThickness(DependencyObject obj, Thickness value)
        {
            obj.SetValue(IconThicknessProperty, value);
        }

        // Using a DependencyProperty as the backing store for IconThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconThicknessProperty =
            DependencyProperty.RegisterAttached("IconThickness", typeof(Thickness), typeof(IconProperties), new PropertyMetadata(new Thickness(0)));



        public static HorizontalAlignment GetIconHorizontalAlignment(DependencyObject obj)
        {
            return (HorizontalAlignment)obj.GetValue(IconHorizontalAlignmentProperty);
        }

        public static void SetIconHorizontalAlignment(DependencyObject obj, HorizontalAlignment value)
        {
            obj.SetValue(IconHorizontalAlignmentProperty, value);
        }

        // Using a DependencyProperty as the backing store for IconHorizontalAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconHorizontalAlignmentProperty =
            DependencyProperty.RegisterAttached("IconHorizontalAlignment", typeof(HorizontalAlignment), typeof(IconProperties));


        #endregion

        #region 图标路径
        public static ImageSource GetIconUrl(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(IconUrlProperty);
        }
        public static void SetIconUrl(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(IconUrlProperty, value);
        }
        public static readonly DependencyProperty IconUrlProperty =
            DependencyProperty.RegisterAttached("IconUrl", typeof(ImageSource), typeof(IconProperties));

        public static ImageSource GetCheckIconUrl(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(CheckIconUrlProperty);
        }

        public static void SetCheckIconUrl(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(CheckIconUrlProperty, value);
        }

        // Using a DependencyProperty as the backing store for CheckIconUrl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CheckIconUrlProperty =
            DependencyProperty.RegisterAttached("CheckIconUrl", typeof(ImageSource), typeof(IconProperties));

        public static ImageSource GetNotActiveIconUrl(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(NotActiveIconUrlProperty);
        }

        public static void SetNotActiveIconUrl(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(NotActiveIconUrlProperty, value);
        }

        // Using a DependencyProperty as the backing store for NotActiveIconUrl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NotActiveIconUrlProperty =
            DependencyProperty.RegisterAttached("NotActiveIconUrl", typeof(ImageSource), typeof(IconProperties));
        #endregion

        #region 图标背景

        public static ImageSource GetIconBackgroundUrl(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(IconBackgroundUrlProperty);
        }

        public static void SetIconBackgroundUrl(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(IconBackgroundUrlProperty, value);
        }

        // Using a DependencyProperty as the backing store for IconBackgroundUrl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconBackgroundUrlProperty =
            DependencyProperty.RegisterAttached("IconBackgroundUrl", typeof(ImageSource), typeof(IconProperties));

        public static ImageSource GetHoverIconBackgroundUrl(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(HoverIconBackgroundUrlProperty);
        }

        public static void SetHoverIconBackgroundUrl(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(HoverIconBackgroundUrlProperty, value);
        }

        // Using a DependencyProperty as the backing store for HoverIconBackgroundUrl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HoverIconBackgroundUrlProperty =
            DependencyProperty.RegisterAttached("HoverIconBackgroundUrl", typeof(ImageSource), typeof(IconProperties));

        #endregion

        #region 图标宽高


        #region 高
        public static double GetIconHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(IconHeightProperty);
        }

        public static void SetIconHeight(DependencyObject obj, double value)
        {
            obj.SetValue(IconHeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for IconHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.RegisterAttached("IconHeight", typeof(double), typeof(IconProperties));
        #endregion

        #region 宽

        public static double GetIconWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(IconWidthProperty);
        }

        public static void SetIconWidth(DependencyObject obj, double value)
        {
            obj.SetValue(IconWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for IconWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.RegisterAttached("IconWidth", typeof(double), typeof(IconProperties));

        #endregion

        #endregion
    }
}
