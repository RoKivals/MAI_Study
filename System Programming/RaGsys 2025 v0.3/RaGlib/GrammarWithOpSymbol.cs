// ============================================================================
// GrammarWithOpSymbol.cs — Грамматика с операционными символами
// ============================================================================
// Данный класс расширяет обычную контекстно-свободную грамматику поддержкой
// *операционных символов*.  
// Операционные символы — это элементы правых частей правил, которые вызывают
// дополнительные действия при преобразовании грамматики в конечный автомат
// (например: запись в файл, вызов функции, разбиение строки и др.).
//
// Метод Transform() строит конечный автомат расширенного типа
// (FSAutomateWithOpSymbols), в котором переходы могут включать выполнение
// операционных операций.
//
// Основная идея:
// - Нетерминалы → состояния автомата.
// - Терминалы → входные символы.
// - Операционные символы → специальные маркеры, вызывающие действие.
// - Добавляется финальное состояние qf.
// ============================================================================

using System.Collections.Generic;
using RaGlib.Core;

namespace RaGlib
{
    public class GrammarWithOpSymbol : Grammar
    {
        public List<Symbol_Operation> Top = null;

        public GrammarWithOpSymbol(List<Symbol> T,
                                   List<Symbol_Operation> Top,
                                   List<Symbol> V,
                                   string S0)
        {
            this.T = T;
            this.V = V;
            this.Top = Top;
            this.S0 = new Symbol(S0);
            this.P = new List<Production>();
        }

        /// <summary>
        /// Преобразует грамматику в конечный автомат,
        /// поддерживающий операционные символы.
        /// </summary>
        public FSAutomateWithOpSymbols Transform()
        {
            var Q = this.V;
            Q.Add(new Symbol("qf"));

            var q0 = this.S0;
            var F = new List<Symbol>();

            // --------------------------------------------------------------------
            // Формируем множество конечных состояний
            // --------------------------------------------------------------------
            foreach (var p in this.P)
            {
                // Если существует правило S0 → ε, то финальными являются S0 и qf
                if (p.LHS.symbol.Contains("S0") && p.RHS.Contains(new Symbol("e")))
                {
                    F = new List<Symbol> { p.LHS, new Symbol("qf") };
                    break;
                }
                // Иначе финальное только qf
                else if (p.LHS.symbol.Contains("S0"))
                {
                    F = new List<Symbol> { new Symbol("qf") };
                    break;
                }
            }

            // --------------------------------------------------------------------
            // Конструируем автомат
            // --------------------------------------------------------------------
            FSAutomateWithOpSymbols KA =
                new FSAutomateWithOpSymbols(Q, this.T, this.Top, F, q0.symbol);

            bool addEpsilonTransition = true;

            foreach (var p in this.P)
            {
                // Если существует правило S0 → ε, добавляем (S0, ε, qf)
                if (addEpsilonTransition &&
                    p.LHS.symbol.Contains("S0") &&
                    p.RHS.Contains(new Symbol("e")))
                {
                    KA.AddRule(p.LHS.symbol, "e", "qf");
                    addEpsilonTransition = false;
                }

                // Обход всех терминалов
                foreach (var t in this.T)
                {
                    // Если справа присутствует операционный символ
                    if (OpSymbolReturn(p.RHS) != null)
                    {
                        // Вариант с операционной операцией
                        if (p.RHS.Contains(t) &&
                            this.Top.Contains(new Symbol_Operation(p.RHS[1].ToString())))
                        {
                            KA.AddRule(p.LHS.symbol, t.symbol,
                                       OpSymbol(p.RHS), "qf");
                        }
                        // Вариант без операционной операции
                        else if (p.RHS.Contains(t))
                        {
                            KA.AddRule(p.LHS.symbol, t.symbol,
                                       OpSymbol(p.RHS),
                                       NoTerminal(p.RHS));
                        }
                    }
                    else
                    {
                        // ---------- Обычный случай ----------
                        // Терминал → qf
                        if (p.RHS.Contains(t) && NoTermReturn(p.RHS) == null)
                            KA.AddRule(p.LHS.symbol, t.symbol, "qf");

                        // Терминал + нетерминал → переход к нетерминалу
                        else if (p.RHS.Contains(t) && NoTermReturn(p.RHS) != null)
                            KA.AddRule(p.LHS.symbol, t.symbol, NoTerminal(p.RHS));
                    }
                }
            }

            return KA;
        }

        // ------------------------------------------------------------------------
        // Вспомогательные методы для анализа RHS правил грамматики
        // ------------------------------------------------------------------------

        private List<Symbol> NoTermReturn(List<Symbol> array)
        {
            var nonTerms = new List<Symbol>();
            foreach (var s in array)
                if (this.V.Contains(s))
                    nonTerms.Add(s);

            return nonTerms.Count == 0 ? null : nonTerms;
        }

        private string NoTerminal(List<Symbol> array)
        {
            foreach (var s in array)
                if (this.V.Contains(s))
                    return s.symbol;

            return "";
        }

        private string OpSymbol(List<Symbol> array)
        {
            foreach (var s in array)
                if (this.Top.Contains(new Symbol_Operation(s.symbol)))
                    return s.symbol;

            return "";
        }

        private List<Symbol_Operation> OpSymbolReturn(List<Symbol> array)
        {
            var ops = new List<Symbol_Operation>();

            foreach (var s in array)
                if (this.Top.Contains(new Symbol_Operation(s.symbol)))
                    ops.Add(new Symbol_Operation(s.symbol));

            return ops.Count == 0 ? null : ops;
        }

        private List<Symbol> TermReturn(List<Symbol> A)
        {
            var terms = new List<Symbol>();
            foreach (var t in this.T)
                if (A.Contains(t))
                    terms.Add(t);

            return terms.Count == 0 ? null : terms;
        }
    }
}
