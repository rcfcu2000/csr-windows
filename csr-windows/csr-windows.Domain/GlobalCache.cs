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
