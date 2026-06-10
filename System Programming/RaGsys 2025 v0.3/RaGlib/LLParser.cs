// ============================================================================
// LLParser.cs - LL(1) анализатор (нисходящий синтаксический анализатор)
// ============================================================================
// LL(1) анализатор - это нисходящий синтаксический анализатор, который читает
// входную цепочку слева направо (Left-to-right) и строит левый вывод (Leftmost derivation).
// Использует таблицу управления для принятия решений о том, какое правило применить.
// Требует вычисления множеств FIRST и FOLLOW для построения таблицы разбора.
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using RaGlib.Core;

namespace RaGlib {

    public class LLParser
    {
        private Grammar G; // Грамматика для разбора
        private Stack<Symbol> Stack; // Стек (магазин) для символов грамматики
        private DataTable Table; // Управляющая таблица LL(1) анализатора
        public string OutputConfigure = ""; // Строка с конфигурациями разбора

        // Конструктор LL(1) анализатора - создает таблицу управления для грамматики
        public LLParser(Grammar grammar)
        {
            this.G = grammar;
            Table = new DataTable("ControlTable");
            Stack = new Stack<Symbol>(); //создаем стек(магазин) для символов
                                         // Создадим таблицу синтаксического анализа для этой грамматики

            // Определим структуру таблицы
            Table.Columns.Add(new DataColumn("VT", typeof(String)));
            Console.WriteLine("Создадим таблицу. Сначала создадим по столбцу для каждого из этих терминалов: ");
            foreach (var termSymbol in G.T)
            {
                Console.Write(termSymbol.symbol);
                Console.Write(", ");
                Table.Columns.Add(new DataColumn(termSymbol.symbol, typeof(Production)));
            }
            Console.WriteLine("\nТакже создаем строку для Эпсилон");
            Table.Columns.Add(new DataColumn("EoI", typeof(Production))); // Epsilon
            grammar.ComputeFirstFollow();
            for (int i = 0; i < grammar.V.Count; i++) // Рассмотрим последовательно все нетерминалы
            {
                DataRow workRow = Table.NewRow(); //Новая строка
                workRow["VT"] = (string)grammar.V[i].symbol;

                Console.Write("Рассмотрим нетерминал ");
                Console.Write((grammar.V[i].symbol));
                Console.Write("\n");

                var rules = getRules(grammar.V[i]);
                // Получим все правила, соответствующие текущему нетерминалу

                foreach (var rule in rules)
                {

                    var currFirstSet = grammar.First(rule.RHS);
                    foreach (var firstSymbol in currFirstSet)
                    {
                        if (firstSymbol != "")
                        {
                            // Добавить в таблицу
                            Console.Write("   Первый символ правила ");
                            Console.Write(rule.LHS);
                            Console.Write(" -> ");
                            for (int j = 0; j < rule.RHS.Count; j++)
                            {
                                Console.Write(rule.RHS[j]);
                            }
                            Console.Write(" - ");
                            Console.WriteLine(firstSymbol);

                            workRow[firstSymbol.symbol] = rule;
                            Console.Write("   Это правило заносим в таблицу на пересечении строки нетерминала ");
                            Console.Write(rule.LHS);
                            Console.Write(" и столбца терминала ");
                            Console.WriteLine(firstSymbol);
                            Console.Write("\n");
                        }
                        else
                        {
                            HashSet<Symbol> currFollowSet = grammar.Follow(rule.LHS);
                            foreach (var currFollowSymb in currFollowSet)
                            {
                                string currFollowSymbFix = (currFollowSymb == "") ? "EoI" : currFollowSymb.symbol;
                                workRow[currFollowSymbFix] = rule;
                            }
                        }
                    }
                }
                Table.Rows.Add(workRow);
            }
        }

