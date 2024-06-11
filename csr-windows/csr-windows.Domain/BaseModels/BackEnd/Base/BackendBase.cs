using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace csr_windows.Domain.BaseModels.BackEnd.Base
{
    public class BackendBase<T>
    {
        /// <summary>
		/// 
		/// </summary>
		[JsonProperty("code")]
        public int Code { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        [JsonProperty("data")]
        public T Data { get; set; }

        /// <summary>
        /// 获取成功
        /// </summary>
        [JsonProperty("msg")]
        public string Msg { get; set; }
    }
}
