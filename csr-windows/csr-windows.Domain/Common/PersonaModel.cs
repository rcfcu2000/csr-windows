using CommunityToolkit.Mvvm.ComponentModel;
using csr_windows.Domain.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csr_windows.Domain.Common
{
    /// <summary>
    /// 人设Model
    /// </summary>
    public class PersonaModel : ObservableRecipient
    {
        private string _name;
        private bool _isChecked;

        /// <summary>
        /// 姓名
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// 个人介绍
        /// </summary>
        public string Introduction { get; set; }

        /// <summary>
        /// 人设枚举
        /// </summary>
        public PersonaEnum PersonaEnum { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public string Price { get; set; }

        /// <summary>
        /// Gif图片地址
        /// </summary>
        public string GifFilePath { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImageUrl { get; set; }


        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsChecked
        {
            get => _isChecked;
            set => SetProperty(ref _isChecked, value);
        }

        /// <summary>
        /// 人设
        /// </summary>
        public string Persona { get; set; }

    }
}
