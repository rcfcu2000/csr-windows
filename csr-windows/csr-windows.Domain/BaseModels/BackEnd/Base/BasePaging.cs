using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace csr_windows.Domain.BaseModels.BackEnd.Base
{
    public class BasePaging<T>
    {
        /// <summary>
		/// 
		/// </summary>
		[JsonProperty("list")]
        public List<T> List { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("total")]
        public int Total { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("page")]
        public int Page { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
    }
}