        public bool Parse(List<Symbol> input)
        {
            Stack.Push("EoS"); // символ окончания входной последовательности
            Stack.Push(G.S0.symbol);
            int i = 0;
            Symbol currInputSymbol = input[0];
            Symbol currStackSymbol;
            do
            {
                currStackSymbol = Stack.Peek();
                if (G.T.Contains(currStackSymbol)) // в вершине стека находится терминал
                {
                    if (currInputSymbol == currStackSymbol) // распознанный символ равен вершине стека
                    {
                        // Извлечь из стека верхний элемент и распознать символ входной последовательности (ВЫБРОС)
                        Stack.Pop();
                        if (i != input.Count())
                        {
                            i++;
                        }
                        currInputSymbol = (i == input.Count()) ? "EoI" : input[i].ToString();
                    }
                    else
                    {
                        // ERROR
                        return false;
                    }
                }
                else // если в вершине стека нетерминал
                {
                    DataRow custRows = Table.Select("VT = '" + currStackSymbol.ToString().Replace(@"'", @"''") + "'", null)[0];
                    if (!custRows.IsNull(currInputSymbol.ToString())) // в клетке[вершина стека, распознанный символ] таблицы разбора существует правило
                    {
                        //  извлечь из стека элемент и занести в стек все терминалы и нетерминалы найденного в таблице правила в стек в порядке обратном порядку их следования в правиле
                        Stack.Pop();
                        List<Symbol> S = ((Production)custRows[currInputSymbol.ToString()]).RHS;
                        S.Reverse();
                        foreach (var chainSymbol in S) 
                        {
                            if (chainSymbol != "")
                            {
                                Stack.Push(chainSymbol);
                            }
                        }
                        OutputConfigure += (((Production)custRows[currInputSymbol.ToString()]).Id);
                    }
                    else
                    {
                        // ERROR
                        return false;
                    }
                }
            } while (Stack.Peek() != "EoS"); // вершина стека не равна концу входной последовательности

            if (i != input.Count()) // распознанный символ не равен концу входной последовательности
            {
                // ERROR
                return false;
            }

            return true;
        }

