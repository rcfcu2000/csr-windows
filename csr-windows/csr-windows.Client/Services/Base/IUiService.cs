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

        /// <summary>
        /// 打开第一次设置界面
        /// </summary>
        void OpenFirstSettingView();

        /// <summary>
        /// 打开顾客界面
        /// </summary>
        void OpenCustomerView();

        /// <summary>
        /// 打开关于界面
        /// </summary>
        void OpenMenuAboutView();

    }

    
}
