using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.WebSocketModels
{
    public class CustomerModel : ObservableRecipient
    {
        #region Fields

        private string _userNiceName;
        private string _userDisplayName;
        private string _ccode;

        #endregion

        #region Properties

        /// <summary>
        /// 用户真实姓名
        /// </summary>
        public string UserNiceName
        {
            get => _userNiceName;
            set => SetProperty(ref _userNiceName, value);
        }

        /// <summary>
        /// 用户显示名称  
        /// </summary>
        public string UserDisplayName
        {
            get => _userDisplayName;
            set => SetProperty(ref _userDisplayName, value);
        }



        public string CCode
        {
            get => _ccode;
            set => SetProperty(ref _ccode, value);
        }

        #endregion

        #region MyRegion

        #endregion
    }
}