        public bool Parse1(List<Symbol> input) //Подробный вариант функции
        {
            Console.WriteLine("Приступаю к чтению цепочки символов...");
            Console.Write("      ("); //Просто выводит текущее состояние на экран!
            Console.Write(input);
            Console.Write(", ");
            Console.Write("S");
            Console.Write(", ");
            Console.Write(OutputConfigure);
            Console.WriteLine(" )\n");

            Stack.Push("EoS"); // символ окончания входной последовательности
            Stack.Push(G.S0.symbol);
            int i = 0;
            Symbol currInputSymbol = input[0];
            Symbol currStackSymbol;
            do
            {
                currStackSymbol = Stack.Peek();
                if (G.T.Contains(currStackSymbol)) // в вершине стека находится терминал
                {
                    Console.Write("   В вершине стека находится терминал ");
                    Console.WriteLine(currStackSymbol);
                    if (currInputSymbol == currStackSymbol) // распознанный символ равен вершине стека
                    {
                        Console.WriteLine("      И данный терминал равен вершине стека...");
                        // Извлечь из стека верхний элемент и распознать символ входной последовательности (ВЫБРОС)
                        Console.WriteLine("      Извлекаю из стека верхний элемент, распознаю символ входной последовательности...");
                        Console.WriteLine("      ВЫБРОС!");
                        Stack.Pop();
                        if (i != input.Count())
                        {
                            i++;
                        }
                        currInputSymbol = (i == input.Count()) ? "EoI" : input[i].ToString();

                        Console.Write("      (");

                        for (int k = i; k < input.Count(); k++)
                        {
                            Console.Write(input[k]);
                        }
                        Console.Write(", ");
                        for (int k = 0; k < Stack.Count; k++)
                        {
                            Symbol[] tmp = new Symbol[Stack.Count];
                            Stack.CopyTo(tmp, 0);
                            Console.Write(tmp[k]);
                        }
                        Console.Write(", ");
                        Console.Write(OutputConfigure);
                        Console.WriteLine(")\n");
                    }
                    else
                    {
                        // ERROR
                        Console.WriteLine("      ОШИБКА! Данный терминал не равен вершине стека!");
                        Console.Write(currInputSymbol);
                        Console.Write(" != ");
                        Console.WriteLine(currStackSymbol);
                        Console.Write("\n");
                        return false;
                    }
                }
                else // если в вершине стека нетерминал
                {
                    Console.Write("   В вершине стека нетерминал ");
                    Console.WriteLine(currStackSymbol);
                    DataRow custRows = Table.Select("VT = '" + currStackSymbol.ToString().Replace(@"'", @"''") + "'", null)[0];
                    if (!custRows.IsNull(currInputSymbol.ToString())) // в клетке[вершина стека, распознанный символ] таблицы разбора существует правило
                    {
                        Console.Write("      В таблице разбора, в клетке [");
                        Console.Write(currStackSymbol);
                        Console.Write(",");
                        Console.Write(currInputSymbol);
                        Console.WriteLine("] существует правило...");
                        //  извлечь из стека элемент и занести в стек все терминалы и нетерминалы найденного в таблице правила в стек в порядке обратном порядку их следования в правиле
                        Console.WriteLine("      Извлекаю из стека элемент и заношу все терминалы и нетерминалы\n      найденного в таблице правила в стек в порядке обратном порядку их следования в правиле.");
                        Console.Write("      Вот так: ");
                        Stack.Pop();
                        List<Symbol> S = ((Production)custRows[currInputSymbol.ToString()]).RHS;
                        S.Reverse();
                        foreach (var chainSymbol in S)
                        {
                            if (chainSymbol != "")
                            {
                                Console.Write(chainSymbol);
                                Stack.Push(chainSymbol);
                            }
                        }
                        Console.WriteLine();
                        OutputConfigure += (((Production)custRows[currInputSymbol.ToString()]).Id);
                        Console.Write("      Использовано правило под номером ");
                        Console.WriteLine(((Production)custRows[currInputSymbol.ToString()]).Id);
                        Console.Write("      (");

                        // int i = 0;
                        // string currInputSymbol = input[i].ToString();

                        for (int k = i; k < input.Count(); k++)
                        {
                            Console.Write(input[k]);
                        }
                        Console.Write(", ");
                        for (int k = 0; k < Stack.Count; k++)
                        {
                            Symbol[] tmp = new Symbol[Stack.Count]; //Цикл просто выводит стек на экран!
                            Stack.CopyTo(tmp, 0);
                            Console.Write(tmp[k]);
                        }
                        Console.Write(", ");
                        Console.Write(OutputConfigure);
                        Console.WriteLine(")\n");
                    }
                    else
                    {
                        // ERROR
                        Console.Write("      ОШИБКА! Не существует правила в клетке [");
                        Console.Write(currStackSymbol);
                        Console.Write(",");
                        Console.Write(currInputSymbol);
                        Console.WriteLine("]!\n");
                        return false;
                    }
                }
            } while (Stack.Peek() != "EoS"); // вершина стека не равна концу входной последовательности

            if (i != input.Count) // распознанный символ не равен концу входной последовательности
            {
                // ERROR
                return false;
            }

            return true;
        }

        public List<string> Tolist(List<Symbol> X)
        {
            List<string> Y = new List<string> { };
            foreach (var x in X)
                Y.Add(x.symbol);
            return Y;
        }

        public List<Production> getRules(Symbol noTermSymbol)
        {
            List<Production> result = new List<Production>();
            for (int i = 0; i < G.P.Count; ++i)
            {
                Production currRule = (Production)G.P[i];
                if (currRule.LHS.symbol == noTermSymbol)
                {
                    result.Add(currRule);
                }
            }
            return result;
        }
    }
}