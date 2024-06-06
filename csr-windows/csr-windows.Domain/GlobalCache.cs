using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Domain.WebSocketModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace csr_windows.Domain
{
    /// <summary>
    /// 全局缓存
    /// </summary>
    public class GlobalCache
    {

        #region Fields
        /// <summary>
        /// 欢迎语
        /// </summary>
        public const string WelcomeConstString = "您好，我是您的智能AI客服助手～\r\n我会实时跟进您和顾客之间的沟通信息，并且在合适的时间给您提供必要的帮助。\r\n在您和顾客沟通的过程中：\r\n1. 如果您不知不知道接下来应该怎么回复顾客的疑虑，可以点击页面下方的【我该怎么回】按钮，我们会立即帮你写出合理的回复文案，供您使用；\r\n2. 如果您已经有了自己的想法，但感觉文字本身并不太合适，可以点击页面下方的【我想这样回】按钮，我们会立即分析您已经输入的文字，并给出优化建议与优化后的文字，供您使用；\r\n我会尽我所能帮助您解决问题，我们开始吧！";
        private static bool _isItPreSalesCustomerService = true;
        private static string _userName;
        private static string _storeName;
        private static bool _haveCustomer;
        private static CustomerModel _currentCustomer;


        public static Dictionary<string,List<UserControl>> CustomerChatList = new Dictionary<string, List<UserControl>>();

        public static bool IsFollowWindow;

        //静态事件处理属性更改
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
        #endregion

        #region Properties

        /// <summary>
        /// 是否售前客服
        /// </summary>
        public static bool IsItPreSalesCustomerService
        {
            get { return _isItPreSalesCustomerService; }
            set 
            { 
                _isItPreSalesCustomerService = value;
                SetStaticPropertyChanged();
            }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        public static string UserName
        {
            get { return _userName; }
            set 
            { 
                _userName = value;
                SetStaticPropertyChanged();
            }
        }

        /// <summary>
        /// 店铺名
        /// </summary>
        public static string StoreName
        {
            get { return _storeName; }
            set 
            {
                _storeName = value;
                //调用通知
                SetStaticPropertyChanged();
            }
        }


        /// <summary>
        /// 是否当前有客户
        /// </summary>
        public static bool HaveCustomer
        {
            get { return _haveCustomer; }
            set
            {
                _haveCustomer = value;
                SetStaticPropertyChanged();
            }
        }

        /// <summary>
        /// 当前客户
        /// </summary>
        public static CustomerModel CurrentCustomer
        {
            get { return _currentCustomer; }
            set 
            { 
                _currentCustomer = value;
                HaveCustomer = !string.IsNullOrEmpty(value.UserDisplayName);
                WeakReferenceMessenger.Default.Send(value, MessengerConstMessage.ChangeCurrentCustomerToken);
                SetStaticPropertyChanged();
            }
        }




        #endregion

        #region Methods

        public static void SetStaticPropertyChanged([CallerMemberName] string name = "")
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
