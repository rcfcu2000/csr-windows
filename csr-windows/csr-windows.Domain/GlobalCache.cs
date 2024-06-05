using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain
{
    /// <summary>
    /// 全局缓存
    /// </summary>
    public class GlobalCache
    {

        #region Fields
        private static bool _isItPreSalesCustomerService = true;


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



        #endregion

        #region Methods

        public static void SetStaticPropertyChanged([CallerMemberName] string name = "")
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(name)));
        }

        #endregion
    }
}
