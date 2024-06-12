using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Domain.BaseModels.BackEnd;
using csr_windows.Domain.Common;
using csr_windows.Domain.WebSocketModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private static readonly Lazy<GlobalCache> instance = new Lazy<GlobalCache>(() => new GlobalCache());
        public static GlobalCache Instance => instance.Value;
        #region Fields
        /// <summary>
        /// 欢迎语
        /// </summary>
        public static string WelcomeConstString
        {
            get 
            { 
                return $"您好，我是{(GlobalCache.CurrentPersonaModel == null ? "晓晓" : GlobalCache.CurrentPersonaModel.Name)}～\r\n我会实时跟进您和顾客之间的沟通信息，并且在合适的时间给您提供必要的帮助。\r\n在您和顾客沟通的过程中：\r\n1. 如果您不知不知道接下来应该怎么回复顾客的疑虑，可以点击页面下方的【我该怎么回】按钮，我们会立即帮你写出合理的回复文案，供您使用；\r\n2. 如果您已经有了自己的想法，但感觉文字本身并不太合适，可以点击页面下方的【我想这样回】按钮，我们会立即分析您已经输入的文字，并给出优化建议与优化后的文字，供您使用；\r\n我会尽我所能帮助您解决问题，我们开始吧！"; 
            }
        }


        private static bool _isItPreSalesCustomerService = true;
        private static string _userName;
        private static string _storeName;
        private static bool _haveCustomer;
        private static CustomerModel _currentCustomer;
        private static MyProduct _currentProduct;
        private static MyProduct _productIntroduction;
        private static List<MyProduct> _recommendedPairingProduct;

        
        private static bool _isHaveProduct;
        private static string _customerServiceNickName;

        /// <summary>
        /// 当前商品想要回复 的guide_content
        /// </summary>
        public static string CurrentProductWant2ReplyGuideContent;

        /// <summary>
        /// 商品推荐的输入场景内容
        /// </summary>
        public static string ProductIntroductionCustomerScene;

        private static bool _haveStoreName;
        private static ShopModel _store;

        private static PersonaModel _currentPersonaModel;


        /// <summary>
        /// 每个客户的聊天记录
        /// </summary>
        public static Dictionary<string,List<UserControl>> CustomerChatList = new Dictionary<string, List<UserControl>>();

        /// <summary>
        /// 每个客户的当前
        /// </summary>
        public static Dictionary<string,MyProduct> CustomerCurrentProductList = new Dictionary<string,MyProduct>();


        /// <summary>
        /// 对话中的列表
        /// </summary>
        public static Dictionary<string, List<MyProduct>> CustomerDialogueProducts = new Dictionary<string, List<MyProduct>>();

        /// <summary>
        /// 热销列表
        /// </summary>
        public static List<MyProduct> HotSellingProducts = new List<MyProduct>();


        /// <summary>
        /// 是否跟随了千牛的Window
        /// </summary>
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
                HaveStoreName = !string.IsNullOrEmpty(value);
                //调用通知
                SetStaticPropertyChanged();
            }
        }

        /// <summary>
        /// 店铺
        /// </summary>
        public static ShopModel shop
        {
            get { return _store; }
            set
            {
                _store = value;
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
        /// 是否获取到了店铺名称
        /// </summary>
        public static bool HaveStoreName
        {
            get { return _haveStoreName; }
            set 
            {
                _haveStoreName = value;
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



        /// <summary>
        /// 当前产品
        /// </summary>
        public static MyProduct CurrentProduct
        {
            get => _currentProduct;
            set
            {
                _currentProduct = value;
                if (value == null || string.IsNullOrEmpty(value.ProductName))
                {
                    IsHaveProduct = false;
                }
                else
                {
                    IsHaveProduct = true;
                }
                SetStaticPropertyChanged();
            }
        }

        /// <summary>
        /// 商品介绍的商品
        /// </summary>
        public static MyProduct ProductIntroduction
        {
            get 
            {
                var value = _productIntroduction;
                ProductIntroduction = null;
                return value;
            }
            set 
            { 
                _productIntroduction = value;
                SetStaticPropertyChanged();
            }
        }

        /// <summary>
        /// 商品推荐列表
        /// </summary>
        public static List<MyProduct> RecommendedPairing
        {
            get { return _recommendedPairingProduct; }
            set { _recommendedPairingProduct = value; }
        }


        /// <summary>
        /// 是否有商品
        /// </summary>
        public static bool IsHaveProduct
        {
            get => _isHaveProduct;
            set
            {
                _isHaveProduct = value;
                SetStaticPropertyChanged();
            }
        }

        /// <summary>
        /// 当前人设
        /// </summary>
        public static PersonaModel CurrentPersonaModel
        {
            get { return _currentPersonaModel; }
            set 
            { 
                _currentPersonaModel = value;
                SetStaticPropertyChanged();
            }
        }

        public static string CustomerServiceNickName
        {
            get { return $"{GlobalCache.StoreName}:{GlobalCache.UserName}"; }
            set { _customerServiceNickName = value; }
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
