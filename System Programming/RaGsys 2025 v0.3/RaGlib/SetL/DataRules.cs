using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RaGlib.SetL
{
    public class DataRules: DataNode
    {
        public List<DataRule> Rules { get; }

        public DataRules(string text)
        {
            Rules = new List<DataRule>();
            var elements = Parser.Matches(Parser.RulesSetRegex, text);
            foreach (var element in elements)
            {
                Rules.Add(new DataRule((element as Match).Groups["rule"].ToString()));
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("{");
            for (int i = 0; i < Rules.Count; i++)
            {
                sb.Append(Rules[i].ToString()).Append(i == Rules.Count - 1 ? "}" : ", ");
            }
            if (Rules.Count == 0)
            {
                sb.Append("}");
            }
            return sb.ToString();
        }
    }
}
