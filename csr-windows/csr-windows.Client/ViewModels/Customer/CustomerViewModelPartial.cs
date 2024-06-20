using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Views.Chat;
using csr_windows.Domain.Common;
using csr_windows.Domain.WebSocketModels;
using csr_windows.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using csr_windows.Resources.Enumeration;
using System.Windows.Controls;
using csr_windows.Client.Services.WebService;
using csr_windows.Client.Services.WebService.Enums;
using csr_windows.Domain.Api;
using csr_windows.Client.ViewModels.Chat;
using csr_windows.Core;
using csr_windows.Domain.BaseModels.BackEnd.Base;
using csr_windows.Domain.BaseModels;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using csr_windows.Common.Utils;

namespace csr_windows.Client.ViewModels.Customer
{
    public partial class CustomerViewModel
    {
        #region Methods

        #region 注册
        private void RegistWeakReferenceMessenger()
        {
            //打开顾客界面窗体
            WeakReferenceMessenger.Default.Register<UserControl, string>(this, MessengerConstMessage.OpenCustomerUserControlToken, (r, m) => { ContentControl = m; });

            //储存当前用户的数据
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.StoreOldCurrentCustomerToekn, (r,m) => 
            {
                //储存历史记录
                if (!string.IsNullOrEmpty(CurrentCustomer?.UserNiceName))
                {
                    GlobalCache.CustomerChatList[CurrentCustomer.UserNiceName] = new List<UserControl>(UserControls);
                    GlobalCache.CustomerCurrentProductList[CurrentCustomer.UserNiceName] = GlobalCache.CurrentProduct;
                    GlobalCache.CustomerDialogueLastTaoBaoId[CurrentCustomer.UserNiceName] = GlobalCache.DialogueLastTaoBaoId;
                    GlobalCache.CustomerAutoReplyRegex[CurrentCustomer.UserNiceName] = GlobalCache.AutoReplyRegex;
                    GlobalCache.CustomerAiChatAutoReplyRegex[CurrentCustomer.UserNiceName] = GlobalCache.AiChatAutoReplyRegex;
                }
                CurrentCustomer = null;
            });

            //改变当前顾客
            WeakReferenceMessenger.Default.Register<CustomerModel, string>(this, MessengerConstMessage.ChangeCurrentCustomerToken, (r, m) =>
            {
                //储存历史记录
                if (!string.IsNullOrEmpty(CurrentCustomer?.UserNiceName))
                {
                    GlobalCache.CustomerChatList[CurrentCustomer.UserNiceName] = new List<UserControl>(UserControls);
                    GlobalCache.CustomerCurrentProductList[CurrentCustomer.UserNiceName] = GlobalCache.CurrentProduct;
                    GlobalCache.CustomerDialogueLastTaoBaoId[CurrentCustomer.UserNiceName] = GlobalCache.DialogueLastTaoBaoId;
                    GlobalCache.CustomerAutoReplyRegex[CurrentCustomer.UserNiceName] = GlobalCache.AutoReplyRegex;
                    GlobalCache.CustomerAiChatAutoReplyRegex[CurrentCustomer.UserNiceName] = GlobalCache.AiChatAutoReplyRegex;
                }
                CurrentCustomer = m;
                var isGetSuccess = GlobalCache.CustomerChatList.TryGetValue(CurrentCustomer?.UserNiceName, out List<UserControl> _tempUserControls);

                if (!isGetSuccess || _tempUserControls.Count == 0)
                {
                    UserControls = new ObservableCollection<UserControl>();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (UserControls.Count == 0)
                        {
                            //添加一个欢迎UserControl
                            AddTextControl(ChatIdentityEnum.Recipient, GlobalCache.WelcomeConstString);
                        }
                    });
                    GlobalCache.CurrentProduct = null;
                }
                else
                {
                    UserControls = new ObservableCollection<UserControl>(_tempUserControls);
                    if (GlobalCache.CustomerCurrentProductList.ContainsKey(CurrentCustomer.UserNiceName))
                    {
                        GlobalCache.CurrentProduct = GlobalCache.CustomerCurrentProductList[CurrentCustomer.UserNiceName];
                        GlobalCache.DialogueLastTaoBaoId = GlobalCache.CustomerDialogueLastTaoBaoId[CurrentCustomer.UserNiceName];
                    }
                    else
                    {
                        GlobalCache.CurrentProduct = null;
                        GlobalCache.DialogueLastTaoBaoId = string.Empty;
                    }
                }

