using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;

/// Синтаксически управляемая трансляция
namespace SDT {  /// Обычный символ грамматики
public class Symbol : ICloneable {
        public string Value; ///< Строковое значение/имя символа
        public Dictionary<string, object> Attributes = null; ///< Атрибуты символа. Доступны также через индексатор

        public Symbol(string value)
        {
            Value = value;
        }

        public Symbol(string value, Dictionary<string, object> attributes) : this(value)
        {
            AddAttributes(attributes);
        }

        public Symbol(Symbol other)
        {
            Symbol symbol = (Symbol)other.Clone();
            Value = symbol.Value;
            Attributes = symbol.Attributes;
        }

        /// Неявное преобразование строки в Symbol
        public static implicit operator Symbol(string str) => new Symbol(str);

        /// Неявное преобразование словаря в Symbol
        /**
         *  Словарь должен иметь запись "NAME", значение которой будет использовано как имя/значение символа
         */
        public static implicit operator Symbol(Dictionary<string, object> dict)
        {
            string name = (string)dict["NAME"];
            dict.Remove("NAME");
            return new Symbol(name, dict);
        }

        /// Неявное преобразование функции в OperationSymbol
        public static implicit operator Symbol(Types.Actions func) => new OperationSymbol(func);

        /// Доступ к атрибутам
        public object this[string name]
        {
            get { return Attributes[name]; }
            set { Attributes[name] = value; }
        }

        /// Клонирование словаря атрибутов
        public void AddAttributes(Dictionary<string, object> attributes)
        {
            if (attributes is null)
            {
                return;
            }
            Attributes = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> pair in attributes)
            {
                if (pair.Value is ICloneable obj)
                {
                    Attributes.Add(pair.Key, obj.Clone());
                }
                else
                {
                    Attributes.Add(pair.Key, pair.Value);
                }
            }
        }

        /// Равенсто. Требуется для Dictionary и HashSet
        public override bool Equals(object other)
        {   
            return (other is Symbol) && (Value == ((Symbol)other).Value);
        }

        /// Хеш-функция. Требуется для Dictionary и HashSet
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// Оператор взят из документации
        public static bool operator ==(Symbol a, Symbol b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Value == b.Value;
        }

        public static bool operator !=(Symbol a, Symbol b)
        {
            return !(a == b);
        }

        public override string ToString() => this != Epsilon ? Value : "e";

        /// Глубокая копия
        public virtual object Clone()
        {
            Symbol clone = new Symbol((string)Value.Clone());
            // Symbol clone = new Symbol(Value is null ? null : (string)Value.Clone());
            clone.AddAttributes(Attributes);
            return clone;
        }

