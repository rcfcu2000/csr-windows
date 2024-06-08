﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using csr_windows.Client.Services.Base;
using csr_windows.Domain.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace csr_windows.Client.ViewModels.Customer
{
    public class ProductIntroductionViewModel:ObservableRecipient
    {
        #region Fields
        private IUiService _uiService;
        private const string _singleProductButtonContent = "帮我介绍一下这个商品";
        private const string _multipleProductButtonContent = "帮我推荐以上商品的搭配";

        private ObservableCollection<MyProduct> _myProductList;
        private bool _isMultipleProduct;
        private string _buttonContent;
        #endregion

        #region Commands

        /// <summary>
        /// 商品推荐
        /// </summary>
        public ICommand ProductIntroductionCommand { get; set; }
        #endregion

        #region Constructor
        public ProductIntroductionViewModel(List<MyProduct> myProducts)
        {
            _uiService = Ioc.Default.GetService<IUiService>();
            ProductIntroductionCommand = new RelayCommand(OnProductIntroductionCommand);

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

        /// <summary>
        /// 商品推荐Command
        /// </summary>
        private void OnProductIntroductionCommand()
        {
            _uiService.OpenCustomerInitBottomView();
        }

        #endregion
    }

 
}