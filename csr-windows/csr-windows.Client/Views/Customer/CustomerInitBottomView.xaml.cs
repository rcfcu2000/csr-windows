using csr_windows.Client.ViewModels.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace csr_windows.Client.Views.Customer
{
    /// <summary>
    /// CustomerInitBottomView.xaml 的交互逻辑
    /// </summary>
    public partial class CustomerInitBottomView : UserControl
    {
        public CustomerInitBottomView()
        {
            InitializeComponent();
            this.DataContext = new CustomerInitBottomViewModel();
        }

        private void tb_LostFocus(object sender, RoutedEventArgs e)
        {
            Task.Delay(150).ContinueWith(t =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    (sender as ToggleButton).IsChecked = false;
                });
            });
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualWidth < 350)
            {
                how2replyBt.Content = null;
                want2replyBt.Content = null;
            }else
            {
                how2replyBt.Content = "我该怎么回";
                want2replyBt.Content = "我想这样回";
            }
        }
    }
}
