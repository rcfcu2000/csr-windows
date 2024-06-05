using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Services.Base;
using csr_windows.Client.ViewModels.Main;
using csr_windows.Client.ViewModels.Menu;
using csr_windows.Client.Views.Main;
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

namespace csr_windows.Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        private static MainViewModel _mainViewModel;
        private IUiService _uiService;
        #endregion

        #region Constructor
        public MainWindow()
        {
            _uiService = Ioc.Default.GetService<IUiService>();
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.OpenPromptMessageToken, (r, m) => 
            { 
                promptWindow.PromptContent = m;
                promptWindow.Visibility = Visibility.Visible; 
            });
            WeakReferenceMessenger.Default.Register<UserControl, string>(this, MessengerConstMessage.OpenMenuUserControlToken, (r, m) => { baseMenuView.Visibility = Visibility.Visible; });
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.CloseMenuUserControlToken, (r, m) => 
            { 
                baseMenuView.Visibility = Visibility.Collapsed;
            });

            //关闭退出界面
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.CloseLogoutViewToken, (r, m) =>
            {
                logoutView.Visibility = Visibility.Collapsed;
            });

            //退出
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.ExitToken, (r, m) =>
            {
                this.Close();
            });

            InitializeComponent();
            _mainViewModel = new MainViewModel();
            this.DataContext = _mainViewModel;
            baseMenuView.DataContext = new BaseMenuViewModel();


            //_uiService.OpenCustomerView();
            _uiService.OpenNoStartClientView();

        }
        #endregion


        #region Methods


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //点击空白获取焦点
            LostButton.Focus();
        }


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
            logoutView.Visibility = Visibility.Visible;

        }

        public void ToggleButton_More_LostFocus(object sender, RoutedEventArgs e)
        {
            var toggleButton = sender as ToggleButton;
            if ((bool)toggleButton.IsChecked)
                toggleButton.IsChecked = false;
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
        #endregion



    }
}
