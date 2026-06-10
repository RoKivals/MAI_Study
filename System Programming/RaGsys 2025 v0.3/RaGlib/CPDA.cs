// ============================================================================
// CPDA.cs — Конфигурируемый магазинный автомат (Configurable Pushdown Automaton)
// ============================================================================
// Данный класс реализует расширяемый магазинный автомат, у которого:
//   • состояние, вход, стек и набор правил задаются пользователем;
//   • логика переходов определяется делегатом `Move`;
//   • автомат может выполнять поиск по пространству конфигураций;
//   • правила переходов хранятся в Delta (унаследовано от Automate).
//
// CPDA — это удобная основа для построения интерпретаторов, анализаторов,
// синтаксических машин и любых моделей, основанных на магазинной автоматике.
// ============================================================================

using System;
using System.Collections.Generic;
using RaGlib.Core;

namespace RaGlib
{
    using Configurations = Queue<(Symbol q, int i, List<Symbol> pdl)>;

    public class CPDA<R> : Automate
    {
        /* Делегат, задающий поведение автомата:
         * configs — очередь конфигураций (позволяет добавлять новые);
         * config  — текущая конфигурация (состояние, позиция во входе, стек);
         * chain   — исходная входная строка;
         * Возвращает true, если автомат может завершить работу успешно.
         */
        public delegate bool Move(Configurations configs,
                                  (Symbol q, int i, List<Symbol> pdl) config,
                                  string chain);

        protected List<Symbol> _gammaSet; // Алфавит стека
        protected Symbol _z0;             // Начальный символ стека
        protected Move _behaviour;        // Пользовательское поведение автомата

        public CPDA(List<Symbol> q_set,
                    List<Symbol> sigma_set,
                    List<Symbol> gamma_set,
                    Symbol q0,
                    Symbol z0,
                    List<Symbol> f_set)
        {
            // --- Инициализация базового автомата ---
            this.Q = q_set;      // Множество состояний
            this.Q0 = q0;         // Начальное состояние
            this.Sigma = sigma_set;  // Входной алфавит
            this.F = f_set;      // Допускающие состояния
            this.Delta = new List<R>();

            // --- Инициализация CPDA-части ---
            _gammaSet = gamma_set;  // Алфавит стека
            _z0 = z0;         // Начальный символ стека
            _behaviour = null;       // Пока поведение не задано
        }

        // Устанавливает пользовательскую функцию переходов
        public void SetUp(Move new_behaviour)
        {
            if (new_behaviour == null)
                throw new ArgumentNullException("new_behaviour");

            _behaviour = new_behaviour;
        }

        // Выполняет автомат на заданной строке
        public bool Execute(string chain)
        {
            if (chain == null)
                throw new ArgumentNullException("chain");

            if (_behaviour == null)
                throw new ArgumentException(
                  "Pushdown automaton is not configured! Use `SetUp` before execution."
                );

            // Добавляем ε как маркер конца строки
            chain += "ε";

            // Создаём очередь конфигураций и добавляем начальную
            Configurations configs = new Configurations();
            configs.Enqueue((this.Q0, 0, new List<Symbol>() { _z0 }));

            // Перебираем конфигурации, пока автомат ещё может развиваться
            while (configs.Count > 0)
            {
                var config = configs.Dequeue();

                if (_behaviour(configs, config, chain))
                    return true;
            }

            return false;
        }

        // Добавляет правило перехода автомата
        public virtual void AddRule(R rule)
        {
            this.Delta.Add(rule);
        }

        // Вывод всех правил (если R поддерживает Debug())
        public void Debug()
        {
            foreach (var rule in this.Delta)
                rule.Debug();
        }
    }
}
