using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Domain;
using csr_windows.Domain.Api;
using csr_windows.Domain.BaseModels.BackEnd.Base;
using csr_windows.Domain.BaseModels.BackEnd;
using csr_windows.Domain.WeakReferenceMessengerModels;
using csr_windows.Resources.Enumeration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using csr_windows.Domain.BaseModels;
using csr_windows.Domain.Common;

namespace csr_windows.Core.RequestService
{
    public class LoginServer
    {
        private static readonly Lazy<LoginServer> instance = new Lazy<LoginServer>(() => new LoginServer());
        public static LoginServer Instance => instance.Value;
        public LoginServer()
        {
            
        }
        public async void Login()
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>()
            {
                {"password","123456" },
                {"ssoUsername",$"{GlobalCache.CustomerServiceNickName}" },
                {"username","admin"}
            };

            WeakReferenceMessenger.Default.Send(string.Empty, MessengerConstMessage.ShowLoadingVisibilityChangeToken);
            string content = await ApiClient.Instance.PostAsync(BackEndApiList.SSOLogin, keyValuePairs);
            if (content == string.Empty)
            {
                return;
            }
            WeakReferenceMessenger.Default.Send(string.Empty, MessengerConstMessage.HiddenLoadingVisibilityChangeToken);

            BackendBase<SSOLoginModel> loginModel = JsonConvert.DeserializeObject<BackendBase<SSOLoginModel>>(content);
            if (loginModel.Data.User.Enable != SSOLoginUserModel.EnableTrue)
            {
                WeakReferenceMessenger.Default.Send(new PromptMessageTokenModel("您的账号已被管理员停用", promptEnum: PromptEnum.Note), MessengerConstMessage.OpenPromptMessageToken);
                return;
            }

            if (loginModel.Code == BankendBaseCodeEnum.Success)
            {
                Task.Delay(500).Wait();
                //这里空指针了？为什么？加个延迟试试
                GlobalCache.StoreSSOLoginModel[GlobalCache.StoreName] = loginModel.Data;
                ApiClient.Instance.SetToken(loginModel.Data.Token);
                GlobalCache.IsItPreSalesCustomerService = loginModel.Data.User.SalesRepType == (int)SalesRepType.PreSale;
            }

            // Get Shop Info
            content = await ApiClient.Instance.GetAsync(BackEndApiList.GetShopInfo + '/' + loginModel.Data.User.ShopId);
            if (content == string.Empty)
            {
                return;
            }
            ShopModel shopModel = JsonConvert.DeserializeObject<ShopModel>(content);
            GlobalCache.StoreShop[GlobalCache.StoreName] = shopModel;
            GlobalCache.Shop = shopModel;



            keyValuePairs = new Dictionary<string, object>()
            {
                { "page",1 },
                { "pageSize",1000 },
                { "shopId",loginModel.Data.User.ShopId }
            };
            // Get All Merchant
            content = await ApiClient.Instance.PostAsync(BackEndApiList.GetMerchantList, keyValuePairs);


            var baseModel = JsonConvert.DeserializeObject<BackendBase<BaseGetMerchantList>>(content);
            if (baseModel.Code == BankendBaseCodeEnum.Success)
            {
                //转化为
                List<MyProduct> myProducts = new List<MyProduct>();
                foreach (var item in baseModel.Data.List)
                {
                    myProducts.Add(new MyProduct()
                    {
                        MerchantId = item.MerchantId,
                        ProductID = item.MerchantLinks.Count > 0 ? item.MerchantLinks[0].TaobaoId.ToString() : "",
                        ProductImage = string.IsNullOrEmpty(item.PictureLink) ? "" : item.PictureLink,
                        ProductName = item.Alias,
                        ProductInfo = item.Info,
                        ProductUrl = $"https://detail.tmall.com/item.htm?id={(item.MerchantLinks.Count > 0 ? item.MerchantLinks[0].TaobaoId : 0)}"
                    });
                }
                GlobalCache.AllProducts = myProducts;
                if (!GlobalCache.StoreAllProducts.ContainsKey(GlobalCache.StoreName))
                {
                    GlobalCache.StoreAllProducts[GlobalCache.StoreName] = myProducts;
                }
            }


        }
    }
}
