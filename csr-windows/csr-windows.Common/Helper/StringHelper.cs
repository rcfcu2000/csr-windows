using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace csr_windows.Common.Helper
{
    public static class StringHelper
    {
        public static string GetTaoBaoIDByActionUrl(string actionUrl)
        {
            string ID = "";
            if (string.IsNullOrEmpty(actionUrl))
            {
               return ID;
            }
            // 使用正则表达式提取ID
            string pattern = @"id=(\d+)";
            Match match = Regex.Match(actionUrl, pattern);

            if (match.Success)
            {
                string id = match.Groups[1].Value;
                ID = id;
            }
            else
            {
                Logger.WriteInfo("ID not found in the URL.");
            }
            return ID;
        }
    }
}
