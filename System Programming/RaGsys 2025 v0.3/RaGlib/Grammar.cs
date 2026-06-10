// ============================================================================
// Grammar.cs - Класс для представления контекстно-свободной грамматики
// ============================================================================
// Класс Grammar представляет контекстно-свободную грамматику (КС-грамматику),
// состоящую из терминалов, нетерминалов, правил продукции и начального символа.
// Поддерживает преобразование грамматики в конечный автомат и различные операции
// над грамматиками (приведение к нормальным формам, проверка принадлежности и т.д.).
// ============================================================================

using System;
using System.Collections.Generic;
using RaGlib.Core;

namespace RaGlib {  
    public class Grammar : AGrammar  {
        // Конструктор грамматики с терминалами, нетерминалами и начальным символом
        public Grammar(List<Symbol> T, List<Symbol> V, string S0) : base(T, V, S0)
        {
            Production.Count = 0; 
        }
        
        // Конструктор грамматики с готовым списком продукций
        public Grammar(List<Symbol> T, List<Symbol> V, List<Production> production, string S0) : base(T, V, S0)
        {
            Production.Count = production.Count;
            this.P = production;
        }

        // Конструктор по умолчанию - создает пустую грамматику
        public Grammar() : base() { Production.Count = 0; }

        // Метод вывода (не реализован в базовом классе)
        public override void Inference() {}

        // Проверка, является ли символ нетерминалом грамматики
        public bool isNoTerm(string v)
        {
            foreach (var vi in this.V)
                if (v.Equals(vi.symbol))
                    return true;
            return false;
        }

        // Преобразование грамматики в эквивалентный конечный автомат
        public FSAutomate Transform()
        {
            var Q = this.V;
            Q.Add(new Symbol("qf"));
            var q0 = this.S0;
            var F = new List<Symbol>();
            //Конструируем множество заключительных состояний
            foreach (var p in this.P)
            {
                //Если начальный символ переходит в конечную цепочку,
                //то в множество F добавляется начальный символ S0 и состояние qf
                // F = {S0, qf}
                if (p.LHS.symbol.Contains("S0") && p.RHS.Contains(new Symbol("e")))
                {
                    F = new List<Symbol> { p.LHS, new Symbol("qf") };
                    break;
                }
                //Иначе F = {qf} множество F(конечных состояний) будет состоять из одного состояния qf
                else if (p.LHS.symbol.Contains("S0"))
                {
                    F = new List<Symbol> { new Symbol("qf") };
                    break;
                }
            }

            //Конструируем конечный автомат
            FSAutomate KA = new FSAutomate(Q, this.T, F, q0.symbol);
            bool flag = true;

            foreach (var p in this.P)
            {
                //Если существует правило порождения,
                //в котором из начального символа существует переход в пустую цепочку,
                //то создаем правило (S0, "e", "qf")
                if (flag && p.LHS.symbol.Contains("S0") && p.RHS.Contains(new Symbol("e")))
                {
                    KA.AddRule(p.LHS.symbol, "e", "qf");
                    flag = false;
                }
                //Проходим по всем входным символам
                foreach (var t in this.T)
                {
                    //Если справа есть символ и этот символ терминал,
                    //то добавляем правило (Нетерминал -> (Терминал,  "qf"))
                    if (p.RHS.Contains(t) && NoTermReturn(p.RHS) == null)
                        KA.AddRule(p.LHS.symbol, t.symbol, "qf");
                    //Если справа есть символ и этот символ нетерминал,
                    //то добавляем правило (Нетерминал -> (Терминал, Нетерминал))
                    else if (p.RHS.Contains(t) && NoTermReturn(p.RHS) != null)
                        KA.AddRule(p.LHS.symbol, t.symbol, NoTerminal(p.RHS));
                }
            }
            return KA;
        }

        // Проверка, является ли символ терминалом грамматики
        public bool isTerm(string t)
        {
            foreach (var ti in this.T)
                if (t.Equals(ti.symbol))
                    return true;
            return false;
        }

    // -------------------------- FIRST & FOLLOW SETS --------------------------
    // Множества FIRST и FOLLOW используются для LL и LR анализаторов
    private Dictionary<Symbol,HashSet<Symbol>> FirstSet =
      new Dictionary<Symbol,HashSet<Symbol>>();

    private Dictionary<Symbol,HashSet<Symbol>> FollowSet =
      new Dictionary<Symbol,HashSet<Symbol>>();

    // Вычисление множеств FIRST и FOLLOW для всех нетерминалов
    public void ComputeFirstFollow() {
      ComputeFirstSets();
      ComputeFollowSets();
    }

