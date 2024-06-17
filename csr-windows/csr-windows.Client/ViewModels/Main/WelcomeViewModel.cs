using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Services.Base;
using csr_windows.Client.Services.WebService;
using csr_windows.Client.Services.WebService.Enums;
using csr_windows.Common.Helper;
using csr_windows.Core;
using csr_windows.Core.RequestService;
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
            FollowWindowHelper.RunQNProcessAtVersion(9, 12);
            bool isRunning = true;

            if (isRunning)
            {
                //有千牛的客服窗口 但是没有websocket连接
                if (GlobalCache.IsFollowWindow)
                {
                    if ((WebServiceClient.Socket == null) || !WebServiceClient.Socket.IsAvailable)
                    {
                        
                        WeakReferenceMessenger.Default.Send(new PromptMessageTokenModel("出了点问题，请重新打开千牛接待中心", promptEnum: PromptEnum.Note), MessengerConstMessage.OpenPromptMessageToken);
                        TopHelp.QNCloseWindow();
                        return;
                    }
                }
                if (!GlobalCache.HaveStoreName)//启动了千牛，但是没打开千牛客服界面,就打开启动千牛未启动界面
                {
                    _uiService.OpenNoStartClientView();
                    return;
                }


                //调用接口
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>()
                {
                    { "name",$"{GlobalCache.CustomerServiceNickName}" }
                };
                WeakReferenceMessenger.Default.Send(string.Empty,MessengerConstMessage.ShowLoadingVisibilityChangeToken);
                string content = await ApiClient.Instance.PostAsync(BackEndApiList.GerUserInfo, keyValuePairs);
                if (content == string.Empty)
                {
                    return;
                }
                BackendBase<object> model = JsonConvert.DeserializeObject<BackendBase<object>>(content);
                isFirstIn = string.IsNullOrEmpty(content) ? false : model.Code != 0;

                //第二个判断是否是第一次进入
                LoginServer.Instance.Login();

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
            else
            {
                _uiService.OpenNoStartClientView();
            }
        }
        #endregion
    }
}
