using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using csr_windows.Domain;
using csr_windows.Domain.Common;
using csr_windows.Domain.WeakReferenceMessengerModels;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace csr_windows.Client.ViewModels.Menu
{
    public class ChangePersonaViewModel:ObservableRecipient
    {
        #region Fields
        private ObservableCollection<PersonaModel> _persionaList;



        #endregion

        #region Commands
        /// <summary>
        /// 切换人设
        /// </summary>
        public ICommand ChangePersionCommand { get; set; }

        /// <summary>
        /// 关闭
        /// </summary>
        public ICommand CloseCommand { get; set; }
        #endregion

        #region Constructor
        public ChangePersonaViewModel()
        {
            PersionaList = new ObservableCollection<PersonaModel>()
            {
                new PersonaModel()
                {
                    Name = "晓晓",
                    Introduction = "我是晓晓，您的贴心客服助手。无论您有任何问题，我都会尽力为您提供最优质的服务和最满意的解决方案。",
                    PersonaEnum = Domain.Enumeration.PersonaEnum.XiaoXiao,
                    Price = "0.1",
                    ImageUrl = "/csr-windows.Resources;component/Images/Menu/xiaoxiao.png",
                    GifFilePath = "/csr-windows.Resources;component/Images/Menu/xiaoxiao.gif",
                    Persona = "glm_flash",
                    IsChecked = true
                },
                new PersonaModel()
                {
                    Name = "乐乐",
                    Introduction = "我是乐乐，您的资深客服助手。我之前是女装导购，特别是在服装场景的尺码推荐上很厉害的哦。",
                    PersonaEnum = Domain.Enumeration.PersonaEnum.LeLe,
                    ImageUrl = "/csr-windows.Resources;component/Images/Menu/lele.png",
                    GifFilePath = "/csr-windows.Resources;component/Images/Menu/lele.gif",
                    Persona = "deep_seek",
                    Price = "1",
                },
                new PersonaModel()
                {
                    Name = "琳达",
                    Introduction = "我是琳达，您的客服主管。无论您有任何商品问题，我都能游刃有余地为您提供最精准的解决方案。",
                    PersonaEnum = Domain.Enumeration.PersonaEnum.LinDa,
                    ImageUrl = "/csr-windows.Resources;component/Images/Menu/linda.png",
                    GifFilePath = "/csr-windows.Resources;component/Images/Menu/linda.gif",
                    Persona = "glm_air",
                    Price = "1",
                },
            };
            GlobalCache.CurrentPersonaModel = PersionaList[0];
            ChangePersionCommand = new RelayCommand<PersonaModel>(OnChangePersionCommand);
            CloseCommand = new RelayCommand(OnCloseCommand);
        }




        #endregion

        #region Properties

        public ObservableCollection<PersonaModel> PersionaList
        {
            get => _persionaList;
            set => SetProperty(ref _persionaList, value);
        }

        #endregion

        #region Methods
        private void OnChangePersionCommand(PersonaModel model)
        {
            PersionaList.ForEach(x => x.IsChecked = false);
            model.IsChecked = true;
            GlobalCache.CurrentPersonaModel = model;
            OnCloseCommand();
            WeakReferenceMessenger.Default.Send(new PromptMessageTokenModel("已成功切换助手"), MessengerConstMessage.OpenPromptMessageToken);
        }

        private void OnCloseCommand()
        {
            WeakReferenceMessenger.Default.Send("", MessengerConstMessage.CloseMenuUserControlToken);
        }
        #endregion

    }
}
