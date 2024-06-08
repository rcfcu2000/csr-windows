using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Services.Base;
using csr_windows.Client.Services.WebService.Enums;
using csr_windows.Client.Services.WebService;
using csr_windows.Domain;
using csr_windows.Domain.WeakReferenceMessengerModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using csr_windows.Domain.Api;

namespace csr_windows.Client.ViewModels.Customer
{
    public class CustomerBottomInputAIViewModel : ObservableRecipient
    {

        #region Fields
        private string _content;
        private IUiService _uiService;
        

        #endregion

        #region Commands

        /// <summary>
        /// 输入AI
        /// </summary>
        public ICommand InputAICommand { get; set; }

        /// <summary>
        /// 返回上一步
        /// </summary>
        public ICommand BackCommand { get; set; }
        #endregion

        #region Constructor
        public CustomerBottomInputAIViewModel()
        {
            _uiService = Ioc.Default.GetService<IUiService>();
            InputAICommand = new RelayCommand(OnInputAICommand);

            BackCommand = new RelayCommand(() => { _uiService.OpenCustomerInitBottomView(); });
        }



        #endregion

        #region Properties
        /// <summary>
        /// 文本输入框内容
        /// </summary>
        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }
        #endregion

        #region Methods

        /// <summary>
        /// 输入AI
        /// </summary>
        private void OnInputAICommand()
        {
            if (string.IsNullOrWhiteSpace(Content))
            {
                WeakReferenceMessenger.Default.Send(new PromptMessageTokenModel("请输入您想回复的内容", false), MessengerConstMessage.OpenPromptMessageToken);

                return;
            }
            //调用接口(发送给大模型)
            _uiService.OpenCustomerInitBottomView();
            WeakReferenceMessenger.Default.Send(Content,MessengerConstMessage.Want2ReplyToken);
        }


        #endregion


    }
}
