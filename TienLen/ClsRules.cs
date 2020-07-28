using System;
using System.Collections.Generic;
using System.Text;

namespace TienLen
{
    public class ClsRules
    {
        static ClsGame g = new ClsGame();
        public bool IsSingleCardWin(ClsCard c, ClsCard d)//Kiểm tra xem quân bài thứ 2 có thắng quân bài thứ nhất không.
        {
            if (d.Value > c.Value || (d.Value == c.Value && d.Character > c.Character))
                return true;
            return false;
        }

        public bool IsDouble(List<ClsCard> doi)//đôi
        {
            if (doi.Count == 2)
            {
                if (doi[0].Value == doi[1].Value)
                    return true;
            }
            return false;
        }
        public bool Is3Equa(List<ClsCard> xamCo)//xám cô
        {
            if (xamCo.Count == 3)
            {
                if (xamCo[0].Value == xamCo[1].Value && xamCo[1].Value == xamCo[2].Value)
                    return true;
            }
            return false;
        }
        public bool Is4Equal(List<ClsCard> tuQuy)//tứ quý
        {
            if (tuQuy.Count == 4)
            {
                for (int i = 0; i < tuQuy.Count - 1; i++)
                {
                    if (tuQuy[i].Value != tuQuy[i + 1].Value)
                        return false;
                }
                return true;
            }
            return false;
        }
        public bool IsOrder(List<ClsCard> sanh)// kiểm tra sãnh
        {
            if (sanh.Count <= 2)
            {
                return false;

            }
            else
            {
                sanh.Sort();
                if (sanh[sanh.Count - 1].Value == 15)
                    return false;
                for (int i = 0; i < sanh.Count - 1; i++)
                {
                    if (sanh[i + 1].Value - sanh[i].Value != 1)
                        return false;
                }
            }
            return true;
        }
        public int IsWin(List<ClsCard> bo1, List<ClsCard> bo2)//Kiểm tra xem bộ bài số 2 đánh ra có thắng bộ bài số 1 đánh trước đó không.
        {
            if (bo2.Count == bo1.Count)
            {
                bool flag = true;
                for (int i = 0; i < bo1.Count; i++)
                {
                    if (bo1[i].Value == bo2[i].Value && bo1[i].Character == bo2[i].Character)
                        continue;
                    else
                    {
                        flag = false; break;
                    }
                }
                if (flag == true) return 1;

                if (IsDouble(bo1) && IsDouble(bo2))
                {
                    if (bo2[0].Value > bo1[0].Value)
                        return 1;
                    else if (bo1[0].Value == bo2[0].Value)
                    {
                        bo2.Sort();

                        bo1.Sort();
                        if (bo2[1].Character > bo1[1].Character)
                            return 1;
                    }
                    return 0;
                }
                else if (Is3Equa(bo1) && Is3Equa(bo2))
                {
                    if (bo2[0].Value > bo1[0].Value)
                        return 1;
                    else if (bo1[0].Value == bo2[0].Value)
                    {
                        bo1.Sort();
                        bo2.Sort();
                        if (bo1[2].Character > bo1[2].Character)
                            return 1;
                    }
                    return 0;
                }
                else if (IsOrder(bo1) && IsOrder(bo2))
                {
                    if (bo1.Count == bo2.Count)
                    {
                        int cuoi = bo1.Count - 1;
                        if (bo2[cuoi].Value != bo1[cuoi].Value)
                        {
                            if (bo2[cuoi].Value > bo1[cuoi].Value)
                                return 1;
                            return 0;
                        }
                        else
                        {
                            if (bo2[cuoi].Character > bo1[cuoi].Character)
                                return 1;
                            return 0;
                        }
                    }
                    return 0;
                }
                else if (IsSingleCardWin(bo1[0], bo2[0]))
                    return 1;
                return 0;
            }
            else
            {
                if (Chat(bo1, bo2))
                {
                    return 2;
                }
            }
            return 0;
        }
        public bool Chat(List<ClsCard> bo1, List<ClsCard> bo2)
        {
            bo2.Sort();
            bo1.Sort();
            if (bo1[0].Value == 15)
            {
                if (Is3DoiThong(bo2) || Is4DoiThong(bo2) || Is4Equal(bo2))
                    return true;
            }
            else if (IsDouble(bo1) && bo1[0].Value == 15)
            {
                if (Is4DoiThong(bo2) || Is4Equal(bo2))
                    return true;
            }
            else if (Is3DoiThong(bo1))
            {
                if (Is4DoiThong(bo2) || Is4Equal(bo2))
                    return true;
                else if (Is3DoiThong(bo2))
                {
                    if (bo2[0].Value > bo1[0].Value || (bo2[0].Value == bo1[0].Value && bo1[5].Character < bo2[5].Character))
                        return true;
                }
            }
            else if (Is4Equal(bo1))
            {
                if (Is4DoiThong(bo2))
                    return true;
            }
            else if (Is4DoiThong(bo1) && Is4DoiThong(bo2))
            {
                if (bo2[0].Value > bo1[0].Value || (bo2[0].Value == bo1[0].Value && bo2[7].Character > bo1[7].Character))
                    return true;
            }
            return false;
        }
        public bool Is3DoiThong(List<ClsCard> bo)
        {
            if (bo.Count == 6)
            {
                bo.Sort();
                List<ClsCard> temp = new List<ClsCard>();
                temp.Add(bo[0]);
                temp.Add(bo[2]);
                temp.Add(bo[4]);
                if (!IsOrder(temp))
                    return false;
                for (int i = 0; i < bo.Count; i += 2)
                {
                    if (bo[i].Value != bo[i + 1].Value)
                        return false;
                }
                return true;
            }
            return false;
        }
        public bool Is4DoiThong(List<ClsCard> bo)
        {
            if (bo.Count == 8)
            {
                bo.Sort();
                List<ClsCard> temp = new List<ClsCard>();
                temp.Add(bo[0]);
                temp.Add(bo[2]);
                temp.Add(bo[4]);
                temp.Add(bo[6]);
                if (!IsOrder(temp))
                    return false;
                for (int i = 0; i < bo.Count; i += 2)
                {
                    if (bo[i].Value != bo[i + 1].Value)
                        return false;
                }
                return true;
            }
            return false;
        }
        public bool IsValid(List<ClsCard> bai)//Kiểm tra xem các quân bài của người chơi chuẩn bị đánh ra có đi được với nhau hay không.
        {
            if (IsDouble(bai) || Is3Equa(bai) || Is4Equal(bai) || IsOrder(bai) || Is4DoiThong(bai) || Is3DoiThong(bai) || bai.Count == 1)
            {
                return true;
            }
            return false;
        }
        public bool SauDoiThong(List<ClsCard> bai)
        {
            bai.Sort();
            int soDoi = 0;
            for (int i = 0; i < bai.Count - 1;)
            {
                List<ClsCard> temp = new List<ClsCard>();
                temp.Add(bai[i]);
                temp.Add(bai[i + 1]);
                if (IsDouble(temp))
                {
                    soDoi += 1;
                    i += 2;
                }
                else
                    i += 1;
            }
            if (soDoi == 6)
                return true;
            return false;
        }
        public bool TuQuy2(List<ClsCard> bai)
        {
            bai.Sort();
            List<ClsCard> temp = new List<ClsCard>();
            temp.Add(bai[bai.Count - 1]);
            temp.Add(bai[bai.Count - 2]);
            temp.Add(bai[bai.Count - 3]);
            temp.Add(bai[bai.Count - 4]);
            if (bai[bai.Count - 1].Value == 15 && Is4Equal(temp))
                return true;
            return false;
        }
        public bool sanhRong(List<ClsCard> bai)
        {
            bai.Sort();
            int soValue = 0;
            for (int i = 3; i <= 14; i++)
            {
                for (int j = 1; j <= 4; j++)
                {
                    ClsCard t = new ClsCard(i, j);
                    if (g.IsContain(bai, t))
                    {
                        soValue += 1;
                        break;
                    }
                }
            }
            if (soValue == 12)
                return true;
            return false;
        }
        public bool isAnTrang(List<ClsCard> bai)
        {
            if (TuQuy2(bai) || sanhRong(bai) || SauDoiThong(bai))
                return true;
            return false;
        }
    }
}
