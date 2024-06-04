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
    public class ChatSwitchProductsViewModel : ObservableRecipient
    {
        #region Fields

        #endregion

        #region Commands
        public ICommand CheckProductCommand { get; set; }
        #endregion

        #region Constrctor
        public ChatSwitchProductsViewModel()
        {
            CheckProductCommand = new RelayCommand(OnCheckProductCommand);
        }

        #endregion

        #region Properties

        #endregion

        #region Methods
        private void OnCheckProductCommand()
        {
        }

        #endregion
    }
}
