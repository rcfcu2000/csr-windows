using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Services.Base;
using csr_windows.Client.Services.WebService.Enums;
using csr_windows.Client.ViewModels.Chat;
using csr_windows.Client.Views.Chat;
using csr_windows.Common.Helper;
using csr_windows.Domain;
using csr_windows.Domain.AIChat;
using csr_windows.Domain.Api;
using csr_windows.Domain.BaseModels;
using csr_windows.Domain.Common;
using csr_windows.Domain.WebSocketModels;
using Fleck;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpDX.Direct3D11;
using Sunny.UI.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Forms.Design;
using System.Windows.Interop;

namespace csr_windows.Client.Services.WebService
{
    internal static class WebServiceClient
    {
        private static IUiService _uIService;
        /// <summary>
        /// 商品聊天模板id列表
        /// </summary>
        private static List<int> ProductChatTemplateIdList = new List<int>() { 241005, 262002, 101, 200005, 129 };
        private static Dictionary<string, IWebSocketConnection> allSockets = new Dictionary<string, IWebSocketConnection>();
        private static SunnyNet syNet = new SunnyNet();
        private static readonly HttpClient _httpClient = new HttpClient();
        public static IWebSocketConnection Socket;
        public static WebSocketServer wsServer = null;

        static string aiURL = "https://www.zhihuige.cc/csrnew/api";
        //static string aiURL = "http://192.168.2.133:5060/api";

        public static void StartHttpsServer()
        {
            bool sunnyNetIsRun = false;

            syNet.绑定端口(80);
            syNet.绑定端口(443);
            syNet.绑定回调地址(Callback.HTTP回调, null, null, null);
            sunnyNetIsRun = syNet.启动();

            //CertificateManager certManager = new CertificateManager();
            //certManager._载入X509KeyPair("E:\\works\\ollydbg\\demo\\server.crt", "E:\\works\\ollydbg\\demo\\server.key");
            //Logger.WriteInfo($"server：{certManager.获取ServerName()}");

            //syNet.设置自定义CA证书(certManager);
            //syNet.安装证书();

            //syNet.强制客户端走TCP(true);

            //syNet.进程代理_设置捕获任意进程(true);
            syNet.进程代理_添加进程名("AliRender.exe");
            syNet.进程代理_添加进程名("AliApp.exe");
            //syNet.进程代理_添加进程名("AliWorkbench.exe");
            //syNet.进程代理_添加进程名("aliapp.exe");
            //syNet.进程代理_添加进程名("Aliapp.exe");
            bool proxyRun = syNet.进程代理_加载驱动();

            Logger.WriteInfo($"中间件启动结果：{proxyRun}");

            Logger.WriteInfo($"中间件启动结果：{proxyRun}");

            if (sunnyNetIsRun)
            {
                Logger.WriteInfo(sunnyNetIsRun.ToString());
            }
            else
            {
                Logger.WriteInfo("启动失败" + syNet.取错误());
                //错误处理
                Logger.WriteError("启动失败" + syNet.取错误());
            }

            Logger.WriteInfo("HTTPS server started.");
        }

