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
        /// Follow千牛
        /// </summary>
        public const string FollowWindowToken = "FollowWindowToken";

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
        /// 打开顾客界面窗体
        /// </summary>
        public const string OpenCustomerUserControlToken  = "OpenCustomerUserControlToken";

        /// <summary>
        /// 关闭菜单的窗体
        /// </summary>
        public const string CloseMenuUserControlToken = "CloseMenuUserControlToken";

        /// <summary>
        /// 关闭退出界面
        /// </summary>
        public const string CloseLogoutViewToken = "CloseLogoutViewToken";

        /// <summary>
        /// 退出程序
        /// </summary>
        public const string ExitToken= "ExitToken";

        /// <summary>
        /// 登录成功
        /// </summary>
        public const string LoginSuccessToken = "LoginSuccessToken";



    }
}
