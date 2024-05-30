using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace csr_windows.Client.ViewModels.Customer
{
    public class CustomerInitBottomViewModel :ObservableRecipient
    {

        #region Fields
        private bool _isHaveProduct;

        private string _productName;



        #endregion

        #region Commands
        /// <summary>
        /// 问AI
        /// </summary>
        public ICommand AskAICommand { get; set; }

        /// <summary>
        /// 输入AI
        /// </summary>
        public ICommand InputAICommand { get; set; }

        /// <summary>
        /// 商品介绍
        /// </summary>
        public ICommand ProductIntroductionCommand{ get; set; }

        /// <summary>
        /// 推荐搭配
        /// </summary>
        public ICommand RecommendedPairing { get; set; }

        /// <summary>
        /// 选择商品
        /// </summary>
        public ICommand ChooseProductCommand { get; set; }
        #endregion

        #region Constructro
        public CustomerInitBottomViewModel()
        {
            AskAICommand = new RelayCommand(OnAskAICommand);
            InputAICommand = new RelayCommand(OnInputAICommand);
            ProductIntroductionCommand = new RelayCommand(OnProductIntroductionCommand);
            RecommendedPairing = new RelayCommand(OnRecommendedPairing);
            ChooseProductCommand = new RelayCommand(OnChooseProductCommand);
        }
        

        #endregion

        #region Properties
        /// <summary>
        /// 是否有商品
        /// </summary>
        public bool IsHaveProduct
        {
            get => _isHaveProduct;
            set => SetProperty(ref _isHaveProduct, value);
        }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName
        {
            get => _productName;
            set 
            {
                SetProperty(ref _productName, value); 
                if (string.IsNullOrEmpty(value.Trim()))
                {
                    IsHaveProduct = false;
                }
                else
                {
                    IsHaveProduct = true;
                }
            } 
        }

        private int myVar;

        public int MyProperty
        {
            get { return myVar; }
            set { myVar = value; }
        }


        #endregion

        #region Methods

        /// <summary>
        /// 问AI
        /// </summary>
        private void OnAskAICommand()
        {
        }

        /// <summary>
        /// 给AI输入
        /// </summary>
        private void OnInputAICommand()
        {
        }

        /// <summary>
        /// 商品介绍
        /// </summary>
        private void OnProductIntroductionCommand()
        {
        }

        /// <summary>
        /// 推荐搭配
        /// </summary>
        private void OnRecommendedPairing()
        {
        }

        /// <summary>
        /// 选择商品
        /// </summary>
        private void OnChooseProductCommand()
        {
        }
        #endregion
    }
}
