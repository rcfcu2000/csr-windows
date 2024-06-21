using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Services.WebService;
using csr_windows.Domain;
using csr_windows.Domain.WeakReferenceMessengerModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace csr_windows.Client.ViewModels.Chat
{
    public class ChatCopyTextViewModel : ObservableRecipient
    {
        #region Fields
        private bool _isHaveProduct;

        private string _productName;

        private const string CopyContentMessage = "已复制粘贴到输入框";
        private const string SendContentMessage = "已发送给顾客";

        private string _allContent;
        #endregion

        #region Commands
        /// <summary>
        /// 单条消息复制
        /// </summary>
        public ICommand SingleChatCopyCommand { get; set; }
        /// <summary>
        /// 单条消息发送
        /// </summary>
        public ICommand SingleChatSendCommand { get; set; }
        /// <summary>
        /// 发送所有消息
        /// </summary>
        public ICommand SendAllCommand { get; set; }
        /// <summary>
        /// 复制所有消息
        /// </summary>
        public ICommand CopyAllCommand { get; set; }

        /// <summary>
        /// 选择商品命令
        /// </summary>
        public ICommand ChooseProductCommand { get; set; }
        #endregion

        #region Constructor
        public ChatCopyTextViewModel(List<ChatTestModel> chatTestModels)
        {
            SingleChatCopyCommand = new RelayCommand<ChatTestModel>(OnSingleChatCopyCommand);
            SingleChatSendCommand = new RelayCommand<ChatTestModel>(OnSingleChatSendCommand);
            SendAllCommand = new RelayCommand(OnSendAllCommand);
            CopyAllCommand = new RelayCommand(OnCopyAllCommand);

            ChooseProductCommand = new RelayCommand(OnChooseProductCommand);

            ChatTestModels = new ObservableCollection<ChatTestModel>(chatTestModels);
        }

    


        #endregion

        #region Properties
        /// <summary>
        /// 聊天内容测试列表
        /// </summary>
        public IList<ChatTestModel> ChatTestModels { get; } = new ObservableCollection<ChatTestModel>();

        /// <summary>
        /// 是否有商品
        /// </summary>
        public bool IsHaveProduct
        {
            get => _isHaveProduct;
            set => SetProperty(ref _isHaveProduct, value);
        }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName
        {
            get => _productName;
            set => SetProperty(ref _productName, value);
        }

        /// <summary>
        /// 所有Content
        /// </summary>
        public string AllContent
        {
            get => _allContent;
            set => SetProperty(ref _allContent, value);
        }
        #endregion

        #region Methods

        /// <summary>
        /// 单个聊天复制
        /// </summary>
        /// <param name="model"></param>
        private void OnSingleChatCopyCommand(ChatTestModel model)
        {
            CopyContent(model.Content);
        }

        /// <summary>
        /// 单个聊天发送
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnSingleChatSendCommand(ChatTestModel model)
        {
            SendContent(model.Content);
        }

        /// <summary>
        /// 发送所有消息
        /// </summary>
        private void OnSendAllCommand()
        {
            SendContent(AllContent);
        }

        /// <summary>
        /// 复制所有消息
        /// </summary>
        private void OnCopyAllCommand()
        {
            CopyContent(AllContent);
        }


        /// <summary>
        /// 选择商品命令
        /// </summary>
        private void OnChooseProductCommand()
        {
        }

        private void CopyContent(string content)
        {
            //弹提示框
            WeakReferenceMessenger.Default.Send(new PromptMessageTokenModel(CopyContentMessage,true), MessengerConstMessage.OpenPromptMessageToken);
            //复制到剪切板
            TopHelp.QNSendMsgVer912(GlobalCache.CurrentCustomer.UserNiceName,content);
        }

        private void SendContent(string content)
        {
            //弹提示框
            WeakReferenceMessenger.Default.Send(new PromptMessageTokenModel(SendContentMessage, true), MessengerConstMessage.OpenPromptMessageToken);
            //发送消息
            var msg = TopHelp.QNSendMsgJS(GlobalCache.CurrentCustomer.UserNiceName, content, GlobalCache.CurrentProduct?.ProductName);
            WebServiceClient.SendSocket(msg,GlobalCache.CustomerServiceNickName);

        }

        #endregion
    }

    public class ChatTestModel : ObservableRecipient
    {

        #region Fields

        private string _content;

        private bool _isLast;

        #endregion

        #region Properties
        /// <summary>
        /// 文本内容
        /// </summary>
        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        /// <summary>
        /// 是否是最后一个
        /// </summary>
        public bool IsLast
        {
            get => _isLast;
            set => SetProperty(ref _isLast, value);
        }
        #endregion

    }
}
