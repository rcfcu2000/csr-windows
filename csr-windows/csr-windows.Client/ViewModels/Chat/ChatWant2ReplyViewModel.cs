using CommunityToolkit.Mvvm.ComponentModel;
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

        private string _content;


        #endregion

        #region Commands

        #endregion

        #region Constructor
        public ChatWant2ReplyViewModel()
        {

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

        #endregion

        #region Methods

        #endregion
    }
}
