using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Services.WebService.Enums;
using csr_windows.Common.Helper;
using csr_windows.Domain;
using csr_windows.Domain.BaseModels;
using csr_windows.Domain.WebSocketModels;
using Fleck;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Client.Services.WebService
{
    internal static class WebServiceClient
    {
        private static List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>();
        private static SunnyNet syNet = new SunnyNet();
        private static readonly HttpClient _httpClient = new HttpClient();
        public static IWebSocketConnection Socket;
        public static void StartHttpsServer()
        {
            bool sunnyNetIsRun = false;

            syNet.绑定端口(80);
            syNet.绑定端口(443);
            syNet.绑定回调地址(Callback.HTTP回调, null, null, null);
            sunnyNetIsRun = syNet.启动();

            //CertificateManager certManager = new CertificateManager();
            //certManager._载入X509KeyPair("E:\\works\\ollydbg\\demo\\server.crt", "E:\\works\\ollydbg\\demo\\server.key");
            //Console.Write($"server：{certManager.获取ServerName()}");

            //syNet.设置自定义CA证书(certManager);
            //syNet.安装证书();

            //syNet.强制客户端走TCP(true);

            //syNet.进程代理_设置捕获任意进程(true);
            syNet.进程代理_添加进程名("AliRender.exe");
            syNet.进程代理_添加进程名("AliApp.exe");
            bool proxyRun = syNet.进程代理_加载驱动();

            Console.Write($"中间件启动结果：{proxyRun}");
            if (sunnyNetIsRun)
            {
                Console.WriteLine(sunnyNetIsRun);
            }
            else
            {
                Console.WriteLine("启动失败" + syNet.取错误());
                //错误处理
            }

            Console.WriteLine("HTTPS server started.");

        }

        public static void StartWebSocketServer()
        {
            var server = new WebSocketServer("ws://0.0.0.0:50000");
            server.Start(socket =>
            {
                Socket = socket;
                socket.OnOpen = () =>
                {
                    Console.WriteLine("WebSocket connection opened.");
                    allSockets.Add(socket);
                    //启动成功
                    SendJSFunc(JSFuncType.GetCurrentCsr);
                };

                socket.OnClose = () =>
                {
                    Console.WriteLine("WebSocket connection closed.");
                    allSockets.Remove(socket);
                };

                socket.OnMessage = message =>
                {
                    dynamic json = JsonConvert.DeserializeObject(message);
                    if (json.type == "goodsList")
                    {
                        JArray glist = json.msg.data.table.dataSource;
                        foreach (dynamic good in glist)
                        {
                            Console.WriteLine($"item get：{good.itemId}, {good.monthlySoldQuantity}, {good.itemDesc.desc[0].text}");
                        }
                    }


                    if (json.type == JSFuncType.ReceiveCurrentCst)
                    {
                        Console.WriteLine($"item get：{json}");
                        MJSResult<CurrentCsrModel> mJSResult;
                        //解析
                        mJSResult = JsonConvert.DeserializeObject<MJSResult<CurrentCsrModel>>(JsonConvert.SerializeObject(json));
                        //逗号分割
                        var list = mJSResult.Msg.Nick.Split(':');
                        GlobalCache.StoreName = list[0];
                        GlobalCache.UserName = list[1];
                        SendJSFunc(JSFuncType.GetCurrentConv);
                    }

                    if (json.type == JSFuncType.ReceiverCurrentConv)
                    {
                        String nick_name = json.msg.nick;
                        String display_name = json.msg.display;
                        string ccode = json.msg.ccode;
                        GlobalCache.CurrentCustomer = new CustomerModel() { UserNiceName = nick_name, UserDisplayName = display_name, CCode = ccode };
                        Console.WriteLine($"GetCurrentConv json:{json}");
                    }


                    if (json.type == JSFuncType.ReceiveConvChange)
                    {
                        String nick_name = json.msg.nick;
                        String display_name = json.msg.display;
                        string ccode = json.msg.ccode;
                        GlobalCache.CurrentCustomer = new CustomerModel() { UserNiceName = nick_name, UserDisplayName = display_name, CCode = ccode };
                        Console.WriteLine($"conversation changed：{nick_name}");
                    }

                    if (json.type == JSFuncType.ActiveReceiveRemoteHisMsg)
                    {
                        Console.WriteLine(message);
                        String user_name = "";
                        String assistant_name = "";
                        String chat_link = null;
                        string apiChatUri = string.Empty;
                        List<JObject> chats = new List<JObject>();

                        TopHelp tp = new TopHelp();
                        List<JObject> messages = new List<JObject>();

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

                            //TODO ：商品
                            //多个商品
                            if (msg.templateId == 200005)
                            {
                                //遍历获取

                            }

                            apiChatUri = string.IsNullOrEmpty(msg.apiChatUri) ? string.Empty : msg.apiChatUri;

                            messages.Add(payload);
                            chats.Add(chat);
                        }

                        tp.SaveMessage(_httpClient, messages);

                        dynamic aichat = new JObject();

                        aichat.shop_name = "蜡笔派家居旗舰店";
                        aichat.assistant_name = assistant_name;

                        //判断
                        if (chat_link != null)
                            aichat.link = chat_link;
                        if (!string.IsNullOrEmpty(GlobalCache.InputAIContent))
                            aichat.guide_content = GlobalCache.InputAIContent;
                        aichat.message_history = JArray.FromObject(chats);

                        string jsonMessage = JsonConvert.SerializeObject(aichat);

                        string aiURL = "https://www.zhihuige.cc/csrnew/api/chat";
                        if (!string.IsNullOrEmpty(apiChatUri))
                        {
                            aiURL = aiURL + apiChatUri;
                        }
                        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, aiURL);
                        requestMessage.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = _httpClient.SendAsync(requestMessage).Result;
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            var responseString = response.Content.ReadAsStringAsync().Result;
                            dynamic aijson = JsonConvert.DeserializeObject(responseString);
                            //tp.QNSendMsgVer912(user_name, (string) aijson.response);
                            string sendMsg = TopHelp.QNSendMsgJS(user_name, (string)aijson.response);
                            WeakReferenceMessenger.Default.Send(sendMsg, MessengerConstMessage.AskAIResponseToken);
                            //给客户端发消息

                            //Console.WriteLine(sendMsg);
                            //socket.Send(sendMsg);

                            // send message to dingding group chat using webhook
                            //tp.SendDingdingMarkdownMessage((string)aijson.response, chat_link);
                        }
                        else
                        {
                            //发送错误消息提示
                            WeakReferenceMessenger.Default.Send(string.Empty, MessengerConstMessage.ApiChatHttpErrorToken);
                        }
                    }

                    if (json.type == "message")
                    {
                        Console.WriteLine(message);
                        String user_name = "";
                        String assistant_name = "";
                        String chat_link = null;
                        string formNickName = "";
                        string toNickName = "";
                        string productMsg = "", sendUserNiceName = "", receiveUserNiceName = "";
                        Int32 msgTemplateId = 0;

                        List<JObject> chats = new List<JObject>();

                        TopHelp tp = new TopHelp();
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
                                switch ((Int32)msg.templateId)
                                {
                                    case 200005:
                                        productMsg = JsonConvert.SerializeObject(msg.msg.E2_items);
                                        break;
                                    case 129:
                                        productMsg = JsonConvert.SerializeObject(msg.ext.dynamic_msg_content[0].templateData.E2_items);
                                        break;
                                    default:
                                        break;
                                }
                                msgTemplateId = msg.templateId;
                                sendUserNiceName = msg.ext.sender_nick;
                                receiveUserNiceName = msg.ext.receiver_nick;
                            }

                            if (msg.templateId == 101)
                            {
                                //todo:这个时候只拿得到ActionUrl
                                if (msg.msg.jsview[0].type == 1)
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
                                if (msg.msg.jsview[0].type == 5)
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

                        //todo:后面改成服务器地址
                        tp.SaveMessage(_httpClient, messages);
                        if (msgTemplateId != 0)
                        {
                            //单个商品
                            if (msgTemplateId == 241005 || msgTemplateId == 262002 || msgTemplateId == 101)
                            {
                                SingleProductModel singleModel = JsonConvert.DeserializeObject<SingleProductModel>(productMsg);
                                singleModel.SendUserNiceName = sendUserNiceName.Replace("cntaobao", ""); ;
                                singleModel.ReceiveUserNiceName = receiveUserNiceName.Replace("cntaobao", ""); ;
                                singleModel.TaoBaoID = StringHelper.GetTaoBaoIDByActionUrl(string.IsNullOrEmpty(singleModel.ActionUrl) ? singleModel.E1ActionUrl : singleModel.ActionUrl);
                                if (!string.IsNullOrEmpty(singleModel.Pic) && !singleModel.Pic.StartsWith("http"))
                                {
                                    singleModel.Pic = $"http:{singleModel.Pic}";
                                }
                                WeakReferenceMessenger.Default.Send(singleModel, MessengerConstMessage.SendMsgSingleProductToken);
                            }
                            if (msgTemplateId == 200005 || msgTemplateId == 129)
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

            Console.WriteLine("WebSocket server started.");
        }

        public static void SendJSFunc(string jsFuncType, string nickName = "", string apiChatUri = "")
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
                root.apiChatUri = "";
            }

            // 将对象转换成JSON字符串
            string jsonString = JsonConvert.SerializeObject(root, Formatting.Indented);

            foreach (var socket in allSockets.ToList())
            {
                socket.Send(jsonString);
            }
        }

        public static void SendSocket(string msg)
        {
            Socket.Send(msg);
        }
    }
}
