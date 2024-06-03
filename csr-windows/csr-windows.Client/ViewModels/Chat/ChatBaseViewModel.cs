using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using csr_windows.Resources.Enumeration;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace csr_windows.Client.ViewModels.Chat
{
    public class ChatBaseViewModel : ObservableRecipient
    {

        #region Fields
        private ChatIdentityEnum _chatIdentityEnum;
        private UserControl _contentConTrol;

    

        #endregion

        #region Commands

        #endregion

        #region Constructor
        public ChatBaseViewModel()
        {
            //TODO: 注册添加新聊天记录事件
        }
        #endregion

        #region Properties

        /// <summary>
        /// 聊天身份枚举
        /// </summary>
        public ChatIdentityEnum ChatIdentityEnum
        {
            get => _chatIdentityEnum;
            set => SetProperty(ref _chatIdentityEnum, value);
        }

        /// <summary>
        /// 内容控件
        /// </summary>
        public UserControl ContentControl 
        {
            get => _contentConTrol;
            set => SetProperty(ref _contentConTrol, value);
        }
        #endregion

        #region Methods

        #endregion
    }
}
