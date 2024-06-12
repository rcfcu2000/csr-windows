using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.BaseModels.BackEnd
{
    public class SSOLoginUserModel
    {
        /// <summary>
		/// 
		/// </summary>
		public int ID { get; set; }

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
        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("userName")]
        public string UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("nickName")]
        public string NickName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("sideMode")]
        public string SideMode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("headerImg")]
        public string HeaderImg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("baseColor")]
        public string BaseColor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("activeColor")]
        public string ActiveColor { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("authorityId")]
        public int AuthorityId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("authority")]
        public SSOLoginAuthorityModel Authority { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("authorities")]
        public List<SSOLoginAuthorityModel> Authorities { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("phone")]
        public string Phone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("enable")]
        public int Enable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("salesRepType")]
        public int SalesRepType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("shopId")]
        public int ShopId { get; set; }

    }
}
