using System;
using System.Collections.Generic;
using System.Text;
namespace TienLen
{
    public class ClsHandCards
    {
        public int room { get; set; }
        public int numOfCards { get; set; }//cho biết số quân bài của người chơi
        public int numOfSelectedCards { get; set; }//cho biết số quân bài người chơi chọn để đánh.
        public int numOfRemainCards { get; set; }//cho biết số quân bài còn lại của người chơi.
        public List<ClsCard> arrCard { get; set; }//cho biết các quân bài của người chơi.
        public List<ClsCard> arrSelCards { get; set; }//cho biết các quân bài người chơi lựa chọn để đánh.
        public bool Activated;//cho biết nguời chơi có quyền được đánh hay không.
        public bool Status { get; set; }//trạng thái của người chơi.
        public int Rank { get; set; }//cho biết vị trí của nguời chơi khi kết thúc ván bài.
        public int money;

        public ClsHandCards()
        {
            arrCard = new List<ClsCard>();
            numOfCards = 0;
            numOfSelectedCards = 0;
            numOfRemainCards = 13;
            Activated = false;
            Status = true;
            Rank = 0;
            arrCard = new List<ClsCard>();
            arrSelCards = new List<ClsCard>();
            money = 1000;
        }

    }
}
