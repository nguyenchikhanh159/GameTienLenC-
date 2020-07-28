using System;
using System.Collections.Generic;
using System.Text;

namespace TienLen
{
    class ClsGame
    {
        public bool Status { get; set; }
        public int numOfPlayers { get; set; }
        public bool avtivatingPlayer { get; set; }
        public int justPlayer { get; set; }
        public List<ClsHandCards> clientOfRoom { get; set; }
        public List<ClsRank> listRank { get; set; }
        public List<ClsCard> temp { get; set; }
        public int dem { get; set; }//kiểm tra kết thúc
        public int tempRank { get; set; }

        private List<ClsCard> SetCard = new List<ClsCard>();
        public ClsRules clsrules = new ClsRules();
        public ClsGame()
        {
            dem = 0;
            tempRank = 0;
            Status = false;
            numOfPlayers = 0;
            avtivatingPlayer = false;
            avtivatingPlayer = false;
            //ClsCard t = new ClsCard();t.Value = 3;t.Character = 1;
            //SetCard.Add(t);
            getBai(SetCard, 52);
            clientOfRoom = new List<ClsHandCards>();
            listRank = new List<ClsRank>();
            temp = new List<ClsCard>();

        }

        public bool IsContain(List<ClsCard> arr, ClsCard t)
        {
            foreach (ClsCard c in arr)
            {
                if (t.Value == c.Value && t.Character == c.Character)
                    return true;
            }
            return false;
        }
        public void getBai(List<ClsCard> arr, int num)
        {
            for (int i = 0; i < num; i++)
            {
                ClsCard t = new ClsCard();
                do
                {
                    Random r = new Random();
                    t.Value = r.Next(3, 16);
                    t.Character = r.Next(1, 5);
                } while (IsContain(arr, t));
                arr.Add(t);
            }
        }
        public void Deal()//chia bài
        {
            for (int i = 0; i < 52; i += 4)
            {
                clientOfRoom[0].arrCard.Add(SetCard[i]);
                clientOfRoom[1].arrCard.Add(SetCard[i + 1]);
                clientOfRoom[2].arrCard.Add(SetCard[i + 2]);
                clientOfRoom[3].arrCard.Add(SetCard[i + 3]);
            }

        }
        public int NextPlayer()//mở rộng
        {
            if (dem == 3)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (clientOfRoom[i].Rank == 1)
                    {
                        justPlayer = i;
                        break;
                    }
                }
            }
            else
            {
                if (clientOfRoom[justPlayer].numOfRemainCards == 0)
                {
                    for (int j = justPlayer + 1; j < justPlayer + 4; j++)
                    {
                        if (clientOfRoom[j % 4].Status == true)
                            return j;
                    }
                    temp.RemoveRange(0, temp.Count);
                    for (int j = justPlayer + 1; j < justPlayer + 4; j++)
                    {
                        if (clientOfRoom[j % 4].Rank == 0)
                            clientOfRoom[j % 4].Status = true;
                    }
                    for (int j = justPlayer + 1; j < justPlayer + 4; j++)
                    {

                        if (clientOfRoom[j % 4].Rank == 0)
                        {
                            justPlayer = (justPlayer + j) % 4;
                            break;
                        }

                    }


                    return justPlayer;
                }

                int i = 0;
                while (i < 3)
                {
                    if (clientOfRoom[(justPlayer + 1 + i) % 4].Status == true)
                    {
                        return (justPlayer + 1 + i) % 4;
                    }
                    else
                    {
                        i += 1;
                    }
                }
                temp.RemoveRange(0, temp.Count);
            }
            return justPlayer;
        }
        public void Clear()
        {
            for (int i = 0; i < 4; i++)
            {
                clientOfRoom[i].arrCard.RemoveRange(0, clientOfRoom[i].arrCard.Count);
                clientOfRoom[i].numOfRemainCards = 13;
            }
            tempRank = 0;
        }
        public void Remove()//mở rộng
        {

        }
        //public bool TestValid()// kiểm tra hành động đánh bài của người chơi có hợp lệ không.
        //{
        //    return true;
        //}
        public void Activate()//kích hoạt một người chơi.
        {
            clientOfRoom[0].Activated = true;
            clientOfRoom[1].Activated = true;
            clientOfRoom[2].Activated = true;
            clientOfRoom[3].Activated = true;
        }


        public void Play()//thực hiện hành động đánh bài của người chơi.
        {

        }
    }
}
