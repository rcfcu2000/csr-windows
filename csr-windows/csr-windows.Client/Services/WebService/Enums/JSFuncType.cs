using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Client.Services.WebService.Enums
{
    public static class JSFuncType
    {
        /// <summary>
        /// 获取热销商品列表
        /// </summary>
        public const string GetGoodsList = "getGoodsList";
        /// <summary>
        /// 获取当前用户
        /// </summary>
        public const string GetCurrentCsr = "getCurrentCsr";



        /// <summary>
        /// 获取当前客户
        /// </summary>
        public const string GetCurrentConv = "getCurrentConv";

        /// <summary>
        /// 获取历史记录
        /// </summary>
        public const string GetRemoteHisMsg = "getRemoteHisMsg";

        #region 接收

        /// <summary>
        /// 接收当前用户
        /// </summary>
        public const string ReceiveCurrentCsr = "currentCsr";

        /// <summary>
        /// 接收当前客户
        /// </summary>
        public const string ReceiverCurrentConv = "currentConv";

        /// <summary>
        /// 接收客户改变
        /// </summary>
        public const string ReceiveConvChange = "conv_change";


        /// <summary>
        /// 主动接收历史消息
        /// </summary>
        public const string ActiveReceiveRemoteHisMsg = "remote_his_message";
        #endregion

    }
}
