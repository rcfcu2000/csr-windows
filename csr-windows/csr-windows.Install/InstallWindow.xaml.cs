using csr_windows.Install.Common;
using csr_windows.Install.Properties;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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
    /// InstallWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InstallWindow : Window
    {
        //TODO 后面要把更新加上去
        const string RunningProgramName = "csr-windows.Client";
        const string InstallFolderName = "csr_windows";
        const string UninstallExeName = "csr-windows.Uninstall.exe";
        const string CsrWindowsStartExeName = "csr-windows.Client.exe";
        const string AppName = "会回";
        const string MenuFolder = @"\会回\";
        const string CsrWindowsStartShortcutName = "会回.lnk";
        const string UninstallShortcutName = "卸载会回.lnk";

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        private extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int msg, uint wParam, uint lParam);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        /// <summary>
        /// 判断窗口是否可见
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        /// <summary>
        /// 是否需要创建快捷方式
        /// </summary>
        private bool _isCreatShortcut { get; set; }

        public InstallWindow()
        {
            InitializeComponent();

            //存在相同的程序，则退出
            if (Common.Common.GetIsExistSameProgram())
            {
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// 窗口初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string localPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            InstallCatalogTb.Text = localPath + @"\" + InstallFolderName;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 安装
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            InstallButton.IsEnabled = false;
            string path = InstallCatalogTb.Text.Trim();
            if (string.IsNullOrEmpty(path) || !Common.Common.CheckPathIsReasonable(path))
            {
                System.Windows.MessageBox.Show("安装路径中包含有非法字符，请重新输入或选择。", "提示", MessageBoxButton.OK);
                InstallButton.IsEnabled = true;
                return;
            }

            if (GetIsExistProgram())
            {
                //检测小助手是否在运行中，没有运行中就可以继续安装，假如是在运行中就二次确认弹窗
                if (System.Diagnostics.Process.GetProcessesByName(RunningProgramName).ToList().Count > 0)
                {
                    PromptWindow promptWindow = new PromptWindow("会回正在运行， 继续安装会自动关闭当前程序，确实继续安装吗？");
                    var result = promptWindow.ShowDialog();
                    if ((bool)result)
                    {
                        KillProcess(RunningProgramName);
                    }
                    else
                    {
                        this.Close();
                        return;
                    }
                }
            }

            InstallProgram(path);
        }



        /// <summary>
        /// 安装程序
        /// </summary>
        /// <param name="path"></param>
        private void InstallProgram(string path)
        {
            InstallBeforeGrid.Visibility = Visibility.Collapsed;
            InstallingGrid.Visibility = Visibility.Visible;
            CloseButton.Visibility = Visibility.Collapsed;

            _isCreatShortcut = ShortcutCheckBox.IsChecked ?? false;
            try
            {
                //创建用户指定的安装目录文件夹
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //解压会回
                DecompressMainProgram(path).ContinueWith(t =>
                {
                    //创建卸载程序
                    CreateUninstallProgram(path);

                    //创建快捷方式
                    CreateShortcut(path);

                    #region 去执行抓包的exe

                    Process myProcess = new Process();
                    myProcess.StartInfo.UseShellExecute = true;
                    myProcess.StartInfo.FileName = $@"{path}{System.IO.Path.DirectorySeparatorChar}工具3.0.exe";
                    myProcess.StartInfo.CreateNoWindow = true;
                    myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    myProcess.Start();


                    IntPtr id = myProcess.MainWindowHandle;
                    Console.WriteLine($"MainWindow:{id}");
                    MoveWindow(myProcess.MainWindowHandle, 0, 0, 1, 1, true);
                    
                    Application.Current.Dispatcher.Invoke(() => 
                    {
                        this.Topmost = true;
                        Width = 500;
                        Height = 500;
                    });
                    while (true)
                    {
                        IntPtr OtherExeWnd = new IntPtr(0);
                        OtherExeWnd = FindWindow("WTWindow", "");
                        Console.WriteLine($"OtherExeWnd:{OtherExeWnd}");

                        if (OtherExeWnd == IntPtr.Zero || !IsWindowVisible(OtherExeWnd))
                        {
                            Task.Delay(2).Wait();
                            continue;
                        }

                        MoveWindow(OtherExeWnd, 0, 0, 1, 1, true);
                        //判断这个窗体是否有效
                        if (OtherExeWnd != IntPtr.Zero)
                        {
                            Console.WriteLine("找到窗口");
                            //ShowWindow(OtherExeWnd, 0);//0表示隐藏窗口
                            while (true)
                            {
                                IntPtr ExeWnd = FindWindow("WTWindow", "Sunny抓包工具3.0 【2024-06-04】      SDK版本【2024-06-03】");
                                Console.WriteLine($"ExeWnd:{ExeWnd}");
                                if (ExeWnd == IntPtr.Zero || !IsWindowVisible(ExeWnd))
                                {
                                    Task.Delay(10).Wait();
                                    continue;
                                }
                                MoveWindow(ExeWnd, 0, 0, 1, 1, true);
                                SendMessage(ExeWnd, 16, 0, 0);//关闭窗口，通过发送消息的方式 
                                break;
                            }
                            break;
                        }
                    }

                    #endregion

                    myProcess.Kill();
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        InstallingGrid.Visibility = Visibility.Collapsed;
                        InstallAfterSuccessedGrid.Visibility = Visibility.Visible;
                        CloseButton.Visibility = Visibility.Visible;
                    }));
                });
            }
            catch (Exception ex)
            {
                //安装失败
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    System.Windows.MessageBox.Show(ex.ToString(), "提示", MessageBoxButton.OK);
                    InstallingGrid.Visibility = Visibility.Collapsed;
                    InstallAfterFailedGrid.Visibility = Visibility.Visible;
                    CloseButton.Visibility = Visibility.Visible;
                }));
            }
        }

        /// <summary>
        /// 解压会回程序
        /// </summary>
        /// <returns></returns>
        private Task DecompressMainProgram(string path)
        {
            return Task.Factory.StartNew(() =>
            {
                ZipHelper.Decompress(Resource.csr_windows_Install, path, (accumSize, maxSize) =>
                {
                    //解压会回的进度值是96
                    double value = 1.0 * accumSize / maxSize * 96;
                    ProgressEvent(value);
                });
            });
        }

        /// <summary>
        /// 创建卸载程序
        /// </summary>
        /// <param name="path"></param>
        private void CreateUninstallProgram(string path)
        {
            string uninstallPath = path + @"\" + UninstallExeName;
            FileStream uninstallFs = File.Open(uninstallPath, FileMode.Create);
            uninstallFs.Write(Resource.csr_windows_Uninstall, 0, Resource.csr_windows_Uninstall.Length);
            uninstallFs.Close();
            ProgressEvent(97);
        }

        /// <summary>
        /// 创建快捷方式
        /// </summary>
        /// <param name="path"></param>
        private void CreateShortcut(string path)
        {
            //会回的exe地址
            string exePath = path + @"\" + CsrWindowsStartExeName;
            //卸载程序的exe地址
            string uninstallExePath = path + @"\" + UninstallExeName;
            //图标的地址
            string icoPath = path + @"\应用图标.ico";

            try
            {
                #region 开始菜单
                //添加开始菜单快捷方式
                RegistryKey HKEY_CURRENT_USER = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders");
                //获取开始菜单程序文件夹路径
                string programsPath = HKEY_CURRENT_USER.GetValue("Programs").ToString();
                //开始菜单文件夹路径
                string menuFolderPath = programsPath + MenuFolder;
                //开始菜单-会回程序路径
                string shortcutPath = menuFolderPath + CsrWindowsStartShortcutName;
                //开始菜单-卸载程序路径
                string uninstallShortcutPath = menuFolderPath + UninstallShortcutName;

                //在程序文件夹中创建快捷方式的文件夹
                Directory.CreateDirectory(menuFolderPath);
                //创建开始菜单的会回快捷方式
                Common.Common.CreateShortcut(exePath, shortcutPath);
                //创建开始菜单的卸载程序快捷方式
                Common.Common.CreateShortcut(uninstallExePath, uninstallShortcutPath);

                ProgressEvent(98);
                #endregion

                #region 创建桌面快捷方式
                if (_isCreatShortcut)
                {
                    //创建桌面快捷方式
                    string desktopShortcutPath = HKEY_CURRENT_USER.GetValue("Desktop").ToString() + @"\" + CsrWindowsStartShortcutName;
                    Common.Common.CreateShortcut(exePath, desktopShortcutPath);
                }
                ProgressEvent(99);
                #endregion

                #region 程序与功能
                //常见控制面板“程序与功能”
                //可以往root里面写，root需要管理员权限，如果使用了管理员权限，主程序也会以管理员打开，如需常规打开，需要在打开进程的时候做降权处理
                RegistryKey CUKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
                var currentVersion = CUKey.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");
                string keyName = @"Software\Microsoft\Windows\CurrentVersion\Uninstall\" + AppName;
                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("DisplayIcon", icoPath);//显示的图标的地址
                dic.Add("DisplayName", AppName);//名称
                                                //dic.Add("Publisher", InstallEntity.Publisher);//发布者
                dic.Add("UninstallString", uninstallExePath);//卸载的exe路径
                                                             //dic.Add("DisplayVersion", InstallEntity.VersionNumber);//版本
                RegistryKey CurrentKey = CUKey.OpenSubKey(keyName, true);
                if (CurrentKey == null)
                {
                    //说明这个路径不存在，需要创建
                    CUKey.CreateSubKey(keyName);
                    CurrentKey = CUKey.OpenSubKey(keyName, true);
                }
                foreach (var item in dic)
                {
                    CurrentKey.SetValue(item.Key, item.Value);
                }
                CurrentKey.Close();

                ProgressEvent(100);
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 检查是否已存在会回程序
        /// </summary>
        private bool GetIsExistProgram()
        {
            RegistryKey CUKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
            string keyName = @"Software\Microsoft\Windows\CurrentVersion\Uninstall\" + AppName;
            RegistryKey CurrentKey = CUKey.OpenSubKey(keyName);
            if (CurrentKey != null)
            {
                string uninstallPath = CurrentKey.GetValue("UninstallString").ToString();
                if (!string.IsNullOrEmpty(uninstallPath) && File.Exists(uninstallPath) && File.Exists(System.IO.Path.GetDirectoryName(uninstallPath) + @"\" + CsrWindowsStartExeName))
                {
                    return true;
                }
            }
            return false;
        }

        private void ProgressEvent(double value)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                InstallProgressBar.Value = (int)value;
            }));
        }

        /// <summary>
        /// 更改目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeCatalogBt_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog cofd = new CommonOpenFileDialog();
            cofd.IsFolderPicker = true;
            cofd.Multiselect = false;
            cofd.Title = "选择安装目录";
            cofd.EnsurePathExists = false;
            cofd.InitialDirectory = InstallCatalogTb.Text.Trim().Substring(0, InstallCatalogTb.Text.Trim().LastIndexOf('\\'));
            //FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (cofd.ShowDialog(this) == CommonFileDialogResult.Ok)
            {
                string selectPath = cofd.FileName;
                string fileName = selectPath.Split('\\')[selectPath.Split('\\').Count() - 1];
                if (!fileName.Equals(InstallFolderName))
                {
                    selectPath = selectPath + @"\" + InstallFolderName;
                }
                InstallCatalogTb.Text = selectPath;
            }
        }

        /// <summary>
        /// 启动站播小助手
        /// </summary>
        /// <returns></returns>
        private bool OpenTeacherLiveAssistantToolsStart()
        {
            Process processMain = new Process();
            processMain.StartInfo.FileName = InstallCatalogTb.Text.Trim() + @"\csr-windows.Client.exe";
            processMain.StartInfo.WorkingDirectory = InstallCatalogTb.Text.Trim();//
            processMain.StartInfo.CreateNoWindow = true;
            processMain.Start();

            int retries = 40;
            SpinWait sw = new SpinWait();
            while (retries-- > 0)
            {
                Process[] proc = Process.GetProcessesByName("csr-windows.Client");
                if (proc.Any())
                {
                    return true;
                }
                sw.SpinOnce();
            }
            return false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && (e.SystemKey == Key.F4 || e.SystemKey == Key.Space))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// 立即体验
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartApplicationBt_Click(object sender, RoutedEventArgs e)
        {
            StartApplicationBt.IsEnabled = false;
            if (OpenTeacherLiveAssistantToolsStart())
            {
                Close();
            }
            else
            {
                StartApplicationBt.IsEnabled = true;
            }
        }

        private void ReInstallBt_Click(object sender, RoutedEventArgs e)
        {
            InstallAfterFailedGrid.Visibility = Visibility.Collapsed;
            string path = InstallCatalogTb.Text.Trim();
            InstallProgram(path);
        }

        /// <summary>
        /// 杀掉FoxitReader进程
        /// </summary>
        /// <param name="strProcessesByName"></param>
        public static void KillProcess(string processName)
        {
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName.Contains(processName))
                {
                    try
                    {
                        p.Kill();
                        p.WaitForExit(); // possibly with a timeout
                        //Logger.WriteInfo($"已杀掉{processName}进程！！！");
                    }
                    catch (Win32Exception e)
                    {
                        Console.WriteLine(e.Message.ToString());
                    }
                    catch (InvalidOperationException e)
                    {
                        Console.WriteLine(e.Message.ToString());
                    }
                }

            }
        }
    }
}
