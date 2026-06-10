// ============================================================================
// ATScheme.cs — Атрибутная трансляционная схема (СУ-схема)
// ============================================================================
// Реализация СУ-схемы (атрибутной трансляционной схемы), которая обеспечивает:
//   • сопоставление входной и выходной грамматики,
//   • построение дерева вывода по последовательности правил,
//   • вычисление атрибутов в дереве,
//   • преобразование дерева согласно выходной грамматике,
//   • перераспределение узлов и выполнение функций атрибутов.
//
// СУ-схема используется для построения трансляторов, преобразователей деревьев,
// компиляторов и систем, основанных на атрибутных грамматиках.
// ============================================================================


using System;
using System.Collections.Generic;
using System.Linq;
using RaGlib.Core;
using RaGlib.Grammars;

namespace RaGlib { 
public class ATScheme
{
    public class ATSProduction : AttrProduction
    {
        public List<Symbol> TranslateProd;
        public List<AttrFunction> TranslateF;

        public ATSProduction(Symbol LHS, List<Symbol> Chain1, List<Symbol> Chain2, List<AttrFunction> F1, List<AttrFunction> F2) : base(LHS, Chain1, F1)
        {
            TranslateF = F2;
            TranslateProd = Chain2;
        }
    }

    public List<Symbol> V; // Множество нетерминалов
    public List<Symbol> OP; // Множество операционных символов
    public List<Symbol> Sigma; // Множество терминалов у входной грамматики
    public List<Symbol> Delta; // Множество терминалов у выходной грамматики
    public List<ATSProduction> Productions;
    public Symbol S0;

    public ATGrammar GrammarInput; // Входная грамматика
    public ATGrammar GrammarOutput; // Выходная грамматика

    public ATParseTree InputTree; // Дерево вывода
    public ATScheme(List<Symbol> _V, List<Symbol> _OP, List<Symbol> _Sigma, List<Symbol> _Delta, Symbol S)
    {
        V = _V;
        OP = _OP;
        Sigma = _Sigma;
        Delta = _Delta;
        S0 = S;
        Productions = new List<ATSProduction>();
    }

    // Создание схемы по входной и выходной грамматикам
    public ATScheme(ATGrammar _GrammarInput, ATGrammar _GrammarOutput)
    {
        if(_GrammarInput.V != _GrammarOutput.V || _GrammarInput.OP != _GrammarOutput.OP
                || _GrammarInput.Rules.Count != _GrammarOutput.Rules.Count || _GrammarInput.S0 != _GrammarOutput.S0)
        {
                throw new Exception("На вход СУ-схеме подали некорректные грамматики");
        }
        GrammarInput = _GrammarInput;
        GrammarOutput = _GrammarOutput;
        V = GrammarInput.V;
        Sigma = GrammarInput.T;
        Delta = GrammarOutput.T;
        OP = new List<Symbol>();
        foreach(var sym in _GrammarInput.OP)
        {
            if(sym.attr == null)
            {
                OP.Add(new Symbol(sym.symbol));        
            }
            else
            {
                OP.Add(new Symbol(sym.symbol, sym.attr));    
            }
        }

        S0 = GrammarInput.S0;
        Productions = new List<ATSProduction>();

        for(int i = 0;i < GrammarInput.Rules.Count; ++i) 
        {
            if(GrammarInput.Rules[i].LHS != GrammarOutput.Rules[i].LHS)
            {
                throw new Exception("На вход СУ-схеме подали некорректные грамматики");
            }
            AddNewProduction(GrammarInput.Rules[i].LHS, GrammarInput.Rules[i].RHS, GrammarOutput.Rules[i].RHS,
                    GrammarInput.Rules[i].F, GrammarOutput.Rules[i].F);
        }
    }

