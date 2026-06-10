using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RaGlib.SetL
{
    public class DataChains: DataNode
    {
        public List<DataChain> Chains { get; }

        public DataChains(string text)
        {
            Chains = new List<DataChain>();
            var elements = Parser.Matches(Parser.ChainRegex, text);
            foreach (var element in elements)
            {
                Chains.Add(new DataChain((element as Match).Groups["chain"].ToString()));
            }
        }

        public DataChains(List<DataChain> chains)
        {
            Chains = chains;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("{");
            for (int i = 0; i < Chains.Count; i++)
            {
                sb.Append(Chains[i]).Append(i == Chains.Count - 1 ? "}" : ", ");
            }
            if (Chains.Count == 0)
            {
                sb.Append("}");
            }
            return sb.ToString();
        }
    }
}
