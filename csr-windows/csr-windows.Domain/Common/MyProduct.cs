using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.Common
{
    public class MyProduct
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品图片
        /// </summary>
        public string ProductImage { get; set; }

        /// <summary>
        /// 商品Url
        /// </summary>
        public string ProductUrl { get; set; }

        /// <summary>
        /// 商品信息
        /// </summary>
        public string ProductInfo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// MerchantID
        /// </summary>
        public int MerchantId { get; set; }
    }
}
