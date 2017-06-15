using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OWZX.Model
{
    public class MD_UserBackRPT
    {
        private int uid;
        public int Uid
        {
            get { return uid; }
            set { uid = value; }
        }

        private string nickname;
        public string NickName
        {
            get { return nickname; }
            set { nickname = value; }
        }
        private int pid;
        public int Pid
        {
            get { return pid; }
            set { pid = value; }
        }
        private decimal profitmoney;
        public decimal ProfitMoney
        {
            get { return profitmoney; }
            set { profitmoney = value; }
        }

        private decimal money;
        public decimal Money
        {
            get { return money; }
            set { money = value; }
        }
        private int status;
        public int Status
        {
            get { return status; }
            set { status = value; }
        }
        private int backrate;
        public int BackRate
        {
            get { return backrate; }
            set { backrate = value; }
        }

        private int backtype;
        public int BackType
        {
            get { return backtype; }
            set { backtype = value; }
        }
        private DateTime addtime;
        public DateTime Addtime
        {
            get { return addtime; }
            set { addtime = value; }
        }
        private DateTime updatetime;
        public DateTime UpdateTime
        {
            get { return updatetime; }
            set { updatetime = value; }
        }
    }
}
