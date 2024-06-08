using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.BaseModels
{
    public class MBaseProduct
    {

        public string TaoBaoID{ get; set; }

        /// <summary>
        /// 发送人的标识
        /// </summary>
        public string SendUserNiceName { get; set; }

        /// <summary>
        /// 接收者的标识
        /// </summary>
        public string ReceiveUserNiceName { get; set; }
    }
}
