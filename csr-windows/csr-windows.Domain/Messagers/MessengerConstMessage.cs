﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain
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

        /// <summary>
        /// 改变当前客户
        /// </summary>
        public const string ChangeCurrentCustomerToken = "ChangeCurrentCustomerToken";

        #region 聊天内容的Toekn

        /// <summary>
        /// 我该怎么回
        /// </summary>
        public const string AskAIToken = "AskAIToken";

        /// <summary>
        /// 我该怎么回解析
        /// </summary>
        public const string AskAIResponseToken = "AskAIResponseToken";

        /// <summary>
        /// 发送Msg单个商品
        /// </summary>
        public const string SendMsgSingleProductToken = "SendMsgSingleProductToken";

        /// <summary>
        /// 发送Msg多个商品
        /// </summary>
        public const string SendMsgMultipleProductToken = "SendMsgMultipleProductToken";

        /// <summary>
        /// 切换商品（客服的）
        /// </summary>
        public const string SendChangeProductCustomerServerToken = "SendChangeProductCustomerServerToken";

        /// <summary>
        /// 切换商品（客户的）
        /// </summary>
        public const string SendChangeProductCustomerToken = "SendChangeProductCustomerToken";

        #endregion

        #region HTTPError
        /// <summary>
        /// 大模型Http异常
        /// </summary>
        public const string ApiChatHttpErrorToken = "ApiChatHttpErrorToken";
        #endregion

      


    }
}