﻿using csr_windows.Resources.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace csr_windows.Resources.Styles.WindowStyles
{
    public partial class WindowsStyle
    {
        /// <summary>
        /// 窗体变为最小化状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">     </param>
        public void Button_Min_Click(object sender, RoutedEventArgs e)
        {
            if (sender is DependencyObject win)
            {
                Window.GetWindow(win).WindowState = WindowState.Minimized;
            }
        }


        public void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            //if (XDD.Control.XlyMessageBox.ShowQuestion(XLY.DF.FoundationService.Localization.Language.Get("LANGKEY_QueDingTuiChu_03937"), XLY.DF.FoundationService.Localization.Language.Get("LANGKEY_Shi_03938"), XLY.DF.FoundationService.Localization.Language.Get("LANGKEY_Fou_03939")))
            Window.GetWindow(sender as UIElement).Close();
        }

        public void ToggleButton_More_LostFocus(object sender, RoutedEventArgs e)
        {
            var toggleButton = sender as ToggleButton;
            if ((bool)toggleButton.IsChecked)
                toggleButton.IsChecked = false;
        }
    }
}