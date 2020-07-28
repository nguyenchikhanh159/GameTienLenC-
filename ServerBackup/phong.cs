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
        public bool flag3bich { get; set; }
        public List<int> indexOfClient { get; set; }
        public int numberOfPlayer = 0;
        public List<ClsCard> lastCard { get; set; }
        public List<ClsRank> listRank { get; set; }
        public ClsGame managerOfTheGame = new ClsGame();
        public ClsRules rule = new ClsRules();
        public int betMoney;

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
