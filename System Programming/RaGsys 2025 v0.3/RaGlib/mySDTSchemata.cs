using System;
using System.Collections.Generic;
using RaGlib.Core;

namespace RaGlib
{
    public class SDTProduction : Production
    {
        public List<Symbol> TranslateProd;

        public SDTProduction(Symbol S0, List<Symbol> RHSin, List<Symbol> RHSout) : base(S0, RHSin)
        {
            TranslateProd = RHSout;
        }
    }

    public class MySDTSchemata
    {
        public List<Symbol> V;          // нетерминалы
        public List<Symbol> Sigma;      // входной алфавит
        public List<Symbol> Delta;      // выходной алфавит
        public List<SDTProduction> Productions;
        public Symbol S0;               // начальный символ

        public MySDTSchemata(List<Symbol> NTS, List<Symbol> IPTS, List<Symbol> OPTS, Symbol FS)
        {
            V = NTS;
            Sigma = IPTS;
            Delta = OPTS;
            if (!V.Contains(FS))
                throw new Exception($"Начальный символ {FS} не принадлежит множеству нетерминалов");
            this.S0 = FS;
            Productions = new List<SDTProduction>();
        }

        public void AddRule(Symbol NotTerminal, List<Symbol> Chain1, List<Symbol> Chain2)
        {
            if (!V.Contains(NotTerminal))
                throw new Exception($"В правиле {NotTerminal} --- ({Utility.convert(Chain1)}, {Utility.convert(Chain2)}) нетерминал {NotTerminal} не принадлежит множеству нетерминалов");

            foreach (Symbol symbol in Chain1)
            {
                // Проверяем: либо нетерминал (возможно с индексом, например A_1), либо терминал из Sigma
                string baseName = symbol.symbol.Split('_')[0];
                if (!V.Contains(new Symbol(baseName)) && !Sigma.Contains(symbol))
                    throw new Exception($"В правиле {NotTerminal} --- ({Utility.convert(Chain1)}, {Utility.convert(Chain2)}) символ {symbol} не принадлежит множеству нетерминалов или входному алфавиту");
            }

            foreach (Symbol symbol in Chain2)
            {
                string baseName = symbol.symbol.Split('_')[0];
                if (!V.Contains(new Symbol(baseName)) && !Delta.Contains(symbol))
                    throw new Exception($"В правиле {NotTerminal} --- ({Utility.convert(Chain1)}, {Utility.convert(Chain2)}) символ {symbol} не принадлежит множеству нетерминалов или выходному алфавиту");
            }

            Productions.Add(new SDTProduction(NotTerminal, Chain1, Chain2));
        }

        public void DebugSDTS()
        {
            Console.WriteLine("\n");
            Console.WriteLine($"Нетерминальные символы: {Utility.convert(V)}");
            Console.WriteLine($"Входные символы: {Utility.convert(Sigma)}");
            Console.WriteLine($"Выходные символы: {Utility.convert(Delta)}");
            Console.WriteLine($"Начальный символ: {S0}");
            Console.WriteLine();

            for (int i = 0; i < Productions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Productions[i].LHS} --- ({Utility.convert(Productions[i].RHS)}, {Utility.convert(Productions[i].TranslateProd)})");
            }
        }

