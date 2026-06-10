using System;
using System.Collections.Generic;
using System.Text;
using RaGlib.Core;
using System.Collections;


namespace RaGlib {
    /* Курсовая работа студента группы М8О-207Б-19 Ляшун Дмитрия */

    public class MyLRParser
    {
        private Grammar grammar;
        public MyLRParser()
        {
            grammar = new Grammar(new List<Symbol>(), new List<Symbol>(), new List<Production>(), "S");
        }
        public void Example()
        {
            grammar.AddRule("S", new List<Symbol> { new Symbol("S"), new Symbol("+"), new Symbol("U") });
            Console.WriteLine("S S+U");
            grammar.AddRule("S", new List<Symbol> { new Symbol("U") });
            Console.WriteLine("S U");
            grammar.AddRule("U", new List<Symbol> { new Symbol("("), new Symbol("S"), new Symbol(")") });
            Console.WriteLine("U (S)");
            grammar.AddRule("U", new List<Symbol> { new Symbol("i") });
            Console.WriteLine("U i");
            grammar.T.AddRange(new List<Symbol>{ new Symbol("+"), new Symbol("("), new Symbol(")"), new Symbol("i")});
            grammar.V.AddRange(new List<Symbol> { new Symbol("S"), new Symbol("U") });
            Console.WriteLine("Пример выводимых цепочек: (i)+(i), (i)");
        }
        
        public void Example_LR1(){
            grammar.AddRule("S", new List<Symbol>{new Symbol("S"), new Symbol("+"), new Symbol("T")});
            Console.WriteLine("S S+T");
            grammar.AddRule("S", new List<Symbol>{new Symbol("T")});
            Console.WriteLine("S T");
            grammar.AddRule("T", new List<Symbol>{new Symbol("T"), new Symbol("*"), new Symbol("P")});
            Console.WriteLine("T T*P");
            grammar.AddRule("T", new List<Symbol>{new Symbol("P")});
            Console.WriteLine("T P");
            grammar.AddRule("P", new List<Symbol>{new Symbol("("), new Symbol("S"), new Symbol(")")});
            Console.WriteLine("P (S)");
            grammar.AddRule("P", new List<Symbol>{new Symbol("i")});
            Console.WriteLine("P I");
            grammar.T.AddRange(new List<Symbol>{new Symbol("+"), new Symbol("("), new Symbol(")"), new Symbol("i"), new Symbol("*")});
            grammar.V.AddRange(new List<Symbol>{new Symbol("S"), new Symbol("T"), new Symbol("P")});
            Console.WriteLine("Пример выводимых цепочек: (i)+(i), (i)");
        }
        public void ReadGrammar()
        {
            string s;
            var term = new Hashtable(); //  временная таблица терминалов
            var nonterm = new Hashtable(); //  и нетерминалов
            Console.WriteLine("\nВведите продукции: \n ");
            while ((s = Console.In.ReadLine()) != "")
            {
                var rhs = new List<Symbol>();
                for (int i = 2; i < s.Length; ++i) rhs.Add(new Symbol(s[i].ToString()));
                grammar.P.Add(new Production(new Symbol(s[0].ToString()), rhs));
                for (int i = 0; i < s.Length; i++)
                    if (s[i] != ' ')
                    {
                        //  если текущий символ - терминал, еще не добавленный в term
                        if (s[i] == s.ToLower()[i] && !term.ContainsKey(s[i]))
                            term.Add(s[i], null);
                        if (s[i] != s.ToLower()[i] && !nonterm.ContainsKey(s[i]))
                            nonterm.Add(s[i], null);
                    }
            }
            //  переписываем терминалы и нетерминалы в строки Terminals и NonTerminals
            for (IDictionaryEnumerator c = term.GetEnumerator(); c.MoveNext();)
                grammar.T.Add(new Symbol(c.Key.ToString()));
            for (IDictionaryEnumerator c = nonterm.GetEnumerator(); c.MoveNext();)
                grammar.V.Add(new Symbol(c.Key.ToString()));
            Console.Write("Введите начальный нетерминал: ");
            grammar.S0 = new Symbol(Console.In.ReadLine());
        }
        protected void Info()
        {
            Console.Write("КС - грамматика :\nАлфавит нетерминальных символов: ");
            foreach (var v in grammar.V) Console.Write(v);
            Console.Write("\nАлфавит терминальных символов: ");
            foreach (var t in grammar.T) Console.Write(t);
            Console.Write("\nПравила:\n");
            foreach (var p in grammar.P)
            {
                Console.Write("(" + p.Id.ToString() + ") " + p.LHS.ToString() + " -> ");
                foreach (var x in p.RHS) Console.Write(x);
                Console.Write("\n");
            }
            Console.WriteLine("Начальный нетерминал: " + grammar.S0);
            Console.Write("\nДля продолжения нажмите <Enter>");
            Console.ReadLine();
        }

