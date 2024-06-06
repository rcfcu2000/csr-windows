using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Services.Base;
using csr_windows.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Client.ViewModels.Main
{
    public class FirstSettingViewModel : ObservableValidator
    {
        #region Fields
        private IUiService _uiService;



        #endregion

        #region Commons
        public IRelayCommand ConfirmCommand{ get; set; }



        #endregion

        #region Constructor
        public FirstSettingViewModel()
        {
            _uiService = Ioc.Default.GetService<IUiService>();
            PropertyChanged += (s, e) =>
            {
                //通知命令能否执行
                ConfirmCommand?.NotifyCanExecuteChanged();
            };
            ConfirmCommand = new RelayCommand(OnConfirmCommand);
        }



        #endregion

        #region Properties
        #endregion

        #region Methods
        /// <summary>
        /// 确认
        /// </summary>
        private void OnConfirmCommand()
        {
            //然后最后进入聊天界面
            WeakReferenceMessenger.Default.Send("", MessengerConstMessage.LoginSuccessToken);
            _uiService.OpenCustomerView();
        }
        #endregion
    }
}
