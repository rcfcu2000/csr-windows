using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private string _userName;

        private string _storeName;

        private bool _isItPreSalesCustomerService = true;


        #endregion

        #region Commons
        public IRelayCommand ConfirmCommand{ get; set; }



        #endregion

        #region Constructor
        public FirstSettingViewModel()
        {
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

        /// <summary>
        /// 是否售前客服
        /// </summary>
        public bool IsItPreSalesCustomerService
        {
            get => _isItPreSalesCustomerService;
            set => SetProperty(ref _isItPreSalesCustomerService, value);
        }
        #endregion

        #region Methods
        /// <summary>
        /// 确认
        /// </summary>
        private void OnConfirmCommand()
        {
           
        }
        #endregion
    }
}
