using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWZX.Model
{
    public class MD_Dummy
    {
        public Int64 id { get; set; }

        private int dummyid;
        public int Dummyid
        {
            get { return dummyid; }
            set { dummyid = value; }
        }

        private string username;
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        private string nickname;
        public string Nickname
        {
            get { return nickname; }
            set { nickname = value; }
        }
        /// <summary>
        /// 房间类型
        /// </summary>
        public string RoomName { get; set; }
        /// <summary>
        /// 投注类型
        /// </summary>
        public string BetType { get; set; }

        private string money;
        public string Money
        {
            get { return money; }
            set { money = value; }
        }

        private string bettime;
        public string Bettime
        {
            get { return bettime; }
            set { bettime = value; }
        }
        /// <summary>
        /// 图像
        /// </summary>
        public string Avatar { get; set; }
        /// <summary>
        /// 等级
        /// </summary>
        public int Vip { get; set; }

        /// <summary>
        /// 休息起始
        /// </summary>
        public string Start { get; set; }

        /// <summary>
        /// 休息截止
        /// </summary>
        public string End { get; set; }
        public int totalcount { get; set; }

        private DateTime addtime;
        public DateTime Addtime
        {
            get { return addtime; }
            set { addtime = value; }
        }

    }
}
