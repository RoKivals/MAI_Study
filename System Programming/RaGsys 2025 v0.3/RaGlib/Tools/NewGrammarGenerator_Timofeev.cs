using System;
using RaGlib.Core;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Tools
{
    public class VarGrammar
    {
        public Symbol S0;
        public List<Symbol> T;
        public List<Symbol> V;
        public List<Production> P;

        public VarGrammar(Symbol S0, List<Symbol> T, List<Symbol> V, List<Production> P)
        {
            this.S0 = S0;
            this.T = T;
            this.V = V;
            this.P = P;
        }

    }
    public class CoreGrammarGenerator
    {
        public VarGrammar g = new VarGrammar("S",
            new List<Symbol>() { "a", "b", "c", "d", "f" },
            new List<Symbol>() { "S", "A", "B", "C", "D" },
            new List<Production>()
            {
                new Production("S", new List<Symbol>() { "a", "A", "B" }),
                new Production("A", new List<Symbol>() { "b", "C" }),
                new Production("B", new List<Symbol>() { "c", "A" }),
                new Production("C", new List<Symbol>() { "d", "D" }),
                new Production("D", new List<Symbol>() { "f" })
            });

        public VarGrammar h = new VarGrammar("S",
            new List<Symbol>() { "a", "d", "c" },
            new List<Symbol>() { "S", "A", "B", "C", "D" },
            new List<Production>()
            {
                new Production("S", new List<Symbol>() { "A", "B"}),
                new Production("A", new List<Symbol>() { "C", "D" }),
                new Production("B", new List<Symbol>() { "a" }),
                new Production("C", new List<Symbol>() { "d" }),
                new Production("D", new List<Symbol>() { "c" })
            });

        public VarGrammar v = new VarGrammar("S",
            new List<Symbol>() { "a", "b", "c", "d", "f", "g" },
            new List<Symbol>() { "S", "A", "B", "C", "D" },
            new List<Production>()
            {
                new Production("S", new List<Symbol>() { "a", "A", "b" }),
                new Production("A", new List<Symbol>() { "B", "C", "D" }),
                new Production("B", new List<Symbol>() { "c", "D" }),
                new Production("C", new List<Symbol>() { "d", "f", "B" }),
                new Production("D", new List<Symbol>() { "g" })
            });


        public string PrintGrammar(VarGrammar vargrammar, int variant, string form)
        {
            String result = variant + " " + form + " " + "G = {(";
            for (int i = 0; i < vargrammar.T.Count; ++i)
            {
                result = result + " ";
                result = result + vargrammar.T[i];
                if (i + 1 == vargrammar.T.Count)
                {
                    result = result + " )";
                }
                else
                {
                    result = result + ",";
                }
            }
            result = result + ",(";
            for (int i = 0; i < vargrammar.V.Count; ++i)
            {
                result = result + " ";
                result = result + vargrammar.V[i];
                if (i + 1 == vargrammar.V.Count)
                {
                    result = result + " )";
                }
                else
                {
                    result = result + ",";
                }
            }
            result = result + ",(";
            for (int i = 0; i < vargrammar.P.Count; ++i)
            {
                result = result + " ";
                result = result + vargrammar.P[i].LHS + " → " + String.Join("", vargrammar.P[i].RHS);
                if (i + 1 == vargrammar.P.Count)
                {
                    result = result + ")," + vargrammar.S0 + "}\n";
                }
                else
                {
                    result = result + ",";
                }
            }
            return result;
        }

        public VarGrammar RandomVariant(List<Symbol> Vn, List<Symbol> Tn, List<Production> Pn, int seed)
        {
            Random rand = new Random(seed);
            List<Symbol> V = Vn;
            List<Symbol> T = Tn;
            List<Production> P = Pn;

            List<char> nonterm = new List<char>("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray());
            List<char> term = new List<char>("abcdfghijklmnopqrstuvwxyz".ToCharArray());
            //-------------------------------------------------------------------------------------------------
            //(1) добавление цепных правил
            //-------------------------------------------------------------------------------------------------
            Production changable = P[rand.Next(P.Count)];
            while (changable.RHS.Count != 1)
            {
                changable = P[rand.Next(P.Count)];
            }
            Symbol nonterm_LHS = Char.ToString(nonterm[rand.Next(nonterm.Count)]);
            while (V.Contains(nonterm_LHS))
            {
                nonterm_LHS = Char.ToString(nonterm[rand.Next(nonterm.Count)]);
            }
            V.Add(nonterm_LHS);
            P.Remove(changable);
            Symbol newterm = changable.RHS[0];
            changable.RHS.Clear();
            changable.RHS.Add(nonterm_LHS);
            P.Add(changable);
            P.Add(new Production(nonterm_LHS, new List<Symbol>() { newterm }));
            //-------------------------------------------------------------------------------------------------
            //(2) добавление eps-правил
            //-------------------------------------------------------------------------------------------------
            T.Add("epsilon");
            nonterm_LHS = Char.ToString(nonterm[rand.Next(nonterm.Count)]);
            while (V.Contains(nonterm_LHS))
            {
                nonterm_LHS = Char.ToString(nonterm[rand.Next(nonterm.Count)]);
            }
            V.Add(nonterm_LHS);
            changable = P[rand.Next(P.Count)];
            while (changable.RHS.Count < 2)
            {
                changable = P[rand.Next(P.Count)];
            }
            P.Remove(changable);
            changable.RHS.Add(nonterm_LHS);
            P.Add(changable);
            P.Add(new Production(nonterm_LHS, new List<Symbol>() { "epsilon" }));
            //-------------------------------------------------------------------------------------------------
            //(3.1) добавление недостижимых символов
            //-------------------------------------------------------------------------------------------------
            nonterm_LHS = Char.ToString(nonterm[rand.Next(nonterm.Count)]);
            while (V.Contains(nonterm_LHS))
            {
                nonterm_LHS = Char.ToString(nonterm[rand.Next(nonterm.Count)]);
            }
            int size_of_right_side = rand.Next(1, 4);
            Symbol term_RHS_1 = Char.ToString(term[rand.Next(term.Count)]);
            Symbol term_RHS_2 = Char.ToString(term[rand.Next(term.Count)]);
            Symbol nonterm_RHS_1 = Char.ToString(nonterm[rand.Next(nonterm.Count)]);
            if (size_of_right_side == 1)
            {
                P.Add(new Production(nonterm_LHS, new List<Symbol>() { term_RHS_1 }));
                if (!V.Contains(nonterm_LHS))
                {
                    V.Add(nonterm_LHS);
                }
                if (!T.Contains(term_RHS_1))
                {
                    T.Add(term_RHS_1);
                }
            }
            else if (size_of_right_side == 2)
            {
                P.Add(new Production(nonterm_LHS, new List<Symbol>() { term_RHS_1, nonterm_RHS_1 }));
                if (!V.Contains(nonterm_LHS))
                {
                    V.Add(nonterm_LHS);
                }
                if (!V.Contains(nonterm_RHS_1))
                {
                    V.Add(nonterm_RHS_1);
                }
                if (!T.Contains(term_RHS_1))
                {
                    T.Add(term_RHS_1);
                }
            }
            else
            {
                P.Add(new Production(nonterm_LHS, new List<Symbol>() { term_RHS_1, nonterm_RHS_1, term_RHS_2 }));
                if (!V.Contains(nonterm_LHS))
                {
                    V.Add(nonterm_LHS);
                }
                if (!V.Contains(nonterm_RHS_1))
                {
                    V.Add(nonterm_RHS_1);
                }
                if (!T.Contains(term_RHS_1))
                {
                    T.Add(term_RHS_1);
                }
                if (!T.Contains(term_RHS_2))
                {
                    T.Add(term_RHS_2);
                }
            }
            //-------------------------------------------------------------------------------------------------
            //(3.2) добавление непроизводящих символов
            //-------------------------------------------------------------------------------------------------
            nonterm_LHS = Char.ToString(nonterm[rand.Next(nonterm.Count)]);
            while (V.Contains(nonterm_LHS))
            {
                nonterm_LHS = Char.ToString(nonterm[rand.Next(nonterm.Count)]);
            }
            size_of_right_side = rand.Next(1, 4);
            term_RHS_1 = Char.ToString(term[rand.Next(term.Count)]);
            term_RHS_2 = Char.ToString(term[rand.Next(term.Count)]);
            string s = Char.ToString(nonterm[rand.Next(4, nonterm.Count)]);
            while (s == nonterm_LHS.symbol || V.Contains(s))
            {
                s = Char.ToString(nonterm[rand.Next(4, nonterm.Count)]);
            }
            nonterm_RHS_1 = s;
            if (size_of_right_side == 1)
            {
                P.Add(new Production(nonterm_LHS, new List<Symbol>() { term_RHS_1 }));
                if (!V.Contains(nonterm_LHS))
                {
                    V.Add(nonterm_LHS);
                }
                if (!T.Contains(term_RHS_1))
                {
                    T.Add(term_RHS_1);
                }
            }
            else if (size_of_right_side == 2)
            {
                P.Add(new Production(nonterm_LHS, new List<Symbol>() { term_RHS_1, nonterm_RHS_1 }));
                if (!V.Contains(nonterm_LHS))
                {
                    V.Add(nonterm_LHS);
                }
                if (!V.Contains(nonterm_RHS_1))
                {
                    V.Add(nonterm_RHS_1);
                }
                if (!T.Contains(term_RHS_1))
                {
                    T.Add(term_RHS_1);
                }
            }
            else
            {
                P.Add(new Production(nonterm_LHS, new List<Symbol>() { term_RHS_1, nonterm_RHS_1, term_RHS_2 }));
                if (!V.Contains(nonterm_LHS))
                {
                    V.Add(nonterm_LHS);
                }
                if (!V.Contains(nonterm_RHS_1))
                {
                    V.Add(nonterm_RHS_1);
                }
                if (!T.Contains(term_RHS_1))
                {
                    T.Add(term_RHS_1);
                }
                if (!T.Contains(term_RHS_2))
                {
                    T.Add(term_RHS_2);
                }
            }
            //-------------------------------------------------------------------------------------------------
            //(4) добавление левой рекурсии
            //-------------------------------------------------------------------------------------------------
            Symbol prerec = V[rand.Next(5)];
            term_RHS_1 = Char.ToString(term[rand.Next(term.Count)]);
            term_RHS_2 = Char.ToString(term[rand.Next(term.Count)]);
            if (!T.Contains(term_RHS_1))
            {
                T.Add(term_RHS_1);
            }
            if (!T.Contains(term_RHS_2))
            {
                T.Add(term_RHS_2);
            }
            nonterm_LHS = Char.ToString(nonterm[rand.Next(nonterm.Count)]);
            while (V.Contains(nonterm_LHS))
            {
                nonterm_LHS = Char.ToString(nonterm[rand.Next(nonterm.Count)]);
            }
            if (!V.Contains(nonterm_LHS))
            {
                V.Add(nonterm_LHS);
            }
            P.Add(new Production(prerec, new List<Symbol>() { term_RHS_2, nonterm_LHS }));
            P.Add(new Production(nonterm_LHS, new List<Symbol>() { nonterm_LHS, term_RHS_1 }));
            P.Add(new Production(nonterm_LHS, new List<Symbol>() { term_RHS_1 }));
            //-------------------------------------------------------------------------------------------------
            return new VarGrammar("S", T, V, P);
        }
        public string Generator(int i, int seed)
        {
            string result = "";
            VarGrammar g1 = RandomVariant(g.V, g.T, g.P, (i+seed));
            string a = PrintGrammar(g1, i, "G");
            result = result + a + "\n";
            VarGrammar h1 = RandomVariant(h.V, h.T, h.P, (i+1+seed));
            string b = PrintGrammar(h1, i+1, "H");
            result = result + b + "\n";
            VarGrammar v1 = RandomVariant(v.V, v.T, v.P, (i+2+seed));
            string c = PrintGrammar(v1, i+2, "V");
            result = result + c + "\n";
            return result;

        }
    }
    public class NewGrammarGenerator
    {
        public VarGrammar GetTask(int variant, string path) //из файла скачивает нужный вариант и переводит в грамматику
        {
            List<Symbol> Ti = new List<Symbol>();
            List<Symbol> Vi = new List<Symbol>();
            List<string> dP = new List<string>();
            List<Production> Pi = new List<Production>();
            StreamReader sr = new StreamReader(path);
            string line = "";
            for (int i = 0; i < variant*2-1; i++)
            {
                line = sr.ReadLine();
            }
            line = sr.ReadLine();

            string[] ss = line.Split('(', ')');
            string s1 = ss[1];
            string s2 = ss[3];
            string s3 = ss[5];
            string[] ss1 = s1.Split(',');
            string[] ss2 = s2.Split(',');
            string[] ss3 = s3.Split(',');
            char[] ss4;
            for (int i = 0; i < ss1.Length; i++)
            {
                Ti.Add(new Symbol(ss1[i].Replace(" ", "")));
            }
            for (int i = 0; i < ss2.Length; i++)
            {
                Vi.Add(new Symbol(ss2[i].Replace(" ", "")));
            }
            for (int i = 0; i < ss3.Length; i++)
            {
                dP.Add(ss3[i].Replace(" ", ""));
            }
            foreach (string val in dP)
            {
                List<Symbol> slist = new List<Symbol>();
                ss4 = val.ToCharArray();
                string[] ss5 = new string[ss4.Length];
                for (int j = 0; j < ss4.Length; j++)
                {
                    ss5[j] = ss4[j].ToString();
                }
                string LHS = ss5[0];
                for (int i = 2; i < ss5.Length; i++)
                {
                    if(ss5[i] == "e")
                    {
                        slist.Add(new Symbol(""));
                        break;
                    }
                    slist.Add(new Symbol(ss5[i]));
                }
                Pi.Add(new Production(LHS, slist));
            }
            sr.Close();
            return new VarGrammar("S", Ti, Vi, Pi);
        }
        public void LogWrite(int seed, int number) //выводит 30 вариантов в файл
        {
            string m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Console.WriteLine(m_exePath);
            try
            {
                using (StreamWriter w = File.AppendText(m_exePath+"\\"+"log.txt"))
                {
                    NewGrammarGenerator ngg = new NewGrammarGenerator();
                    string logMessage = ngg.TableGenerator(seed, number);
                    w.Write(logMessage);
                    Console.WriteLine(logMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
        }
        public string TableGenerator(int seed, int number) //делает таблицу из 30 вариантов
        {
            string result = "Seed - " + seed + "\n";
            CoreGrammarGenerator cgg1 = new CoreGrammarGenerator();
            result = result + cgg1.Generator(1, seed);
            CoreGrammarGenerator cgg2 = new CoreGrammarGenerator();
            result = result + cgg2.Generator(4, seed+4);
            CoreGrammarGenerator cgg3 = new CoreGrammarGenerator();
            result = result + cgg3.Generator(7, seed+8);
            CoreGrammarGenerator cgg4 = new CoreGrammarGenerator();
            result = result + cgg4.Generator(10, seed+12);
            CoreGrammarGenerator cgg5 = new CoreGrammarGenerator();
            result = result + cgg5.Generator(13, seed+16);
            CoreGrammarGenerator cgg6 = new CoreGrammarGenerator();
            result = result + cgg6.Generator(16, seed+20);
            CoreGrammarGenerator cgg7 = new CoreGrammarGenerator();
            result = result + cgg7.Generator(19, seed+24);
            CoreGrammarGenerator cgg8 = new CoreGrammarGenerator();
            result = result + cgg8.Generator(22, seed+28);
            CoreGrammarGenerator cgg9 = new CoreGrammarGenerator();
            result = result + cgg9.Generator(25, seed+32);
            CoreGrammarGenerator cgg10 = new CoreGrammarGenerator();
            result = result + cgg10.Generator(28, seed+36);
            CoreGrammarGenerator cgg11 = new CoreGrammarGenerator();
            result = result + cgg11.Generator(31, seed+40);
            CoreGrammarGenerator cgg12 = new CoreGrammarGenerator();
            result = result + cgg12.Generator(34, seed+44);
            CoreGrammarGenerator cgg13 = new CoreGrammarGenerator();
            result = result + cgg13.Generator(37, seed+48);
            CoreGrammarGenerator cgg14 = new CoreGrammarGenerator();
            result = result + cgg14.Generator(40, seed+52);
            CoreGrammarGenerator cgg15 = new CoreGrammarGenerator();
            result = result + cgg15.Generator(43, seed+56);
            CoreGrammarGenerator cgg16 = new CoreGrammarGenerator();
            result = result + cgg16.Generator(46, seed+60);
            CoreGrammarGenerator cgg17 = new CoreGrammarGenerator();
            result = result + cgg17.Generator(49, seed+64);
            CoreGrammarGenerator cgg18 = new CoreGrammarGenerator();
            result = result + cgg18.Generator(52, seed+68);
            CoreGrammarGenerator cgg19 = new CoreGrammarGenerator();
            result = result + cgg19.Generator(55, seed+72);
            CoreGrammarGenerator cgg20 = new CoreGrammarGenerator();
            result = result + cgg20.Generator(58, seed+76);
            string form = number.ToString();
            return result.Substring(0, result.IndexOf(form));
        }
    }
}

