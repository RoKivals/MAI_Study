using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaGlib.SetL
{
    public class DataSymbol: DataNode
    {
        public char Symbol { get; }

        public DataSymbol(string text)
        {
           // sas Symbol = Parser.Matches(Parser.SymbolRegex, text).First().Groups["symbol"].Value[0];
        }

        public override bool Equals(object obj)
        {
            return obj is DataSymbol o && o.Symbol == Symbol;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return Symbol.ToString();
        }
    }
}
