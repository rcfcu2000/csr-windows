using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace csr_windows.Client.Services.WebService
{
    /// <summary>
    ///  回调函数接口
    /// </summary>
    public static class Callback
    {

        /// <summary>
        /// HTTP/HTTPS 回调
        /// </summary>
        /// <param name="唯一ID"></param>
        /// <param name="MessageId"></param>
        /// <param name="消息类型">Const.Net_Http_</param>
        /// <param name="Method"></param>
        /// <param name="Url"></param>
        /// <param name="err"></param>
        /// <param name="pid">进程PID 若等于0 表示通过代理远程请求 无进程PID</param>
        public static bool HTTP回调(int SunnyContext, int 唯一ID, int MessageId, int 消息类型, string Method, string Url, string err, int pid)
        {
            SunnyNet synet = new SunnyNet();

            //Console.WriteLine($"test reached... {唯一ID} {MessageId} {pid}");

            //Console.WriteLine(synet.身份验证模式_获取授权的S5账号(唯一ID));
            Request Request = Sunny.MessageIdToSunny(MessageId);

            // 操作请求数据（可以再任意消息类型时使用）
            // Request.request  
            //Console.WriteLine($"test reached... {消息类型} {Method} {Url}");

            if (消息类型 == Const.Net_Http_Request && Url.Contains("iseiya.taobao.com/imsupport"))
            {
                Console.WriteLine($"test reached... {Url}");
            }
            else if (消息类型 == Const.Net_Http_Response && Url.Contains("iseiya.taobao.com/imsupport"))
            {
                Request.response.修改或新增协议头("Content-Type: application/javascript");
                Request.response.修改状态码();

                string contents = File.ReadAllText(@"aliu.js");
                Request.response.修改响应内容_字符串_UTF8(contents);
            }

            //这里返回值是什么不重要，但得有
            return false;
        }
    }
}
