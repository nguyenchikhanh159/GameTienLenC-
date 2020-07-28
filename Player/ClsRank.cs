using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TienLen
{
    [Serializable]
    public class ClsRank
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public int rank { get; set; }
        public ClsRank()
        {
            name = "";
            rank = 0;
        }
        public ClsRank(string name, int rank)
        {
            this.name = name;
            this.rank = rank;
        }


    }
}
