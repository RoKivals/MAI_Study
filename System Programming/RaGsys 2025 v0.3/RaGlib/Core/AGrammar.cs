using System;
using System.Collections.Generic;
using System.Collections;
using RaGlib;

namespace RaGlib.Core {
    public abstract class AGrammar
    {
        public Symbol S0 = null; ///< Начальный символ
        public List<Symbol> T = null; ///< Множество терминалов
        public List<Symbol> V = null; ///< Множество нетерминалов
        public List<Production> P = null; ///< Множество правил продукций (порождений)

        public AGrammar() {}

        public AGrammar(List<Symbol> T, List<Symbol> V, string S0)
        {
            this.T = T;
            this.V = V;
            this.S0 = new Symbol(S0);
            this.P = new List<Production>();
        }

        abstract public void Inference(); // вывод =>

        public void AddRule(string LeftNoTerm, List<Symbol> RHS) {
           this.P.Add(new Production(LeftNoTerm, RHS)); 
        }

        
        /// Определение множествa производящих нетерминальных символов
        protected List<Symbol> producingSymb()
        {
            var Vp = new List<Symbol>();
            foreach (var p in this.P)
            {
                bool flag = true;
                foreach (var t in this.T)
                    if (p.RHS.Contains(t))
                        flag = false;
                if (!flag && !Vp.Contains(p.LHS))
                    Vp.Add(p.LHS);
            }
            return Vp;
        }

    /// Определение множества достижимых символов за 1 шаг
        protected List<Symbol> ReachableByOneStep(string state)
        {
            var Reachable = new List<Symbol>() { new Symbol(state) };
            var tmp = new List<Symbol>();
            int flag = 0;
            foreach (var p in this.P)
            {
                if (p.LHS.ToString() == state)
                    for (int i = 0; i < p.RHS.Count; i++)
                        for (int j = 0; j < Reachable.Count; j++)
                            if (p.RHS[i].ToString() != Reachable[j].ToString())
                            {
                                tmp.Add(p.RHS[i]); // Debug(tmp);Console.WriteLine("");
                                break;
                            }
            }
            foreach (var s in tmp)
            {
                flag = 0;
                for (int i = 0; i < Reachable.Count; i++)
                    if (Reachable[i].symbol == s.symbol)
                        flag = 1;
                if (flag == 0)
                    Reachable.Add(s);
            }
            return Reachable;
        }

        /// Определение множества достижимых символов
        private List<Symbol> Reachable(string StartState)
        {
            var Vr = new List<Symbol>() { this.S0 };
            var nextStates = ReachableByOneStep(StartState);
            Debug("NEXT", nextStates);
            var NoTermByStep = NoTermReturn(nextStates);
            Debug("NoTermByStep", NoTermByStep);
            Vr = Unify(Vr, NoTermByStep);
            foreach (var NoTerm in NoTermByStep)
            {
                Vr = Unify(Vr, ReachableByOneStep(NoTerm.symbol));
            }
            return Vr;
        }

        /// Удаление бесполезных символов
        public Grammar unUsefulDelete()
        {
            Console.WriteLine("\t\tDeleting unuseful symbols");
            Console.WriteLine("Executing: ");
            var Vp = new List<Symbol>();
            var Vr = new List<Symbol>();
            Vr.Add(this.S0);
            var Pp = new List<Production>();
            var P1 = new List<Production>(this.P);
            bool flag = false, noadd = false;
            // Создааем множество порождающих символов и одновременно непроизводящие правила
            do
            {
                flag = false;
                foreach (var p in P1)
                {
                    noadd = false;
                    // DebugPrule(p);
                    if (p.RHS == null || p.RHS.Contains(new Symbol("")))
                    {
                        Pp.Add(p);
                        if (!Vp.Contains(p.LHS))
                        {
                            Vp.Add(p.LHS);
                        }
                        P1.Remove(p);
                        flag = true;
                        break;
                    }
                    else
                    {
                        foreach (var t in p.RHS)
                        {
                            if (!this.T.Contains(t) && !Vp.Contains(t))
                            {
                                // Console.WriteLine(t);
                                noadd = true;
                                break;
                            }
                        }
                        if (!noadd)
                        {
                            Pp.Add(p);
                            if (!Vp.Contains(p.LHS))
                            {
                                Vp.Add(p.LHS);
                            }
                            P1.Remove(p);
                            flag = true;
                            break;
                        }
                    }
                }
            } while (flag);

            Debug("Vp", Vp);
            P1.Clear();
            if (!Vp.Contains(this.S0))
            {
                return new Grammar(new List<Symbol>(), new List<Symbol>(), this.S0.symbol);
            }
            var T1 = new List<Symbol>();
            //Создаем множество достижимых символов
            do
            {
                flag = false;
                foreach (var p in Pp)
                {
                    if (Vr.Contains(p.LHS))
                    {
                        foreach (var t in p.RHS)
                        {
                            if (!Vr.Contains(t))
                            {
                                Vr.Add(t);
                                // noadd = true;
                            }
                        }
                        P1.Add(p);
                        Pp.Remove(p);
                        flag = true;
                        break;
                    }
                }
            } while (flag);

            Debug("Vr", Vr);
            Vp.Clear();
            // Обновляем множества терминалов и нетерминалов
            foreach (var t in Vr)
            {
                if (this.T.Contains(t))
                {
                    T1.Add(t);
                }
                else if (this.V.Contains(t))
                {
                    Vp.Add(t);
                }
            }
            Debug("T1", T1);
            Debug("V1", Vp);
            Console.WriteLine("\tUnuseful symbols have been deleted");
            return new Grammar(T1, Vp, P1, this.S0.symbol);
        }

