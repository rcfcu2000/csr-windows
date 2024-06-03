using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace csr_windows.Client.ViewModels.Menu
{
    public class AboutViewModel : ObservableRecipient
    {

        #region Fields
        private string _version;

        #endregion

        #region Commands
        /// <summary>
        /// 检查版本
        /// </summary>
        public ICommand CheckVersionCommand { get; set; }

        /// <summary>
        /// 关闭
        /// </summary>
        public ICommand CloseCommand { get; set; }

        #endregion

        #region Constructor
        public AboutViewModel()
        {
            CheckVersionCommand = new RelayCommand(OnCheckVersionCommand);
            CloseCommand = new RelayCommand(() => 
            {
                WeakReferenceMessenger.Default.Send("", MessengerConstMessage.CloseMenuUserControlToken);
            });
        }


        #endregion

        #region Properties

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
        }
        #endregion

        #region Methods

        /// <summary>
        /// 检查更新
        /// </summary>
        private void OnCheckVersionCommand()
        {
        
        }

        #endregion
    }
}
