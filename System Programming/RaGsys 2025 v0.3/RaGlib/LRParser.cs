using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Data;
using System.Text;

namespace RaGlib {
    /// Канонический LR(1)
    /**
     *  **Важно**:
     *  - Начальным нетерминалом может быть только S.
     *  - Символы грамматики могут состоять только из одного символа.
     *  - Использует собственные представления для грамматики.
     *  - Пересчитывает все составляющие при каждом вызове Execute.
     */
    public class CanonicalLRParser
    {

        protected ArrayList Grammar = new ArrayList(); ///< правила грамматики
        protected string Terminals; ///< список терминалов
        protected string NonTerminals; ///< список нетерминалов

        public CanonicalLRParser() {}

        protected struct Tablekey
        {
            public int I;
            public char J;
            public Tablekey(int i, char j) { I = i; J = j; }
        }

        public void Execute()
        {
            Console.WriteLine("\nИсходная ");
            Info();
            RemoveEpsilonRules();
            Console.WriteLine("\nПосле удаления е-продукций");

            Grammar.Add("П S"); //дополнить грамматику правилом П -> S
            NonTerminals += "П";
            Terminals += "$";

            Console.WriteLine("\nПравила: \n ");
            for (IEnumerator rule = Grammar.GetEnumerator(); rule.MoveNext();)
                Console.WriteLine(rule.Current);

            Console.WriteLine("Терминалы : " + Terminals);
            Console.WriteLine("Нетерминалы: " + NonTerminals);
            Console.WriteLine("-----");

            // генерация LR(1) таблицы

            ComputeFirstSets(); // вычислить множества FIRST

            Console.WriteLine("Вычислены множества FIRST для символов грамматики и строк \n ");

            string Symbols = NonTerminals;
            for (int i = 0; i < Symbols.Length; i++)
            { //для каждого символа грамматики X
                char X = Symbols[i];
                Console.WriteLine("First( " + X + " ): " + First(X));
            }
            Console.WriteLine();
            for (IEnumerator rule = Grammar.GetEnumerator(); rule.MoveNext();)
            {
                string str = ((string)rule.Current).Substring(2);
                Console.WriteLine("First( " + str + " ): " + First(str));
            }

            ArrayList[] CArray = CreateCArray(); // создать последовательность С
            Console.WriteLine("Cоздана последовательность С: \n ");
            for (int i = 0; i < CArray.Length; i++)
            {
                Console.WriteLine("I" + i + DebugArrayList(CArray[i]));
            }

            Hashtable ACTION = CreateActionTable(CArray); // создать ACTION таблицу
            if (ACTION == null)
            {
                Console.WriteLine("Грамматика не является LR(1)");
                Console.ReadLine();
                return;
            }
            Hashtable GOTO = CreateGotoTable(CArray); // создать GOTO таблицу
            // распечатать содержимое ACTION и GOTO таблиц:
            Console.WriteLine("\nСоздана ACTION таблица \n ");

            for (IDictionaryEnumerator c = ACTION.GetEnumerator(); c.MoveNext();)
                Console.WriteLine("ACTION[" + ((Tablekey)c.Key).I + ", " + ((Tablekey)c.Key).J + "] = " + c.Value);
            Console.WriteLine("\nСоздана GOTO таблица \n ");

            for (IDictionaryEnumerator c = GOTO.GetEnumerator(); c.MoveNext();)
                Console.WriteLine("GOTO[" + ((Tablekey)c.Key).I + ", " + ((Tablekey)c.Key).J + "] = " + c.Value);


            //синтакический анализ
            string answer = "y";
            while (answer[0] == 'y')
            {
                string input;
                Console.WriteLine("Введите строку: ");
                input = Console.In.ReadLine() + "$"; //считать входную строку
                Console.WriteLine("\nВведена строка: " + input + "\n");
                Console.WriteLine("\nПроцесс вывода: \n ");
                if (input.Equals("$"))
                { //случай пустой строки
                    Console.WriteLine(AcceptEmptyString ? "Строка допущена" : "Строка отвергнута");
                    Console.ReadLine();
                    continue; // return;
                }
                Stack stack = new Stack(); //Стек автомата
                stack.Push("0"); //поместить стартовое
                //нулевое состояние
                try
                {
                    for (;;)
                    {
                        int s = Convert.ToInt32((string)stack.Peek());
                        //вершина стека
                        char a = input[0]; //входной симол
                        string action = (string)ACTION[new Tablekey(s, a)];
                        //элемент
                        // ACTION -таблицы
                        if (action[0] == 's')
                        { // shift
                            stack.Push(a.ToString()); //поместить в стек а
                            stack.Push(action.Substring(2));
                            //поместить в стек s'
                            input = input.Substring(1);
                            //перейти к следующему символу строки
                        }
                        else if (action[0] == 'r')
                        { // reduce
                            // rule[1] = A, rule[2] = alpha
                            string[] rule = action.Split(' ');
                            //удалить 2 * Length(alpha) элементов стека
                            for (int i = 0; i < 2 * rule[2].Length; i++)
                                stack.Pop();
                            //вершина стека
                            int state = Convert.ToInt32((string)stack.Peek());
                            //поместить в стек А и GOTO[state, A]
                            stack.Push(rule[1]);
                            stack.Push((GOTO[new Tablekey(state, rule[1][0])]).ToString());

                            //вывести правило
                            Console.WriteLine(rule[1] + "->" + rule[2]);
                        }
                        else if (action[0] == 'a') // accept
                            break;
                    }
                    Console.WriteLine("Строка допущена");
                }
                catch (Exception)
                {
                    Console.WriteLine("Строка отвергнута");
                }
                Console.WriteLine("\n Продолжить? (y or n) \n");
                answer = Console.ReadLine();
            }
        }

