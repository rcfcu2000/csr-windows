using CommunityToolkit.Mvvm.ComponentModel;
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

        #endregion
    }
}
