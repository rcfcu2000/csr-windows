using CommunityToolkit.Mvvm.DependencyInjection;
using csr_windows.Client.Services.Base;
using csr_windows.Client.ViewModels.Main;
using csr_windows.Client.Views.Main;
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
            InitializeComponent();
            _mainViewModel = new MainViewModel();
            this.DataContext = _mainViewModel;
            

            _uiService.OpenFirstSettingView();
            //_uiService.OpenWelcomeView();

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

        #endregion



    }
}
