using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.WebSocketModels
{
    [Serializable]
    public class Single101ProductModel
    {
        /// <summary>
		/// 蜡笔派「国画花鸟系列」法式中国风高级氛围感客厅沙发靠垫枕定制
		/// </summary>
		[JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("sales")]
        public int Sales { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("shopName")]
        public string ShopName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("price")]
        public string Price { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("originalPrice")]
        public string OriginalPrice { get; set; }

        [JsonProperty("actionUrl")]
        public string ActrionUrl { get; set; }
    }
}
