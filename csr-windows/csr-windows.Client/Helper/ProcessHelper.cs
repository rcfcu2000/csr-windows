using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Client.Helpers
{
    public class ProcessHelper
    {
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
