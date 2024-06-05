using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Client.Services.Base;
using csr_windows.Common.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Client.ViewModels.Main
{
    public class NoStartClientViewModel : ObservableRecipient
    {
        #region Fields
        private IUiService _uiService;
        #endregion

        #region Commands
        public IRelayCommand StartClientCommand { get; set; }
        #endregion

        #region Constructor
        public NoStartClientViewModel()
        {
            _uiService = Ioc.Default.GetService<IUiService>();
            StartClientCommand = new RelayCommand(OnStartClientCommand);
        }


        #endregion

        #region Properties
        #endregion

        #region Methods
        /// <summary>
        /// 启动千牛客户端
        /// </summary>
        private void OnStartClientCommand()
        {
            //打开千牛
            string programDisplayName = "千牛工作台"; 

            string programPath = FollowWindowHelper.FindProgramPath(programDisplayName);

            if (!string.IsNullOrEmpty(programPath))
            {
                try
                {
                    Process.Start(programPath + "\\AliWorkbench.exe");
                    Console.WriteLine($"{programDisplayName} started successfully from: {programPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to start {programDisplayName}: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Program {programDisplayName} not found in the registry.");
            }


            WeakReferenceMessenger.Default.Send(string.Empty, MessengerConstMessage.FollowWindowToken);
            //然后最后进入聊天界面
            WeakReferenceMessenger.Default.Send("", MessengerConstMessage.LoginSuccessToken);
            _uiService.OpenCustomerView();
        }
        #endregion
    }
}
