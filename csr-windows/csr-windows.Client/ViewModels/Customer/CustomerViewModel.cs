using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Services.Base;
using csr_windows.Client.Services.WebService;
using csr_windows.Client.Services.WebService.Enums;
using csr_windows.Client.View.Chat;
using csr_windows.Client.ViewModels.Chat;
using csr_windows.Client.Views.Chat;
using csr_windows.Common;
using csr_windows.Core;
using csr_windows.Domain;
using csr_windows.Domain.Api;
using csr_windows.Domain.BaseModels;
using csr_windows.Domain.BaseModels.BackEnd.Base;
using csr_windows.Domain.Common;
using csr_windows.Domain.WebSocketModels;
using csr_windows.Resources.Enumeration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace csr_windows.Client.ViewModels.Customer
{
    public partial class CustomerViewModel : ObservableRecipient
    {

        #region Fields
        private IUiService _uiService;
        private string _storeName;
        private string _userName;

        private ObservableCollection<UserControl> _userControls = new ObservableCollection<UserControl>();

        private CustomerModel _currentCustomer;


        private UserControl _contentControl;
        private ChatBaseView _loadingChatBaseView = new ChatBaseView()
        {
            DataContext = new ChatBaseViewModel()
            {
                ChatIdentityEnum = ChatIdentityEnum.Recipient,
                ContentControl = new ChatLoadingView()
            }
        };

        private UserControl sseUserControl;

        #endregion

        #region Commands
        /// <summary>
        /// 打开切换人设的窗口
        /// </summary>
        public ICommand OpenChangePersonaCommand { get; set; }
        #endregion

        #region Constructor
        public CustomerViewModel()
        {
            WebServiceClient.SendJSFunc(JSFuncType.GetCurrentCsr);
            _uiService = Ioc.Default.GetService<IUiService>();
            RegistWeakReferenceMessenger();
            OpenChangePersonaCommand = new RelayCommand(OnOpenChangePersonaCommand);
            _uiService.OpenCustomerInitBottomView();


        }

        private void OnKeyCombinationPressed(KeyEventEnum @enum)
        {
            switch (@enum)
            {
                case KeyEventEnum.AltC:

                    break;
                case KeyEventEnum.AltD:
                    break;
                default:
                    break;
            };
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


        public UserControl ContentControl
        {
            get => _contentControl;
            set => SetProperty(ref _contentControl, value);
        }

        /// <summary>
        /// 储存的聊天控件
        /// </summary>
        public ObservableCollection<UserControl> UserControls
        {
            get => _userControls;
            set => SetProperty(ref _userControls, value);
        }

        /// <summary>
        /// 当前客户
        /// </summary>
        public CustomerModel CurrentCustomer
        {
            get => _currentCustomer;
            set => SetProperty(ref _currentCustomer, value);
        }

        #endregion

        #region Methods


        private void OnOpenChangePersonaCommand()
        {
            _uiService.OpenChangePersonaView();
        }

        #endregion


    }
}
