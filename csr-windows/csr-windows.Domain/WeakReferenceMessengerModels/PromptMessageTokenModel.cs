using csr_windows.Resources.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.WeakReferenceMessengerModels
{
    public class PromptMessageTokenModel
    {
        public string Msg { get; set; }
        public bool IsShowIcon { get; set; }

        public PromptEnum PromptEnum { get; set; }

        public PromptMessageTokenModel(string msg,bool isShowIcon = true, PromptEnum promptEnum = PromptEnum.Success)
        {
            Msg = msg;
            IsShowIcon = isShowIcon;
            PromptEnum = promptEnum;
        }
    }
}