    public void BuildInputTree()
    {
        InputTree = new ATParseTree(S0, false);
              
            var input = Console.ReadLine();
            var words = input.Split(' ');
            foreach (var word in words)
            {
                var pos = Convert.ToInt32(word) - 1;
                if (pos < 0 || pos >= GrammarInput.Rules.Count)   //words.count (92 строка)
                {
                    throw new Exception($"Правило {pos + 1} либо отрицательно, либо превышает общее количество правил");
                }
                var node = InputTree.FindFirst(InputTree, GrammarInput.Rules.ElementAt(pos).LHS.symbol);
                node.Id = pos;
                node.RealAttrs = GrammarInput.Rules.ElementAt(pos).LHS.attr;
                node.F = GrammarInput.Rules.ElementAt(pos).F;
                foreach (var sym in GrammarInput.Rules.ElementAt(pos).RHS)
                {
                    node.Add(sym, OP.Contains(sym));
                }
            }
            var leaves = InputTree.CountLeaves(InputTree);
            Console.WriteLine($"Введите {leaves} начальных атрибутов:");
            input = Console.ReadLine();
            words = input.Split(' ');

            var initialValues = new List<Symbol>();
            foreach (var i in words)
            {
                initialValues.Add(i);
            }

            InputTree.pos = 0;
            InputTree.vals = initialValues;
            InputTree.SetAttr(InputTree);
    }

    public void TreeTransform()
    {
        Transform(InputTree);
        InputTree.pos = 0;
        InputTree.SetAttr(InputTree);
    }
//Алгоритм 6.1  Преобразование деревьев при помощи СУ-схемы
public void Transform(ATParseTree node)
{
    node.F = Productions.ElementAt(node.Id).TranslateF;
    node.RealAttrs = Productions.ElementAt(node.Id).LHS.attr;
    int len = (node.Symbol.attr == null ? 0 : node.Symbol.attr.Count);
    node.Calculated = new List<int>(new int[len]);

    //Проверка на лист
    if (!node.Next.Any())
    {
        return;
    }

    //Удаление всех терминальных листьев
    var T = new List<ATParseTree>();
    foreach (var child in node.Next)
    {
        var ch = child.Symbol;
        if (Sigma.Contains(ch.ToString()))
        {
            T.Add(child);
        }
    }
    foreach (var del in T)
    {
        node.Next.Remove(del);
    }

    //Перестановка соответствующих потомков и добавление новых терминалов
    var Betta = Productions.ElementAt(node.Id).TranslateProd;
    var NewChildren = new LinkedList<ATParseTree>();

    foreach (var item in Betta)
    {
        if (Delta.Contains(item.symbol))
        {
            NewChildren.AddLast(new ATParseTree(item, false));
            continue;
        }
        foreach (var element in node.Next)
        {
            if (element.Symbol.symbol == item.symbol)
            {
                NewChildren.AddLast(element);
                node.Next.Remove(element);
                break;
            }
        }
    }

    node.Next = NewChildren;

    //Рекурсивно спускаемся и продолжаем алгоритм в потомках
    foreach (var child in node.Next)
    {
        Transform(child);
    }
}

    public void PrintTree()
    {
        InputTree.Print();
        Console.Write("Крона дерева: ");
        InputTree.PrintCrown(InputTree);
        Console.WriteLine();
    }

    public void AddNewProduction(Symbol LHS, List<Symbol> Chain1, List<Symbol> Chain2, List<AttrFunction> F1, List<AttrFunction> F2)
    {
        Productions.Add(new ATSProduction(LHS, Chain1, Chain2, F1, F2));
    }
}

    public class ATParseTree
    {
        public Symbol Symbol; ///< Символ
        public LinkedList<ATParseTree> Next; ///< Потомки
        public List<AttrFunction> F;
        public List<int> Calculated;
        public List<Symbol> RealAttrs;
        public int Id; ///< Номер правила
        public bool isOP;
        public int pos;
        public List<Symbol> vals;

        public ATParseTree(Symbol S, bool op)
        {
            Symbol = S;
            Next = new LinkedList<ATParseTree>();
            isOP = op;
            int len = (S.attr == null ? 0 : S.attr.Count);
            Calculated = new List<int>(new int[len]);
        }

