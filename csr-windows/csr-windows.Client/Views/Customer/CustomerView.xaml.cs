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

namespace csr_windows.Client.Views.Customer
{
    /// <summary>
    /// CustomerView.xaml 的交互逻辑
    /// </summary>
    public partial class CustomerView : UserControl
    {
        public CustomerView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Grid grid = new Grid();
            grid.Width = 200;
            grid.Height = 200;
            grid.Background = Brushes.Red;
            grid.Margin = new Thickness(10);
            testSP.Children.Add(grid);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            testSP.Children.Clear();
        }
    }
}
