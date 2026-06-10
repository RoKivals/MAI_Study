using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Data;
using System.Text;
using RaGlib.Core;
using RaGlib.Automata;

namespace RaGlib {
   public class SLRGrammar : Grammar
    {
        /// Пара символ-число
        protected class PairSymbInt : IEquatable<PairSymbInt>
        {
            public Symbol First { get; set; }
            public int Second { get; set; }

            public PairSymbInt() {}

            public PairSymbInt(char c, int num)
            {
                First = new Symbol(c.ToString());
                Second = num;
            }

            public PairSymbInt(string str, int num)
            {
                First = new Symbol(str);
                Second = num;
            }

            public PairSymbInt(Symbol sym, int num)
            {
                First = sym;
                Second = num;
            }

            /// Проверка двух пар на равенство
            public bool Equals(PairSymbInt pair)
            {
                return (pair.Second == Second) && Equals(pair.First, First);
            }

            public void Debug()
            {
                Console.WriteLine(First + " " + Second.ToString());
            }
        }

        /// Компаратор пар. Требуется для корректной работы Dictionary
        protected class PairComparer : IEqualityComparer<PairSymbInt>
        {
            /// Метод проверки равенства
            public bool Equals(PairSymbInt lhs, PairSymbInt rhs)
            {
                return lhs.Equals(rhs);
            }

            /// Метод вычисления хеш-функции
            public int GetHashCode(PairSymbInt pair)
            {
                string s = pair.First.ToString() + pair.Second.ToString();
                return s.GetHashCode();
            }
        }

        /// LR(0)-ситуация - продукция с точкой в некоторой позиции правой части
        protected class State : IEquatable<State>
        {
            /// Номер правила грамматика
            public int rulePos { get; }
            /// Позиция точки в правой части правила
            public int dotPos { get; }

            public State() {}

            public State(int rulePosition, int dotPosition)
            {
                rulePos = rulePosition;
                dotPos = dotPosition;
            }

            public bool Equals(State st)
            {
                return (st.rulePos == rulePos) && (st.dotPos == dotPos);
            }

            /// Получение символа, стоящего после точки
            public Symbol GetRHSymbol(List<Production> P)
            {
                Production rule = P[rulePos];
                if (dotPos == rule.RHS.Count)
                {
                    return Symbol.Epsilon;
                }
                else
                {
                    return rule.RHS[dotPos];
                }
            }

            /// Получение символа, стоящего в левой части ситуации
            public Symbol GetLHSymbol(List<Production> P)
            {
                Production rule = P[rulePos];
                return rule.LHS;
            }

            /// Отладочная печать ситуации
            public void Debug(List<Production> P)
            {
                Production rule = P[rulePos];
                string curStateRHS = "";
                for (int i = 0; i < rule.RHS.Count; ++i)
                {
                    if (i == dotPos)
                    {
                        curStateRHS += ".";
                    }
                    curStateRHS += rule.RHS[i].ToString();
                }
                if (rule.RHS.Count == dotPos)
                {
                    curStateRHS += ".";
                }
                Console.Write(rule.LHS.ToString() + " -> " + curStateRHS);
            }
        }

        /// Канонический набор множеств LR(0)-ситуаций
        protected List< List< State > > phi = new List< List< State > >();
        /// LR(0)-автомат переходов грамматики
        protected FSAutomate LRA = new FSAutomate();
        /// Управляющая SLR-таблица, представленная в виде словаря
        protected Dictionary<PairSymbInt, PairSymbInt> M = null;
        /// Новый начальный нетерминал S'
        protected Symbol startSymbol = new Symbol("S'");

        public SLRGrammar() : base() { Production.Count = 0; }

        public SLRGrammar(List<Symbol> T, List<Symbol> V, List<Production> production, string S0) : base(T, V, S0)
        {
            Production.Count = 0;
            this.P = production;
        }

        public void Construct()
        {
            // Пополнение грамматики
            Production newStartProduction = new Production(startSymbol, new List<Symbol>() { S0 });
            P.Insert(0, newStartProduction);
            V.Add(startSymbol);
            T.Add(Symbol.Sentinel);
            DebugPrules();

            InitAutomate();
            BuildLRAutomate();
            BuildControlTable();
        }

