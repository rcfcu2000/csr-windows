using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Services.Base;
using csr_windows.Client.Services.WebService;
using csr_windows.Client.Services.WebService.Enums;
using csr_windows.Common.Helper;
using csr_windows.Core;
using csr_windows.Domain;
using csr_windows.Domain.Api;
using csr_windows.Domain.BaseModels.BackEnd;
using csr_windows.Domain.BaseModels.BackEnd.Base;
using csr_windows.Resources.Enumeration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Client.ViewModels.Main
{
    public class WelcomeViewModel : ObservableRecipient
    {
        #region Fields
        private IUiService _uiService;
        private string _version = "1.0";

        #endregion

        #region Commands
        public IRelayCommand UseCommand { get; set; }
        #endregion

        #region Constructor
        public WelcomeViewModel()
        {
            _uiService = Ioc.Default.GetService<IUiService>();
            UseCommand = new RelayCommand(OnUseCommand);
        }



        #endregion

        #region Properties

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
        }

        #endregion

        #region Methods
        private bool isFirstIn;
        private async void OnUseCommand()
        {
            //这里首先有三个判断

            //第一个判断是否启动了千牛
            bool isRunning = FollowWindowHelper.IsProcessRunning(FollowWindowHelper.ProcessName);

            if (isRunning)
            {
                //还没有获取千牛的时候怎么处理？
                //kill接待中心，重新打开
                if (!GlobalCache.HaveStoreName)
                {
                    _uiService.OpenNoStartClientView();
                    return;
                }

                //调用接口
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>()
                {
                    { "name",$"{GlobalCache.CustomerServiceNickName}" }
                };

                string content = await ApiClient.Instance.PostAsync(BackEndApiList.GerUserInfo, keyValuePairs);
                if (content == string.Empty)
                {
                    return;
                }
                BackendBase<object> model = JsonConvert.DeserializeObject<BackendBase<object>>(content);
                isFirstIn = string.IsNullOrEmpty(content) ? false : model.Code != 0;
                //todo:这里可能会 请求错误
                //第二个判断是否是第一次进入
                if (isFirstIn)
                {
                    keyValuePairs = new Dictionary<string, string>()
                    {
                        {"password","123456" },
                        {"ssoUsername",$"{GlobalCache.CustomerServiceNickName}" },
                        {"username","admin"}
                    };
                    content = await ApiClient.Instance.PostAsync(BackEndApiList.SSOLogin, keyValuePairs);
                    if (content == string.Empty)
                    {
                        return;
                    }
                    BackendBase<SSOLoginModel> loginModel = JsonConvert.DeserializeObject<BackendBase<SSOLoginModel>>(content);
                    if (model.Code == 0)
                    {
                        ApiClient.Instance.SetToken(loginModel.Data.Token);
                        GlobalCache.IsItPreSalesCustomerService = loginModel.Data.User.SalesRepType == (int)SalesRepType.PreSale;
                    }
                    _uiService.OpenFirstSettingView();
                }
                else
                {

                    //然后最后进入聊天界面
                    WeakReferenceMessenger.Default.Send("", MessengerConstMessage.LoginSuccessToken);
                    _uiService.OpenCustomerView();
                }
            }
            else
            {
                _uiService.OpenNoStartClientView();
            }
        }
        #endregion
    }
}
