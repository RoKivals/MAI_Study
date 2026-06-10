using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RaGlib.SetL
{
    public class Parser
    {
        public const string VariableRegex = "(?<name>\\w+)\\s*=\\s*(?<data>.+)";
        public const string SetRegex = "{\\s*(?<elements>.*)\\s*}";
        public const string OperationRegex = "^\\s*(?<operation>.+\\|.+|.+∈.+|.+\\=\\>.+|[a-zA-Z]\\*|[a-zA-Z])\\s*$";
        public const string ElementOperationRegex = "^\\s*(?<operation>.+∩.+|.+∪.+)\\s*$";
        public const string RulesSetRegex = "(?:\\s+|^)(?<rule>(?<from>[a-zA-Z])\\s*\\-\\>\\s*(?<to>[a-zA-Z]+))\\s*(?:\\,|$)";
        public const string ChainsSetRegex = "(?:\\s+|^)\'(?<chains>\\w+)\'\\s*(?:\\,|$)";
        public const string SymbolSetRegex = "(?:\\s+|^)(?<symbol>[a-zA-Z])\\s*(?:\\,|$)";
        public const string RuleRegex = "(?:\\s+|^)(?<from>[a-zA-Z])\\s*\\-\\>\\s*(?<to>[a-zA-Z]+)\\s*";
        public const string ChainRegex = "(?:\\s+|^)\'(?<chain>\\w+)\'";
        public const string SymbolRegex = "\\s*(?<symbol>[a-zA-Z])\\s*";

        public const string OpVariableRegex = "\\s*(?<name>\\w+)\\s*";
        public const string OpKliniRegex = "\\s*(?<symbol>\\w+)\\*\\s*";
        public const string OpFromRegex = "^(?<from>(?<left>[^∈]*)∈(?<right>[^$]*))$";
        public const string OpAutomateRegex = "^(?<automate>(?<left>[^\\=]*)\\=\\>(?<right>[^$]*))$";
        public const string OpConditionRegex = "^(?<condition>(?<left>[^\\|]*)\\|(?<right>[^$]*))$";
        public const string OpSetIntersectRegex = "^(?<intersect>(?<left>[^∩]*)∩(?<right>[^$]*))$";
        public const string OpSetUnionRegex = "^(?<union>(?<left>[^∪]*)∪(?<right>[^$]*))$";

        public static MatchCollection Matches(string pattern, string text)
        {
            var rgx = new Regex(pattern);
            return rgx.Matches(text);
        }
    }
}
