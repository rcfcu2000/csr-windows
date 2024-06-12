﻿using CommunityToolkit.Mvvm.ComponentModel;
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



        #endregion

        #region Commands
        #endregion

        #region Constructor
        public CustomerViewModel()
        {
            WebServiceClient.SendJSFunc(JSFuncType.GetCurrentCsr);
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

            WeakReferenceMessenger.Default.Register<ChatBaseView, string>(this, MessengerConstMessage.SSESteamReponseToken, OnSSESteamReponse);

            //我该怎么回
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.AskAIToken, (r, m) =>
            {
                // 切换到UI线程更新UI
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
                AddWant2ReplyControl(ChatIdentityEnum.Sender,m);
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
            //选择商品界面点击切换商品
            WeakReferenceMessenger.Default.Register<MyProduct, string>(this, MessengerConstMessage.ChooseProductChangeToken, OnChooseProductChange);
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

        private UserControl sseUserControl;
        /// <summary>
        /// SSE流式返回
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        private void OnSSESteamReponse(object recipient, ChatBaseView message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                sseUserControl = message;
                RemoveLoadingControl();
                UserControls.Add(message);
            });
        }

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
                UserControls.Remove(sseUserControl);

                var param = JsonConvert.DeserializeObject<MChatApiResult<ChatApiParam>>(message);
                //
                // 使用正则表达式分割字符串
                //string[] splitText = Regex.Split(param.Param.Msg, @"(?<=[。；？！～ ： ”])");
                string[] splitText = Regex.Split(param.Param.Msg, @"(?<=[。；？！～]|[，。；？！～]”|”[，。；？！～])");


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
            
        public void AddWant2ReplyControl(ChatIdentityEnum identityEnum,string content)
        {
            ChatBaseView chatBaseView = new ChatBaseView()
            {
                DataContext = new ChatBaseViewModel()
                {
                    ChatIdentityEnum = identityEnum,
                    ContentControl = new ChatWant2ReplyView()
                    {
                        DataContext = new ChatWant2ReplyViewModel()
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
                if (UserControls.Contains(_loadingChatBaseView))
                    UserControls.Remove(_loadingChatBaseView);
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
                AddCustomerDialogueProducts(mBaseProduct.SendUserNiceName, myProducts);
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
                AddCustomerDialogueProducts(GlobalCache.CurrentCustomer.UserNiceName, myProducts);
            }
        }

        private void AddCustomerDialogueProducts(string userNickName,List<MyProduct> list)
        {
            var isGetSuccess = GlobalCache.CustomerDialogueProducts.TryGetValue(userNickName, out List<MyProduct> _tempMyProduct);
            if (!isGetSuccess)
            {
                GlobalCache.CustomerDialogueProducts[userNickName] = list;
                return;
            }
            foreach (var item in list)
            {
                var _list = GlobalCache.CustomerDialogueProducts[userNickName].Where(x => x.ProductName == item.ProductName);
                if (_list.Count() == 0)//没有重复的
                {
                    GlobalCache.CustomerDialogueProducts[userNickName].Add(item);
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


        /// <summary>
        /// 选择商品界面切换商品
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnChooseProductChange(object recipient, MyProduct message)
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
                            StartContent = "后续内容基于这款商品：",
                        }
                    }
                }
            };
            UserControls.Add(chatBaseView);
            GlobalCache.CurrentProduct = message;
        }

        #endregion


    }
}
