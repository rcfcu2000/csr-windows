using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Domain;
using csr_windows.Domain.Common;
using csr_windows.Resources.Enumeration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace csr_windows.Client.ViewModels.Chat
{
    public class ChatTextAndProductViewModel : ObservableRecipient
    {

        #region Fields
        private string _startContent;
        private string _endContent;
        private bool _isShowEndContent;
        private ChatTextAndProductIdentidyEnum _chatTextAndProductIdentidyEnum;
        #endregion

        #region Commands
        public ICommand ChooseCommand { get; set; }
        #endregion

        #region Constructor

        public ChatTextAndProductViewModel(List<MyProduct> myProducts, ChatTextAndProductIdentidyEnum chatTextAndProductIdentidyEnum)
        {
            ProductsList=  new ObservableCollection<MyProduct>(myProducts);
            ChatTextAndProductIdentidyEnum = chatTextAndProductIdentidyEnum;
            ChooseCommand = new RelayCommand<MyProduct>(OnChooseCommand);
        }

       

        #endregion

        #region Properties
        /// <summary>
        /// 商品列表
        /// </summary>
        public IList<MyProduct> ProductsList { get; } = new ObservableCollection<MyProduct>();

        /// <summary>
        /// 商品数量
        /// </summary>
        public int ProductNum { get; set; }


        /// <summary>
        /// 开始内容
        /// </summary>
        public string StartContent
        {
            get => _startContent;
            set => SetProperty(ref _startContent, value);
        }

        /// <summary>
        /// 结束内容
        /// </summary>
        public string EndContent
        {
            get => _endContent;
            set 
            {
                SetProperty(ref _endContent, value);
                if(string.IsNullOrEmpty(_endContent))
                    IsShowEndContent = false;
                else
                    IsShowEndContent = true;
            }
        }

        /// <summary>
        /// 是否显示结束内容
        /// </summary>
        public bool IsShowEndContent
        {
            get => _isShowEndContent;
            set => SetProperty(ref _isShowEndContent, value);
        }

        /// <summary>
        /// 文本身份枚举
        /// </summary>
        public ChatTextAndProductIdentidyEnum ChatTextAndProductIdentidyEnum
        {
            get => _chatTextAndProductIdentidyEnum;
            set => SetProperty(ref _chatTextAndProductIdentidyEnum, value);
        }
        #endregion

        #region Methods

        private void OnChooseCommand(MyProduct product)
        {
            //发送切换商品
            //客户
            if (ChatTextAndProductIdentidyEnum == ChatTextAndProductIdentidyEnum.CustomerService)
            {
                if (ProductNum <= 1)
                {
                    return;
                }
                WeakReferenceMessenger.Default.Send(product, MessengerConstMessage.SendChangeProductCustomerToken);
            }
            else //客服
            {
                WeakReferenceMessenger.Default.Send(product, MessengerConstMessage.SendChangeProductCustomerServerToken);
            }


        }

        #endregion

    }
}