        public void ReadGrammar()
        {
            Terminals = "";
            NonTerminals = "";
            Grammar.Clear();
            string s;
            Hashtable term = new Hashtable(); //  временная таблица терминалов
            Hashtable nonterm = new Hashtable(); //  и нетерминалов
            Console.WriteLine("\nВведите продукции: \n ");
            while ((s = Console.In.ReadLine()) != "")
            { //считывание правил
                Grammar.Add(s); //добавитьть правило в грамматику
                for (int i = 0; i < s.Length; i++)
                    //  анализ элементов правила
                    if (s[i] != ' ')
                    {
                        //  если текущий символ - терминал, еще не добавленный в term
                        if (s[i] == s.ToLower()[i] && !term.ContainsKey(s[i]))
                            term.Add(s[i], null);
                        if (s[i] != s.ToLower()[i] && !nonterm.ContainsKey(s[i]))
                            nonterm.Add(s[i], null);
                    }
            }
            //  переписываем терминалы и нетерминалы в строки Terminals и NonTerminals
            for (IDictionaryEnumerator c = term.GetEnumerator(); c.MoveNext();)
                Terminals += (char)c.Key;
            for (IDictionaryEnumerator c = nonterm.GetEnumerator(); c.MoveNext();)
                NonTerminals += (char)c.Key;
        }

        protected string DebugArrayList(ArrayList arraylist)
        {
            string arraylist_str = " { ";
            for (int i = 0; i < arraylist.Count; i++)
            {
                if (i == 0)
                    arraylist_str = arraylist_str + arraylist[i].ToString();
                else
                    arraylist_str = arraylist_str + "; " + arraylist[i].ToString();
            }
            arraylist_str = arraylist_str + " } ";
            return arraylist_str;
        }

        protected void Info()
        {
            Console.WriteLine("КС - грамматика : " + " \nАлфавит нетерминальных символов: " + NonTerminals + " \nАлфавит терминальных символов: : " + Terminals + " \nПравила : \n" + DebugArrayList(Grammar));
            Console.ReadLine();
        }

        /// список найденных комбинаций
        protected ArrayList combinations = new ArrayList();
        protected void GenerateCombinations(int depth, string s)
        {
            if (depth == 0)
                combinations.Add(s);
            else
            {
                GenerateCombinations(depth - 1, "0" + s);
                GenerateCombinations(depth - 1, "1" + s);
            }
        }

