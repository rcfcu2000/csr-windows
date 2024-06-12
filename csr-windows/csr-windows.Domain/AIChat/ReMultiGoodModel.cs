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
    public class ReMultiGoodModel
    {

        /// <summary>
        /// 店铺的id
        /// </summary>
        [JsonProperty("shop_id")]
        public int ShopId { get; set; }

        /// <summary>
        /// 店铺信息
        /// </summary>
        [JsonProperty("brand_info")]
        public string BrandInfo { get; set; }

        /// <summary>
        /// 店铺名
        /// </summary>
        [JsonProperty("shop_name")]
        public string ShopName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("assistant_name")]
        public string AssistantName { get; set; }

        /// <summary>
        /// 地中海桌布
        /// </summary>
        [JsonProperty("good_a_name")]
        public string GoodAName { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("message_history")]
        public JArray MessageHistory { get; set; }


        /// <summary>
        /// 客户已经购买的商品知识库
        /// </summary>
        [JsonProperty("good_a_name_knowledge")]
        public string GoodANameKnowledge { get; set; }

        /// <summary>
        /// 推荐商品的列表以其知识库，格式为：{“商品名”：“对应的知识库”，}
        /// </summary>
        [JsonProperty("goods_list_knowledge")]
        public Dictionary<string, string> GoodsListKnowledge { get; set; }

        /// <summary>
        /// 指导客户购买的场景：商品正在打折，客户场景等
        /// </summary>
        [JsonProperty("customer_scene")]
        public string CustomerScene { get; set; }

        /// <summary>
        /// 店铺类目
        /// </summary>
        [JsonProperty("industry_category")]
        public string IndustryCategory { get; set; }

        /// <summary>
        /// 切换人设（模型），只能是三个参数（“deep_seek”or “glm_air”or”glm_flash””）
        /// </summary>
        [JsonProperty("persona")]
        public string Persona { get; set; }
    }
}
