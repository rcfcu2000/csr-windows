using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Client.ViewModels.Chat
{
    public class ChatTextViewModel : ObservableRecipient
    {
        #region Fields
        /// <summary>
        /// 文本内容
        /// </summary>
        private string _content;

   

        #endregion

        #region Commands

        #endregion

        #region Constructor
        public ChatTextViewModel(string content)
        {
            _content = content;
        }
        #endregion

        #region Properties
        /// <summary>
        /// 内容
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
