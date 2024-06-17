using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.BaseModels.BackEnd
{
    public class MerchantLink
    {
        /// <summary>
		/// 
		/// </summary>
		public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DeletedAt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("linkId")]
        public int LinkId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("taobaoId")]
        public long TaobaoId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("updatedBy")]
        public string UpdatedBy { get; set; }
    }
}