        ///  создает список правил, в которых вычеркнут один или более символов А в правой части
        protected ArrayList GenerateRulesWithout(char A)
        {
            ArrayList result = new ArrayList(); //  итоговый список
            //цикл по правилам
            for (IEnumerator rule = Grammar.GetEnumerator(); rule.MoveNext();)
            {
                string current = (string)rule.Current; //  текущее правило,
                string rhs = current.Substring(2); //  его правая часть,
                string[] rhs_split = rhs.Split(A); //  отдельные сегменты rhs, разделенные А
                int counter;
                if (rhs.IndexOf(A) != -1)
                { //если правая часть содержит А
                    counter = 0; //подсчитывает количество вхождений А
                    for (int i = 0; i < rhs.Length; i++)
                        if (rhs[i] == A)
                            counter++;
                    combinations.Clear();
                    GenerateCombinations(counter, ""); //генерация комбинаций
                    for (IEnumerator element = combinations.GetEnumerator(); element.MoveNext();)
                        if (((string)element.Current).IndexOf('1') != -1)
                        {
                            //  если текущая комбинация содержит хоть один вычеркиваемый символ (т.е. единицу)
                            string combination = (string)element.Current;
                            string this_rhs = rhs_split[0];
                            //  если текущий символ комвинации - единица,
                            //  то вычеркиваем А(просто соединяем сегменты правой части правила),
                            //  иначе вставляем дополнительный символ А)
                            //
                            for (int i = 0; i < combination.Length; i++)
                                this_rhs += (combination[i] == '0' ? A.ToString() : "") + rhs_split[i + 1];
                            result.Add(current[0] + " " + this_rhs);
                        }
                } // end if
            } // end for
            return result;
        }

        protected bool AcceptEmptyString; ///< допускать ли пустую строку
        protected void RemoveEpsilonRules()
        { /// удаление е-правил
            AcceptEmptyString = false; // флаг принадлежности пустой строки языку
            bool EpsilonRulesExist;
            do
            {
                EpsilonRulesExist = false;
                for (IEnumerator rule = Grammar.GetEnumerator(); rule.MoveNext();)
                    if (((string)rule.Current)[2] == 'e')
                    { // нашли эпсилон-правило
                        // принимаем пустую строку, если левая часть правила содержит стартовый символ
                        char A = ((string)rule.Current)[0];
                        if (A == 'S')
                        {
                            AcceptEmptyString = true;
                        }
                        Grammar.AddRange(GenerateRulesWithout(A));
                        Grammar.Remove(rule.Current); // удаляем e-правило
                        EpsilonRulesExist = true;
                        break;
                    }
            } while (EpsilonRulesExist); //  пока существуют эпсилон-правила
        }

        protected Hashtable FirstSets = new Hashtable(); ///< Набор множеств First
        protected void ComputeFirstSets()
        {
            for (int i = 0; i < Terminals.Length; i++)
                FirstSets[Terminals[i]] = Terminals[i].ToString(); // FIRST[c] = {c}*/
            for (int i = 0; i < NonTerminals.Length; i++)
                FirstSets[NonTerminals[i]] = ""; // First[x] = ""
            bool changes;
            do
            {
                changes = false;
                for (IEnumerator rule = Grammar.GetEnumerator(); rule.MoveNext();)
                {
                    // Для каждого правила X-> Y0Y1…Yn
                    char X = ((string)rule.Current)[0];
                    string Y = ((string)rule.Current).Substring(2);
                    for (int k = 0; k < Terminals.Length; k++)
                    {
                        char a = Terminals[k]; // для всех терминалов а
                        // а принадлежит First[Y0]
                        if (((string)FirstSets[Y[0]]).IndexOf(a) != -1)
                            if (((string)FirstSets[X]).IndexOf(a) == -1)
                            {
                                //Добавить а в FirstSets[X]
                                FirstSets[X] = (string)FirstSets[X] + a;
                                changes = true;
                            }
                    }
                }
            } while (changes); //  пока вносятся изменения
        }

        /// функции доступа ко множествам FIRST
        protected string First(char X) { return (string)FirstSets[X]; }

        protected string First(string X) { return First(X[0]); }

