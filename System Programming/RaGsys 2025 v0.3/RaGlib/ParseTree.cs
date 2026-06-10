using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SDT
{
    /// Дерево разбора
    public class ParseTree : ICloneable
    {
        public Symbol Symbol; ///< Символ
        public LinkedList<ParseTree> Next; ///< Потомки
        public int Id = -1; ///< Номер правила

        public ParseTree(Symbol symbol)
        {
            Symbol = symbol;
            Next = new LinkedList<ParseTree>();
        }

        /// Добавление дочернего узла
        public void Add(ParseTree node) { Next.AddLast(node); }

        /// Добавление дочернего узла
        public void Add(Symbol symbol) { Next.AddLast(new ParseTree(symbol)); }

        /// Выполнение СУТ в процессе прямого обхода дерева
        public void Execute()
        {
            if (Next.Count == 0)
            {
                return;
            }

            // Подсчет индексов символов
            Dictionary<string, int> count = new Dictionary<string, int>();
            List<int> indexes = new List<int>();
            foreach (ParseTree node in Next)
            {
                if (node.Symbol is OperationSymbol || node.Symbol == Symbol.Epsilon)
                {
                    indexes.Add(0);
                    continue;
                }

                int index;
                count.TryGetValue(node.Symbol.Value, out index);
                count[node.Symbol.Value] = index + 1;
                indexes.Add(index + 1);
            }

            Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>() { [Symbol.Value] = Symbol };

            int i = 0;
            foreach (ParseTree node in Next)
            {
                if (node.Symbol is OperationSymbol || node.Symbol == Symbol.Epsilon)
                {
                    ++i;
                    continue;
                }

                symbols.Add(node.Symbol.Value + (count[node.Symbol.Value] > 1 || node.Symbol == Symbol ? indexes[i].ToString() : ""), node.Symbol);
                ++i;
            }

            foreach (ParseTree child in Next)
            {
                if (child.Symbol is OperationSymbol op && op != null)
                {
                    op.Value.Invoke(symbols);
                }
                else
                {
                    child.Execute();
                }
            }
        }

        /// Глубокая копия
        public object Clone()
        {
            ParseTree clone = new ParseTree((Symbol)Symbol.Clone());
            foreach (ParseTree child in Next)
            {
                clone.Add((ParseTree)child.Clone());
            }
            return clone;
        }

        /// Печать дерева в консоль
        public void Print(int d = 0)
        {
            Console.WriteLine("{0," + (d * 4).ToString() + "}", this.Symbol is OperationSymbol ? "{}" : Symbol);
            foreach (ParseTree child in Enumerable.Reverse(Next))
            {
                child.Print(d + 1);
            }
        }

        /// Генерация файла для GraphViz
        /**
         *  Получение изображения: `dot parse_tree.dot -Tpng > parse_tree.png`
         */
        public void PrintToFile(string filenName, bool printAttrs)
        {
            StreamWriter f = new StreamWriter(filenName);
            f.WriteLine("graph ParseTree {");
            f.WriteLine("graph [splines=line]");
            int id = 0;
            PrintToFile(f, ref id, printAttrs);
            f.WriteLine("}");
            f.Close();
        }

        /// Генерация файла для GraphViz
        private void PrintToFile(StreamWriter f, ref int id, bool printAttrs)
        {
            int parent_id = id;

            if (this.Symbol is OperationSymbol) {
                f.WriteLine("\tOperSymbol_{1} [label=\"{0}\", shape=rectangle]", Symbol.ToString().Replace("\"", "'"), parent_id);
            }

            else if (printAttrs)
            {
                string attrs = "";
                if (Symbol.Attributes != null)
                {
                    foreach (KeyValuePair<string, object> kvp in Symbol.Attributes)
                    {
                        attrs += kvp.Key.ToString().Replace("\"", "'") + " = " + kvp.Value.ToString().Replace("\"", "'") + "|";
                    }
                }
                if (attrs != "")
                {    // need 9.0 sas 
                    //attrs = "|{" + attrs[..^1] + "}";  // numbers[^1] is the same with numbers[length-1]
                }
                f.WriteLine("\t\"{0}_{1}\" [label=\"<name> {0} {2}\", shape=Mrecord]", Symbol.ToString().Replace("\"", "'"), parent_id, attrs);
            }
            
            else
            {
                f.WriteLine("\t\"{0}_{1}\" [label=\"{0}\", shape=circle]", Symbol.ToString().Replace("\"", "'"), parent_id);
            }

            ++id;
            foreach (ParseTree child in Next)
            {
                int child_id = id;
                child.PrintToFile(f, ref id, printAttrs);

                if (child.Symbol is OperationSymbol)
                {
                    f.WriteLine("\t\"{0}_{1}\"{3} -- OperSymbol_{2};", Symbol.ToString().Replace("\"", "'"), parent_id, child_id, printAttrs ? ":name" : "");
                }

                else if (printAttrs)
                {
                    f.WriteLine("\t\"{0}_{2}\":name -- \"{1}_{3}\":name;", Symbol.ToString().Replace("\"", "'"), child.Symbol.ToString().Replace("\"", "'"), parent_id, child_id);
                }

                else
                {
                    f.WriteLine("\t\"{0}_{2}\" -- \"{1}_{3}\";", Symbol.ToString().Replace("\"", "'"), child.Symbol.ToString().Replace("\"", "'"), parent_id, child_id);
                }
            }
        }
    }

    /// Получение дерева разбора в ходе выполнения соответствующей СУТ
    /**
     * **Важно:** Символы грамматики не должны содержать атрибут "node"
     */
    public class ParseTreeTranslator : LLTranslator
    {
        private Scheme OriginalGrammar; ///< Оригинальная грамматика
        private ParseTree Root = null; ///< Корень дерева

        private readonly Symbol StartNoTerm = "~S~"; ///< Стартовый символ пополненной грамматики

        /// Генератор операционных символов составления дерева
        private Types.Actions AttachNodes(string parent, List<string> childs, int id) =>
            new Types.Actions((S) => 
                {
                    ((ParseTree)S[parent]["node"]).Id = id;
                    foreach (string child in childs)
                        ((ParseTree)S[parent]["node"]).Add((ParseTree)S[child]["node"]);
                }
            );

        public ParseTreeTranslator(Scheme grammar)
        {
            // Составление транслирующей грамматики для построения дерева
            // Все символы клонируются
            List<Symbol> V = new List<Symbol>();
            foreach (Symbol noTerm in grammar.V)
            {
                Symbol newNoTerm = new Symbol(noTerm);
                // Добавляется новый атрибут
                if (newNoTerm.Attributes == null) newNoTerm.Attributes = new Types.Attrs();
                newNoTerm.Attributes["node"] = new ParseTree(noTerm);
                V.Add(newNoTerm);
            }
            List<Symbol> T = new List<Symbol>();
            foreach (Symbol term in grammar.T)
            {
                Symbol newTerm = new Symbol(term);
                // Добавляется новый атрибут
                if (newTerm.Attributes == null) newTerm.Attributes = new Types.Attrs();
                newTerm.Attributes["node"] = new ParseTree(term);
                T.Add(newTerm);
            }
            Scheme treeGrammar = new Scheme(T, V, grammar.S0.Value);

            // Составление правил новой грамматики
            int ruleNumber = 0;
            foreach (Rule rule in grammar.Prules)
            {
                Symbol LeftNoTerm = new Symbol(rule.LeftNoTerm.Value);
                List<Symbol> RightChain = new List<Symbol>();

                // Подсчет индексов для составления операционных символов
                Dictionary<string, int> count = new Dictionary<string, int>();
                List<int> indexes = new List<int>();
                foreach (Symbol s in rule.RightChain)
                {
                    if (s is OperationSymbol || s == Symbol.Epsilon)
                    {
                        indexes.Add(0);
                        continue;
                    }

                    int index;
                    count.TryGetValue(s.Value, out index);
                    count[s.Value] = index + 1;
                    indexes.Add(index + 1);
                }

                // Составление правила
                List<string> childs = new List<string>();
                int symbNumber = 0;
                foreach (Symbol symbol in rule.RightChain)
                {
                    // Операционные символы пропускаются
                    if (symbol is OperationSymbol)
                    {
                        ++symbNumber;
                        continue;
                    }

                    RightChain.Add(new Symbol(symbol.Value));

                    // Эпсилон не учитывается операционным символом
                    if (symbol == Symbol.Epsilon)
                    {
                        ++symbNumber;
                        continue;
                    }

                    childs.Add(symbol.Value + (count[symbol.Value] > 1 || symbol == rule.LeftNoTerm ? indexes[symbNumber].ToString() : ""));
                    ++symbNumber;
                }

                RightChain.Add(AttachNodes(rule.LeftNoTerm.Value, childs, ruleNumber)); // Вставка операционного символа составления дерева
                treeGrammar.AddRule(LeftNoTerm, RightChain);
                ++ruleNumber;
            }

            // Пополнение грамматики для возможности получить результат
            treeGrammar.V.Add(StartNoTerm);
            string oldS0 = treeGrammar.S0.Value;
            treeGrammar.AddRule(StartNoTerm, new List<Symbol>() { treeGrammar.S0, new Types.Actions((S) => Root = (ParseTree)S[oldS0]["node"]) });
            treeGrammar.S0 = new Symbol(StartNoTerm);

            OriginalGrammar = grammar;
            G = treeGrammar;
            G.ComputeFirstFollow();
            Table = new Dictionary<Symbol, Dictionary<Symbol, Rule>>();
            Stack = new Stack<Symbol>();
            Construct();
            // DebugMTable();
        }

        public new ParseTree Parse(List<Symbol> input)
        {
            foreach (Symbol symbol in input)
            {
                // Добавляется новый атрибут
                if (symbol.Attributes == null) symbol.Attributes = new Types.Attrs();
                symbol["node"] = new ParseTree(symbol);
            }

            if (!base.Parse(input))
            {
                return null;
            }

            // Доработка дерева
            Stack<ParseTree> nodes = new Stack<ParseTree>();
            nodes.Push(Root);
            while (nodes.Count > 0)
            {
                ParseTree node = nodes.Pop();
                node.Next.AddFirst(new ParseTree(null)); // Фиктивный элемент
                LinkedListNode<ParseTree> iter = node.Next.First;
                foreach (Symbol symbol in OriginalGrammar.Prules[node.Id].RightChain)
                {
                    if (iter.Value.Id >= 0)
                    {
                        nodes.Push(iter.Value);
                    }

                    // Удаляем ненужный атрибут node
                    if (iter.Value.Symbol != null && iter.Value.Symbol.Attributes != null)
                        iter.Value.Symbol.Attributes.Remove("node");
                    
                    // Добавляем оригинальные операционные символы и эпсилон
                    if (symbol is OperationSymbol || symbol == Symbol.Epsilon)
                    {
                        node.Next.AddAfter(iter, new ParseTree(symbol));
                    }
                    iter = iter.Next;
                }

                if (iter.Value.Id >= 0)
                {
                    nodes.Push(iter.Value);
                }
                if (iter.Value.Symbol.Attributes != null) iter.Value.Symbol.Attributes.Remove("node");
                node.Next.RemoveFirst(); // Удаляем фиктивный элемент

            }
            
            return Root;
        }
    }
}