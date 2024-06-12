using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.BaseModels.BackEnd
{
    public class SSOLoginAuthorityModel
    {
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
        [JsonProperty("authorityId")]
        public int AuthorityId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("authorityName")]
        public string AuthorityName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("defaultRouter")]
        public string DefaultRouter { get; set; }
    }
}
