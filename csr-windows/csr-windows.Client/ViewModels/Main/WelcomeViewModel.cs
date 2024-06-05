using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Services.Base;
using csr_windows.Common.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private void OnUseCommand()
        {
            //这里首先有三个判断

            //第一个判断是否启动了千牛
            bool isRunning = FollowWindowHelper.IsProcessRunning(FollowWindowHelper.ProcessName);

            if (isRunning)
            {
                //第二个判断是否是第一次进入
                _uiService.OpenFirstSettingView();
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
