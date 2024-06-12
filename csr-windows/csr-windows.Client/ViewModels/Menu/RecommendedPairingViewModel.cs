using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Services.Base;
using csr_windows.Domain;
using csr_windows.Domain.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Input;
using System.Windows.Threading;

namespace csr_windows.Client.ViewModels.Menu
{
    /// <summary>
    /// 推荐搭配视图模型
    /// </summary>
    public class RecommendedPairingViewModel : ObservableRecipient
    {
        #region Fields
        private IUiService _uiService;
        private bool _isSearch;
        private bool _isSearchNull;
        private string _searchContent;
        private int _chooseNum;

        private bool _isSearchResult;
        private int _searchProductNum;

        private bool _haveDialogueProduct;





        private ObservableCollection<MyProduct> _searchProducts;
        private List<MyProduct> storeProducts = new List<MyProduct>();

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

        /// <summary>
        /// 删除选项
        /// </summary>
        public ICommand DeleteChooseCommand { get; set; }

        /// <summary>
        /// 下一步命令
        /// </summary>
        public RelayCommand NextCommand { get; set; }
        #endregion

        #region Constructor
        public RecommendedPairingViewModel()
        {
            PropertyChanged += (s, e) =>
            {
                //通知命令能否执行
                NextCommand?.NotifyCanExecuteChanged();
            };
            _uiService = Ioc.Default.GetService<IUiService>();
            CloseCommand = new RelayCommand(OnCloseCommand);
            SearchCommand = new RelayCommand(OnSearchCommand);
            ChooseCommand = new RelayCommand<MyProduct>(OnChooseCommand);
            DeleteChooseCommand = new RelayCommand<ChooseProduct>(OnDeleteChooseCommand);
            NextCommand = new RelayCommand(OnNextCommand,CanNextCommand);
            for (int i = 0; i < 2; i++)
            {
                ChooseProducts.Add(new ChooseProduct());
            }


            #region 初始化

            if (GlobalCache.HaveCustomer && GlobalCache.CustomerDialogueProducts.ContainsKey(GlobalCache.CurrentCustomer.UserNiceName))
            {
                HaveDialogueProduct = GlobalCache.CustomerDialogueProducts[GlobalCache.CurrentCustomer.UserNiceName].Count != 0;
                DialogueProducts = GlobalCache.CustomerDialogueProducts[GlobalCache.CurrentCustomer.UserNiceName];
            }

            HotSellingProducts = GlobalCache.HotSellingProducts;

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
        /// 选择的商品数量
        /// </summary>
        public int ChooseNum
        {
            get => _chooseNum;
            set => SetProperty(ref _chooseNum, value);
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


        /// <summary>
        /// 下面已经选择的数组
        /// </summary>
        public IList<ChooseProduct> ChooseProducts { get; } = new ObservableCollection<ChooseProduct>();

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
            var firstNotChosenProduct = ChooseProducts.FirstOrDefault(p => !p.IsChoose);
            if (firstNotChosenProduct != null)
            {
                ChooseNum++;
                firstNotChosenProduct.Product = product;
                firstNotChosenProduct.IsChoose = true;
            }
  
        }

        private void OnDeleteChooseCommand(ChooseProduct product)
        {
            if (product.IsChoose)
            {
                ChooseNum--;
                product.Product = null;
                product.IsChoose = false;
            }
        }

        private void OnNextCommand()
        {
            var chooses = ChooseProducts.Where(p => p.IsChoose).Select(p => p.Product).ToList();
            _uiService.OpenProductIntroductionView(chooses);
            OnCloseCommand();
        }


        private bool CanNextCommand()
        {
            return ChooseNum > 0;
        }



        #endregion
    }

    /// <summary>
    /// 选择的商品Model
    /// </summary>
    public class ChooseProduct : ObservableRecipient
    {
        #region Fields
        private MyProduct _product;
        private bool _isChoose;

        #endregion

        #region Properties

        /// <summary>
        /// 商品
        /// </summary>
        public MyProduct Product
        {
            get => _product;
            set => SetProperty(ref _product, value);
        }



        /// <summary>
        /// 是否是有选择中了
        /// </summary>
        public bool IsChoose
        {
            get => _isChoose;
            set => SetProperty(ref _isChoose, value);
        }
        #endregion

    }
}
