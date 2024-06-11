using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Services.Base;
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

            //todo：去判断



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


            if (isFirstIn)
            {
                if (GlobalCache.HaveCustomer)
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
                    if (loginModel.Code == 0)
                    {
                        ApiClient.Instance.SetToken(loginModel.Data.Token);
                        GlobalCache.IsItPreSalesCustomerService = loginModel.Data.User.SalesRepType == (int)SalesRepType.PreSale;
                    }
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
        #endregion
    }
}
