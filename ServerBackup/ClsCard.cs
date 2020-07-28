using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;



namespace TienLen
{
    [Serializable]
    public class ClsCard : IComparable<ClsCard>
    {
        [XmlAttribute]
        public int Value { get; set; }//tên quân bài
        [XmlAttribute]
        public int Character { get; set; }// chất quân bài
        [XmlAttribute]
        public bool Played { get; set; }//trạng thái quân bài đã đánh hay chưa đánh
        public ClsCard()
        {
            Value = 6;
            Character = 6;
            Played = false;
        }
        public ClsCard(int a,int b)
        {
            Value = a;
            Character = b;
            Played = false;
        }

        public int CompareTo(ClsCard c)
        {
            if (this.Value == c.Value)
                return this.Character.CompareTo(c.Character);
            if (this.Value < c.Value)
                return -1;
            if (this.Value > c.Value)
                return 1;
            return 0;
        }
    }
}
