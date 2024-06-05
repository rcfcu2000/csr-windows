using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Services.Base;
using csr_windows.Client.View.Chat;
using csr_windows.Client.ViewModels.Chat;
using csr_windows.Client.Views.Chat;
using csr_windows.Domain.Common;
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
    public class CustomerViewModel : ObservableRecipient
    {

        #region Fields
        private IUiService _uiService;
        private string _storeName = "蜡笔派家居旗舰店";
        private string _userName = "小玲";

        private string _customerNickname = "章盼angela";
        public IList<UserControl> UserControls { get; } = new ObservableCollection<UserControl>();

        private bool _haveCustomer = true;

        private UserControl _contentControl;


        #endregion

        #region Commands
        public ICommand TestCommand { get; set; }
        #endregion

        #region Constructor
        public CustomerViewModel()
        {
            TestCommand = new RelayCommand(OnTestCommand);
            _uiService = Ioc.Default.GetService<IUiService>();
            WeakReferenceMessenger.Default.Register<UserControl, string>(this, MessengerConstMessage.OpenCustomerUserControlToken, (r, m) => { ContentControl = m; });
            _uiService.OpenCustomerInitBottomView();
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

        /// <summary>
        /// 是否有顾客
        /// </summary>
        public bool HaveCustomer
        {
            get => _haveCustomer;
            set => SetProperty(ref _haveCustomer, value);
        }


        public UserControl ContentControl
        {
            get => _contentControl;
            set => SetProperty(ref _contentControl, value);
        }

        #endregion

        #region Methods
        private int _textCount = 3;
        private int _bottomBoldTextCount = 2;
        private int _copyTextCount = 2;
        private int _chatTextAndProductCount = 2;
        private int _loadingCount = 1;
        private int _chooseProductCount = 1;
        private int _endConvertsationContent = 1;
        private ChatIdentityEnum LastEnum;
        private void OnTestCommand()
        {

            ChatBaseView chatBaseView = new ChatBaseView();
            ChatBaseViewModel chatBaseViewModel = new ChatBaseViewModel()
            {
                ChatIdentityEnum = LastEnum
            };

            if (_textCount != 0)
            {
                
                ChatTextView chatTextView = new ChatTextView()
                {
                    DataContext = new ChatTextViewModel("我是测试代码啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊啊")
                };

                chatBaseViewModel.ContentControl = chatTextView;
                _textCount--;
                goto AddFlag;
            }

            if (_bottomBoldTextCount != 0)
            {
                ChatBottomBoldTextView chatBottomBoldTextView = new ChatBottomBoldTextView()
                {
                    DataContext = new ChatBottomBoldTextViewModel()
                    {
                        Content = $"我是底部加粗文本：{_bottomBoldTextCount}！！！"
                    }
                };
                chatBaseViewModel.ContentControl = chatBottomBoldTextView;
                _bottomBoldTextCount--;
                goto AddFlag;
            }

            if (_copyTextCount != 0)
            {
                List<ChatTestModel> chatTestModels = new List<ChatTestModel>()
                    {
                        new ChatTestModel()
                        {
                            Content = "亲亲，非常感谢您的反馈，关于您提到的台布裁剪问题，我们深感抱歉",
                            IsLast = false,
                        },
                        new ChatTestModel()
                        {
                            Content = "我们的裁剪确实是手工进行的，可能会有一些不完美的地方",
                            IsLast = false,
                        },
                        new ChatTestModel()
                        {
                            Content = "请问这块台布是否还能满足您的使用需求呢？",
                            IsLast = true,
                        }
                    };
                ChatCopyTextView chatCopyTextView = new ChatCopyTextView()
                {
                    DataContext = new ChatCopyTextViewModel(chatTestModels)
                    {
                        IsHaveProduct = _copyTextCount == 1 ?  true :false,
                        ProductName = _copyTextCount == 1 ?  "测试商品" : "" 
                    }
                };
                chatBaseViewModel.ChatIdentityEnum = ChatIdentityEnum.Recipient;
                chatBaseViewModel.ContentControl = chatCopyTextView;
                _copyTextCount--;
                goto AddFlag;
            }

            if (_loadingCount != 0)
            {
                ChatLoadingView chatLoadingView = new ChatLoadingView();
                chatBaseViewModel.ContentControl = chatLoadingView;
                chatBaseViewModel.ChatIdentityEnum = ChatIdentityEnum.Recipient;
                _loadingCount--;
                goto AddFlag;

            }

            if (_chatTextAndProductCount != 0)
            {
                List<MyProduct> myProducts;
                ChatTextAndProductView chatTextAndProductView = new ChatTextAndProductView();
                if (_chatTextAndProductCount == 2)
                {
                    myProducts = new List<MyProduct>();
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            myProducts.Add(new MyProduct()
                            {
                                ProductImage = "https://pic1.zhimg.com/v2-0dda71bc9ced142bf7bb2d6adbebe4f0_r.jpg?source=1940ef5c",
                                ProductName = $"商品名称 Index:{i}"
                            });
                        }
                    }
                    chatTextAndProductView.DataContext = new ChatTextAndProductViewModel(myProducts, LastEnum)
                    {
                        StartContent = "根据您和顾客的对话，我发现顾客发来了一个商品链接，包含如下商品：",
                        EndContent = "请向顾客确认后选择具体商品，用于后续回复。如不选择，我将默认为您选择第一款。",
                    };
                }
                else
                {
                    myProducts = new List<MyProduct>();
                    {
                        myProducts.Add(new MyProduct()
                        {
                            ProductImage = "https://pic1.zhimg.com/v2-0dda71bc9ced142bf7bb2d6adbebe4f0_r.jpg?source=1940ef5c",
                            ProductName = $"商品名称 Index:{"啊啊啊啊啊啊啊啊"}"
                        });
                    }
                    chatTextAndProductView.DataContext = new ChatTextAndProductViewModel(myProducts, LastEnum)
                    {
                        StartContent = "顾客咨询的是这一款：",
                    };
                }

                
                chatBaseViewModel.ContentControl = chatTextAndProductView;
                _chatTextAndProductCount--;
                goto AddFlag;
            }

            if (_chooseProductCount != 0)
            {
                ChatSwitchProductsView chatSwitchProductsView = new ChatSwitchProductsView();
                chatSwitchProductsView.DataContext = new ChatSwitchProductsViewModel();
                chatBaseViewModel.ContentControl = chatSwitchProductsView;
                chatBaseViewModel.ChatIdentityEnum = ChatIdentityEnum.Recipient;
                _chooseProductCount--;
                goto AddFlag;
            }
            if (_endConvertsationContent != 0)
            {
                ChatEndConversationView chatEndConversationView = new ChatEndConversationView();
                chatEndConversationView.DataContext = new ChatEndConversationViewModel();
                chatBaseViewModel.ContentControl = chatEndConversationView;
                chatBaseViewModel.ChatIdentityEnum = ChatIdentityEnum.Recipient;
                _endConvertsationContent--;
                goto AddFlag;
            }
            



        AddFlag:

            if (LastEnum == ChatIdentityEnum.Recipient)
            {
                LastEnum = ChatIdentityEnum.Sender;
            }
            else
            {
                LastEnum = ChatIdentityEnum.Recipient;
            }
            chatBaseView.DataContext = chatBaseViewModel;
            UserControls.Add(chatBaseView);
        }
        #endregion


    }
}
