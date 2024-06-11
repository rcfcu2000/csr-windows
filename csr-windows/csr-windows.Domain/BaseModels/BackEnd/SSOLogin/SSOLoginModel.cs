using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.BaseModels.BackEnd
{
    public class SSOLoginModel
    {
        /// <summary>
		/// 
		/// </summary>
		[JsonProperty("user")]
        public SSOLoginUserModel User { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("expiresAt")]
        public long ExpiresAt { get; set; }
    }
}
