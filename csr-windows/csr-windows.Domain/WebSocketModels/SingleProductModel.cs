using csr_windows.Domain.BaseModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.WebSocketModels
{
    [Serializable]
    public class SingleProductModel : MBaseProduct
    {
        /// <summary>
		/// 当前用户来自 淘宝移动端 宝贝详情页 
		/// </summary>
		[JsonProperty("E0_text")]
        public string Text { get; set; }

        /// <summary>
        /// 蜡笔派莉吉亚桌布立体提花防水纯色简约氛围感长方形餐桌盖布定制
        /// </summary>
        [JsonProperty("E1_title")]
        public string Title { get; set; }

        /// <summary>
        /// 92.00
        /// </summary>
        [JsonProperty("E1_price")]
        public string Price { get; set; }

        /// <summary>
        /// https://item.taobao.com/item.htm?id=714162914700&scm=20140619.rec.2995099000.714162914700
        /// </summary>
        [JsonProperty("E1_actionUrl")]
        public string E1ActionUrl { get; set; }

        [JsonProperty("actionUrl")]
        public string ActionUrl { get; set; }


        /// <summary>
        /// https://img.alicdn.com/bao/uploaded/i3/2995099000/O1CN01qTKf5F2GM3YPIFeYz_!!2995099000.jpg
        /// </summary>
        [JsonProperty("E1_pic")]
        public string Pic { get; set; }


        

    }
}
