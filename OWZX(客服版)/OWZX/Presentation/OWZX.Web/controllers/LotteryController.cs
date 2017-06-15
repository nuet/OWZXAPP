using Newtonsoft.Json;
using OWZX.Core;
using OWZX.Model;
using OWZX.Services;
using OWZX.Web.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OWZX.Web.Controllers
{
    /// <summary>
    /// 彩票
    /// </summary>
    public class LotteryController : BaseWebController
    {
        #region 竞猜
        private object lkbtlow = new object();
        private object lkbtmin = new object();
        private object lkbthigh = new object();
        NameValueCollection parmas;
        /// <summary>
        /// 投注 （添加投注记录，扣除用户金额）
        /// </summary>
        /// <returns></returns>
        public ActionResult Bett()
        {
            try
            {
                NameValueCollection parmas = WorkContext.postparms;
                if (parmas.Keys.Count != 7)
                {
                    return APIResult("error", "缺少请求参数");
                }
                
                string room = parmas["room"].Trim();
                string msg = Lottery.ValidateBett(parmas["account"], parmas["expect"], parmas["money"], room, parmas["vip"], int.Parse(parmas["bttypeid"]));
                if (msg != string.Empty)
                {
                    return APIResult("error", msg);
                }

                if (room == "初级")
                {
                    int btmoney = int.Parse(parmas["money"]);
                    //判断投注的最高注数 是否有效
                    if (btmoney < 10)
                    {
                        return APIResult("error", "单笔投注金额不能小于10元宝");
                    }
                    if (btmoney > 20000)
                    {
                        return APIResult("error", "单笔投注金额不能大于20000元宝");
                    }
                    return DealBettLow(parmas);
                }
                else if (room == "中级")
                {
                    int btminmoney = int.Parse(parmas["money"]);
                    if (btminmoney < 50)
                    {
                        return APIResult("error", "单笔投注金额不能小于50元宝");
                    }
                    if (btminmoney > 30000)
                    {
                        return APIResult("error", "单笔投注金额不能大于30000元宝");
                    }
                    return DealBettMid(parmas);
                }
                else if (room == "高级")
                {
                    int bthighmoney = int.Parse(parmas["money"]);
                    if (bthighmoney < 50)
                    {
                        return APIResult("error", "单笔投注金额不能小于50元宝");
                    }
                    if (bthighmoney > 30000)
                    {
                        return APIResult("error", "单笔投注金额不能大于30000元宝");
                    }
                    return DealBettHigh(parmas);
                }

              
                return APIResult("error", "投注失败");
            }
            catch (Exception ex)
            {
                return APIResult("error", "投注失败");
            }
        }

        private ActionResult DealBettLow(NameValueCollection parmas)
        {
            lock (lkbtlow)
            {
                //单注10-20000，总注80000封顶
                //大小单双20000封顶，极值5000封顶，猜数字5000封顶，组合10000封顶，红绿蓝20000封顶，豹子5000封顶

                int typeid = int.Parse(parmas["bttypeid"]);
                int money = int.Parse(parmas["money"]);
                string valres = Lottery.ValidateBetMoney(parmas["expect"], typeid, money, parmas["room"]);
                if (!valres.Contains("验证通过"))
                {
                    return APIResult("error", valres);
                }

                MD_Bett bet = new MD_Bett
                {
                    Account = parmas["account"],
                    Room = parmas["room"],
                    Vip = parmas["vip"],
                    Lotterynum = parmas["expect"],
                    Money = int.Parse(parmas["money"]),
                    Bttypeid = int.Parse(parmas["bttypeid"])
                };

                bool betres = Lottery.AddBett(bet);
                if (betres)
                    return APIResult("success", "投注成功");
                else
                    return APIResult("error", "投注失败");
            }
        }
        private ActionResult DealBettMid(NameValueCollection parmas)
        {
            lock (lkbtmin)
            {
                int typeid = int.Parse(parmas["bttypeid"]);
                int money = int.Parse(parmas["money"]);
                string valres = Lottery.ValidateBetMoney(parmas["expect"], typeid, money, parmas["room"]);
                if (!valres.Contains("验证通过"))
                {
                    return APIResult("error", valres);
                }

                MD_Bett bet = new MD_Bett
                {
                    Account = parmas["account"],
                    Room = parmas["room"],
                    Vip = parmas["vip"],
                    Lotterynum = parmas["expect"],
                    Money = int.Parse(parmas["money"]),
                    Bttypeid = int.Parse(parmas["bttypeid"])
                };

                bool betres = Lottery.AddBett(bet);
                if (betres)
                    return APIResult("success", "投注成功");
                else
                    return APIResult("error", "投注失败");
            }
        }
        private ActionResult DealBettHigh(NameValueCollection parmas)
        {
            lock (lkbthigh)
            {
                int typeid = int.Parse(parmas["bttypeid"]);
                int money = int.Parse(parmas["money"]);
                string valres = Lottery.ValidateBetMoney(parmas["expect"], typeid, money, parmas["room"]);
                if (!valres.Contains("验证通过"))
                {
                    return APIResult("error", valres);
                }

                MD_Bett bet = new MD_Bett
                {
                    Account = parmas["account"],
                    Room = parmas["room"],
                    Vip = parmas["vip"],
                    Lotterynum = parmas["expect"],
                    Money = int.Parse(parmas["money"]),
                    Bttypeid = int.Parse(parmas["bttypeid"])
                };

                bool betres = Lottery.AddBett(bet);
                if (betres)
                    return APIResult("success", "投注成功");
                else
                    return APIResult("error", "投注失败");
            }
        }
        /// <summary>
        /// 投注记录
        /// </summary>
        /// <returns></returns>
        public ActionResult BettRecord()
        {
            try
            {
                NameValueCollection parmas = WorkContext.postparms;

                string account = parmas["account"];
                int page = int.Parse(parmas["page"]);
                StringBuilder strb = new StringBuilder();
                string type = parmas["type"];

                string start = string.Empty;
                string end = string.Empty;
                if (parmas.AllKeys.Contains("start") && parmas.AllKeys.Contains("end"))
                {
                    start = parmas["start"];
                    end = parmas["end"];

                    string[] st = start.Split('-');
                    if (st[1].Length == 1)
                        st[1] = "0" + st[1];
                    start = string.Join("-", st);

                    string[] ed = end.Split('-');
                    if (ed[1].Length == 1)
                        ed[1] = "0" + ed[1];
                    end = string.Join("-", ed);
                }


                strb.Append(" where 1=1");
                if (type != string.Empty && type != "0")
                    strb.Append("  and c.type=" + type);
                if (start != string.Empty && end != string.Empty)
                    strb.Append("  and convert(varchar(10),a.addtime,120) between '" + start + "' and '" + end + "'");
               
                DataTable list = NewUser.GetUserBettList(page, 15, account, strb.ToString());
                if (list.Rows.Count == 0)
                {
                    return APIResult("error", "暂无投注记录");
                }

                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                jsetting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                //jsetting.ContractResolver = new JsonLimitOutPut(new string[] { "Expect", "Result" }, true);
                string data = JsonConvert.SerializeObject(list, jsetting).ToLower();
                return APIResult("success", data, true);
            }
            catch (Exception ex)
            {
                return APIResult("error", "获取失败");
            }
        }
        /// <summary>
        /// 最新10期开奖结果
        /// </summary>
        /// <returns></returns>
        public ActionResult LastBett()
        {
            try
            {
                NameValueCollection parmas = WorkContext.postparms;

                string type = parmas["type"];

                List<MD_Lottery> list = Lottery.LastLotteryList(int.Parse(type));
                if (list.Count == 0)
                {
                    return APIResult("error", "暂无开奖记录");
                }
                list.ForEach((x) =>
                {
                    if (x.Status == 1 && x.Result == null)
                    {
                        x.Expect = "第" + x.Expect + "期";
                        x.Result ="?+?+?=? (类型)";
                    }
                    else
                    {
                        x.Expect = "第" + x.Expect + "期";
                        string res = "(";
                        if (int.Parse(x.Resultnum) <= 13)
                        {
                            res += "小";
                        }
                        else
                        {
                            res += "大";
                        }

                        if (int.Parse(x.Resultnum) % 2 == 0)
                        {
                            res += ",双)";
                        }
                        else
                        {
                            res += ",单)";
                        }
                        x.Result += res;
                    }
                });

                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                jsetting.ContractResolver = new JsonLimitOutPut(new string[] { "Expect", "Result" }, true);
                string data = JsonConvert.SerializeObject(list, jsetting).ToLower();
                return APIResult("success", data, true);
            }
            catch (Exception ex)
            {
                return APIResult("error", "获取失败:"+ex.Message);
            }
        }

        /// <summary>
        /// 走势图
        /// </summary>
        /// <returns></returns>
        public ActionResult Trend()
        {
            try
            {
                NameValueCollection parmas = WorkContext.postparms;

                string type = parmas["type"];
                int page = int.Parse(parmas["page"]);
                DataTable list = Lottery.LotteryTrend(page, 15, type);
                if (list.Rows.Count == 0)
                {
                    return APIResult("error", "暂无开奖记录");
                }

                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                //jsetting.ContractResolver = new JsonLimitOutPut(new string[] { "Expect", "Result" }, true);
                string data = JsonConvert.SerializeObject(list, jsetting).ToLower();
                return APIResult("success", data, true);
            }
            catch (Exception ex)
            {
                return APIResult("error", "获取失败");
            }
        }

        public ActionResult GetProfitloss()
        {
            NameValueCollection parmas = WorkContext.postparms;

            string type = parmas["type"];

            DataTable list = Lottery.GetProfitloss(int.Parse(type), parmas["account"].ToString());

            if (list != null && list.Rows.Count > 0)
            {
                var status = list.Rows[0]["status"].ToString();
                var proft = "↑" + list.Rows[0]["luckresult"].ToString();
                if (status == "2")
                {
                    proft = "↓" + list.Rows[0]["luckresult"].ToString();
                }
                return APIResult("success", proft, false, "status", status);
            }
            else
            {
                return APIResult("error", "");
            }
        }

        /// <summary>
        /// 最新竞猜信息
        /// </summary>
        /// <returns></returns>
        public ActionResult LastLottery()
        {
            try
            {
                NameValueCollection parmas = WorkContext.postparms;

                string type = parmas["type"];
                string resjson = string.Empty;


                if (type == "10")
                {
                    //游戏是否维护中
                    BaseInfo baseinfo = BSPConfig.BaseConfig.BaseList.Find(x => x.Key == "北京28");
                    if (baseinfo.Account.Trim() == "是")
                    {
                        resjson = "{\"expect\":\"?\",\"time\":\"维护中\"}";
                    }
                    else
                    {
                        TimeSpan startTime = DateTime.Parse("09:00").TimeOfDay;
                        TimeSpan endTime = DateTime.Parse("23:55").TimeOfDay;
                        TimeSpan tmNow = DateTime.Now.TimeOfDay;

                        if (tmNow <= startTime || tmNow >= endTime)
                        {
                            //禁止投注时间
                            resjson = "{\"expect\":\"?\",\"time\":\"已停售\"}";
                        }
                    }

                }
                else if (type == "11")
                {
                    BaseInfo baseinfo = BSPConfig.BaseConfig.BaseList.Find(x => x.Key == "加拿大28");
                    if (baseinfo.Account.Trim() == "是")
                    {
                        resjson = "{\"expect\":\"?\",\"time\":\"维护中\"}";
                    }
                    else
                    {
                        TimeSpan startTime = DateTime.Parse("20:00").TimeOfDay;
                        TimeSpan endTime = DateTime.Parse("21:00").TimeOfDay;
                        TimeSpan tmNow = DateTime.Now.TimeOfDay;

                        if (tmNow >= startTime && tmNow <= endTime)
                        {
                            //禁止投注时间
                            resjson = "{\"expect\":\"?\",\"time\":\"已停售\"}";
                        }
                    }
                }

                if (resjson == string.Empty)
                {
                    DataTable list = Lottery.LastLottery(type);
                    if (list.Rows.Count == 0)
                    {
                        return APIResult("error", "暂无竞猜信息");
                    }
                    if (list.Rows[0]["time"].ToString().Trim() == "维护中")
                    {
                        list = Lottery.LastLottery(type);
                        if (list.Rows.Count == 0)
                        {
                            return APIResult("error", "暂无竞猜信息");
                        }
                    }

                    JsonSerializerSettings jsetting = new JsonSerializerSettings();
                    jsetting.ContractResolver = new JsonLimitOutPut(new string[] { "expect", "time" }, true);
                    resjson = JsonConvert.SerializeObject(list, jsetting).ToLower();

                }

                return APIResult("success", resjson.Replace("[", "").Replace("]", ""), true);
            }
            catch (Exception ex)
            {
                return APIResult("error", "获取失败");
            }
        }
        /// <summary>
        /// 赔率说明
        /// </summary>
        /// <returns></returns>
        public ActionResult SetRemark()
        {
            try
            {
                NameValueCollection parmas = WorkContext.postparms;

                string type = parmas["type"];

                DataTable list = Lottery.SetRemark(type);
                if (list.Rows.Count == 0)
                {
                    return APIResult("error", "获取失败");
                }

                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                string resjson = JsonConvert.SerializeObject(list, jsetting).ToLower();

                return APIResult("success", resjson, true);
            }
            catch (Exception ex)
            {
                return APIResult("error", "获取失败");
            }
        }
        /// <summary>
        /// 关于
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            try
            {

                BaseInfo info = BSPConfig.BaseConfig.BaseList.Find(x => x.Key == "关于配置");

                StringBuilder strb = new StringBuilder();
                strb.Append("{");
                string Image = BSPConfig.ShopConfig.SiteUrl + "/upload/imgs/" + info.Image;
                strb.Append("\"version\":\"" + info.BankAddress + "\",\"url\":\"" + info.Name + "\",\"qq\":\"" + info.Account + "\",\"wechat\":\"" + info.Bank + "\",\"img\":\"" + Image + "\"");
                strb.Append("}");
                return APIResult("success", strb.ToString(), true);
            }
            catch (Exception ex)
            {
                return APIResult("error", "获取失败");
            }
        }
        #endregion

        #region 聊天室
        string root = ConfigurationManager.AppSettings["hxurl"];

        /// <summary>
        /// 获取Token
        /// </summary>
        /// <returns></returns>
        public ActionResult Token()
        {
            MD_AccessTokenResult token = Lottery.GetAccessToken();
            return Content(token.SuccessResult.access_token);
        }
        /// <summary>
        /// 创建聊天室
        /// </summary>
        /// <returns></returns>
        public ActionResult ChatRoom()
        {
            string hxurl = root + "/chatrooms";
            MD_AccessTokenResult token = Lottery.GetAccessToken();
            string[] chat = new string[]{"bj-fir-vip1","bj-fir-vip2","bj-fir-vip3","bj-fir-vip4",
                                         "bj-sec-vip1","bj-sec-vip2","bj-sec-vip3","bj-sec-vip4",
                                         "bj-thr-vip1","bj-thr-vip2","bj-thr-vip3","bj-thr-vip4",
                                         "cakeno-fir-vip1","cakeno-fir-vip2","cakeno-fir-vip3","cakeno-fir-vip4",
                                         "cakeno-sec-vip1","cakeno-sec-vip2","cakeno-sec-vip3","cakeno-sec-vip4",
                                         "cakeno-thr-vip1","cakeno-thr-vip2","cakeno-thr-vip3","cakeno-thr-vip4"};

            StringBuilder strb = new StringBuilder();
            foreach (string str in chat)
            {
                strb.Append("{\"name\": \"" + str + "\",\"description\": \"" + str + "\",\"maxusers\": 500,\"owner\": \"8001\"}");
                string result = WebHelper.GetHXRequestData(hxurl, "post", token.SuccessResult.access_token, true, strb.ToString());
                if (result.Contains("error"))
                {
                    return APIResult("error", "聊天室创建失败，返回信息 ：" + result);
                }
                strb = new StringBuilder();
            }
            //获取聊天室信息
            hxurl += "?pagenum=1&pagesize=24";
            string chats = WebHelper.GetHXRequestData(hxurl, "get", token.SuccessResult.access_token, true, "");

            if (!chats.Contains("error"))
                return APIResult("success", "创建成功", true);

            return APIResult("success", "创建失败", true);
        }

        /// <summary>
        /// 获取聊天室
        /// </summary>
        /// <returns></returns>
        public ActionResult GetChatRoom()
        {
            try
            {
                string hxurl = root + "/chatrooms";
                MD_AccessTokenResult token = Lottery.GetAccessToken();

                //获取聊天室信息
                hxurl += "?pagenum=1&pagesize=24";
                string chats = WebHelper.GetHXRequestData(hxurl, "get", token.SuccessResult.access_token, true, "");
                MD_HXRoomData room = JsonConvert.DeserializeObject<MD_HXRoomData>(chats);
                JsonSerializerSettings jsetting = new JsonSerializerSettings();
                jsetting.ContractResolver = new JsonLimitOutPut(new string[] { "id", "name", "owner", "affiliations_count" }, true);

                string data = JsonConvert.SerializeObject(room.data.OrderBy(x => x.id),jsetting);
                return APIResult("success", data, true);
            }
            catch (Exception ex)
            {
                return APIResult("success", "获取失败", true);
            }
        }
        /// <summary>
        /// 房间在线人数
        /// </summary>
        /// <returns></returns>
        public ActionResult RoomOnline()
        {
            try
            {
                NameValueCollection parmas = WorkContext.postparms;

                string hxurl = root + "/chatrooms";
                MD_AccessTokenResult token = Lottery.GetAccessToken();

                string type = parmas["type"];
                if (type == "10")
                {
                    type = "bj";
                }
                else if (type == "11")
                {
                    type = "cakeno";
                }

                //获取聊天室信息
                hxurl += "?pagenum=1&pagesize=24";
                string chats = WebHelper.GetHXRequestData(hxurl, "get", token.SuccessResult.access_token, true, "");
                MD_HXRoomData room = JsonConvert.DeserializeObject<MD_HXRoomData>(chats);
                List<MD_RoomData> dtlist = room.data.FindAll(x => x.name.Contains(type));

                string[] rooms = new string[] { "fir", "sec", "thr" };
                StringBuilder strb = new StringBuilder();

                int rmtotal = 0;
                strb.Append("[");
                foreach (string rmstr in rooms)
                {
                    strb.Append("{");
                    List<MD_RoomData> items = dtlist.FindAll(x => x.name.Contains(rmstr));
                    for (int i = 1; i < 5; i++)
                    {
                        MD_RoomData rmdt = items.OrderBy(x => x.id).First(x => x.name.Contains("vip" + i.ToString()));
                        strb.Append("\"vip" + i.ToString() + "\":" + rmdt.affiliations_count + ",");
                        rmtotal += rmdt.affiliations_count;
                    }
                    if (strb.Length > 1)
                        strb = strb.Remove(strb.Length - 1, 1);
                    strb.Append(",\"rmtotal\":" + rmtotal.ToString() + "},");


                    rmtotal = 0;
                }
                if (strb.Length > 1)
                    strb = strb.Remove(strb.Length - 1, 1).Append("]");

                //JsonSerializerSettings jsetting = new JsonSerializerSettings();
                //string data = JsonConvert.SerializeObject(strb.ToString()).ToLower();

                return APIResult("success", strb.ToString(), true);
            }
            catch (Exception ex)
            {
                return APIResult("error", "获取失败");
            }
        }
        /// <summary>
        /// 删除聊天室(MVC 不支持Delete)
        /// </summary>
        /// <returns></returns>
        public ActionResult DelChatRoom()
        {
            try
            {

                string hxurl = root + "/chatrooms";
                MD_AccessTokenResult token = Lottery.GetAccessToken();

                //获取聊天室信息
                hxurl += "?pagenum=1&pagesize=24";
                string chats = WebHelper.GetHXRequestData(hxurl, "get", token.SuccessResult.access_token, true, "");
                MD_HXRoomData room = JsonConvert.DeserializeObject<MD_HXRoomData>(chats);

                foreach (MD_RoomData rd in room.data)
                {
                    hxurl = root + "/chatrooms/" + rd.id.ToString();
                    chats = WebHelper.GetHXRequestData(hxurl, "delete", token.SuccessResult.access_token, true, "");
                    room = JsonConvert.DeserializeObject<MD_HXRoomData>(chats);
                }
                return APIResult("success", "删除成功", true);
            }
            catch (Exception ex)
            {
                return APIResult("error", "删除失败", true);
            }
        }
        /// <summary>
        /// 删除聊天室成员
        /// </summary>
        /// <returns></returns>
        public ActionResult DelChatRoomUser()
        {
            try
            {
                NameValueCollection parmas = WorkContext.postparms;
                string hxurl = root + "/chatrooms/" + parmas["chatroomid"] + "/users/" + parmas["mobile"];
                MD_AccessTokenResult token = Lottery.GetAccessToken();

                string chats = WebHelper.GetHXRequestData(hxurl, "delete", token.SuccessResult.access_token, true, "");
                if (chats.Contains("error"))
                {
                    ErrorMsg errm = JsonConvert.DeserializeObject<ErrorMsg>(chats);
                    return APIResult("error", errm.error_description);
                }
                else
                {
                    MD_HXRoomData room = JsonConvert.DeserializeObject<MD_HXRoomData>(chats);

                    if (room.data[0].result)
                        return APIResult("success", "删除成功");
                }

                return APIResult("error", "删除失败");
            }
            catch (Exception ex)
            {
                return APIResult("error", "删除失败");
            }
        }
       
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <returns></returns>
        private ActionResult SendMsgs()
        {

            try
            {
                string hxurl = root + "/messages";
                MD_AccessTokenResult token = Lottery.GetAccessToken();

                //获取聊天室信息
                string ptdata = "{\"target_type\":\"chatrooms\",\"target\":[\"275831248121758236\"], \"msg\":{\"type\":\"txt\",\"msg\":\"hello from rest\"},\"from\":\"8001\"}";
                string chats = WebHelper.GetHXRequestData(hxurl, "post", token.SuccessResult.access_token, true, ptdata);
                MD_HXRoomData room = JsonConvert.DeserializeObject<MD_HXRoomData>(chats);
                string data = JsonConvert.SerializeObject(room.data.OrderBy(x => x.id));
                return APIResult("success", data, true);
            }
            catch (Exception ex)
            {
                return APIResult("error", "获取失败", true);
            }
        }

        private static object lkmsg = new object();
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <returns></returns>
        public ActionResult SendMsg()
        {
            try
            {
                NameValueCollection parmas = WorkContext.postparms;

                string type = parmas["type"];
                string vip = parmas["roomid"];
                string account = parmas["account"];

                PartUserInfo user = Users.GetPartUserByMobile(account);

                string hxurl = "https://a1.easemob.com/1117170524115941/lk28/messages";
                MD_AccessTokenResult token = Lottery.GetAccessToken();
                

                string msg = "欢迎【" + user.NickName+"】 "  +(type=="1"?"进入":"退出")+ "房间...";
                //获取聊天室信息
                string ptdata = "{\"target_type\":\"chatrooms\",\"target\":[\"" + vip+
                    "\"], \"msg\":{\"type\":\"txt\",\"msg\":\"" + msg + "\"},\"from\":\"8002\"}";
                string chats = WebHelper.GetHXRequestData(hxurl, "post", token.SuccessResult.access_token, true, ptdata);
                if (chats.Contains("error"))
                {
                    Logs.Write("发送消息失败：" + chats);
                    return APIResult("error", "发送失败");
                }
                else
                {
                    return APIResult("success", "发送成功");
                }

            }
            catch (Exception ex)
            {
                Logs.Write("发送消息失败：" + ex.Message);
                return APIResult("error", "发送失败", true);
            }
        }

        /// <summary>
        /// 获取聊天室
        /// </summary>
        /// <returns></returns>
        public List<MD_RoomData> GetChatRoomList()
        {
            List<MD_RoomData> listroom = MemoryCacheHelper.GetCacheItem<List<MD_RoomData>>("allchatrooms", delegate()
            {
                List<MD_RoomData> list = new List<MD_RoomData>();
                string hxurl = "https://a1.easemob.com/1117170524115941/lk28/chatrooms";
                MD_AccessTokenResult token = Lottery.GetAccessToken();

                //获取聊天室信息
                hxurl += "?pagenum=1&pagesize=24";
                string chats = WebHelper.GetHXRequestData(hxurl, "get", token.SuccessResult.access_token, true, "");
                MD_HXRoomData room = JsonConvert.DeserializeObject<MD_HXRoomData>(chats);
                list = room.data;
                return list;
            },
                 new TimeSpan(24, 0, 0)//过期
             );

            return listroom;
        }
        /// <summary>
        /// 获取用户
        /// </summary>
        public void GetIMList()
        {
            
                List<MD_RoomData> list = new List<MD_RoomData>();
                string hxurl = "https://a1.easemob.com/1117170524115941/lk28/users";
                MD_AccessTokenResult token = Lottery.GetAccessToken();

                //获取聊天室信息
                hxurl += "?limit=500";
                string chats = WebHelper.GetHXRequestData(hxurl, "get", token.SuccessResult.access_token, true, "");
               
        }
        /// <summary>
        /// 批量添加用户
        /// </summary>
        /// <returns></returns>
        public ActionResult AddIM()
        {
            string[] username = new string[]{
"小贤",
"浮浮沉沉",       
"(独自一人)",
"折现浪漫",
"爱情的执着",
"分裂i",
"海蓝色裙子少女",
"笑丶",
"温柔",
"喵≧^ω^≦喵",
"单行线",
"酒颂",
"时光盗走",
"脑残°惹人爱",
"羁绊的感情",
"你是我",
"誮訫尐羅卟",
"你是我的",
"小宇宙※",
"￥红尘多败笔￥",
"浅念",
"沧古烟",
"眼泪无法表达",
"内心的挣扎",
"﹏来来去去",
"何必在意",
"冰了海",
"香蕉味i",
"野猫",
"笑衬孤独",
"じ☆ve",
"且爱 n",
"一样生",
"浅笑未央",
"洎俬の",
"嘟比嘟比嘟",
"舞林萌猪",
"豬頭龙",
"囧妹子",
"晗笑半步癫",
"为欢几何",
"脑子短路",
"情罙γùaη淺",
"任性不认命 ",
"扼守回忆",
"Home丨Team",
"深知",
"你是梦",
"我怎敢触碰",
"你的世界",
"匆匆那年",
"爱我的人",
"请呼吸",
"兔zi",
"南巷烈酒",
"独饮悲",
"女汉子",
"卖萌",
"零纪念",
"∠不要脸",
"身虽存",
"思想不断折磨",
"胆怯",
"那些、快乐",
"那伤、很别致",
"My Sunshine",
"蓝铯の裂痕",
"Dirty",
"宿命",
"偏执的悲伤≡",
"咒怨』",
"孤者为王",
"芳草碧色",
"不疯不野",
"不温柔",
"一味寻找着",
"Jian人范儿",
"『逍遙』",
"女神經i",
"戀上你的眼",
"蝶蝶蝶蝶、变",
"南望",
"马不停蹄",
"失恋算个鸟i",
"未来的CD",
"有它我暧",
"皒鈊為亇流血",
"寂寞会发芽",
"空无一人",
"不假╮思索",
"- 宁缺勿滥",
"大众男友",
"微醉的丶阳光",
"丶Fire",
"萌咖软妹",
"热泪都为你",
"゛画上妆",
"掩饰自己",
"被猪拱了的白菜",
"Wаit 尖叫",
"走廊里的花香",
"执此的想念",
"栖世",
"poison 宝贝",
"遗落の悲傷~",
"①個亽の蕜傷",
"你是我",
"抹不去的忧伤",
"ぺ爱éг卜得☆",
"╰爱在奔跑",
"糖糖开开",
"影子",
"錵開や落幕┓",
"非凉薄之人°",
"花开半夏琉璃殇",
"ぁ定格~你的帅が",
"下一站&失忆",
"瓦解一空",
"懂了情的浪女←",
"涛@爱你",
"自在乄",
"他是我心",
"情话迷人",
"依旧是你@",
"无敌～哔",
"余生终未归°",
"**矜持",
"抱抱啊霖",
"栀晴",
"浪女无家i",
"伴我暖i",
"固执か",
"半暖半夏半流年",
"嗯哼！",
"酷到爆炸",
"第一抹阳光",
"墨锦倾城",
"水晶～沬兮",
"被情所伤的心",
"小骄傲！",
"你不配",
"染青衣@",
"怪我喽",
"亡命.傲气.蠢货",
"Azure",
"滥情",
"若汐",
"酷似你爱人。",
"命里缺她",
"怪姐姐",
"浅夏",
"穿透心灵的冰",
"落尽殇寒",
"北柒^陌人",
"仺白了青春", 
"黒色ン誘惑灬", 
"累@",
"@重返20岁",
"步非+烟花", 
"背叛的报应",
"彼此爱人i@", 
"长欢尽", 
"永恆的承諾", 
"戏子", 
"卟屬於我", 
"爱情自以为是", 
"釹王控", 
"对半感情", 
"ら樱雪之城ペ", 
"蝼蚁@", 
"灿勋zzang ",
"嘚瑟的小情绪ぃ", 
"曾经少年薄荷凉", 
"无 、尽 寂寞", 
"分开走@", 
"华年乱了谁的浮生", 
"り午夜↘清醒依旧", 
"颓废式╭流年", 
"萌主﹫", 
"花香洇染",
"智商╮偏d1", 
"阳光下的少年", 
"冷酷‰杀神", 
"做个低调の孩纸",
"C丶F灬梦之队", 
"ɡ1rl。女孩", 
"女孩般的幸福", 
"我姓黄我心慌！", 
"天青色等烟雨.", 
"烟染╰　素人颜", 
"丿super丶潮流",
"煙消雲散只為成全", 
"爱没有所谓亏欠", 
"奥利奥",
"我在地狱仰望天堂", 
"青春的爱恋", 
"Ｓòrγy︶", 
"萌@爹", 
"女王(Queen)ゆ性", 
"- Vie", 
"爱情有保质期", 
"徒留一场笑谈一场心伤", 
"最爱还是你i", 
"掌心温差", 
"玫瑰香旳誘惑", 
"看我不爽就滚i", 
"樱花树下、那纯美一笑 ",
"柠栀@", 
"分开也不一定分手", 
"Queenie. 女帝", 
"万能男神经i", 
"糕富帅#", 
"愛上╮寂寞", 
"人情薄如纸", 
"倾城一笑，抵我半壁江山", 
"◆帅气范儿つ", 
"黑的不是社会，是心", 
"这年头、寂寞", 
"揍性！", 
"你不爱我、但我爱你", 
"℡懒懒DE猪", 
"╰华灯初上、旧人可安", 
"乱的很有节奏ゆ", 
"婚姻终结者", 
"②号当铺，典当灵魂", 
"曾经飞蛾扑火", 
"颓废囧妳", 
"嬡過庅", 
"杯中酒，鸳鸯情", 
"繁复",
"等你醒了",
"空瞳",
"好像热情",
"放下一切",
"独孤久贱",
"暖终你",
"深巷老街",
"少时心慕.",
"心里设防",
"只为初见",
"柠檬不萌i",
"故作矜持",
"岁月未晚",
"天翻地覆又怎样",
"我不言语",
"善变人@",
"温酒往事",
"竹林已觅",
"还映枫林",
"初相识",
"欲封情",
"清风换歌",
"等一个晴天",
"听说你走了",
"吉他及她",
"比如明天比如你",
"借你体温",
"执手听风",
"旧城离人",
"旧了容颜",
"千百个你",
"好梦如旧",
"孤唇有毒i",
"给你感动",
"别逞强",
"停歇",
"寄语风雨",
"只若初见",
"故人不朽",
"错落年华",
"为你着迷",
"离调",
"时而想起",
"需要你归位",
"提刀杀红眼",
"忠于心@",
"几度斜阑",
"只是昨天",
"等风再来",
"从未有过",
"南风挽凉",
"趋着歌",
"尽头里哭泣",
"咧嘴笑",
"挽心",
"匹诺曹没有说谎i",
"逃离.",
"孤独似我",
"秒速",
"会保护你的",
"冻结灵魂",
"无谓",
"不靠谱先森",
"站在冰箱上会变高冷",
"借风拥你",
"丑角?",
"如果不是心太软",
"听够珍惜",
"矜言今后",
"别再说分手我不让你走",
"终究孤独i",
"键盘王者",
"世界末日i",
"来我讲故事",
"好战雨总?",
"目光",
"凉城小镇放肆少年",
"任你瞻仰",
"慰风尘",
"够局",
"不离她",
"别讲借口",
"月黑风高",
"傻瓜i i ",
"你已苍老待我远行",
"寂落",
"倾心~",
"我亦念旧",
"丶木棉花的春天",
"时光怂恿深爱的人放手",
"-别低头、皇冠会掉?",
"物以类聚",
"你美目如当年",
"敢永远,",
"一人留",
"单身狗??",
"烟酒烫心",
"她的国",
"一袭白衫",
"蹉跎","眉间眷恋",
"祭情"};

            string hxurl = "https://a1.easemob.com/1117170524115941/lk28/users";
            MD_AccessTokenResult token = Lottery.GetAccessToken();
            StringBuilder result = new StringBuilder();
            StringBuilder strb = new StringBuilder();
            strb.Append("[");
            int index=1;
            for(int i=0;i<username.Length;i++)
            {
                strb.Append("{\"username\":\"usn" + (i+1).ToString() + "\",\"password\":\"p28\"},");
                if (index == 20 || i == username.Length - 1)
                {
                    strb = strb.Remove(strb.Length - 1, 1);
                    strb.Append("]");
                    string chats = WebHelper.GetHXRequestData(hxurl, "post", token.SuccessResult.access_token, true, strb.ToString());
                    result.Append(chats + "           ");
                    index = 1;
                    strb = new StringBuilder();
                    strb.Append("[");
                }
                else
                {
                    index++;
                }
            }
            return AjaxResult("success", result.ToString());
        }

        
        #endregion

    }
}
