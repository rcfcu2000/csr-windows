using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Helpers;
using csr_windows.Client.Services.Base;
using csr_windows.Client.ViewModels.Main;
using csr_windows.Client.ViewModels.Menu;
using csr_windows.Common.Helper;
using csr_windows.Domain;
using csr_windows.Domain.WeakReferenceMessengerModels;
using csr_windows.Domain.WebSocketModels;
using csr_windows.Resources.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using static csr_windows.Client.Services.WebService.TopHelp;
using csr_windows.Client.Services.WebService;
using csr_windows.Core.RequestService;
using csr_windows.Client.Services.WebService.Enums;
using System.Windows.Media.Animation;

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
        private bool _isUpdatePos = true;
        private bool isAnimating = false; // 是否在动画中
        private bool _isDocked = false; //是否停靠


        private Win32.RECT _lastRect = new Win32.RECT();


        //检测边距距离
        private const int _dockMargin = 0;

        private double scaleY;
        private double scaleX;

        #endregion

        private const int ShownTop = 0; // 窗口隐藏时的高度
        private const int HiddenTop = -1500; // 窗口显示时的高度
        #region Constructor

        public MainWindow()
        {
            if (ProcessHelper.GetIsExistSameProgram())
            {
                Application.Current.Shutdown();
                return;
            }

            //this.Height = HiddenHeight;
            //this.Top = 0;
            //this.Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;

            this.LocationChanged += MainWindow_LocationChanged;

            Topmost = true;
            Topmost = false;
            //this.Height = ScreenManager.GetScreenHeight() > 1200 ? 1200 : ScreenManager.GetScreenHeight();
            this.SourceInitialized += MainWindow_SourceInitialized;
            

            this.Loaded += MainWindow_Loaded;

            _uiService = Ioc.Default.GetService<IUiService>();

            //提示~
            WeakReferenceMessenger.Default.Register<PromptMessageTokenModel, string>(this, MessengerConstMessage.OpenPromptMessageToken, (r, m) =>
            {
                Application.Current.Dispatcher.Invoke(() => 
                {
                    promptWindow.PromptContent = m.Msg;
                    promptWindow.IsShowIcon = m.IsShowIcon;
                    promptWindow.PromptEnum = m.PromptEnum;
                    promptWindow.Visibility = Visibility.Visible;
                });
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

            //退出x
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.ExitToken, (r, m) =>
            {
                _isUpdatePos = false;
                WebServiceClient.CloseAll();
                this.Close();
                Application.Current.Shutdown();
            });

            //获取热销列表
            WeakReferenceMessenger.Default.Register<List<GetGoodProductModel>, string>(this, MessengerConstMessage.GetGoodsListToken, OnGetGoodsList);

            //显示loading
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.ShowLoadingVisibilityChangeToken, (r, m) => {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    loading.Visibility = Visibility.Visible;
                });
            });

            //隐藏loading
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.HiddenLoadingVisibilityChangeToken, (r, m) => 
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    loading.Visibility = Visibility.Collapsed;
                });
            });

            InitializeComponent();
            this.DataContext = _mainViewModel;
            baseMenuView.DataContext = new BaseMenuViewModel();


            //_uiService.OpenCustomerView();
            _uiService.OpenWelcomeView();

        }

        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            // 检查窗口是否停靠在屏幕顶部
            if (isAnimating)
            {
                return;
            }
            if (this.Top <= 0 && !_isDocked)
            {
                if (Mouse.LeftButton == MouseButtonState.Released)
                {
                    Topmost = true;
                    HideWindow();
                }
            }
            else if (this.Top > ShownTop && _isDocked)
            {
                Topmost = false;
                _isDocked = false;
            }
        }

        private void ShowWindow()
        {

            Console.WriteLine("ShowWindow In");
            isAnimating = true;
            _mainViewModel.IsShowMainGrid = true;
            var animation = new DoubleAnimation(HiddenTop, ShownTop, new Duration(TimeSpan.FromSeconds(0.3)));
            animation.Completed += ((r, m) =>
            {
                this.BeginAnimation(TopProperty, null);
                isAnimating = false;
            });
            var animationOption = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.3)));
            animationOption.Completed += ((r, m) =>
            {
                this.BeginAnimation(Window.OpacityProperty, null);
                _mainViewModel.IsShowDockControl = false;
            });
            this.BeginAnimation(Window.OpacityProperty, animationOption);
            this.BeginAnimation(Window.TopProperty, animation);
        }

        private void HideWindow()
        {
            Console.WriteLine("HideWindow In");
            isAnimating = true;
            var animation = new DoubleAnimation(ShownTop, HiddenTop, new Duration(TimeSpan.FromSeconds(0.2)));
            animation.Completed += ((r, m) =>
            {
                this.BeginAnimation(TopProperty, null);
                _isDocked = true;
                _mainViewModel.IsShowMainGrid = false;
                _mainViewModel.IsShowDockControl = true;
                Top = 0;
                isAnimating = false;
            });
            this.BeginAnimation(Window.TopProperty, animation);
            var animationOption = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.2)));
            animationOption.Completed += ((r, m) =>
            {
                this.BeginAnimation(Window.OpacityProperty, null);
            });
            this.BeginAnimation(Window.OpacityProperty, animationOption);
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            //return;
            ShowWindow();
        }

        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            Matrix m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
            scaleY = (double)1 / m.M22;
            scaleX = (double)1 / m.M11;

            GlobalCache.FollowHandle = FindWindowByProcessAndTitle("AliWorkbench", "接待中心");
            GlobalCache.IsFollowWindow = GlobalCache.FollowHandle != IntPtr.Zero;
            //GlobalCache.IsFollowWindow = FollowWindowHelper.GetQianNiuIntPrt(ref GlobalCache.FollowHandle);

            //if (!GlobalCache.IsFollowWindow)
            //{
            //    #region Init
            //    this.SizeToContent = SizeToContent.Manual;
            //    //this.Width = 411 * GetDpiX();
            //    this.Width = 411;
            //    this.Left = (ScreenManager.GetScreenWidth() - Width) / 2;
            //    //this.Height = 811 * GetDpiY();
            //    this.Height = 811 * scaleY;
            //    this.Top = (ScreenManager.GetScreenHeight() - Height) / 2;
            //    this.Visibility = Visibility.Visible;
            //    this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //    Visibility = Visibility.Visible;
            //    #endregion
            //}



        }
        #endregion

        #region Properties


        public IntPtr Handle { get; set; }



        #endregion

        #region Methods


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isAnimating)
            {
                // 如果正在动画中，停止动画并手动设置位置
                this.BeginAnimation(Window.LeftProperty, null);
                this.Top = _isDocked ? HiddenTop : ShownTop;
                isAnimating = false;
            }
            this.DragMove();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //点击空白获取焦点
            LostButton.Focus();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //this.Visibility = Visibility.Hidden;
            FollowWindow();
        }


        public void FollowWindow()
        {
            Application.Current.Dispatcher.Invoke(() => 
            {
                Handle = new WindowInteropHelper(this).Handle;
            });
            Task.Factory.StartNew(() =>
            {
                string title;
                while (true)
                {
                    var _tempIntprt = GlobalCache.FollowHandle;
                    GlobalCache.FollowHandle = FindWindowByProcessAndTitle("AliWorkbench", "接待中心");
                    GlobalCache.IsFollowWindow = GlobalCache.FollowHandle != IntPtr.Zero;

                    if (GlobalCache.FollowHandle == IntPtr.Zero)
                    {
                        Application.Current?.Dispatcher.Invoke(() =>
                        {
                            if ((this.DataContext as MainViewModel).IsInIM)
                            {
                                _uiService.OpenNoStartClientView();
                                (this.DataContext as MainViewModel).IsInIM = false;
                                WebServiceClient.CloseAll();
                            }
                        });
                        continue;
                    }

                    if (GlobalCache.FollowHandle != _tempIntprt)
                        SetThisFollowWindow();
                    //GlobalCache.IsFollowWindow = FollowWindowHelper.GetQianNiuIntPrt(ref GlobalCache.FollowHandle);

                    title = TopHelp.GetQNChatTitle();
                    if (title == TopHelp.DefaultSocketTitle)
                    {
                        continue;
                    }
                    if (title != null && title != GlobalCache.CustomerServiceNickName)
                    {
                       
                        var list = title.Split(':');
                        GlobalCache.StoreName = list[0];
                        if (list.Count() >= 2)
                        {
                            GlobalCache.UserName = list[1];
                        }
                        WebServiceClient.SendJSFunc(JSFuncType.GetCurrentConv);

                        #region 处理登录
                        if (!GlobalCache.StoreSSOLoginModel.ContainsKey(GlobalCache.StoreName))
                        {
                            LoginServer.Instance.Login();
                        }

                        #endregion

                    }
                    System.Threading.Thread.Sleep(2000);
                };
            });
            //Dispatcher.Invoke(() =>
            //{

            //    try
            //    {
            //        SetThisFollowWindow();
            //    }
            //    catch
            //    {
            //        this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            //    }
            //    Visibility = Visibility.Visible;
            //    //TODO：需要去判断接待中心窗体是否找到了
            //    UpdateWindowPos(true);
            //});
        }

        private void SetThisFollowWindow()
        {
            return;
            Application.Current.Dispatcher.Invoke(() =>
            {

                Win32.RECT rect = new Win32.RECT();
                Win32.GetWindowRect(GlobalCache.FollowHandle, ref rect);
                if (rect.Bottom != 0 || rect.Top != 0 || rect.Left != 0 || rect.Right != 0)
                {

                    this.SizeToContent = SizeToContent.Manual;
                    var hight = Math.Abs(rect.Bottom - rect.Top);
                    var windowWith = Math.Abs(rect.Right - rect.Left);
                    this.Height = (hight + 15) * scaleY;
                    this.Width = 411;
                    //右边
                    this.Left = (rect.Right + _dockMargin) * scaleX;
                    //左边
                    //this.Left = rect.Left - this.Width;
                    this.Top = (rect.Top - 8) * scaleY;
                }
                else
                {
                    this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
            });
        }

        /// <summary>
        /// 更新窗口位置
        /// </summary>
        private void UpdateWindowPos(bool isFirst)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    do
                    {
                        if (GlobalCache.FollowHandle == null || GlobalCache.FollowHandle == IntPtr.Zero)
                        {
                            System.Threading.Thread.Sleep(100);
                            continue;
                        }
                        Win32.RECT rect = new Win32.RECT();
                        Win32.GetWindowRect(GlobalCache.FollowHandle, ref rect);
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
                catch (Exception ex)
                {
                }
            });
        }

    Win32.RECT rect = new Win32.RECT();
    Win32.RECT thisRect = new Win32.RECT();
    private void _subTaskHandler()
    {
        Win32.GetWindowRect(GlobalCache.FollowHandle, ref rect);
        if (rect.Bottom == 0 && rect.Top == 0 && rect.Left == 0 && rect.Right == 0)
            return;
        var hight = Math.Abs(rect.Bottom - rect.Top);
        double windowWith = 0, windowHeight = 0, left = 0, top = 0;

        this.Dispatcher.Invoke(() =>
        {
            this.Topmost = true;
            var hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            if (hwndSource == null)
                return;
            Win32.GetWindowRect(hwndSource.Handle, ref thisRect);
            left = this.Left;
            top = this.Top;
            this.Height = (hight + 15) * scaleY;
            windowWith = this.Width;
            windowHeight = this.Height;
            this.Topmost = false;
        });
        Point sp = new Point(rect.Right - _lastRect.Right + thisRect.Left, rect.Top - _lastRect.Top + thisRect.Top);
        Win32.SetWindowPos(Handle, GlobalCache.FollowHandle, (int)sp.X, (int)sp.Y, (int)windowWith, (int)windowHeight, 0x0001 | 0x0004);
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
        Task.Delay(150).ContinueWith(t =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                (sender as ToggleButton).IsChecked = false;
            });
        });
    }

    private void OnGetGoodsList(object recipient, List<GetGoodProductModel> message)
    {
            //Task.Factory.StartNew(async () => 
            //{
            //    GlobalCache.HotSellingProducts.Clear();
            //    foreach (var item in message)
            //    {
            //        //根据信息去请求接口
            //        string msg = await ApiClient.Instance.GetAsync(string.Format($"{BackEndApiList.GetMerchantByTid}/{item.ItemId}"));
            //        BaseGetMerchantByTidModel model = JsonConvert.DeserializeObject<BaseGetMerchantByTidModel>(msg);
            //        if (model.Data == null)
            //        {
            //            continue;
            //        }
            //        foreach (var value in model.Data)
            //        {
            //            MyProduct myProduct = new MyProduct()
            //            {
            //                MerchantId = value.MerchantId,
            //                ProductID = item.ItemId,
            //                ProductImage = string.IsNullOrEmpty(value.PictureLink) ? item.Pic : value.PictureLink,
            //                ProductInfo = value.Info,
            //                ProductName = value.Alias,
            //                ProductUrl = item.ActionUrl
            //            };
            //            GlobalCache.HotSellingProducts.Add(myProduct);
            //        }
            //    }
            //});
        
     }

    private double GetDpiX()
    {
        const int DesignWidth = 2560;
        Matrix m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
        double scaleX = 1 / m.M11;
        double nWidth = ScreenManager.GetScreenWidth();
        var dpiX = nWidth / DesignWidth * scaleX;
        return dpiX;
    }

    private double GetDpiY()
    {
        const int DesignHeight = 1440;
        Matrix m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
        double scaleY = 1 / m.M22;
        double nHeight = ScreenManager.GetScreenHeight();
        var dpiY = nHeight / DesignHeight * scaleY;
        return dpiY;
    }


        #endregion

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            if (isAnimating)
            {
                return;
            }
            if (_isDocked && !_mainViewModel.IsShowDockControl)
            {
                HideWindow();
            }
        }
    }
}