        private List<Symbol> ShortNoTerm()
        {
            var Ve = new List<Symbol>();
            foreach (var p in this.P)
            {
                if (p.RHS.Contains(new Symbol("")))
                    Ve.Add(p.LHS);
            }
            int i = 0; ///!!!
            if (Ve.Count != 0)
                // Console.WriteLine("  {0}",Ve.Count);
                while ((FromWhat(Ve[i].ToString()) != null) && (Ve.Count < i))
                {
                    Ve = Unify(Ve, FromWhat(Ve[0].symbol));
                    i++;
                }
            Debug("Ve", Ve);

            return Ve;
        }

        /// Удаление эпсилон правил
        public Grammar EpsDelete()
        {
            Console.WriteLine("\tDelete e-rules:");
            Console.WriteLine("Executing:");
            var Erule = new List<Production>();
            var Ps = new List<Production>(this.P);
            // ArrayList NoTerm = new ArrayList();
            Console.WriteLine("e-rules:");
            //находим множество е-правил
            foreach (var p in this.P)
            {
                if (p.RHS.Contains(new Symbol("")))
                {
                    DebugPrule(p);
                    Erule.Add(p);
                    Ps.Remove(p);
                }
            }
            //определяем множество неукорачивающихся символов
            var NoTerms = new List<Symbol>();

            foreach (var p in Erule)
            {
                if (!NoTerms.Contains(p.LHS))
                {
                    NoTerms.Add(p.LHS);
                }
            }
            bool flag = false, noadd = false;
            do
            {
                flag = false;
                foreach (var p in Ps)
                {
                    noadd = false;
                    // DebugPrule(p);
                    foreach (var t in p.RHS)
                    {
                        if (!NoTerms.Contains(t))
                        {
                            noadd = true;
                            break;
                        }
                    }
                    if (!noadd)
                    {
                        if (!NoTerms.Contains(p.LHS))
                        {
                            NoTerms.Add(p.LHS);
                        }
                        flag = true;
                        Ps.Remove(p);
                        break;
                    }
                }
            } while (flag);
            Debug("NoShortNoTerms", NoTerms);
            Ps.Clear();
            //Удаляем е-правила и создаем новые в соответствии с алгоритмом
            foreach (var p in this.P)
            {
                if (Erule.Contains(p))
                    continue;
                Ps.Add(p);
                foreach (var s in p.RHS)
                {
                    if (NoTerms.Contains(s))
                    {
                        var NR = new List<Symbol>(p.RHS);
                        NR.Remove(s);
                        Ps.Add(new Production(p.LHS, NR));
                    }
                }
            }
            //проверяем есть ли порождение е из нач символа
            if (NoTerms.Contains(this.S0))
            {
                var V1 = new List<Symbol>(this.V);
                V1.Add(new Symbol("S1"));
                Ps.Add(new Production(new Symbol("S1"), new List<Symbol>() { this.S0 }));
                Ps.Add(new Production(new Symbol("S1"), new List<Symbol>() { new Symbol("") }));
                Debug("V1", V1);
                Console.WriteLine("\te-rules have been deleted!");
                return new Grammar(this.T, V1, Ps, "S1");
            }
            else
            {
                Debug("V1:", this.V);
                Console.WriteLine("\te-rules have benn deleted!");
                return new Grammar(this.T, this.V, Ps, this.S0.symbol);
            }
        }

        

