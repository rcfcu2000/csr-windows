using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.BaseModels.BackEnd.QA
{
    public class QaTypesItem
    {
        /// <summary>
		/// 
		/// </summary>
		public int ID { get; set; }

        /// <summary>
        /// 快递相关
        /// </summary>
        public string QType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int KbType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int RefCount { get; set; }
    }
}
