using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaGlib.SetL
{
    public class DataRule: DataNode
    {
        public DataSymbol From { get; }
        public DataChain To { get; }

        public DataRule(string text)
        {
// sas            var match = Parser.Matches(Parser.RuleRegex, text).First().Groups;
//            From = new DataSymbol(match["from"].Value);
//            To = new DataChain(match["to"].Value);
        }

        public override string ToString()
        {
            return $"{From} -> {To}";
        }
    }
}
