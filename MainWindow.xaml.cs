using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Fleck;
using Path = System.IO.Path;
using SunnyTest;
using Sunny.UI;
using System.Xml.Linq;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using System.Data.Common;

namespace csr_new
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        static SunnyNet syNet = new SunnyNet();
        public MainWindow()
        {
            InitializeComponent();
            StartHttpsServer();
            StartWebSocketServer();
        }

        static void ModifyHostsFile()
        {
            string hostsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"drivers\etc\hosts");
            string newEntry = "127.0.0.1 iseiya.taobao.com";

            if (!File.ReadAllText(hostsPath).Contains(newEntry))
            {
                File.AppendAllText(hostsPath, Environment.NewLine + newEntry);
                Console.WriteLine("Hosts file updated.");
            }
            else
            {
                Console.WriteLine("Hosts file already contains the entry.");
            }
        }


        static void StartHttpsServer()
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
            }

            //string certPath = @"E:\works\ollydbg\demo\server.pfx"; // Update this path
            //string certPassword = "windows"; // Update this password
            //X509Certificate2 certificate = new X509Certificate2(certPath, certPassword);
            //X509Store store = new X509Store(StoreLocation.LocalMachine);
            //store.Open(OpenFlags.ReadWrite);
            //store.Add(certificate);
            //store.Close();


            //HttpListener listener = new HttpListener();
            //listener.Prefixes.Add("https://*:443/");
            //listener.Prefixes.Add("http://*:444/");
            //listener.Start();

            //listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            //listener.Start();

            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        var context = listener.GetContext();
            //        var response = context.Response;

            //        if (context.Request.Url.AbsolutePath == "public/im_support/aliu.js")
            //        {
            //            string jsContent = File.ReadAllText("aliu.js");
            //            byte[] buffer = Encoding.UTF8.GetBytes(jsContent);
            //            response.ContentType = "application/javascript";
            //            response.ContentLength64 = buffer.Length;
            //            response.OutputStream.Write(buffer, 0, buffer.Length);
            //        }
            //        else
            //        {
            //            string htmlContent = @"
            //        <html>
            //        <head>
            //            <script type='text/javascript'>
            //                var scriptImSupport = document.createElement('script');
            //                scriptImSupport.type = 'text/javascript';
            //                scriptImSupport.src = 'https://g.alicdn.com/bshop/im_lib/0.0.14/app.js';
            //                document.getElementsByTagName('head')[0].appendChild(scriptImSupport);

            //                if (typeof qnFunc == 'undefined') {
            //                    var mScript = document.createElement('script');
            //                    mScript.type = 'text/javascript';
            //                    mScript.src = 'https://127.0.0.1/public/im_support/aliu.js';
            //                    document.getElementsByTagName('head')[0].appendChild(mScript);
            //                }
            //            </script>
            //        </head>
            //        <body></body>
            //        </html>";

            //            byte[] buffer = Encoding.UTF8.GetBytes(htmlContent);
            //            response.ContentType = "text/html";
            //            response.ContentLength64 = buffer.Length;
            //            response.OutputStream.Write(buffer, 0, buffer.Length);
            //        }

            //        response.OutputStream.Close();
            //    }
            //});

            Console.WriteLine("HTTPS server started.");
        }

        static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true; // Accept all certificates
        }

        static void StartWebSocketServer()
        {
            var server = new WebSocketServer("ws://0.0.0.0:50000");
            var allSockets = new List<IWebSocketConnection>();
            server.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine("WebSocket connection opened.");
                    allSockets.Add(socket);
                };

                socket.OnClose = () =>
                {
                    Console.WriteLine("WebSocket connection closed.");
                    allSockets.Remove(socket);
                };

                socket.OnMessage = message =>
                {
                    dynamic json = JsonConvert.DeserializeObject(message);
                    if (json.type == "conv_change")
                    {
                        String nick_name = json.msg.nick;
                        Console.WriteLine($"conversation changed：{nick_name}");
                    }
                    
                    if (json.type == "message")
                    {
                        Console.WriteLine(message);
                        String user_name = "";
                        String assistant_name = "";
                        String chat_link = null;
                        List<JObject> chats = new List<JObject>();

                        foreach (dynamic msg in json.msg) 
                        {
                            dynamic chat = new JObject();

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
                                }
                            }

                            if (msg.ext.dep_chain_id != null)
                            {
                                user_name = msg.fromid;
                                chat.role = "user";
                                String chat_content = msg.msg.text;
                                chat.content = chat_content;
                                chat.date = msg.msgtime;
                                if (chat_content.Contains("item.taobao.com/item.htm"))
                                    chat_link = chat_content;
                            }
                            else
                            {
                                assistant_name = msg.fromid;
                                chat.role = "assistant";
                                chat.content = msg.msg.text;
                                chat.date = msg.msgtime;
                            }
                            chats.Add(chat);
                        }
                        dynamic aichat = new JObject();
                        aichat.user_name = user_name;
                        aichat.assistant_name = assistant_name;
                        if (chat_link != null)
                            aichat.link = chat_link;
                        aichat.all_content = JArray.FromObject(chats);

                        string jsonMessage = JsonConvert.SerializeObject(aichat);

                        string aiURL = "https://www.zhihuige.cc/csrnew/api/chat";                       
                        using (HttpClient client = new HttpClient())
                        {
                            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, aiURL);
                            requestMessage.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");
                            HttpResponseMessage response = client.SendAsync(requestMessage).Result;
                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var responseString = response.Content.ReadAsStringAsync().Result;
                                dynamic aijson = JsonConvert.DeserializeObject(responseString);
                                TopHelp tp = new TopHelp();
                                //tp.QNSendMsgVer912(user_name, (string) aijson.response);
                                string sendMsg = tp.QNSendMsgJS(user_name, (string)aijson.response);
                                //给客户端发消息
                                Console.WriteLine(sendMsg);
                                socket.Send(sendMsg);
                            }
                        }
                    }
                    //allSockets.ToList().ForEach(s => s.Send("{\"data\": " + message + '}'));
                };
            });

            Console.WriteLine("WebSocket server started.");
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // ModifyHostsFile();
            // TopHelp tp = new TopHelp();
            // tp.QNSendMsgVer912("", "test");
            StartHttpsServer();
            StartWebSocketServer();
        }
    }
}
