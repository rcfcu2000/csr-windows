using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Resources.Enumeration
{
    /// <summary>
    /// 文本身份枚举
    /// </summary>
    public enum ChatIdentityEnum
    {
        /// <summary>
        /// 接收者（左边）
        /// </summary>
        Recipient,
        /// <summary>
        /// 发送者（右边）
        /// </summary>
        Sender
    }

    /// <summary>
    /// 文本跟商品的身份枚举
    /// </summary>
    public enum ChatTextAndProductIdentidyEnum
    {
        /// <summary>
        /// 客服
        /// </summary>
        CustomerService,
        /// <summary>
        /// 客户
        /// </summary>
        Customer
    }

    /// <summary>
    /// 文本内容枚举
    /// </summary>
    public enum ChatTypeEnum
    {
        /// <summary>
        /// 复制文本
        /// </summary>
        CopyText,

        /// <summary>
        /// 纯文本
        /// </summary>
       Text,

       /// <summary>
       /// 最下面是粗文本
       /// </summary>
       BottomBoldText,

        /// <summary>
        /// 文本 + 商品
        /// </summary>
        TextAndProduct,

        /// <summary>
        /// 加载中
        /// </summary>
        Loading

    }

    /// <summary>
    /// 销售的类型
    /// </summary>
    public enum SalesRepType
    {
        /// <summary>
        /// 售前
        /// </summary>
        PreSale = 1,

        /// <summary>
        /// 售后
        /// </summary>
        AfterSale,
    }
}
