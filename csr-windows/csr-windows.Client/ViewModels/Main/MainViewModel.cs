using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Services.Base;
using csr_windows.Core;
using csr_windows.Domain;
using csr_windows.Domain.AIChat;
using csr_windows.Domain.Api;
using csr_windows.Domain.BaseModels.BackEnd.Base;
using csr_windows.Domain.BaseModels.BackEnd.QA;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace csr_windows.Client.ViewModels.Main
{
    public class MainViewModel : ObservableRecipient
    {

        private static string ZHGWebURL = "https://www.zhihuige.cc/lmr/#";
        //private static string ZHGWebURL = "http://192.168.2.36:8080/login";

        #region Fields

        private UserControl _mainUserControl;
        private IUiService _uiService;
        private bool _isInIM;

        private bool _isShowDockControl;
        private bool _isShowMainGrid = true;



        #endregion

        #region Commands

        public ICommand OpenAICommand { get; set; }
        public ICommand PersonalDataCommand { get; set; }
        public ICommand AboutCommand { get; set; }



        #endregion

        #region Constructor
        public MainViewModel()
        {
            _uiService = Ioc.Default.GetService<IUiService>();
            WeakReferenceMessenger.Default.Register<UserControl, string>(this, MessengerConstMessage.OpenMainUserControlToken, (r, m) => { ChangeContent(m); });
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.LoginSuccessToken, (r, m) => { IsInIM = true; });
            WeakReferenceMessenger.Default.Register<string, string>(this, MessengerConstMessage.GetQARegexToken, OnGetQARegexToken);
            MainUserControl = new Views.Main.WelcomeView();

            OpenAICommand = new RelayCommand(OnOpenAICommand);
            PersonalDataCommand = new RelayCommand(OnPersonalDataCommand);
           AboutCommand = new RelayCommand(OnAboutCommand);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 主界面的UserControl
        /// </summary>
        public UserControl MainUserControl
        {
            get => _mainUserControl;
            set => SetProperty(ref _mainUserControl, value);
        }


        /// <summary>
        /// 是否是IM界面
        /// </summary>
        public bool IsInIM
        {
            get => _isInIM;
            set => SetProperty(ref _isInIM, value);
        }

        /// <summary>
        /// 是否停靠
        /// </summary>
        public bool IsShowDockControl
        {
            get => _isShowDockControl;
            set => SetProperty(ref _isShowDockControl, value);
        }


        /// <summary>
        /// 是否是展示MainGrid
        /// </summary>
        public bool IsShowMainGrid
        {
            get => _isShowMainGrid;
            set => SetProperty(ref _isShowMainGrid, value);
        }

        #endregion

        #region Methods



        /// <summary>
        /// 切换content
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        private void ChangeContent<T>(T content) where T : UserControl
        {
            MainUserControl = content;
        }



        /// <summary>
        /// 打开ai界面
        /// </summary>
        private void OnOpenAICommand()
        {
            // Start the default browser with the URL
            Process.Start(new ProcessStartInfo
            {
                FileName = ZHGWebURL + "?token=" + GlobalCache.UserToken + "&name=" + GlobalCache.CustomerServiceNickName,
                UseShellExecute = true
            });
        }

        /// <summary>
        /// 个人资料
        /// </summary>
        private void OnPersonalDataCommand()
        {
            _uiService.OpenPersonalDataView();
        }

        /// <summary>
        /// 关于助手
        /// </summary>
        private void OnAboutCommand()
        {
            _uiService.OpenMenuAboutView();
        }


        /// <summary>
        /// 获取QA正则
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="message"></param>
        private async void OnGetQARegexToken(object recipient, string message)
        {
            //调用获取自动回复 接口
            Dictionary<string, object> autoReplykeyValuePairs = new Dictionary<string, object>()
            {
                { "page",1 },
                { "pageSize",500 },
                { "shopId",GlobalCache.Shop.ID }
            };
            string content = await ApiClient.Instance.PostAsync(BackEndApiList.AutoreplayGetList, autoReplykeyValuePairs);
            if (content == string.Empty)
            {
                return;
            }
            BackendBase<BasePaging<BackendAutoReplyModel>> autoReplyModel = JsonConvert.DeserializeObject<BackendBase<BasePaging<BackendAutoReplyModel>>>(content);
            if (autoReplyModel.Code == 0)
            {
                GlobalCache.AutoReplyModels = autoReplyModel.Data.List;
            }
            else
            {
                Logger.WriteError($"OnGetQARegexToken Func AutoreplayGetList RequestErrorCode:{autoReplyModel.Code}");
                Logger.WriteError($"OnGetQARegexToken Func AutoreplayGetList RequestErrorMsg:{autoReplyModel.Msg}");
            }

            GlobalCache.QAModels = new List<QAModel>();

            //调用获取QA 接口
            Dictionary<string, object> QAKeyWord1Pairs = new Dictionary<string, object>()
            {
                { "keyword","1"},//  通用 
                { "page",1 },
                { "pageSize",500 },
                { "shopId",GlobalCache.Shop.ID }
            };
            content = await ApiClient.Instance.PostAsync(BackEndApiList.GetQAList, QAKeyWord1Pairs);
            if (content == string.Empty)
            {
                return;
            }
            BackendBase<BasePaging<QAModel>> qaKeyword1model = JsonConvert.DeserializeObject<BackendBase<BasePaging<QAModel>>>(content);
            if (qaKeyword1model.Code == 0)
            {
                GlobalCache.QAModels.AddRange(qaKeyword1model.Data.List);
            }
            else
            {
                Logger.WriteError($"OnGetQARegexToken Func GetQAList keyword1 RequestErrorCode:{qaKeyword1model.Code}");
                Logger.WriteError($"OnGetQARegexToken Func GetQAList keyword1 RequestErrorMsg:{qaKeyword1model.Msg}");
            }

            Dictionary<string, object> QAKeyWord3Pairs = new Dictionary<string, object>()
            {
                { "keyword","3"},//   行业
                { "page",1 },
                { "pageSize",500 },
                { "shopId",GlobalCache.Shop.ID }
            };
            content = await ApiClient.Instance.PostAsync(BackEndApiList.GetQAList, QAKeyWord3Pairs);
            if (content == string.Empty)
            {
                return;
            }
            BackendBase<BasePaging<QAModel>> qaKeyword1mode3 = JsonConvert.DeserializeObject<BackendBase<BasePaging<QAModel>>>(content);
            if (qaKeyword1mode3.Code == 0)
            {
                GlobalCache.QAModels.AddRange(qaKeyword1mode3.Data.List);
            }
            else
            {
                Logger.WriteError($"OnGetQARegexToken Func GetQAList keyword3 RequestErrorCode:{qaKeyword1mode3.Code}");
                Logger.WriteError($"OnGetQARegexToken Func GetQAList keyword3 RequestErrorMsg:{qaKeyword1mode3.Msg}");
            }
        }

        #endregion




    }
}
