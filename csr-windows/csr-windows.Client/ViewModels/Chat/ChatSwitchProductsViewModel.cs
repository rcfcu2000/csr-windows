using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Domain;
using csr_windows.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace csr_windows.Client.ViewModels.Chat
{
    public class ChatSwitchProductsViewModel : ObservableRecipient
    {
        #region Fields

        #endregion

        #region Commands

        public ICommand CheckProductCommand { get; set; }
        private MyProduct _myProduct;


        #endregion

        #region Constrctor
        public ChatSwitchProductsViewModel(MyProduct myProduct)
        {
            CheckProductCommand = new RelayCommand(OnCheckProductCommand);
            MyProduct = myProduct;
        }

        #endregion

        #region Properties


        public MyProduct MyProduct
        {
            get => _myProduct;
            set => SetProperty(ref _myProduct, value);
        }

        #endregion

        #region Methods
        private void OnCheckProductCommand()
        {
            WeakReferenceMessenger.Default.Send(MyProduct, MessengerConstMessage.SendSwitchProductToken);
        }

        #endregion
    }
}
