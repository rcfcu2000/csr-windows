using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.WebSocketModels
{
    [Serializable]
    public class ChatApiParam
    {
        /// <summary>
		/// tb358660788
		/// </summary>
		[JsonProperty("userid")]
        public string Userid { get; set; }

        /// <summary>
        /// 亲亲您好呀！感谢您对我们蜡笔派家居旗舰店的关注和支持哦~ 您提到的产品，我们非常乐意为您详细介绍。我们的桌布、桌旗、椅垫等产品都是采用高品质材料，经过精心设计和制作，旨在为您的家居环境增添时尚与舒适。每一件产品都可以根据您的需求进行独家定制，确保完美融入您的家居风格。此外，我们目前还有优惠活动，购买非常划算哦！如果您对产品质量或任何其他方面有疑问，或者需要我为您推荐适合的产品，请随时告诉我，我会尽全力为您提供满意的解答和服务。期待您的光临，祝您生活愉快！
        /// </summary>
        [JsonProperty("msg")]
        public string Msg { get; set; }
    }
}
