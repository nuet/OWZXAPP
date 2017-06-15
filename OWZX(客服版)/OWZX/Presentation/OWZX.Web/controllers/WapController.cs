using System;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

using OWZX.Core;
using OWZX.Model;
using OWZX.Services;
using OWZX.Web.Framework;
using OWZX.Web.Models;

namespace OWZX.Web.Controllers
{
    /// <summary>
    /// 账号控制器类
    /// </summary>
    public partial class WapController : BaseWebController
    { 
        /// <summary>
        /// 注册
        /// </summary>
        public ActionResult Register()
        { 
            string returnUrl = WebHelper.GetQueryString("returnUrl");
            if (returnUrl.Length == 0)
                returnUrl = "/"; 
            if (WorkContext.ShopConfig.RegType.Length == 0)
                return PromptView(returnUrl, "目前已经关闭注册功能!");
            if (WorkContext.Uid > 0)
                return PromptView(returnUrl, "你已经是本站的注册用户，无需再注册!");
            if (WorkContext.ShopConfig.RegTimeSpan > 0)
            {
                DateTime registerTime = Users.GetRegisterTimeByRegisterIP(WorkContext.IP);
                if ((DateTime.Now - registerTime).Minutes <= WorkContext.ShopConfig.RegTimeSpan)
                    return PromptView(returnUrl, "你注册太频繁，请间隔一定时间后再注册!");
            }

            //get请求
            if (WebHelper.IsGet())
            {
                RegisterModel model = new RegisterModel();

                model.ReturnUrl = "www.baidu.com";
                model.ShadowName = WorkContext.ShopConfig.ShadowName;
                model.IsVerifyCode = CommonHelper.IsInArray(WorkContext.PageKey, WorkContext.ShopConfig.VerifyPages);

                return View(model);
            }

            //ajax请求 
            string loginname = Randoms.CreateRandomValue(6); //用户名
            string password = WebHelper.GetFormString("password"); 
            string verifyCode = WebHelper.GetFormString("usercode"); 
            try
            {
                string phone = string.Empty;
                string account = phone = WebHelper.GetFormString("username").Trim();

                int userid = Users.GetUidByMobile(account);
                if (userid > 0)
                {
                    return AjaxResult("error", "账号已存在");
                }
                var smsmodel = NewUser.GetSMSCodeList(1, 1, " where account='" + account + "'").FirstOrDefault();
                if (smsmodel == null || smsmodel.Code != verifyCode)
                {
                    return AjaxResult("error", "手机验证码不正确");
                }
                else
                {
                    NewUser.DeleteSMSCode(smsmodel.Codeid+"");
                }
                int invitecode = -1; 
                invitecode = int.Parse(WebHelper.GetFormString("pid")); //介绍用户标识号
                string imei = Randoms.CreateRandomValue(16);

                UserInfo userInfo = null;

                userInfo = new UserInfo();
                userInfo.UserName = account;
                userInfo.UserId = Randoms.CreateRandomValue(8);
                userInfo.Email = string.Empty;
                userInfo.Mobile = phone;

                userInfo.Salt = Randoms.CreateRandomValue(6);
                userInfo.Password = Users.CreateUserPassword(password, userInfo.Salt);
                userInfo.UserRid = 7;//普通用户 UserRanks.GetLowestUserRank().UserRid;
                userInfo.AdminGid = 1;//非管理员组
                userInfo.NickName = Randoms.CreateRandomValue(6);
                userInfo.Avatar = "";
                userInfo.PayCredits = 0;
                userInfo.RankCredits = 0;
                userInfo.VerifyEmail = 0;
                userInfo.VerifyMobile = 0;
                userInfo.LastVisitIP = WorkContext.IP;
                userInfo.LastVisitRgId = WorkContext.RegionId;
                userInfo.LastVisitTime = DateTime.Now;
                userInfo.RegisterIP = WorkContext.IP;
                userInfo.RegisterRgId = WorkContext.RegionId;
                userInfo.RegisterTime = DateTime.Now;
                userInfo.Gender = WebHelper.GetFormInt("gender");
                userInfo.RealName = WebHelper.HtmlEncode(WebHelper.GetFormString("realName"));
                userInfo.Bday = new DateTime(1900, 1, 1);
                userInfo.IdCard = WebHelper.GetFormString("idCard");
                userInfo.RegionId = WebHelper.GetFormInt("regionId");
                userInfo.Address = WebHelper.HtmlEncode(WebHelper.GetFormString("address"));
                userInfo.Bio = WebHelper.HtmlEncode(WebHelper.GetFormString("bio"));
                userInfo.InviteCode = invitecode;
                userInfo.IMEI = imei;
                //创建用户
                userInfo.Uid = Users.CreateUser(userInfo);
                //添加用户失败
                if (userInfo.Uid < 1)
                    return AjaxResult("error", "注册失败");
                return AjaxResult("success", "注册成功");
            }
            catch (Exception ex)
            {
                Logs.Write("注册失败:" + ex.Message);
                return AjaxResult("error", "注册失败");
            }
        }

        public ActionResult App()
        {
            
            RegisterModel model = new RegisterModel();

            model.ReturnUrl = "www.baidu.com"; 

            return View(model); 
        }
        public ActionResult XY()
        { 
            return View();
        }

        /// <summary>
        /// 发送找回密码短信
        /// </summary>
        public ActionResult SendMSGMobile()
        {
            //发送找回密码短信
            string moibleCode = Randoms.CreateRandomValue(6); 
            try
            {
                string mobile = WebHelper.GetFormString("mobile");
                if (string.IsNullOrEmpty(mobile))
                    return AjaxResult("nouser", "请输入正确的手机号");
                var smsmodel = NewUser.GetSMSCodeList(1, 1, " where account='" + mobile + "'").FirstOrDefault();
                if (smsmodel != null)
                {
                    moibleCode = smsmodel.Code;
                    string body = "【PC蛋蛋】您正在注册,验证码" + moibleCode + ",若非本人操作，请勿泄露。";
                    bool smsres = SMSes.SendSY(mobile, HttpUtility.UrlEncode(body, Encoding.UTF8));
                    if (!smsres)
                    {
                        return AjaxResult("error", "发送失败");
                    }
                    return AjaxResult("success", "发送成功");
                }
                else
                {
                     string body = "【PC蛋蛋】您正在注册,验证码" + moibleCode + ",若非本人操作，请勿泄露。";

                    MD_SMSCode smscode = new MD_SMSCode
                    {
                        Account = mobile,
                        Code = moibleCode,
                        Expiretime = DateTime.Now.AddMinutes(10)
                    };
                    bool sms = NewUser.AddSMSCode(smscode);
                    if (sms)
                    {
                        //发送短信
                        bool smsres = SMSes.SendSY(mobile, HttpUtility.UrlEncode(body, Encoding.UTF8));
                        if (!smsres)
                        {
                            return AjaxResult("error", "发送失败");
                        }
                        return AjaxResult("success", "发送成功");
                    }
                    else
                        return AjaxResult("error", "发送失败");
                }
               
            }
            catch (Exception ex)
            {
                return AjaxResult("error", "发送失败");
            }  
        }
         
         
    }
}
