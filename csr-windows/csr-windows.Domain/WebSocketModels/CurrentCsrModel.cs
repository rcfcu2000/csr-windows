using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.WebSocketModels
{
    [Serializable]
    public class CurrentCsrModel
    {
        /// <summary>
		/// 
		/// </summary>
		[JsonProperty("targetType")]
        public string TargetType { get; set; }

        /// <summary>
        /// 蜡笔派家居旗舰店:华
        /// </summary>
        [JsonProperty("nick")]
        public string Nick { get; set; }

        /// <summary>
        /// 蜡笔派家居旗舰店:华
        /// </summary>
        [JsonProperty("display")]
        public string Display { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("portrait")]
        public string Portrait { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("appkey")]
        public string Appkey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("targetId")]
        public string TargetId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("havMainId")]
        public string HavMainId { get; set; }
    }
}
