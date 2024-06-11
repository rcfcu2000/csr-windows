using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Core;
using csr_windows.Domain;
using csr_windows.Domain.Api;
using csr_windows.Domain.WeakReferenceMessengerModels;
using csr_windows.Resources.Enumeration;
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
            CloseCommand = new RelayCommand(async () =>
            {
                int saleType = (int)(GlobalCache.IsItPreSalesCustomerService ? SalesRepType.PreSale : SalesRepType.AfterSale);
                //调用接口
                Dictionary<string, dynamic> keyValuePairs = new Dictionary<string, dynamic>()
                {
                    { "salesRepType",saleType }
                };

                WeakReferenceMessenger.Default.Send(string.Empty, MessengerConstMessage.ShowLoadingVisibilityChangeToken);
                string content = await ApiClient.Instance.PutAsync(BackEndApiList.SetSelfInfo, keyValuePairs);
                if (content == string.Empty)
                {
                    return;
                }
                WeakReferenceMessenger.Default.Send(string.Empty, MessengerConstMessage.HiddenLoadingVisibilityChangeToken);

                string promptString = GlobalCache.IsItPreSalesCustomerService ? "已切换至售前客服" : "已切换至售后客服";
                WeakReferenceMessenger.Default.Send(new PromptMessageTokenModel($"{promptString}"), MessengerConstMessage.OpenPromptMessageToken);

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
