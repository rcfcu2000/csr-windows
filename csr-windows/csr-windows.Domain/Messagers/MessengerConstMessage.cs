using System;
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
        /// 储存 旧当前客户的数据
        /// </summary>
        public const string StoreOldCurrentCustomerToekn = "StoreOldCurrentCustomerToekn";

        /// <summary>
        /// 改变当前客户
        /// </summary>
        public const string ChangeCurrentCustomerToken = "ChangeCurrentCustomerToken";

        /// <summary>
        /// Loading显示
        /// </summary>
        public const string ShowLoadingVisibilityChangeToken = "ShowLoadingVisibilityChangeToken";

        /// <summary>
        /// Loading隐藏
        /// </summary>
        public const string HiddenLoadingVisibilityChangeToken = "HiddenLoadingVisibilityChangeToken";

        #region 切换

        /// <summary>
        /// 切换登录用户的Token
        /// </summary>
        public const string ChangeLoginToken = "ChangeLoginToken";

        /// <summary>
        /// 获取QA正则的Token
        /// </summary>
        public const string GetQARegexToken = "GetQARegexToken";

        #endregion

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
        /// 流式返回
        /// </summary>
        public const string SSESteamReponseToken = "SSESteamReponseToken";

        /// <summary>
        /// 我想怎么回
        /// </summary>
        public const string Want2ReplyToken = "Want2ReplyToken";

        /// <summary>
        /// 我想怎么回 回调
        /// </summary>
        public const string Want2ReplyResponseToken = "Want2ReplyResponseToken";

        public const string ReMultiGoodToken = "ReMultiGoodToken";

        /// <summary>
        /// 获取热销列表
        /// </summary>
        public const string GetGoodsListToken = "GetGoodsListToken";

        /// <summary>
        /// 发送Msg单个商品
        /// </summary>
        public const string SendMsgSingleProductToken = "SendMsgSingleProductToken";

        /// <summary>
        /// 发送Msg多个商品
        /// </summary>
        public const string SendMsgMultipleProductToken = "SendMsgMultipleProductToken";

        /// <summary>
        /// 切换商品（客服的）（多个商品）
        /// </summary>
        public const string SendChangeProductCustomerServerToken = "SendChangeProductCustomerServerToken";

        /// <summary>
        /// 切换商品（客户的）（多个商品）
        /// </summary>
        public const string SendChangeProductCustomerToken = "SendChangeProductCustomerToken";

        /// <summary>
        /// 切换单个商品(点击聊天中的商品切换)
        /// </summary>
        public const string SendChangeSingleProductToken = "SendChangeSingleProductToken";

        /// <summary>
        /// 选择商品界面切换商品
        /// </summary>
        public const string ChooseProductChangeToken = "ChooseProductChangeToken";

        /// <summary>
        /// 商品介绍 选择商品
        /// </summary>
        public const string ProductIntroductionToken = "ProductIntroductionToken";

        /// <summary>
        /// 推荐搭配
        /// </summary>
        public const string RecommendedPairingToken = "RecommendedPairing";

        /// <summary>
        /// 推荐搭配接口返回
        /// </summary>
        public const string ReMultiGoodReponseToken = "ReMultiGoodReponseToken";

        /// <summary>
        /// 点击切换的时候切换商品
        /// </summary>
        public const string SendSwitchProductToken = "SendSwitchProductToken";

        #endregion

        #region HTTPError
        /// <summary>
        /// 大模型Http异常
        /// </summary>
        public const string ApiChatHttpErrorToken = "ApiChatHttpErrorToken";

        /// <summary>
        /// 主动接收远程历史消息为空
        /// </summary>
        public const string ActiveReceiveRemoteHisMsgHistoryNull = "ActiveReceiveRemoteHisMsgHistoryNull";
        #endregion




    }
}
