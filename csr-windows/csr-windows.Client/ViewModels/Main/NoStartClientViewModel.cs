using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Services.Base;
using csr_windows.Client.Services.WebService;
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
            FollowWindowHelper.RunQNProcessAtVersion(9, 12);
            
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
                    WeakReferenceMessenger.Default.Send(new PromptMessageTokenModel("请重新打开千牛接待中心", promptEnum: PromptEnum.Note), MessengerConstMessage.OpenPromptMessageToken);

                    TopHelp.QNCloseWindow();
                    return;
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
                LoginServer.Instance.Login();
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
