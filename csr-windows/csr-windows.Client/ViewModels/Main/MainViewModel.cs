using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Services.Base;
using csr_windows.Domain;
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
        #endregion




    }
}
