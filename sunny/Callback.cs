using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyTest
{
    /// <summary>
    ///  回调函数接口
    /// </summary>
    static class Callback
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
            Console.WriteLine(synet.身份验证模式_获取授权的S5账号(唯一ID));
            Request Request = Sunny.MessageIdToSunny(MessageId);

            // 操作请求数据（可以再任意消息类型时使用）
            // Request.request  
            
            if (消息类型 == Const.Net_Http_Request && Url.Contains("tce.taobao.com/api/data.htm"))
            {
                //检测到新订单来了
                Logger.WriteInfo("检测到新订单来了");
                string topCookie = Request.request.取全部cookie();
                //Console.WriteLine(topCookie);
                TopHelp tpHelp = new TopHelp();
                SqlHelper sqlHelper = new SqlHelper();
                Console.WriteLine("开始关闭 -消息通知 窗口");
                Logger.WriteInfo("开始关闭 -消息通知 窗口");
                bool isClosed = tpHelp.CloseWindowByProcessAndTitle("AliWorkbench", "-消息通知");
                if (isClosed)
                {
                    // 淘宝_取待发货列表
                    Console.WriteLine("开始 淘宝_取待发货列表");
                    List<OrderInfo> orderInfos = tpHelp.TopGetWaitSend(topCookie).GetAwaiter().GetResult(); ;
                    Console.WriteLine($"本次共检测到{orderInfos.Count}个订单");

                    //遍历待发货订单
                    foreach (OrderInfo info in orderInfos)
                    {

                        Console.WriteLine($"当前订单的商家编码是：{info.SJBM}");
                        //看看订单的商家编码在数据库中是否存在
                        int bbId = sqlHelper.GetBBBySjbm(info.SJBM);                       
                        if (bbId!=-1) //存在就去发货
                        {                            
                            //取对应的宝贝信息
                            string msg = sqlHelper.GetKMOne((int)bbId);

                            if (msg != null)
                            {
                                //获取到了对应的卡密信息,开始发千牛消息
                                Console.WriteLine($"获取到了对应的卡密信息{msg} 开始 开始发千牛消息"); 

                                bool isSend = tpHelp.QNSendMsg(info.BuyerNick, msg, 200);
                                if(isSend)
                                {
                                    //千牛发消息成功，开始淘宝发货
                                    isSend = tpHelp.TopSend(info.TradeId, topCookie).GetAwaiter().GetResult(); ;
                                    if (isSend)
                                    {
                                        Logger.WriteInfo($"买家{info.BuyerNick} 订单{info.TradeId} 发货成功！"); 
                                        //更新数据库卡密信息

                                    }
                                    else
                                    {
                                        Console.WriteLine("开始 淘宝_去发货 失败了");
                                        Logger.WriteError($"买家{info.BuyerNick} 订单{info.TradeId} 发货失败！");
                                    }


                                }
                                else
                                {
                                    Console.WriteLine("千牛发消息 失败了，下次轮询到再发");
                                    Logger.WriteError($"买家{info.BuyerNick} 的订单{info.TradeId} 千牛发消息失败！");
                                }
                             

                            }
                            else
                            {
                                Console.WriteLine("根据bbId未找到对应的卡密。");
                                Logger.WriteError($"检测到新订单{info.TradeId},该订单没有对应的到卡密信息！");

                            }

                        }
                        else {
                            Console.WriteLine("根据SJBM未找到对应的宝贝ID。");
                            Logger.WriteError($"检测到新订单{info.TradeId},该订单没有对应的到宝贝信息！");

                        }
                  
                    }

                }

            }

            //这里返回值是什么不重要，但得有
            return false;
        }
    }
}
