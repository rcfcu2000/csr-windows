using CommunityToolkit.Mvvm.ComponentModel;
using csr_windows.Resources.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Client.ViewModels.Chat
{
    public class ChatWant2ReplyViewModel :ObservableRecipient
    {
        #region Fields

        private string _firstContent = "帮我优化这句话：";

        private string _content;

        private ChatIdentityEnum _chatIdentityEnum;


        #endregion

        #region Commands

        #endregion

        #region Constructor
        public ChatWant2ReplyViewModel()
        {

        }

        public ChatWant2ReplyViewModel(string firstContent)
        {
            FirstContent = firstContent;
        }
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

        public string FirstContent
        {
            get => _firstContent;
            set => SetProperty(ref _firstContent, value);
        }


        public ChatIdentityEnum ChatIdentityEnum
        {
            get => _chatIdentityEnum;
            set => SetProperty(ref _chatIdentityEnum, value);
        }


        #endregion

        #region Methods

        #endregion
    }
}
