// ============================================================================
// DPDA.cs — Детерминированный магазинный автомат (Deterministic PDA)
// ============================================================================
// Класс DPDA наследует CPDA и задаёт конкретное поведение детерминированного
// магазинного автомата. Логика переходов задаётся внутри конструктора через
// делегат `_behaviour`.
//
// Особенности реализации DPDA:
//   • поддерживает детерминированные переходы по вершине стека и входному символу;
//   • выполняет подробное логирование работы автомата;
//   • позволяет строить DPDA по КС-грамматике (второй конструктор);
//   • может выполнять шаги со считыванием или ε-переходы;
//   • конфигурации обрабатываются через очередь (поиск в ширину).
//
// Класс используется для моделирования синтаксического анализа,
// интерпретаторов, учебных автоматов и инструментов трансляции.
// ============================================================================

using System;
using System.Linq;
using System.Collections.Generic;

using RaGlib.Core;
using RaGlib.Automata;

namespace RaGlib
{

    public class DPDA : CPDA<DeltaQSigmaGamma>
    {

        // ------------------------------------------------------------------------
        // Основной конструктор DPDA: определяет поведение автомата через делегат
        // ------------------------------------------------------------------------
        public DPDA(List<Symbol> q_set,
                   List<Symbol> sigma_set,
                   List<Symbol> gamma_set,
                   Symbol q0, Symbol z0,
                   List<Symbol> f_set)
        : base(q_set, sigma_set, gamma_set, q0, z0, f_set)
        {

            _behaviour = (configs, config, chain) => {

                // ========== Лог текущей конфигурации ==========
                Console.Error.Write("Current config: ({0}, {1}, ",
                    config.q, config.i < chain.Length ? chain.Remove(0, config.i) : "ε");

                foreach (var s in config.pdl)
                    Console.Error.Write(s);

                Console.Error.WriteLine(config.pdl.Count > 0 ? ")" : "ε)");

                // Шаг 1 — ищем правило по вершине стека
                DeltaQSigmaGamma delta = findDelta(config.pdl.Last());

                if (delta == null)
                    return false;

                Console.Error.Write("Step 1: ");
                delta.Debug();


                // ==============================================================
                // Если правило НЕ является ε-переходом — выполняем чтение входа
                // ==============================================================
                if (!delta.LHSS.symbol.Equals("ε"))
                {

                    for (; config.i < chain.Length;)
                    {

                        if (chain[config.i].Equals("ε"))
                            return true;

                        Console.Error.WriteLine($"Step 2: {chain[config.i]} {delta.LHSS.symbol}");

                        // Если входной символ соответствует правилу перехода
                        if (chain[config.i].ToString().Equals(delta.LHSS.symbol))
                        {

                            Console.Error.WriteLine($"Step 3: {chain[config.i]}");

                            // Если нужно удалить символ из стека
                            if (delta.RHSZ.First().symbol.Equals("ε"))
                                config.pdl.RemoveAt(config.pdl.Count - 1);
                            else
                                config.pdl.Add(delta.RHSZ.First());
                        }
                        else
                        {
                            // Пытаемся найти другое правило по входному символу
                            delta = findDelta(chain[config.i].ToString(), config.pdl.Last());

                            if (delta == null)
                                return false;

                            Console.Error.Write("Step 4: ");
                            delta.Debug();

                            if (delta.RHSZ.First().symbol.Equals("ε"))
                            {

                                Console.Error.WriteLine("Step 5: " + config.pdl.Last().symbol);
                                config.pdl.RemoveAt(config.pdl.Count - 1);

                                // Если правило ведёт к допускающему состоянию
                                if (delta.RHSQ.First().symbol.Equals(this.F.First().symbol))
                                    return true;
                            }
                            else
                                config.pdl.Add(delta.RHSZ.First());
                        }

                        ++config.i;
                        break;
                    }
                }

                // ==============================================================
                // Если найденный переход — ε-переход
                // ==============================================================
                else
                {
                    config.pdl.RemoveAt(config.pdl.Count - 1);
                    foreach (var symbol in delta.RHSZ)
                        config.pdl.Add(symbol);
                }

                Console.Error.WriteLine();

                // Добавляем новую конфигурацию в очередь
                configs.Enqueue((delta.RHSQ.First(), config.i, config.pdl));

                return false;
            };
        }


        // ------------------------------------------------------------------------
        // Конструктор построения DPDA по КС-грамматике
        // ------------------------------------------------------------------------
        public DPDA(Grammar KCgrammar)
        : this(new List<Symbol>() { "q" },            // Q
               new List<Symbol>(KCgrammar.T),         // Sigma
               new List<Symbol>(KCgrammar.V),         // Gamma
               "q0", KCgrammar.S0,                    // начальное состояние и символ стека
               new List<Symbol>())
        {

            // Добавляем терминалы в стековый алфавит
            foreach (var t in KCgrammar.T)
                _gammaSet.Add(t);

            // TODO: реализация построения правил Delta по грамматике требует доработки
            DeltaQSigmaGamma delta = null;

            var rhsq = new List<Symbol>();
            var rhsz = new List<Symbol>();
            int i = 0;

            foreach (var v in KCgrammar.V)
            {

                foreach (var p in KCgrammar.P)
                {

                    if (p.LHS.Equals(v))
                    {
                        var tempList = new List<Symbol>();
                        var rhs = new List<Symbol>(p.RHS);
                        rhs.Reverse();

                        foreach (var simbol in rhs)
                        {
                            i++;
                            tempList.Add(simbol);
                            rhsz.Concat(tempList);
                            rhsq.Add("q" + i);
                            this.Q.Add(simbol);
                        }
                    }

                    delta = new DeltaQSigmaGamma("q" + i, "e", v.symbol, rhsq, rhsz);
                    Delta.Add(delta);
                }
            }

            i++;

            // Переходы для терминалов
            foreach (var t in KCgrammar.T)
            {
                delta = new DeltaQSigmaGamma("q" + i,
                                             t.symbol,
                                             t.symbol,
                                             new List<Symbol>() { "q" + i },
                                             new List<Symbol>() { "e" });
                Delta.Add(delta);
            }
        }


        // ------------------------------------------------------------------------
        // Добавление правила перехода вручную
        // ------------------------------------------------------------------------
        public void addDeltaRule(Symbol LHSQ, Symbol LHSS, Symbol LHSZ,
                                 List<Symbol> RHSQ, List<Symbol> RHSZ)
        {
            this.Delta.Add(new DeltaQSigmaGamma(LHSQ, LHSS, LHSZ, RHSQ, RHSZ));
        }


        // ------------------------------------------------------------------------
        // Поиск правила delta(q, a, z)
        // ------------------------------------------------------------------------
        private DeltaQSigmaGamma findDelta(Symbol sigma, Symbol z)
        {
            foreach (var delta in this.Delta)
                if (delta.LHSS.symbol.Equals(sigma.symbol) &&
                    delta.LHSZ.symbol.Equals(z.symbol))
                    return delta;

            return null;
        }

        // ------------------------------------------------------------------------
        // Поиск правила по символу в вершине стека
        // delta(q, ε, z)
        // ------------------------------------------------------------------------
        private DeltaQSigmaGamma findDelta(Symbol z)
        {
            foreach (var delta in this.Delta)
                if (delta.LHSZ.symbol.Equals(z.symbol))
                    return delta;

            return null;
        }
    }
}
