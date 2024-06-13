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
using System.Windows.Shapes;

namespace csr_windows.Install
{
    /// <summary>
    /// PromptWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PromptWindow : Window
    {
        public PromptWindow(string content)
        {
            InitializeComponent();
            this.myTB.Text = content;
        }


        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            //this.myTB.
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
