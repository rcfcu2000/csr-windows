using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.AIChat
{
    [Serializable]
    public class How2ReplyModel
    {
        /// <summary>
        /// 店铺名
        /// </summary>
        [JsonProperty("shop_name")]
        public string ShopName { get; set; }

        /// <summary>
        /// 后台配置的客服的昵称
        /// </summary>
        [JsonProperty("assistant_name")]
        public string AssistantName { get; set; }

        /// <summary>
        /// 多轮对话数据，对话内容有30条
        /// </summary>
        [JsonProperty("message_history")]
        public JArray MessageHistory { get; set; }

        /// <summary>
        /// 匹配的商品名字
        /// </summary>
        [JsonProperty("goods_name")]
        public string GoodsName { get; set; }

        /// <summary>
        /// 商品的商品知识
        /// </summary>
        [JsonProperty("goods_knowledge")]
        public string GoodsKnowledge { get; set; }

        /// <summary>
        /// 版本目前分为售前版和售后版两个版本，故这个参数只返回”sale_pre”售前和”sale_post”售后
        /// </summary>
        [JsonProperty("sale_mode")]
        public string SaleMode { get; set; }

        /// <summary>
        /// “deep_seek”or “glm_air”or”glm_flash”
        /// </summary>
        [JsonProperty("persona")]
        public string Persona { get; set; }
    }
}
