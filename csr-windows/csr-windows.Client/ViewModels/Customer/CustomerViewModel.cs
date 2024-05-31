using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using csr_windows.Client.ViewModels.Chat;
using csr_windows.Client.Views.Chat;
using csr_windows.Resources.Enumeration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace csr_windows.Client.ViewModels.Customer
{
    public class CustomerViewModel: ObservableRecipient
    {

        #region Fields
        private string _storeName = "蜡笔派家居旗舰店";
        private string _userName = "小玲";

        private string _customerNickname = "章盼angela";
        public IList<UserControl> UserControls { get; } = new ObservableCollection<UserControl>();

        #endregion

        #region Commands
        public ICommand TestCommand { get; set; } 
        #endregion

        #region Constructor
        public CustomerViewModel()
        {
            TestCommand = new RelayCommand(OnTestCommand);
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
        private ChatIdentityEnum LastEnum;
        private void OnTestCommand()
        {
           
            ChatBaseView chatBaseView = new ChatBaseView();
            ChatBaseViewModel chatBaseViewModel = new ChatBaseViewModel()
            {
                ChatIdentityEnum = LastEnum
            };
         
            ChatTextView chatTextView = new ChatTextView()
            {
                DataContext = new ChatTextViewModel("我是测试代码啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊")
            };
               if (LastEnum == ChatIdentityEnum.Recipient)
            {
                LastEnum = ChatIdentityEnum.Sender;
            }
            else
            {
                LastEnum = ChatIdentityEnum.Recipient;
            }
            chatBaseViewModel.ContentControl = chatTextView;

            chatBaseView.DataContext = chatBaseViewModel;
            UserControls.Add(chatBaseView);
        }
        #endregion


    }
}
