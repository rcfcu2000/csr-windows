using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.BaseModels.BackEnd
{
    [Serializable]
    public class GetMerchantByTidModel
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("merchantId")]
        public int MerchantId { get; set; }

        /// <summary>
        /// 初荷抱枕
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 初荷抱枕(简写 用这个字段)
        /// </summary>
        [JsonProperty("alias")]
        public string Alias { get; set; }

        /// <summary>
        /// 材质：特定北欧貂绒底布
		/// </summary>
		[JsonProperty("info")]
        public string Info { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("pictureLink")]
        public string PictureLink { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("merchantLinks")]
        public List<MerchantLink> MerchantLinks { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("updatedBy")]
        public string UpdatedBy { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string DeletedAt { get; set; }
    }
}
