using CommunityToolkit.Mvvm.ComponentModel;
using csr_windows.Domain.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Client.ViewModels.Chat
{
    public class ChatTextAndProductViewModel : ObservableRecipient
    {

        #region Fields
        private string _startContent;
        private string _endContent;
        private bool _isShowEndContent;


        #endregion

        #region Commands

        #endregion

        #region Constructor

        public ChatTextAndProductViewModel()
        {
            
        }

        #endregion

        #region Properties
        /// <summary>
        /// 商品列表
        /// </summary>
        public IList<MyProduct> ProductsList { get; } = new ObservableCollection<MyProduct>();

        /// <summary>
        /// 开始内容
        /// </summary>
        public string StartContent
        {
            get => _startContent;
            set => SetProperty(ref _startContent, value);
        }

        /// <summary>
        /// 结束内容
        /// </summary>
        public string EndContent
        {
            get => _endContent;
            set 
            {
                SetProperty(ref _endContent, value);
                if(string.IsNullOrEmpty(_endContent))
                    IsShowEndContent = false;
                else
                    IsShowEndContent = true;
            }
        }

        /// <summary>
        /// 是否显示结束内容
        /// </summary>
        public bool IsShowEndContent
        {
            get => _isShowEndContent;
            set => SetProperty(ref _isShowEndContent, value);
        }

        #endregion

        #region Methods

        #endregion

    }
}
