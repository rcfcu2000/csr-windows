using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sunny.UI;
using Sunny.UI.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;
using System.Windows;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace csr_new
{
    public class TopHelp
    {
        // 导入用户32库中的函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        // 导入获取窗口标题长度的函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern int GetWindowTextLength(IntPtr hWnd);
        // 发送消息到窗口
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        // 导入获取窗口标题的函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder title, int size);

        // 导入user32.dll中的函数，用于激活窗口
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        //模拟鼠标点击
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern IntPtr SetActiveWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;


        // 辅助方法：遍历顶层窗口并查找匹配的窗口标题
        private IntPtr FindTopLevelWindowsClassName(uint processId, IntPtr hWndStart, string className)
        {
            hWndStart = FindWindowEx(IntPtr.Zero, hWndStart, null, null); // 开始遍历

            while (hWndStart != IntPtr.Zero)
            {
                uint currentProcessId;
                GetWindowThreadProcessId(hWndStart, out currentProcessId);

                if (currentProcessId == processId)
                {
                    // 检查窗口标题是否包含子串
                    string clsName = GetWindowClassName(hWndStart);
                    if (clsName.Contains(className))
                    {
                        return hWndStart;
                    }
                }

                hWndStart = GetWindow(hWndStart, 2 /* GW_HWNDNEXT */);
            }

            return IntPtr.Zero;
        }

        private IntPtr FindTopLevelWindowsTittle(uint processId, IntPtr hWndStart, string titleSubstring)
        {
            hWndStart = FindWindowEx(IntPtr.Zero, hWndStart, null, null); // 开始遍历

            while (hWndStart != IntPtr.Zero)
            {
                uint currentProcessId;
                GetWindowThreadProcessId(hWndStart, out currentProcessId);

                if (currentProcessId == processId)
                {
                    // 检查窗口标题是否包含子串
                    string windowTitle = GetWindowTitle(hWndStart);
                    if (windowTitle.Contains(titleSubstring))
                    {
                        return hWndStart;
                    }
                }

                hWndStart = GetWindow(hWndStart, 2 /* GW_HWNDNEXT */);
            }

            return IntPtr.Zero;
        }

        // 获取窗口标题的辅助方法
        private string GetWindowTitle(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            if (length > 0)
            {
                var builder = new System.Text.StringBuilder(length + 1);
                GetWindowText(hWnd, builder, length + 1);
                return builder.ToString();
            }
            return String.Empty;
        }


        // 方法：根据进程名和窗口标题子串查找窗口句柄
        public IntPtr FindWindowByProcessAndTitle(string processName, string titleSubstring)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
            {
                Console.WriteLine($"No process found with the name '{processName}'.");
                return IntPtr.Zero;
            }

            uint processId = 0;
            IntPtr hWnd = IntPtr.Zero;
            foreach (Process process in processes)
            {
                processId = (uint)process.Id;

                // 遍历顶层窗口
                hWnd = FindTopLevelWindowsTittle(processId, hWnd, titleSubstring);
                if (hWnd != IntPtr.Zero)
                {
                    break; // 找到后立即返回
                }
            }

            return hWnd;
        }
        private static string GetWindowClassName(IntPtr hWnd)
        {
            var sb = new StringBuilder(256);
            GetClassName(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        /// <returns>找到的窗口句柄，未找到则返回IntPtr.Zero</returns>
        public IntPtr FindWindowByProcessAndClassName(string processName, string className)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
            {
                Console.WriteLine($"No process found with the name '{processName}'.");
                return IntPtr.Zero;
            }

            IntPtr hWnd = IntPtr.Zero;
            uint processId = 0;

            foreach (Process process in processes)
            {
                processId = (uint)process.Id;

                // 遍历顶层窗口
                hWnd = FindTopLevelWindowsClassName(processId, hWnd, className);
                if (hWnd != IntPtr.Zero)
                {
                    break; // 找到后立即返回
                }
            }

            Console.WriteLine("未找到匹配的窗口。");
            return hWnd;

        }



        // 方法：关闭指定进程和窗口标题子串的窗口,窗口标题支持模糊查找


        public bool CloseWindowByProcessAndTitle(string processName, string titleSubstring)
        {

            IntPtr windowHandle = FindWindowByProcessAndTitle(processName, titleSubstring);

            if (windowHandle != IntPtr.Zero)
            {
                // 如果找到了窗口，尝试发送WM_CLOSE消息关闭它
                const int WM_CLOSE = 0x0010;
                return SendMessage(windowHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero) == IntPtr.Zero;
            }

            // 如果没找到窗口，返回false表示操作失败
            return false;
        }


        // 方法：唤起aliim聊天窗口，构造aliim协议的URL，其中nick为交谈对象
        public bool OpenAliim(string nick)
        {
            string url = "aliim:sendmsg?uid=cntaobao&touid=cntaobao" + nick;

            // 使用System.Diagnostics.Process启动默认浏览器并打开URL
            try
            {
                Process.Start(new ProcessStartInfo(url)
                {
                    UseShellExecute = true, // 需要为true以使用Shell执行操作
                });
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"无法打开链接: {ex.Message}");
                Logger.WriteError($"唤起aliim聊天窗口失败: {ex.Message}");
                return false;
            }
        }





        //给指定买家发消息
        public bool QNSendMsg(string nick, string msg, int sleep = 200)
        {
            //bool isSuccess = OpenAliim(nick);
            bool isSuccess = true;
            if (isSuccess)
            {
                IntPtr hwd = FindWindowByProcessAndTitle("AliWorkbench", "-接待中心");

                if (hwd == IntPtr.Zero)
                {
                    Console.WriteLine("窗口句柄无效。");
                    Logger.WriteError("没有找到 接待中心 窗口");
                    return false;
                }
                else
                {
                    // 找到窗口后默认要延时200毫秒，否则会发失败(增加鼠标点击以后好像不延时也行)
                    Thread.Sleep(sleep);

                    // 尝试将目标窗口设置为前台活动窗口
                    //if (SetForegroundWindow(hwd))
                    //{
                    // 将相对坐标转换为屏幕坐标
                    Point clientPoint = new Point(500, 500);
                    ClientToScreen(hwd, ref clientPoint);

                    for (int i = 0; i < 2; i++)
                    {
                        // 将鼠标光标移动到指定位置
                        SetCursorPos((int)clientPoint.X, (int)clientPoint.Y);
                        // 模拟鼠标左键按下和释放
                        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                        Thread.Sleep(5);
                    }


                    try
                    {
                        // 使用SendKeys类模拟输入文本消息
                        SendKeys.SendWait(msg + "^{ENTER}");
                        //SendTextToWindow(hwd,msg);
                        Console.WriteLine($"消息 '{msg}' 已发送至窗口 {hwd.ToInt64()}");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // 捕获SendKeys异常
                        Console.WriteLine("给指定买家发消息发送按键时发生错误: " + ex.Message);
                        Logger.WriteError($"给买家{nick}发消息时发生错误:{ex.Message}");
                        return false;
                    }

                    //}
                    //else
                    //{
                    //    Console.WriteLine("无法将窗口置于前台。");
                    //    Logger.WriteError("无法将 接待中心 窗口置于前台");
                    //    return false;
                    //}
                }

            }
            else
            {
                //唤起聊天失败
                Console.WriteLine("唤起聊天失败。");
                Logger.WriteError("唤起聊天失败。");
                return false;
            }
        }

        public string QNSendMsgJS(string nick, string msg)
        {

            dynamic root = new JObject();
            root.act = "sendMsg";
            // 添加userid和msg值
            root.param = new JObject();
            root.param.userid = nick;
            root.param.msg = msg;

            // 将对象转换成JSON字符串
            string jsonString = JsonConvert.SerializeObject(root, Formatting.Indented);
            return jsonString;
        }
        //淘宝-获取待发货订单列表

        public bool QNSendMsgVer912(string nick, string msg, int sleep = 200)
        {
            //bool isSuccess = OpenAliim(nick);

            POINT lpPoint;
            GetCursorPos(out lpPoint);

            Console.WriteLine($"cursor: {lpPoint.X}, {lpPoint.Y}");

            bool isSuccess = true;
            if (isSuccess)
            {
                IntPtr hwd = FindWindowByProcessAndTitle("AliWorkbench", "-接待中心");

                if (hwd == IntPtr.Zero)
                {
                    Console.WriteLine("窗口句柄无效。");
                    Logger.WriteError("没有找到 接待中心 窗口");
                    return false;
                }
                else
                {
                    // 找到窗口后默认要延时200毫秒，否则会发失败(增加鼠标点击以后好像不延时也行)
                    Thread.Sleep(sleep);

                    // 尝试将目标窗口设置为前台活动窗口
                    if (SetForegroundWindow(hwd))
                    {
                        // 将相对坐标转换为屏幕坐标

                        SetActiveWindow(hwd);

                        RECT rect = new RECT();
                        GetWindowRect(hwd, ref rect);

                        Console.WriteLine($"window rect: {rect.Left}, {rect.Top}, {rect.Right}, {rect.Bottom}");

                        Point clientPoint = new Point((rect.Right + rect.Left) / 2, rect.Top + (rect.Bottom - rect.Top)* 0.7);
                        ClientToScreen(hwd, ref clientPoint);

                        for (int i = 0; i < 2; i++)
                        {
                            // 将鼠标光标移动到指定位置
                            SetCursorPos((int)clientPoint.X, (int)clientPoint.Y);
                            Console.WriteLine($"set cursor: {clientPoint.X}, {clientPoint.Y}");
                            // 模拟鼠标左键按下和释放
                            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
                            Thread.Sleep(5);
                        }


                        try
                        {

                            Thread th = new Thread(new ThreadStart(delegate ()
                            {
                                System.Windows.Clipboard.SetText(msg);
                                //System.Windows.Clipboard.SetDataObject(msg);
                            }));
                            th.TrySetApartmentState(ApartmentState.STA);
                            th.Start();
                            th.Join();

                            // 使用SendKeys类模拟输入文本消息
                            SendKeys.SendWait("^v");
                            SendKeys.SendWait("{ENTER}");
                            //SendTextToWindow(hwd,msg);
                            Console.WriteLine($"消息 '{msg}' 已发送至窗口 {hwd.ToInt64()}");
                            return true;
                        }
                        catch (Exception ex)
                        {
                            // 捕获SendKeys异常
                            Console.WriteLine("给指定买家发消息发送按键时发生错误: " + ex.Message);
                            Logger.WriteError($"给买家{nick}发消息时发生错误:{ex.Message}");
                            return false;
                        }

                    }
                    else
                    {
                        Console.WriteLine("无法将窗口置于前台。");
                        Logger.WriteError("无法将 接待中心 窗口置于前台");
                        return false;
                    }
                }

            }
            else
            {
                //唤起聊天失败
                Console.WriteLine("唤起聊天失败。");
                Logger.WriteError("唤起聊天失败。");
                return false;
            }
        }
    }
}

