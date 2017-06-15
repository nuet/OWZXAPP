using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWZX.Model
{
    /// <summary>
    /// 房间配置信息
    /// </summary>
    public class MD_BackRate
    {
        public Int64 Id { get; set; }
        private int rateid;
        public int Rateid
        {
            get { return rateid; }
            set { rateid = value; }
        }
        private string room;
        /// <summary>
        /// 房间类型
        /// </summary>
        public string Room
        {
            get { return room; }
            set { room = value; }
        }

        private int roomtypeid;
        /// <summary>
        /// 房间类型id
        /// </summary>
        [Required(ErrorMessage = "请选择房间类型")]
        [Range(1, int.MaxValue, ErrorMessage = "请选择房间类型")]
        public int Roomtypeid
        {
            get { return roomtypeid; }
            set { roomtypeid = value; }
        }

        private int minloss;
        /// <summary>
        /// 亏损下限金额（当天）
        /// </summary>
        [Required(ErrorMessage = "请输入亏损下限金额")]
        [Range(1, int.MaxValue, ErrorMessage = "请输入亏损下限金额")]
        public int Minloss
        {
            get { return minloss; }
            set { minloss = value; }
        }

        private int maxloss;
        /// <summary>
        /// 亏损上限金额（当天）
        /// </summary>
        [Required(ErrorMessage = "请输入亏损上限金额")]
        [Range(1, int.MaxValue, ErrorMessage = "请输入亏损上限金额")]
        public int Maxloss
        {
            get { return maxloss; }
            set { maxloss = value; }
        }

        private short backrate;
        /// <summary>
        /// 回水比例
        /// </summary>
        [Required(ErrorMessage = "请输入回水比例")]
        [Range(1, int.MaxValue, ErrorMessage = "请输入回水比例")]
        public short Backrate
        {
            get { return backrate; }
            set { backrate = value; }
        }

        private DateTime addtime;
        public DateTime Addtime
        {
            get { return addtime; }
            set { addtime = value; }
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errorList = new List<ValidationResult>();

            if (minloss>maxloss)
                errorList.Add(new ValidationResult("亏损上限必须大于亏损下限!", new string[] { "Maxloss" }));

            return errorList;
        }
    }


    public class MD_UserBackRate
    {
        public Int64 AutoID { get; set; }
        private string name;
        /// <summary>
        /// 房间类型
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private decimal backfee;
        /// <summary>
        /// 佣金
        /// </summary> 
        public decimal BackFee
        {
            get { return backfee; }
            set { backfee = value; }
        }

        private decimal minfee;
        /// <summary>
        /// 佣金下限金额（当天）
        /// </summary>
        [Required(ErrorMessage = "请输佣金损下限金额")]
        public decimal MinFee
        {
            get { return minfee; }
            set { minfee = value; }
        }

        private decimal maxfee;
        /// <summary>
        /// 佣金上限金额（当天）
        /// </summary>
        [Required(ErrorMessage = "请输入佣金上限金额")]
        public decimal MaxFee
        {
            get { return maxfee; }
            set { maxfee = value; }
        }


        private DateTime addtime;
        public DateTime Addtime
        {
            get { return addtime; }
            set { addtime = value; }
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> errorList = new List<ValidationResult>();

            if (minfee > maxfee)
                errorList.Add(new ValidationResult("佣金上限必须大于佣金下限!", new string[] { "MaxFee" }));

            return errorList;
        }
    }
}
