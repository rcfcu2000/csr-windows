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
    public class CustomerViewModel : ObservableRecipient
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



        #endregion

        #region Commands
        public ICommand TestCommand { get; set; }
        #endregion

        #region Constructor
        public CustomerViewModel()
        {
            WebServiceClient.SendJSFunc(JSFuncType.GetCurrentCsr);
            TestCommand = new RelayCommand(OnTestCommand);
            _uiService = Ioc.Default.GetService<IUiService>();
            //打开顾客界面窗体
            WeakReferenceMessenger.Default.Register<UserControl, string>(this, MessengerConstMessage.OpenCustomerUserControlToken, (r, m) => { ContentControl = m; });
            //改变当前顾客
            WeakReferenceMessenger.Default.Register<CustomerModel, string>(this, MessengerConstMessage.ChangeCurrentCustomerToken, (r, m) =>
            {
                //储存历史记录
                if (!string.IsNullOrEmpty(CurrentCustomer?.UserNiceName))
                {
                    GlobalCache.CustomerChatList[CurrentCustomer.UserNiceName] = new List<UserControl>(UserControls);
                    GlobalCache.CustomerCurrentProductList[CurrentCustomer.UserNiceName] = GlobalCache.CurrentProduct;
                }
                CurrentCustomer = m;
                var isGetSuccess = GlobalCache.CustomerChatList.TryGetValue(CurrentCustomer?.UserNiceName, out List<UserControl> _tempUserControls);

                if (!isGetSuccess || _tempUserControls.Count == 0)
                {
                    UserControls = new ObservableCollection<UserControl>();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        //添加一个欢迎UserControl
                        AddTextControl(ChatIdentityEnum.Recipient, GlobalCache.WelcomeConstString);
                    });
                    GlobalCache.CurrentProduct = null;
                }
                else
                {
                    UserControls = new ObservableCollection<UserControl>(_tempUserControls);
                    if (GlobalCache.CustomerCurrentProductList.ContainsKey(CurrentCustomer.UserNiceName))
                    {
                        GlobalCache.CurrentProduct = GlobalCache.CustomerCurrentProductList[CurrentCustomer.UserNiceName];
                    }
                    else
                    {
                        GlobalCache.CurrentProduct = null;
                    }
                }

            });
            //我该怎么回
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.AskAIToken, (r, m) =>
            {
                AddTextControl(ChatIdentityEnum.Sender, "现在我要怎么回答顾客？");

                if (GlobalCache.CurrentCustomer == null)
                {
                    //发送一条消息
                    AddLoadingControl();
                    Task.Delay(500).ContinueWith(t =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            RemoveLoadingControl();
                            AddTextControl(ChatIdentityEnum.Recipient, "您还没有选择任何顾客进行沟通，我没办法提供建议哦～");
                        });
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        //发送一条消息
                        AddLoadingControl();
                    });
                    WebServiceClient.SendJSFunc(JSFuncType.GetRemoteHisMsg, GlobalCache.CurrentCustomer.CCode, AIChatApiList.How2Replay);
                    //WebServiceClient.SendJSFunc(JSFuncType.GetRemoteHisMsg, GlobalCache.CurrentCustomer.UserNiceName);
                }
            });
            //我该怎么回 回调
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.AskAIResponseToken, AnalysisAskAIReponse);

            //我想这样回
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.Want2ReplyToken, (r, m) => 
            {
                AddBottomBoldControl(ChatIdentityEnum.Sender, m);
                AddLoadingControl();
                GlobalCache.CurrentProductWant2ReplyGuideContent = m;

                WebServiceClient.SendJSFunc(JSFuncType.GetRemoteHisMsg, GlobalCache.CurrentCustomer.CCode, AIChatApiList.Want2Reply);
            });

            //我想这样回 回调
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.Want2ReplyResponseToken, AnalysisAskAIReponse);

            //HTTPError回调
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.ApiChatHttpErrorToken, (r, m) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    RemoveLoadingControl();
                    AddTextControl(ChatIdentityEnum.Recipient, "尴尬了，好像出了点问题，您的问题存在敏感信息，抱歉麻烦等下再试试～");
                });
            });

            //接收千牛Msg单个商品
            WeakReferenceMessenger.Default.Register<SingleProductModel, string>(this, MessengerConstMessage.SendMsgSingleProductToken, OnSendMsgSingleProduct);
            //接收千牛Msg多个商品
            WeakReferenceMessenger.Default.Register<MultipleProductModel, string>(this, MessengerConstMessage.SendMsgMultipleProductToken, OnSendMsgMultipleProduct);

            //点击客服的多个切换商品
            WeakReferenceMessenger.Default.Register<MyProduct, string>(this, MessengerConstMessage.SendChangeProductCustomerServerToken,OnChangeCustomerMulipteProduct);

            //点击客户的多个切换商品
            WeakReferenceMessenger.Default.Register<MyProduct, string>(this, MessengerConstMessage.SendChangeProductCustomerToken, OnChangeCustomerMulipteProduct);

            //点击单个商品的时候切换商品
            WeakReferenceMessenger.Default.Register<MyProduct, string>(this, MessengerConstMessage.SendChangeSingleProductToken, OnChangeSingleProductToken);
            //点击切换商品
            WeakReferenceMessenger.Default.Register<MyProduct, string>(this, MessengerConstMessage.SendSwitchProductToken, OnSendSwitchProductToken);
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

        #region 测试代码

        private int _textCount = 3;
        private int _bottomBoldTextCount = 2;
        private int _copyTextCount = 2;
        private int _chatTextAndProductCount = 2;
        private int _loadingCount = 1;
        private int _chooseProductCount = 1;
        private int _endConvertsationContent = 1;
        private ChatIdentityEnum LastEnum;
        private int _num;
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
                        IsHaveProduct = _copyTextCount == 1 ? true : false,
                        ProductName = _copyTextCount == 1 ? "测试商品" : ""
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
                    chatTextAndProductView.DataContext = new ChatTextAndProductViewModel(myProducts, ChatTextAndProductIdentidyEnum.Customer)
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
                    chatTextAndProductView.DataContext = new ChatTextAndProductViewModel(myProducts, ChatTextAndProductIdentidyEnum.CustomerService)
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
                chatSwitchProductsView.DataContext = new ChatSwitchProductsViewModel(new MyProduct());
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


        /// <summary>
        /// 解析回答
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void AnalysisAskAIReponse(object recipient, string message)
        {
            Console.WriteLine(message);
            //解析msg
            // 切换到UI线程更新UI
            Application.Current.Dispatcher.Invoke(() =>
            {
                var param = JsonConvert.DeserializeObject<MChatApiResult<ChatApiParam>>(message);
                //
                // 使用正则表达式分割字符串
                string[] splitText = Regex.Split(param.Param.Msg, @"(?<=[。；？！～ ： ”])");

                string[] filteredText = splitText
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToArray();
                List<ChatTestModel> chatTestModels = new List<ChatTestModel>();
                for (int i = 1; i < filteredText.Length + 1; i++)
                {

                    var content = filteredText[i - 1].Trim(new char[] { ' ', '\n', '\r', '。' });

                    chatTestModels.Add(new ChatTestModel()
                    {
                        Content = content,
                        IsLast = i == filteredText.Length,
                    });
                }

                //添加文本
                ChatCopyTextView chatCopyTextView = new ChatCopyTextView()
                {
                    DataContext = new ChatCopyTextViewModel(chatTestModels)
                    {
                        IsHaveProduct = !string.IsNullOrEmpty(param.ProductName),
                        ProductName = param.ProductName,
                        AllContent = param.Param.Msg
                    }
                };

                ChatBaseView chatBaseView = new ChatBaseView()
                {
                    DataContext = new ChatBaseViewModel()
                    {
                        ChatIdentityEnum = ChatIdentityEnum.Recipient,
                        ContentControl = chatCopyTextView
                    }
                };


                RemoveLoadingControl();
                UserControls.Add(chatBaseView);
            });

        }

        public ChatBaseView AddTextControl(ChatIdentityEnum identityEnum, string content, bool toAddCurrentUserControls = true)
        {
            // 切换到UI线程更新UI

            ChatBaseView chatBaseView = new ChatBaseView();
            ChatBaseViewModel chatBaseViewModel = new ChatBaseViewModel()
            {
                ChatIdentityEnum = identityEnum
            };


            ChatTextView chatTextView = new ChatTextView()
            {
                DataContext = new ChatTextViewModel(content)
            };

            chatBaseViewModel.ContentControl = chatTextView;
            chatBaseView.DataContext = chatBaseViewModel;
            if (toAddCurrentUserControls)
            {
                UserControls.Add(chatBaseView);
            }
            return chatBaseView;
        }

        public void AddBottomBoldControl(ChatIdentityEnum identityEnum,string content)
        {
            ChatBaseView chatBaseView = new ChatBaseView() 
            {
                DataContext = new ChatBaseViewModel()
                {
                    ChatIdentityEnum = identityEnum,
                    ContentControl = new ChatBottomBoldTextView()
                    {
                        DataContext = new ChatBottomBoldTextViewModel()
                        {
                            Content = content
                        }
                    }
                }
            };

            UserControls.Add(chatBaseView);
        }

        /// <summary>
        /// 添加loading控件
        /// </summary>
        public void AddLoadingControl()
        {
            try
            {
                UserControls.Add(_loadingChatBaseView);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                RemoveLoadingControl();
            }
        }

        /// <summary>
        /// 移除loading控件
        /// </summary>
        public void RemoveLoadingControl()
        {
            UserControls.Remove(_loadingChatBaseView);
        }


        /// <summary>
        /// 接收Msg单个商品
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private async void OnSendMsgSingleProduct(object recipient, SingleProductModel message)
        {
            //根据信息去请求接口
            string msg = await ApiClient.Instance.GetAsync(string.Format($"{BackEndApiList.GetMerchantByTid}/{message.TaoBaoID}"));
            Application.Current.Dispatcher.Invoke(() =>
            {
                AddTextAndProductChat(msg, message, message.Pic, string.IsNullOrEmpty(message.ActionUrl) ? message.E1ActionUrl : message.ActionUrl);
            });
        }

        /// <summary>
        /// 接收Msg多个商品
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private async void OnSendMsgMultipleProduct(object recipient, MultipleProductModel message)
        {
            //根据信息去请求接口
            string msg = await ApiClient.Instance.GetAsync($"{BackEndApiList.GetMerchantByTid}/{message.TaoBaoID}");
            Application.Current.Dispatcher.Invoke(() =>
            {
                AddTextAndProductChat(msg, message, message.Pic, message.ActionUrl);
            });
        }

        /// <summary>
        /// 添加商品聊天控件
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="mBaseProduct"></param>
        /// <param name="picUrl"></param>
        /// <param name="actionUrl"></param>
        private void AddTextAndProductChat(string msg, MBaseProduct mBaseProduct, string picUrl, string actionUrl)
        {

            string startContent, endContent;
            BaseGetMerchantByTidModel model = JsonConvert.DeserializeObject<BaseGetMerchantByTidModel>(msg);
            if (model.Data == null || model.Data.Count == 0)
            {
                if (mBaseProduct.SendUserNiceName == GlobalCache.CurrentCustomer.UserNiceName || mBaseProduct.ReceiveUserNiceName == GlobalCache.CurrentCustomer.UserNiceName)
                {
                    //发送一条消息
                    AddTextControl(ChatIdentityEnum.Recipient, "尴尬了，我好像没见过顾客刚刚发来的商品，抱歉暂时帮不到您了哦～您可以在您的商品知识库中添加，我就会主动学习掌握这件商品的信息了！");
                }
                return;
            }

            ChatBaseView chatBaseView = new ChatBaseView();
            ChatBaseViewModel chatBaseViewModel = new ChatBaseViewModel()
            {
                ChatIdentityEnum = ChatIdentityEnum.Recipient
            };

            ChatTextAndProductView chatTextAndProductView = new ChatTextAndProductView();
            List<MyProduct> myProducts = new List<MyProduct>();
            //判断当前发送人以及接收人是谁
            //如果发送人是当前客服，就需要额外设置w
            ChatTextAndProductIdentidyEnum _chatTextAndProductIdentidyEnum = ChatTextAndProductIdentidyEnum.CustomerService;
            if (mBaseProduct.SendUserNiceName.Contains(GlobalCache.CustomerServiceNickName))
            {
                startContent = "您给顾客发送了一个商品链接，包含如下商品：";
                endContent = "请选择您想要用于后续回复的商品。不选将保持之前所选商品。";
                _chatTextAndProductIdentidyEnum = ChatTextAndProductIdentidyEnum.CustomerService;
            }
            else
            {
                _chatTextAndProductIdentidyEnum = ChatTextAndProductIdentidyEnum.Customer;
                startContent = "顾客发来了一个商品链接，包含如下商品：";
                if (model.Data.Count > 1)
                {
                    endContent = "请向顾客确认后选择具体商品，用于后续回复。如不选择，我将默认为您选择第一款。";
                }
                else
                {
                    endContent = "后续回答将基于该商品。";
                }
            }

            foreach (var item in model.Data)
            {
                myProducts.Add(new MyProduct()
                {
                    MerchantId = item.MerchantId,
                    ProductID = mBaseProduct.TaoBaoID,
                    ProductImage = string.IsNullOrEmpty(item.PictureLink) ? picUrl : item.PictureLink,
                    ProductName = item.Alias,
                    ProductInfo = item.Info,
                    ProductUrl = actionUrl
                });
            }

            chatTextAndProductView.DataContext = new ChatTextAndProductViewModel(myProducts, _chatTextAndProductIdentidyEnum)
            {
                ProductNum = model.Data.Count,
                StartContent = startContent,
                EndContent = endContent,
            };

            //发送者不是当前客户 就去判断是否有第一条
            if (mBaseProduct.SendUserNiceName != GlobalCache.CurrentCustomer.UserNiceName && mBaseProduct.SendUserNiceName != GlobalCache.CustomerServiceNickName)
            {
                var isGetSuccess = GlobalCache.CustomerChatList.TryGetValue(mBaseProduct.SendUserNiceName, out List<UserControl> _tempUserControls);
                if (!isGetSuccess || _tempUserControls.Count == 0)
                {
                    //添加一个欢迎UserControl
                    var firstChat = AddTextControl(ChatIdentityEnum.Recipient, GlobalCache.WelcomeConstString, toAddCurrentUserControls: false);
                    GlobalCache.CustomerChatList[mBaseProduct.SendUserNiceName] = new List<UserControl>() { firstChat };

                }
                chatBaseViewModel.ContentControl = chatTextAndProductView;
                chatBaseView.DataContext = chatBaseViewModel;
                GlobalCache.CustomerChatList[mBaseProduct.SendUserNiceName].Add(chatBaseView);
                if (myProducts.Count == 1)
                {
                    GlobalCache.CustomerCurrentProductList[mBaseProduct.SendUserNiceName] = myProducts[0];
                }
            }
            else
            {
                chatBaseViewModel.ContentControl = chatTextAndProductView;
                chatBaseView.DataContext = chatBaseViewModel;
                if (myProducts.Count == 1)
                {
                    if (GlobalCache.CurrentProduct?.ProductName != myProducts[0].ProductName)
                    {
                        //客户发送的才切换
                        if (_chatTextAndProductIdentidyEnum == ChatTextAndProductIdentidyEnum.Customer)
                        {
                            GlobalCache.CurrentProduct = myProducts[0];
                        }
                        UserControls.Add(chatBaseView);
                    }
                }
                else
                {
                    UserControls.Add(chatBaseView);
                }
            }
        }

        /// <summary>
        /// 切换多个商品
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnChangeCustomerMulipteProduct(object recipient, MyProduct message)
        {
            ChatBaseView chatBaseView = new ChatBaseView() 
            {
               DataContext = new ChatBaseViewModel()
               {
                   ChatIdentityEnum = ChatIdentityEnum.Sender,
                   ContentControl = new ChatTextAndProductView()
                   {
                       DataContext = new ChatTextAndProductViewModel(new List<MyProduct>() { message }, ChatTextAndProductIdentidyEnum.CustomerService)
                       {
                           StartContent = "换成这款商品：",
                       }
                   }
               }
            };

            UserControls.Add(chatBaseView);

            Task.Delay(500).ContinueWith(t => 
            {
                Application.Current.Dispatcher.Invoke(() => 
                {
                    AddTextControl(ChatIdentityEnum.Recipient, "明白了，后续回答将基于该商品。");
                    GlobalCache.CurrentProduct = message;
                });
            });
        }

        /// <summary>
        /// 切换单个商品
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnChangeSingleProductToken(object recipient, MyProduct message)
        {
            ChatBaseView chatBaseView = new ChatBaseView()
            {
                DataContext = new ChatBaseViewModel()
                {
                    ChatIdentityEnum = ChatIdentityEnum.Sender,
                    ContentControl = new ChatTextAndProductView()
                    {
                        DataContext = new ChatTextAndProductViewModel(new List<MyProduct>() { message }, ChatTextAndProductIdentidyEnum.CustomerService)
                        {
                            StartContent = "我想换到这个商品：",
                        }
                    }
                }
            };

            UserControls.Add(chatBaseView);


            ChatBaseView switchProductBaseView = new ChatBaseView()
            {
                DataContext = new ChatBaseViewModel()
                {
                    ChatIdentityEnum = ChatIdentityEnum.Recipient,
                    ContentControl = new ChatSwitchProductsView()
                    {
                        DataContext = new ChatSwitchProductsViewModel(message)
                    }
                }
            };

            UserControls.Add(switchProductBaseView);

        }

        /// <summary>
        /// 切换商品
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnSendSwitchProductToken(object recipient, MyProduct message)
        {
            GlobalCache.CurrentProduct = message;
            AddTextControl(ChatIdentityEnum.Sender, "切换到该商品");
            AddTextControl(ChatIdentityEnum.Recipient, "明白了，后续回答将基于该商品。");
        }


        #endregion


    }
}
