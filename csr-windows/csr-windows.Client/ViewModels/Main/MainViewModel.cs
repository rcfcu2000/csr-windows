using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace csr_windows.Client.ViewModels.Main
{
    public class MainViewModel : ObservableRecipient
    {

        #region Fields

        private UserControl _mainUserControl;

        #endregion

        #region Constructor
        public MainViewModel()
        {
            WeakReferenceMessenger.Default.Register<UserControl, string>(this, MessengerConstMessage.OpenMainUserControlToken, (r, m) => { ChangeContent(m); });
            MainUserControl = new Views.Main.WelcomeView();
        }
        #endregion

        #region Methods

        /// <summary>
        /// 主界面的UserControl
        /// </summary>
        public UserControl MainUserControl
        {
            get => _mainUserControl;
            set => SetProperty(ref _mainUserControl, value);
        }

        /// <summary>
        /// 切换content
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        private void ChangeContent<T>(T content) where T : UserControl
        {
            MainUserControl = content;
        }
        #endregion




    }
}
