using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.Api
{
    public class BackEndApiList
    {
        /// <summary>
        /// 根据淘宝ID获取商品卡
        /// </summary>
        public const string GetMerchantByTid = "/merchant/getByTid";

        /// <summary>
        /// 获取用户信息
        /// </summary>
        public const string GerUserInfo = "/base/userinfo";

        /// <summary>
        /// SSO登录注册
        /// </summary>
        public const string SSOLogin = "/base/ssoLogin";

        /// <summary>
        /// 设置用户信息
        /// </summary>
        public const string SetSelfInfo = "/base/setSelfInfo";

        /// <summary>
        /// 获取店铺信息
        /// </summary>
        public const string GetShopInfo = "/shop/getbyid";

        /// <summary>
        /// 获取商品列表
        /// </summary>
        public const string GetMerchantList = "/merchant/getMerchantList";
    }
}
