using csr_windows.Resources.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace csr_windows.Client.Views.Public
{
    /// <summary>
    /// PromptWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PromptWindow : UserControl
    {
        public PromptWindow()
        {
            InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((sender as UserControl).Visibility == Visibility.Visible)
            {
                StartCloseTimer();
            }
        }


        private void StartCloseTimer(Int32 milliseconds = 2000)
        {
            Task.Delay(milliseconds).ContinueWith(t =>
            {
                Dispatcher.Invoke(() =>
                {
                    this.Visibility = Visibility.Collapsed;
                });
            });
        }



        public string PromptContent
        {
            get { return (string)GetValue(PromptContentProperty); }
            set { SetValue(PromptContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PromptContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PromptContentProperty =
            DependencyProperty.Register("PromptContent", typeof(string), typeof(PromptWindow), new PropertyMetadata(""));



        public bool IsShowIcon
        {
            get { return (bool)GetValue(IsShowIconProperty); }
            set { SetValue(IsShowIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShowIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShowIconProperty =
            DependencyProperty.Register("IsShowIcon", typeof(bool), typeof(PromptWindow), new PropertyMetadata(true));


        public PromptEnum PromptEnum
        {
            get { return (PromptEnum)GetValue(PromptEnumProperty); }
            set { SetValue(PromptEnumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PromptEnum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PromptEnumProperty =
            DependencyProperty.Register("PromptEnum", typeof(PromptEnum), typeof(PromptWindow));



    }
}
