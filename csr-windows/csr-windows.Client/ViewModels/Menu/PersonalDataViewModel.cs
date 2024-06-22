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

        private bool _isItPreSalesCustomerService;

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
            IsItPreSalesCustomerService = GlobalCache.IsItPreSalesCustomerService;

            CloseCommand = new RelayCommand(async () =>
            {
                //如果不一样 再弹提示
                if (IsItPreSalesCustomerService != GlobalCache.IsItPreSalesCustomerService)
                {
                    int saleType = (int)(IsItPreSalesCustomerService ? SalesRepType.PreSale : SalesRepType.AfterSale);
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

              
                    GlobalCache.IsItPreSalesCustomerService = IsItPreSalesCustomerService;
                    string promptString = GlobalCache.IsItPreSalesCustomerService ? "已切换至售前客服" : "已切换至售后客服";
                    WeakReferenceMessenger.Default.Send(new PromptMessageTokenModel($"{promptString}"), MessengerConstMessage.OpenPromptMessageToken);
                }

                WeakReferenceMessenger.Default.Send("", MessengerConstMessage.CloseMenuUserControlToken);
            });
        }
        #endregion

        #region Properties

        /// <summary>
        /// 是否是售前
        /// </summary>
        public bool IsItPreSalesCustomerService
        {
            get => _isItPreSalesCustomerService;
            set => SetProperty(ref _isItPreSalesCustomerService, value);
        }
        #endregion

        #region Methods

        #endregion

    }
}
