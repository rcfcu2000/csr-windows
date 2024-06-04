using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace csr_windows.Client.ViewModels.Public
{
    public class LogoutViewModel : ObservableRecipient
    {

        #region Fields
        #endregion

        #region Commands
        /// <summary>
        /// 退出命令
        /// </summary>
        public ICommand ExitCommand { get; set; }
        /// <summary>
        /// 继续使用
        /// </summary>
        public ICommand ContinueToUseCommand{ get; set; }
        #endregion

        #region Constructor
        public LogoutViewModel()
        {
            ExitCommand = new RelayCommand(OnExitCommand);
            ContinueToUseCommand = new RelayCommand(OnContinueToUseCommand);
        }


        #endregion

        #region Properties

        #endregion

        #region Methods
        /// <summary>
        /// 退出命令
        /// </summary>
        private void OnExitCommand()
        {
            WeakReferenceMessenger.Default.Send(string.Empty,MessengerConstMessage.ExitToken);
        }

        /// <summary>
        /// 继续使用
        /// </summary>
        private void OnContinueToUseCommand()
        {
            WeakReferenceMessenger.Default.Send(string.Empty, MessengerConstMessage.CloseLogoutViewToken);
        }

        #endregion
    }
}
