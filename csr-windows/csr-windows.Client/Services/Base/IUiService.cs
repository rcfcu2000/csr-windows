using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Client.Services.Base
{
    public interface IUiService
    {
        /// <summary>
        /// 打开欢迎界面
        /// </summary>
        void OpenWelcomeView();

        /// <summary>
        /// 打开未启动界面
        /// </summary>
        void OpenNoStartClientView();


    }

    
}