        /// Инициализация автомата и добавление символов в алфавит
        protected void InitAutomate()
        {
            LRA.Q = new List<Symbol>() { new Symbol("0") };
            LRA.Sigma = new List<Symbol>();
            foreach (Symbol t in T)
            {
                LRA.Sigma.Add(t);
            }
            foreach (Symbol v in V)
            {
                LRA.Sigma.Add(v);
            }
            LRA.Delta = new List<DeltaQSigma>();
            LRA.Q0 = "0";
            LRA.F = new List<Symbol>();
        }

        /// Построение замыкания множества LR(0)-ситуаций
        protected List<State> Closure(List<State> I)
        {
            List<State> J0 = new List<State>(I);
            List<State> J = null;
            bool changed = false;
            do
            {
                changed = false;
                J = new List<State>(J0);
                foreach (State curState in J)
                {
                    Symbol B = curState.GetRHSymbol(P);
                    for (int i = 0; i < P.Count; ++i) {
                        Production rule = P[i];
                        if (B.Equals(rule.LHS))
                        {
                            State gammaState = new State(i, 0);
                            if (!J0.Contains(gammaState))
                            {
                                J0.Add(gammaState);
                                changed = true;
                            }
                        }
                    }
                }

            } while (changed);
            return J;
        }

        /// Вычисление множества GOTO(I, X)
        protected List<State> Goto(List<State> I, Symbol X)
        {
            List<State> J = null;
            foreach (State st in I)
            {
                if (X.Equals(st.GetRHSymbol(P)))
                {
                    List<State> movedDotState = new List<State>();
                    movedDotState.Add(new State(st.rulePos, st.dotPos + 1));
                    List<State> movedDotClosure = Closure(movedDotState);
                    if (J == null)
                    {
                        J = new List<State>();
                    }
                    foreach (State movedDotSt in movedDotClosure)
                    {
                        if (!J.Contains(movedDotSt))
                        {
                            J.Add(movedDotSt);
                        }
                    }
                }
            }
            return J;
        }

        /// Возвращает истину, если уже есть правило перехода в автомате
        protected bool FindDeltaRuleInLRA(DeltaQSigma rule)
        {
            if (rule == null)
            {
                return false;
            }
            foreach (DeltaQSigma d in LRA.Delta)
            {
                if (rule.LHSQ == d.LHSQ && rule.LHSS == d.LHSS)
                {
                    return true;
                }
            }
            return false;
        }

