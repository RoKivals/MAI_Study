using System;
using System.Linq;
using System.Collections.Generic;

using RaGlib.Core;

namespace RaGlib.Automata {
  public class DeltaQSigmaGammaEx : DeltaQSigmaGamma {
    public List<Symbol> LHSZX {
      get;
      set;
    }
    public DeltaQSigmaGammaEx(Symbol LHSQ,Symbol LHSS,List<Symbol> LHSZ,
                              List<Symbol> RHSQ,List<Symbol> RHSZ)
    : base(LHSQ,LHSS,LHSZ.First(),RHSQ,RHSZ) {
      this.LHSZX=LHSZ;
    }

    public static implicit operator DeltaQSigmaGammaEx((Symbol LHSQ, Symbol LHSS, List<Symbol> LHSZ,
                                                        List<Symbol> RHSQ, List<Symbol> RHSZ) rule)
       => new DeltaQSigmaGammaEx(rule.LHSQ,rule.LHSS,rule.LHSZ,rule.RHSQ,rule.RHSZ);

    public override void Debug() {
      Console.Error.Write("δ({0}, {1}, ",LHSQ.symbol,! LHSS.IsEpsilon() ? LHSS.symbol : "ε");

      foreach (Symbol z in LHSZX)
        Console.Error.Write(!z.IsEpsilon() ? z.symbol : "ε");

      Console.Error.Write(") -> {{{0}, ",RHSQ.First().symbol);

      foreach (Symbol z in RHSZ)
        Console.Error.Write(! z.IsEpsilon() ? z.symbol : "ε");

      Console.Error.WriteLine("}");
    }

  }
}