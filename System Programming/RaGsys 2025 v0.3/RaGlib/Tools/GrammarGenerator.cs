using System;
using RaGlib.Core;
using System.Collections;
using System.Collections.Generic;

namespace Assignment
{
    public class GrammarGenerator
    {
        private Symbol Terminals;
        private Symbol NonTerminals;
        private ArrayList Rules = new ArrayList();
        private Symbol first_symbol;
        private void GenerationInfo()
        {
            Console.WriteLine("g - Нормальная форма Грейбах");
            Console.WriteLine("c - Нормальная форма Хомского");
            Console.WriteLine("a - Произвольная\n");
            Console.WriteLine("Необходимо написать в строку вид грамматики и количество пополнений по позициям");
            Console.WriteLine("1 позиция - Недостижимые и не производящие символы,");
            Console.WriteLine("2 позиция - eps-правила,");
            Console.WriteLine("3 позиция - цепные правила,");
            Console.WriteLine("4 позиция - левая рекурсия)");
            Console.WriteLine("Пример ввода: g 1111, где g - грамматика в нормальной форме Грейбах," +
                " 1111 - каждый вид пополнения по одному разу");
        }
        private Symbol ReadGrammar(string selection)
        {
            bool ex = false;
            Symbol first_symbol = new Symbol();
            first_symbol.symbol = "0";
            Terminals = "";
            NonTerminals = "";
            Rules.Clear();
            string s;
            Hashtable term = new Hashtable();       //  временная таблица терминалов 
            Hashtable nonterm = new Hashtable();    //  и нетерминалов
            Console.WriteLine("Введите продукции");
            if (selection[0] == 'g')
            {
                Console.WriteLine("Грамматки без eps-правил, вида:");
                Console.WriteLine("A → aB");
                Console.WriteLine("B → b\n");
            }
            if (selection[0] == 'c')
            {
                Console.WriteLine("Грамматки без eps-правил, вида:");
                Console.WriteLine("A → BC");
                Console.WriteLine("B → b\n");
            }
            if (selection[0] == 'a')
            {
                Console.WriteLine("Грамматки без eps-правил,  произвольного вида:\n");
            }
            while ((s = Console.In.ReadLine()) != "")
            {
                if (!ex)
                {
                    ex = true;
                    first_symbol.symbol = s[0].ToString();
                }
                Rules.Add(s); //добавитьть правило в грамматику
                for (int i = 0; i < s.Length; i++)
                    //  анализ элементов правила
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
                Terminals.symbol += (char)c.Key;
            for (IDictionaryEnumerator c = nonterm.GetEnumerator(); c.MoveNext();)
                NonTerminals.symbol += (char)c.Key;

            return first_symbol;
        }
        private void GrammarDistribution(string selection)
        {
            Random rand = new Random();
            int b, c, d, e, f, g, h;
            int x1 = selection[2] - '0', x2 = selection[3] - '0', x3 = selection[4] - '0', x4 = selection[5] - '0';

            first_symbol = ReadGrammar(selection);

            ArrayList Nonterm1 = new ArrayList("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray());
            ArrayList term1 = new ArrayList("abcdfghijklmnopqrstuvwxyz".ToCharArray());
            ArrayList signs = new ArrayList("-+=@%^&*()/".ToCharArray());

            for (int i = 0; i < NonTerminals.symbol.Length; i++)
                Nonterm1.Remove(NonTerminals.symbol[i]);
            for (int i = 0; i < Terminals.symbol.Length; i++)
                term1.Remove(Terminals.symbol[i]);
            for (int i = 0; i < Terminals.symbol.Length; i++)
                signs.Remove(Terminals.symbol[i]);
            if (2 * x1 + x2 + x3 + x4 > Nonterm1.Count)
            {
                Console.WriteLine("Ошибка! Слишком большие значения");
            }

            if (x1 > 0)
            {
                for (int j = 0; j < x1; j++)
                {
                    for (int i = 0; i < NonTerminals.symbol.Length; i++)
                        Nonterm1.Remove(NonTerminals.symbol[i]);
                    for (int i = 0; i < Terminals.symbol.Length; i++)
                        term1.Remove(Terminals.symbol[i]);
                    for (int i = 0; i < Terminals.symbol.Length; i++)
                        signs.Remove(Terminals.symbol[i]);
                    if (selection[0] == 'g')
                    {
                        b = rand.Next(0, Nonterm1.Count);
                        c = rand.Next(0, Nonterm1.Count);
                        d = rand.Next(1, NonTerminals.symbol.Length);
                        e = rand.Next(0, term1.Count);
                        f = rand.Next(0, Terminals.symbol.Length);
                        g = rand.Next(0, Terminals.symbol.Length);
                        h = rand.Next(0, NonTerminals.symbol.Length);
                        NonTerminals.symbol += Nonterm1[b];
                        NonTerminals.symbol += Nonterm1[c];
                        Terminals.symbol += term1[e];
                        Rules.Add(NonTerminals.symbol[d] + " " + Terminals.symbol[f] + Nonterm1[c]);
                        if (h % 2 == 1)
                        {
                            Rules.Add(Nonterm1[c] + " " + Terminals.symbol[g] + Nonterm1[b] + Nonterm1[c]);
                        }
                        else
                        {
                            Rules.Add(Nonterm1[c] + " " + Terminals.symbol[g] + NonTerminals.symbol[h] + Nonterm1[b] + Nonterm1[c]);
                        }
                        Rules.Add(Nonterm1[b] + " " + term1[e]);
                    }
                    else if (selection[0] == 'c')
                    {
                        b = rand.Next(0, Nonterm1.Count);
                        c = rand.Next(0, Nonterm1.Count);
                        d = rand.Next(0, term1.Count);
                        e = rand.Next(0, NonTerminals.symbol.Length);
                        f = rand.Next(1, NonTerminals.symbol.Length);
                        g = rand.Next(1, NonTerminals.symbol.Length);
                        NonTerminals.symbol += Nonterm1[b];
                        NonTerminals.symbol += Nonterm1[c];
                        Terminals.symbol += term1[d];
                        Rules.Add(Nonterm1[b] + " " + term1[d]);
                        if (g % 2 == 1)
                        {
                            Rules.Add(NonTerminals.symbol[e] + " " + NonTerminals.symbol[f] + Nonterm1[c]);
                        }
                        else
                        {
                            Rules.Add(NonTerminals.symbol[e] + " " + NonTerminals.symbol[g] + NonTerminals.symbol[f] + Nonterm1[c]);
                        }
                        Rules.Add(Nonterm1[c] + " " + Nonterm1[b] + Nonterm1[c]);
                    }
                    else
                    {
                        b = rand.Next(0, Nonterm1.Count);
                        c = rand.Next(0, Nonterm1.Count);
                        d = rand.Next(0, term1.Count);
                        e = rand.Next(0, signs.Count);
                        f = rand.Next(1, NonTerminals.symbol.Length);
                        g = rand.Next(1, NonTerminals.symbol.Length);
                        NonTerminals.symbol += Nonterm1[b];
                        NonTerminals.symbol += Nonterm1[c];
                        Terminals.symbol += term1[d];
                        Terminals.symbol += signs[e];
                        Rules.Add(Nonterm1[b] + " " + term1[d]);
                        if (e % 2 == 1)
                        {
                            Rules.Add(NonTerminals.symbol[f] + " " + NonTerminals.symbol[g] + Nonterm1[c]);
                            Rules.Add(Nonterm1[c] + " " + NonTerminals.symbol[f] + signs[e] + Nonterm1[c]);
                        }
                        else
                        {
                            Rules.Add(Nonterm1[c] + " " + NonTerminals.symbol[g] + signs[e] + Nonterm1[c]);
                            Rules.Add(NonTerminals.symbol[g] + " " + NonTerminals.symbol[g] + Nonterm1[c]);
                        }
                    }
                }
            }

            if (x2 > 0)
            {
                for (int j = 0; j < x2; ++j)
                {
                    for (int i = 0; i < NonTerminals.symbol.Length; i++)
                        Nonterm1.Remove(NonTerminals.symbol[i]);
                    for (int i = 0; i < Terminals.symbol.Length; i++)
                        term1.Remove(Terminals.symbol[i]);
                    for (int i = 0; i < Terminals.symbol.Length; i++)
                        signs.Remove(Terminals.symbol[i]);

                    IEnumerator r1 = Rules.GetEnumerator();
                    b = rand.Next(0, Nonterm1.Count);
                    int n = rand.Next(1, Rules.Count);
                    for (int i = 0; i < n; ++i)
                    {
                        r1.MoveNext();
                    }
                    string str = ((string)r1.Current);
                    Rules.Remove(r1.Current);
                    str = str + Nonterm1[b];
                    Rules.Add(str);
                    Rules.Add(Nonterm1[b] + " " + "e");
                    NonTerminals.symbol += Nonterm1[b];
                }
                Terminals += "e";
            }

            if (x3 > 0)
            {
                for (int j = 0; j < x3; ++j)
                {
                    for (int i = 0; i < NonTerminals.symbol.Length; i++)
                        Nonterm1.Remove(NonTerminals.symbol[i]);
                    for (int i = 0; i < Terminals.symbol.Length; i++)
                        term1.Remove(Terminals.symbol[i]);
                    for (int i = 0; i < Terminals.symbol.Length; i++)
                        signs.Remove(Terminals.symbol[i]);

                    IEnumerator r2 = Rules.GetEnumerator();
                    r2.MoveNext();
                    b = rand.Next(0, Nonterm1.Count);
                    while (((string)r2.Current).Length != 3)
                    {
                        r2.MoveNext();
                    }
                    string sym1 = ((string)r2.Current).Substring(0);
                    string sym2 = ((string)r2.Current).Substring(2);
                    Rules.Remove(r2.Current);
                    NonTerminals.symbol += Nonterm1[b];
                    Rules.Add(sym1.Remove(2) + Nonterm1[b]);
                    Rules.Add(Nonterm1[b] + " " + sym2);
                }
            }

            if (x4 > 0)
            {
                for (int j = 0; j < x4; ++j)
                {
                    for (int i = 0; i < NonTerminals.symbol.Length; i++)
                        Nonterm1.Remove(NonTerminals.symbol[i]);
                    for (int i = 0; i < Terminals.symbol.Length; i++)
                        term1.Remove(Terminals.symbol[i]);
                    for (int i = 0; i < Terminals.symbol.Length; i++)
                        signs.Remove(Terminals.symbol[i]);
                    int l = rand.Next(1, 2);
                    b = rand.Next(0, Nonterm1.Count);
                    c = rand.Next(0, NonTerminals.symbol.Length);
                    d = rand.Next(0, NonTerminals.symbol.Length);
                    f = rand.Next(0, NonTerminals.symbol.Length);
                    e = rand.Next(0, term1.Count);
                    g = rand.Next(0, Terminals.symbol.Length);
                    NonTerminals.symbol += Nonterm1[b];
                    Terminals.symbol += term1[e];
                    if (selection[0] == 'a')
                    {
                        Rules.Add(NonTerminals.symbol[f] + " " + term1[e] + Nonterm1[b]);
                        Rules.Add(Nonterm1[b] + " " + Nonterm1[b] + Terminals.symbol[g] + NonTerminals.symbol[c]);
                    }
                    else
                    {
                        if (l == 2)
                        {
                            Rules.Add(NonTerminals.symbol[f] + " " + NonTerminals.symbol[c] + Nonterm1[b]);
                            Rules.Add(Nonterm1[b] + " " + Nonterm1[b] + Nonterm1[d]);
                        }
                        else
                        {
                            Rules.Add(NonTerminals.symbol[f] + " " + term1[e] + Nonterm1[b]);
                            Rules.Add(Nonterm1[b] + " " + Nonterm1[b] + Terminals.symbol[g]);
                        }
                    }
                }
                Nonterm1.Clear();
                term1.Clear();
                signs.Clear();
            }

        }
        private void PrintGrammar()
        {
            Console.WriteLine("\nГрамматика: \n ");
            Console.Write("G = {(");
            for (int i = 0; i < Terminals.symbol.Length; ++i)
            {
                Console.Write(" ");
                Console.Write(Terminals.symbol[i]);
                if (i + 1 == Terminals.symbol.Length)
                {
                    Console.Write(" )");

                }
                else
                {
                    Console.Write(",");
                }
            }
            Console.Write(",(");
            for (int i = 0; i < NonTerminals.symbol.Length; ++i)
            {
                Console.Write(" ");
                Console.Write(NonTerminals.symbol[i]);
                if (i + 1 == NonTerminals.symbol.Length)
                {
                    Console.Write(" )");

                }
                else
                {
                    Console.Write(",");
                }
            }
            Console.Write(",(");
            for (IEnumerator rule = Rules.GetEnumerator(); rule.MoveNext();)
            {
                string str = ((string)rule.Current);
                Console.Write(" ");
                Console.Write(str[0] + " → ");
                for (int i = 2; i < str.Length; ++i)
                    Console.Write(str[i]);
                Console.Write(",");
            }
            Console.Write(")," + first_symbol + "}\n");
        }
        public RaGlib.Grammar GenerateGrammar()
        {
            GenerationInfo();
            string selection = Console.ReadLine();
            GrammarDistribution(selection);
            PrintGrammar();
            List<Symbol> T = new List<Symbol>();
            for (int i = 0; i < Terminals.symbol.Length; ++i)
            {
                string buffer = "";
                buffer += Terminals.symbol[i];
                T.Add(new Symbol(buffer));
            }
            List<Symbol> V = new List<Symbol>();
            for (int i = 0; i < NonTerminals.symbol.Length; ++i)
            {
                string buffer = "";
                buffer += NonTerminals.symbol[i];
                V.Add(new Symbol(buffer));
            }
            RaGlib.Grammar newGrammar = new RaGlib.Grammar(T, V, first_symbol.symbol);
            for (IEnumerator rule = Rules.GetEnumerator(); rule.MoveNext();)
            {
                string str = ((string)rule.Current);
                string bufferLHS = "";
                bufferLHS += str[0];
                newGrammar.AddRule(bufferLHS, new List<Symbol>() { new Symbol(str.Substring(2)) });
            }
            return newGrammar;
        }
    }
}
