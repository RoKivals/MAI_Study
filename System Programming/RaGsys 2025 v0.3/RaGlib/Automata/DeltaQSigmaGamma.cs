using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaGlib.Core;

namespace RaGlib.Automata {
  public  class DeltaQSigmaGamma : DeltaQSigma {
    public Symbol LHSZ { get; set; } ///< Верхний символ магазина
    public List<Symbol> RHSZ { get; set; } ///< Множество символов магазина
                                           /// Отображение                         // так же для недетерминированного автомата
                                           /// Delta (  q1   ,   a    ,   z0   ) = {  {q, az0}, ???{q2,z0z2...  }  }
                                           ///         LHSQ     LHSS      LHSZ       RHSQ RHSZ
    public DeltaQSigmaGamma(Symbol LHSQ, Symbol LHSS, Symbol LHSZ,
                            List<Symbol> RHSQ, List<Symbol> RHSZ)
    : base(LHSQ,LHSS,RHSQ)
    {
      this.LHSZ=LHSZ;
      this.RHSZ=RHSZ;
    }

    public static implicit operator DeltaQSigmaGamma((Symbol LHSQ, Symbol LHSS, Symbol LHSZ,
                                                      List<Symbol> RHSQ, List<Symbol> RHSZ) rule)
        => new DeltaQSigmaGamma(rule.LHSQ, rule.LHSS, rule.LHSZ, rule.RHSQ, rule.RHSZ);

    public override void Debug() {
      Console.WriteLine($" delta({String.Join(",",LHSQ)},"+
                               $"{String.Join(",",LHSS)},"+
                               $"{String.Join(",",this.LHSZ)}) "+
                               $"-> ({String.Join(",",RHSQ)},"+
                               $"{String.Join(",",this.RHSZ)})");
    } // end debug

  } // end class DeltaQSigmaGamma

  class DeltaQSigmaGammaSix : DeltaQSigmaGamma {

    /// Delta (  q1   ,   a    ,   z   ) = {  {q}   ,   {z1z2...} }
    /// Delta (  q1   ,   a    ,   z   ) = {  {q}   ,   {z1z2...}, {b1b2.....} }
    /// RightO b1,b2 выходные операционные символы
    ///         LHSQ    LHSS     LHSZ         RHSQ       RHSZ        RHSNew
    public DeltaQSigmaGammaSix(string LeftQ,string LeftT,string LeftZ,List<Symbol> RightQ,List<Symbol> RightZ,List<Symbol> RightSix) : base(LeftQ,LeftT,LeftZ,RightQ,RightZ) { this.rightSix=RightSix; }

    public List<Symbol> rightSix { get; set; }
  } // end class DeltaQSigmaGammaSix

}
