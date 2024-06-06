using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.BaseModels
{
    [Serializable]
    public class MChatApiResult<T>
    {
        /// <summary>
		/// sendMsg
		/// </summary>
		[JsonProperty("act")]
        public string Act { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("param")]
        public T Param { get; set; }
    }
}
