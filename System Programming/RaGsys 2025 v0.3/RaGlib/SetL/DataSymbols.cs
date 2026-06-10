using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RaGlib.SetL
{
    public class DataSymbols: DataNode
    {
        public List<DataSymbol> Symbols { get; }

        public DataSymbols(string text)
        {
            Symbols = new List<DataSymbol>();
            var elements = Parser.Matches(Parser.SymbolSetRegex, text);
            foreach(var element in elements)
            {
                Symbols.Add(new DataSymbol((element as Match).Groups["symbol"].ToString()));
            }
        }

        public DataSymbols(IEnumerable<DataSymbol> symbols)
        {
            Symbols = symbols.ToList();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("{");
            for(int i = 0; i < Symbols.Count; i++)
            {
                sb.Append(Symbols[i]).Append(i == Symbols.Count - 1 ? "}" : ", ");
            }
            if(Symbols.Count == 0)
            {
                sb.Append("}");
            }
            return sb.ToString();
        }
    }
}
