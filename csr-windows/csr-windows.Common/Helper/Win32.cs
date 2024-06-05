using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Common.Helper
{
    public class Win32
    {
        // Import the necessary Windows API functions
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessageW(IntPtr hWnd, UInt32 Msg, IntPtr wParam, [Out] StringBuilder lParam);

        [DllImport("user32.dll")]
        public static extern int SetParent(IntPtr hWndChild, IntPtr hWndParent);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint newLong);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int EnumWindows(CallBackPtr callPtr, ref WindowInfo WndInfoRef);

        [DllImport("User32.dll")]
        public static extern int GetWindowText(IntPtr handle, StringBuilder text, int MaxLen);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string classname, string windowname);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowExW(IntPtr handle, IntPtr cHandle, byte[] classname, byte[] windowname);

        [DllImport("user32.dll")]
        public static extern int GetWindowRect(IntPtr hwnd, ref RECT rc);

        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, uint wMsg, int wParam, int lParam); //对外部软件窗口发送一些消息(如 窗口最大化、最小化等)

        [DllImport("user32.dll", EntryPoint = "SetWindowPos", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int SetWindowPos(IntPtr hwnd, IntPtr hWndInsertAfter, int x, int y, int w, int h, uint uflags);

        [DllImport("user32.dll", EntryPoint = "MoveWindow", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool MoveWindow(IntPtr hwnd, int x, int y, int w, int h, bool bRepaint);

        [DllImport("user32.dll")]
        public static extern int ShowWindow(IntPtr hwnd, uint nCmdShow);

        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetFocus(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hwnd, bool fUnknown);


        #region 控制鼠标移动

        //移动鼠标 
        public const int MOUSEEVENTF_MOVE = 0x0001;
        //模拟鼠标左键按下 
        public const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        //模拟鼠标左键抬起 
        public const int MOUSEEVENTF_LEFTUP = 0x0004;
        //模拟鼠标右键按下 
        public const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        //模拟鼠标右键抬起 
        public const int MOUSEEVENTF_RIGHTUP = 0x0010;
        //模拟鼠标中键按下 
        public const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        //模拟鼠标中键抬起 
        public const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        //标示是否采用绝对坐标 
        public const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        [Flags]
        public enum MouseEventFlag : uint
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            MiddleDown = 0x0020,
            MiddleUp = 0x0040,
            XDown = 0x0080,
            XUp = 0x0100,
            Wheel = 0x0800,
            VirtualDesk = 0x4000,
            Absolute = 0x8000
        }

        //[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")]
        public static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        #endregion



        public const int
            GWL_WNDPROC = (-4),
            GWL_HINSTANCE = (-6),
            GWL_HWNDPARENT = (-8),
            GWL_STYLE = (-16),
            GWL_EXSTYLE = (-20),
            GWL_USERDATA = (-21),
            GWL_ID = (-12);

        public const uint
              WS_CHILD = 0x40000000,
              WS_VISIBLE = 0x10000000,
              LBS_NOTIFY = 0x00000001,
              HOST_ID = 0x00000002,
              LISTBOX_ID = 0x00000001,
              WS_VSCROLL = 0x00200000,
              WS_BORDER = 0x00800000,
              WS_POPUP = 0x80000000,
              WS_SIZEBOX = 0x00040000,
              WS_SYSMENU = 0x00080000,
              WS_CLIPCHILDREN = 0x02000000,
              WS_CLIPSIBLINGS = 0x04000000;

        public const int HWND_TOP = 0x0;
        public const int WM_COMMAND = 0x0112;
        public const int WM_QT_PAINT = 0xC2DC;
        public const int WM_PAINT = 0x000F;
        /// <summary>
        /// 改变一个窗口的大小
        /// </summary>
        public const int WM_SIZE = 0x0005;
        public const int SWP_FRAMECHANGED = 0x0020;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const uint WM_LBUTTONDOWN = 0x0201;
        public const uint WM_LBUTTONUP = 0x0202;
        public const int BM_CLICK = 0xF5;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WindowInfo
        {
            public String winTitle;
            public RECT r;
            public IntPtr hwnd;
        }
        public delegate bool CallBackPtr(IntPtr hwnd, ref WindowInfo WndInfoRef);

        public static CallBackPtr CallBackPtrHandler;
    }
}
