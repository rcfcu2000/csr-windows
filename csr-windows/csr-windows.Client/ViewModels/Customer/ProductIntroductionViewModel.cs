using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Client.ViewModels.Customer
{
    public class ProductIntroductionViewModel:ObservableRecipient
    {
        #region Fields
        private const string _singleProductButtonContent = "帮我介绍一下这个商品";
        private const string _multipleProductButtonContent = "帮我推荐以上商品的搭配";

        private ObservableCollection<MyProduct> _myProductList;
        private bool _isMultipleProduct;
        private string _buttonContent;
        #endregion

        #region Constructor
        public ProductIntroductionViewModel()
        {
            List<MyProduct> myProducts = new List<MyProduct>();
            for (int i = 0; i < 3; i++)
            {
                myProducts.Add(new MyProduct()
                {
                    ProductImage = "https://pic1.zhimg.com/v2-0dda71bc9ced142bf7bb2d6adbebe4f0_r.jpg?source=1940ef5c",
                    ProductName = $"商品名称 Index:{i}"
                });
            }
            MyProductList = new ObservableCollection<MyProduct>(myProducts);
        }


        #endregion

        #region Properties

        /// <summary>
        /// 是否是多个商品
        /// </summary>
        public bool IsMultipleProduct
        {
            get => _isMultipleProduct;
            set 
            { 
                SetProperty(ref _isMultipleProduct, value); 
                if (value)
                    ButtonContent = _multipleProductButtonContent;
                else
                    ButtonContent = _singleProductButtonContent;
            }
        }

        /// <summary>
        /// 列表
        /// </summary>
        public ObservableCollection<MyProduct> MyProductList
        {
            get => _myProductList;
            set
            {
                SetProperty(ref _myProductList, value);
                if (value.Count > 1)
                    IsMultipleProduct = true;
                else
                    IsMultipleProduct = false;
            }
        }

        /// <summary>
        /// Button内容
        /// </summary>
        public string ButtonContent
        {
            get => _buttonContent;
            set => SetProperty(ref _buttonContent, value);
        }

        #endregion
    }

    public class MyProduct
    {
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
    }
}
