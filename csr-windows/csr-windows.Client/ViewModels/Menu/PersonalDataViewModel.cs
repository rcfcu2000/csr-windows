using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace csr_windows.Client.ViewModels.Menu
{
    public class PersonalDataViewModel : ObservableRecipient
    {
        #region Fields
        private string _userName = "小玲";

        private string _storeName = "蜡笔派家居旗舰店";

        #endregion

        #region Commands
        /// <summary>
        /// 关闭
        /// </summary>
        public ICommand CloseCommand { get; set; }
        #endregion

        #region Constructor
        public PersonalDataViewModel()
        {
            CloseCommand = new RelayCommand(() =>
            {
                WeakReferenceMessenger.Default.Send("", MessengerConstMessage.CloseMenuUserControlToken);
            });
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

        #endregion

    }
}
