using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Client.ViewModels.Customer
{
    public class CustomerViewModel: ObservableRecipient
    {

        #region Fields
        private string _storeName = "蜡笔派家居旗舰店";
        private string _userName = "小玲";

        private string _customerNickname = "章盼angela";

    



        #endregion

        #region Commands

        #endregion

        #region Constructor
        public CustomerViewModel()
        {
            
        }
        #endregion

        #region Properties

        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName
        {
            get => _storeName;
            set => SetProperty(ref _storeName, value);
        }
        
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        /// <summary>
        /// 顾客昵称
        /// </summary>
        public string CustomerNickname
        {
            get => _customerNickname;
            set => SetProperty(ref _customerNickname, value);
        }

        #endregion

        #region Methods

        #endregion


    }
}
