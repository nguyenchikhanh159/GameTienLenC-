using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TienLen
{
    class phong
    {
        public int soPhong { get; set; }
        public bool flag3bich { get; set; }// kiểm tra 3 bích
        public List<int> indexOfClient { get; set; }//index của client ở ngoài sv trong phòng chơi
        public int numberOfPlayer= 0;
        public List<ClsRank> listRank { get; set; }
        public ClsGame managerOfTheGame = new ClsGame();//quản lý ván chơi trong phòng 
        public ClsRules rule = new ClsRules();
        public int betMoney;//tiền cược       
        public int tempRank { get; set; }
        public phong()
        {
            soPhong = 0;
            flag3bich = true;
            numberOfPlayer = 0;
            indexOfClient = new List<int>();
            listRank = new List<ClsRank>();
            betMoney = 0;
            tempRank = 0;
        }
       
    }
}
