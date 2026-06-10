using RaGlib.SETL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaGlib.SetL
{
    public abstract class OpNode
    {
        public static OpNode Parse(string input)
        {
            input = input.Trim();
/* sas
            var operation = Parser.Matches(Parser.OpConditionRegex, input)?.FirstOrDefault()?.Groups;
            if (operation?.ContainsKey("condition") == true)
            {
                return new OpCondition(Parse(operation["left"].Value) as OpFrom, Parse(operation["right"].Value));
            }
            operation = Parser.Matches(Parser.OpFromRegex, input)?.FirstOrDefault()?.Groups;
            if (operation?.ContainsKey("from") == true)
            {
                return new OpFrom(new OpVariable(operation["left"].Value), Parse(operation["right"].Value));
            }
            operation = Parser.Matches(Parser.OpAutomateRegex, input)?.FirstOrDefault()?.Groups;
            if (operation?.ContainsKey("automate") == true)
            {
                return new OpAutomate(Parse(operation["left"].Value) as OpVariable, Parse(operation["right"].Value) as OpVariable);
            }
            operation = Parser.Matches(Parser.OpKliniRegex, input)?.FirstOrDefault()?.Groups;
            if (operation?.ContainsKey("symbol") == true)
            {
                return new OpKlini(new OpVariable(operation["symbol"].Value));
            }
            operation = Parser.Matches(Parser.OpSetUnionRegex, input)?.FirstOrDefault()?.Groups;
            if (operation?.ContainsKey("union") == true)
            {
                return new OpSetUnion(Parse(operation["left"].Value), Parse(operation["right"].Value));
            }
            operation = Parser.Matches(Parser.OpSetIntersectRegex, input)?.FirstOrDefault()?.Groups;
            if (operation?.ContainsKey("intersect") == true)
            {
                return new OpSetIntersect(Parse(operation["left"].Value), Parse(operation["right"].Value));
            }
            operation = Parser.Matches(Parser.OpVariableRegex, input)?.FirstOrDefault()?.Groups;
            if (operation?.ContainsKey("name") == true)
            {
                return new OpVariable(operation["name"].Value);
            }
*/
            return null;
        }

        public abstract DataNode GetResult(Dictionary<string, Variable> variables);
    }
}