        /// Удаление левой рекурсии Работает корректно
        ///
        public Grammar LeftRecursDelete_new6()
        {
            List<Production> Ph = new List<Production>(this.P);//изначальные правила
            List<Production> P = new List<Production>(); // выходные правила 
            List<Symbol> V1 = new List<Symbol>(this.V); //вывод - нетерминалы
            List<Production> Pdel = new List<Production>(); //хранение удаляемых правил 
            int i = 1; int j = 1;
            Console.WriteLine("Left recursion:");
            foreach (Symbol vi in this.V)
            {

                j = 1;
                List<Production> Ph_h = new List<Production>();//добавление правил из циклов (вспомогательное множество)
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
                    foreach (Production ph_h in Ph_h)
                    {
                        if (!Ph.Contains(ph_h)) { Ph.Add(ph_h); }
                    }
                }


                List<Symbol> Vr = new List<Symbol>(); //хранение нетерминала с левой рекурсией
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
                foreach (Symbol vr in Vr)
                {
                    foreach (Production r in Ph)
                    {
                        if (r.LHS == vi && vr == r.LHS && !Pdel.Contains(r))
                        {
                            Symbol new_v = vi + "'"; //создание нового элемента
                            if (!V1.Contains(new_v)) { V1.Add(new_v); } //добавление во множество нетерминалов

                            if (r.LHS == r.RHS[0].ToString())
                            {
                                DebugPrule(r);
                                List<Symbol> a_h = new List<Symbol>();// для создания правой части в правилах вида A'->alpha из  A->Aalpha
                                List<Symbol> a_a = new List<Symbol>();//для создания правой части в правилах вида A'->alpha A'  из  A->Aalpha
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
                                List<Symbol> b_h = new List<Symbol>();//для создания правой части в правилах вида A-> beta
                                List<Symbol> b_b = new List<Symbol>();//для создания правой части в правилах вида A-> betaA'
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
        }

     

        // **   Debug   **
        public void DebugPrules()
        {
            Console.WriteLine("Prules:");
            foreach (var p in this.P)
            {
                string right = "";
                for (int i = 0; i < p.RHS.Count; i++)
                    right += p.RHS[i].ToString();
                Console.WriteLine(p.LHS + " -> " + right);
            }
        }
        public void DebugPrule(Production p)
        {
            var right = "";
            for (int i = 0; i < p.RHS.Count; i++)
                right += p.RHS[i].ToString();
            Console.WriteLine(p.LHS + " -> " + right + " ");
        }

        public void Debug(string step, List<Symbol> list)
        {
            Console.Write(step + " : ");
            if (list == null)
                Console.WriteLine("null");
            else
                foreach (var s in list)
                    Console.Write(s.symbol + " ");
            Console.WriteLine("");
        }

        public void Debug(string step, string line)
        {
            Console.Write(step + " : ");
            Console.WriteLine(line);
        }

        /// Откуда можем прийти в состояние
        private List<Symbol> FromWhat(string state)
        {
            var from = new List<Symbol>();
            bool flag = true;
            foreach (var p in this.P)
            {
                if (p.RHS.Contains(new Symbol(state)))
                {
                    from.Add(p.LHS);
                    flag = false;
                }
            }
            if (flag)
                return null;
            else
                return from;
        }

        // Объединение множеств A or B
        private List<Symbol> Unify(List<Symbol> A, List<Symbol> B)
        {
            var unify = A;
            foreach (var s in B)
                if (!A.Contains(s))
                    unify.Add(s);
            return unify;
        }

        // Пересечение множеств A & B
        private List<Symbol> intersection(List<Symbol> A, List<Symbol> B)
        {
            var intersection = new List<Symbol>();
            foreach (var s in A)
                if (B.Contains(s))
                    intersection.Add(s);
            return intersection;
        }

        // Нетерминальные символы из массива
        protected List<Symbol> NoTermReturn(List<Symbol> array)
        {
            var NoTerm = new List<Symbol>();
            bool flag = true; // added
            foreach (var s in array)
                if (this.V.Contains(s))
                {
                    flag = false; // added
                    NoTerm.Add(s);
                }
            if (flag)
                return null; // added
            else
                return NoTerm;
        }

        protected string NoTerminal(List<Symbol> array)
        {
            var NoTermin = "";
            foreach (var s in array)
            {
                if (this.V.Contains(s))
                    NoTermin = s.symbol;
            }
            return NoTermin;
        }

        // Терминальные символы из массива
        private List<Symbol> TermReturn(List<Symbol> A)
        {
            var Term = new List<Symbol>();
            bool flag = true;
            foreach (var t in this.T)
                if (A.Contains(t))
                {
                    flag = false;
                    Term.Add(t);
                }
            if (flag)
                return null;
            else
                return Term;
        }

        // Все символы в правиле
        private List<Symbol> SymbInRules(Production p)
        {
            var SymbInRules = new List<Symbol>() { p.LHS };
            for (int i = 0; i < p.RHS.Count; i++)
                SymbInRules.Add(p.RHS[i]);
            return SymbInRules;
        }

        // Проверка пустоты правой цепочки
        private bool ContainEps(Production p)
        {
            if (p.RHS.ToString().Contains(""))
                return true;
            return false;
        }





        /// Алгоритм  Хомского
        ///
        // принадлежность к множеству
        private bool CheckExist(string b, List<Symbol> PrulesList)
        {
            foreach (var p in PrulesList)
                if (b == p) return true;
            return false;
        }


        // Удаление длинных правил
        public Grammar DeleteLongRules()
        {
            /*
             Создаем новое множество нетерминалов, включив в него все старые нетерминалы, множество правил (пустое) и массив индексов,
             в котором i-й элемент соответствует символу из множества нетерминалов, а значение - количество использованных индексов
             Например, ind[0] = 1 означает, что 1-й нетерминал (например, A) из V в новом множестве использован как A и A1
             */
            var Vr = new List<Symbol>(this.V);
            int[] ind = new int[this.V.Count];
            var Pr = new List<Production>();

            foreach (Production p in this.P)
            {
                if (p.RHS.Count > 2) // длина правила больше 2
                {
                    int size_ = p.RHS.Count;
                    int index = this.V.IndexOf(p.LHS); // индекс для ind[]
                    var Vr1 = new List<Symbol>();
                    // создаем k-2 новых нетерминала
                    ind[index]++;
                    for (int i = 1; i <= size_ - 2; i++)
                    {
                        /* Выполняем проверку на принадлежность нового нетерминала множеству нетерминалов: пока нетерминал с текущим индексом есть во 
                           множестве нетерминалов, увеличиваем индекс на 1 - затем добавляем новый нетерминал во множество нетерминалов*/
                        while (CheckExist(p.LHS + Convert.ToString(ind[index]), this.V)) ind[index]++;
                        Vr1.Add(p.LHS + Convert.ToString(ind[index]));
                    }
                    Vr.AddRange(Vr1);
                    // Добавляем новое правило вида A -> a1A1
                    var new_p = new Production(p.LHS, new List<Symbol>() { p.RHS[0], Vr1[0] });
                    Pr.Add(new_p);
                    int size_2 = Vr1.Count;
                    // Добавляем правила вида Ai -> ai+1Ai+1
                    for (int i = 1; i < size_2 - 1; i++)
                    {
                        var new_ = new Production(Vr1[i], new List<Symbol>() { p.RHS[i], Vr1[i + 1] });
                        Pr.Add(new_);
                    }
                    // Добавляем новое правило вида Ak -> a(k-1)a(k)
                    var new__ = new Production(Vr1[size_2 - 1], new List<Symbol>() { p.RHS[size_ - 2], p.RHS[size_ - 1] });
                    Pr.Add(new__);
                }
                else
                    Pr.Add(p);
            }
            Console.WriteLine("\tLong rules have been deleted");
            return new Grammar(this.T, Vr, Pr, this.S0.symbol);

        }


        // удаление начального символа из правой части правил
        public Grammar DeleteS0Rules()
        {
            // Создаем новые множества нетерминалов и правил, равные старым
            var Vr = new List<Symbol>(this.V);
            var Pr = new List<Production>(this.P);
            string S = null;

            int flag = 1;
            foreach (Production p in this.P)
            {
                foreach (var pr in p.RHS)
                {

                    if (pr == this.S0) // если встречаем начальный символ в правой части правила
                    {
                        if (flag == 1)
                        {
                            Console.WriteLine("\tRight-S0 rules have been deleted");
                            flag = 0;
                            int i = 0;
                            /* Выполняем проверку на принадлежность нового начального нетерминала множеству нетерминалов: пока нетерминал 
                               с текущим индексом есть во множестве нетерминалов, увеличиваем индекс на 1 - затем добавляем новый нетерминал 
                               во множество нетерминалов и создаем новое правило S' -> S0*/
                            while (CheckExist(this.S0 + Convert.ToString(i), this.V)) i++;
                            S = this.S0 + Convert.ToString(i);
                            var new_p = new Production(S, new List<Symbol>() { this.S0 });
                            Vr.Insert(0, S);
                            Pr.Insert(0, new_p);
                            Console.WriteLine("New start symbol: " + S);
                            break;
                        }
                    }
                }
                if (flag == 0) break;
            }

            if (flag == 0)
                return new Grammar(this.T, Vr, Pr, S);
            return new Grammar(this.T, Vr, Pr, this.S0.symbol);
        }



        // проверка правила на существование правила вида A -> b
        private string CheckPruleExist(string b, List<Production> PrulesList)
        {
            foreach (var p in PrulesList)
                if (p.RHS.Count == 1 && p.RHS[0] == b)
                    return p.LHS.symbol;
            return null;
        }
       


        // удаление терминальных символов из правил вида A->a1a2
        public Grammar DeleteTermRules()
        {
            // Создаем новые множества нетерминалов (равно старому) и правил (пустое)
            var Vr = new List<Symbol>(this.V);
            var Pr = new List<Production>();
            int i = 1;

            foreach (Production p in this.P)
            {
                if (p.RHS.Count == 2) // если длина правила равна 2
                {
                    // если в правой части оба символа - терминалы
                    if (CheckExist(p.RHS[0].symbol, this.T) && CheckExist(p.RHS[1].symbol, this.T))
                    {
                        /* если в изначальном множестве правил и в новом нет правил вида U1 -> a1 и U2 -> a2, то 
                           добавляем в новое правила A->U1U2, U1->a1, U2->a2 */
                        if (CheckPruleExist(p.RHS[0].symbol, this.P) == null && CheckPruleExist(p.RHS[0].symbol, Pr) == null && CheckPruleExist(p.RHS[1].symbol, this.P) == null && CheckPruleExist(p.RHS[1].symbol, Pr) == null)
                        {
                            /* Выполняем проверку на принадлежность нового нетерминала множеству нетерминалов: пока нетерминал с текущим индексом есть во 
                           множестве нетерминалов, увеличиваем индекс на 1 - затем добавляем новый нетерминал во множество нетерминалов*/
                            while (CheckExist("U" + Convert.ToString(i), Vr)) i++;
                            string U1 = "U" + Convert.ToString(i);
                            Vr.Add(U1);
                            var new_p1 = new Production(U1, new List<Symbol>() { p.RHS[0] }); // Создание правила U1 -> a1
                            while (CheckExist("U" + Convert.ToString(i), Vr)) i++; // поиск подходящего нетерминала
                            string U2 = "U" + Convert.ToString(i);
                            Vr.Add(U2);
                            var new_p2 = new Production(U2, new List<Symbol>() { p.RHS[1] }); // Создание правила U2 -> a2
                            var new_p = new Production(p.LHS, new List<Symbol>() { U1, U2 }); // Создание правила A -> U1U2
                            Pr.Add(new_p);
                            Pr.Add(new_p1);
                            Pr.Add(new_p2);
                        }
                        /* если в изначальном множестве правил и в новом нет правил вида U1 ->a1, но есть X -> a2, то 
                          добавляем в новое правила A->U1X, U1->a1: выполяется с целью избежать дублирования правил (здесь - X -> a2 и U2 -> a2)*/
                        else if (CheckPruleExist(p.RHS[0].symbol, this.P) == null && CheckPruleExist(p.RHS[0].symbol, Pr) == null)
                        {
                            while (CheckExist("U" + Convert.ToString(i), Vr)) i++; // поиск подходящего нетерминала
                            string U1 = "U" + Convert.ToString(i);
                            Vr.Add(U1);
                            var new_p1 = new Production(U1, new List<Symbol>() { p.RHS[0] }); // добавляем правило U1 -> a1
                            /* выполняем поиск нетерминала X из правила X -> a2 в исходном множестве правил, если такого правила нет в исходном, то выполняем
                               поиск в новом множестве*/
                            string U2 = CheckPruleExist(p.RHS[1].symbol, this.P);
                            if (U2 == null)
                                U2 = CheckPruleExist(p.RHS[1].symbol, Pr);
                            var new_p = new Production(p.LHS, new List<Symbol>() { U1, U2 }); // добавляем правило A -> U1X
                            Pr.Add(new_p); Pr.Add(new_p1);
                        }
                        /* если в изначальном множестве правил и в новом нет правил вида U2 ->a2, но есть X -> a1, то 
                          добавляем в новое правила A->XU2, U2->a2: выполяется с целью избежать дублирования правил (здесь - X -> a1 и U1 -> a1) */
                        else if (CheckPruleExist(p.RHS[1].symbol, this.P) == null && CheckPruleExist(p.RHS[1].symbol, Pr) == null)
                        {
                            while (CheckExist("U" + Convert.ToString(i), Vr)) i++; // поиск подходящего нетерминала
                            string U2 = "U" + Convert.ToString(i);
                            Vr.Add(U2);
                            var new_p1 = new Production(U2, new List<Symbol>() { p.RHS[1] }); // Создание правила U2 -> a2
                            /* выполняем поиск нетерминала X из правила X -> a1 в исходном множестве правил, если такого правила нет в исходном, то выполняем
                               поиск в новом множестве*/
                            string U1 = CheckPruleExist(p.RHS[0].symbol, this.P);
                            if (U1 == null)
                                U1 = CheckPruleExist(p.RHS[0].symbol, Pr);
                            var new_p = new Production(p.LHS, new List<Symbol>() { U1, U2 }); // добавляем правило A -> XU2
                            Pr.Add(new_p); Pr.Add(new_p1);
                        }
                        /* если в изначальном множестве правил и в новом есть правила X->a1 и Y->a2, то в новое
                         добавляем A->XY: выполяется с целью избежать дублирования правил (здесь - X -> a1 и U1 -> a1, X -> a2 и U2 -> a2)*/
                        else
                        {
                            /* выполняем поиск нетерминала X из правила X -> a1 в исходном множестве правил, если такого правила нет в исходном, то выполняем
                               поиск в новом множестве*/
                            string U1 = CheckPruleExist(p.RHS[0].symbol, this.P);
                            if (U1 == null)
                                U1 = CheckPruleExist(p.RHS[0].symbol, Pr);
                            /* выполняем поиск нетерминала Y из правила Y -> a2 в исходном множестве правил, если такого правила нет в исходном, то выполняем
                               поиск в новом множестве*/
                            string U2 = CheckPruleExist(p.RHS[1].symbol, this.P);
                            if (U2 == null)
                                U2 = CheckPruleExist(p.RHS[1].symbol, Pr);
                            var new_p = new Production(p.LHS, new List<Symbol>() { U1, U2 }); // добавляем правило A -> XY
                            Pr.Add(new_p);
                        }
                    }
                    // Аналогично дальше
                    // если только первый символ - терминал
                    else if (CheckExist(p.RHS[0].symbol, this.T))
                    {
                        // если нет правила X - > a1, то добавляем правила A -> U1Y, U1 -> a1
                        if (CheckPruleExist(p.RHS[0].symbol, this.P) == null && CheckPruleExist(p.RHS[0].symbol, Pr) == null)
                        {
                            while (CheckExist("U" + Convert.ToString(i), Vr)) i++; // поиск подходящего нетерминала
                            string U1 = "U" + Convert.ToString(i);
                            Vr.Add(U1);
                            var new_p1 = new Production(U1, new List<Symbol>() { p.RHS[0] }); // добавляем правило U1 -> a1
                            var new_p = new Production(p.LHS, new List<Symbol>() { U1, p.RHS[1] }); // добавляем правило A -> U1Y
                            Pr.Add(new_p); Pr.Add(new_p1);
                        }
                        // если есть правило X - > a1, то добавляем правила A -> XY
                        else
                        {
                            /* выполняем поиск нетерминала X из правила X -> a1 в исходном множестве правил, если такого правила нет в исходном, то выполняем
                               поиск в новом множестве*/
                            string U1 = CheckPruleExist(p.RHS[0].symbol, this.P);
                            if (U1 == null)
                                U1 = CheckPruleExist(p.RHS[0].symbol, Pr);
                            var new_p = new Production(p.LHS, new List<Symbol>() { U1, p.RHS[1] });// добавляем правило A -> XY
                            Pr.Add(new_p);
                        }
                    }
                    //  если только второй символ - терминал
                    else if (CheckExist(p.RHS[1].symbol, this.T))
                    {
                        // если нет правила X - > a2, то добавляем правила A -> YU2, U2->a2
                        if (CheckPruleExist(p.RHS[1].symbol, this.P) == null && CheckPruleExist(p.RHS[1].symbol, Pr) == null)
                        {
                            while (CheckExist("U" + Convert.ToString(i), Vr)) i++; // поиск подходящего нетерминала
                            string U2 = "U" + Convert.ToString(i);
                            Vr.Add(U2);
                            var new_p1 = new Production(U2, new List<Symbol>() { p.RHS[1] }); // добавляем правило U2 -> a2
                            var new_p = new Production(p.LHS, new List<Symbol>() { p.RHS[0], U2 }); // добавляем правило A -> YU2
                            Pr.Add(new_p); Pr.Add(new_p1);
                        }
                        // если есть правило X - > a2, то добавляем правило A -> YX
                        else
                        {
                            /* выполняем поиск нетерминала Y из правила Y -> a2 в исходном множестве правил, если такого правила нет в исходном, то выполняем
                              поиск в новом множестве*/
                            string U2 = CheckPruleExist(p.RHS[1].symbol, this.P);
                            if (U2 == null)
                                U2 = CheckPruleExist(p.RHS[1].symbol, Pr);
                            var new_p = new Production(p.LHS, new List<Symbol>() { p.RHS[0], U2 }); // добавляем правило A->YX
                            Pr.Add(new_p);
                        }
                    }
                    // Если оба символа - нетерминалы, то просто добавляем правило в новое множество
                    else Pr.Add(p);
                }
                // Если длина правила = 1, то просто добавляем правило в новое множество
                else Pr.Add(p);
            }

            Console.WriteLine("\tRules length = 2 with terminals have been deleted");
            return new Grammar(this.T, Vr, Pr, this.S0.symbol);
        }


       

        //удаление цепных правил (Работает корректно)

        public Grammar ChainRuleDelete()
        {
            Console.WriteLine("\tChainRule Deleting:");
            Console.WriteLine("Executing: ");
            //  Поиск цепных пар
            var chain_pair_list = new List<List<Symbol>>();
            var chain_rules = new List<Production>();
            
            foreach (var v in this.V)
            {
                var chain_pair = new List<Symbol>();
                chain_pair.Add(v);
                chain_pair.Add(v);
                chain_pair_list.Add(chain_pair);
            }
            Console.WriteLine("ChainRules:");

            foreach (Production p in this.P)
            {
                if (TermReturn(p.RHS) == null && NoTermReturn(p.RHS) != null && NoTermReturn(p.RHS).Count == 1)
                {
                    chain_rules.Add(p);
                    DebugPrule(p);
                    for (int i = 0; i < chain_pair_list.Count; ++i)
                    {
                        var chain_pair = new List<Symbol>(chain_pair_list[i]);
                        if (chain_pair[1] == p.LHS)
                        {
                            var chain_pair1 = new List<Symbol>();
                            chain_pair1.Add(chain_pair[0]);
                            chain_pair1.Add(NoTermReturn(p.RHS)[0]);
                            chain_pair_list.Add(chain_pair1);
                        }
                    }
                }
            }

            Console.WriteLine("Deleting...");

            //  Работа основная

            var P = new List<Production>();

            foreach (List<Symbol> chain_pair in chain_pair_list)
            {
                foreach (Production p in this.P)
                {
                    if (p.LHS == chain_pair[1] && !(TermReturn(p.RHS) == null && NoTermReturn(p.RHS) != null && NoTermReturn(p.RHS).Count == 1))
                    {
                        Production P_1 = new Production(chain_pair[0], p.RHS);
                        if (!P.Contains(P_1))
                        {
                            P.Add(P_1);
                        }
                    }
                }
            }

            Console.WriteLine("\tChainrules have been deleted;");
            return new Grammar(this.T, this.V, P, this.S0.symbol);
        }



        //Алгоритм Грейбах

        // если A -> aB, то B < A
        private bool ALessB(string A, string B, List<Production> PrulesList)
        {
            foreach (var K in PrulesList)
                if (K.LHS == A)
                    foreach (var symbol in K.RHS)
                        if (symbol == B)
                            return true;
            return false;
        }
        

        /* создание правил из LeftHandSym -> FirstRightHandSym RightHand (т.е. RightHand - это правая часть правила без
           первого символа) правила вида  LeftHandSym -> B RightHand, где FirstRightHandSym -> B (т.е. B - правая часть правила) */
        private List<Production> NewRules(string LeftHandSym, string FirstRightHandSym, List<Symbol> RightHand, List<Production> PrulesList)
        {
            Console.Write("\nTransform: " + LeftHandSym + " -> " + FirstRightHandSym);
            foreach (var symbol in RightHand)
                Console.Write(symbol);
            Console.WriteLine("\n");

            var NewPrules = new List<Production>();
            foreach (var P in PrulesList)
                // если мы находим правило, у которого левая часть = FirstRightHandSym
                if (FirstRightHandSym == P.LHS)
                {
                    /* создаем новую правую цепочку right_chain, равную правой части правила, левой частью которого является FirstRightHandSym, и
                       и RightHand, создаем правило LeftHandSym -> right_chain*/
                    var right_chain = new List<Symbol>(P.RHS);
                    right_chain.AddRange(RightHand);
                    var new_prule = new Production(LeftHandSym, right_chain);
                    NewPrules.Add(new_prule);
                }

            if (NewPrules.Count != 0)
            {

                Console.WriteLine("New rules");
                foreach (Production P in NewPrules)
                {
                    Console.Write(P.LHS + " -> ");
                    foreach (var symbol in P.RHS)
                        Console.Write(symbol);
                    Console.WriteLine();
                }
                Console.WriteLine("\n");
                return NewPrules;
            }
            return null;
        }

        // преобразование грамматики в нормальную форму Грейбах
        public Grammar TransformGrForm()
        {
            var Pr = new List<Production>(this.P);

            foreach (Production P in this.P)
            {
                // если первый элемент - это нетерминал
                if (!CheckExist(P.RHS[0].symbol, this.T))
                {
                    // res_right_chain - правая цепочка без первого символа
                    var res_right_chain = new List<Symbol>(P.RHS.GetRange(1, P.RHS.Count - 1));
                    // удаляем данное правило и создаем новые правила
                    Pr.Remove(P);
                    var NewPrules = new List<Production>(NewRules(P.LHS.symbol, P.RHS[0].symbol, res_right_chain, this.P));
                    
                    // если новое правило начинается с терминала, то включаем его в итоговый список и удаляем его из NewPrules
                    foreach (Production P1 in NewPrules)
                        if (CheckExist(P1.RHS[0].symbol, this.T)) Pr.Add(P1);

                    foreach (Production P1 in Pr)
                        if (NewPrules.Contains(P1)) NewPrules.Remove(P1);
                    // пока в списке а1 есть правила, начинающиеся с нетерминала, создаем новые правила и включаем из в итоговый список,
                    // если правило начинается с нетрминала, т.е. пока в NewPrules есть какие-нибудь правила
                    while (NewPrules.Count > 0)
                    {
                        var NewPrules2 = new List<Production>(NewPrules);
                        // теперь преобразовываем каждое правило из NewPrules в нормальную форму Грейбах
                        foreach (Production P1 in NewPrules2)
                        {
                            // res_right_chain_ - правая цепочка без первого символа
                            var new_right_chain_ = new List<Symbol>(P1.RHS.GetRange(1, P1.RHS.Count - 1));
                            // удаляем данное правило и создаем новые правила
                            var a3 = NewRules(P1.LHS.symbol, P1.RHS[0].symbol, new_right_chain_, this.P);
                            NewPrules.Remove(P1);
                            NewPrules.AddRange(a3);
                        }
                        // если новое правило начинается с терминала, то включаем его в итоговый список и удаляем его из NewPrules
                        foreach (Production P1 in NewPrules)
                            if (CheckExist(P1.RHS[0].symbol, this.T)) Pr.Add(P1);

                        foreach (Production P1 in Pr)
                            if (NewPrules.Contains(P1)) NewPrules.Remove(P1);
                    }
                }
            }
            Console.WriteLine("\n\tRules have transformed in Greibach form");
            return new Grammar(this.T, this.V, Pr, this.S0.symbol);
        }

    } // end abstract Grammar class

}
