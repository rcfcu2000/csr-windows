using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Domain.Common;
using csr_windows.Domain.Enumeration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace csr_windows.Client.ViewModels.Menu
{
    public class ChooseProductViewModel : ObservableRecipient
    {
        #region Fields

        private bool _isSearch;
        private bool _isSearchNull;
        private string _searchContent;

        private bool _isSearchResult;
        private int _searchProductNum;

        private bool _haveDialogueProduct;

        private ObservableCollection<MyProduct> _searchProducts;
        private List<MyProduct> storeProducts = new List<MyProduct>();

        private ChooseWindowType _chooseWindowType;



        #endregion

        #region Commands
        /// <summary>
        /// 关闭命令
        /// </summary>
        public ICommand CloseCommand { get; set; }
        /// <summary>
        /// 搜索
        /// </summary>
        public ICommand SearchCommand { get; set; }

        /// <summary>
        /// 选择
        /// </summary>
        public ICommand ChooseCommand { get; set; }


        #endregion

        #region Constructor
        public ChooseProductViewModel(ChooseWindowType chooseWindowType)
        {
            CloseCommand = new RelayCommand(OnCloseCommand);
            SearchCommand = new RelayCommand(OnSearchCommand);
            ChooseCommand = new RelayCommand<MyProduct>(OnChooseCommand);


            #region 测试代码

            for (int i = 0; i < 5; i++)
            {
                DialogueProducts.Add(new MyProduct()
                {
                    ProductImage = "https://pic1.zhimg.com/v2-0dda71bc9ced142bf7bb2d6adbebe4f0_r.jpg?source=1940ef5c",
                    ProductName = $"对话商品 Index:{i}"
                });
            }


            for (int i = 0; i < 15; i++)
            {
                HotSellingProducts.Add(new MyProduct()
                {
                    ProductImage = "https://pic1.zhimg.com/v2-0dda71bc9ced142bf7bb2d6adbebe4f0_r.jpg?source=1940ef5c",
                    ProductName = $"热销商品 Index:{i}"
                });
            }

            storeProducts.AddRange(DialogueProducts);
            storeProducts.AddRange(HotSellingProducts);
            #endregion

        }



        #endregion

        #region Properties
        /// <summary>
        /// 是否是搜索内容中
        /// </summary>
        public bool IsSearch
        {
            get => _isSearch;
            set => SetProperty(ref _isSearch, value);
        }

        /// <summary>
        /// 是否是搜索为空
        /// </summary>
        public bool IsSearchNull
        {
            get => _isSearchNull;
            set => SetProperty(ref _isSearchNull, value);
        }

        /// <summary>
        /// 搜索内容
        /// </summary>
        public string SearchContent
        {
            get => _searchContent;
            set => SetProperty(ref _searchContent, value);
        }

        /// <summary>
        /// 是否有搜索结果
        /// </summary>
        public bool IsSearchResult
        {
            get => _isSearchResult;
            set => SetProperty(ref _isSearchResult, value);
        }

        /// <summary>
        /// 搜索到的数量
        /// </summary>
        public int SearchProductNum
        {
            get => _searchProductNum;
            set => SetProperty(ref _searchProductNum, value);
        }

        /// <summary>
        /// 是否有对话中的商品
        /// </summary>
        public bool HaveDialogueProduct
        {
            get => _haveDialogueProduct;
            set => SetProperty(ref _haveDialogueProduct, value);
        }

        /// <summary>
        /// 选择窗口的Type类型
        /// </summary>
        public ChooseWindowType ChooseWindowType
        {
            get => _chooseWindowType;
            set => SetProperty(ref _chooseWindowType, value);
        }


        /// <summary>
        /// 对话中提到的商品
        /// </summary>
        public IList<MyProduct> DialogueProducts { get; } = new ObservableCollection<MyProduct>();

        /// <summary>
        /// 热销产品
        /// </summary>
        public IList<MyProduct> HotSellingProducts { get; } = new ObservableCollection<MyProduct>();

        /// <summary>
        /// 搜索的商品
        /// </summary>
        public ObservableCollection<MyProduct> SearchProducts
        {
            get => _searchProducts;
            set => SetProperty(ref _searchProducts, value);
        }


        #endregion

        #region Methods

        /// <summary>
        /// 关闭命令
        /// </summary>
        private void OnCloseCommand()
        {
            WeakReferenceMessenger.Default.Send("", MessengerConstMessage.CloseMenuUserControlToken);
        }

        /// <summary>
        /// 搜索命令
        /// </summary>
        private void OnSearchCommand()
        {
            IsSearch = string.IsNullOrEmpty(SearchContent.Trim()) ? false : true;
            var matchingProducts = storeProducts.Where(p => p.ProductName.Contains(SearchContent)).ToList();
            IsSearchResult = matchingProducts.Count > 0;
            SearchProducts = new ObservableCollection<MyProduct>(matchingProducts);
            SearchProductNum = matchingProducts.Count;
        }

        /// <summary>
        /// 选择命令
        /// </summary>
        /// <param name="product"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnChooseCommand(MyProduct product)
        {
            

        }

        #endregion
    }
}
