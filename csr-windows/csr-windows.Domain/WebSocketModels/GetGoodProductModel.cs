using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.WebSocketModels
{
    public class GetGoodProductModel
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        [JsonProperty("itemId")]
        public string ItemId { get; set; }

        /// <summary>
        /// 商品链接
        /// </summary>
        [JsonProperty("actionUrl")]
        public string ActionUrl { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        [JsonProperty("pic")]
        public string Pic { get; set; }

        /// <summary>
        /// 月销量
        /// </summary>
        [JsonProperty("monthlySoldQuantity")]

        public string MmonthlySoldQuantity { get; set; }
    }
}
