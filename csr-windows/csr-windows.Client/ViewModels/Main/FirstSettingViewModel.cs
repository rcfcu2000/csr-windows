using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using csr_windows.Client.Services.Base;
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
        private string _userName = "小玲";

        private string _storeName = "蜡笔派家居旗舰店";



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

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        /// <summary> 
        /// 店铺名称
        /// </summary>
        public string StoreName
        {
            get => _storeName;
            set => SetProperty(ref _storeName, value);
        }
        #endregion

        #region Methods
        /// <summary>
        /// 确认
        /// </summary>
        private void OnConfirmCommand()
        {
           _uiService.OpenWelcomeView();
        }
        #endregion
    }
}
