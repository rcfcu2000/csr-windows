using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace csr_windows.Domain.BaseModels
{
    [Serializable]
    public class MJSResult<T>
    {
        /// <summary>
		/// 
		/// </summary>
		[JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("msg")]
        public T Msg { get; set; }
    }
}
