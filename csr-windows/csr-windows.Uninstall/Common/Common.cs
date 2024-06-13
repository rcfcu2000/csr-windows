using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Uninstall.Common
{
    public static class Common
    {
        /// <summary>
        /// 删除自身
        /// </summary>
        public static void DeleteItselfByCMD()
        {
            string baseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            StringBuilder sb = new StringBuilder();
            sb.Append(@" /C ping 1.1.1.1 -n 1 -w 1000 > Nul");//ping 一次等1秒 不输出结果
            sb.Append(" & DEL \"");//删除文件
            sb.Append(baseDirectory);
            sb.Append("\" /f /s /q");
            sb.Append(@" & cd..");//删除文件时会占用当前文件句柄？需要退出当前文件夹或者关闭当前文件夹，然后再调用rd删除文件夹。
            sb.Append(string.Format("& rd /s /q \"{0}\"", baseDirectory));
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", sb.ToString());
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.CreateNoWindow = true;
            Process.Start(psi);
        }


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="noDelFolder"></param>
        /// <param name="noDelFile"></param>
        /// <param name="deletedFileNum"></param>
        /// <param name="maxFileNum"></param>
        /// <param name="progressEvent"></param>
        public static void DeleteFile(string path, List<string> noDelFolder, List<string> noDelFile, ref int deletedFileNum, int maxFileNum, Action<int, int> progressEvent)
        {
            string[] FileSystem = null;
            try
            {
                FileSystem = Directory.GetFileSystemEntries(path);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (FileSystem != null)
            {
                foreach (string fileFullName in FileSystem)
                {
                    if (File.Exists(fileFullName))
                    {
                        FileInfo fileInfo = null;
                        fileInfo = new FileInfo(fileFullName);

                        //将只读文件改为可以删除
                        if (fileInfo?.Attributes.ToString().IndexOf("ReadOnly", StringComparison.Ordinal) != -1)
                        {
                            fileInfo.Attributes = FileAttributes.Normal;
                        }
                        //剔除不能删除的文件
                        if (!noDelFile.Any(_ => _ == fileInfo.Name))
                        {
                            try
                            {
                                File.Delete(fileFullName);
                            }
                            catch { }
                            //更新删除的文件数
                            deletedFileNum++;
                            progressEvent?.Invoke(deletedFileNum, maxFileNum);
                        }
                    }
                    else
                    {
                        DeleteFile(fileFullName, noDelFolder, noDelFile, ref deletedFileNum, maxFileNum, progressEvent);
                    }
                }

                //剔除不能删除的文件夹
                if (!noDelFolder.Any(_ => _ == path) && Directory.Exists(path))
                {
                    try
                    {
                        Directory.Delete(path, true);
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// 获取文件数量（只获取文件）
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileNum"></param>
        public static void GetFileNumber(string path, ref int fileNum)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);

                DirectoryInfo[] dirArray = null;
                dirArray = directoryInfo.GetDirectories();
                if (dirArray != null)
                {
                    foreach (DirectoryInfo item in dirArray)
                    {
                        GetFileNumber(item.FullName, ref fileNum);
                    }
                }

                FileInfo[] fileArray = null;
                fileArray = directoryInfo.GetFiles();
                if (fileArray != null)
                {
                    foreach (FileInfo item in fileArray)
                    {
                        fileNum++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// kill卸载程序除了本次
        /// </summary>
        public static void KillUninstallExceptSelf()
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

        /// <summary>
        /// kill进程
        /// </summary>
        /// <param name="processName"></param>
        public static void KillProcess(string processName)
        {
            Process[] proc = Process.GetProcessesByName(processName);
            if (proc.Any())
            {
                foreach (Process p in proc)
                {
                    try
                    {
                        p.Kill();
                        p.Dispose();
                    }
                    catch { continue; }
                }
            }
        }
    }
}
