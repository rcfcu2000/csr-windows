using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Install.Common
{
    public static class Common
    {
        /// <summary>
        /// 检查路径是否合理
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckPathIsReasonable(string path)
        {
            //遍历用户录入路径字符串的每一个字符
            foreach (char userPathChar in path.ToArray<char>())
            {
                //判断用户录入的路径字符串中是否包含有特殊非法字符
                foreach (var pathChars in System.IO.Path.GetInvalidPathChars())
                {
                    if (userPathChar.Equals(pathChars))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 创建快捷方式
        /// </summary>
        /// <param name="exePath">目标exe地址</param>
        /// <param name="shortcutPath">快捷方式的地址</param>
        /// <returns></returns>
        public static bool CreateShortcut(string exePath, string shortcutPath)
        {
            try
            {
                if (!System.IO.File.Exists(exePath))
                {
                    return false;
                }
                WshShell shell = new WshShell();

                //快捷键方式创建的位置、名称
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = exePath; //目标文件 //该属性指定应用程序的工作目录，当用户没有指定一个具体的目录时，快捷方式的目标应用程序将使用该属性所指定的目录来装载或保存文件。
                shortcut.WorkingDirectory = Path.GetDirectoryName(exePath);
                shortcut.WindowStyle = 1; //目标应用程序的窗口状态分为普通、最大化、最小化【1,3,7】
                shortcut.Description = Path.GetFileName(shortcutPath); //描述
                shortcut.Save(); //必须调用保存快捷才成创建成功
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// kill卸载程序除了本次
        /// </summary>
        public static void KillInstallExceptSelf()
        {
            Process[] proc = Process.GetProcessesByName(Assembly.GetExecutingAssembly().GetName().Name);
            if (proc.Length > 1)
            {
                Process currentProcess = Process.GetCurrentProcess();
                for (int i = 0; i < proc.Length; i++)
                {
                    //kill除自己以外的进程
                    if (proc[i].Id != currentProcess.Id)
                    {
                        try
                        {
                            proc[i].Kill();
                        }
                        catch { continue; }
                    }
                }
            }
        }

        /// <summary>
        /// 判断是否存在相同的程序
        /// </summary>
        /// <returns></returns>
        public static bool GetIsExistSameProgram()
        {
            Process[] proc = Process.GetProcessesByName(Assembly.GetExecutingAssembly().GetName().Name);
            if (proc.Length > 1)
            {
                return true;
            }
            return false;
        }
    }
}
