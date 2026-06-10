using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaGlib.SetL
{
    public class OpKlini: OpNode
    {
        private const int _maxLength = 4;
        private OpVariable _variable;

        public OpKlini(OpVariable variable)
        {
            _variable = variable;
        }

        public override DataNode GetResult(Dictionary<string, Variable> variables)
        {
            return GetKliniChains(variables[_variable.Name].Data as DataSymbols);
        }

        private DataChains GetKliniChains(DataSymbols symbols)
        {
            var result = new DataChains("");
            //result.Chains.Add(new DataChain(""));
            var strs = new Stack<string>();
            foreach(var c in symbols.Symbols)
            {
                strs.Push(c.ToString());
            }
            while(strs.Count > 0)
            {
                var top = strs.Pop();
                if(top.Length > _maxLength)
                {
                    continue;
                }
                result.Chains.Add(new DataChain(top));
                foreach (var c in symbols.Symbols)
                {
                    strs.Push(top + c.ToString());
                }
            }
            return result;
        }
    }
}
