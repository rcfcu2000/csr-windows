using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows;

namespace csr_windows.Resources.Controls
{
    public class ImageButton : ButtonBase
    {
        private const string NormalImageSourcePropertyName = "NormalImageSource";
        private const string MouseOverImageSourcePropertyName = "MouseOverImageSource";
        private const string MouseOverPressedImageSourcePropertyName = "MouseOverPressedImageSource";
        private const string EnableImageSourcePropertyName = "EnableImageSource";





        public static readonly DependencyProperty NormalImageSourceProperty =
            DependencyProperty.Register(NormalImageSourcePropertyName, typeof(ImageSource), typeof(ImageButton));
        public static readonly DependencyProperty MouseOverImageSourceProperty =
            DependencyProperty.Register(MouseOverImageSourcePropertyName, typeof(ImageSource), typeof(ImageButton));
        public static readonly DependencyProperty MouseOverPressedImageSourceProperty =
            DependencyProperty.Register(MouseOverPressedImageSourcePropertyName, typeof(ImageSource), typeof(ImageButton));
        public static readonly DependencyProperty EnableImageSourceProperty =
            DependencyProperty.Register(EnableImageSourcePropertyName, typeof(ImageSource), typeof(ImageButton));




        static ImageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
        }

        public ImageSource NormalImageSource
        {
            get
            {
                return (ImageSource)GetValue(NormalImageSourceProperty);
            }
            set
            {
                SetValue(NormalImageSourceProperty, value);
            }
        }

        public ImageSource MouseOverImageSource
        {
            get
            {
                return (ImageSource)GetValue(MouseOverImageSourceProperty);
            }
            set
            {
                SetValue(MouseOverImageSourceProperty, value);
            }
        }

        public ImageSource MouseOverPressedImageSource
        {
            get
            {
                return (ImageSource)GetValue(MouseOverPressedImageSourceProperty);
            }
            set
            {
                SetValue(MouseOverPressedImageSourceProperty, value);
            }
        }

        public ImageSource EnableImageSource
        {
            get
            {
                return (ImageSource)GetValue(EnableImageSourceProperty);
            }
            set
            {
                SetValue(EnableImageSourceProperty, value);
            }
        }
    }
}
