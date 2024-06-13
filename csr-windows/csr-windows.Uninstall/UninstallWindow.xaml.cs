using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace csr_windows.Uninstall
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UninstallWindow : Window
    {
        const string AppName = "会回";
        const string MenuFolder = @"\会回\";
        const string CsrWindowsStartShortcutName = "会回.lnk";
        const string UninstallShortcutName = "卸载会回.lnk";
        const string UninstallExeName = "csr-windows.Uninstall.exe";

        /// <summary>
        /// 是否已卸载
        /// </summary>
        private bool _isUninstalled = false;

        public UninstallWindow()
        {
            InitializeComponent();
            if (Common.Common.GetIsExistSameProgram())
            {
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// 卸载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            UninstallBeforeSp.Visibility = Visibility.Collapsed;
            UninstallingSp.Visibility = Visibility.Visible;
            CloseButton.Visibility = Visibility.Collapsed;
            try
            {
                //Kill会回相关程序
                KillLivePrograms();
                //卸载会回相关程序文件
                UninstallFile().ContinueWith(t =>
                {
                    //卸载快捷方式 
                    UninstallShortcut();
                    _isUninstalled = true;
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        UninstallingSp.Visibility = Visibility.Collapsed;
                        UninstallAfterSuccessedSp.Visibility = Visibility.Visible;
                        CloseButton.Visibility = Visibility.Visible;
                    }));
                });
            }
            catch
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    UninstallingSp.Visibility = Visibility.Collapsed;
                    UninstallAfterFailedSp.Visibility = Visibility.Visible;
                    CloseButton.Visibility = Visibility.Visible;
                }));
            }
        }

        /// <summary>
        /// Kill会回相关程序
        /// </summary>
        private void KillLivePrograms()
        {
            //Kill会回启动程序
            Common.Common.KillProcess("csr-windows.Client");
        }

        /// <summary>
        /// 卸载站播小助手程序（除卸载程序外的其他文件）
        /// </summary>
        /// <returns></returns>
        private Task UninstallFile()
        {
            return Task.Factory.StartNew(() =>
            {
                string mainProgramPath = AppDomain.CurrentDomain.BaseDirectory;
                //不能删除会回的父文件夹
                List<string> noDelFolder = new List<string> { mainProgramPath };
                //不能删除卸载程序
                List<string> noDelFile = new List<string> { UninstallExeName };

                int fileNum = 0, maxFileNum = 0, deleteFileNum = 0;
                //获取会回的文件总数
                Common.Common.GetFileNumber(mainProgramPath, ref fileNum);
                maxFileNum = fileNum - noDelFile.Count;

                Common.Common.DeleteFile(mainProgramPath, noDelFolder, noDelFile, ref deleteFileNum, maxFileNum, (accumNum, maxNum) =>
                {
                    //卸载会回进度为97
                    double value = 1.0 * accumNum / maxNum * 97;
                    ProgressEvent(value);
                });
            });
        }

        /// <summary>
        /// 更新进度条
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxValue"></param>
        private void ProgressEvent(double value)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                UninstallProgressBar.Value = (int)value;
            }));
        }

        /// <summary>
        /// 卸载快捷方式
        /// </summary>
        private void UninstallShortcut()
        {
            try
            {
                //获取系统默认目录
                RegistryKey HKEY_CURRENT_USER = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders");

                #region 卸载-开始菜单
                //获取开始菜单程序文件夹路径
                string programsPath = HKEY_CURRENT_USER.GetValue("Programs").ToString();

                //开始菜单文件夹路径
                string menuFolderPath = programsPath + MenuFolder;
                //开始菜单-会回端程序路径
                string shortcutPath = menuFolderPath + CsrWindowsStartShortcutName;
                //开始菜单-卸载程序路径
                string uninstallShortcutPath = menuFolderPath + UninstallShortcutName;

                //删除开始菜单快捷方式
                if (Directory.Exists(menuFolderPath))
                {
                    if (File.Exists(shortcutPath))
                    {
                        File.Delete(shortcutPath);
                    }
                    if (File.Exists(uninstallShortcutPath))
                    {
                        File.Delete(uninstallShortcutPath);
                    }
                    Directory.Delete(menuFolderPath, true);
                }

                ProgressEvent(98);
                #endregion

                #region 卸载桌面快捷方式
                //教师端快捷方式路径
                string desktopShortcutPath = HKEY_CURRENT_USER.GetValue("Desktop").ToString() + @"\" + CsrWindowsStartShortcutName;

                //删除桌面快捷方式
                if (File.Exists(desktopShortcutPath))
                {
                    File.Delete(desktopShortcutPath);
                }

                ProgressEvent(99);
                #endregion

                #region 程序与功能
                //常见控制面板“程序与功能”
                RegistryKey CUKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
                string keyName = @"Software\Microsoft\Windows\CurrentVersion\Uninstall\" + AppName;
                RegistryKey CurrentKey = CUKey.OpenSubKey(keyName, true);
                if (CurrentKey != null)
                {
                    //删除
                    CUKey.DeleteSubKeyTree(keyName);
                    CurrentKey.Close();
                }

                ProgressEvent(100);
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 卸载结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UninstallAfterBt_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_isUninstalled)
            {
                //删除卸载程序自身，以及文件夹
                Common.Common.DeleteItselfByCMD();
            }
            Application.Current.Shutdown();
        }
    }
}
