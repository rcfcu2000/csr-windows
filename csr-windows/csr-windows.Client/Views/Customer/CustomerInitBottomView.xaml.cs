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
            Task.Delay(50).ContinueWith(t =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    (sender as ToggleButton).IsChecked = false;
                });
            });
        }
    }
}