        public void Add(ATParseTree T)
        {
            Next.AddLast(T);
        }
        public void Add(Symbol symbol, bool op)
        {
            Next.AddLast(new ATParseTree(symbol, op));
        }
        private string PrintList(List<int> l)
        {
            if (l == null)
            {
                return "";
            }
            string res = "";
            for (int i = 0; i < l.Count; ++i)
            {
                res += l.ElementAt(i);
                if (i + 1 < l.Count)
                {
                    res += ", ";
                }
            }

            return res;
        }
        public void Print(int d = 0)
        {
            Console.WriteLine("{0," + (d * 10).ToString() + "}", (isOP ? "{" : "") + Symbol + (Symbol.attr != null && Symbol.attr.Count > 0 ? "_" : "") + PrintList(Calculated) + (this.isOP ? "}" : ""));
            foreach (ATParseTree child in Enumerable.Reverse(Next))
            {
                child.Print(d + 1);
            }
        }
        public ATParseTree FindFirst(ATParseTree node, string need)
        {
            if (!node.Next.Any() && (node.Symbol == need || node.Symbol.ToString().Split(' ')[0] == need))
            {
                return node;
            }

            foreach (var child in node.Next)
            {
                var result = FindFirst(child, need);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public int CountLeaves(ATParseTree node)
        {
            if (node.Next.Count == 0)
            {
                return (node.Symbol.attr == null ? 0 : node.Symbol.attr.Count);
            }

            int res = 0;
            foreach (var child in node.Next)
            {
                res += CountLeaves(child);
            }

            return res;
        }

        public void PrintCrown(ATParseTree node)
        {
            if (!node.Next.Any() && !node.isOP)
            {
                Console.Write(node.Symbol);
            }

            foreach (var child in node.Next)
            {
                PrintCrown(child);
            }
        }

        public void SetAttr(ATParseTree node)
        {
            if (node.Next.Count == 0)
            {
                if (node.Symbol.attr == null)
                {
                    return;
                }

                for (int i = 0; i < node.Symbol.attr.Count; ++i)
                {
                    node.Calculated[i] = Int32.Parse(vals.ElementAt(pos).ToString());
                    ++pos;
                }
                return;
            }

            foreach (var child in node.Next)
            {
                SetAttr(child);
            }

            if (node.Symbol.attr == null)
            {
                return;
            }

            for (int i = 0; i < node.Symbol.attr.Count; ++i)
            {
                foreach (var it in node.F)
                {
                    Symbol sym = it.LH[0];
                    if (sym == node.RealAttrs[i])
                    {
                        node.Calculated[i] = -1;
                        for (int CurrentP = 0; CurrentP < it.RH.Count; CurrentP += 2)
                        {

                            foreach (var child in node.Next)
                            {
                                if (child.Symbol.attr == null) continue;
                                bool ok = false;
                                for (int j = 0; j < child.Symbol.attr.Count; ++j)
                                {
                                    if (it.RH[CurrentP] == child.Symbol.attr[j])
                                    {
                                        if (CurrentP == 0) node.Calculated[i] = child.Calculated[j];
                                        else
                                        {
                                            if (it.RH[CurrentP - 1] == "+")
                                            {
                                                node.Calculated[i] += child.Calculated[j];
                                            }
                                            else if (it.RH[CurrentP - 1] == "-")
                                            {
                                                node.Calculated[i] -= child.Calculated[j];
                                            }
                                            else if (it.RH[CurrentP - 1] == "*")
                                            {
                                                node.Calculated[i] *= child.Calculated[j];
                                            }
                                            else if (it.RH[CurrentP - 1] == "/")
                                            {
                                                node.Calculated[i] /= child.Calculated[j];
                                            }
                                        }
                                        ok = true;
                                        break;
                                    }
                                }
                                if (ok)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return;
        }

    }

}