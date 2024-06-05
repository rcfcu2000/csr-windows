using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace csr_windows.Resources.Extensions
{
    public static class ScrollViewerExtensions
    {
        public static readonly DependencyProperty AutoScrollToEndProperty =
            DependencyProperty.RegisterAttached(
                "AutoScrollToEnd",
                typeof(bool),
                typeof(ScrollViewerExtensions),
                new PropertyMetadata(false, OnAutoScrollToEndChanged));

        public static bool GetAutoScrollToEnd(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToEndProperty);
        }

        public static void SetAutoScrollToEnd(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToEndProperty, value);
        }

        private static void OnAutoScrollToEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scrollViewer && e.NewValue is bool autoScroll && autoScroll)
            {
                scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
                if (scrollViewer.Content is FrameworkElement content)
                {
                    content.SizeChanged += (s, ev) => ScrollToEnd(scrollViewer);
                }
            }
            else if (d is ScrollViewer sv)
            {
                sv.ScrollChanged -= ScrollViewer_ScrollChanged;
            }
        }

        private static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer && e.ExtentHeightChange > 0)
            {
                ScrollToEnd(scrollViewer);
            }
        }

        private static void ScrollToEnd(ScrollViewer scrollViewer)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.ExtentHeight);
        }
    }
}
