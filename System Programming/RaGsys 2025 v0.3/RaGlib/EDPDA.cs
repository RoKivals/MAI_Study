// ============================================================================
// EDPDA.cs — Расширенный детерминированный магазинный автомат (Extended DPDA)
// ============================================================================
// Класс реализует магазинный автомат, в котором левый контекст правила может
// включать несколько символов стека (LHSZX), а не только вершину.
//
// Особенности EDPDA:
//   • поддержка расширенных переходов вида δ(q, a, Z1Z2…Zn) → (q', γ);
//   • возможность работы с несколькими символами стека одновременно;
//   • пошаговое подробное логирование процесса работы автомата;
//   • обработка ε-переходов и последовательных ε-шагов;
//   • очередь конфигураций обеспечивает поиск в ширину.
//
// Класс используется для моделирования сложных грамматик,
// трансляторов и обобщённых синтаксических автоматов.
// ============================================================================

using System;
using System.Linq;
using System.Collections.Generic;

using RaGlib.Core;
using RaGlib.Automata;

namespace RaGlib
{

    public class EDPDA : CPDA<DeltaQSigmaGammaEx>
    {

        // ------------------------------------------------------------------------
        // Основной конструктор: задаёт поведение расширенного DPDA
        // ------------------------------------------------------------------------
        public EDPDA(List<Symbol> q_set,
                     List<Symbol> sigma_set,
                     List<Symbol> gamma_set,
                     Symbol q0, Symbol z0,
                     List<Symbol> f_set)
        : base(q_set, sigma_set, gamma_set, q0, z0, f_set)
        {

            _behaviour = (configs, config, chain) => {

                DeltaQSigmaGammaEx delta;

                // ===== Логирование текущей конфигурации =====
                Console.Error.Write("Current config: ({0}, {1}, ",
                   config.q, config.i < chain.Length ? chain.Remove(0, config.i) : "ε");

                foreach (var s in config.pdl)
                    Console.Error.Write(s);

                Console.Error.WriteLine(config.pdl.Count > 0 ? ")" : "ε)");

                // Шаг 1: поиск перехода по входному символу и содержимому стека
                delta = findDelta(chain[config.i].ToString(), config.pdl);

                if (delta == null)
                    return false;

                Console.Error.Write("Step 1: ");
                delta.Debug();


                // =====================================================================
                // Если правило НЕ является ε-переходом
                // =====================================================================
                if (!delta.LHSS.symbol.Equals("ε"))
                {

                    for (; config.i < chain.Length;)
                    {

                        if (chain[config.i].Equals("ε"))
                            return true;

                        Console.Error.WriteLine($"Step 2: {chain[config.i]} {delta.LHSS.symbol}");

                        // Добавление правой части в стек, если она не ε
                        if (!delta.RHSZ.First().symbol.Equals("ε"))
                            config.pdl.AddRange(delta.RHSZ);

                        ++config.i;
                        break;
                    }
                }

                // =====================================================================
                // Если правило является ε-переходом
                // =====================================================================
                else
                {

                    int size = delta.LHSZX.Count;
                    int index = config.pdl.Count - size;

                    Console.Error.WriteLine("Step 3: " + chain[config.i]);

                    // Снимаем LHSZX со стека, если это не ε
                    if (!delta.LHSZ.symbol.Equals("ε"))
                        config.pdl.RemoveRange(index, size);

                    // Добавляем RHSZ
                    if (!delta.RHSZ.First().symbol.Equals("ε"))
                        config.pdl.AddRange(delta.RHSZ);

                    Console.Error.Write("Step 4: ");
                    delta.Debug();


                    // Поиск следующего ε-правила
                    DeltaQSigmaGammaEx next_delta = findDelta(config.pdl);
                    List<Symbol> next_stack = new List<Symbol>(config.pdl);

                    if (next_delta != null)
                    {

                        size = next_delta.LHSZX.Count;
                        index = next_stack.Count - size;

                        // Если ε на RHS — снимаем символы
                        if (next_delta.RHSZ.First().symbol.Equals("ε"))
                        {

                            next_stack.RemoveRange(index, size);

                            Console.Error.WriteLine("Step 5: " + config.pdl.Last().symbol);

                            // Если пришли в допускающее состояние
                            if (next_delta.RHSQ.First().symbol.Equals(this.F.First().symbol))
                                return true;
                        }
                        else
                        {
                            next_stack.AddRange(next_delta.RHSZ);
                        }
                    }
                }

                Console.Error.WriteLine();

                // Добавляем новую конфигурацию для продолжения работы
                configs.Enqueue((delta.RHSQ.First(), config.i, config.pdl));
                return false;
            };
        }


        // ------------------------------------------------------------------------
        // Добавление перехода вручную
        // ------------------------------------------------------------------------
        public void addDeltaRule(Symbol LHSQ,
                                 Symbol LHSS,
                                 List<Symbol> LHSZ,
                                 List<Symbol> RHSQ,
                                 List<Symbol> RHSZ)
        {
            this.Delta.Add(new DeltaQSigmaGammaEx(LHSQ, LHSS, LHSZ, RHSQ, RHSZ));
        }


        // ------------------------------------------------------------------------
        // Поиск перехода δ(q, a, Z1…Zn)
        // ------------------------------------------------------------------------
        private DeltaQSigmaGammaEx findDelta(Symbol sigma, List<Symbol> z)
        {

            foreach (var delta in this.Delta)
            {

                int size = delta.LHSZX.Count;
                int index = z.Count - size;

                if (index >= 0)
                {

                    for (int i = 0; i < size; ++i)
                        if (z[index + i] != delta.LHSZX[i])
                            goto skip;

                    return delta;
                }

            skip: continue;
            }

            // Поиск ε-перехода по символу
            foreach (var delta in this.Delta)
                if (delta.LHSS.symbol.Equals(sigma.symbol) &&
                    delta.LHSZ.symbol.Equals("ε"))
                    return delta;

            return null;
        }


        // ------------------------------------------------------------------------
        // Поиск перехода по стеку: δ(q, ε, Z1…Zn)
        // ------------------------------------------------------------------------
        private DeltaQSigmaGammaEx findDelta(List<Symbol> z)
        {

            foreach (var delta in this.Delta)
            {

                int size = delta.LHSZX.Count;
                int index = z.Count - size;

                if (index >= 0)
                {

                    for (int i = 0; i < size; ++i)
                        if (z[index + i] != delta.LHSZX[i])
                            goto skip;

                    return delta;
                }

            skip: continue;
            }

            return null;
        }
    }
}
