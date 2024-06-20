using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.BaseModels.BackEnd.QA
{
    public class QAModel
    {
        /// <summary>
		/// 
		/// </summary>
		public int ID { get; set; }

        /// <summary>
        /// 快递公司
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// 只发顺丰
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// (常规|普通|什么|啥|发|哪|那).{0,5}(快递|物流)|^快递$|快递.{0,5}(是什么|哪家|是啥)|物流公司|顺(丰|风)|中通|圆通|安能|韵达|邮政|德邦|申通|京东|^物流$|(走|这个)(物流|快递)
        /// </summary>
        public string RegEx { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Enable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int KbType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("qa_types")]
        public List<QaTypesItem> QaTypes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("shopId")]
        public int ShopId { get; set; }
    }
}
