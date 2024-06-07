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

        public PromptMessageTokenModel(string msg,bool isShowIcon)
        {
            Msg = msg;
            IsShowIcon = isShowIcon;
        }
    }
}
