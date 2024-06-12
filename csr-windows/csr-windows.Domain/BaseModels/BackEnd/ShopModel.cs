using Newtonsoft.Json;

namespace csr_windows.Domain.BaseModels.BackEnd
{
    public class ShopModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 蜡笔派家居旗舰店
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 蜡笔派
        /// </summary>
        [JsonProperty("nickName")]
        public string NickName { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("category")]
        public Category Category { get; set; }

        /// <summary>
        /// 布艺,服饰
        /// </summary>
        [JsonProperty("brandManagement")]
        public string BrandManagement { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("brandBelief")]
        public string BrandBelief { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("brandAdvantage")]
        public string BrandAdvantage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("brandInfo")]
        public string BrandInfo { get; set; }
    }
}