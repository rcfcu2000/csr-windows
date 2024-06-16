using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

using System.Windows.Automation;
using System.Windows.Automation.Text;
using ManagedWinapi.Windows;
using ManagedWinapi.Accessibility;

namespace csr_windows.Common.Utils
{
    public static class AutomationUtil
    {
        // Import the necessary Windows API functions
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr SendMessageW(IntPtr hWnd, UInt32 Msg, IntPtr wParam, [Out] StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public static string findQNChatTitle()
        {

            string targetProcess = "AliWorkbench";
            string targetTitlePart = "接待中心";

            // Get the desktop element
            AutomationElement desktop = AutomationElement.RootElement;

            // Get all top-level windows
            Condition condition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window);

            // Find all windows matching the condition
            AutomationElementCollection windows = desktop.FindAll(TreeScope.Children, condition);

            foreach (AutomationElement window in windows)
            {
                string windowTitle = window.Current.Name;
                IntPtr windowHandle = new IntPtr(window.Current.NativeWindowHandle);

                // Get the process ID of the window
                GetWindowThreadProcessId(windowHandle, out uint processId);
                IntPtr w11 = FindWindowEx(windowHandle, IntPtr.Zero, "pane", String.Empty);

                // Get the process name using the process ID
                Process process = Process.GetProcessById((int)processId);
                string processName = process.ProcessName;

                if (processName == targetProcess && windowTitle.Contains(targetTitlePart))
                {
                    return WalkControlElements(window);
                }
            }
            return null;
        }

        private static string WalkControlElements(AutomationElement rootElement)
        {
            //// Conditions for the basic views of the subtree (content, control, and raw) 
            //// are available as fields of TreeWalker, and one of these is used in the 
            //// following code.
            Condition condition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Tab);

            // Find all windows matching the condition
            AutomationElementCollection windows = rootElement.FindAll(TreeScope.Children, condition);

            foreach (AutomationElement window in windows)
            {
                return window.Current.Name;
            }

            return null;
        }
    }
}
