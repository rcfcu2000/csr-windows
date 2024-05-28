using CommunityToolkit.Mvvm.DependencyInjection;
using csr_windows.Client.Services.Base;
using csr_windows.Client.Services.Impl;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace csr_windows.Client
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Ioc.Default.ConfigureServices(new ServiceCollection().AddSingleton<IUiService, UISerivce>().BuildServiceProvider());
        }
    }
}
