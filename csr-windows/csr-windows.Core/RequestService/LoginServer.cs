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
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>()
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

            if (loginModel.Code == 0)
            {
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
        }
    }
}
