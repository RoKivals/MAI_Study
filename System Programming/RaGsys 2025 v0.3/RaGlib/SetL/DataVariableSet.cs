using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaGlib.SetL
{
    public class DataVariableSet: DataNode
    {
        public string Name { get; }
        public List<DataNode> Elements { get; }

        public DataVariableSet(string name, DataNode elements)
        {
            Name = name;
            Elements = new List<DataNode>();
            if(elements is DataChains dc)
            {
                foreach(var c in dc.Chains)
                {
                    Elements.Add(c);
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder("");
            sb.Append(Name).Append(" = [");
            for(int i = 0; i < Elements.Count; i++)
            {
                sb.Append(Elements[i]).Append(i == Elements.Count - 1 ?  string.Empty : ", ");
            }
            sb.Append("]");
            return sb.ToString();
        }
    }
}
