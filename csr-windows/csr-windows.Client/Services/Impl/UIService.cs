﻿using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Services.Base;
using csr_windows.Client.ViewModels.Customer;
using csr_windows.Client.ViewModels.Main;
using csr_windows.Client.ViewModels.Menu;
using csr_windows.Client.Views.Customer;
using csr_windows.Client.Views.Main;
using csr_windows.Client.Views.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace csr_windows.Client.Services.Impl
{
    public class UISerivce : IUiService
    {
        #region Fields

        private WelcomeView _welcomeView;
        #endregion

        #region Methods


        private void DoWork(Action action)
        {
            App.Current.Dispatcher.Invoke(action);
            //DispatcherHelper.CheckBeginInvokeOnUI(action);
        }

        
        public void OpenWelcomeView()
        {
            Action ac = new Action(() =>
            {
                _welcomeView = new WelcomeView();
                _welcomeView.DataContext = new WelcomeViewModel();
                WeakReferenceMessenger.Default.Send(_welcomeView as UserControl, MessengerConstMessage.OpenMainUserControlToken);
            });
            DoWork(ac);
        }

        public void OpenNoStartClientView()
        {
            Action ac = new Action(() =>
            {
                var view = new NoStartClientView();
                view.DataContext = new NoStartClientViewModel();
                WeakReferenceMessenger.Default.Send(view as UserControl, MessengerConstMessage.OpenMainUserControlToken);
            });
            DoWork(ac);
        }

        public void OpenFirstSettingView()
        {
            Action ac = new Action(() =>
            {
                var view = new FirstSettingView();
                view.DataContext = new FirstSettingViewModel();
                WeakReferenceMessenger.Default.Send(view as UserControl, MessengerConstMessage.OpenMainUserControlToken);
            });
            DoWork(ac);
        }

        public void OpenCustomerView()
        {
            Action ac = new Action(() =>
            {
                var view = new CustomerView();
                view.DataContext = new CustomerViewModel();
                WeakReferenceMessenger.Default.Send(view as UserControl, MessengerConstMessage.OpenMainUserControlToken);
            });
            DoWork(ac);
        }


        public void OpenMenuAboutView()
        {
            Action ac = new Action(() =>
            {
                var view = new AboutView();
                view.DataContext = new AboutViewModel();
                WeakReferenceMessenger.Default.Send(view as UserControl, MessengerConstMessage.OpenMenuUserControlToken);
            });
            DoWork(ac);
        }

        #endregion

    }
    
}
