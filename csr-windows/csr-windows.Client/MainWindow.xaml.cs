using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Helpers;
using csr_windows.Client.Services.Base;
using csr_windows.Client.ViewModels.Main;
using csr_windows.Client.ViewModels.Menu;
using csr_windows.Client.Views.Main;
using csr_windows.Client.Views.Public;
using csr_windows.Common.Helper;
using csr_windows.Core;
using csr_windows.Domain;
using csr_windows.Domain.Api;
using csr_windows.Domain.BaseModels.BackEnd.Base;
using csr_windows.Domain.Common;
using csr_windows.Domain.WeakReferenceMessengerModels;
using csr_windows.Domain.WebSocketModels;
using csr_windows.Resources.Helpers;
using Newtonsoft.Json;
using Sunny.UI;
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
using static csr_windows.Client.Services.WebService.TopHelp;
using static csr_windows.Client.Services.WebService.WebServiceClient;
using csr_windows.Client.Services.WebService;
using csr_windows.Domain.BaseModels;
using csr_windows.Domain.BaseModels.BackEnd;
using csr_windows.Resources.Enumeration;
using System.Web.UI.WebControls;
using csr_windows.Core.RequestService;
using System.Net.WebSockets;
using csr_windows.Client.Services.WebService.Enums;

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


        private Win32.RECT _lastRect = new Win32.RECT();


        //检测边距距离
        private const int _dockMargin = 0;

        private double scaleY;
        private double scaleX;

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

            //退出
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


        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            Matrix m = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
            scaleY = (double)1 / m.M22;
            scaleX = (double)1 / m.M11;

            GlobalCache.FollowHandle = FindWindowByProcessAndTitle("AliWorkbench", "接待中心");
            GlobalCache.IsFollowWindow = GlobalCache.FollowHandle != IntPtr.Zero;
            //GlobalCache.IsFollowWindow = FollowWindowHelper.GetQianNiuIntPrt(ref GlobalCache.FollowHandle);
            if (!GlobalCache.IsFollowWindow)
            {
                #region Init
                this.SizeToContent = SizeToContent.Manual;
                //this.Width = 411 * GetDpiX();
                this.Width = 411;
                this.Left = (ScreenManager.GetScreenWidth() - Width) / 2;
                //this.Height = 811 * GetDpiY();
                this.Height = 811 * scaleY;
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
            Dispatcher.Invoke(() =>
            {

                try
                {
                    SetThisFollowWindow();
                }
                catch
                {
                    this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
                Visibility = Visibility.Visible;
                //TODO：需要去判断接待中心窗体是否找到了
                UpdateWindowPos(true);
            });
        }

        private void SetThisFollowWindow()
        {
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
            this.Topmost = true;
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
            var hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            if (hwndSource == null)
                return;
            Win32.GetWindowRect(hwndSource.Handle, ref thisRect);
            left = this.Left;
            top = this.Top;
            this.Height = (hight + 15) * scaleY;
            windowWith = this.Width;
            windowHeight = this.Height;
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



}
}