        private List<Symbol> OFirst(Symbol occur) // Возвращает множество OFIRST для грам. вхождения
        {
            var result = new List<Symbol>();
            result.Add(occur);
            foreach (var p in grammar.P)
            {
                var rhs = p.RHS;
                if (p.LHS == occur.symbol)
                {
                    if (occur.symbol == rhs[0])
                    {
                        result.Add(new Symbol(rhs[0].symbol, p.Id, 1));
                    }
                    else
                    {
                        var newResult = OFirst(new Symbol(rhs[0].symbol, p.Id, 1));
                        result.AddRange(newResult);
                    }
                }
            }
            return result;
        }
        private string GetVp(List<Symbol> Teta, Dictionary<string, List<Symbol> > M) // Возвращает для множества грам. вхождений их магазинный символ
        {
            string Key = "";
            foreach (var v in Teta)
            {
                Key += v.symbol + v.production.ToString() + v.symbolPosition.ToString();
            }
            if (!M.ContainsKey(Key)) M.Add(Key, Teta);
            return Key;
        }
        
        public void Execute()
        {
            Console.WriteLine("\nИсходная ");
            Info();
            grammar.EpsDelete();
            var augmentedRHS = new List<Symbol>();
            augmentedRHS.Add(grammar.S0);
            var augmentedRule = new Production(new Symbol("П"), augmentedRHS);
            augmentedRule.Id = 0;
            grammar.P.Insert(0, augmentedRule);
            grammar.V.Add(new Symbol("П"));
            grammar.T.Add(new Symbol("$"));
            var start = new Symbol(grammar.S0.symbol, 0, 1);
            Console.WriteLine("\nПополненная грамматика: ");
            Info();

            // Нахождение матрицы отношения OBLOW и множества грамматических вхождений
            var oblow = new Dictionary<Symbol, Dictionary<Symbol, int>>();
            var allGrammarOccurs = new HashSet<Symbol>();
            foreach (var p in grammar.P)
            {
                var rhs = p.RHS;
                if (p.LHS == "П")
                {
                    var symbol = new Symbol(rhs[0].ToString(), p.Id, 1);
                    var botMarker = new Symbol("^", 0, 0);
                    allGrammarOccurs.Add(botMarker);
                    allGrammarOccurs.Add(symbol);
                    var first = OFirst(symbol);
                    oblow.Add(botMarker, new Dictionary<Symbol, int>());
                    foreach (var occur in first)
                    {
                        oblow[botMarker].Add(occur, 1);
                    }
                }
                else
                {
                    for (int j = 0; j < rhs.Count; ++j)
                    {
                        var current = new Symbol(rhs[j].ToString(), p.Id, j+1);
                        allGrammarOccurs.Add(current);
                        if (j != rhs.Count - 1)
                        {
                            var first = OFirst(new Symbol(rhs[j + 1].ToString(), p.Id, j+2));
                            oblow.Add(current, new Dictionary<Symbol, int>());
                            foreach (var occur in first) oblow[current].Add(occur, 1);
                        }
                    }
                }
            }

            Console.WriteLine("\nПолученная матрица отношения OBLOW: ");
            Console.Write("    ");
            foreach (var x in allGrammarOccurs)
            {
                if (x.symbol != "^") Console.Write("{0, 4}", x.symbol + x.production.ToString() + x.symbolPosition.ToString());
            }
            Console.Write("\n");
            foreach (var x in allGrammarOccurs)
            {
                Console.Write("{0, 4}", x.symbol + (x.symbol == "^"? "" : x.production.ToString() + x.symbolPosition.ToString()));
                foreach (var y in allGrammarOccurs)
                {
                    if (y.symbol == "^") continue;
                    Console.Write("{0, 4}",(oblow.ContainsKey(x) && oblow[x].ContainsKey(y) ? "1" : " "));
                }
                Console.Write("\n");
            }

            Console.Write("\nДля продолжения нажмите <Enter>");
            Console.ReadLine();

            // Построение промежуточной матрицы для таблицы переходов g(X)
            var building = new Dictionary<Symbol, Dictionary<Symbol, List<Symbol> >>();
            HashSet<Symbol> nonDeterministic = new HashSet<Symbol>();

            foreach (var x in allGrammarOccurs)
            {
                building.Add(x, new Dictionary<Symbol, List<Symbol>>());
                foreach (var y in allGrammarOccurs)
                {
                    if (y != "^" && oblow.ContainsKey(x) && oblow[x].ContainsKey(y))
                    {
                        if (!building[x].ContainsKey(y.symbol)) building[x].Add(y.symbol, new List<Symbol>());
                        building[x][y.symbol].Add(y);
                        if (building[x][y.symbol].Count > 1)
                        {
                            foreach (var bad in building[x][y.symbol]) { nonDeterministic.Add(bad); }
                        }
                    }
                }
            }

            var Z = new Dictionary<string, Dictionary<Symbol, string>>();
            var Vp = new HashSet<string>();
            var M = new Dictionary<string, List<Symbol>>();

            var alphabet = new List<Symbol>(grammar.T);
            alphabet.AddRange(grammar.V);
            foreach (var x in allGrammarOccurs)
            {
                if (nonDeterministic.Contains(x)) continue;
                var XArray = new List<Symbol>();
                XArray.Add(x);
                string vX = GetVp(XArray, M);
                Vp.Add(vX);
                Z.Add(vX, new Dictionary<Symbol, string>());
                foreach (var t1 in alphabet)
                {
                    if (t1 == "$" || t1 == "П") continue;
                    if (!building.ContainsKey(x) || !building[x].ContainsKey(t1)) continue;
                    string vD = GetVp(building[x][t1], M);
                    Vp.Add(vD);
                    Z[vX].Add(t1, vD);
                    if (building[x][t1].Count > 1 && !Z.ContainsKey(vD)) //добавлен if
                    {
                        Z.Add(vD, new Dictionary<Symbol, string>());
                        foreach (var d in building[x][t1])
                        {
                            foreach (var t2 in alphabet)
                            {
                                if (building.ContainsKey(d) && building[d].ContainsKey(t2))
                                {
                                    Z[vD].Add(t2, GetVp(building[d][t2], M));
                                }
                            }
                        }
                    }
                }
            }

            Console.Write("\nПолученная матрица для функции переходов g(X):\n      ");
            foreach (var t in alphabet)
            {
                if (t != "$" && t != "П") Console.Write("{0, 8}", t.symbol);
            }
            Console.Write("\n");
            foreach (var v in Vp)
            {
                Console.Write("{0, 8}", v);
                foreach (var t in alphabet)
                {
                    if (t == "$" || t == "$") continue;
                    Console.Write("{0, 8}", (Z.ContainsKey(v) && Z[v].ContainsKey(t) ? Z[v][t] : ""));
                }
                Console.Write("\n");
            }

            Console.Write("\nДля продолжения нажмите <Enter>");
            Console.ReadLine();


            var H = new Dictionary<string, Dictionary<Symbol, string>>();

            foreach (var v in Vp)
            {
                H.Add(v, new Dictionary<Symbol, string>());
                foreach (var t in grammar.T)
                {
                    var first = (Symbol)M[v][0];
                    if (first.Equals(start) && M[v].Count == 1 && t == "$")
                    {
                        H[v].Add(t, "ДОПУСК");
                        continue;
                    }
                    else if (!first.Equals(start) && first != "^" && M[v].Count == 1 && grammar.P[first.production].RHS.Count == first.symbolPosition)
                    {
                        H[v].Add(t, "СВЕРТКА " + first.production.ToString());
                        continue;
                    }
                    if (t != "$")
                    {
                        bool check = true;
                        foreach (var x in M[v])
                        {
                            if (grammar.P[x.production].RHS.Count == x.symbolPosition) { check = false; }
                        }
                        if (check)
                        {
                            H[v].Add(t, "ПЕРЕНОС");
                            continue;
                        }
                    }
                    if (M[v].Contains(start))
                    {
                        bool check = false;
                        foreach (var x in M[v])
                        {
                            if (!v.Equals(start) && grammar.P[x.production].RHS.Count != x.symbolPosition)
                            {
                                check = true;
                                break;
                            }
                        }
                        if (check)
                        {
                            if (t == "$")
                            {
                                H[v].Add(t, "ДОПУСК");
                            }
                            else
                            {
                                H[v].Add(t, "ПЕРЕНОС");
                            }
                        }
                    }
                }
            }

            Console.Write("\nПолученная матрицы для функции действий f(X):\n      ");
            foreach (var t in grammar.T)
            {
                Console.Write("{0, 10}", t.symbol);
            }
            Console.Write("\n");
            foreach (var v in Vp)
            {
                Console.Write("{0, 8}", v);
                foreach (var t in grammar.T)
                {
                    Console.Write("{0, 10}", (H[v].ContainsKey(t) ? H[v][t] : "ОШИБКА"));
                }
                Console.Write("\n");
            }

            Console.Write("\nДля продолжения нажмите <Enter>");
            Console.ReadLine();

            for(; ;)
            {
                Console.Write("\nВведите цепочку: ");
                string input = Console.In.ReadLine();
                input += "$";
                var result = new List<int>();
                var VpSymbols = new List<string>();
                VpSymbols.Add("^00");
                bool good = false;
                for (int i = 0; i < input.Length; ++i)
                {
                    var inputSymbol = new Symbol(input[i].ToString());
                    if (!H.ContainsKey(VpSymbols[VpSymbols.Count - 1].ToString()) || !H[VpSymbols[VpSymbols.Count - 1].ToString()].ContainsKey(inputSymbol))
                    {
                        break;
                    }
                    if (H[VpSymbols[VpSymbols.Count - 1].ToString()][inputSymbol] == "ДОПУСК")
                    {
                        good = true;
                        break;
                    }
                    else if (H[VpSymbols[VpSymbols.Count - 1].ToString()][inputSymbol] == "ПЕРЕНОС")
                    {
                        if (!Z.ContainsKey(VpSymbols[VpSymbols.Count - 1].ToString()) || !Z[VpSymbols[VpSymbols.Count - 1].ToString()].ContainsKey(inputSymbol))
                        {
                            break;
                        }
                        VpSymbols.Add(Z[VpSymbols[VpSymbols.Count - 1].ToString()][inputSymbol]);
                    }
                    else
                    {
                        string numberStr = H[VpSymbols[VpSymbols.Count - 1].ToString()][inputSymbol].Substring(8);
                        int numberInt = Int32.Parse(numberStr);
                        result.Add(numberInt);
                        VpSymbols.RemoveRange(VpSymbols.Count - grammar.P[numberInt].RHS.Count, grammar.P[numberInt].RHS.Count);
                        if (!Z.ContainsKey(VpSymbols[VpSymbols.Count - 1].ToString()) || !Z[VpSymbols[VpSymbols.Count - 1].ToString()].ContainsKey(grammar.P[numberInt].LHS))
                        {
                            break;
                        }
                        VpSymbols.Add(Z[VpSymbols[VpSymbols.Count - 1].ToString()][grammar.P[numberInt].LHS]);
                        --i;
                    }
                }
                if (good)
                {
                    Console.WriteLine("Входная цепочка " + input + " распознана.");
                    Console.Write("Результат правого вывода: ");
                    foreach (var rule in result)
                    {
                        Console.Write(rule.ToString() + " ");
                    }
                    Console.Write("\n");
                }
                else
                {
                    Console.WriteLine("Входная цепочка не распознана.");
                }
                for (; ;)
                {
                    Console.Write("Вы хотите продолжить (да/нет)? - ");
                    string answer = Console.In.ReadLine();
                    if (answer == "нет")
                    {
                        return;
                    }
                    else if (answer == "да")
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
        
        public void Execute_LR1()
        {
            Console.WriteLine("\nИсходная ");
            Info();
            grammar.EpsDelete();
            var augmentedRHS = new List<Symbol>();
            augmentedRHS.Add(grammar.S0);
            var augmentedRule = new Production(new Symbol("П"), augmentedRHS);
            augmentedRule.Id = 0;
            grammar.P.Insert(0, augmentedRule);
            grammar.V.Add(new Symbol("П"));
            grammar.T.Add(new Symbol("$"));
            var start = new Symbol(grammar.S0.symbol, 0, 1);
            Console.WriteLine("\nПополненная грамматика: ");
            Info();

            // Нахождение матрицы отношения OBLOW и множества грамматических вхождений
            var oblow = new Dictionary<Symbol, Dictionary<Symbol, int>>();
            var allGrammarOccurs = new HashSet<Symbol>();
            foreach (var p in grammar.P)
            {
                var rhs = p.RHS;
                if (p.LHS == "П")
                {
                    var symbol = new Symbol(rhs[0].ToString(), p.Id, 1);
                    var botMarker = new Symbol("^", 0, 0);
                    allGrammarOccurs.Add(botMarker);
                    allGrammarOccurs.Add(symbol);
                    var first = OFirst(symbol);
                    oblow.Add(botMarker, new Dictionary<Symbol, int>());
                    foreach (var occur in first)
                    {
                        oblow[botMarker].Add(occur, 1);
                    }
                }
                else
                {
                    for (int j = 0; j < rhs.Count; ++j)
                    {
                        var current = new Symbol(rhs[j].ToString(), p.Id, j+1);
                        allGrammarOccurs.Add(current);
                        if (j != rhs.Count - 1)
                        {
                            var first = OFirst(new Symbol(rhs[j + 1].ToString(), p.Id, j+2));
                            oblow.Add(current, new Dictionary<Symbol, int>());
                            foreach (var occur in first) oblow[current].Add(occur, 1);
                        }
                    }
                }
            }

            Console.WriteLine("\nПолученная матрица отношения OBLOW: ");
            Console.Write("    ");
            foreach (var x in allGrammarOccurs)
            {
                if (x.symbol != "^") Console.Write("{0, 4}", x.symbol + x.production.ToString() + x.symbolPosition.ToString());
            }
            Console.Write("\n");
            foreach (var x in allGrammarOccurs)
            {
                Console.Write("{0, 4}", x.symbol + (x.symbol == "^"? "" : x.production.ToString() + x.symbolPosition.ToString()));
                foreach (var y in allGrammarOccurs)
                {
                    if (y.symbol == "^") continue;
                    Console.Write("{0, 4}",(oblow.ContainsKey(x) && oblow[x].ContainsKey(y) ? "1" : " "));
                }
                Console.Write("\n");
            }

            Console.Write("\nДля продолжения нажмите <Enter>");
            Console.ReadLine();

            // Построение промежуточной матрицы для таблицы переходов g(X)
            var building = new Dictionary<Symbol, Dictionary<Symbol, List<Symbol> >>();
            HashSet<Symbol> nonDeterministic = new HashSet<Symbol>();

            foreach (var x in allGrammarOccurs)
            {
                building.Add(x, new Dictionary<Symbol, List<Symbol>>());
                foreach (var y in allGrammarOccurs)
                {
                    if (y != "^" && oblow.ContainsKey(x) && oblow[x].ContainsKey(y))
                    {
                        if (!building[x].ContainsKey(y.symbol)) building[x].Add(y.symbol, new List<Symbol>());
                        building[x][y.symbol].Add(y);
                        if (building[x][y.symbol].Count > 1)
                        {
                            foreach (var bad in building[x][y.symbol]) { nonDeterministic.Add(bad); }
                        }
                    }
                }
            }

            var Z = new Dictionary<string, Dictionary<Symbol, string>>();
            var Vp = new HashSet<string>();
            var M = new Dictionary<string, List<Symbol>>();

            var alphabet = new List<Symbol>(grammar.T);
            alphabet.AddRange(grammar.V);
            foreach (var x in allGrammarOccurs)
            {
                if (nonDeterministic.Contains(x)) continue;
                var XArray = new List<Symbol>();
                XArray.Add(x);
                string vX = GetVp(XArray, M);
                Vp.Add(vX);
                Z.Add(vX, new Dictionary<Symbol, string>());
                foreach (var t1 in alphabet)
                {
                    if (t1 == "$" || t1 == "П") continue;
                    if (!building.ContainsKey(x) || !building[x].ContainsKey(t1)) continue;
                    string vD = GetVp(building[x][t1], M);
                    Vp.Add(vD);
                    Z[vX].Add(t1, vD);
                    if (building[x][t1].Count > 1 && !Z.ContainsKey(vD)) 
                    {
                        Z.Add(vD, new Dictionary<Symbol, string>());
                        foreach (var d in building[x][t1])
                        {
                            foreach (var t2 in alphabet)
                            {
                                if (building.ContainsKey(d) && building[d].ContainsKey(t2))
                                {
                                    Z[vD].Add(t2, GetVp(building[d][t2], M));
                                }
                            }
                        }
                    }
                }
            }

            Console.Write("\nПолученная матрица для функции переходов g(X):\n      ");
            foreach (var t in alphabet)
            {
                if (t != "$" && t != "П") Console.Write("{0, 8}", t.symbol);
            }
            Console.Write("\n");
            foreach (var v in Vp)
            {
                Console.Write("{0, 8}", v);
                foreach (var t in alphabet)
                {
                    if (t == "$" || t == "$") continue;
                    Console.Write("{0, 8}", (Z.ContainsKey(v) && Z[v].ContainsKey(t) ? Z[v][t] : ""));
                }
                Console.Write("\n");
            }

            Console.Write("\nДля продолжения нажмите <Enter>");
            Console.ReadLine();
            
            grammar.ComputeFirstFollow();
            

            var H = new Dictionary<string, Dictionary<Symbol, string>>();

            foreach (var v in Vp)
            {
                H.Add(v, new Dictionary<Symbol, string>());
                foreach (var t in grammar.T)
                {
                    var first = (Symbol)M[v][0];
                    if (first.Equals(start) && t == "$")
                    {
                        H[v].Add(t, "ДОПУСК");
                        continue;
                    }

                    var check = false;
                    foreach (var x in M[v])
                    {
                        if (grammar.P[x.production].RHS.Count == x.symbolPosition && 
                            grammar.Follow(grammar.P[x.production].LHS).Contains(t))
                        {
                            check = true;
                            break;
                        }
                    }
                    if (check)
                    {
                        H[v].Add(t, "СВЕРТКА " + first.production.ToString());
                        continue;
                    }
                    if ((Z.ContainsKey(v) && Z[v].ContainsKey(t)) && t != "$")
                    {
                        H[v].Add(t, "ПЕРЕНОС");
                        continue;
                    }
                }
            }

            Console.Write("\nПолученная матрицы для функции действий f(X):\n      ");
            foreach (var t in grammar.T)
            {
                Console.Write("{0, 10}", t.symbol);
            }
            Console.Write("\n");
            foreach (var v in Vp)
            {
                Console.Write("{0, 8}", v);
                foreach (var t in grammar.T)
                {
                    Console.Write("{0, 10}", (H[v].ContainsKey(t) ? H[v][t] : "ОШИБКА"));
                }
                Console.Write("\n");
            }

            Console.Write("\nДля продолжения нажмите <Enter>");
            Console.ReadLine();

            for(; ;)
            {
                Console.Write("\nВведите цепочку: ");
                string input = Console.In.ReadLine();
                input += "$";
                var result = new List<int>();
                var VpSymbols = new List<string>();
                VpSymbols.Add("^00");
                bool good = false;
                for (int i = 0; i < input.Length; ++i)
                {
                    var inputSymbol = new Symbol(input[i].ToString());
                    if (!H.ContainsKey(VpSymbols[VpSymbols.Count - 1].ToString()) || !H[VpSymbols[VpSymbols.Count - 1].ToString()].ContainsKey(inputSymbol))
                    {
                        break;
                    }
                    if (H[VpSymbols[VpSymbols.Count - 1].ToString()][inputSymbol] == "ДОПУСК")
                    {
                        good = true;
                        break;
                    }
                    else if (H[VpSymbols[VpSymbols.Count - 1].ToString()][inputSymbol] == "ПЕРЕНОС")
                    {
                        if (!Z.ContainsKey(VpSymbols[VpSymbols.Count - 1].ToString()) || !Z[VpSymbols[VpSymbols.Count - 1].ToString()].ContainsKey(inputSymbol))
                        {
                            break;
                        }
                        VpSymbols.Add(Z[VpSymbols[VpSymbols.Count - 1].ToString()][inputSymbol]);
                    }
                    else
                    {
                        string numberStr = H[VpSymbols[VpSymbols.Count - 1].ToString()][inputSymbol].Substring(8);
                        int numberInt = Int32.Parse(numberStr);
                        result.Add(numberInt);
                        VpSymbols.RemoveRange(VpSymbols.Count - grammar.P[numberInt].RHS.Count, grammar.P[numberInt].RHS.Count);
                        if (!Z.ContainsKey(VpSymbols[VpSymbols.Count - 1].ToString()) || !Z[VpSymbols[VpSymbols.Count - 1].ToString()].ContainsKey(grammar.P[numberInt].LHS))
                        {
                            break;
                        }
                        VpSymbols.Add(Z[VpSymbols[VpSymbols.Count - 1].ToString()][grammar.P[numberInt].LHS]);
                        --i;
                    }
                }
                if (good)
                {
                    Console.WriteLine("Входная цепочка " + input + " распознана.");
                    Console.Write("Результат правого вывода: ");
                    foreach (var rule in result)
                    {
                        Console.Write(rule.ToString() + " ");
                    }
                    Console.Write("\n");
                }
                else
                {
                    Console.WriteLine("Входная цепочка не распознана.");
                }
                for (; ;)
                {
                    Console.Write("Вы хотите продолжить (да/нет)? - ");
                    string answer = Console.In.ReadLine();
                    if (answer == "нет" || answer == "n")
                    {
                        return;
                    }
                    else if (answer == "да" || answer == "y")
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }
    }
}

/*
E E+T
E T
T T*P
T P
P (E)
P i
 */