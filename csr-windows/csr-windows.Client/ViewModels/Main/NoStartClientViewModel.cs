using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Services.Base;
using csr_windows.Client.Services.WebService;
using csr_windows.Common.Helper;
using csr_windows.Core;
using csr_windows.Domain;
using csr_windows.Domain.Api;
using csr_windows.Domain.BaseModels.BackEnd;
using csr_windows.Domain.BaseModels.BackEnd.Base;
using csr_windows.Domain.WeakReferenceMessengerModels;
using csr_windows.Resources.Enumeration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Client.ViewModels.Main
{
    public class NoStartClientViewModel : ObservableRecipient
    {
        #region Fields
        private IUiService _uiService;
        #endregion

        #region Commands
        public IRelayCommand StartClientCommand { get; set; }
        #endregion

        #region Constructor
        public NoStartClientViewModel()
        {
            _uiService = Ioc.Default.GetService<IUiService>();
            StartClientCommand = new RelayCommand(OnStartClientCommand);
        }


        #endregion

        #region Properties
        #endregion

        #region Methods
        private bool isFirstIn;
        /// <summary>
        /// 启动千牛客户端
        /// </summary>
        private async void OnStartClientCommand()
        {
            //第一个判断是否启动了千牛
            bool isRunning = FollowWindowHelper.IsProcessRunning(FollowWindowHelper.ProcessName);
            if (!isRunning)
            {
                //打开千牛
                string programDisplayName = "千牛工作台";

                string programPath = FollowWindowHelper.FindProgramPath(programDisplayName);

                if (!string.IsNullOrEmpty(programPath))
                {
                    try
                    {
                        Process process = Process.Start(programPath + "\\AliWorkbench.exe");
                        Console.WriteLine($"{programDisplayName} started successfully from: {programPath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to start {programDisplayName}: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"Program {programDisplayName} not found in the registry.");
                }
                return;
            }
            //启动了千牛判断是否启动了窗口
            else
            {
                //没启动接待窗口就去启动接待窗口
                if (!GlobalCache.IsFollowWindow)
                {
                    WeakReferenceMessenger.Default.Send(new PromptMessageTokenModel("请打开千牛接待中心", promptEnum: PromptEnum.Note), MessengerConstMessage.OpenPromptMessageToken);
                    return;
                }

                //有千牛的客服窗口 但是没有websocket连接
                if (WebServiceClient.Socket == null || !WebServiceClient.Socket.IsAvailable || !GlobalCache.HaveStoreName)
                {
                    WeakReferenceMessenger.Default.Send(new PromptMessageTokenModel("出了点问题，请重启千牛客户端", promptEnum: PromptEnum.Note), MessengerConstMessage.OpenPromptMessageToken);
                }
            }
           



            //调用接口
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>()
            {
                { "name",$"{GlobalCache.CustomerServiceNickName}" }
            };
            WeakReferenceMessenger.Default.Send(string.Empty, MessengerConstMessage.ShowLoadingVisibilityChangeToken);
            string content = await ApiClient.Instance.PostAsync(BackEndApiList.GerUserInfo, keyValuePairs);
            if (content == string.Empty)
            {
                return;
            }
            BackendBase<object> model = JsonConvert.DeserializeObject<BackendBase<object>>(content);
            isFirstIn = string.IsNullOrEmpty(content) ? false : model.Code != 0;
            WeakReferenceMessenger.Default.Send(string.Empty, MessengerConstMessage.HiddenLoadingVisibilityChangeToken);

            if (GlobalCache.HaveStoreName)
            {
                keyValuePairs = new Dictionary<string, string>()
                    {
                        {"password","123456" },
                        {"ssoUsername",$"{GlobalCache.CustomerServiceNickName}" },
                        {"username","admin"}
                    };

                WeakReferenceMessenger.Default.Send(string.Empty, MessengerConstMessage.ShowLoadingVisibilityChangeToken);
                content = await ApiClient.Instance.PostAsync(BackEndApiList.SSOLogin, keyValuePairs);
                if (content == string.Empty)
                {
                    return;
                }
                WeakReferenceMessenger.Default.Send(string.Empty, MessengerConstMessage.HiddenLoadingVisibilityChangeToken);

                BackendBase<SSOLoginModel> loginModel = JsonConvert.DeserializeObject<BackendBase<SSOLoginModel>>(content);
                if (loginModel.Code == 0)
                {
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
                GlobalCache.shop = shopModel;
            }

            if (isFirstIn)
            {
                
                _uiService.OpenFirstSettingView();
            }
            else
            {
                //然后最后进入聊天界面
                WeakReferenceMessenger.Default.Send("", MessengerConstMessage.LoginSuccessToken);
                _uiService.OpenCustomerView();
            }
        }
        #endregion
    }
}