        protected ArrayList Closure(ArrayList I)
        {
            ArrayList result = new ArrayList();
            //добавляем все элементы I в замыкание
            for (IEnumerator item = I.GetEnumerator(); item.MoveNext();)
                result.Add(item.Current);
            bool changes;
            do
            {
                changes = false;
                //для каждого элемента R
                for (IEnumerator item = result.GetEnumerator(); item.MoveNext();)
                {
                    // A -> alpha.Bbeta,a
                    string itvalue = (string)item.Current;
                    int Bidx = itvalue.IndexOf('.') + 1;
                    char B = itvalue[Bidx]; //  B
                    if (NonTerminals.IndexOf(B) == -1) //  если после точки терминал, то ситуацию не обрабатываем
                        continue;
                    string beta = itvalue.Substring(Bidx + 1);
                    beta = beta.Substring(0, beta.Length - 2); //  beta
                    char a = itvalue[itvalue.Length - 1]; //  a
                    //для каждого правила B -> gamma
                    for (IEnumerator rule = Grammar.GetEnumerator(); rule.MoveNext();)
                        if (((string)rule.Current)[0] == B)
                        { //  B - >gramma
                            string gamma = ((string)rule.Current).Substring(2); //  gamma
                            string first_betaa = First(beta + a);
                            // для каждого b из FIRST(betaa)
                            for (int i = 0; i < first_betaa.Length; i++)
                            {
                                char b = first_betaa[i]; //  b
                                string newitem = B + " ." + gamma + "," + b;
                                // добавить элемент B -> .gamma,b
                                if (!result.Contains(newitem))
                                {
                                    result.Add(newitem);
                                    changes = true;
                                    goto breakloop;
                                }
                            }
                        } // for по правилам B -> gamma
                } // for по  ситуациям R
            breakloop:;
            } while (changes);
            return result;
        }

        /// Функция GoTo
        protected ArrayList GoTo(ArrayList I, char X)
        {
            ArrayList J = new ArrayList();
            // для всех ситуаций из I
            for (IEnumerator item = I.GetEnumerator(); item.MoveNext();)
            {
                string itvalue = (string)item.Current;
                string[] parts = itvalue.Split('.');
                if ((parts[1])[0] != X)
                    continue;
                //если ситуация имеет вид A alpha.Xbeta, a
                J.Add(parts[0] + X + "." + parts[1].Substring(1));
            }
            return Closure(J);
        }

        /// Процедура получения последовательности С
        protected bool SetsEqual(ArrayList lhs, ArrayList rhs)
        {
            string[] lhsArr = new string[lhs.Count];
            // преобразование списка
            lhs.CopyTo(lhsArr); // в массив
            Array.Sort(lhsArr); // и его сортировка
            string[] rhsArr = new string[rhs.Count];
            // то же для второго множества
            rhs.CopyTo(rhsArr);
            Array.Sort(rhsArr);
            if (lhsArr.Length != rhsArr.Length) // если размеры не равны множества точно не равны
                return false;
            for (int i = 0; i < rhsArr.Length; i++)
                if (!lhsArr[i].Equals(rhsArr[i])) // если же размеры равны, проверяем по элементам
                    return false;
            return true;
        }

        /// Функция SetsEqual() используется функцией Contatains,
        /// определяющей, является ли множество g элементом списка С
        protected bool Contains(ArrayList C, ArrayList g)
        {
            for (IEnumerator item = C.GetEnumerator(); item.MoveNext();)
                if (SetsEqual((ArrayList)item.Current, g))
                    return true;
            return false;
        }

