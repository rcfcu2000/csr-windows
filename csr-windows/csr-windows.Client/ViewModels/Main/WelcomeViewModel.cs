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

namespace csr_windows.Client.ViewModels.Main
{
    public class WelcomeViewModel : ObservableRecipient
    {
        #region Fields
        private IUiService _uiService;
        private string _version = "1.0";

        #endregion

        #region Commands
        public IRelayCommand UseCommand { get; set; }
        #endregion

        #region Constructor
        public WelcomeViewModel()
        {
            _uiService = Ioc.Default.GetService<IUiService>();
            UseCommand = new RelayCommand(OnUseCommand);
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
        private void OnUseCommand()
        {
            WeakReferenceMessenger.Default.Send("", MessengerConstMessage.LoginSuccessToken);
            _uiService.OpenCustomerView();
        }
        #endregion
    }
}
