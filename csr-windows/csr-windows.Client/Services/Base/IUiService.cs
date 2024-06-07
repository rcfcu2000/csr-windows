using csr_windows.Domain.Common;
using csr_windows.Domain.Enumeration;
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

        /// <summary>
        /// 打开商品推荐
        /// </summary>
        void OpenRecommendedPairingView();


        /// <summary>
        /// 打开个人资料界面
        /// </summary>
        void OpenPersonalDataView();

        /// <summary>
        /// 打开选择商品界面
        /// </summary>
        void OpenChooseProductView(ChooseWindowType chooseWindowType);

        /// <summary>
        /// 打开产品介绍界面
        /// </summary>
        /// <param name="myProducts"></param>
        void OpenProductIntroductionView(List<MyProduct> myProducts);

        /// <summary>
        /// 顾客初始化底部界面
        /// </summary>
        void OpenCustomerInitBottomView();

        /// <summary>
        /// 打开 我想这样回界面
        /// </summary>
        void OpenCustomerBottomInputAIView();
    }

    
}
