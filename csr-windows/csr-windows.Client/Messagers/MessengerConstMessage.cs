using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Client
{
    public static class MessengerConstMessage
    {
        /// <summary>
        /// 打开Main中的窗体
        /// </summary>
        public const string OpenMainUserControlToken = "OpenMainUserControlToken";

        /// <summary>
        /// 打开PromptMessage
        /// </summary>
        public const string OpenPromptMessageToken = "OpenPromptMessageToken";

        /// <summary>
        /// 打开菜单的窗体
        /// </summary>
        public const string OpenMenuUserControlToken = "OpenMenuUserControlToken";

        /// <summary>
        /// 关闭菜单的窗体
        /// </summary>
        public const string CloseMenuUserControlToken = "CloseMenuUserControlToken";
    }
}
