using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaGlib.SetL
{
    public abstract class DataNode
    {
        public static DataNode Parse(SETL setl, string input)
        {
/* sas
            var pattern = Parser.Matches(Parser.SetRegex, input)?.FirstOrDefault()?.Groups;
            if(pattern?.ContainsKey("elements") == true)
            {
                var subPattern = Parser.Matches(Parser.RulesSetRegex, pattern["elements"].Value)?.FirstOrDefault()?.Groups;
                if (subPattern?.ContainsKey("rule") == true)
                {
                    return new DataRules(pattern["elements"].Value);
                }
                subPattern = Parser.Matches(Parser.ChainsSetRegex, pattern["elements"].Value)?.FirstOrDefault()?.Groups;
                if (subPattern?.ContainsKey("chains") == true)
                {
                    return new DataChains(pattern["elements"].Value);
                }
                subPattern = Parser.Matches(Parser.OperationRegex, pattern["elements"].Value)?.FirstOrDefault()?.Groups;
                if (subPattern?.ContainsKey("operation") == true)
                {
                    return OpNode.Parse(pattern["elements"].Value).GetResult(setl.Variables);
                }
                subPattern = Parser.Matches(Parser.SymbolSetRegex, pattern["elements"].Value)?.FirstOrDefault()?.Groups;
                if(subPattern?.ContainsKey("symbol") == true)
                {
                    return new DataSymbols(pattern["elements"].Value);
                }
            }
            pattern = Parser.Matches(Parser.ElementOperationRegex, input)?.FirstOrDefault()?.Groups;
            if (pattern?.ContainsKey("operation") == true)
            {
                return OpNode.Parse(pattern["operation"].Value).GetResult(setl.Variables);
            }
            pattern = Parser.Matches(Parser.RuleRegex, input)?.FirstOrDefault()?.Groups;
            if (pattern?.ContainsKey("rule") == true)
            {
                return new DataRule(pattern["rule"].Value);
            }
            pattern = Parser.Matches(Parser.ChainRegex, input)?.FirstOrDefault()?.Groups;
            if (pattern?.ContainsKey("chain") == true)
            {
                return new DataChain(pattern["chain"].Value);
            }
            pattern = Parser.Matches(Parser.SymbolRegex, input)?.FirstOrDefault()?.Groups;
            if (pattern?.ContainsKey("symbol") == true)
            {
                return new DataSymbol(pattern["symbol"].Value);
            }
*/
            return null;
        }
    }
}
