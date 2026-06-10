using RaGlib.SetL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaGlib.SETL
{
    public class OpSetUnion : OpNode
    {
        private OpNode _left;
        private OpNode _right;

        public OpSetUnion(OpNode left, OpNode right)
        {
            _left = left;
            _right = right;
        }

        public override DataNode GetResult(Dictionary<string, Variable> variables)
        {
            var lr = _left.GetResult(variables);
            var rr = _right.GetResult(variables);
            {
                if (lr is DataSymbols _lr && rr is DataSymbols _rr)
                {
                    var l = _lr.Symbols.Select(ds => new Core.Symbol(ds.Symbol.ToString())).ToList();
                    var r = _rr.Symbols.Select(ds => new Core.Symbol(ds.Symbol.ToString())).ToList();
                    return new DataSymbols(Set.Union(l, r).Select(cs => new DataSymbol(cs.symbol)));
                }
            }
            {
                if (lr is DataChains _lr && rr is DataChains _rr)
                {
                    var l = _lr.Chains.Select(ds => new DataChain(ds.Symbols)).ToList();
                    var r = _rr.Chains.Select(ds => new DataChain(ds.Symbols)).ToList();
                    return new DataChains(l.Union(r).ToList());
                }
            }
            return null;
        }
    }
}
