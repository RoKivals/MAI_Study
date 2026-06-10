using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaGlib.Core {
  public class Symbol_Attribute : Symbol {
    public Symbol_Attribute(string s,List<Symbol> a,List<Symbol> L,List<Symbol> R) :
        base(s,a) {
      //      Console.WriteLine("### Symbol_Attribute" );
    }
  }
}
