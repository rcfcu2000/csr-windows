using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace csr_windows.Client.ViewModels.Customer
{
    public class CustomerBottomAskAIViewModel : ObservableRecipient
    {

        #region Fields

        #endregion

        #region Commands

        /// <summary>
        /// 输入AI
        /// </summary>
        public ICommand InputAICommand { get; set; }

        #endregion

        #region Constructor
        public CustomerBottomAskAIViewModel()
        {
            InputAICommand = new RelayCommand(OnInputAICommand);
        }


        #endregion

        #region Properties

        #endregion

        #region Methods

        /// <summary>
        /// 输入AI
        /// </summary>
        private void OnInputAICommand()
        {
        }
        #endregion


    }
}
