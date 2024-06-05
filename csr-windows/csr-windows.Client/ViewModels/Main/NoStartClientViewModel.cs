using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using csr_windows.Client.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Client.ViewModels.Main
{
    public class NoStartClientViewModel : ObservableRecipient
    {
        #region Fields
        private IUiService _uiService;
        #endregion

        #region Commands
        public IRelayCommand StartClientCommand { get; set; }
        #endregion

        #region Constructor
        public NoStartClientViewModel()
        {
            _uiService = Ioc.Default.GetService<IUiService>();
            StartClientCommand = new RelayCommand(OnStartClientCommand);
        }


        #endregion

        #region Properties
        #endregion

        #region Methods
        /// <summary>
        /// 启动千牛客户端
        /// </summary>
        private void OnStartClientCommand()
        {
            _uiService.OpenFirstSettingView();
        }
        #endregion
    }
}