        public static void StartWebSocketServer()
        {
            var server = new WebSocketServer("ws://0.0.0.0:50000");
            wsServer = server;
            server.Start(socket =>
            {
                Socket = socket;
                socket.OnOpen = () =>
                {
                    Logger.WriteInfo("WebSocket connection opened.");
                    
                    //启动成功
                    SendJSFunc(JSFuncType.GetCurrentCsr,socket:socket);
                };

                socket.OnClose = () =>
                {
                    Logger.WriteInfo("WebSocket connection closed.");
                    allSockets.Remove(TopHelp.GetQNChatTitle());

                    if (allSockets.Count == 0)
                    {
                        GlobalCache.IsFollowWindow = false;
                        GlobalCache.FollowHandle = IntPtr.Zero;
                        _uIService = Ioc.Default.GetService<IUiService>();
                        _uIService.OpenNoStartClientView();
                    }
                };

                socket.OnMessage = message =>
                {
                    dynamic json = JsonConvert.DeserializeObject(message);
                    if (json.type == "goodsList")
                    {
                        JArray glist = json.msg.data.table.dataSource;
                        Logger.WriteInfo(JsonConvert.SerializeObject(glist));
                        List<GetGoodProductModel> list = new List<GetGoodProductModel>();
                        foreach (dynamic good in glist)
                        {
                            string pic = good.itemDesc.img;
                            if (!pic.StartsWith("http"))
                            {
                                pic = $"http:{pic}";
                            }
                            list.Add(new GetGoodProductModel()
                            {
                                ItemId = good.itemId,
                                Pic = pic,
                                ActionUrl = good.itemDesc.desc[0].href
                            });
                            Logger.WriteInfo($"item get：{good.itemId}, {good.monthlySoldQuantity}, {good.itemDesc.desc[0].text}");
                        }
                        list.OrderBy(s => s.MmonthlySoldQuantity);
                        WeakReferenceMessenger.Default.Send(list,MessengerConstMessage.GetGoodsListToken);
                    }


                    if (json.type == JSFuncType.ReceiveCurrentCsr)
                    {
                        Logger.WriteInfo($"item get：{json}");
                        MJSResult<CurrentCsrModel> mJSResult;
                        //解析
                        mJSResult = JsonConvert.DeserializeObject<MJSResult<CurrentCsrModel>>(JsonConvert.SerializeObject(json));
                        //逗号分割
                        var list = mJSResult.Msg.Nick.Split(':');
                        GlobalCache.StoreName = list[0];
                        if (list.Count() >= 2)
                        {
                            GlobalCache.UserName = list[1];
                        }

                        allSockets[mJSResult.Msg.Nick] = socket;

                        SendJSFunc(JSFuncType.GetCurrentConv);
                    }

                    if (json.type == JSFuncType.ReceiverCurrentConv)
                    {
                        String nick_name = json.msg.nick;
                        String display_name = json.msg.display;
                        string ccode = json.msg.ccode;
                        GlobalCache.CurrentCustomer = new CustomerModel() { UserNiceName = nick_name, UserDisplayName = display_name, CCode = ccode };
                        Logger.WriteInfo($"GetCurrentConv json:{json}");
                    }


                    if (json.type == JSFuncType.ReceiveConvChange)
                    {
                        String nick_name = json.msg.nick;
                        String display_name = json.msg.display;
                        string ccode = json.msg.ccode;
                        GlobalCache.CurrentCustomer = new CustomerModel() { UserNiceName = nick_name, UserDisplayName = display_name, CCode = ccode };
                        Logger.WriteInfo($"conversation changed：{nick_name}");
                    }

                    if (json.type == JSFuncType.ActiveReceiveRemoteHisMsg)
                    {
                        Logger.WriteInfo(message);
                        String user_name = "";
                        String assistant_name = "";
                        String chat_link = null;
                        string apiChatUri = string.Empty;
                        List<JObject> chats = new List<JObject>();

                        List<JObject> messages = new List<JObject>();

                        var msgList = JsonConvert.DeserializeObject<List<dynamic>>(JsonConvert.SerializeObject(json.msg));
                        if (msgList.Count == 0)
                        {
                            WeakReferenceMessenger.Default.Send(string.Empty, MessengerConstMessage.ActiveReceiveRemoteHisMsgHistoryNull);
                            return;
                        }


                        foreach (dynamic msg in json.msg)
                        {
                            dynamic chat = new JObject();
                            dynamic payload = new JObject();

                            if (msg.ext.dep_chain_id != null)
                            {
                                user_name = msg.fromid;
                                chat.role = "user";
                                String chat_content = msg.msg.text == null ? string.Empty : msg.msg.text;
                                chat.content = chat_content;
                                chat.date = msg.msgtime;

                                long unixDate = chat.date;
                                DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                                DateTime date = start.AddMilliseconds(unixDate);

                                if (chat_content.Contains("item.taobao.com/item.htm"))
                                    chat_link = chat_content;

                                payload.content = chat_content;
                                payload.user_nick = msg.fromid;
                                payload.m_time = date.ToString("yyyy-MM-dd HH:mm:ss");
                                payload.direction = 1;
                                payload.csr_nick = msg.toid;
                                payload.template_id = msg.templateId;
                                payload.url_link = chat_link;
                            }
                            else
                            {
                                assistant_name = msg.fromid;
                                chat.role = "assistant";
                                chat.content = msg.msg.text;
                                chat.date = msg.msgtime;
                                long unixDate = chat.date;
                                DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                                DateTime date = start.AddMilliseconds(unixDate);

                                payload.content = chat.content;
                                payload.user_nick = msg.toid;
                                payload.m_time = date.ToString("yyyy-MM-dd HH:mm:ss");
                                payload.direction = 2;
                                payload.csr_nick = msg.fromid;
                                payload.template_id = msg.templateId;
                                payload.url_link = chat_link;
                            }


                            // template text
                            if (msg.templateId == 273001)
                            {
                                chat.content = msg.msg.E0_text + '\n' + msg.msg.E1_text;
                                payload.content = chat.content;
                            }

                            //单个商品
                            if (msg.templateId == 241005)
                            {
                                if (msg.ext.dep_chain_id != null)
                                {
                                    String chat_content = (String)msg.msg.E1_title;
                                    chat.content = chat_content;
                                    chat.date = msg.msgtime;
                                    String chat_actionurl = (String)msg.msg.actionUrl;
                                    if (chat_actionurl.Contains("item.taobao.com/item.htm"))
                                        chat_link = chat_actionurl;
                                    payload.content = chat.content;
                                }
                            }

                            //多个商品
                            if (msg.templateId == 129)
                            {
                                if (msg.ext.dynamic_msg_content[0].templateData.templateId == 295001)
                                    chat.content = msg.ext.dynamic_msg_content[0].templateData.E2_items[0].actionUrl;
                                else if (msg.ext.dynamic_msg_content[0].templateData.templateId == 164002)
                                    chat.content = msg.ext.dynamic_msg_content[0].templateData.E2_actionUrl;
                                else
                                    chat.content = $"未能解析：{msg.ext.dynamic_msg_content[0].templateData.templateId}的数据";

                                chat.date = msg.msgtime;
                                payload.content = chat.content;
                            }


                            apiChatUri = string.IsNullOrEmpty($"{msg.apiChatUri}") ? string.Empty : msg.apiChatUri;

                            messages.Add(payload);
                            chats.Add(chat);
                        }

                        TopHelp.SaveMessage(_httpClient, messages);
                        dynamic aichat = new JObject();

                        //判断
                        aichat.message_history = JArray.FromObject(chats);


                        var _aiUrl = string.Empty;
              
                        if (!string.IsNullOrEmpty(apiChatUri))
                        {
                            switch (apiChatUri)
                            {
                                case AIChatApiList.How2Replay:
                                case AIChatApiList.Want2Reply:
                                case AIChatApiList.ReMultiGood:
                                    _aiUrl = $"{aiURL}{apiChatUri}";
                                    break;
                                case ApiConstToken.MsgRegexToken:
                                    _aiUrl = $"{aiURL}{AIChatApiList.AutoReplay}";
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            //发送错误消息提示
                            WeakReferenceMessenger.Default.Send(string.Empty, MessengerConstMessage.ApiChatHttpErrorToken);
                            return;
                        }
                        string jsonMessage = "";
                        switch (apiChatUri)
                        {
                            case AIChatApiList.How2Replay:
                                How2ReplyModel how2ReplyModel = new How2ReplyModel()
                                {
                                    AssistantName = assistant_name,
                                    MessageHistory = JArray.FromObject(chats),
                                    GoodsName = GlobalCache.IsHaveProduct ? GlobalCache.CurrentProduct.ProductName : null,
                                    GoodsKnowledge = GlobalCache.IsHaveProduct ? GlobalCache.CurrentProduct.ProductInfo : null,
                                    ShopName = GlobalCache.Shop.Name,
                                    IndustryCategory = GlobalCache.Shop.Category.Name,
                                    BrandInfo = GlobalCache.Shop.BrandInfo,
                                    ShopId = GlobalCache.Shop.ID,
                                    Persona = GlobalCache.CurrentPersonaModel.Persona,
                                    SaleMode = GlobalCache.IsItPreSalesCustomerService ? "sale_pre" : "sale_post"
                                };
                                jsonMessage = JsonConvert.SerializeObject(how2ReplyModel);
                                break;
                            case AIChatApiList.Want2Reply:
                                Want2ReplyModel want2ReplyModel = new Want2ReplyModel()
                                {
                                    AssistantName = assistant_name,
                                    MessageHistory = JArray.FromObject(chats),
                                    GoodsName = GlobalCache.IsHaveProduct ? GlobalCache.CurrentProduct.ProductName : null,
                                    GoodsKnowledge = GlobalCache.IsHaveProduct ? GlobalCache.CurrentProduct.ProductInfo : null,
                                    GuideContent = string.IsNullOrEmpty(GlobalCache.CurrentProductWant2ReplyGuideContent) ? null : GlobalCache.CurrentProductWant2ReplyGuideContent,
                                    SaleMode = GlobalCache.IsItPreSalesCustomerService ? "sale_pre" : "sale_post",
                                    ShopName = GlobalCache.Shop.Name,
                                    IndustryCategory = GlobalCache.Shop.Category.Name,
                                    BrandInfo = GlobalCache.Shop.BrandInfo,
                                    ShopId = GlobalCache.Shop.ID,
                                    Persona = GlobalCache.CurrentPersonaModel.Persona
                                };
                                GlobalCache.CurrentProductWant2ReplyGuideContent = null;
                                jsonMessage = JsonConvert.SerializeObject(want2ReplyModel);
                                break;
                            case AIChatApiList.ReMultiGood:
                                Dictionary<string, string> goodsListKnowledge = new Dictionary<string, string>();
                                foreach (var item in GlobalCache.RecommendedPairing)
                                {
                                    if (!goodsListKnowledge.ContainsKey(item.ProductName))
                                    {
                                        goodsListKnowledge.Add(item.ProductName, item.ProductInfo);
                                    }
                                }
                                ReMultiGoodModel reMultiGoodModel = new ReMultiGoodModel()
                                {
                                    AssistantName = assistant_name,
                                    ShopId = GlobalCache.Shop.ID,
                                    BrandInfo = GlobalCache.Shop.BrandInfo,
                                    MessageHistory = JArray.FromObject(chats),
                                    CustomerScene = string.IsNullOrEmpty(GlobalCache.ProductIntroductionCustomerScene) ? null : GlobalCache.ProductIntroductionCustomerScene,
                                    Persona = GlobalCache.CurrentPersonaModel.Persona,
                                    GoodAName = GlobalCache.IsHaveProduct ? GlobalCache.CurrentProduct.ProductName : null,
                                    GoodANameKnowledge = GlobalCache.IsHaveProduct ? GlobalCache.CurrentProduct.ProductInfo : null,
                                    ShopName = GlobalCache.Shop.Name,
                                    GoodsListKnowledge = goodsListKnowledge,
                                    IndustryCategory = GlobalCache.Shop.Category.Name,
                                };
                                GlobalCache.ProductIntroductionCustomerScene = null;
                                jsonMessage = JsonConvert.SerializeObject(reMultiGoodModel);
                                break;
                            case ApiConstToken.MsgRegexToken:
                                //先去匹配通用问题自动回复正则
                                string Regexquestion = string.Empty;

                                //倒叙
                                for (int i = chats.Count - 1; i >= 0; i--)
                                {
                                    string role = chats[i]["role"].ToString();

                                    if (role == "user")
                                    {
                                        //做匹配
                                        Regexquestion = chats[i]["content"].ToString();

                                      
                                        foreach (var item in GlobalCache.QAModels)
                                        {
                                            if (!string.IsNullOrEmpty(item.RegEx))
                                            {
                                                Regex regex = new Regex(item.RegEx);
                                                if (regex.IsMatch(Regexquestion))
                                                {
                                                    if (GlobalCache.AiChatAutoReplyRegex == item.RegEx)
                                                    {
                                                        return;
                                                    }
                                                    GlobalCache.AiChatAutoReplyRegex = item.RegEx;
                                                    //请求大模型
                                                    AutoReplayModel autoReplayModel = new AutoReplayModel()
                                                    {
                                                        Answer = item.Answer,
                                                        GoodsName = GlobalCache.IsHaveProduct ? GlobalCache.CurrentProduct.ProductName : null,
                                                        Question = item.Question
                                                    };
                                                    jsonMessage = JsonConvert.SerializeObject(autoReplayModel);
                                                    GlobalCache.AutoReplyRegex = string.Empty;
                                                    goto continueRequest;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                break;
                            default:
                                break;
                        }
                        if (string.IsNullOrEmpty(jsonMessage))
                        {
                            return;
                        }
                        continueRequest:
                        RequestAiChat(user_name, apiChatUri, _aiUrl, jsonMessage);
                    }

                    if (json.type == "window")
                    {
                        Console.WriteLine(json.msg);
                    }


                    if (json.type == "message")
                    {
                        Logger.WriteInfo(message);
                        String user_name = "";
                        String assistant_name = "";
                        String chat_link = null;
                        string formNickName = "";
                        string toNickName = "";
                        string productMsg = "", sendUserNiceName = "", receiveUserNiceName = "";
                        Int32 msgTemplateId = 0;

                        List<JObject> chats = new List<JObject>();

                        List<JObject> messages = new List<JObject>();

                        foreach (dynamic msg in json.msg)
                        {
                            formNickName = msg.fromid;
                            toNickName = msg.toid;
                            dynamic chat = new JObject();
                            dynamic payload = new JObject();

                            if (msg.ext.dep_chain_id != null)
                            {
                                user_name = msg.fromid;
                                chat.role = "user";
                                String chat_content = msg.msg.text;
                                chat.content = chat_content;
                                chat.date = msg.msgtime;
                                chat.formNickName = formNickName;

                                long unixDate = chat.date;
                                DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                                DateTime date = start.AddMilliseconds(unixDate);

                                if (!string.IsNullOrEmpty(chat_content) && chat_content.Contains("item.taobao.com/item.htm"))
                                    chat_link = chat_content;

                                payload.content = chat_content;
                                payload.user_nick = msg.fromid;
                                payload.m_time = date.ToString("yyyy-MM-dd HH:mm:ss");
                                payload.direction = 1;
                                payload.csr_nick = msg.toid;
                                payload.template_id = msg.templateId;
                                payload.url_link = chat_link;
                            }
                            else
                            {
                                assistant_name = msg.fromid;
                                chat.role = "assistant";
                                chat.content = msg.msg.text;
                                chat.date = msg.msgtime;
                                chat.formNickName = formNickName;
                                long unixDate = chat.date;
                                DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                                DateTime date = start.AddMilliseconds(unixDate);

                                payload.content = chat.content;
                                payload.user_nick = msg.toid;
                                payload.m_time = date.ToString("yyyy-MM-dd HH:mm:ss");
                                payload.direction = 2;
                                payload.csr_nick = msg.fromid;
                                payload.template_id = msg.templateId;
                                payload.url_link = chat_link;
                            }


                            // template text
                            if (msg.templateId == 273001)
                            {
                                chat.content = msg.msg.E0_text + '\n' + msg.msg.E1_text;
                                payload.content = chat.content;
                            }

                            //单个商品
                            if (msg.templateId == 241005 || msg.templateId == 262002)
                            {
                                if (msg.ext.dep_chain_id != null)
                                {
                                    String chat_content = (String)msg.msg.E1_title;
                                    chat.content = chat_content;
                                    chat.date = msg.msgtime;
                                    String chat_actionurl = (String)msg.msg.actionUrl;
                                    if (chat_actionurl.Contains("item.taobao.com/item.htm"))
                                        chat_link = chat_actionurl;
                                    payload.content = chat.content;
                                }
                                productMsg = JsonConvert.SerializeObject(msg.msg);
                                msgTemplateId = msg.templateId;
                                sendUserNiceName = msg.ext.sender_nick;
                                receiveUserNiceName = msg.ext.receiver_nick;
                            }

                            //多个商品
                            if (msg.templateId == 200005 || msg.templateId == 129)
                            {
                                bool isAdd = true;
                                switch ((Int32)msg.templateId)
                                {
                                    case 200005:
                                        productMsg = JsonConvert.SerializeObject(msg.msg.E2_items);
                                        break;
                                    case 129:
                                        if (msg.ext.dynamic_msg_content[0].templateId == 295001)
                                        {
                                            productMsg = JsonConvert.SerializeObject(msg.ext.dynamic_msg_content[0].templateData.E2_items[0]);
                                            sendUserNiceName = msg.ext.sender_nick;
                                            receiveUserNiceName = msg.ext.receiver_nick;
                                        }
                                        else if (msg.ext.dynamic_msg_content[0].templateId == 164002)
                                            isAdd = false;
                                        break;
                                    default:
                                        break;
                                }
                                if (isAdd)
                                {
                                    msgTemplateId = msg.templateId;
                                    sendUserNiceName = msg.ext.sender_nick;
                                    receiveUserNiceName = msg.ext.receiver_nick;
                                }
                            }

                            if (msg.templateId == 101)
                            {
                                //这个时候只拿得到ActionUrl
                                if (msg.msg?.jsview != null && msg.msg.jsview[0].type == 1)
                                {
                                    SingleProductModel singleProduct = new SingleProductModel()
                                    {
                                        ActionUrl = msg.msg.text
                                    };
                                    productMsg = JsonConvert.SerializeObject(singleProduct);
                                    msgTemplateId = msg.templateId;
                                    sendUserNiceName = msg.ext.sender_nick;
                                    receiveUserNiceName = msg.ext.receiver_nick;
                                }
                                if (msg.msg?.jsview != null && msg.msg.jsview[0].type == 5)
                                {
                                    Single101ProductModel model = JsonConvert.DeserializeObject<Single101ProductModel>($"{msg.msg.jsview[0].value.urlinfo}");
                                    SingleProductModel singleProduct = new SingleProductModel()
                                    {
                                        ActionUrl = msg.msg.text,
                                        Pic = model.ImageUrl,
                                        Title = model.Title,
                                    };
                                    productMsg = JsonConvert.SerializeObject(singleProduct);
                                    msgTemplateId = msg.templateId;
                                    sendUserNiceName = msg.ext.sender_nick;
                                    receiveUserNiceName = msg.ext.receiver_nick;
                                }
                            }

                            messages.Add(payload);
                            chats.Add(chat);
                        }

                        TopHelp.SaveMessage(_httpClient, messages);


                        //先去匹配通用问题自动回复正则
                        string Regexquestion = string.Empty;

                        //倒叙
                        for (int i = chats.Count - 1; i >= 0; i--)
                        {
                            string role = chats[i]["role"].ToString();
                            string nickName = chats[i]["formNickName"].ToString();
                            if (role == "user")
                            {
                                //做匹配
                                Regexquestion = chats[i]["content"].ToString();

                                //去匹配正则
                                foreach (var item in GlobalCache.AutoReplyModels)
                                {
                                    if (!string.IsNullOrEmpty(item.RegEx))
                                    {
                                        Regex regex = new Regex(item.RegEx);
                                        if (regex.IsMatch(Regexquestion))
                                        {
                                            //判断当前用户的上一次正则是哪个？
                                            //如果一样的话就不发送
                                            if (GlobalCache.CustomerAutoReplyRegex.ContainsKey(nickName) && GlobalCache.CustomerAutoReplyRegex[nickName] == item.RegEx)
                                            {
                                                WeakReferenceMessenger.Default.Send(string.Empty, MessengerConstMessage.AutoReplyRemindToken);
                                                goto continueProcessing;
                                            }
                                            GlobalCache.CustomerAutoReplyRegex[nickName] = item.RegEx;
                                            var msg = TopHelp.QNSendMsgJS(nickName, item.Answer, (GlobalCache.CustomerCurrentProductList.ContainsKey(nickName) && GlobalCache.CustomerCurrentProductList[nickName] != null) ? GlobalCache.CustomerCurrentProductList[nickName].ProductName : string.Empty);
                                            //发送socket
                                            SendSocket(msg);
                                            WeakReferenceMessenger.Default.Send(item.Answer, MessengerConstMessage.AutoReplyRemindToken);
                                            goto continueProcessing;
                                        }
                                    }
                                }

                                if (nickName == GlobalCache.CurrentCustomer.UserNiceName)
                                {
                                    foreach (var item in GlobalCache.QAModels)
                                    {
                                        if (!string.IsNullOrEmpty(item.RegEx))
                                        {
                                            Regex regex = new Regex(item.RegEx);
                                            if (regex.IsMatch(Regexquestion))
                                            {
                                                if (GlobalCache.AiChatAutoReplyRegex == item.RegEx)
                                                {
                                                    goto continueProcessing;
                                                }
                                                GlobalCache.AiChatAutoReplyRegex = item.RegEx;
                                                //请求大模型
                                                AutoReplayModel autoReplayModel = new AutoReplayModel()
                                                {
                                                    Answer = item.Answer,
                                                    GoodsName = GlobalCache.IsHaveProduct ? GlobalCache.CurrentProduct.ProductName : null,
                                                    Question = item.Question
                                                };
                                                string jsonMessage = JsonConvert.SerializeObject(autoReplayModel);
                                                GlobalCache.AutoReplyRegex = string.Empty;
                                                RequestAiChat(user_name, ApiConstToken.MsgRegexToken, $"{aiURL}{AIChatApiList.AutoReplay}", jsonMessage);
                                                goto continueProcessing;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                goto continueProcessing;
                            }
                        }
                        continueProcessing:
                        int lastTemplateId = json.msg[chats.Count-1].templateId;
                        bool lastTemplateIsProduct =  ProductChatTemplateIdList.Contains(lastTemplateId);

                        if (lastTemplateIsProduct && msgTemplateId != 0)
                        {
                            if (msgTemplateId == 129)
                            {
                                MultipleProductModel multipleModel = JsonConvert.DeserializeObject<MultipleProductModel>(productMsg);
                                multipleModel.TaoBaoID = multipleModel.ItemId;
                                multipleModel.SendUserNiceName = sendUserNiceName.Replace("cntaobao", "");
                                multipleModel.ReceiveUserNiceName = receiveUserNiceName.Replace("cntaobao", "");
                                multipleModel.TaoBaoID = multipleModel.TaoBaoID;
                                if (!multipleModel.Pic.StartsWith("http"))
                                {
                                    multipleModel.Pic = $"http:{multipleModel.Pic}";
                                }
                                WeakReferenceMessenger.Default.Send(multipleModel, MessengerConstMessage.SendMsgMultipleProductToken);
                            }
                            //单个商品
                            else if (msgTemplateId == 241005 || msgTemplateId == 262002 || msgTemplateId == 101)
                            {
                                //文本
                                if (lastTemplateId == 101 && json.msg[chats.Count - 1].msg.jsview[0].type == 0)
                                {
                                    return;
                                }
                                SingleProductModel singleModel = JsonConvert.DeserializeObject<SingleProductModel>(productMsg);
                                singleModel.SendUserNiceName = sendUserNiceName.Replace("cntaobao", "");
                                singleModel.ReceiveUserNiceName = receiveUserNiceName.Replace("cntaobao", "");
                                singleModel.TaoBaoID = StringHelper.GetTaoBaoIDByActionUrl(string.IsNullOrEmpty(singleModel.ActionUrl) ? singleModel.E1ActionUrl : singleModel.ActionUrl);
                                if (!string.IsNullOrEmpty(singleModel.Pic) && !singleModel.Pic.StartsWith("http"))
                                {
                                    singleModel.Pic = $"http:{singleModel.Pic}";
                                }
                                WeakReferenceMessenger.Default.Send(singleModel, MessengerConstMessage.SendMsgSingleProductToken);
                            }
                            else if (msgTemplateId == 200005)
                            {
                                List<MultipleProductModel> multipleModels = JsonConvert.DeserializeObject<List<MultipleProductModel>>(productMsg);
                                for (int i = 0; i < multipleModels.Count; i++)
                                {
                                    multipleModels[i].TaoBaoID = multipleModels[i].ItemId;
                                    multipleModels[i].SendUserNiceName = sendUserNiceName.Replace("cntaobao", ""); ;
                                    multipleModels[i].ReceiveUserNiceName = receiveUserNiceName.Replace("cntaobao", "");;
                                    multipleModels[i].TaoBaoID = multipleModels[i].TaoBaoID;
                                    if (!multipleModels[i].Pic.StartsWith("http"))
                                    {
                                        multipleModels[i].Pic = $"http:{multipleModels[i].Pic}";
                                    }
                                    WeakReferenceMessenger.Default.Send(multipleModels[i], MessengerConstMessage.SendMsgMultipleProductToken);
                                }
                            }
                        }
                    }
                };
            });

        }

        private static void RequestAiChat(string user_name, string apiChatUri, string aiURL, string jsonMessage)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, aiURL);
            requestMessage.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
            ChatTextViewModel chatTextViewModel = null;
            ChatBaseView chatBaseView = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                chatTextViewModel = new ChatTextViewModel();
                chatBaseView = new ChatBaseView()
                {
                    DataContext = new ChatBaseViewModel()
                    {
                        ChatIdentityEnum = Resources.Enumeration.ChatIdentityEnum.Recipient,
                        ContentControl = new ChatTextView()
                        {
                            DataContext = chatTextViewModel
                        }
                    }
                };
            });


            Task.Factory.StartNew(async () =>
            {
                HttpResponseMessage response = null;
                try
                {
                    response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
                }
                catch (Exception ex)
                {
                    Logger.WriteInfo(ex.Message);
                }
                if (response?.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    WeakReferenceMessenger.Default.Send(chatBaseView, MessengerConstMessage.SSESteamReponseToken);
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(stream))
                    {
                        while (!reader.EndOfStream)
                        {

                            string line = await reader.ReadLineAsync();
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                chatTextViewModel.Content += line;
                            }
                        }
                    }

                    string sendMsg = TopHelp.QNSendMsgJS(user_name, chatTextViewModel.Content, GlobalCache.CurrentProduct?.ProductName);
                    string messengerToken = MessengerConstMessage.AskAIResponseToken;
                    switch (apiChatUri)
                    {
                        case AIChatApiList.How2Replay:
                            messengerToken = MessengerConstMessage.AskAIResponseToken;
                            break;
                        case AIChatApiList.Want2Reply:
                            messengerToken = MessengerConstMessage.Want2ReplyResponseToken;
                            break;
                        case AIChatApiList.ReMultiGood:
                            messengerToken = MessengerConstMessage.ReMultiGoodReponseToken;
                            break;
                        case ApiConstToken.MsgRegexToken:
                            //todo
                            messengerToken = MessengerConstMessage.AskAIResponseToken;
                            break;
                        default:
                            break;
                    }
                    WeakReferenceMessenger.Default.Send(sendMsg, messengerToken);
                }
                else
                {
                    //发送错误消息提示
                    WeakReferenceMessenger.Default.Send(string.Empty, MessengerConstMessage.ApiChatHttpErrorToken);
                }
            });
        }

        public static void SendJSFunc(string jsFuncType, string nickName = "", string apiChatUri = "", IWebSocketConnection socket = null)
        {
            dynamic root = new JObject();
            //root.act = "getGoodsList";
            root.act = jsFuncType;
            if (!string.IsNullOrEmpty(nickName))
            {
                root.nickName = nickName;
            }
            if (!string.IsNullOrEmpty(apiChatUri))
            {
                root.apiChatUri = apiChatUri;
            }

            // 将对象转换成JSON字符串
            string jsonString = JsonConvert.SerializeObject(root, Formatting.Indented);
            string csrName = TopHelp.GetQNChatTitle();
            if (socket != null)
                socket.Send(jsonString);
            else if (allSockets.ContainsKey(csrName) && allSockets[csrName] != null)
                allSockets[csrName].Send(jsonString);
        }

        public static void SendSocket(string msg)
        {
            string csrName = TopHelp.GetQNChatTitle();
            if (allSockets.ContainsKey(csrName) && allSockets[csrName] != null)
                allSockets[csrName].Send(msg);
        }

        public static void CloseAll() {
            foreach (var socket in allSockets.Values)
            {
                socket.Close();
            }
            allSockets.Clear();
            //if (wsServer != null )
            //    wsServer.Dispose();
            //if (syNet != null)
            //    syNet = null;
        }
    }
}