        protected ArrayList[] CreateCArray()
        {
            string Symbols = Terminals + NonTerminals; // все символы грамматики
            ArrayList C = new ArrayList();
            Console.WriteLine("CreateCArray: ");
            // добавить элемент I0 = Closure ({"П .S,$"})
            C.Add(Closure(new ArrayList(new Object[] { "П .S,$" })));
            Console.WriteLine("I0 : " + DebugArrayList(Closure(new ArrayList(new Object[] { "П .S,$" }))));
            int counter = 0;
            bool modified;
            do
            {
                modified = false;
                for (int i = 0; i < Symbols.Length; i++)
                { //для каждого символа грамматики X
                    char X = Symbols[i];
                    Console.WriteLine("Для символа " + X);
                    // для каждого элемента последовательности С
                    for (IEnumerator item = C.GetEnumerator(); item.MoveNext();)
                    {
                        ArrayList g = GoTo((ArrayList)item.Current, X); // GoTo(Ii, X)
                        Console.WriteLine("GoTo( " + DebugArrayList((ArrayList)item.Current) + "," + X + "): \n" + DebugArrayList(g));
                        // если множество g непусто и еще не включено в С
                        if (g.Count != 0 && !Contains(C, g))
                        {
                            C.Add(g);
                            counter++;
                            Console.WriteLine("добавлено I" + counter + " : " + DebugArrayList(g));
                            modified = true;
                            break;
                        }
                    }
                }
            } while (modified); // пока вносятся изменения
            ArrayList[] CArray = new ArrayList[C.Count];
            // преобразование списка  в массив
            C.CopyTo(CArray);
            return CArray;
        }

        protected bool WriteActionTableValue(Hashtable ACTION, int I, char J, string action)
        {
            Tablekey Key = new Tablekey(I, J);
            if (ACTION.Contains(Key) && !ACTION[Key].Equals(action))
            {
                Console.WriteLine("не LR(1) грамматика");
                Console.ReadLine();
                return false;
            } // не LR(1) вид
            else
            {
                ACTION[Key] = action;
                return true;
            }
        }

        protected Hashtable CreateActionTable(ArrayList[] CArray)
        {
            Hashtable ACTION = new Hashtable();
            for (int i = 0; i < CArray.Length; i++)
            { // цикл по элементам C
                // Для каждой ситуации из множества CArray[i]
                for (IEnumerator item = CArray[i].GetEnumerator(); item.MoveNext();)
                {
                    string itvalue = (string)item.Current; // ситуация
                    char a = itvalue[itvalue.IndexOf('.') + 1]; // символ за точкой
                    // Если ситуация имеет вид "A alpha.abeta,b"
                    if (Terminals.IndexOf(a) != -1) // если a - терминал
                        for (int j = 0; j < CArray.Length; j++)
                            if (SetsEqual(GoTo(CArray[i], a), CArray[j]))
                            {
                                // существует элемент CArray[j], такой,
                                // что GoTo(CArray[i], a) == CArray[j]
                                // запись ACTION[i, a] = shift j
                                if (WriteActionTableValue(ACTION, i, a, "s " + j) == false)
                                    return null;
                                // грамматика не LR(1)
                                break;
                            }
                    // Если ситуация имеет вид "A alpha., a"
                    if (itvalue[itvalue.IndexOf('.') + 1] == ',')
                    { // за точкой запятая
                        a = itvalue[itvalue.Length - 1]; // определить значение a
                        string alpha = itvalue.Split('.') [0].Substring(2); // и alpha                      5!
                        if (itvalue[0] != 'П')
                        { // если левая часть не равна П
                            // ACTION[i, a] = reduce A -> alpha
                            if (WriteActionTableValue(ACTION, i, a, "r " + itvalue[0] + " " + alpha) == false)
                                return null; // грамматика не LR(1)
                        }
                    }
                    // Если ситуация имеет вид "П S.,$"
                    if (itvalue.Equals("П S.,$"))
                    {
                        // ACTION[i, '$'] = accept
                        if (WriteActionTableValue(ACTION, i, '$', "a") == false)
                            return null; // грамматика не LR(1)
                    }
                }
            }
            return ACTION;
        }

        protected Hashtable CreateGotoTable(ArrayList[] CArray)
        {
            Hashtable GOTO = new Hashtable();
            for (int c = 0; c < NonTerminals.Length; c++)
                // для каждого нетерминала А
                for (int i = 0; i < CArray.Length; i++)
                { // для каждого элемента Ii из С
                    ArrayList g = GoTo(CArray[i], NonTerminals[c]);
                    // g=GoTo[Ii, A]
                    for (int j = 0; j < CArray.Length; j++)
                        // если в С есть Ij=g
                        if (SetsEqual(g, CArray[j]))
                            // GOTO[i, A] =j
                            GOTO[new Tablekey(i, NonTerminals[c])] = j;
                }
            return GOTO;
        }
    }
}