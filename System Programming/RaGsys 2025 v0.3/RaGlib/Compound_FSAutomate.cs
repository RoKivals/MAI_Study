// ============================================================================
// Compound_FSAutomate.cs — Операции над конечными автоматами
// ============================================================================
// Набор функций для объединения и слияния конечных автоматов (FSAutomate).
// Реализованы операции:
//   • Merge2 — прямое слияние двух автоматов с учётом финальных состояний,
//   • Union2 — объединение (дизъюнкция) двух автоматов с добавлением нового старта,
//   • Merge — последовательное слияние массива автоматов,
//   • Union — объединение массива автоматов.
//
// Используется при построении больших автоматов, комбинировании лексеров,
// создании КА из подвыражений регулярных грамматик и т.п.
// ============================================================================

using System.Collections.Generic;
using RaGlib.Core;


namespace RaGlib
{
    public class Compound_FSAutomate
    {
        // Слияние двух конечных автоматов. 
        // Объединяет множества состояний, алфавит и финальные состояния.
        // Переносит переходы и добавляет дополнительные переходы
        // от финальных состояний первого автомата, если требуется.

        public static FSAutomate Merge2(FSAutomate a, FSAutomate b)
        {
            // Объединяем множества состояний
            var Q = new List<Symbol>();
            Q.AddRange(a.Q);
            Q.AddRange(b.Q);

            // Объединяем алфавит
            var E = new List<Symbol>(a.Sigma);
            foreach (var s in b.Sigma)
                if (!E.Contains(s)) E.Add(s);

            // Объединяем финальные состояния
            var F = new List<Symbol>();
            if (b.F.Contains(b.Q0))
                F.AddRange(a.F);
            F.AddRange(b.F);

            // Создаём новый автомат
            var merge = new FSAutomate(Q, E, F, a.Q0.ToString());

            // Копируем все правила обоих автоматов
            foreach (var d in a.Delta)
                merge.AddRule(d.LHSQ.symbol, d.LHSS.symbol, d.RHSQ[0].symbol);

            foreach (var d in b.Delta)
            {
                merge.AddRule(d.LHSQ.symbol, d.LHSS.symbol, d.RHSQ[0].symbol);

                // Если переход начинается в начальном состоянии b —
                // создаём соответствующие переходы из финальных состояний a.
                if (d.LHSQ.symbol == b.Q0.ToString())
                {
                    foreach (var f in a.F)
                        merge.AddRule(f.symbol, d.LHSS.symbol, d.RHSQ[0].symbol);
                }
            }

            return merge;
        }

        // Объединение двух автоматов (операция OR).
        // Создаётся новое стартовое состояние, которое ε-переходами
        // ведёт в стартовые состояния обоих автоматов.
        public static FSAutomate Union2(FSAutomate a, FSAutomate b)
        {
            var Q = new List<Symbol>() { new Symbol("S") };
            Q.AddRange(a.Q);
            Q.AddRange(b.Q);

            // Алфавит
            var E = new List<Symbol>(a.Sigma);
            foreach (var s in b.Sigma)
                if (!E.Contains(s)) E.Add(s);

            // Финальные состояния
            var F = new List<Symbol>();
            F.AddRange(a.F);
            F.AddRange(b.F);

            var union = new FSAutomate(Q, E, F, "S");

            // Добавляем ε-переходы из нового стартового состояния
            union.AddRule("S", "", a.Q0.symbol);
            union.AddRule("S", "", b.Q0.symbol);

            // Копируем правила
            foreach (var d in a.Delta)
                union.AddRule(d.LHSQ.symbol, d.LHSS.symbol, d.RHSQ[0].symbol);

            foreach (var d in b.Delta)
                union.AddRule(d.LHSQ.symbol, d.LHSS.symbol, d.RHSQ[0].symbol);

            return union;
        }

        // Слияние массива автоматов — последовательный вызов Merge2.
        public static FSAutomate Merge(FSAutomate[] automates)
        {
            var temp = automates[0];
            for (int i = 1; i < automates.Length; i++)
                temp = Merge2(temp, automates[i]);
            return temp;
        }

        /// <summary>
        /// Объединение массива автоматов — последовательный вызов Union2.
        /// </summary>
        public static FSAutomate Union(FSAutomate[] automates)
        {
            var temp = automates[0];
            for (int i = 1; i < automates.Length; i++)
                temp = Union2(temp, automates[i]);
            return temp;
        }
    }
}
