using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.BaseModels.BackEnd.QA
{
    public class BackendAutoReplyModel
    {
        /// <summary>
		/// 
		/// </summary>
		public int ID { get; set; }

        /// <summary>
        /// 开始语（如你好、在吗、人呢等）
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// 你好
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RegEx { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("shopId")]
        public int ShopId { get; set; }
    }
}
