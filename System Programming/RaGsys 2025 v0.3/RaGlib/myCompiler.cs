using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using RaGlib.Core;
using RaGlib.Automata;

namespace RaGlib {
    public abstract class Automate
    {
        public List<Symbol> Q = null; ///< множество состояний
        public List<Symbol> Sigma = null; ///< множество алфавит
        public dynamic Delta = null;  ///< множество правил перехода
        public Symbol Q0 = null; ///< начальное состояние
        public List<Symbol> F = null; ///< множество конечных состояний
        private List<Symbol> config = new List<Symbol>();
        private List<DeltaQSigma> DeltaD = new List<DeltaQSigma>(); ///< правила детерминированного автомата

        public Automate() {}

        public Automate(List<Symbol> Q, List<Symbol> Sigma, List<Symbol> F, Symbol q0)
        {
            this.Q = Q;
            this.Sigma = Sigma;
            this.Q0 = q0;
            this.F = F;
            this.Delta = new List<DeltaQSigma>();
        }

        public void AddRule(string state, string term, string nextState) { this.Delta.Add(new DeltaQSigma(state, term, new List<Symbol> { new Symbol(nextState) })); }

        //для пустого символа "" currStates добавляются в ArrayList - ReachableStates

        private List<Symbol> EpsClosure(List<Symbol> currStates)
        {
            // Debug("Eps-Closure", currStates);
            return EpsClosure(currStates, null);
        }

        /// Все достижимые состояния из множества состояний states
        /// по правилам в которых ,LeftTerm = term
        private List<Symbol> EpsClosure(List<Symbol> currStates, List<Symbol> ReachableStates)
        {
            if (ReachableStates == null)
                ReachableStates = new List<Symbol>();
            List<Symbol> nextStates = null;
            var next = new List<Symbol>();
            int count = currStates.Count;
            for (int i = 0; i < count; i++) {
              
                nextStates = FromStateToStates(currStates[i].ToString(), "");

                // 1. если nextStates = null и это e-clouser
                if (!ReachableStates.Contains(currStates[i])) {
                    ReachableStates.Add(new Symbol(currStates[i].ToString()));
                    
                }
                if (nextStates != null) {
                  
                    // 1. из одного состояния возможен переход в несколько состояний,
                    //но это состояние в множестве должно быть только один раз,
                    //то есть для него выполняется операция объединения
                    foreach (var nxt in nextStates) {
                        ReachableStates.Add(nxt);
                        next.Add(nxt);
                    }
                    
                }
            }
            
            if (nextStates == null)
                return ReachableStates;
            else
                return EpsClosure(next, ReachableStates);
        }
        /// Возвращает множество достижимых состояний по символу term
        /// из currStates за один шаг
        private List<Symbol> move(List<Symbol> currStates, string term)
        {
            var ReachableStates = new List<Symbol>();
            var nextStates = new List<Symbol>();
            foreach (var s in currStates) {
                nextStates = FromStateToStates(s.symbol, term);
                if (nextStates != null)
                    foreach (var st in nextStates)
                        if (!ReachableStates.Contains(st))
                            ReachableStates.Add(st);
            }
            return ReachableStates;
        }

        /// Все состояния в которые есть переход из текущего состояния currState
        /// по символу term за один шаг
        private List<Symbol> FromStateToStates(string currState, string term) {
            var NextStates = new List<Symbol>(); //{currState};
            bool flag = false;
            foreach (var d in Delta) {
                if (d.LHSQ == currState && d.LHSS == term) {
                    NextStates.Add(new Symbol(d.RHSQ[0].ToString()));
                    flag = true;
                }
            }
            if (flag) return NextStates;            
            return null;
        }

        private void BuildWithQueue(Symbol Q0)
        {
            List<List<Symbol>> queue = new List<List<Symbol>>(); // Имитируем очередь, ибо нельзя сделать queue<list<symbol>>
            List<Symbol> curStates = null;
            List<Symbol> newStates = null;
            bool is_start = true;
            queue.Add(new List<Symbol> { Q0 });
            while (queue.Count != 0)
            {
                curStates = EpsClosure(queue[0]);
                queue.RemoveAt(0);
                foreach (var a in Sigma)
                {
                    newStates = move(curStates, a.symbol);
                    if (!config.Contains(SetName(EpsClosure(newStates))))
                    {
                        if (is_start)
                        {
                            config.Add(SetName(curStates));
                            is_start = false;
                        }
                        queue.Add(newStates);
                        config.Add(SetName(EpsClosure(newStates)));
                    }
                    if (newStates.Count != 0)
                    {
                        if (SetName(curStates) != SetName(EpsClosure(newStates)))
                        {
                            var delta = new DeltaQSigma(SetName(curStates), a.symbol, new List<Symbol> { SetName(EpsClosure(newStates)) });
                            DeltaD.Add(delta);
                        }

                    }
                }
            }
        }


