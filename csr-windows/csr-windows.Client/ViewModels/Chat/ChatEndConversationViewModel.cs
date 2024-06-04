using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace csr_windows.Client.ViewModels.Chat
{
    public class ChatEndConversationViewModel : ObservableRecipient
    {
        #region Fields

        #endregion

        #region Commands

        /// <summary>
        /// 结束对话
        /// </summary>
        public ICommand EndConversationCommand { get; set; }
        #endregion

        #region Constructor
        public ChatEndConversationViewModel()
        {
            EndConversationCommand = new RelayCommand(OnEndConversationCommand);
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        private void OnEndConversationCommand()
        {
        }
        #endregion
    }
}
