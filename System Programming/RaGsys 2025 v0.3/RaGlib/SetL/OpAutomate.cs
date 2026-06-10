using RaGlib;
using RaGlib.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaGlib.SetL
{
    public class OpAutomate: OpNode
    {
        private OpVariable _startSymbol;
        private OpVariable _chain;

        public OpAutomate(OpVariable startSymbol, OpVariable chain)
        {
            _startSymbol = startSymbol;
            _chain = chain;
        }

        public override DataNode GetResult(Dictionary<string, Variable> variables)
        {
            var startSymbol = (variables[_startSymbol.Name].Data as DataSymbol).Symbol;
            var chain = (variables[_chain.Name].Data as DataChain).ToString();
            var T = new List<Symbol>();
            var V = new List<Symbol>();
            foreach(var t in (variables["T"].Data as DataSymbols).Symbols)
            {
                T.Add(new Symbol(t.ToString()));
            }
            foreach (var v in (variables["V"].Data as DataSymbols).Symbols)
            {
                V.Add(new Symbol(v.ToString()));
            }
            var LL = new Grammar(T, V, startSymbol.ToString());
            foreach (var p in (variables["P"].Data as DataRules).Rules)
            {
                var to = new List<Symbol>();
                foreach(var s in p.To.Symbols)
                {
                    to.Add(new Symbol(s.ToString()));
                }
                LL.AddRule(p.From.Symbol.ToString(), to);
            }
            var stdout = Console.Out;
            Console.SetOut(TextWriter.Null);
            var parser = new LLParser(LL);
            Console.SetOut(stdout);
            var chain1 = new List<Symbol>();
            foreach (var x in chain)
                chain1.Add(new Symbol(x.ToString()));
            return new DataBool(parser.Parse(chain1));
        }
    }
}
