using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using RaGlib.Core;
using RaGlib.Automata;

namespace RaGlib {
  
  /// Push Down Automate МП = {}
  public class PDA : Automate {
    // Q - множество состояний МП - автоматa
    // Sigma - алфавит входных символов
    // Delta - правила перехода
    // Q0 - начальное состояние
    // F - множество конечных состояний
    public List<Symbol> Gamma = null; ///< Алфавит магазинных символов
    public Stack<Symbol> Z = null;
    public Symbol currState;
    
   // public List<DeltaQSigmaGamma> Delta = new List<DeltaQSigmaGamma>();

    /// МП для дельта-правил
    public PDA(List<Symbol> Q,List<Symbol> Sigma,List<Symbol> Gamma,Symbol Q0,Symbol z0,List<Symbol> F) {
      this.Q = Q;
      this.Sigma = Sigma;
      this.Gamma = Gamma;
      this.Q0 = Q0;
      this.Z = new Stack<Symbol>();
      Z.Push(z0);  // начальный символ в магазине    
      this.F = F;
      this.Delta = new List<DeltaQSigmaGamma>();      
    }

    /// Алгоритм построения МП  (PDA) по КС-грамматике
    public PDA(Grammar KCgrammar) {
      this.Q=new List<Symbol>() { "q" };
      this.Sigma=new List<Symbol>(KCgrammar.T); // множество входные символов
      this.F=new List<Symbol>(); //множество заключительных состояний
      this.Gamma=new List<Symbol>(KCgrammar.V); // множество магазинных символов

      foreach (var t in KCgrammar.T) // добавить в Gamma 
        Gamma.Add(t);
      this.Q0="q0";        // начальное состояние
      this.Z=new Stack<Symbol>();
      Z.Push(new Symbol("z0")); // символ дна в магазине
      Z.Push(KCgrammar.S0); // начальный символ в магазине

      this.Delta=new List<DeltaQSigmaGamma>();
      DeltaQSigmaGamma delta = null;

      // Algorithm  build DeltaQSigmaGamma by P !! доработать
      // 1. build эпсилон правила
      var rhsq = new List<Symbol>() { "q" }; // RHS for delta
      var rhsz = new List<Symbol>() { "ε" };
      int i = 0;

      foreach (var p in KCgrammar.P) {
        if (!KCgrammar.V.Contains(p.LHS.symbol)) {
          Console.Write($"This grammar cant have rule: {p.LHS.symbol} -> ");
          foreach (var s in p.RHS) {
            Console.Write(s.symbol);
          }
          Console.Write("\n");
          return;
        }
        foreach (var t in p.RHS) {
          if (!KCgrammar.T.Contains(t)&&!KCgrammar.V.Contains(t)) {
            Console.Write($"This grammar cant have rule: {p.LHS.symbol} -> ");
            foreach (var s in p.RHS) {
              Console.Write(s.symbol);
            }
            Console.Write("\n");
            return;
          }
        }
      }

      foreach (var p in KCgrammar.P) { // правила первого типа
        delta=new DeltaQSigmaGamma("q","ε",p.LHS.symbol,rhsq,p.RHS);
        Delta.Add(delta);
      }
      foreach (var t in KCgrammar.T) { // правила второго типа
        delta=new DeltaQSigmaGamma("q",t.symbol,t.symbol,rhsq,rhsz);
        Delta.Add(delta);
      }

      // delta(q0,ε,z0) = (qf,ε)
      delta=new DeltaQSigmaGamma("q0","ε","z0",new List<Symbol>{"qf"}, new List<Symbol>{"ε"});
      Delta.Add(delta);

    } // end Algorithm  and constructor

    public virtual void addDeltaRule(string LHSQ,string LHSS,string LHSZ,List<Symbol> RHSQ,List<Symbol> RHSZ) {
      this.Delta.Add(new DeltaQSigmaGamma(LHSQ,LHSS,LHSZ,RHSQ,RHSZ)); }
    public virtual void addDeltaRule(string LHSQ,string LHSS,string LHSZ,List<Symbol> RHSQ,List<Symbol> RHSZ,List<Symbol> RightSix) {
      this.Delta.Add(new DeltaQSigmaGammaSix(LHSQ,LHSS,LHSZ,RHSQ,RHSZ,RightSix)); }
    