        private void Dtran(List<Symbol> currState)
        {
            List<Symbol> newStates = null;
            foreach (var a in Sigma) {
                newStates = EpsClosure(move(currState, a.symbol));
                Debug("Dtran " + a.symbol + " " + a.symbol, newStates); 
                if (SetName(newStates) != null) {
                    if (SetName(currState) != SetName(newStates))
                    {
                        var delta = new DeltaQSigma(SetName(currState), a.symbol, new List<Symbol> { SetName(newStates) });
                        DeltaD.Add(delta);
                    }     
                }
                if (!config.Contains(SetName(newStates)))
                {
                   
                    config.Add(SetName(newStates));
                    Dtran(newStates);
                }
                Console.WriteLine("Building completed");
            }
        }

        /// Построить Delta-правила ДКА
        public void BuildDeltaDKAutomate(FSAutomate ndka, bool with_queue) {
            this.Sigma = ndka.Sigma;
            this.Delta = ndka.Delta;
            if (with_queue)
            {
                BuildWithQueue(ndka.Q0);
            } else {
                List<Symbol> currState = EpsClosure(new List<Symbol>() { ndka.Q0 });
                config.Add(new Symbol(SetName(currState)));
                Dtran(currState);
            }
           
            this.Q = config;
            this.Q0 = this.Q[0].ToString();
            this.Delta = DeltaD;
            this.F = getF(config, ndka.F);
        }

        private List<Symbol> getF(List<Symbol> config, List<Symbol> F) {
            var F_ = new List<Symbol>();
            foreach (var f in F) {
                foreach (var name in this.config) {
                    if (name.symbol != null && name.symbol.Contains(f.symbol)) {
                        F_.Add(name);
                    }
                }
            }
            return F_;
        }

        /// Состояние StateTo достижимо по дельта-правилам из состояния currState
        private bool ReachableStates(string currState, string StateTo) {
            string nextstate = currState;
            bool b = true;
            if (currState == StateTo) return false;
            while (b) {
                b = false;
                foreach (var d in this.Delta) {
                    if (nextstate == d.LHSQ)  {
                        if (nextstate == StateTo) return true;
                        nextstate = d.RHSQ[0].symbol; // DFS
                        b = true;
                        break;
                    }
                }
            }
            return false;
        } // end ReachableStates

        private Hashtable names = new Hashtable();

        private List<Symbol> makeNames(List<Symbol> config) {
            var Names_ = new List<Symbol>(); // new names
            for (int i = 0; i < config.Count; i++) {
                Names_.Add(new Symbol(i.ToString()));
            }
            return Names_;
        }

        private List<DeltaQSigma> NameRules(List<DeltaQSigma> D) {
            var D_ = new List<DeltaQSigma>(); // new delta functions
            string LHSQ = null;
            var RHS = new List<Symbol>();

            foreach (var d in D) {
                for (int i = 0; i < this.config.Count; i++) {
                    if (d.LHSQ == this.config[i].ToString())
                        LHSQ = this.Q[i].symbol;
                }
                for (int i = 0; i < this.Q.Count; i++) {
                    if (d.RHSQ[0].symbol == this.config[i].ToString().ToString()) // DFS
                        RHS.Add(new Symbol(this.Q[i].symbol));
                }
                D_.Add(new DeltaQSigma(LHSQ, d.LHSQ, RHS));
            }
            return D_;
        }

        private string SetName(List<Symbol> list)  {
            string line = null;
            if (list == null)  {
                return "";
            }
      foreach (var sym in list)
        line+=sym.symbol; 
            return line;
        }

        //***  Debug ***//
        public void Debug(string step, string line)
        {
            Console.Write(step + ": ");
            Console.WriteLine(line);
        }

        public void Debug(string step, List<Symbol> list)
        {
            Console.Write(step + ": ");
            if (list == null)
            {
                Console.WriteLine("null");
                return;
            }
            for (int i = 0; i < list.Count; i++)
                if (list[i] != null)
                    Console.Write(list[i].ToString() + " ");
            Console.Write("\n");
        }

        public void Debug(List<Symbol> list)
        {
            Console.Write("{ ");
            if (list == null)
            {
                Console.WriteLine("null");
                return;
            }
            for (int i = 0; i < list.Count; i++)
                Console.Write(list[i].ToString() + " ");
            Console.Write(" }\n");
        }

        public void DebugAuto() {
            Console.WriteLine("\nAutomate definition:");
            Debug("Q", this.Q);
            Debug("Sigma", this.Sigma);
            Debug("Q0", this.Q0.symbol);
            Debug("F", this.F);
            Console.WriteLine("DeltaList:");
            foreach (var d in this.Delta)
              d.Debug(); 
        }
    } // end class Automate

}
