using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Helpers;
using csr_windows.Client.Services.Base;
using csr_windows.Client.ViewModels.Main;
using csr_windows.Client.ViewModels.Menu;
using csr_windows.Client.Views.Main;
using csr_windows.Common.Helper;
using csr_windows.Domain;
using csr_windows.Resources.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
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
        private static MainViewModel _mainViewModel = new MainViewModel();
        private IUiService _uiService;
        /// <summary>
        /// 是否更新位置中
        /// </summary>
        private bool _isUpdatePos = false;


        private Win32.RECT _lastRect = new Win32.RECT();
        /// <summary>
        /// 跟随窗口句柄
        /// </summary>
        public IntPtr FollowHandle;

        /// <summary>
        /// 是否找到句柄
        /// </summary>
        private bool _isFoundIntPrt;


        //检测边距距离
        private const int _dockMargin = 0;
        #endregion

        #region Constructor
        public MainWindow()
        {
            if (ProcessHelper.GetIsExistSameProgram())
            {
                Application.Current.Shutdown();
                return;
            }

            this.SourceInitialized += MainWindow_SourceInitialized;
            

            this.Loaded += MainWindow_Loaded;

            _uiService = Ioc.Default.GetService<IUiService>();

            //提示~
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.OpenPromptMessageToken, (r, m) =>
            {
                promptWindow.PromptContent = m;
                promptWindow.Visibility = Visibility.Visible;
            });

            //打开菜单User控件
            WeakReferenceMessenger.Default.Register<UserControl, string>(this, MessengerConstMessage.OpenMenuUserControlToken, (r, m) => { baseMenuView.Visibility = Visibility.Visible; });

            //关闭菜单User控件
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
            this.DataContext = _mainViewModel;
            baseMenuView.DataContext = new BaseMenuViewModel();


            //_uiService.OpenCustomerView();
            _uiService.OpenWelcomeView();

        }

        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            _isFoundIntPrt = FollowWindowHelper.GetQianNiuIntPrt(ref FollowHandle);
            if (!_isFoundIntPrt)
            {
                #region Init
                this.SizeToContent = SizeToContent.Manual;
                this.Width = 411 * GetDpiX();
                this.Left = (ScreenManager.GetScreenWidth() - Width) / 2;
                this.Height = 811 * GetDpiY();
                this.Top = (ScreenManager.GetScreenHeight() - Height) / 2;
                this.Visibility = Visibility.Visible;
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                Visibility = Visibility.Visible;
                #endregion
            }
        }
        #endregion

        #region Properties


        public IntPtr Handle { get; set; }



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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            FollowWindow();
        }


        public void FollowWindow()
        {
            Handle = new WindowInteropHelper(this).Handle;
            Task.Factory.StartNew(() =>
            {
                do
                {
                    _isFoundIntPrt = FollowWindowHelper.GetQianNiuIntPrt(ref FollowHandle);
                    System.Threading.Thread.Sleep(1000);
                } while (!_isFoundIntPrt);
                GlobalCache.IsFollowWindow = true;
                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        Win32.RECT rect = new Win32.RECT();
                        Win32.GetWindowRect(FollowHandle, ref rect);
                        if (rect.Bottom != 0 || rect.Top != 0 || rect.Left != 0 || rect.Right != 0)
                        {
                            this.SizeToContent = SizeToContent.Manual;
                            var hight = Math.Abs(rect.Bottom - rect.Top);
                            var windowWith = Math.Abs(rect.Right - rect.Left);
                            this.Height = hight + 15;
                            this.Width = 411 * GetDpiX();
                            //右边
                            this.Left = rect.Right + _dockMargin;
                            //左边
                            //this.Left = rect.Left - this.Width;
                            this.Top = rect.Top - 8;
                        }
                        else
                        {
                            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        }
                    }
                    catch
                    {
                        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    }
                    Visibility = Visibility.Visible;
                    //TODO：需要去判断接待中心窗体是否找到了
                    UpdateWindowPos(true);
                });
            });
        }

        /// <summary>
        /// 更新窗口位置
        /// </summary>
        private void UpdateWindowPos(bool isFirst)
    {
        if (_isUpdatePos)
        {
            return;
        }
        _isUpdatePos = true;
        this.Topmost = true;
        if (FollowHandle != null && FollowHandle != IntPtr.Zero)
        {
            var cHandle = FollowHandle;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    do
                    {
                        Win32.RECT rect = new Win32.RECT();
                        Win32.GetWindowRect(FollowHandle, ref rect);
                        if (!(_lastRect.Bottom == rect.Bottom && _lastRect.Top == rect.Top && _lastRect.Left == rect.Left && _lastRect.Right == rect.Right))
                        {
                            if (isFirst)
                            {
                                isFirst = false;
                                _lastRect = rect;
                                continue;
                            }
                            _subTaskHandler();
                        }
                        System.Threading.Thread.Sleep(10);
                    } while (_isUpdatePos);
                }
                catch (Exception)
                {
                }
                _isUpdatePos = false;
            });
        }
        else
        {
            _isUpdatePos = false;
        }
    }

    private void _subTaskHandler()
    {

        Win32.RECT rect = new Win32.RECT();
        Win32.GetWindowRect(FollowHandle, ref rect);
        if (rect.Bottom == 0 && rect.Top == 0 && rect.Left == 0 && rect.Right == 0)
            return;
        var hight = Math.Abs(rect.Bottom - rect.Top);
        //var width = Math.Abs(rect.Right - rect.Left);
        var width = 400;
        double windowWith = 0, windowHeight = 0, left = 0, top = 0;
        this.Dispatcher.Invoke(() =>
        {
            left = this.Left;
            top = this.Top;
            this.Height = hight + 15;
            windowWith = this.Width;
            windowHeight = this.Height;
        });
        Point sp = new Point(rect.Left - _lastRect.Left + left, rect.Top - _lastRect.Top + top);
        Win32.SetWindowPos(Handle, FollowHandle, (int)sp.X, (int)sp.Y, (int)windowWith, (int)windowHeight, 0x0001 | 0x0004);
        _lastRect = rect;
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

    private double GetDpiX()
    {
        const int DesignWidth = 1920;
        Matrix m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
        double scaleX = 1 / m.M11;
        double nWidth = ScreenManager.GetScreenWidth();
        var dpiX = nWidth / DesignWidth * scaleX;
        return dpiX;
    }

    private double GetDpiY()
    {
        const int DesignHeight = 1080;
        Matrix m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
        double scaleY = 1 / m.M22;
        double nHeight = ScreenManager.GetScreenHeight();
        var dpiY = nHeight / DesignHeight * scaleY;
        return dpiY;
    }
    #endregion



}
}