        public static readonly Symbol Epsilon = new Symbol(""); ///< Пустой символ
        public static readonly Symbol Sentinel = new Symbol("$$"); ///< Cимвол конца строки / Символ дна стека
    }

    /// Операционный символ (Семантическое действие)
    public class OperationSymbol : Symbol
    {
        public new Types.Actions Value; ///< Функция семантического действия ???????????????????

        public string StringView = null; ///< Строковое представление функции. Необязательное поле
        public List<string> OutAttrs = null; ///< Атрибуты, значения которых изменяет функция. Необязательное поле
        public List<string> InAttrs = null; ///< Атрибуты, значения которых использует функция. Необязательное поле

        public OperationSymbol(Types.Actions value) : base(null)
        {
            Value = value;
        }


        public OperationSymbol(Types.Actions value, string stringView) : this(value)
        {
            StringView = stringView;
        }

        public OperationSymbol(Types.Actions value, string stringView, List<string> outAttrs, List<string> inAttrs) : this(value, stringView)
        {
            OutAttrs = outAttrs;
            InAttrs = inAttrs;
        }

        public OperationSymbol(OperationSymbol other) : this(((OperationSymbol)other.Clone()).Value) {}

        public override string ToString() => StringView ?? "{}";

        /// Поверхностная копия
        public override object Clone() => new OperationSymbol(Value, StringView, InAttrs, OutAttrs);
    }

    /// Правило синтаксически управляемой схемы трансляции
    public class Rule
    {
        public Symbol LeftNoTerm; ///< Левая часть продукции
        public List<Symbol> RightChain; ///< Правая часть продукции

        public Rule(Symbol leftNoTerm, List<Symbol> rightChain)
        {
            LeftNoTerm = leftNoTerm;
            RightChain = rightChain;
        }

        /// " -> " разделитель. e вместо эпсилон. {} вместо операционного символа
        public override string ToString()
        {
            string str = LeftNoTerm.Value + " -> ";
            foreach (Symbol symbol in RightChain)
            {
                if (symbol is OperationSymbol)
                {
                    str += "{}";
                }
                else if (symbol == Symbol.Epsilon)
                {
                    str += "e";
                }
                else
                {
                    str += symbol.Value;
                }
            }
            return str;
        }
    }

    /// Синтаксически управляемая ( САС ????????????????? не верно) схема трансляции
      public class Scheme {
        public Symbol S0; ///< Начальный символ
        public List<Symbol> T; ///< Терминальные символы
        public List<Symbol> V; ///< Нетерминальные символы
        public List<Rule> Prules; ///< Продукции

        private Dictionary<Symbol, HashSet<Symbol>> FirstSet;
        private Dictionary<Symbol, HashSet<Symbol>> FollowSet;

        public Scheme(List<Symbol> t, List<Symbol> v, Symbol s0)
        {
            T = t;
            V = v;
            S0 = s0;
            Prules = new List<Rule>();
            FirstSet = new Dictionary<Symbol, HashSet<Symbol>>();
            FollowSet = new Dictionary<Symbol, HashSet<Symbol>>();

            S0.AddAttributes(V.Find(x => x == S0).Attributes);
        }

        public Scheme(List<Symbol> t, List<Symbol> v, Symbol s0, List<Rule> prules) : this(t, v, s0)
        {
            foreach (Rule rule in prules)
            {
                AddRule(rule.LeftNoTerm, rule.RightChain);
            }
            ComputeFirstFollow();
        }

        public void ComputeFirstFollow()
        {
            ComputeFirstSets();
            ComputeFollowSets();
        }

        public void AddRule(Symbol leftNoTerm, List<Symbol> rightChain)
        {
            Rule rule = new Rule(leftNoTerm, rightChain);

            // Клонирование атрибутов для каждого символа
            if (rule.LeftNoTerm.Attributes is null)
            {
                rule.LeftNoTerm.AddAttributes(V.Find(x => x == rule.LeftNoTerm).Attributes);
            }
            foreach (Symbol s in rule.RightChain)
            {
                if (s is OperationSymbol || !(s.Attributes is null) || s == Symbol.Epsilon)
                {
                    continue;
                }

                if (V.Contains(s))
                {
                    s.AddAttributes(V.Find(x => x == s).Attributes);
                }
                else
                {
                    s.AddAttributes(T.Find(x => x == s).Attributes);
                }
            }
            Prules.Add(rule);
        }

        public string Execute() { return null; }

        /// Перенесен с небольшими изменениями из LLParser
        private void ComputeFirstSets()
        {
            FirstSet.Clear();
            foreach (Symbol term in T)
                FirstSet[term] = new HashSet<Symbol>() { term }; // FIRST[c] = {c}
            FirstSet[Symbol.Epsilon] = new HashSet<Symbol>() { Symbol.Epsilon }; // для единообразия
            foreach (Symbol noTerm in V)
                FirstSet[noTerm] = new HashSet<Symbol>(); // First[X] = empty set
            bool changes = true;
            while (changes)
            {
                changes = false;
                foreach (Rule rule in Prules)
                {
                    // Для каждого правила X-> Y0Y1…Yn
                    Symbol X = rule.LeftNoTerm;
                    foreach (Symbol Y in rule.RightChain)
                    {
                        if (Y is OperationSymbol)
                        {
                            continue;
                        }
                        foreach (Symbol curFirstSymb in FirstSet[Y])
                        {
                            if (FirstSet[X].Add(curFirstSymb)) // Добавить а в FirstSets[X]
                            {
                                changes = true;
                            }
                        }
                        if (!FirstSet[Y].Contains(Symbol.Epsilon))
                        {
                            break;
                        }
                    }
                }
            } // пока вносятся изменения
        }

        public HashSet<Symbol> First(Symbol X)
        {
            return FirstSet[X];
        }

        public HashSet<Symbol> First(List<Symbol> X)
        {
            HashSet<Symbol> result = new HashSet<Symbol>();
            foreach (Symbol Y in X)
            {
                if (Y is OperationSymbol)
                {
                    continue;
                }

                foreach (Symbol curFirstSymb in FirstSet[Y])
                {
                    result.Add(curFirstSymb);
                }
                if (!FirstSet[Y].Contains(Symbol.Epsilon))
                {
                    break;
                }
            }
            return result;
        }

        /// Перенесен с большими изменениями из LLParser
        private void ComputeFollowSets() {
            FollowSet.Clear();
            foreach (Symbol noTerm in V)
                FollowSet[noTerm] = new HashSet<Symbol>();
            FollowSet[S0].Add(Symbol.Sentinel);
            bool changes = true;
            while (changes)
            {
                changes = false;
                foreach (Rule rule in Prules)
                {
                    for (int curIndex = 0; curIndex < rule.RightChain.Count; ++curIndex)
                    {
                        Symbol curSymbol = rule.RightChain[curIndex];
                        if (T.Contains(curSymbol) || curSymbol is OperationSymbol || curSymbol == Symbol.Epsilon)
                        {
                            continue;
                        }

                        // Поиск следующего не операционного символа
                        int nextIndex = curIndex + 1;
                        while (nextIndex < rule.RightChain.Count && rule.RightChain[nextIndex] is OperationSymbol)
                        {
                            ++nextIndex;
                        }
                        Symbol nextSymbol;
                        if (nextIndex < rule.RightChain.Count)
                        {
                            nextSymbol = rule.RightChain[nextIndex];
                        }
                        else
                        {
                            nextSymbol = Symbol.Epsilon;
                        }

                        bool epsFound = false;
                        foreach (Symbol symbol in First(nextSymbol))
                        {
                            if (symbol != Symbol.Epsilon)
                            {
                                if (FollowSet[curSymbol].Add(symbol))
                                {
                                    changes = true;
                                }
                            }
                            else
                            {
                                epsFound = true;
                            }
                        }
                        if (epsFound)
                        {
                            foreach (Symbol symbol in FollowSet[rule.LeftNoTerm])
                            {
                                if (FollowSet[curSymbol].Add(symbol))
                                {
                                    changes = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        public HashSet<Symbol> Follow(Symbol X)
        {
            return FollowSet[X];
        }
    }

    /// Реализация L-атрибутного ?????? СУТ в процессе LL анализа
    public class LLTranslator {
        /// Запись синтеза. Используется в LL-трансляции.
        class SynthSymbol : Symbol  // ????????????????????????????
        {
            public string Name; ///< Имя символа
            public Dictionary<string, Symbol> Copies; ///< Копии других записей синтеза

            public SynthSymbol() : base(null)
            {
                Copies = new Dictionary<string, Symbol>();
            }

            public SynthSymbol(string name, Symbol symbol) : this()
            {
                Copies = new Dictionary<string, Symbol>();
                Name = name;
                Value = symbol.Value;
                Attributes = symbol.Attributes;
            }

            public SynthSymbol(string name, string value, Dictionary<string, object> attributes) : this()
            {
                Copies = new Dictionary<string, Symbol>();
                Name = name;
                Value = value;
                Attributes = attributes;
            }

            /// Добавление другой записи синтеза
            public void Add(SynthSymbol other)
            {
                Copies.Add(other.Name, other);
                foreach (KeyValuePair<string, Symbol> pair in other.Copies)
                {
                    Copies.Add(pair.Key, pair.Value);
                }
            }

            public override string ToString() => "s" + Name;
        }
    
        protected Scheme G; ///< АТ-грамматика
        protected Stack<Symbol> Stack; ///< Стек символов
        protected Dictionary<Symbol, Dictionary<Symbol, Rule>> Table; ///< Управляющая таблица. Table[нетерминал][терминал]

        public LLTranslator()
        {
            G = null;
            Stack = null;
            Table = null;
        }

        public LLTranslator(Scheme grammar)
        {
            G = grammar;
            G.ComputeFirstFollow();
            Table = new Dictionary<Symbol, Dictionary<Symbol, Rule>>();
            Stack = new Stack<Symbol>();
            Construct();
        }

        public void Construct()
        {
            // Построение управляющей таблицы
            foreach (Symbol noTermSymbol in G.V)
            {
                Table[noTermSymbol] = new Dictionary<Symbol, Rule>();
            }

            // Для каждого правила A -> alpha
            foreach (Rule rule in G.Prules)
            {
                // Для каждого a из First(alpha)
                foreach (Symbol firstSymbol in G.First(rule.RightChain))
                {
                    if (firstSymbol != Symbol.Epsilon)
                    {
                        // Добавлем правило в таблицу на пересечение A и a
                        Table[rule.LeftNoTerm][firstSymbol] = rule;
                    }
                    // Если в First(alpha) входит эпсилон
                    else
                    {
                        // Для каждого b из Follow(A)
                        foreach (Symbol followSymbol in G.Follow(rule.LeftNoTerm))
                        {
                            // Добавлем правило в таблицу на пересечении A и b
                            Table[rule.LeftNoTerm][followSymbol] = rule;
                        }
                    }
                }
            }
            // DebugMTable();
        }

        private static readonly Symbol EndOfRule = "~EOR~"; ///< Символ конца правила

        /// Анализ строки
        public bool Parse(List<Symbol> input)
        {
            input.Add(Symbol.Sentinel); // Символ окончания входной последовательности

            Stack.Clear();
            Stack.Push(Symbol.Sentinel); // Символ дна стека
            Stack.Push(new SynthSymbol(G.S0.Value, new Symbol(G.S0)));
            Stack.Push(G.S0);

            int i = 0;
            Symbol curInputSymbol = input[i];
            Symbol curStackSymbol;
            do
            {
                curStackSymbol = Stack.Peek();
                if (curStackSymbol is OperationSymbol op && op != null) // в вершине стека операционный символ
                {
                    Stack.Pop();
                    // Функции операционного символа передаются записи синтеза до конца правила
                    SynthSymbol symbols = new SynthSymbol();
                    Stack<Symbol> tmp = new Stack<Symbol>();
                    while (Stack.Peek() != EndOfRule)
                    {
                        if (Stack.Peek() is SynthSymbol)
                        {
                            symbols.Add((SynthSymbol)Stack.Peek());
                        }
                        tmp.Push(Stack.Pop());
                    }

                    while (tmp.Count > 0)
                    {
                        Stack.Push(tmp.Pop());
                    }

                    op.Value.Invoke(symbols.Copies);
                }
                else if (curStackSymbol is SynthSymbol synth && synth != null) // в вершине стека запись синтеза
                {
                    Stack.Pop();
                    // Запись синтеза копируется в запись синтеза ниже, в пределах данного правила
                    Stack<Symbol> tmp = new Stack<Symbol>();
                    while (Stack.Count > 0 && !(Stack.Peek() is SynthSymbol))
                    {
                        if (Stack.Peek() == EndOfRule)
                        {
                            break;
                        }
                        tmp.Push(Stack.Pop());
                    }

                    if (Stack.Count > 0 && Stack.Peek() is SynthSymbol newSynth && newSynth != null)
                    {
                        newSynth.Add(synth);
                    }

                    while (tmp.Count > 0)
                    {
                        Stack.Push(tmp.Pop());
                    }
                }
                else if (curStackSymbol == EndOfRule) // в вершине стека символ конца правила
                {
                    Stack.Pop();
                }
                else if (G.T.Contains(curStackSymbol)) // в вершине стека терминал
                {
                    if (curInputSymbol == curStackSymbol) // распознанный символ равен вершине стека
                    {
                        Stack.Pop();
                        // В записи синтеза нетерминала обновляются атрибуты
                        Stack.Peek().Attributes = curInputSymbol.Attributes;
                        ++i;
                        curInputSymbol = input[i];
                    }
                    else
                    {
                        // ERROR
                        return false;
                    }
                }
                else // в вершине стека нетерминал
                {
                    Rule rule;
                    if (Table[curStackSymbol].TryGetValue(curInputSymbol, out rule)) // в клетке[вершина стека, распознанный символ] таблицы разбора существует правило
                    {
                        Stack.Pop();

                        // Подсчет индексов символов
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

                        // Символы правила заносятся в обратном порядке в стек
                        SynthSymbol headSynth = new SynthSymbol(rule.LeftNoTerm.Value, rule.LeftNoTerm.Value, Stack.Peek().Attributes);
                        Stack.Push(EndOfRule); // Конец правила
                        Stack.Push(headSynth); // Локальная запись синтеза для заголовка
                        int j = rule.RightChain.Count - 1;
                        foreach (Symbol rightSymbol in Enumerable.Reverse(rule.RightChain))
                        {
                            if (rightSymbol == Symbol.Epsilon)
                            {
                                continue;
                            }

                            if (rightSymbol is OperationSymbol)
                            {
                                Stack.Push(rightSymbol);
                            }
                            else
                            {
                                Symbol copy = new Symbol(rightSymbol);
                                // Запись синтеза копии
                                Stack.Push(new SynthSymbol(copy.Value + (count[rightSymbol.Value] > 1 || copy == rule.LeftNoTerm ? indexes[j].ToString() : ""), copy));
                                // Копия символа
                                Stack.Push(copy);
                            }
                            --j;
                        }
                    }
                    else
                    {
                        // ERROR
                        return false;
                    }
                }
            } while (Stack.Peek() != Symbol.Sentinel); // вершина стека не равна дну стека

            if (curInputSymbol != Symbol.Sentinel) // распознанный символ не равен концу входной последовательности
            {
                // ERROR
                return false;
            }

            return true;
        }

        /// Печать управляющей таблицы
        public void DebugMTable()
        {
            int maxLenV = 0;
            foreach (Symbol s in G.V)
            {
                maxLenV = Math.Max(maxLenV, s.Value.Length);
            }
            int maxLenSymb = Math.Max(maxLenV, 2);
            foreach (Symbol s in G.T)
            {
                maxLenSymb = Math.Max(maxLenSymb, s.Value.Length);
            }
            int maxLenRule = 0;
            foreach (Rule rule in G.Prules)
            {
                maxLenRule = Math.Max(maxLenRule, rule.RightChain.Count);
            }
            maxLenRule = maxLenV + 4 + maxLenRule * maxLenSymb;

            Console.Write("{0,-" + maxLenV.ToString() + "} | ", " ");
            foreach (Symbol s in G.T)
            {
                Console.Write("{0,-" + maxLenRule.ToString() + "} | ", s.Value);
            }
            Console.WriteLine("{0,-" + maxLenRule.ToString() + "} | ", Symbol.Sentinel.Value);
            foreach (Symbol s in G.V)
            {
                Console.Write("{0,-" + maxLenV.ToString() + "} | ", s.Value);
                Rule rule;
                string str;
                foreach (Symbol s2 in G.T)
                {
                    if (Table[s].TryGetValue(s2, out rule))
                    {
                        str = rule.ToString();
                    }
                    else
                    {
                        str = " ";
                    }
                    Console.Write("{0,-" + maxLenRule.ToString() + "} | ", str);
                }
                if (Table[s].TryGetValue(Symbol.Sentinel, out rule))
                {
                    str = rule.ToString();
                }
                else
                {
                    str = " ";
                }
                Console.WriteLine("{0,-" + maxLenRule.ToString() + "} | ", str);
            }
        }
    }

    /// Сокращения для некоторых типов
    namespace Types {
        /// Функция семантического действия
        public delegate void Actions(Dictionary<string, Symbol> _);

        /// Словарь атрибутов
        public class Attrs : Dictionary<string, object> {}
    }

    /// "Таблица" со стандартными семантическими действиями
    static public class Actions {
        /// Печать something  консоль
        static public Types.Actions Print(object obj) =>
            (_) => Console.Write(obj.ToString());
    }
}