        /**
         *  Возвращает индекс замыкания множества LR(0)-ситуаций в указанном
         *  каноническом наборе. Если множество не было найдено, возращает -1
         */
        protected int FindSetOfStates(List< List< State > > C, List<State> I)
        {
            if (I == null)
            {
                return -1;
            }
            for (int i = 0; i < C.Count; ++i)
            {
                List<State> J = C[i];
                if (J.Count == I.Count)
                {
                    bool equals = true;
                    foreach (State st in J)
                    {
                        if (!I.Contains(st))
                        {
                            equals = false;
                            break;
                        }
                    }
                    if (equals)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// Построение канонического набора множеств LR(0)-ситуаций и автомата перехода между ситуациями
        protected void BuildLRAutomate()
        {
            List< List< State > > phi0 = new List< List< State > >();
            List<State> I0 = new List<State>();
            I0.Add(new State(0, 0));
            phi0.Add(Closure(I0));
            bool changed = false;
            do {
                changed = false;
                phi = new List< List< State > > (phi0);
                for (int i = 0; i < phi.Count; ++i)
                {
                    List<State> I = phi[i];
                    Symbol curStateSymbol = new Symbol(i.ToString());
                    foreach (Symbol X in LRA.Sigma)
                    {
                        List<State> nextState = Goto(I, X);
                        if (nextState != null)
                        {
                            int nextStateId = FindSetOfStates(phi0, nextState);
                            // Если замыкание не найдено
                            if (nextStateId == -1)
                            {
                                nextStateId = phi0.Count;
                                LRA.Q.Add(new Symbol(nextStateId.ToString()));
                                phi0.Add(nextState);
                                changed = true;
                            }
                            DeltaQSigma nextStateEdge = new DeltaQSigma(curStateSymbol, X, new List<Symbol> { new Symbol(nextStateId.ToString()) });
                            if (!FindDeltaRuleInLRA(nextStateEdge))
                            {
                                LRA.Delta.Add(nextStateEdge);
                            }
                        }
                    }
                    if (!LRA.F.Contains(curStateSymbol))
                    {
                        foreach (State stateI in I)
                        {
                            Symbol stateISymbol = stateI.GetRHSymbol(P);
                            if (stateISymbol.Equals(Symbol.Epsilon))
                            {
                                LRA.F.Add(curStateSymbol);
                                break;
                            }
                        }
                    }
                }
            } while (changed);

            Console.WriteLine("Сanonical set of states phi");
            for (int i = 0; i < phi.Count; ++i)
            {
                List<State> I = phi[i];
                Console.Write("I_" + i.ToString() + " = CLOSURE( ");
                I[0].Debug(P);
                Console.WriteLine(" )");
                foreach (State st in I) {
                    st.Debug(P);
                    Console.WriteLine();
                }
            }

            Console.WriteLine("LR-automate");
            LRA.DebugAuto();
        }

        /// Отладочная печать управляющей таблицы M
        protected void DebugControlTable()
        {
            // Это было сделано на скорую руку...
            Console.WriteLine("Contol table M");
            Console.Write("    | ");
            foreach (Symbol X in LRA.Sigma)
            {
                for (int i = 2; i > X.symbol.Count(); --i)
                {
                    Console.Write(" ");
                }
                Console.Write(X.symbol + "  | ");
            }
            Console.WriteLine();
            for (int i = 0; i < phi.Count; ++i)
            {
                for (int j = 0; j < (1 + LRA.Sigma.Count()) * 6 - 1; ++j)
                {
                    Console.Write("-");
                }
                Console.WriteLine();
                if (i > 9)
                {
                    Console.Write(i.ToString());
                }
                else
                {
                    Console.Write(" " + i.ToString());
                }
                Console.Write("  | ");
                foreach (Symbol X in LRA.Sigma)
                {
                    PairSymbInt conditionFrom = new PairSymbInt(X, i);
                    PairSymbInt conditionTo = null;
                    if (!M.TryGetValue(conditionFrom, out conditionTo))
                    {
                        Console.Write("    | ");
                    }
                    else
                    {
                        if (conditionTo.First.symbol == "A")
                        {
                            Console.Write(" A  | ");
                        }
                        else
                        {
                            if (conditionTo.First.symbol == "G")
                            {
                                Console.Write(" ");
                            }
                            else
                            {
                                Console.Write(conditionTo.First.ToString());
                            }
                            if (conditionTo.Second > 9)
                            {
                                Console.Write(conditionTo.Second.ToString() + " | ");
                            }
                            else
                            {
                                Console.Write(" " + conditionTo.Second.ToString() + " | ");
                            }
                        }
                    }
                }
                Console.WriteLine();
            }
        }

        /// Построение управляющей таблицы КС-грамматики
        protected void BuildControlTable()
        {
            PairComparer comp = new PairComparer();
            M = new Dictionary<PairSymbInt, PairSymbInt>(comp);
            for (int i = 0; i < phi.Count; ++i)
            {
                List<State> I = phi[i];
                foreach (DeltaQSigma edge in LRA.Delta)
                {
                    if (i.ToString() == edge.LHSQ.symbol)
                    {
                        Symbol X = edge.LHSS;
                        Symbol I_j = edge.RHSQ[0];
                        int j = int.Parse(I_j.symbol);
                        if (T.Contains(X))
                        {
                            PairSymbInt conditionFrom = new PairSymbInt(X.ToString(), i);
                            PairSymbInt conditionTo = new PairSymbInt("S", j);
                            M[conditionFrom] = conditionTo;
                        }
                        if (V.Contains(X))
                        {
                            PairSymbInt conditionFrom = new PairSymbInt(X.ToString(), i);
                            PairSymbInt conditionTo = new PairSymbInt("G", j);
                            M[conditionFrom] = conditionTo;
                        }
                        if (LRA.F.Contains(I_j))
                        {
                            foreach (State st in phi[j])
                            {
                                Symbol a = st.GetRHSymbol(P);
                                Symbol A = st.GetLHSymbol(P);
                                if (a.Equals(Symbol.Epsilon))
                                {
                                    if (A.Equals(startSymbol))
                                    {
                                        PairSymbInt conditionFrom = new PairSymbInt("$", j);
                                        PairSymbInt conditionTo = new PairSymbInt("A", -1);
                                        M[conditionFrom] = conditionTo;
                                    }
                                    else
                                    {
                                        // Для просмотра следующего символа требуется FOLLOW(A)
                                        // foreach (Symbol Y in FOLLOW(A))
                                        foreach (Symbol Y in T)
                                        {
                                            PairSymbInt conditionFrom = new PairSymbInt(Y.ToString(), j);
                                            PairSymbInt conditionTo = new PairSymbInt("R", st.rulePos);
                                            if (!M.ContainsKey(conditionFrom))
                                            {
                                                M[conditionFrom] = conditionTo;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            DebugControlTable();
        }

        /// Выполнение LR-анализа
        /**
         *  LR-анализатор состоит из выходного буфера, выхода, стека,
         *  программы-драйвера и таблицы синтаксического анализа,
         *  состоящей из двух частей (ACTION и GOTO). Программа-драйвера
         *  одинакова для всех LR-анализаторов; от одного к другому
         *  меняются таблицы синтаксического анализа. Программа
         *  синтаксического анализа по одному считывает символы из
         *  входного буфера.
         */
        public override void Inference()
        {
            string answer = "y";
            do
            {
                Console.WriteLine("\n Введите строку: \n");
                string input = Console.In.ReadLine();
                Console.WriteLine("\n Введена строка: " + input + "\n");

                string w = input + "$";
                Stack<int> st = new Stack<int>();
                st.Push(0);
                int i = 0;
                Stack<int> res = new Stack<int>();
                bool accepted = false;
                bool error = false;
                do
                {
                    char a = w[i];
                    PairSymbInt curCondition = new PairSymbInt(a, st.Peek());
                    PairSymbInt tableCondition = null;
                    if (!M.TryGetValue(curCondition, out tableCondition))
                    {
                        error = true;
                        break;
                    }
                    // Switch-case для LR-анализа: обработка действий из управляющей таблицы M.
                    // Обрабатывает три типа действий: Accept (принятие), Shift (перенос), Reduction (свёртка).
                    switch (tableCondition.First.symbol)
                    {
                        // Accept (A) - Принятие строки.
                        // Действие выполняется, когда анализатор успешно распознал входную строку.
                        case "A":
                            accepted = true;
                            break;
                        // Shift (S) - Перенос символа из входной строки в стек.
                        // Переход в новое состояние, указанное в tableCondition.Second.
                        case "S":
                            st.Push(tableCondition.Second);
                            ++i;
                            break;
                        // Reduction (R) - Свёртка по правилу грамматики.
                        // Удаляет символы правой части правила из стека и добавляет левую часть.
                        case "R":
                            int rulePos = tableCondition.Second;
                            Production rule = P[rulePos];
                            for (int j = 0; j < rule.RHS.Count; ++j)
                            {
                                // Если эпсилон-правило, то снимать со стека ничего не нужно!
                                if (!rule.RHS[j].Equals(Symbol.Epsilon))
                                {
                                    st.Pop();
                                }
                            }
                            curCondition = new PairSymbInt(rule.LHS.ToString(), st.Peek());
                            if (M.TryGetValue(curCondition, out tableCondition))
                            {
                                st.Push(tableCondition.Second);
                                res.Push(rulePos);
                            }
                            else
                            {
                                error = true;
                            }
                            break;
                        // default - Ошибка: неизвестное действие в управляющей таблице
                        default:
                            error = true;
                            break;
                    }
                }
                while (!accepted && !error);

                if (accepted)
                {
                    Console.WriteLine("Строка допущена");
                    Console.Write("Вывод:");
                    while (res.Count > 0)
                    {
                        Console.Write(" " + res.Pop().ToString());
                    }
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Строка отвергнута");
                }

                Console.WriteLine("\n Продолжить? (y or n) \n");
                answer = Console.ReadLine();
            }
            while (answer == "y");
        }
    }
}
