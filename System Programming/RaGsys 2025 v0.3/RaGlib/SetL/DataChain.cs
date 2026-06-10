using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaGlib.SetL
{
    public class DataChain: DataNode
    {
        public List<DataSymbol> Symbols { get; }

        public DataChain(string text)
        {
            //Symbols = new();
            Symbols = new List<DataSymbol>();
            foreach (var c in text.ToCharArray())
            {
                Symbols.Add(new DataSymbol(c.ToString()));
            }
        }

        public DataChain(List<DataSymbol> symbols)
        {
            Symbols = symbols;
        }

        public override bool Equals(object obj)
        {
            return obj is DataChain o && Symbols.Intersect(o.Symbols).Count() == Symbols.Count;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach(var symbol in Symbols)
            {
                sb.Append(symbol);
            }
            return sb.ToString();
        }
    }
}