    // for nonPDA ++ 
    public virtual bool Execute_(string str,int i,int Length) {
      //сразу нулевое правило брать
      var delta = (DeltaQSigmaGamma)this.Delta[0];
      currState = this.Q0;
      //      int i = 0;  // sas!!
      //int j = 0;
      str=str+"ε"; // empty step вставить "" не получается, так как это считается пустым символом,
                   //который не отображается в строке
                   // \n символ конца строки ??
                   // 
//      delta.Debug();
      for (;;)
      {
        if (delta==null) return false;
        if (delta.LHSS.Equals("")) { // И В ВЕРШИНЕ СТЕКА ТЕРМИНАЛЬНЫЙ СИМВОЛ LeftT!!!! пустой такт
          for (; i<str.Length;) { //модель считывающего устройства
            if (Z.Peek().ToString()==str[i].ToString()) {
              this.Z.Pop();
              currState = delta.RHSQ[0]; // Детерминированный 1 symbol 
              i++;
            } else
              return false;
            break;
          }
        } else if (delta.LHSS.Equals("")) // И В ВЕРШИНЕ СТЕКА НЕ ТЕРМИНАЛЬНЫЙ СИМВОЛ LeftT!!!!
          {
          //шаг 1 вытолкнуть из стека и занести в стек RHSZ
          this.Z.Pop();
           foreach (var symbol in delta.RHSZ)  
            this.Z.Push(symbol);
        }
        if (this.Z.Count!=0) {
          currState = delta.RHSQ[0]; // Determ avtyomat 1 symbol
          delta.Debug();
          // Execute_ (str,i, str.Length);
   //       delta = findDelta(currState,Z.Peek().ToString());
          delta.Debug();
        } else if (str[i].ToString()=="e") // end chain
          return true;
        else
          return false;
      } // end for

        //проверка на терминал или нетерминал в вершине стека
        //изменение правила по верхушке стека
    } // end Execute_


        public virtual bool Execute(string str) {      
          currState = this.Q0;
          int i = 0;     
          str = str +"ε";  // add greek symbol at the end       
          DeltaQSigmaGamma delta = null;

          for (;;) {
                 // delta(q0,ε,z0) = (qf,ε)
                delta = findDelta(Z.Peek());  // шаг 1. взять правило по содержимому стека  
                
                if (delta==null) return false; 
                Console.WriteLine(" step 1");
                delta.Debug();
                
                if (! delta.LHSS.symbol.Equals("ε")) { // not ε-yeild        
                      for (; i < str.Length;) {  //модель считывающего устройства
                            
                            if (str[i].Equals("ε")) return true; // end symbol chain
                            Console.WriteLine(" step 2 "+str[i] + "  "+delta.LHSS);     

                            if (str[i].ToString().Equals(delta.LHSS.symbol)) {     //δ(q, LHSS, z0) = (q, z0) нашлось правило, где LHSS = str[i]
                                  Console.WriteLine(" step 3 "+str[i]);
                                  if (delta.RHSZ[0].symbol.Equals("ε"))        //δ(q, LHSS, z0) = (q, ε (RHSZ)) удаление Z0 из стека, если RHSZ = ε 
                                this.Z.Pop();
                                else  // for example (q0,a,az0)               //δ(q, LHSS, z0) = (q, с (RHSZ)) добавляем с в стек --> Z = z0, c
                                this.Z.Push(delta.RHSZ[0]);  // надо все 1 symbol det PDA
                            } 
                        
                            else { //не нашли правило, в котором  LHSS = str[i], значит считываем другое
                                  // считать другое правило 
                                  delta=findDelta(str[i].ToString(),Z.Peek());
                                  if (delta==null) return false; // Exception 
                                  Console.WriteLine(" step 4 ");
                                  delta.Debug();
                                  if (delta==null) return false;  // не найдено
                                  if (delta.RHSZ[0].symbol.Equals("ε")) {
                                        Console.WriteLine(" step 5 "+this.Z.Peek().symbol);
                                        this.Z.Pop();
                                        if (delta.RHSQ[0].symbol.Equals("qf")) return true; // delta(q0,ε,z0) = (qf,ε) stack пуст
                                  } 
                                  else  // for example (q0,a,az0)
                                        this.Z.Push(delta.RHSZ[0]);  // 1 symbol det PDA
                            }        
                            i++;     
                            if (str[i] == 'ε') return true;
                        break;
                      } // end for 
                } 
                else { // эпсилон (empty) такт       // delta(q0,ε,z0) = (qf,а) удаляем из стека символ и вставляем в него "а"                  
                    this.Z.Pop();
                    delta.RHSZ.Reverse();
                    foreach (var symbol in delta.RHSZ)
                        this.Z.Push(symbol);
                } // end if        
                currState = delta.RHSQ[0]; // Determ automat 1 symbol

          } // end for
    } // end Execute    |- 

        /// Поиск правила delta (q0,a,z0) по символу "a" и в вершине магазина "z0"
        public DeltaQSigmaGamma findDelta(string sigma, Symbol z) {
      foreach (var delta in this.Delta) 
        if (delta.LHSS.symbol.Equals(sigma) &&
            delta.LHSZ.symbol.Equals(z.symbol)) return delta;
      return null; // not found
    }
    // Поиск правила delta (q0,a,z0) по символу в вершине магазина "z0"
    public DeltaQSigmaGamma findDelta(Symbol z) {
            foreach (var delta in this.Delta)
                if (delta.LHSZ.symbol.Equals(z.symbol)) return delta;
            return null; // not found
        }
    public void Debug() {
      if (Delta.Count==0) {
        Console.Write("PDA has no one delta rule\n");
      } else {
        Console.Write("Delta rules:\n");
        foreach (var delta in this.Delta) {
          delta.Debug();
        }
      }
    }

  } // end class PDA

}