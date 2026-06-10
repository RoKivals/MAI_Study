using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaGlib.Core;

namespace RaGlib.Grammars {
    public class AttrProduction : Production
    {
        // продукция c функциями для аттрибутов
        public List<AttrFunction> F; // all functions for attributes (also copy functions a + b, a <- b  )
        public AttrProduction(Symbol LHS,List<Symbol> RHS,List<AttrFunction> F) :
            base (LHS,RHS) {
            this.F   = new List<AttrFunction>(F);
        }
        public AttrProduction(Symbol LHS,List<Symbol> RHS) : base(LHS,RHS) { }
        public void print() {
            LHS.print();
            Console.Write(" -> ");
            for (int i = 0; i < RHS.Count; ++i)
            {
                RHS[i].print();
            }
            Console.Write("\n");
            for (int i = 0; i < F.Count; ++i)
            {
                F[i].print();
                if (i != (F.Count - 1))
                    Console.Write("\n");
            }
        }
    } // end class AttrProduction
}
