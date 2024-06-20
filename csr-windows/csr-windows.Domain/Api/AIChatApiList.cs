using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.Api
{
    public static class AIChatApiList
    {
        /// <summary>
        /// 我该怎么回
        /// </summary>
        public const string How2Replay = "/how_2_reply";

        /// <summary>
        /// 我想这样回
        /// </summary>
        public const string Want2Reply = "/want_2_reply";

        /// <summary>
        /// 推荐商品
        /// </summary>
        public const string ReSingleGood = "/re_single_good";

        /// <summary>
        /// 推荐搭配
        /// </summary>
        public const string ReMultiGood = "/re_multi_good";

        /// <summary>
        /// 正则匹配自动回复
        /// </summary>
        public const string AutoReplay = "/auto_reply";
    }
}
