using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.WebSocketModels
{
    [Serializable]
    public class ProductModel
    {
        /// <summary>
		/// 商品ID
		/// </summary>
		[JsonProperty("itemId")]
        public string ItemId { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        [JsonProperty("subTitle")]
        public string SubTitle { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        [JsonProperty("price")]
        public string Price { get; set; }

        /// <summary>
        /// 商品Url
        /// </summary>
        [JsonProperty("actionUrl")]
        public string ActionUrl { get; set; }

        /// <summary>
        /// 图片地址 需要加https
        /// </summary>
        [JsonProperty("pic")]
        public string Pic { get; set; }

        /// <summary>
        /// 标题
        /// 示例：蜡笔派莉吉亚桌布立体提花防水纯色简约氛围感长方形餐桌盖布定制
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
