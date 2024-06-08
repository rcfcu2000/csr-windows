using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.BaseModels.BackEnd.Base
{
    [Serializable]
    public class BaseGetMerchantByTidModel
    {
        /// <summary>
		/// 
		/// </summary>
		[JsonProperty("data")]
        public List<GetMerchantByTidModel> Data { get; set; }
    }
}
