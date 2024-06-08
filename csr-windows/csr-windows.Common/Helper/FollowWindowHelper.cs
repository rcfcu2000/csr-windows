using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace csr_windows.Common.Helper
{
    public class FollowWindowHelper
    {
        public const string ProcessName = "AliWorkbench"; // 要检测的进程名称，不需要包含扩展名

        public static bool GetQianNiuIntPrt(ref IntPtr intPtr)
        {

            string targetProcess = "AliWorkbench";
            string targetTitlePart = "接待中心";

            bool found = false;

            // Get the desktop element
            AutomationElement desktop = AutomationElement.RootElement;

            // Get all top-level windows
            System.Windows.Automation.Condition condition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window);

            // Find all windows matching the condition
            AutomationElementCollection windows = desktop.FindAll(TreeScope.Children, condition);

            foreach (AutomationElement window in windows)
            {
                try
                {
                    string windowTitle = window.Current.Name;
                    IntPtr windowHandle = new IntPtr(window.Current.NativeWindowHandle);

                    // Get the process ID of the window
                    Win32.GetWindowThreadProcessId(windowHandle, out uint processId);
                    var _intPtr = Win32.FindWindowEx(windowHandle, IntPtr.Zero, "pane", String.Empty);

                    // Get the process name using the process ID
                    Process process = Process.GetProcessById((int)processId);
                    string processName = process.ProcessName;

                    if (processName == targetProcess && windowTitle.Contains(targetTitlePart))
                    {
                        Console.WriteLine($"Window found with title containing '{targetTitlePart}': {windowTitle}");
                        intPtr = new IntPtr(window.Current.NativeWindowHandle);
                        Console.WriteLine($"Find IntPtr:{intPtr}");
                        found = true;
                        break;
                    }
                }
                catch (Exception)
                {
                    continue;
                }
              
            }

            if (!found)
            {
                Console.WriteLine($"No window found with title containing '{targetTitlePart}'.");
            }
            return found;
        }

        /// <summary>
        /// 判断进程是否启动
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static bool IsProcessRunning(string processName)
        {
            // 获取所有运行中的进程
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Any();
        }

        /// <summary>
        /// 查找程序路径
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public static string FindProgramPath(string displayName)
        {
            // 遍历所有卸载信息的注册表路径
            string[] uninstallKeys = new string[]
            {
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
            @"Software\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            foreach (string uninstallKey in uninstallKeys)
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(uninstallKey) ??
                                         Registry.CurrentUser.OpenSubKey(uninstallKey))
                {
                    if (key != null)
                    {
                        foreach (string subkeyName in key.GetSubKeyNames())
                        {
                            using (RegistryKey subkey = key.OpenSubKey(subkeyName))
                            {
                                if (subkey != null)
                                {
                                    string currentDisplayName = subkey.GetValue("DisplayName") as string;
                                    if (string.Equals(currentDisplayName, displayName, StringComparison.OrdinalIgnoreCase))
                                    {
                                        string installLocation = subkey.GetValue("InstallLocation") as string;
                                        string uninstallString = subkey.GetValue("UninstallString") as string;

                                        // 检查安装路径
                                        if (!string.IsNullOrEmpty(installLocation))
                                        {
                                            return installLocation;
                                        }

                                        // 如果安装路径为空，检查卸载字符串
                                        if (!string.IsNullOrEmpty(uninstallString))
                                        {
                                            // 如果卸载字符串是路径，则返回路径
                                            if (System.IO.File.Exists(uninstallString))
                                            {
                                                return uninstallString;
                                            }
                                            else
                                            {
                                                // 如果卸载字符串包含可执行文件路径，提取路径
                                                string[] parts = uninstallString.Split('"');
                                                foreach (string part in parts)
                                                {
                                                    if (System.IO.File.Exists(part))
                                                    {
                                                        return part;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