    // Вычисление множества FIRST для всех символов грамматики
    private void ComputeFirstSets() {
      FirstSet.Clear();
      foreach (var term in T)
        FirstSet[term]=new HashSet<Symbol>() { term }; // FIRST[c] = {c}
      FirstSet[Symbol.Epsilon]=new HashSet<Symbol>() { Symbol.Epsilon }; // для единообразия
      foreach (Symbol noTerm in V)
        FirstSet[noTerm]=new HashSet<Symbol>(); // First[X] = empty set
      bool changes = true;
      while (changes) {
        changes=false;
        foreach (var rule in P) {
          // Для каждого правила X-> Y0Y1…Yn
          var X = rule.LHS;
          foreach (var Y in rule.RHS) {
            foreach (var curFirstSymb in FirstSet[Y]) {
              if (FirstSet[X].Add(curFirstSymb)) // Добавить а в FirstSets[X]
              {
                changes=true;
              }
            }
            if (!FirstSet[Y].Contains(Symbol.Epsilon)) {
              break;
            }
          }
        }
      } // пока вносятся изменения
    }

    public HashSet<Symbol> First(Symbol X) {return FirstSet[X];}

    public HashSet<Symbol> First(List<Symbol> X) {
      var result = new HashSet<Symbol>();
      foreach (Symbol Y in X) {
        foreach (Symbol curFirstSymb in FirstSet[Y]) {
          result.Add(curFirstSymb);
        }
        if (!FirstSet[Y].Contains(Symbol.Epsilon)) {
          break;
        }
      }
      return result;
    }

    private void ComputeFollowSets() {
      foreach (Symbol noTerm in V)
        FollowSet[noTerm]=new HashSet<Symbol>();
      FollowSet[S0].Add(Symbol.Sentinel);
      bool changes = true;
      while (changes) {
        changes=false;
        foreach (Production rule in P) {
          // Для каждого правила X-> Y0Y1…Yn
          for (int indexOfSymbol = 0; indexOfSymbol<rule.RHS.Count; ++indexOfSymbol) {
            Symbol curSymbol = rule.RHS[indexOfSymbol];
            if (T.Contains(curSymbol)||curSymbol==Symbol.Epsilon) {
              continue;
            }
            if (indexOfSymbol==rule.RHS.Count-1) {
              foreach (Symbol curFollowSymbol in FollowSet[rule.LHS]) {
                if (FollowSet[curSymbol].Add(curFollowSymbol)) {
                  changes=true;
                }
              }
            } else {
              var curFirst = First(rule.RHS[indexOfSymbol+1]);
              bool epsFound = false;
              foreach (Symbol curFirstSymbol in curFirst) {
                if (curFirstSymbol!=Symbol.Epsilon) {
                  if (FollowSet[rule.RHS[indexOfSymbol]].Add(curFirstSymbol)) {
                    changes=true;
                  }
                } else {
                  epsFound=true;
                }
              }
              if (epsFound) {
                foreach (Symbol curFollowSymbol in FollowSet[rule.LHS]) {
                  if (FollowSet[rule.RHS[indexOfSymbol]].Add(curFollowSymbol)) {
                    changes=true;
                  }
                }
              }
            }
          }
        }
      }
    }

    public HashSet<Symbol> Follow(Symbol X) {
      if (FollowSet.ContainsKey(X)) return FollowSet[X];
      return new HashSet<Symbol>();
    }

