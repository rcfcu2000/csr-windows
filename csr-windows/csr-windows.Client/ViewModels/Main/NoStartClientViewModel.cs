using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        #endregion

        #region Commands
        public IRelayCommand StartClientCommand { get; set; }
        #endregion

        #region Constructor
        public NoStartClientViewModel()
        {
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
        }
        #endregion
    }
}
