using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace csr_windows.Client.ViewModels.Menu
{
    public class BaseMenuViewModel : ObservableRecipient
    {

        #region Fields
        private UserControl _contentControl;

        #endregion

        #region Commands

        #endregion

        #region Constructor

        public BaseMenuViewModel()
        {
             WeakReferenceMessenger.Default.Register<UserControl, string>(this, MessengerConstMessage.OpenMenuUserControlToken, (r, m) => { ChangeContent(m); });
        }

        #endregion

        #region Properties

        /// <summary>
        /// 内容模块
        /// </summary>
        public UserControl ContentControl
        {
            get => _contentControl;
            set => SetProperty(ref _contentControl, value);
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
            ContentControl = content;
        }

        #endregion
    }
}