        public void TranslateByRules(List<int> RulesNumbers)
        {
            var Chain1 = new List<Symbol>() { S0 };
            var Chain2 = new List<Symbol>() { S0 };
            Console.WriteLine("\nВывод в данной СУ-схеме по правилам: {0}", Utility.convertInt(RulesNumbers));
            Console.Write("({0}, {1})", Utility.convert(Chain1), Utility.convert(Chain2));

            foreach (var ruleNum in RulesNumbers)
            {
                int realRuleIndex = ruleNum - 1;

                if (realRuleIndex >= Productions.Count || realRuleIndex < 0)
                    throw new Exception($"Не существует правила с номером {ruleNum}. Вывод не завершён");

                var prod = Productions[realRuleIndex];
                var NT = prod.LHS;

                // Находим позицию нетерминала в цепочках (учитывая возможные индексы вида A_1)
                int index1 = -1, index2 = -1;
                Symbol actualNT = null;

                for (int i = 0; i < Chain1.Count; i++)
                {
                    var s1 = Chain1[i];
                    string baseName1 = s1.symbol.Split('_')[0];
                    if (baseName1 == NT.symbol)
                    {
                        index1 = i;
                        actualNT = s1;
                        break;
                    }
                }

                for (int i = 0; i < Chain2.Count; i++)
                {
                    var s2 = Chain2[i];
                    string baseName2 = s2.symbol.Split('_')[0];
                    if (baseName2 == NT.symbol && s2.symbol == actualNT?.symbol)
                    {
                        index2 = i;
                        break;
                    }
                }

                if (index1 == -1 || index2 == -1)
                    throw new Exception($"Не удалось найти нетерминал {NT} в текущих цепочках для применения правила {ruleNum}");

                // Применяем правило к Chain1 и Chain2
                var rhs1 = new List<Symbol>(prod.RHS);   // входная часть правила
                var rhs2 = new List<Symbol>(prod.TranslateProd); // выходная часть

                // Обновляем индексированные нетерминалы во входной цепочке и синхронизируем в выходной
                Chain1.RemoveAt(index1);
                for (int i = rhs1.Count - 1; i >= 0; i--)
                {
                    var sym = rhs1[i];
                    if (!Sigma.Contains(sym))
                    { // это нетерминал
                        string baseSym = sym.symbol.Split('_')[0];
                        int maxIndex = 0;
                        foreach (var existing in Chain1)
                        {
                            if (existing.symbol.StartsWith(baseSym + "_"))
                            {
                                if (int.TryParse(existing.symbol.Split('_')[1], out int idx) && idx > maxIndex)
                                    maxIndex = idx;
                            }
                        }
                        string newIndex = (maxIndex + 1).ToString();
                        Symbol newSym = new Symbol($"{baseSym}_{newIndex}");
                        rhs1[i] = newSym;
                        // Обновляем соответствующий символ и в rhs2
                        for (int j = 0; j < rhs2.Count; j++)
                        {
                            if (rhs2[j].symbol == sym.symbol)
                            {
                                rhs2[j] = newSym;
                            }
                        }
                    }
                }
                Chain1.InsertRange(index1, rhs1);

                Chain2.RemoveAt(index2);
                Chain2.InsertRange(index2, rhs2);

                Console.Write($" => {ruleNum}\n({Utility.convert(Chain1)}, {Utility.convert(Chain2)})");
            }
            Console.WriteLine();
        }

        private bool RecursiveDescentParser(List<Symbol> currentChain, List<Symbol> targetChain, int depth, ref List<int> derivation)
        {
            if (depth < 0) return false;

            if (Utility.IsSameArrayList(currentChain, targetChain))
            {
                return true;
            }

            // Ищем первый нетерминал в currentChain
            Symbol ntToReplace = null;
            int ntIndex = -1;
            for (int i = 0; i < currentChain.Count; i++)
            {
                var sym = currentChain[i];
                string baseName = sym.symbol.Split('_')[0];
                if (V.Contains(new Symbol(baseName)))
                {
                    ntToReplace = new Symbol(baseName);
                    ntIndex = i;
                    break;
                }
            }

            if (ntToReplace == null)
            {
                // Нетерминалов больше нет, но цепочка не совпадает — неудача
                return false;
            }

            // Перебираем правила для этого нетерминала
            for (int i = 0; i < Productions.Count; i++)
            {
                var rule = Productions[i];
                if (rule.LHS.symbol == ntToReplace.symbol)
                {
                    // Применяем правило: заменяем ntToReplace на RHS
                    var newChain = new List<Symbol>(currentChain);
                    newChain.RemoveAt(ntIndex);

                    var rhsCopy = new List<Symbol>();
                    foreach (var sym in rule.RHS)
                    {
                        rhsCopy.Add(new Symbol(sym.symbol.Split('_')[0])); // без индексов на этом этапе
                    }
                    newChain.InsertRange(ntIndex, rhsCopy);

                    if (RecursiveDescentParser(newChain, targetChain, depth - 1, ref derivation))
                    {
                        derivation.Add(i + 1); // номер правила (с 1)
                        return true;
                    }
                }
            }

            return false;
        }

        public List<int> Derivation(List<Symbol> targetChain)
        {
            var derivation = new List<int>();
            // Перебираем глубину от 1 до, скажем, 10 (настройте по необходимости)
            for (int maxDepth = 1; maxDepth <= 10; maxDepth++)
            {
                var trial = new List<int>();
                if (RecursiveDescentParser(new List<Symbol> { S0 }, targetChain, maxDepth, ref trial))
                {
                    trial.Reverse();
                    return trial;
                }
            }
            throw new Exception($"Не удалось вывести цепочку {Utility.convert(targetChain)} за разумное число шагов");
        }

        public void Translate(List<Symbol> inputChain)
        {
            Console.WriteLine();
            Console.WriteLine($"Входная строка: {Utility.convert(inputChain)}");

            try
            {
                var derivation = Derivation(inputChain);
                Console.Write("Её вывод во входной грамматике: ");
                foreach (int rule in derivation)
                {
                    Console.Write($"{rule} ");
                }
                Console.WriteLine();

                TranslateByRules(derivation);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при трансляции: {ex.Message}");
            }
        }
    }
}