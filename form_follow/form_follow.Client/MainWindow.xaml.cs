using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace form_follow.Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Init
        #endregion


        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
            this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
            //this.LocationChanged += MainWindow_LocationChanged;
        }

        private void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }



        #region Fields
        /// <summary>
        /// 是否更新位置中
        /// </summary>
        private bool _isUpdatePos = false;


        private Win32.RECT _lastRect = new Win32.RECT();



        //检测边距距离
        private const int _dockMargin = 10;
        #endregion

        #region Properties


        public IntPtr Handle { get; set; }

        /// <summary>
        /// 跟随窗口句柄
        /// </summary>
        public IntPtr FollowHandle { get; set; }


        #endregion

        #region Methods


        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            //if ( Mouse.LeftButton == MouseButtonState.Released )
            //    return;
            //Console.WriteLine("我进来了！！" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            ////获取窗体的xy
            //var leftX = this.Left;
            //var rightX = this.Left + this.Width;
            ////比较 窗体x跟是否小于5px
            //Win32.RECT rect = new Win32.RECT();
            //Win32.GetWindowRect(FollowHandle, ref rect);
            //if (Math.Abs(rect.Left - leftX) < _dockMargin || Math.Abs(rect.Left - rightX) < _dockMargin)
            //{
            //    _followType = FollowType.Left;
            //    MyTextBlock.Text = "Left";
            //    _subTaskHandler();
            //}
            //else if (_followType != FollowType.Right && (Math.Abs(rect.Right - rightX) < _dockMargin || Math.Abs(rect.Right - leftX) < _dockMargin))
            //{
            //    _followType = FollowType.Right;
            //    MyTextBlock.Text = "Right";
            //    _subTaskHandler();
            //}
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Handle = new WindowInteropHelper(this).Handle;
            //TODO: 这里需要去模糊匹配包含接待中心
            FollowHandle = Win32.FindWindow("Qt5152QWindowIcon", null);
            try
            {
                Win32.RECT rect = new Win32.RECT();
                Win32.GetWindowRect(FollowHandle, ref rect);
                if (rect.Bottom != 0 || rect.Top != 0 || rect.Left != 0 || rect.Right != 0)
                {
                    var hight = Math.Abs(rect.Bottom - rect.Top);
                    var windowWith = Math.Abs(rect.Right - rect.Left);
                    this.Height = hight;
                    this.Width = 250;
                    //右边
                    this.Left = rect.Right + _dockMargin;
                    //左边
                    //this.Left = rect.Left - this.Width;
                    this.Top = rect.Top;
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


            //TODO：需要去判断接待中心窗体是否找到了
            UpdateWindowPos(true);
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
            var width = 250;
            double windowWith = 0, windowHeight = 0,left = 0,top = 0;
            this.Dispatcher.Invoke(() =>
            {
                left = this.Left;
                top = this.Top;
                this.Height = hight;
                windowWith = this.Width;
                windowHeight = this.Height;
            });
            //left
            //Point sp = new Point(rect.Right + _dockMargin, rect.Bottom - windowHeight);
            //right
            //Point sp = new Point(rect.Left - windowWith - _dockMargin, rect.Bottom - windowHeight);
            //相对位置
            Point sp = new Point(rect.Left - _lastRect.Left + left, rect.Top - _lastRect.Top + top);
            //if (_followType == FollowType.Right)
            //     sp = new Point(rect.Right + _dockMargin, rect.Bottom - windowHeight);
            //else 
            //     sp = new Point(rect.Left - windowWith - _dockMargin, rect.Bottom - windowHeight);
            Win32.SetWindowPos(Handle, FollowHandle, (int)sp.X, (int)sp.Y, (int)windowWith, (int)windowHeight, 0x0001 | 0x0004);
            _lastRect = rect;
        }

        private void StopUpdateWindowPos()
        {
            _isUpdatePos = false;
        }

        #endregion
    }
}
