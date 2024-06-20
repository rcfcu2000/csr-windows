using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.AIChat
{
    [Serializable]
    public class AutoReplayModel
    {
        /// <summary>
        /// 根据目前匹配的商品名字作为参数，如果没有则返回None
        /// </summary>
        [JsonProperty("goods_name")]
        public string GoodsName { get; set; }

        /// <summary>
        /// 成功匹配到正则的顾客消息（原始消息）举例：亲，这款还有优惠吗 回头客了
        /// </summary>
        [JsonProperty("question")]
        public string Question { get; set; }

        /// <summary>
        /// 顾客消息所属问题对应的参考答案  举例：1元锁定618优惠，享6大权益
        /// </summary>
        [JsonProperty("answer")]
        public string Answer { get; set; }
    }
}
