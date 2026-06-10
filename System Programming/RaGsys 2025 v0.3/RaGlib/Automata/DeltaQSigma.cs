using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaGlib.Core;  

namespace RaGlib.Automata {
  /// Delta: Q x Sigma -> Q
  public  class DeltaQSigma {
    public Symbol LHSQ { set; get; } = null; ///< Q
    public Symbol LHSS { set; get; } = null; ///< Sigma
    public List<Symbol> RHSQ { set; get; } = null; ///< Q
    public DeltaQSigma(Symbol LHSQ,Symbol LHSS,List<Symbol> RHSQ) {
      this.LHSQ=LHSQ;
      this.LHSS=LHSS;
      this.RHSQ=RHSQ;
    }
    public virtual void Debug() {
      Console.WriteLine($" delta({String.Join(",",this.LHSQ)},"+
                               $"{String.Join(",",this.LHSS)},"+
                               $"-> ({String.Join(",",this.RHSQ)}");
    } // end Debug
  } // end DeltaQSigma
}
