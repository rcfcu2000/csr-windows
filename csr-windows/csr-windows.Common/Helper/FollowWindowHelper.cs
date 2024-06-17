﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
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

                        CheckVersion();

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
                //Console.WriteLine($"No window found with title containing '{targetTitlePart}'.");
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

        public static void RunQNProcessAtVersion(int major, int minor)
        {
            // 获取所有运行中的进程
            Process[] processes = Process.GetProcessesByName(ProcessName);
            bool restartQN = false;
            if (processes.Length <= 0)
            {
                restartQN = true;
            }
            else
            {
                FileVersionInfo qnVersion = processes[0].MainModule.FileVersionInfo;
                if (!(qnVersion != null && qnVersion.FileMajorPart == major && qnVersion.FileMinorPart == minor))
                {
                    processes[0].Kill();
                    restartQN = true;
                }
            }

            if (restartQN) {
                string programPath = $@"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}others{Path.DirectorySeparatorChar}AliWorkbench_9.12.01N{Path.DirectorySeparatorChar}AliWorkbench.exe";

                string programDisplayName = "";


                try
                {
                    Process process = Process.Start(programPath);
                    Console.WriteLine($"{programDisplayName} started successfully from: {programPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to start {programDisplayName}: {ex.Message}");
                }
            }
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

        public static bool CheckVersion()
        {
            // 创建WMI查询对象
            string query = "SELECT * FROM Win32_Process WHERE Name LIKE '%" + ProcessName + "%'";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            foreach (ManagementObject process in searcher.Get())
            {
                try
                {
                    // 获取进程ID
                    int processId = Convert.ToInt32(process["ProcessId"]);
                    // 获取进程路径
                    string executablePath = process["ExecutablePath"]?.ToString();

                    if (!string.IsNullOrEmpty(executablePath))
                    {
                        // 读取文件版本信息
                        var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(executablePath);
                        Console.WriteLine($"Process ID: {processId}, Version: {versionInfo.FileVersion}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error retrieving version info: {ex.Message}");
                }
            }
            return true;
        }
    }
}
