using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Domain;
using csr_windows.Domain.Common;
using csr_windows.Domain.Enumeration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
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
        private bool _searchToMore;

        private bool _haveDialogueProduct;

        private ObservableCollection<MyProduct> _searchProducts = new ObservableCollection<MyProduct>();
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
            ChooseWindowType = chooseWindowType;
            CloseCommand = new RelayCommand(OnCloseCommand);
            SearchCommand = new RelayCommand(OnSearchCommand);
            ChooseCommand = new RelayCommand<MyProduct>(OnChooseCommand);


            #region 测试代码

            if (GlobalCache.HaveCustomer && GlobalCache.CustomerDialogueProducts.ContainsKey(GlobalCache.CurrentCustomer.UserNiceName))
            {
                HaveDialogueProduct = GlobalCache.CustomerDialogueProducts[GlobalCache.CurrentCustomer.UserNiceName].Count != 0;
                DialogueProducts = GlobalCache.CustomerDialogueProducts[GlobalCache.CurrentCustomer.UserNiceName];
            }



            //HotSellingProducts = GlobalCache.HotSellingProducts;
            AllProducts = GlobalCache.AllProducts.Take(20).ToList();

            storeProducts.AddRange(DialogueProducts);
            storeProducts.AddRange(GlobalCache.AllProducts);
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
            set 
            {
                SetProperty(ref _searchContent, value);
                //if (string.IsNullOrEmpty(value))
                //{
                //    OnSearchCommand();
                //}
                OnSearchCommand();
            }
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
        /// 搜索过多
        /// </summary>
        public bool SearchToMore
        {
            get => _searchToMore;
            set => SetProperty(ref _searchToMore, value);
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
        //public IList<MyProduct> HotSellingProducts { get; set; } = new ObservableCollection<MyProduct>();

        /// <summary>
        /// 全部商品
        /// </summary>
        public IList<MyProduct> AllProducts { get; set; } = new ObservableCollection<MyProduct>();

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
            if (IsSearch)
            {
                BindingOperations.EnableCollectionSynchronization(SearchProducts, new object());
            }else
            {
                BindingOperations.DisableCollectionSynchronization(SearchProducts);
            }
            var indexedStoreProducts = storeProducts
            .GroupBy(p => p.ProductName)
            .Select(g => g.First())
            .ToDictionary(p => p.ProductName);


            List<MyProduct> matchingProducts = indexedStoreProducts
            .AsParallel()
            .Where(pair => pair.Key.Contains(SearchContent))
            .Select(pair => pair.Value)
            .ToList();

            //var matchingProducts = storeProducts.Where(p => p.ProductName.Contains(SearchContent)).ToList();
            IsSearchResult = matchingProducts.Count > 0;
            if (IsSearch)
            {
                SearchToMore = matchingProducts.Count > 20;
                SearchProducts = new ObservableCollection<MyProduct>(matchingProducts.Take(20).ToList());
                SearchProductNum = matchingProducts.Count;
            }
        }

        /// <summary>
        /// 选择命令
        /// </summary>
        /// <param name="product"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnChooseCommand(MyProduct product)
        {
            OnCloseCommand();
            if (ChooseWindowType == ChooseWindowType.ChooseProduct)
            {
                WeakReferenceMessenger.Default.Send(product, MessengerConstMessage.ChooseProductChangeToken);
            }
            else
            {

                GlobalCache.ProductIntroduction = product;

                WeakReferenceMessenger.Default.Send(product,MessengerConstMessage.ProductIntroductionToken);
            }
            

        }

        #endregion
    }
}
