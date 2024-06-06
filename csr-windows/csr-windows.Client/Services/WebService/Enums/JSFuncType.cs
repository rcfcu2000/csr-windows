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

        #region 接收

        /// <summary>
        /// 接收当前用户
        /// </summary>
        public const string ReceiveCurrentCst = "currentCsr";

        /// <summary>
        /// 接收当前客户
        /// </summary>
        public const string ReceiverCurrentConv = "currentConv";

        /// <summary>
        /// 接收客户改变
        /// </summary>
        public const string ReceiveConvChange = "conv_change";
        #endregion

    }
}