     public Grammar LeftRecursDelete_new6() {
            var Ph = new List<Production>(this.P);//изначальные правила
            var P = new List<Production>(); // выходные правила 
            var V1 = new List<Symbol>(this.V); //вывод - нетерминалы
            var Pdel = new List<Production>(); //хранение удаляемых правил 
            int i = 1; int j = 1;
            Console.WriteLine("Left recursion:");

            foreach (Symbol vi in this.V) {

                j = 1;
                var Ph_h = new List<Production>();//добавление правил из циклов (вспомогательное множество)
                                                 //следующие циклы выполняются только при i>1 
                foreach (Symbol vj in this.V)
                {
                    if (j < i)
                    {
                        foreach (Production r in Ph)
                        {
                            if (r.LHS == vi && r.RHS[0].ToString() == vj) // проходим по правилам вида Ai -> Aj gamma
                            {
                                foreach (Production r1 in Ph)
                                {
                                    if (r1.LHS == vj && !Pdel.Contains(r1))  // проходим по правилам вида  Aj-> xk
                                    {
                                        //цикл для добавления  правил вида Ai -> xk gamma (нахождение косвенной рекурсии)
                                        List<Symbol> gamma = new List<Symbol>(r1.RHS); // составление правой части  (добавлена часть xk)

                                        for (int ii = 1; ii < r.RHS.Count; ii++) // составление правой части  (добавлена часть gamma)
                                        {
                                            gamma.Add(r.RHS[ii]);
                                        }

                                        Ph_h.Add(new Production(vi, gamma));
                                        Pdel.Add(r); //добавление неиспользуемых правил 
                                    }
                                }
                            }

                        }

                    }
                    j++;
                    //цикл для добавления  новых правил в Ph. Нужен только в реализации 
                    foreach (Production p in Ph_h)
                    {
                        if (!Ph.Contains(p)) { Ph.Add(p); }
                    }
                }


                var Vr  = new List<Symbol>(); //хранение нетерминала с левой рекурсией
                                                      //для определения в правилах  левой рекурсией для нетерминала Ai
                foreach (Production p in Ph)
                {
                    if (p.LHS == p.RHS[0].ToString())
                    {
                        if (!Vr.Contains(p.LHS))
                        {
                            Vr.Add(p.LHS);
                        }
                    }
                }



                //добавление правил, если присутствует левая рекурсия 
                foreach (Symbol vr in Vr) {
                    foreach (Production r in Ph) {
                        if (r.LHS == vi && vr == r.LHS && !Pdel.Contains(r))
                        {
                            Symbol new_v = vi + "'"; //создание нового элемента
                            if (!V1.Contains(new_v)) { V1.Add(new_v); } //добавление во множество нетерминалов

                            if (r.LHS == r.RHS[0].ToString()) {
                                DebugPrule(r);
                                var a_h = new List<Symbol>();// для создания правой части в правилах вида A'->alpha из  A->Aalpha
                                var a_a = new List<Symbol>();//для создания правой части в правилах вида A'->alpha A'  из  A->Aalpha
                                for (int ii = 1; ii < r.RHS.Count; ii++) //создание правой части в правилах вида A'->alpha
                                {
                                    a_h.Add(r.RHS[ii]);
                                }

                                P.Add(new Production(new_v, a_h)); //добавление правил в конечное множество
                                Ph_h.Add(new Production(new_v, a_h));
                                for (int ii = 1; ii < r.RHS.Count; ii++) //создание правой части в правилах вида A'->alpha A'
                                {
                                    a_a.Add(r.RHS[ii]);
                                }
                                a_a.Add(new_v);
                                P.Add(new Production(new_v, a_a)); //добавление правил в конечное множество
                                Ph_h.Add(new Production(new_v, a_a));
                                Pdel.Add(r);

                            }
                            if (r.LHS != r.RHS[0].ToString())
                            {
                                var b_h = new List<Symbol>();//для создания правой части в правилах вида A-> beta
                                var b_b = new List<Symbol>();//для создания правой части в правилах вида A-> betaA'
                                for (int ii = 0; ii < r.RHS.Count; ii++) //создания правой части в правилах вида A-> beta
                                {
                                    b_h.Add(r.RHS[ii]);
                                }
                                P.Add(new Production(vi, b_h)); //добавление правил в конечное множество
                                                           //Ph_h.Add(new Prule(vi, b_h));
                                for (int ii = 0; ii < r.RHS.Count; ii++) //создание правой части в правилах вида A->betaA'
                                {
                                    b_b.Add(r.RHS[ii]);
                                }
                                b_b.Add(new_v);
                                P.Add(new Production(vi, b_b)); //добавление правил в конечное множество
                                Ph_h.Add(new Production(vi, b_b)); //добавление правил вначальное множество при помощи вспомогательного множества

                            }
                        }
                    }
                }

                //добавление правил, если левой рекурсии не было 
                foreach (Production p in Ph)
                {
                    if (p.LHS == vi)
                    {
                        if (!Vr.Contains(p.LHS) && !P.Contains(p) && !Pdel.Contains(p))
                        {
                            P.Add(p);
                        }
                    }
                }
                //цикл для добавления  новых правил в Ph. Нужен только в реализации
                foreach (Production ph_h in Ph_h)
                {
                    if (!Ph.Contains(ph_h)) { Ph.Add(ph_h); }
                }
                i++;
            }
            Console.WriteLine("\nLeft Recursion delete.");
  
            return new Grammar(this.T, V1, P, this.S0.symbol);
    } // end LeftRecursDelete_new6
  } // end class Grammar

}