                //更换当前用户的是否有匹配的正则
                if (GlobalCache.CustomerAutoReplyRegex.ContainsKey(CurrentCustomer.UserNiceName))
                    GlobalCache.AutoReplyRegex = GlobalCache.CustomerAutoReplyRegex[CurrentCustomer.UserNiceName];
                //更换当前用户已经被AI处理过的自动回复
                if (GlobalCache.CustomerAiChatAutoReplyRegex.ContainsKey(CurrentCustomer.UserNiceName))
                    GlobalCache.AiChatAutoReplyRegex = GlobalCache.CustomerAiChatAutoReplyRegex[CurrentCustomer.UserNiceName];


                //去匹配正则
                WebServiceClient.SendJSFunc(JSFuncType.GetRemoteHisMsg, GlobalCache.CurrentCustomer.CCode, ApiConstToken.MsgRegexToken);

            });

            WeakReferenceMessenger.Default.Register<ChatBaseView, string>(this, MessengerConstMessage.SSESteamReponseToken, OnSSESteamReponse);
            //主动接收历史消息为空
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.ActiveReceiveRemoteHisMsgHistoryNull, (r,m) =>
            {
                RemoveLoadingControl();
                AddTextControl(ChatIdentityEnum.Recipient, "好像您最近没有和这位顾客有过沟通呢～");
            });

            //我该怎么回
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.AskAIToken, (r, m) =>
            {
                // 切换到UI线程更新UI
                AddTextControl(ChatIdentityEnum.Sender, "现在我要怎么回答顾客？");

                if (GlobalCache.CurrentCustomer == null || string.IsNullOrEmpty(GlobalCache.CurrentCustomer.UserNiceName))
                {
                    //发送一条消息
                    AddLoadingControl();
                    Task.Delay(500).ContinueWith(t =>
                    {
                        RemoveLoadingControl();
                        AddTextControl(ChatIdentityEnum.Recipient, "您还没有选择任何顾客进行沟通，我没办法提供建议哦～");
                    });
                }
                else
                {
                    //发送一条消息
                    AddLoadingControl();
                    WebServiceClient.SendJSFunc(JSFuncType.GetRemoteHisMsg, GlobalCache.CurrentCustomer.CCode, AIChatApiList.How2Replay);
                    //WebServiceClient.SendJSFunc(JSFuncType.GetRemoteHisMsg, GlobalCache.CurrentCustomer.UserNiceName);
                }
            });
            //我该怎么回 回调
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.AskAIResponseToken, AnalysisAskAIReponse);

            //我想这样回
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.Want2ReplyToken, (r, m) =>
            {
                if (GlobalCache.CurrentCustomer == null || string.IsNullOrEmpty(GlobalCache.CurrentCustomer.UserNiceName))
                {
                    //发送一条消息
                    AddLoadingControl();
                    Task.Delay(500).ContinueWith(t =>
                    {
                        RemoveLoadingControl();
                        AddTextControl(ChatIdentityEnum.Recipient, "您还没有选择任何顾客进行沟通，我没办法提供建议哦～");
                    });
                }
                else
                {
                    AddWant2ReplyControl(ChatIdentityEnum.Sender, m);
                    GlobalCache.CurrentProductWant2ReplyGuideContent = m;
                    //发送一条消息
                    AddLoadingControl();
                    WebServiceClient.SendJSFunc(JSFuncType.GetRemoteHisMsg, GlobalCache.CurrentCustomer.CCode, AIChatApiList.Want2Reply);
                }
            });

            //我想这样回 回调
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.Want2ReplyResponseToken, AnalysisAskAIReponse);

            //HTTPError回调
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.ApiChatHttpErrorToken, (r, m) =>
            {
                RemoveLoadingControl();
                AddTextControl(ChatIdentityEnum.Recipient, "尴尬了，好像出了点问题，您的问题存在敏感信息，抱歉麻烦等下再试试～");
            });

            //接收千牛Msg单个商品
            WeakReferenceMessenger.Default.Register<SingleProductModel, string>(this, MessengerConstMessage.SendMsgSingleProductToken, OnSendMsgSingleProduct);
            //接收千牛Msg多个商品
            WeakReferenceMessenger.Default.Register<MultipleProductModel, string>(this, MessengerConstMessage.SendMsgMultipleProductToken, OnSendMsgMultipleProduct);

            //点击客服的多个切换商品
            WeakReferenceMessenger.Default.Register<MyProduct, string>(this, MessengerConstMessage.SendChangeProductCustomerServerToken, OnChangeCustomerMulipteProduct);

            //点击客户的多个切换商品
            WeakReferenceMessenger.Default.Register<MyProduct, string>(this, MessengerConstMessage.SendChangeProductCustomerToken, OnChangeCustomerMulipteProduct);

            //点击单个商品的时候切换商品
            WeakReferenceMessenger.Default.Register<MyProduct, string>(this, MessengerConstMessage.SendChangeSingleProductToken, OnChangeSingleProductToken);
            //点击切换商品
            WeakReferenceMessenger.Default.Register<MyProduct, string>(this, MessengerConstMessage.SendSwitchProductToken, OnSendSwitchProductToken);
            //选择商品界面点击切换商品
            WeakReferenceMessenger.Default.Register<MyProduct, string>(this, MessengerConstMessage.ChooseProductChangeToken, OnChooseProductChange);
            //商品介绍选择
            WeakReferenceMessenger.Default.Register<MyProduct, string>(this, MessengerConstMessage.ProductIntroductionToken, OnProductIntroduction);
            //商品推荐
            WeakReferenceMessenger.Default.Register<ObservableCollection<MyProduct>, string>(this, MessengerConstMessage.RecommendedPairingToken, OnRecommendedPairing);
            //商品推荐 回调
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.ReMultiGoodReponseToken, OnReMultiGoodReponse);


        }






        #endregion

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
                if (!UserControls.Contains(message))
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
            //解析msg
            // 切换到UI线程更新UI
            Application.Current.Dispatcher.Invoke(() =>
            {
                UserControls.Remove(sseUserControl);

                var param = JsonConvert.DeserializeObject<MChatApiResult<ChatApiParam>>(message);
                
                // 使用正则表达式分割字符串
                var result = StringProcessor.Divide(param.Param.Msg);

                // 使用LINQ去除空字符串
                List<List<string>> cleanedListOfresult = result
                    .Select(subList => subList.Where(str => !string.IsNullOrEmpty(str)).ToList())
                    .Where(subList => subList.Any()) // 如果需要去除空子列表
                    .ToList();

                List<ChatTestModel> chatTestModels = new List<ChatTestModel>();
                for (int i = 0; i < cleanedListOfresult.Count; i++)
                {
                    for (int j = 0; j < cleanedListOfresult[i].Count; j++)
                    {
                        var content = cleanedListOfresult[i][j].Trim(new char[] {' ', '\n', '\r', '。' });

                        chatTestModels.Add(new ChatTestModel()
                        {
                            Content = content,
                            IsLast = i == cleanedListOfresult.Count + 1 && j == cleanedListOfresult[i].Count + 1,
                        });
                    }

                   
                }

                //添加文本
                ChatBaseView chatBaseView = new ChatBaseView()
                {
                    DataContext = new ChatBaseViewModel()
                    {
                        ChatIdentityEnum = ChatIdentityEnum.Recipient,
                        ContentControl = new ChatCopyTextView()
                        {
                            DataContext = new ChatCopyTextViewModel(chatTestModels)
                            {
                                IsHaveProduct = !string.IsNullOrEmpty(param.ProductName),
                                ProductName = param.ProductName,
                                AllContent = param.Param.Msg
                            }
                        }
                    }
                };


                RemoveLoadingControl();
                if (!UserControls.Contains(chatBaseView))
                    UserControls.Add(chatBaseView);
            });

        }

        public ChatBaseView AddTextControl(ChatIdentityEnum identityEnum, string content, bool toAddCurrentUserControls = true)
        {
            ChatBaseView chatBaseView1 = Application.Current.Dispatcher.Invoke(() =>
            {
                // 切换到UI线程更新UI
                ChatBaseView chatBaseView = new ChatBaseView()
                {
                    DataContext = new ChatBaseViewModel()
                    {
                        ChatIdentityEnum = identityEnum,
                        ContentControl = new ChatTextView()
                        {
                            DataContext = new ChatTextViewModel(content)
                        }
                    }
                };

                if (toAddCurrentUserControls)
                {
                    try
                    {
                        if (!UserControls.Contains(chatBaseView))
                            UserControls.Add(chatBaseView);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        Logger.WriteError($"异常信息: {ex.Message}");
                        Logger.WriteError($"参数名: {ex.ParamName}");
                        Logger.WriteError($"堆栈跟踪: {ex.StackTrace}");
                    }
                }
                return chatBaseView;
            });

            return chatBaseView1;
        }

        public void AddBottomBoldControl(ChatIdentityEnum identityEnum, string content)
        {
            Application.Current.Dispatcher.Invoke(() => 
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
                if (!UserControls.Contains(chatBaseView))
                    UserControls.Add(chatBaseView);
            });
        }

        public void AddWant2ReplyControl(ChatIdentityEnum identityEnum, string content)
        {
            Application.Current.Dispatcher.Invoke(() =>
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
                if (!UserControls.Contains(chatBaseView))
                    UserControls.Add(chatBaseView);
            });
        }
        /// <summary>
        /// 添加loading控件
        /// </summary>
        public void AddLoadingControl()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() => 
                {
                    if (UserControls.Contains(_loadingChatBaseView))
                        UserControls.Remove(_loadingChatBaseView);
                    UserControls.Add(_loadingChatBaseView);
                });

            }
            catch (Exception ex)
            {
                Logger.WriteInfo(ex.Message);
                RemoveLoadingControl();
            }
        }

        /// <summary>
        /// 移除loading控件
        /// </summary>
        public void RemoveLoadingControl()
        {
            Application.Current.Dispatcher.Invoke(() => 
            {
                if (UserControls.Contains(_loadingChatBaseView))
                    UserControls.Remove(_loadingChatBaseView);
            });
            
        }


        /// <summary>
        /// 接收Msg单个商品
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private async void OnSendMsgSingleProduct(object recipient, SingleProductModel message)
        {
            //判断是否对话中提到的最后一个taobaoid 跟现在的商品taobaoid是否一样，一样的话就直接return
            if (string.IsNullOrEmpty(GlobalCache.DialogueLastTaoBaoId))
                GlobalCache.DialogueLastTaoBaoId = message.TaoBaoID;
            else if(GlobalCache.DialogueLastTaoBaoId == message.TaoBaoID)
                return;
             else
                GlobalCache.DialogueLastTaoBaoId = message.TaoBaoID;

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
            //判断是否对话中提到的最后一个taobaoid 跟现在的商品taobaoid是否一样，一样的话就直接return
            if (string.IsNullOrEmpty(GlobalCache.DialogueLastTaoBaoId))
                GlobalCache.DialogueLastTaoBaoId = message.TaoBaoID;
            else if (GlobalCache.DialogueLastTaoBaoId == message.TaoBaoID)
                return;
            else
                GlobalCache.DialogueLastTaoBaoId = message.TaoBaoID;

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

            //发送者不是当前聊天 就去储存到相应的聊天人那里
            if (mBaseProduct.SendUserNiceName != GlobalCache.CurrentCustomer.UserNiceName && mBaseProduct.SendUserNiceName != GlobalCache.CustomerServiceNickName)
            {
                var isGetSuccess = GlobalCache.CustomerChatList.TryGetValue(mBaseProduct.SendUserNiceName, out List<UserControl> _tempUserControls);
                //如果没显示过欢迎语
                if (!isGetSuccess || _tempUserControls.Count == 0)
                {
                    //添加一个欢迎UserControl
                    var firstChat = AddTextControl(ChatIdentityEnum.Recipient, GlobalCache.WelcomeConstString, toAddCurrentUserControls: false);
                    GlobalCache.CustomerChatList[mBaseProduct.SendUserNiceName] = new List<UserControl>() { firstChat };
                }
                chatBaseViewModel.ContentControl = chatTextAndProductView;
                chatBaseView.DataContext = chatBaseViewModel;
                if (!GlobalCache.CustomerChatList[mBaseProduct.SendUserNiceName].Contains(chatBaseView))
                    GlobalCache.CustomerChatList[mBaseProduct.SendUserNiceName].Add(chatBaseView);
                GlobalCache.CustomerDialogueLastTaoBaoId[mBaseProduct.SendUserNiceName] = mBaseProduct.TaoBaoID;
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
                GlobalCache.DialogueLastTaoBaoId = mBaseProduct.TaoBaoID;
                if (myProducts.Count == 1)
                {
                    if (GlobalCache.CurrentProduct?.ProductName != myProducts[0].ProductName)
                    {
                        //客户发送的才切换
                        if (_chatTextAndProductIdentidyEnum == ChatTextAndProductIdentidyEnum.Customer)
                        {
                            GlobalCache.CurrentProduct = myProducts[0];
                        }
                        if (!UserControls.Contains(chatBaseView))
                            UserControls.Add(chatBaseView);
                    }
                }
                else
                {
                    if (!UserControls.Contains(chatBaseView))
                        UserControls.Add(chatBaseView);
                }
                AddCustomerDialogueProducts(GlobalCache.CurrentCustomer.UserNiceName, myProducts);
            }
        }

        private void AddCustomerDialogueProducts(string userNickName, List<MyProduct> list)
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
                    if (!GlobalCache.CustomerDialogueProducts[userNickName].Contains(item))
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
            Application.Current.Dispatcher.Invoke(() => 
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
                if (!UserControls.Contains(chatBaseView))
                    UserControls.Add(chatBaseView);
            });
            Task.Delay(500).ContinueWith(t =>
            {
                AddTextControl(ChatIdentityEnum.Recipient, "明白了，后续回答将基于该商品。");
                Application.Current.Dispatcher.Invoke(() =>
                {
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
            Application.Current.Dispatcher.Invoke(() => 
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

                if (!UserControls.Contains(chatBaseView))
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
                if (!UserControls.Contains(switchProductBaseView))
                    UserControls.Add(switchProductBaseView);
            });
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
            Application.Current.Dispatcher.Invoke(() => 
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
                if (!UserControls.Contains(chatBaseView))
                    UserControls.Add(chatBaseView);
                AddTextControl(ChatIdentityEnum.Recipient, "明白了，后续回答将基于该商品。");
                GlobalCache.CurrentProduct = message;
            });
            
        }

        /// <summary>
        /// 商品介绍
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        private void OnProductIntroduction(object recipient, MyProduct message)
        {
            
        }
        
        /// <summary>
        /// 商品搭配
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnRecommendedPairing(object recipient, ObservableCollection<MyProduct> message)
        {

            if (GlobalCache.CurrentCustomer == null || string.IsNullOrEmpty(GlobalCache.CurrentCustomer.UserNiceName))
            {
                //发送一条消息
                AddLoadingControl();
                Task.Delay(500).ContinueWith(t =>
                {
                    RemoveLoadingControl();
                    AddTextControl(ChatIdentityEnum.Recipient, "您还没有选择任何顾客进行沟通，我没办法提供建议哦～");
                });
            }
            else
            {
                GlobalCache.RecommendedPairing = new List<MyProduct>(message);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    List<MyProduct> _myProducts = new List<MyProduct>(message);
                    var firstProduct = _myProducts.FirstOrDefault((x => x.ProductName == GlobalCache.CurrentProduct.ProductName));
                    if (firstProduct == null)
                    {
                        _myProducts.Add(GlobalCache.CurrentProduct);
                    }

                    ChatBaseView chatBaseView = new ChatBaseView()
                    {
                        DataContext = new ChatBaseViewModel()
                        {
                            ChatIdentityEnum = ChatIdentityEnum.Sender,
                            ContentControl = new ChatTextAndProductView()
                            {
                                DataContext = new ChatTextAndProductViewModel(_myProducts, ChatTextAndProductIdentidyEnum.CustomerService)
                                {
                                    StartContent = $"帮我结合“{GlobalCache.ProductIntroductionCustomerScene}”推荐以下商品的搭配："
                                }
                            }
                        }
                    };
                    if (!UserControls.Contains(chatBaseView))
                        UserControls.Add(chatBaseView);
                    //发送一条消息
                    AddLoadingControl();
                });
                WebServiceClient.SendJSFunc(JSFuncType.GetRemoteHisMsg, GlobalCache.CurrentCustomer.CCode, AIChatApiList.ReMultiGood);
            }
        }


        private void OnReMultiGoodReponse(object recipient, string message)
        {
            GlobalCache.RecommendedPairing = null;

            //您刚刚选择的商品

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
                        ProductName = "您刚刚选择的商品",
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
                if (!UserControls.Contains(chatBaseView))
                    UserControls.Add(chatBaseView);
            });
        }
        #endregion

    }
}
