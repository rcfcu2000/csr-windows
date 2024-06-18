using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Services.Base;
using csr_windows.Client.Services.WebService;
using csr_windows.Client.Services.WebService.Enums;
using csr_windows.Domain;
using csr_windows.Domain.Enumeration;
using csr_windows.Domain.WeakReferenceMessengerModels;
using csr_windows.Domain.WebSocketModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows.Controls;
using System.Windows.Input;

namespace csr_windows.Client.ViewModels.Customer
{
    public class CustomerInitBottomViewModel :ObservableRecipient
    {

        #region Fields
     


        private IUiService _uIService;


        #endregion

        #region Commands
        /// <summary>
        /// 问AI
        /// </summary>
        public ICommand AskAICommand { get; set; }

        /// <summary>
        /// 输入AI
        /// </summary>
        public ICommand InputAICommand { get; set; }

        /// <summary>
        /// 商品介绍
        /// </summary>
        public ICommand ProductIntroductionCommand{ get; set; }

        /// <summary>
        /// 推荐搭配
        /// </summary>
        public ICommand RecommendedPairingCommand { get; set; }

        /// <summary>
        /// 选择商品
        /// </summary>
        public ICommand ChooseProductCommand { get; set; }

        /// <summary>
        /// 打开商品
        /// </summary>
        public ICommand OpenProductUrlCommand { get; set; } 
        #endregion

        #region Constructro
        public CustomerInitBottomViewModel()
        {
            _uIService = Ioc.Default.GetService<IUiService>();
            AskAICommand = new RelayCommand(OnAskAICommand);
            InputAICommand = new RelayCommand(OnInputAICommand);
            ProductIntroductionCommand = new RelayCommand(OnProductIntroductionCommand);
            RecommendedPairingCommand = new RelayCommand(OnRecommendedPairingCommand);
            ChooseProductCommand = new RelayCommand(OnChooseProductCommand);
            OpenProductUrlCommand = new RelayCommand(OnOpenProductUrlCommand);
        }




        #endregion

        #region Properties




        #endregion

        #region Methods

        /// <summary>
        /// 问AI
        /// </summary>
        private void OnAskAICommand()
        {
            WeakReferenceMessenger.Default.Send<string, string>(string.Empty,MessengerConstMessage.AskAIToken);
        }

        /// <summary>
        /// 给AI输入
        /// </summary>
        private void OnInputAICommand()
        {
            string chat_text = TopHelp.GetQNChatInputText();
            WeakReferenceMessenger.Default.Send<string, string>(chat_text, MessengerConstMessage.Want2ReplyToken);
            //_uIService.OpenCustomerBottomInputAIView();
        }

        /// <summary>
        /// 商品介绍
        /// </summary>
        private void OnProductIntroductionCommand()
        {
            _uIService.OpenChooseProductView(ChooseWindowType.ProductIntroduction);
        }

        /// <summary>
        /// 推荐搭配
        /// </summary>
        private void OnRecommendedPairingCommand()
        {
            if (!GlobalCache.IsHaveProduct)
            {
                WeakReferenceMessenger.Default.Send(new PromptMessageTokenModel("请选择您需要搭配的主商品", true,Resources.Enumeration.PromptEnum.Note), MessengerConstMessage.OpenPromptMessageToken);
                return;
            }
            _uIService.OpenRecommendedPairingView();
        }

        /// <summary>
        /// 选择商品
        /// </summary>
        private void OnChooseProductCommand()
        {
            //WebServiceClient.SendJSFunc(JSFuncType.GetCurrentCsr);
            _uIService.OpenChooseProductView(ChooseWindowType.ChooseProduct);
        }

        /// <summary>
        /// 打开商品链接
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void OnOpenProductUrlCommand()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = GlobalCache.CurrentProduct.ProductUrl, // 在这里替换为你想要打开的网页 URL
                UseShellExecute = true
            });
        }
        #endregion
    }
}
