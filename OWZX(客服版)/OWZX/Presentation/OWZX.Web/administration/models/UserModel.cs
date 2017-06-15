using System;
using System.Data;
using System.Web.Mvc;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using OWZX.Core;
using OWZX.Services;
using OWZX.Web.Framework;
using OWZX.Model;
using System.Text.RegularExpressions;

namespace OWZX.Web.Admin.Models
{
    /// <summary>
    /// 用户列表模型类
    /// </summary>
    public class UserListModel
    {
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 用户列表
        /// </summary>
        public DataTable UserList { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 用户手机
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 用户等级id
        /// </summary>
        public int UserRid { get; set; }
        /// <summary>
        /// 用户等级列表
        /// </summary>
        public List<SelectListItem> UserRankList { get; set; }
        /// <summary>
        /// 管理员组id
        /// </summary>
        public int AdminGid { get; set; }
        ///<summary>
        /// 管理员组列表
        /// </summary>
        public List<SelectListItem> AdminGroupList { get; set; }
    }

    /// <summary>
    /// 虚拟用户列表
    /// </summary>
    public class UserDummyListModel
    {
        /// <summary>
        /// 分页对象
        /// </summary>
        public PageModel PageModel { get; set; }
        /// <summary>
        /// 用户列表
        /// </summary>
        public List<MD_Dummy> UserList { get; set; }
      
    }

    /// <summary>
    /// 用户模型类
    /// </summary>
    public class UserModel : IValidatableObject
    {
        /// <summary>
        /// 用户名
        /// </summary>
        //[Required(ErrorMessage = "名称不能为空")]
        //[StringLength(20, ErrorMessage = "名称长度不能大于20")]
        public string UserName { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        //[Email]
        //[StringLength(50, ErrorMessage = "邮箱长度不能大于50")]
        public string Email { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        [Mobile]
        public string Mobile { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [StringLength(32, MinimumLength = 4, ErrorMessage = "密码长度必须大于3且小于33")]
        public string Password { get; set; }
        /// <summary>
        /// 确认密码
        /// </summary>
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "密码必须相同")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// 用户等级id
        /// </summary>
        //[Range(1, int.MaxValue, ErrorMessage = "请选择正确的用户等级")]
        //[DisplayName("用户等级")]
        public int UserRid { get; set; }
        /// <summary>
        /// 管理员组id
        /// </summary>
        //[Range(1, int.MaxValue, ErrorMessage = "请选择正确的管理员组")]
        //[DisplayName("管理员组")]
        public int AdminGid { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(20, ErrorMessage = "名称长度不能大于20")]
        public string NickName { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 支付积分
        /// </summary>
        //[Required(ErrorMessage = "支付积分不能为空")]
        //[Range(0, int.MaxValue, ErrorMessage = "支付积分不能为负数")]
        //[DisplayName("支付积分")]
        public int PayCredits { get; set; }
        /// <summary>
        /// 性别(0代表未知，1代表男，2代表女)
        /// </summary>
        //[Range(0, 2, ErrorMessage = "请选择确切的性别")]
        //[DisplayName("性别")]
        public int Gender { get; set; }
        /// <summary>
        /// 真实名称
        /// </summary>
        //[StringLength(5, ErrorMessage = "名称长度不能大于5")]
        public string RealName { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        [DisplayName("出生日期")]
        public DateTime? Bday { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        [IdCard]
        public string IdCard { get; set; }
        /// <summary>
        /// 所在地区域
        /// </summary>
        [DisplayName("区域")]
        public int RegionId { get; set; }
        /// <summary>
        /// 所在地详细地址
        /// </summary>
        [StringLength(75, ErrorMessage = "所在地详细地址长度不能大于75")]
        public string Address { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        [StringLength(150, ErrorMessage = "简介长度不能大于150")]
        public string Bio { get; set; }
    

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errorList = new List<ValidationResult>();

            //if (!SecureHelper.IsSafeSqlString(UserName))
            //{
            //    errorList.Add(new ValidationResult("用户名中包含不安全的字符,请删除!", new string[] { "UserName" }));
            //}

            //if (!SecureHelper.IsSafeSqlString(Email))
            //{
            //    errorList.Add(new ValidationResult("邮箱名中包含不安全的字符,请删除!", new string[] { "Email" }));
            //}

            return errorList;
        }
    }

    /// <summary>
    /// 用户模型类
    /// </summary>
    public class UserDummyModel : IValidatableObject
    {
        /// <summary>
        /// 投注类型
        /// </summary>
        [Required]
        [DisplayName("投注类型")]
        public string BetType { get; set; }
        /// <summary>
        /// 投注房间
        /// </summary>
        [Required]
        [DisplayName("投注房间")]
        public string RoomType { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        [Required]
        [DisplayName("昵称")]
        public string NickName { get; set; }
        /// <summary>
        /// 投注金额
        /// </summary>
        [Required]
        [DisplayName("投注金额")]
        public string Money{get;set;}
        /// <summary>
        /// 投注时间
        /// </summary>
        [Required]
        [DisplayName("投注时间")]
        public string BetTime { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        [Required]
        [DisplayName("用户头像")]
        public string Avatar { get; set; }
        /// <summary>
        /// 用户等级
        /// </summary>
        [Required]
        [DisplayName("用户等级")]
        public string Vip { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
             //验证Base64字符串的正则表达式
            Regex info = new Regex(@"[0-9|]");
            List<ValidationResult> errorList = new List<ValidationResult>();

            if (!SecureHelper.IsSafeSqlString(NickName))
            {
                errorList.Add(new ValidationResult("昵称中包含不安全的字符,请删除!", new string[] { "NickName" }));
            }
            if (BetType == "0")
            {
                errorList.Add(new ValidationResult("请选择房间类型!", new string[] { "BetType" }));
            }
            if (!info.IsMatch(Money))
            {
                errorList.Add(new ValidationResult("请输入有效的投注金额!", new string[] { "Money" }));
            }
            if (!info.IsMatch(BetTime))
            {
                errorList.Add(new ValidationResult("请输入有效的投注时间!", new string[] { "BetTime" }));
            }
            return errorList;
        }
    }

}
