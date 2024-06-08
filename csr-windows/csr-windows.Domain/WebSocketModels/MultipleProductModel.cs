using csr_windows.Domain.BaseModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.WebSocketModels
{
    public class MultipleProductModel : MBaseProduct
    {
        /// <summary>
		/// 719653832977
		/// </summary>
		[JsonProperty("itemId")]
        public string ItemId { get; set; }

        /// <summary>
        /// 林中鹿--桌旗;35*250cm
        /// </summary>
        [JsonProperty("subTitle")]
        public string SubTitle { get; set; }

        /// <summary>
        /// 241.68
        /// </summary>
        [JsonProperty("price")]
        public string Price { get; set; }

        /// <summary>
        /// https://h5.m.taobao.com/awp/core/detail.htm?id=719653832977&skuId=5045291612764
        /// </summary>
        [JsonProperty("actionUrl")]
        public string ActionUrl { get; set; }

        /// <summary>
        /// //img.alicdn.com/bao/uploaded/i3/2995099000/O1CN01rKZkaI2GM3RolnDfn_!!2995099000.jpg
        /// </summary>
        [JsonProperty("pic")]
        public string Pic { get; set; }

        /// <summary>
        /// 蜡笔派林中鹿桌旗法式黑色高级流苏轻奢简约茶几台布床旗盖布定制
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
