// ============================================================================
// RaGsys - Система для работы с формальными языками, грамматиками и автоматами
// ============================================================================
// Программа предоставляет интерактивный интерфейс для выполнения лабораторных
// работ и курсовых проектов по теории формальных языков и автоматов.
// 
// Основные возможности:
// - Работа с конечными автоматами (детерминированными и недетерминированными)
// - Преобразование грамматик (КС-грамматики, нормальные формы)
// - Построение и анализ МП-автоматов
// - LL и LR анализ
// - Синтаксически управляемая трансляция (SDT)
// - Атрибутные грамматики (AT-Grammar)
// ============================================================================

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Data;
using System.Text;
using System.Linq.Expressions;
using System.IO;         // Для генератора заданий (4.0) и работы с файлами
using System.Reflection; // Для генератора заданий (4.0) и рефлексии

using RaGlib;           // Основная библиотека для работы с грамматиками и автоматами
using RaGlib.Core;      // Базовые классы (Symbol, Symbol_Operation и т.д.)
using RaGlib.Grammars;  // Классы для работы с грамматиками
using RaGlib.Automata;   // Классы для работы с автоматами
using RaGlib.SetL;      // Интерпретатор SetL для работы с множествами
using System.Text.RegularExpressions; // Для работы с регулярными выражениями

namespace RaGsystems
{

  // Главный класс программы RaGsys.
  // Предоставляет интерактивное меню для выбора и выполнения лабораторных работ
  // и курсовых проектов по теории формальных языков.
  class Program
  {

    // Выводит на экран интерактивное меню со списком доступных лабораторных работ
    // и курсовых проектов. Пользователь может выбрать нужный пункт, введя его номер.
    // Меню разделено на несколько категорий:
    // - Лабораторные работы (1.x, 2.x, 3.x, 4.x, 6.x, 7.x, 9.x, 14, 16.x)
    // - Курсовые проекты (I1-I12)
    static void Dialog()
    {
      Console.WriteLine("Наберите соответствующий номер лабораторной или курсовой и нажмите <Enter>");
      Console.WriteLine("Лабораторные работы:");
      Console.WriteLine("1.1  Лемма о накачке рег., КС языки");
      Console.WriteLine("1.2  Составные автоматы");
      Console.WriteLine("2    Спроектировать конечный автомат DFS (lab 2)");
      Console.WriteLine("2.1  Алгоритм построения КА по заданной грамматике");
      Console.WriteLine("2.2  Грамматика с операционными символами и алгоритм  преобразование в КА");
      Console.WriteLine(
        "2.3  Алгоритм построения КА по заданной грамматике + составные автоматы (конкатенация - merge(), объединение - union())");
      Console.WriteLine("3    НДКА ");
      Console.WriteLine("3.1  Convert NDFS to DFS (lab 3)");
      Console.WriteLine("3.2  Convert NDFS to DFS (lab 3) example");
      Console.WriteLine("3.3  Example FA with the extension of the AddRule by Regex [A-Z]  ");
      Console.WriteLine("");
      Console.WriteLine("4.0  Генератор заданий Grammar_generator");
      Console.WriteLine("");
      Console.WriteLine("4.1  Приведение граматики к нормальной форме CFGrammar (lab 4 - 6)");
      Console.WriteLine("6.1  Grammar in Greibach normal form");
      Console.WriteLine("6.2  Grammar in Chomsky normal form");
      Console.WriteLine("7    Алгоритм построения МП автомата по приведенной КС-грамматики PDA (lab 7-8)");
      Console.WriteLine("7.1 КС-грамматика в МП-автомат пример 1");
      Console.WriteLine("7.2 КС-грамматика в МП-автомат пример 2");
      Console.WriteLine("7.3 МП - автомат пример 3");
      Console.WriteLine("7.4 НДМП - автомат пример 4");
      Console.WriteLine("7.5 МП - автомат пример 5");
      Console.WriteLine("7.10 Конфигурируемый МП-автомат: недетерминированный МПА");
      Console.WriteLine("7.11 Расширенный МП-автомат (детерминированный)");
      Console.WriteLine("7.12 Конфигурируемый МП-автомат: недетерминированный РМПА");


      Console.WriteLine("");
      Console.WriteLine("9.1  Для LL(1) анализатора построить управляющую таблицу M.\n" +
                        "     Аналитически написать такты работы LL(1) анализатора для выведенной цепочки.");
      Console.WriteLine("9.2  Для LL(1) анализатора построить управляющую таблицу M.\n" +
                        "     Аналитически написать такты работы LL(1) анализатора для выведенной цепочки с подробным разбором.");
      Console.WriteLine("");
      Console.WriteLine("14   Построить каноническую форму множества ситуаций.\n" +
                        "     Построить управляющую таблицу для функции перехода  g(х) и действий f(u)");
      Console.WriteLine("16.1 LR(0) using g(X), f(a)");
      Console.WriteLine("16.2 LR(0) using g(X), f(a) example");
      Console.WriteLine("16.3 LR(1) using g(X), f(a)");
      Console.WriteLine("16.4 LR(1) using g(X), f(a) example ");
      Console.WriteLine("");
      Console.WriteLine("Контекстно-Зависимые грамматики");
      Console.WriteLine("17. Контекстно-зависимая грамматика a^n b^n c^n на Машине Тьюринга");
      Console.WriteLine("18. Контекстно-зависимая грамматика a^(2^n) на Машине Тьюринга");
      Console.WriteLine("19. Контекстно-зависимая грамматика a^n b^n c^n на LDA");
      Console.WriteLine("20. Контекстно-зависимая грамматика a^(2^n) на LDA");
      Console.WriteLine("\n\nКурсовые проеты: I");
      Console.WriteLine("I1   Терия перевода SDTSchemata");
      Console.WriteLine("I2   Преобразование КС-грамматики в транслирующую с операционными символами");
      Console.WriteLine("I3   Grammar to AT-Grammar");
      Console.WriteLine("I4   AT-Grammar");
      Console.WriteLine("I5   AT-Grammar for python vars types");
      Console.WriteLine("I6   AT-grammar for translating for C ++ into Python");
      Console.WriteLine("");
      Console.WriteLine("I7   Chain Translation example");
      Console.WriteLine("I8   L-attribute translation");
      Console.WriteLine("I9   Parse Tree translation");
      Console.WriteLine("");
      Console.WriteLine("I10  SDT schema builder");
      Console.WriteLine("I11  SetL");
      Console.WriteLine("I12  Преобразование деревьев при помощи СУ-схемы");
      Console.WriteLine(
        "I19  Разработка контекстно-свободной грамматики исчисления высказываний к нормальной форме и построение эквивалентного магазинного автомата");
      Console.WriteLine("I20 Контекстно-зависимая грамматика (LBA/TM) для a^n b^n c^n");

    }

    // Точка входа в программу. Реализует основной цикл работы приложения.
    // Программа работает в интерактивном режиме:
    // 1. Выводит меню доступных лабораторных работ и курсовых проектов
    // 2. Ожидает ввода номера от пользователя
    // 3. Выполняет соответствующую работу через switch-case
    // 4. После завершения работы возвращается к меню
    // 
    // Для выхода из программы пользователь должен ввести команду, обрабатываемую в default case.

    public static void Main()
    {
      // Настройка кодировки консоли для корректного отображения Unicode символов
      // (включая греческие буквы, такие как ε - эпсилон)
      // Console.OutputEncoding = System.Text.Encoding.Unicode;

      Console.OutputEncoding = System.Text.Encoding.UTF8;
      Console.InputEncoding = System.Text.Encoding.UTF8;

      // Основной цикл программы - работает до явного выхода пользователя
      for (;;)
      {
        // Вывод меню на экран
        Dialog();

                // Обработка выбора пользователя через switch-case конструкцию
                // Каждый case соответствует определенной лабораторной работе или курсовому проекту
                // Switch-case обеспечивает маршрутизацию к соответствующему коду выполнения
                switch (Console.ReadLine())
                {
                    // Лабораторная работа 1.0: Построение регулярного выражения с использованием операционных символов.
                    // Демонстрирует создание конечного автомата с поддержкой операционных символов,
                    // которые позволяют использовать регулярные выражения в правилах переходов автомата.
                    // 
                    // Алгоритм работы:
                    // 1. Создается словарь операционных символов {a1}-{a8} с соответствующими функциями проверки
                    // 2. Создается конечный автомат с операционными символами (FSAutomateWithOpSymbols)
                    // 3. Пользователь вводит входную цепочку и последовательность операционных символов
                    // 4. Автомат динамически строится на основе введенных операционных символов
                    // 5. Выполняется проверка входной цепочки на соответствие построенному автомату
                    // 
                    // Операционные символы реализуют различные проверки регулярных выражений:
                    // - {a1}: \W - не буква/цифра/подчеркивание
                    // - {a2}: проверка на символ 'p'
                    // - {a3}: проверка на символ 'o'
                    // - {a4}: [#-] - символ '#' или '-'
                    // - {a5}: \s - пробельный символ
                    // - {a6}: \d - цифра
                    // - {a7}: [\s-] - пробел или '-'
                    case "1.0":
                        {
                            // Словарь операционных символов и их функций проверки
                            // Ключ: Symbol_Operation (например, "{a1}")
                            // Значение: функция проверки, принимающая (строка, начальный_индекс, конечный_индекс) 
                            //           и возвращающая (результат_проверки, модифицированная_строка)

                            var D = new Dictionary<Symbol_Operation, Func<string, int, int, (bool, string)>>()
            {
              { new Symbol_Operation("{a1}"), Operations.OperationA },
              { new Symbol_Operation("{a2}"), Operations.OperationB },
              { new Symbol_Operation("{a3}"), Operations.OperationC },
              { new Symbol_Operation("{a4}"), Operations.OperationD },
              { new Symbol_Operation("{a5}"), Operations.OperationE },
              { new Symbol_Operation("{a6}"), Operations.OperationF },
              { new Symbol_Operation("{a7}"), Operations.OperationG },
              { new Symbol_Operation("{a8}"), Operations.OperationH },
            };

                            // Словарь для хранения операций в виде делегатов (для совместимости с различными API)
                            var D2 = new Dictionary<Symbol_Operation, Delegate>();

                            // Копирование операций из типизированного словаря в словарь делегатов
                            foreach (var item in D)
                            {
                                D2.Add(item.Key, item.Value);
                            }

                            // Создание списка операционных символов для передачи в конструктор автомата
                            // Эти символы определяют алфавит операций, доступных автомату
                            var sigmaOP = new List<Symbol_Operation>()
            {
              new Symbol_Operation("{a1}"),
              new Symbol_Operation("{a2}"),
              new Symbol_Operation("{a3}"),
              new Symbol_Operation("{a4}"),
              new Symbol_Operation("{a5}"),
              new Symbol_Operation("{a6}"),
              new Symbol_Operation("{a7}")
            };

                            // Создаем автомат: Q (состояния), Sigma (алфавит), SigmaOP (операционные символы), F (финальные), q0 (начальное)
                            var automate = new FSAutomateWithOpSymbols(
                              new List<Symbol>() { "A0" }, // Q - состояния
                              new List<Symbol>(), // Sigma - алфавит (пока пустой, будет заполняться при добавлении правил)
                              sigmaOP, // SigmaOP - операционные символы
                              new List<Symbol>() { "qf" }, // F - финальные состояния
                              new Symbol("A0") // q0 - начальное состояние
                            );

                            // Добавляем операции в словарь автомата (если есть доступ к OpDictionary)
                            // Примечание: D2 создан, но не используется напрямую в конструкторе

                            Console.WriteLine("Операционные символы:");
                            Console.WriteLine(
                              "\t{a1} (\\W) - проверка символа на то, что он не является символом латиницы, цифрой или нижним подчёркиванием");
                            Console.WriteLine("\t{a2} (p) - проверка символа на соответствие символу 'p'");
                            Console.WriteLine("\t{a3} (o) - проверка символа на соответствие символу 'p'");
                            Console.WriteLine("\t{a4} ([#-]) - проверка символа на соответствие символу '#' или '-'");
                            Console.WriteLine("\t{a5} (\\s) - проверка символа на то, что он является пробельным");
                            Console.WriteLine("\t{a6} (\\d) - проверка символа на то, что он является цифрой");
                            Console.WriteLine("\t{a7} ([\\s-]) - проверка символа на то, что он является пробельным или равен '-'");
                            Console.Write("Примечание : Операционные символы имеют параметры, представляющие");
                            Console.WriteLine(
                              " собой пару чисел a, b. Эти числа обозначают собой диапазон кол-ва повторений паттерна.");
                            Console.WriteLine("Пример: набор операционных символов {a1{0, 1}} соответствует паттерну \\W{0, 1}");
                            Console.WriteLine();

                            // Интерактивный ввод данных от пользователя
                            Console.WriteLine("Набор примеров входных цепочек:");
                            Console.WriteLine("bcdrfgth");
                            Console.WriteLine("1237534639");
                            Console.WriteLine("bdfks73h7f");
                            string p = Console.ReadLine(); // Входная цепочка для проверки

                            Console.WriteLine("Набор примеров операционных символов: ");
                            Console.WriteLine("\t{a1}");
                            Console.WriteLine("\t{a6}");
                            Console.WriteLine("\t{a7}");
                            string s = Console.ReadLine(); // Последовательность операционных символов

                            // Парсинг введенной строки операционных символов с помощью регулярного выражения
                            // Паттерн: (\w+) - имя символа, (?:\{([\d,\s]*)\})? - опциональные параметры {min, max}
                            var x = Regex.Matches(s, @"((\w+)(?:\{([\d,\s]*)\})?)");
                            int i = 0;

                            // Динамическое построение автомата: для каждого найденного операционного символа
                            // создается переход из состояния A{i} в состояние A{i+1}
                            foreach (Match match in x)
                            {
                                automate.AddRule($"A{i}", match.Value, $"A{i + 1}");
                                i++;
                            }

                            // Установка финального состояния: последнее созданное состояние становится финальным
                            automate.F = new List<Symbol>() { new Symbol($"A{i}") };

                            // Выполнение автомата на входной цепочке
                            // Автомат проверяет, принадлежит ли цепочка языку, определяемому операционными символами
                            automate.Execute(p);
                            Console.ReadLine();
                        }
                        break;

                    // Лабораторная работа 1.1: Лемма о накачке для регулярных и контекстно-свободных языков.

                    // Демонстрирует применение леммы о накачке (Pumping Lemma) для доказательства 
                    // нерегулярности или неконтекстно-свободности языков.
                    // 
                    // Лемма о накачке - это важный инструмент в теории формальных языков, который позволяет
                    // доказать, что некоторый язык не является регулярным (или КС-языком), показав, что
                    // для достаточно длинных цепочек языка невозможно выполнить условия леммы.
                    // 
                    // Метод использует интерактивный диалог для ввода языка и проверки условий леммы.
                    case "1.1": // Pumping lemma

                        var check_pumping = new Pumping();
                        check_pumping.Dialog();

                        break;

                    // Лабораторная работа 1.2: Составные автоматы - операции объединения и конкатенации.

                    // Демонстрирует построение составных конечных автоматов из нескольких простых автоматов
                    // с использованием операций над автоматами.
                    // 
                    // Основные операции:
                    // - Merge (конкатенация): язык L1·L2 = {xy | x ∈ L1, y ∈ L2}
                    //   Автомат для конкатенации строится путем соединения финальных состояний первого
                    //   автомата с начальным состоянием второго автомата.
                    // 
                    // - Union (объединение): язык L1 ∪ L2 = {x | x ∈ L1 или x ∈ L2}
                    //   Автомат для объединения строится путем создания нового начального состояния,
                    //   из которого идут ε-переходы в начальные состояния обоих автоматов.
                    // 
                    // Работа демонстрирует построение составных автоматов из трех базовых автоматов
                    // и тестирование их на различных входных цепочках.
                    case "1.2": // Compound automata  Lab 1.
                        FSAutomate[] automats = new FSAutomate[]
                        {
              new FSAutomate(
                new List<Symbol>() { "S01", "A1", "B1", "C1", "qf1" },
                new List<Symbol>() { "0" },
                new List<Symbol>() { "qf1" },
                "S01"
              ),
              new FSAutomate(
                new List<Symbol>() { "S02", "A2", "B2", "C2", "D2", "qf2" },
                new List<Symbol>() { "0", "1", "(", ")", "+" },
                new List<Symbol>() { "qf2" },
                "S02"
              ),
              new FSAutomate(
                new List<Symbol>() { "S03", "qf3" },
                new List<Symbol>() { "0", "1" },
                new List<Symbol>() { "qf3" },
                "S03"
              ),
                        };
                        //deltas
                        automats[0].AddRule("S01", "0", "A1");
                        automats[0].AddRule("A1", "0", "B1");
                        automats[0].AddRule("B1", "0", "C1");
                        automats[0].AddRule("C1", "0", "A1");
                        automats[0].AddRule("C1", "0", "qf1");

                        automats[1].AddRule("S02", "(", "A2");
                        automats[1].AddRule("A2", "0", "B2");
                        automats[1].AddRule("B2", "+", "C2");
                        automats[1].AddRule("C2", "1", "D2");
                        automats[1].AddRule("D2", ")", "qf2");

                        automats[2].AddRule("S03", "0", "S03");
                        automats[2].AddRule("S03", "0", "qf3");
                        automats[2].AddRule("S03", "1", "S03");
                        automats[2].AddRule("S03", "1", "qf3");
                        automats[2].AddRule("S03", "", "qf3");

                        var dka1 = new FSAutomate(); // правильно
                        dka1.BuildDeltaDKAutomate(automats[0], false); // правильно
                        var dka2 = new FSAutomate();
                        dka2.BuildDeltaDKAutomate(automats[1], false);
                        var dka3 = new FSAutomate();
                        dka3.BuildDeltaDKAutomate(automats[2], false);

                        // правильно
                        var automats1 = new FSAutomate[] { dka1, dka2, dka3, };
                        //merge
                        var merged12 = Compound_FSAutomate.Merge2(dka1, dka2); // неправильно
                        var merged23 = Compound_FSAutomate.Merge2(dka2, dka3);
                        var merged123 = Compound_FSAutomate.Merge(automats1);
                        var union12 = Compound_FSAutomate.Union2(dka1, dka2);
                        var union1 = new FSAutomate();
                        union1.BuildDeltaDKAutomate(union12, false);
                        var union123 = Compound_FSAutomate.Union(automats1);
                        var union2 = new FSAutomate();
                        union2.BuildDeltaDKAutomate(union123, false);

                        var exectionOrder = new FSAutomate[] { union1, union2, merged12, merged23, merged123 };
                        string[] names =
                          { "объединение КА1, КА2", "объединение КА1, КА2, КА3", "КА1+КА2", "КА2+КА3", "КА1+КА2+КА3" };

                        Console.WriteLine();

                        Console.WriteLine("Были построены составные автоматы:");
                        Console.WriteLine("1. объединение КА1, КА2;");
                        Console.WriteLine("2. объединение КА1, КА2, КА3;");
                        Console.WriteLine("3. КА1 +КА2;");
                        Console.WriteLine("4. КА2+КА3;");
                        Console.WriteLine("5. КА1+КА2+КА3;");

                        Console.WriteLine();
                        Console.WriteLine("Примеры цепочек для постоенных составных автоматов 1, 2, 3, 4, 5: ");
                        Console.WriteLine("0, 1, 01, 0000, 0000000, (0+1), 0000(0+1), 0000(0+1)0, 0000(0+1)1");
                        for (; ; )
                        {
                            Console.WriteLine();
                            Console.WriteLine("Введите цепочку (или выйти stop):");
                            string s = Console.ReadLine();
                            Console.WriteLine();
                            if (s == "stop") break;

                            for (int i = 0; i < exectionOrder.Length; i++)
                            {
                                if (exectionOrder[i].Execute_FSA(s))
                                {
                                    Console.WriteLine("Автомат " + names[i] + " распознал цепочку " + s);
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("Автомат " + names[i] + " не распознал цепочку " + s);
                                    if (i == exectionOrder.Length - 1)
                                        Console.WriteLine("Данная цепочка не была распознана");
                                    //  Console.ReadLine();
                                }
                            }
                        }

                        break;

                    // Лабораторная работа 2: Проектирование детерминированного конечного автомата (ДКА/DFS).

                    // Демонстрирует ручное проектирование детерминированного конечного автомата
                    // для распознавания конкретной цепочки символов.
                    // 
                    // Автомат распознает цепочку вида: "1010-1+0" или "1010-1+1"
                    // Структура цепочки: четыре символа (1,0,1,0), затем '-', затем '1', затем '+', затем '0' или '1'
                    // 
                    // Автомат построен с явным указанием всех состояний и переходов,
                    // что позволяет понять процесс проектирования ДКА "с нуля".

                    case "2":
                        var ka = new FSAutomate(
                          new List<Symbol>() { "S0", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "qf" },
                          new List<Symbol>() { "0", "1", "-", "+", "" },
                          new List<Symbol>() { "qf" },
                          "S0");
                        ka.AddRule("S0", "1", "A");
                        ka.AddRule("A", "0", "B");
                        ka.AddRule("B", "1", "C");
                        ka.AddRule("C", "0", "D");
                        ka.AddRule("D", "-", "E");
                        ka.AddRule("E", "1", "F");
                        ka.AddRule("F", "+", "G");
                        ka.AddRule("G", "0", "qf");
                        ka.AddRule("G", "1", "qf");

                        Console.WriteLine("Enter line to execute :");
                        ka.Execute(Console.ReadLine());
                        break;

                    // Лабораторная работа 2.1: Алгоритм построения конечного автомата по заданной грамматике.

                    // Демонстрирует преобразование контекстно-свободной грамматики (автоматной грамматики)
                    // в эквивалентный конечный автомат.
                    // 
                    // Алгоритм преобразования:
                    // 1. Нетерминалы грамматики становятся состояниями автомата
                    // 2. Правила грамматики преобразуются в переходы автомата
                    // 3. Начальный символ грамматики становится начальным состоянием
                    // 4. Правила вида A -> a (терминал) создают переходы в финальное состояние
                    // 
                    // Данная работа показывает фундаментальную связь между грамматиками и автоматами,
                    // демонстрируя, что для каждого регулярного языка существует как грамматика,
                    // так и эквивалентный конечный автомат.

                    case "2.1":
                        var Gram = new Grammar(new List<Symbol>() { "0", "1" },
                          new List<Symbol>() { "S0", "A", "B" },
                          "S0");
                        Gram.AddRule("S0", new List<Symbol>() { "0" });
                        Gram.AddRule("S0", new List<Symbol>() { "0", "A" });
                        Gram.AddRule("A", new List<Symbol>() { "1", "B" });
                        Gram.AddRule("B", new List<Symbol>() { "0" });
                        Gram.AddRule("B", new List<Symbol>() { "0", "A" });

                        // Преобразование автоматной грамматики в конечный автомат (State Machine)
                        // Метод Transform() реализует алгоритм построения КА по грамматике
                        var KA = Gram.Transform();
                        KA.DebugAuto();
                        break;


                    // Лабораторная работа 2.2: Грамматика с операционными символами и преобразование в КА.

                    // Демонстрирует работу с грамматиками, содержащими операционные символы (семантические действия),
                    // и их преобразование в конечные автоматы с поддержкой операций.
                    // 
                    // Операционные символы позволяют выполнять действия во время разбора:
                    // - {save} - сохранение данных в файл
                    // - {load} - загрузка данных из файла
                    // - {split} - разделение строки
                    // 
                    // При преобразовании грамматики в автомат операционные символы сохраняются
                    // и выполняются при прохождении соответствующих переходов автомата.
                    // 
                    // Это позволяет создавать трансляторы и интерпретаторы, которые не только
                    // проверяют синтаксис, но и выполняют семантические действия.

                    case "2.2":
                        var GramOP = new GrammarWithOpSymbol(new List<Symbol>() { "a", "e", "f", "d" },
                          new List<Symbol_Operation>()
                            { new Symbol_Operation("{save}"), new Symbol_Operation("{split}"), new Symbol_Operation("{load}") },
                          new List<Symbol>() { "S0", "E", "F", "D" },
                          "S0");
                        GramOP.AddRule("S0", new List<Symbol>() { "a", "E" });
                        GramOP.AddRule("E", new List<Symbol>() { "e", "F", "{load}" });
                        GramOP.AddRule("F", new List<Symbol>() { "f", "D", "{split}" });
                        GramOP.AddRule("D", new List<Symbol>() { "d", "{save}" });

                        var KAOP = GramOP.Transform();
                        KAOP.DebugAuto();
                        Console.WriteLine("Enter line to execute :");
                        KAOP.Execute(Console.ReadLine());
                        Console.ReadLine();
                        break;


                    // Лабораторная работа 2.3: Построение КА по грамматике с составными автоматами.

                    // Демонстрирует построение конечных автоматов из нескольких грамматик
                    // с последующим применением операций над автоматами (конкатенация и объединение).
                    // 
                    // Алгоритм работы:
                    // 1. Создаются несколько грамматик, каждая описывает свой подъязык
                    // 2. Каждая грамматика преобразуется в конечный автомат
                    // 3. Автоматы объединяются с помощью операций:
                    //    - Merge (конкатенация): последовательное соединение автоматов
                    //    - Union (объединение): параллельное соединение автоматов
                    // 
                    // Это демонстрирует модульный подход к построению сложных автоматов:
                    // сложный язык можно разбить на простые подъязыки, построить для каждого
                    // автомат, а затем объединить их с помощью операций.

                    case "2.3":
                        // grammar 1
                        var grammar2_2__1 = new Grammar(new List<Symbol>() { "a", "l", "t", "i", "p", "s", " " },
                          new List<Symbol>() { "S0", "A", "B", "C", "D", "E", "F", "G" }, "S0");
                        grammar2_2__1.AddRule("S0", new List<Symbol>() { "a", "A" });
                        grammar2_2__1.AddRule("A", new List<Symbol>() { "l", "B" });
                        grammar2_2__1.AddRule("B", new List<Symbol>() { "l", "C" });
                        grammar2_2__1.AddRule("C", new List<Symbol>() { " ", "D" });
                        grammar2_2__1.AddRule("D", new List<Symbol>() { "t", "E" });
                        grammar2_2__1.AddRule("E", new List<Symbol>() { "i", "F" });
                        grammar2_2__1.AddRule("F", new List<Symbol>() { "p", "G" });
                        grammar2_2__1.AddRule("G", new List<Symbol>() { "s" });

                        // From Automaton Grammar to State Machine(KA)
                        var fsa_1 = grammar2_2__1.Transform();
                        Console.WriteLine("fsa_1:\n");
                        fsa_1.DebugAuto();
                        Console.WriteLine("Enter line to execute(fsa1) :");
                        string str_1 = "all tips";
                        fsa_1.Execute(str_1);

                        // grammar 3
                        var grammar2_2__3 = new Grammar(new List<Symbol>() { "g", "r", "a", "v", "i", "a", "!", "?", "*", " " },
                          new List<Symbol>() { "S0", "A", "B", "C", "D", "E", "F" }, "S0");
                        grammar2_2__3.AddRule("S0", new List<Symbol>() { "!", "S0" });
                        grammar2_2__3.AddRule("S0", new List<Symbol>() { "*", "S0" });
                        grammar2_2__3.AddRule("S0", new List<Symbol>() { "?", "S0" });
                        grammar2_2__3.AddRule("S0", new List<Symbol>() { "g", "A" });
                        grammar2_2__3.AddRule("A", new List<Symbol>() { "r", "B" });
                        grammar2_2__3.AddRule("B", new List<Symbol>() { "a", "C" });
                        grammar2_2__3.AddRule("B", new List<Symbol>() { "@", "C" });
                        grammar2_2__3.AddRule("C", new List<Symbol>() { "v", "D" });
                        grammar2_2__3.AddRule("D", new List<Symbol>() { "i", "E" });
                        grammar2_2__3.AddRule("D", new List<Symbol>() { "1", "E" });
                        grammar2_2__3.AddRule("D", new List<Symbol>() { "!", "E" });
                        grammar2_2__3.AddRule("E", new List<Symbol>() { "a", "C" });
                        grammar2_2__3.AddRule("E", new List<Symbol>() { "@", "C" });
                        grammar2_2__3.AddRule("F", new List<Symbol>() { "!", "F" });
                        grammar2_2__3.AddRule("F", new List<Symbol>() { "*", "F" });
                        grammar2_2__3.AddRule("F", new List<Symbol>() { "?", "F" });
                        grammar2_2__3.AddRule("F", new List<Symbol>() { " " });

                        // From Automaton Grammar to State Machine(KA)
                        var fsa_3 = grammar2_2__3.Transform();
                        Console.WriteLine("fsa_3:\n");
                        fsa_3.DebugAuto();
                        Console.WriteLine("Enter line to execute(fsa3) :");
                        string str_3 = "gravia ";
                        fsa_3.Execute(str_3);

                        // grammar 9
                        var grammar2_2__9 = new Grammar(
                          new List<Symbol>() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " " },
                          new List<Symbol>() { "S0", "A", "B", "C", "D", "E", "F", "G" }, "S0");
                        grammar2_2__9.AddRule("S0", new List<Symbol>() { "0", "A" });
                        grammar2_2__9.AddRule("S0", new List<Symbol>() { "1", "A" });
                        grammar2_2__9.AddRule("S0", new List<Symbol>() { "2", "A" });
                        grammar2_2__9.AddRule("S0", new List<Symbol>() { "3", "A" });
                        grammar2_2__9.AddRule("S0", new List<Symbol>() { "4", "A" });
                        grammar2_2__9.AddRule("S0", new List<Symbol>() { "5", "A" });
                        grammar2_2__9.AddRule("S0", new List<Symbol>() { "6", "A" });
                        grammar2_2__9.AddRule("S0", new List<Symbol>() { "7", "A" });
                        grammar2_2__9.AddRule("S0", new List<Symbol>() { "8", "A" });
                        grammar2_2__9.AddRule("S0", new List<Symbol>() { "9", "A" });

                        grammar2_2__9.AddRule("A", new List<Symbol>() { "0", "A" });
                        grammar2_2__9.AddRule("A", new List<Symbol>() { "1", "A" });
                        grammar2_2__9.AddRule("A", new List<Symbol>() { "2", "A" });
                        grammar2_2__9.AddRule("A", new List<Symbol>() { "3", "A" });
                        grammar2_2__9.AddRule("A", new List<Symbol>() { "4", "A" });
                        grammar2_2__9.AddRule("A", new List<Symbol>() { "5", "A" });
                        grammar2_2__9.AddRule("A", new List<Symbol>() { "6", "A" });
                        grammar2_2__9.AddRule("A", new List<Symbol>() { "7", "A" });
                        grammar2_2__9.AddRule("A", new List<Symbol>() { "8", "A" });
                        grammar2_2__9.AddRule("A", new List<Symbol>() { "9", "A" });
                        grammar2_2__9.AddRule("A", new List<Symbol>() { " " });

                        // From Automaton Grammar to State Machine(KA)
                        var fsa_9 = grammar2_2__9.Transform();
                        Console.WriteLine("fsa_9:\n");
                        fsa_9.DebugAuto();
                        Console.WriteLine("Enter line to execute(fsa9) :");
                        string str_9 = "124134 ";
                        fsa_9.Execute(str_9);


                        var FSAs = new FSAutomate[] { fsa_1, fsa_3, fsa_9 };
                        //merge
                        var Concatenation123 = Compound_FSAutomate.Merge(FSAs);
                        var Union123 = Compound_FSAutomate.Union(FSAs);

                        Console.WriteLine("\nMerge of three automatas");
                        Concatenation123.DebugAuto();

                        Console.WriteLine("\nUnion of three automatas");
                        Union123.DebugAuto();
                        Console.WriteLine();

                        Console.WriteLine("Enter line to execute(Concatenation123) :");
                        Concatenation123.Execute("ara tips ");
                        Console.WriteLine("Enter line to execute(union123) :");
                        Union123.Execute("eeall tips");

                        break;


                    // 3.1 - Преобразование недетерминированного КА в детерминированный (NDFS to DFS).
                    // Применяет алгоритм построения детерминированного автомата из недетерминированного.
                    case "3.1":
                        var ndfsa = new FSAutomate(
                          new List<Symbol>()
                          {
                "S0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18",
                "19", "20", "qf"
                          },
                          new List<Symbol>() { "1", "0", "+", "2" },
                          new List<Symbol>() { "qf" },
                          "S0");
                        ndfsa.AddRule("S0", "1", "1"); //W1
                        ndfsa.AddRule("1", "0", "2");
                        ndfsa.AddRule("2", "+", "3");

                        ndfsa.AddRule("3", "", "4"); //W2
                        ndfsa.AddRule("4", "", "5");
                        ndfsa.AddRule("4", "", "7");
                        ndfsa.AddRule("4", "", "9");
                        ndfsa.AddRule("5", "1", "6");
                        ndfsa.AddRule("7", "2", "8");
                        ndfsa.AddRule("6", "", "9");
                        ndfsa.AddRule("8", "", "9");
                        ndfsa.AddRule("9", "", "4");
                        ndfsa.AddRule("9", "", "10");

                        ndfsa.AddRule("10", "1", "11"); //W3
                        ndfsa.AddRule("11", "0", "12");
                        ndfsa.AddRule("12", "", "13");
                        ndfsa.AddRule("13", "", "9");
                        ndfsa.AddRule("13", "", "14");

                        ndfsa.AddRule("14", "", "15"); //W4
                        ndfsa.AddRule("14", "", "17");
                        ndfsa.AddRule("15", "0", "16");
                        ndfsa.AddRule("17", "1", "18");
                        ndfsa.AddRule("16", "", "19");
                        ndfsa.AddRule("18", "", "19");
                        ndfsa.AddRule("19", "", "14");
                        ndfsa.AddRule("19", "", "20");
                        ndfsa.AddRule("20", "", "15");
                        ndfsa.AddRule("14", "", "qf");
                        ndfsa.AddRule("20", "", "qf");

                        var dka = new FSAutomate();
                        dka.BuildDeltaDKAutomate(ndfsa, false);
                        dka.DebugAuto();
                        Console.WriteLine("Enter line to execute 10+10 :");
                        dka.Execute(Console.ReadLine());
                        break;

                    // 3.2 - Пример преобразования NDFS в DFS.
                    // Демонстрирует работу алгоритма преобразования на конкретном примере.
                    case "3.2":
                        var example = new FSAutomate(new List<Symbol>() { "S0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "qf" },
                          new List<Symbol>() { "a", "b" },
                          new List<Symbol>() { "qf" },
                          "S0");
                        example.AddRule("S0", "", "1");
                        example.AddRule("S0", "", "7");
                        example.AddRule("1", "", "2");
                        example.AddRule("1", "", "4");
                        example.AddRule("2", "a", "3");
                        example.AddRule("4", "b", "5");
                        example.AddRule("3", "", "6");
                        example.AddRule("5", "", "6");
                        example.AddRule("6", "", "1");
                        example.AddRule("6", "", "7");
                        example.AddRule("7", "a", "8");
                        example.AddRule("8", "b", "9");
                        example.AddRule("9", "b", "qf");

                        var dkaEX = new FSAutomate();
                        dkaEX.BuildDeltaDKAutomate(example, false); // false по таблице  
                        dkaEX.DebugAuto(); // true Алгоритм Томсона по очереди 
                        Console.WriteLine("Enter line to execute :");
                        dkaEX.Execute(Console.ReadLine());
                        break;
                    // 3.3 - Пример КА с расширением AddRule по регулярному выражению [A-Z].
                    // Демонстрирует использование регулярных выражений в правилах автомата.
                    case "3.3":
                        {
                            var ka8 = new FSAutomate(new List<Symbol>() { "S0", "A", "C", "B", "D", "qf" },
                              new List<Symbol>() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "#", "$", "%" },
                              new List<Symbol>() { "qf" }, "S0");
                            ka8.AddRule("S0", "[0-8]", "A");
                            ka8.AddRule("A", "[#-%]", "B");
                            ka8.AddRule("B", "[02468]", "C");
                            foreach (char i in "0123456789".ToCharArray())
                                ka8.AddRule("C", i.ToString(), "D");
                            foreach (char i in "0123456789".ToCharArray())
                                ka8.AddRule("D", i.ToString(), "qf");

                            ka8.DebugAuto();
                            Console.WriteLine("Enter line to execute 1#845 :");
                            ka8.Execute(Console.ReadLine());
                            break;
                        }
                    // 4.0 - Генератор заданий для КС-грамматик.
                    // Позволяет генерировать варианты контекстно-свободных грамматик
                    // и сохранять/загружать их из файла для учебных целей.
                    case "4.0":
                        // v2.2022 201- Тихонов 
                        var generator = new Tools.CoreGrammarGenerator();
                        var newGrammarGenerator = new Tools.NewGrammarGenerator();
                        string path40 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + "log.txt";
                        Console.WriteLine(
                          "Выберите: Сгенерировать варианты КС грамматики и сохранить в  файл(1), и загрузить требуемый вариант из файла(2)");
                        int var40 = Convert.ToInt32(Console.ReadLine());
                        if (var40 == 1)
                        {
                            File.WriteAllText(@path40, string.Empty);
                            Console.WriteLine("Введите количество вариантов в таблице одним числом:");
                            int numbers40 = Convert.ToInt32(Console.ReadLine());
                            numbers40++;
                            Console.WriteLine("Введите seed (целое число > 0)");
                            int seed40 = Convert.ToInt32(Console.ReadLine());
                            newGrammarGenerator.LogWrite(seed40, numbers40);
                            Console.WriteLine("Файл с вариантами был добавлен на ваше устройство");
                        }
                        else if (var40 == 2)
                        {
                            Console.WriteLine("Введите ваш вариант");
                            int variant40 = Convert.ToInt32(Console.ReadLine());
                            var vg40 = newGrammarGenerator.GetTask(variant40, path40);
                            string beb_ra = generator.PrintGrammar(vg40, variant40, "V");
                            Console.WriteLine(beb_ra);
                            //загрузка из файла одного варианта в объект Grammar для приведения
                            var NewRegGr = new Grammar(vg40.T, vg40.V, "S");
                            foreach (Production pr40 in vg40.P)
                            {
                                string LHSp = pr40.LHS.ToString();
                                NewRegGr.AddRule(LHSp, pr40.RHS);
                            }
                        }

                        Console.ReadKey();
                        break;

                    // 4.1 - Приведение грамматики к нормальной форме CFGrammar.
                    // Выполняет последовательность преобразований: удаление эпсилон-правил,
                    // удаление бесполезных символов, удаление цепных правил, удаление левой рекурсии.
                    case "4.1":
                        var regGr = new Grammar(new List<Symbol>() { "b", "c" },
                          new List<Symbol>() { "S", "A", "B", "C" },
                          "S");

                        regGr.AddRule("S", new List<Symbol>() { "c", "A", "B" });
                        regGr.AddRule("S", new List<Symbol>() { "b" });
                        regGr.AddRule("B", new List<Symbol>() { "c", "B" });
                        regGr.AddRule("B", new List<Symbol>() { "b" });
                        regGr.AddRule("A", new List<Symbol>() { "Ab" });
                        regGr.AddRule("A", new List<Symbol>() { "B" });
                        regGr.AddRule("A", new List<Symbol>() { "" });
                        Console.WriteLine("Grammar:");
                        regGr.Debug("T", regGr.T);
                        regGr.Debug("T", regGr.V);
                        regGr.DebugPrules();

                        Grammar G1 = regGr.EpsDelete();
                        G1.DebugPrules();

                        Grammar G2 = G1.unUsefulDelete();
                        G2.DebugPrules();

                        Grammar G3 = G2.ChainRuleDelete();
                        G3.DebugPrules();

                        Grammar G4 = G3.LeftRecursDelete_new6();
                        G4.DebugPrules();
                        // G4 - приведенная грамматика

                        Console.WriteLine("--------------------------------------------");
                        Console.WriteLine("Normal Grammatic:");
                        G4.Debug("T", G4.T);
                        G4.Debug("V", G4.V);
                        G4.DebugPrules();
                        Console.Write("Start symbol: ");
                        Console.WriteLine(G4.S0 + "\n");
                        break;


                    // 6.1 - Приведение грамматики к нормальной форме Грейбаха.
                    // Преобразует грамматику в форму, где каждое правило имеет вид A -> aB1B2...Bk,
                    // где a - терминал, а B1...Bk - нетерминалы.
                    case "6.1":
                        {
                            var g = new Grammar(new List<Symbol>() { "+", "*", "(", ")", "i" },
                              new List<Symbol>() { "S", "F", "L", "K" },
                              "S");
                            g.AddRule("S", new List<Symbol>() { "S", "+", "F" });
                            g.AddRule("S", new List<Symbol>() { "F" });
                            g.AddRule("F", new List<Symbol>() { "F", "*", "L" });
                            g.AddRule("F", new List<Symbol>() { "L" });
                            g.AddRule("L", new List<Symbol>() { "(", "S", ")" });
                            g.AddRule("L", new List<Symbol>() { "i" });
                            g.AddRule("K", new List<Symbol>() { "i" });

                            Console.WriteLine("Grammar:");
                            g.Debug("T", g.T);
                            g.Debug("T", g.V);
                            g.DebugPrules();

                            var g1 = g.DeleteLongRules();

                            var g2 = g1.unUsefulDelete();

                            var g3 = g2.EpsDelete();

                            var g4 = g3.DeleteS0Rules();

                            var g5 = g4.ChainRuleDelete();

                            var g6 = g5.DeleteTermRules();
                            g6.DebugPrules();

                            var g7 = g6.LeftRecursDelete_new6();
                            g7.DebugPrules();
                            g7.Debug("V", g7.V);

                            var g9 = g7.TransformGrForm();

                            Console.WriteLine("--------------------------------------------");
                            Console.WriteLine("Greibach normal form:");
                            g9.Debug("T", g9.T);
                            g9.Debug("V", g9.V);
                            g9.DebugPrules();
                            Console.Write("Start symbol: ");
                            Console.WriteLine(g9.S0 + "\n");
                            break;
                        }


                    // 6.2 - Приведение грамматики к нормальной форме Хомского.
                    // Преобразует грамматику в форму, где каждое правило имеет вид A -> BC или A -> a,
                    // где A, B, C - нетерминалы, a - терминал.
                    // Algorithms приведенная грамматика G
                    case "6.2":
                        {
                            var g = new Grammar(new List<Symbol>() { "+", "*", "(", ")", "i" },
                              new List<Symbol>() { "S", "F", "L", "K" },
                              "S");
                            g.AddRule("S", new List<Symbol>() { "S", "+", "F" });
                            g.AddRule("S", new List<Symbol>() { "F" });
                            g.AddRule("F", new List<Symbol>() { "F", "*", "L" });
                            g.AddRule("F", new List<Symbol>() { "L" });
                            g.AddRule("L", new List<Symbol>() { "(", "S", ")" });
                            g.AddRule("L", new List<Symbol>() { "i" });
                            g.AddRule("K", new List<Symbol>() { "i" });

                            Console.WriteLine("Grammar:");
                            g.Debug("T", g.T);
                            g.Debug("T", g.V);
                            g.DebugPrules();

                            var g1 = g.DeleteLongRules();
                            g1.Debug("T", g1.T);
                            g1.Debug("V", g1.V);
                            g1.DebugPrules();

                            var g2 = g1.unUsefulDelete();
                            g2.DebugPrules();

                            var g3 = g2.EpsDelete();
                            g3.DebugPrules();

                            var g4 = g3.DeleteS0Rules();
                            g4.Debug("T", g4.T);
                            g4.Debug("V", g4.V);
                            g4.DebugPrules();
                            Console.Write("Start symbol: ");
                            Console.WriteLine(g4.S0 + "\n");

                            var g5 = g4.ChainRuleDelete();
                            g5.DebugPrules();

                            var g6 = g5.DeleteTermRules();

                            Console.WriteLine("--------------------------------------------");
                            Console.WriteLine("Chomsky normal form:");
                            g6.Debug("T", g6.T);
                            g6.Debug("V", g6.V);
                            g6.DebugPrules();
                            Console.Write("Start symbol: ");
                            Console.WriteLine(g6.S0 + "\n");
                            break;
                        }


                    // 7.0 - Построение МП-автомата по КС-грамматике (базовый пример).
                    // Демонстрирует преобразование контекстно-свободной грамматики в магазинный автомат.
                    case "7.0":
                        {
                            var CFGrammar = new Grammar(new List<Symbol>() { "a", "z", "#", "@" },
                              new List<Symbol>() { "D", "U", "G", "Y", "M", "F" },
                              "F");
                            CFGrammar.AddRule("F", new List<Symbol>() { "z", "D" });
                            CFGrammar.AddRule("D", new List<Symbol>() { "#", "U" });
                            CFGrammar.AddRule("U", new List<Symbol>() { "z", "G" });
                            CFGrammar.AddRule("G", new List<Symbol>() { "@", "Y" });
                            CFGrammar.AddRule("Y", new List<Symbol>() { "z", "M" });
                            CFGrammar.AddRule("M", new List<Symbol>() { "a" });
                            CFGrammar.DebugPrules();

                            var pda = new PDA(CFGrammar);
                            pda.Debug();
                            Console.Write("Enter chain:\n");
                            Console.WriteLine(pda.Execute(Console.ReadLine()).ToString());
                            break;
                        }


                    // 7 - Алгоритм преобразования КС-грамматики в недетерминированный МП-автомат.
                    // Строит магазинный автомат, эквивалентный заданной контекстно-свободной грамматике.
                    // Algorithm Grammar to PDA не детерменированный
                    case "7":
                        {
                            var CFGrammar = new Grammar(new List<Symbol>() { "b", "c" },
                              new List<Symbol>() { "S", "A", "B", "D" },
                              "S");

                            CFGrammar.AddRule("S", new List<Symbol>() { "b" });
                            CFGrammar.AddRule("S", new List<Symbol>() { "c", "A", "B" });
                            CFGrammar.AddRule("S", new List<Symbol>() { "c", "B" });

                            CFGrammar.AddRule("A", new List<Symbol>() { "b", "D" });
                            CFGrammar.AddRule("A", new List<Symbol>() { "b" });
                            CFGrammar.AddRule("A", new List<Symbol>() { "c", "B", "D" });
                            CFGrammar.AddRule("A", new List<Symbol>() { "c", "B" });

                            CFGrammar.AddRule("D", new List<Symbol>() { "b" });
                            CFGrammar.AddRule("D", new List<Symbol>() { "b", "D" });

                            CFGrammar.AddRule("B", new List<Symbol>() { "b" });
                            CFGrammar.AddRule("B", new List<Symbol>() { "c", "B" });

                            Console.Write("Debug KC-Grammar ");
                            CFGrammar.DebugPrules();

                            var pda = new PDA(CFGrammar);
                            pda.Debug();

                            Console.WriteLine("\nEnter the line   :");
                            Console.WriteLine(pda.Execute(Console.ReadLine()).ToString());
                            break;
                        }

                    // 7.1 - КС-грамматика в МП-автомат, пример 1.
                    // Демонстрирует построение МП-автомата для языка {a^n b^n | n >= 1}.
                    case "7.1":
                        {
                            // !! Algorithm Grammar to PDA {aabb, aaaabbbb}
                            // see 7.2 PDA  
                            var cfgr = new Grammar(new List<Symbol>() { "a", "b" },
                              new List<Symbol>() { "S", "A", "B" },
                              "S");

                            cfgr.AddRule("S", new List<Symbol>() { "a", "A", "b" }); // S -> aAb
                            cfgr.AddRule("A", new List<Symbol>() { "a", "B", "b" }); // A -> aBb
                            cfgr.AddRule("B", new List<Symbol>() { "a", "b" }); // B -> ab
                            Console.Write("Debug KC-Grammar ");
                            cfgr.DebugPrules();

                            var pda = new PDA(new List<Symbol>() { "q0", "q1", "q2", "qf" },
                              new List<Symbol>() { "a", "b" },
                              new List<Symbol>() { "z0", "a", "b", "S", "A", "B" },
                              "q0",
                              "S",
                              new List<Symbol>() { "qf" });
                            pda.addDeltaRule("q0", "ε", "S", new List<Symbol>() { "q1" },
                              new List<Symbol>() { "a", "A", "b" }); //δ(q0,ε,S) = (a,A,b)
                            pda.addDeltaRule("q", "ε", "A", new List<Symbol>() { "q" },
                              new List<Symbol>() { "a", "B", "b" }); //δ(q,ε,A) = (a,B,b)
                            pda.addDeltaRule("q", "ε", "B", new List<Symbol>() { "q" },
                              new List<Symbol>() { "a", "b" }); //δ(q,ε,B) = (a,b)
                            pda.addDeltaRule("q", "a", "a", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" }); //δ(q,a,a) = (ε)
                            pda.addDeltaRule("q", "a", "b", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" }); //δ(q,a,b) = (ε)
                            pda.addDeltaRule("q", "b", "b", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" }); //δ(q,b,b) = (ε)
                            pda.addDeltaRule("q", "b", "a", new List<Symbol>() { "qf" }, new List<Symbol>() { "ε" }); //δ(q,b,a) = (ε)


                            pda.Debug();

                            Console.WriteLine("\nВведите строку, пример :"); // aaabbb
                            Console.WriteLine(pda.Execute(Console.ReadLine()).ToString());
                            break;
                        }


                    // 7.2 - КС-грамматика в МП-автомат, пример 2.
                    // Демонстрирует построение МП-автомата для выражения вида i = i.
                    case "7.2":
                        {
                            // !! Algorithm Grammar to PDA {aabb, aaaabbbb}
                            var cfgr1 = new Grammar(new List<Symbol>() { "i", "=" },
                              new List<Symbol>() { "S", "F", "L" },
                              "S");

                            cfgr1.AddRule("S", new List<Symbol>() { "F", "=", "L" }); //S -> F=L
                            cfgr1.AddRule("F", new List<Symbol>() { "i" }); //F -> i
                            cfgr1.AddRule("L", new List<Symbol>() { "F" }); //L -> F
                            Console.Write("Debug KC-Grammar ");
                            cfgr1.DebugPrules();
                            var pda = new PDA(new List<Symbol>() { "q0", "q1", "q2", "qf" },
                              new List<Symbol>() { "i", "=" },
                              new List<Symbol>() { "z0", "i" },
                              "q0",
                              "z0",
                              new List<Symbol>() { "qf" });
                            pda.addDeltaRule("q0", "i", "z0", new List<Symbol>() { "q1" },
                              new List<Symbol>() { "i", "z0" }); //δ(q0,i,z0) = (i,z0)
                            pda.addDeltaRule("q", "=", "i", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" }); //δ(q,=,i) = (ε)
                            pda.addDeltaRule("q", "i", "i", new List<Symbol>() { "q" },
                              new List<Symbol>() { "i", "i" }); //δ(q,i,i) = (i,i)
                            pda.addDeltaRule("q", "ε", "i", new List<Symbol>() { "qf" }, new List<Symbol>() { "ε" }); //δ(q,ε,i) = (ε)
                            pda.Debug();
                            Console.WriteLine("\nВведите строку, пример :"); // i=i
                            Console.WriteLine(pda.Execute(Console.ReadLine()).ToString());
                            break;
                        }


                    // 7.3 - МП-автомат, пример 3.
                    // Демонстрирует работу магазинного автомата для языка {a^n b^n | n >= 1}.
                    case "7.3":
                        {
                            //МП - автоматы  {ab, aaaabbbb}         
                            var pda = new PDA(new List<Symbol>() { "q0", "q1", "q2", "qf" },
                              new List<Symbol>() { "a", "b" },
                              new List<Symbol>() { "z0", "a" },
                              "q0",
                              "z0",
                              new List<Symbol>() { "qf" });
                            pda.addDeltaRule("q0", "a", "z0", new List<Symbol>() { "q1" },
                              new List<Symbol>() { "a", "z0" }); //δ(q0,a,z0) = (a, z0)
                            pda.addDeltaRule("q", "a", "a", new List<Symbol>() { "q" },
                              new List<Symbol>() { "a", "a" }); //δ(q,a,a) = (a,a)
                            pda.addDeltaRule("q", "b", "a", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" }); //δ(q,b,a) = (ε)
                            pda.addDeltaRule("q", "ε", "z0", new List<Symbol>() { "qf" }, new List<Symbol>() { "ε" }); //δ(q,ε,z0) = (ε)

                            pda.Debug();
                            Console.WriteLine("Execute example: ab");
                            Console.WriteLine(pda.Execute("ab"));
                            break;
                        }


                    // 7.4 - Недетерминированный МП-автомат (NPDA), пример 4.
                    // Демонстрирует работу недетерминированного МП-автомата для арифметических выражений.
                    // NPDA  automata  (v + v )
                    case "7.4":
                        {
                            var npda = new PDA(
                              new List<Symbol>() { "q", "qf" },
                              new List<Symbol>() { "v", "+", "*", "(", ")" },
                              new List<Symbol>() { "v", "+", "*", "(", ")", "S", "F", "L" },
                              "q0",
                              "S",
                              new List<Symbol>() { "qf" });

                            // S -> S + F | F
                            // F -> F * L || L
                            // L -> v || (S)
                            npda.addDeltaRule("q", "v", "v", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" }); //δ(q,v,v) = (ε)
                            npda.addDeltaRule("q", "+", "+", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" }); //δ(q,+,+) = (ε)
                            npda.addDeltaRule("q", "*", "*", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" }); //δ(q,*,*) = (ε)
                            npda.addDeltaRule("q", "(", "(", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" }); //δ(q,(,() = (ε)
                            npda.addDeltaRule("q", ")", ")", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" }); //δ(q,),) ) = (ε)

                            npda.addDeltaRule("q", "+", "*", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" }); //δ(q,+,*) = (ε)
                            npda.addDeltaRule("q", "ε", "*", new List<Symbol>() { "qf" }, new List<Symbol>() { "ε" }); //δ(q,ε,*) = (ε)
                            npda.addDeltaRule("q", "(", "v", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" }); //δ(q,(,v) = (ε)
                            npda.addDeltaRule("q", "v", "*", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" }); //δ(q,v,*) = (ε)
                            npda.addDeltaRule("q", "+", "v", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" }); //δ(q,+,v) = (ε)
                            npda.addDeltaRule("q", ")", "v", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" }); //δ(q,),v) = (ε)

                            npda.addDeltaRule("q", "ε", "S", new List<Symbol>() { "q" }, new List<Symbol>() { "F" }); //δ(q,ε,S) = (F)
                            npda.addDeltaRule("q", "ε", "F", new List<Symbol>() { "q" },
                              new List<Symbol>() { "F", "*", "L" }); //δ(q,ε,F) = (F,*,L)
                            npda.addDeltaRule("q", "ε", "F", new List<Symbol>() { "q" }, new List<Symbol>() { "L" }); //δ(q,ε,F) = (L)
                            npda.addDeltaRule("q", "ε", "L", new List<Symbol>() { "q" }, new List<Symbol>() { "v" }); //δ(q,ε,L) = (v)
                            npda.addDeltaRule("q", "ε", "L", new List<Symbol>() { "q" },
                              new List<Symbol>() { "(", "S", ")" }); //δ(q,ε,L) = ((,S,))

                            npda.Debug();
                            Console.WriteLine("\nEnter the line :");
                            // Example: v+v
                            //          v*(v+v)
                            Console.WriteLine(npda.Execute(Console.ReadLine()).ToString());

                            break;
                        }


                    // 7.5 - МП-автомат, пример 5.
                    // Демонстрирует работу МП-автомата для выражения вида i @ i.
                    case "7.5":
                        {
                            // i @ i
                            // S -> F@L
                            // F -> i
                            // L -> i
                            var pda = new PDA(new List<Symbol>() { "q0", "q1", "q2", "qf" },
                              new List<Symbol>() { "i", "@" },
                              new List<Symbol>() { "z0", "i", },
                              "q0",
                              "z0",
                              new List<Symbol>() { "qf" });
                            pda.addDeltaRule("q0", "i", "z0", new List<Symbol>() { "q1" },
                              new List<Symbol>() { "i", "z0" }); //δ(q0,i,z0) = (i,z0)
                            pda.addDeltaRule("q", "@", "i", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" }); //δ(q,@,i) = (ε)
                            pda.addDeltaRule("q", "i", "i", new List<Symbol>() { "q" },
                              new List<Symbol>() { "i", "i" }); //δ(q,i,i) = (i, i)
                            pda.addDeltaRule("q", "ε", "i", new List<Symbol>() { "qf" }, new List<Symbol>() { "ε" }); //δ(q,ε,i) = (ε)
                            pda.Debug();
                            Console.WriteLine("Example: i@i\n");
                            Console.WriteLine(pda.Execute("i@i"));
                            break;

                        }


                    // 7.6 - Проверка работы МП-автомата.
                    // Тестирует корректность работы магазинного автомата на примере.
                    case "7.6": //проверка PDA 
                        var pda1 = new PDA(
                          new List<Symbol>() { "q0", "q", "qf" },
                          new List<Symbol>() { "m", "h" },
                          new List<Symbol>() { "m", "h", "z0", "F", "D" },
                          "q0",
                          "z0",
                          new List<Symbol>() { "qf" });
                        pda1.addDeltaRule("q0", "ε", "z0", new List<Symbol>() { "q" }, new List<Symbol>() { "F", "D" });
                        pda1.addDeltaRule("q", "ε", "D", new List<Symbol>() { "q" }, new List<Symbol>() { "m" });
                        pda1.addDeltaRule("q", "ε", "F", new List<Symbol>() { "q" }, new List<Symbol>() { "h" });
                        pda1.addDeltaRule("q", "h", "h", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" });
                        pda1.addDeltaRule("q", "m", "m", new List<Symbol>() { "qf" }, new List<Symbol>() { "ε" });
                        pda1.Debug();

                        Console.WriteLine("Example: hm\n");
                        Console.WriteLine(pda1.Execute(Console.ReadLine()).ToString());
                        break;

                    // 7.10 - Конфигурируемый МП-автомат: недетерминированный МПА.
                    // Демонстрирует работу конфигурируемого недетерминированного магазинного автомата
                    // с настраиваемой логикой выполнения переходов.
                    /* Конфигурируемый МП-автомат: недетерминированный МПА */
                    case "7.10":
                        {
                            // NOTE: Распознаёт любой набор из 0 и 1
                            CPDA<DeltaQSigmaGamma> cpda = new CPDA<DeltaQSigmaGamma>(
                              new List<Symbol>() { "q", "p", "r" },
                              new List<Symbol>() { "0", "1" },
                              new List<Symbol>() { "0", "1", "z" },
                              "q", "z",
                              new List<Symbol>() { "r" }
                            );

                            cpda.AddRule(("q", "0", "ε", new List<Symbol>() { "q" }, new List<Symbol>() { "0" }));
                            cpda.AddRule(("q", "1", "ε", new List<Symbol>() { "q" }, new List<Symbol>() { "1" }));
                            cpda.AddRule(("q", "ε", "ε", new List<Symbol>() { "p" }, new List<Symbol>() { "ε" }));

                            cpda.AddRule(("p", "0", "0", new List<Symbol>() { "p" }, new List<Symbol>() { "ε" }));
                            cpda.AddRule(("p", "1", "1", new List<Symbol>() { "p" }, new List<Symbol>() { "ε" }));
                            cpda.AddRule(("p", "ε", "z", new List<Symbol>() { "r" }, new List<Symbol>() { "ε" }));

                            cpda.SetUp((Queue<(Symbol, int, List<Symbol>)> configs, (Symbol q, int i, List<Symbol> pdl) config,
                              string chain) =>
                            {
                                // NOTE: Accept if the whole chain was read AND the current state is accepting (belongs to F)
                                if (config.i == chain.Length - 1 && cpda.F.Contains(config.q))
                                    return true;

                                foreach (DeltaQSigmaGamma rule in cpda.Delta)
                                {
                                    // NOTE: If current state does not match rule's state, skip
                                    if (rule.LHSQ != config.q)
                                        continue;

                                    // NOTE: If current input symbol does not match rule's symbol AND it is not epsilon-move, skip
                                    if (!((config.i < chain.Length - 1 && chain[config.i].ToString().Equals(rule.LHSS.symbol)) ||
                          rule.LHSS.IsEpsilon()))
                                    {
                                        continue;
                                    }

                                    // NOTE: Skip rules that make stack empty
                                    if (config.pdl.Count < 1)
                                        continue;

                                    int new_i = config.i;
                                    List<Symbol> new_pdl = new List<Symbol>(config.pdl);

                                    // NOTE: Pop symbol from the stack if LHSZ is not epsilon
                                    if (!rule.LHSZ.IsEpsilon())
                                        new_pdl.RemoveAt(new_pdl.Count - 1);

                                    // NOTE: Push symbols on to the stack if RHSZ is not epsilon
                                    if (!rule.RHSZ.First().IsEpsilon())
                                        new_pdl.AddRange(Enumerable.Reverse(rule.RHSZ));

                                    // NOTE: Move reading head one symbol to the right if the input symbol is not epsilon
                                    if (!rule.LHSS.IsEpsilon())
                                        ++new_i;

                                    // NOTE: Put new configurations in the queue for further execution
                                    configs.Enqueue((rule.RHSQ.First(), new_i, new_pdl));
                                }

                                return false;
                            });

                            string[] tests =
                            {
              "0",
              "1",
              "00",
              "0101",
              "0110",
              "10101",
              "0012100",
              "011001",
              "111111",
              "0110110",
            };

                            cpda.Debug();
                            foreach (string test in tests)
                                Console.WriteLine("{0} {1} recognised by custom PDA", test, cpda.Execute(test) ? "was" : "was NOT");
                        }
                        break;

                    // 7.11 - Расширенный МП-автомат (детерминированный).
                    // Демонстрирует работу расширенного детерминированного МП-автомата
                    // для распознавания арифметических выражений.
                    case "7.11":
                        {
                            /* Расширенный МП-автомат (детерминированный) */
                            EDPDA epda = new EDPDA(new List<Symbol>() { "q, r" },
                              new List<Symbol>() { "w", "v", "+", "*", "(", ")" },
                              new List<Symbol>() { "w", "v", "+", "*", "(", ")", "S0", "E", "F", "L", "⊥" },
                              "q", "⊥",
                              new List<Symbol>() { "r" });

                            epda.addDeltaRule("q", "w", new List<Symbol>() { "ε" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "w" });
                            epda.addDeltaRule("q", "v", new List<Symbol>() { "ε" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "v" });
                            epda.addDeltaRule("q", "+", new List<Symbol>() { "ε" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "+" });
                            epda.addDeltaRule("q", "*", new List<Symbol>() { "ε" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "*" });
                            epda.addDeltaRule("q", "(", new List<Symbol>() { "ε" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "(" });
                            epda.addDeltaRule("q", ")", new List<Symbol>() { "ε" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { ")" });

                            epda.addDeltaRule("q", "ε", new List<Symbol>() { "(", "E", ")", "*", "L" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "S0" });
                            epda.addDeltaRule("q", "ε", new List<Symbol>() { "E", "+", "F" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "E" });
                            epda.addDeltaRule("q", "ε", new List<Symbol>() { "F" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "S0" });
                            epda.addDeltaRule("q", "ε", new List<Symbol>() { "w" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "E" });
                            epda.addDeltaRule("q", "ε", new List<Symbol>() { "F", "*", "L" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "F" });
                            epda.addDeltaRule("q", "ε", new List<Symbol>() { "L" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "F" });
                            epda.addDeltaRule("q", "ε", new List<Symbol>() { "v" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "L" });
                            epda.addDeltaRule("q", "ε", new List<Symbol>() { "⊥", "S0" }, new List<Symbol>() { "r" },
                              new List<Symbol>() { "ε" });

                            string[] tests =
                            {
              "(w+v)*v",
              "v+v",
              "v+(w+v)*v",
              "v",
              "w*w"
            };

                            epda.Debug();
                            foreach (string test in tests)
                                Console.WriteLine("{0} {1} recognised by EPDA\n", test, epda.Execute(test) ? "was" : "was NOT");
                        }
                        break;

                    // 7.12 - Конфигурируемый МП-автомат: недетерминированный РМПА.
                    // Демонстрирует работу конфигурируемого недетерминированного расширенного МП-автомата
                    // с поддержкой чтения нескольких символов со стека за один переход.
                    case "7.12":
                        {
                            /* Конфигурируемый МП-автомат: недетерминированный РМПА */

                            CPDA<DeltaQSigmaGammaEx> cpda = new CPDA<DeltaQSigmaGammaEx>(
                              new List<Symbol>() { "q, r" },
                              new List<Symbol>() { "a", "+", "*", "(", ")" },
                              new List<Symbol>() { "a", "+", "*", "(", ")", "S", "F", "L", "Z" },
                              "q", "Z",
                              new List<Symbol>() { "r" }
                            );

                            cpda.AddRule(("q", "a", new List<Symbol>() { "ε" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "a" }));
                            cpda.AddRule(("q", "+", new List<Symbol>() { "ε" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "+" }));
                            cpda.AddRule(("q", "*", new List<Symbol>() { "ε" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "*" }));
                            cpda.AddRule(("q", "(", new List<Symbol>() { "ε" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "(" }));
                            cpda.AddRule(("q", ")", new List<Symbol>() { "ε" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { ")" }));

                            cpda.AddRule(("q", "ε", new List<Symbol>() { "S", "+", "L" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "S" }));
                            cpda.AddRule(("q", "ε", new List<Symbol>() { "F" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "S" }));
                            cpda.AddRule(("q", "ε", new List<Symbol>() { "F", "*", "L" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "F" }));
                            cpda.AddRule(("q", "ε", new List<Symbol>() { "L" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "F" }));
                            cpda.AddRule(("q", "ε", new List<Symbol>() { "a" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "L" }));
                            cpda.AddRule(("q", "ε", new List<Symbol>() { "(", "S", ")" }, new List<Symbol>() { "q" },
                              new List<Symbol>() { "L" }));
                            cpda.AddRule(("q", "ε", new List<Symbol>() { "Z", "S" }, new List<Symbol>() { "r" },
                              new List<Symbol>() { "ε" }));

                            cpda.SetUp((Queue<(Symbol, int, List<Symbol>)> configs, (Symbol q, int i, List<Symbol> pdl) config,
                              string chain) =>
                            {
                                if (config.i == chain.Length - 1 && cpda.F.Contains(config.q))
                                    return true;

                                foreach (DeltaQSigmaGammaEx rule in cpda.Delta)
                                {
                                    if (rule.LHSQ != config.q)
                                        continue;

                                    if (!((config.i < chain.Length - 1 && chain[config.i].ToString().Equals(rule.LHSS.symbol)) ||
                          rule.LHSS.IsEpsilon()))
                                    {
                                        continue;
                                    }

                                    int size = rule.LHSZX.Count;
                                    int index = config.pdl.Count - size;

                                    if (index < 0)
                                        continue;

                                    int new_i = config.i;
                                    List<Symbol> new_pdl = new List<Symbol>(config.pdl);

                                    if (!rule.LHSZX.First().IsEpsilon())
                                    {
                                        for (int i = 0; i < size; ++i)
                                        {
                                            if (new_pdl[index + i] != rule.LHSZX[i])
                                                goto skip;
                                        }

                                        new_pdl.RemoveRange(index, size);
                                    }

                                    if (!rule.RHSZ.First().IsEpsilon())
                                        new_pdl.AddRange(rule.RHSZ);

                                    if (!rule.LHSS.IsEpsilon())
                                        ++new_i;

                                    configs.Enqueue((rule.RHSQ.First(), new_i, new_pdl));

                                skip:
                                    continue;
                                }

                                return false;
                            });

                            string[] tests =
                            {
              "(a*a)",
              "a+(a+a)",
              "a+a+a",
              "(a+)",
              "a+(a*a)+a+a+(a+a)"
            };

                            cpda.Debug();
                            foreach (string test in tests)
                                Console.WriteLine("{0} {1} recognised by custom PDA", test, cpda.Execute(test) ? "was" : "was NOT");
                        }
                        break;


                    // 9.1 - LL(1) анализатор: построение управляющей таблицы и разбор.
                    // Демонстрирует работу LL(1) анализатора с построением управляющей таблицы M
                    // и аналитическим разбором выведенной цепочки.
                    case "9.1":
                        {
                            // LL Разбор
                            var LL = new Grammar(new List<Symbol>() { "i", "(", ")", "+", "*" },
                              new List<Symbol>() { "S", "F", "L" },
                              "S");

                            LL.AddRule("S", new List<Symbol>() { "(", "F", "+", "L", ")" });
                            LL.AddRule("F", new List<Symbol>() { "*", "L" });
                            LL.AddRule("F", new List<Symbol>() { "i" });
                            LL.AddRule("L", new List<Symbol>() { "F" });

                            var parser = new LLParser(LL);
                            Console.WriteLine("Пример вводимых строк: (i+i), (i+*i)");
                            Console.WriteLine("Введите строку: ");
                            string stringChain = Console.ReadLine();

                            var chain = new List<Symbol> { };
                            foreach (var x in stringChain)
                                chain.Add(new Symbol(x.ToString()));
                            if (parser.Parse(chain))
                            {
                                Console.WriteLine("Допуск. Цепочка символов = L(G).");
                                Console.WriteLine(parser.OutputConfigure);
                            }
                            else
                            {
                                Console.WriteLine("Не допуск. Цепочка символов не = L(G).");
                            }

                            break;
                        }

                    // 9.2 - LL(1) анализатор с подробным разбором.
                    // Демонстрирует работу LL(1) анализатора с детальным выводом всех тактов работы.
                    case "9.2":
                        {
                            // LL Разбор
                            var LL1 = new Grammar(new List<Symbol>() { "i", "&", "^", "(", ")", "" },
                              new List<Symbol>() { "S", "S'", "F" },
                              "S");

                            LL1.AddRule("S", new List<Symbol>() { "(", "S'" });
                            LL1.AddRule("S'", new List<Symbol>() { "F", "^", "F", ")" });
                            LL1.AddRule("S'", new List<Symbol>() { "S", ")" });
                            LL1.AddRule("F", new List<Symbol>() { "&", "F" });
                            LL1.AddRule("F", new List<Symbol>() { "i" });

                            var parser1 = new LLParser(LL1);
                            Console.WriteLine("Введите строку: ");
                            string stringChain = Console.ReadLine();

                            var chain = new List<Symbol> { };
                            foreach (var x in stringChain)
                                chain.Add(new Symbol(x.ToString()));
                            if (parser1.Parse1(chain))
                            {
                                Console.WriteLine("Допуск. Цепочка символов = L(G).");
                                Console.WriteLine(parser1.OutputConfigure);
                            }
                            else
                            {
                                Console.WriteLine("Не допуск. Цепочка символов не = L(G).");
                            }

                            break;
                        }

                    // 14 - Построение канонической формы множества ситуаций для LR-анализа.
                    // Строит управляющую таблицу для функции перехода g(X) и действий f(u).
                    case "14":
                        var LR0Grammar = new SLRGrammar(new List<Symbol>() { "i", "j", "&", "^", "(", ")" },
                          new List<Symbol>() { "S", "F", "L" },
                          new List<Production>(),
                          "S");

                        LR0Grammar.AddRule("S", new List<Symbol>() { "F", "^", "L" });
                        LR0Grammar.AddRule("S", new List<Symbol>() { "(", "S", ")" });
                        LR0Grammar.AddRule("F", new List<Symbol>() { "&", "L" });
                        LR0Grammar.AddRule("F", new List<Symbol>() { "i" });
                        LR0Grammar.AddRule("L", new List<Symbol>() { "j" });

                        LR0Grammar.Construct();
                        LR0Grammar.Inference();
                        break;

                    // 16.1 - LR(0) анализатор с использованием функций g(X) и f(a).
                    // Демонстрирует работу LR(0) анализатора с построением управляющей таблицы.
                    case "16.1":
                        {
                            var parser = new MyLRParser();
                            parser.ReadGrammar();
                            parser.Execute();
                            break;
                        }

                    // 16.2 - LR(0) анализатор: пример использования.
                    // Демонстрирует работу LR(0) анализатора на конкретном примере грамматики.
                    case "16.2":
                        {
                            var parser = new MyLRParser();
                            Console.WriteLine("Пример ввода продукций:");
                            parser.Example();
                            parser.Execute();
                            break;
                        }

                    // 16.3 - LR(1) анализатор с использованием функций g(X) и f(a).
                    // Демонстрирует работу LR(1) анализатора с построением управляющей таблицы.
                    case "16.3":
                        {
                            var parser = new MyLRParser();
                            parser.ReadGrammar();
                            parser.Execute_LR1();
                            break;
                        }

                    // 16.4 - LR(1) анализатор: пример использования.
                    // Демонстрирует работу LR(1) анализатора на конкретном примере грамматики.
                    case "16.4":
                        {
                            var parser = new MyLRParser();
                            Console.WriteLine("Пример ввода продукций:");
                            parser.Example_LR1();
                            parser.Execute_LR1();
                            break;
                        }

                    // Курсовой проект I1: Теория перевода - SDT-схемы (Syntax-Directed Translation).

                    // Демонстрирует работу синтаксически управляемого транслятора (SDT-схемы)
                    // для преобразования входных цепочек в выходные по заданным правилам трансляции.
                    // 
                    // SDT-схема - это расширение контекстно-свободной грамматики, где каждому правилу
                    // грамматики сопоставлена пара цепочек: входная (синтаксическая) и выходная (семантическая).
                    // 
                    // Основные концепции:
                    // - Входной алфавит: символы входного языка
                    // - Выходной алфавит: символы выходного языка
                    // - Правила трансляции: (входная_цепочка, выходная_цепочка)
                    // 
                    // Проект демонстрирует:
                    // 1. Создание SDT-схемы с правилами трансляции
                    // 2. Трансляцию входных цепочек в выходные
                    // 3. Гомоморфизмы - специальный случай трансляции, где каждый символ
                    //    входного алфавита заменяется на соответствующий символ выходного алфавита
                    // 
                    // Применение: компиляторы, трансляторы, преобразователи форматов данных.
                    case "I1": // SDT
                        try
                        {
                            var sdt = new MySDTSchemata(new List<Symbol>() { "S", "A" },
                              new List<Symbol>() { "0", "1" },
                              new List<Symbol>() { "a", "b" },
                              "S");

                            sdt.AddRule(new Symbol("S"),
                              new List<Symbol>() { "0", "A", "S" },
                              new List<Symbol>() { "S", "A", "a" });
                            sdt.AddRule(new Symbol("A"),
                              new List<Symbol>() { "0", "S", "A" },
                              new List<Symbol>() { "A", "S", "a" });
                            sdt.AddRule(new Symbol("S"),
                              new List<Symbol>() { "1" },
                              new List<Symbol>() { "b" });
                            sdt.AddRule(new Symbol("A"),
                              new List<Symbol>() { "1" },
                              new List<Symbol>() { "b" });

                            Console.Write("\nDebug SDTranslator:");
                            sdt.DebugSDTS();

                            sdt.Translate(new List<Symbol>() { "0", "0", "1", "0", "1", "1", "1" });

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"\nОшибка: {e.Message}");
                        }

                        // Homomorphism
                        try
                        {
                            var h_table = new myHTable(new List<Symbol>() { "0", "1" },
                              new List<Symbol>() { "1", "0" });

                            Console.WriteLine("\nDebug Homomorphism:");
                            h_table.debugHTable();

                            Console.WriteLine("\nInput chain:");
                            var r = new List<Symbol>() { "0", "1", "0", "0", "1", "1" };
                            Console.WriteLine(Utility.convert(r));
                            Console.WriteLine("\nTranslation:");
                            Console.WriteLine(h_table.h(r));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"\nОшибка: {e.Message}");
                        }

                        try
                        {
                            var sdt = new MySDTSchemata(new List<Symbol>() { "S" },
                              new List<Symbol>() { "+", "i" },
                              new List<Symbol>() { "+", "i" },
                              "S");

                            sdt.AddRule(new Symbol("S"),
                              new List<Symbol>() { "+", "S_1", "S_2" },
                              new List<Symbol>() { "S_2", "+", "S_1" });

                            sdt.AddRule(new Symbol("S"),
                              new List<Symbol>() { "i" },
                              new List<Symbol>() { "i" });

                            Console.Write("\nDebug SDTranslator:");
                            sdt.DebugSDTS();

                            sdt.Translate(new List<Symbol>() { "+", "+", "+", "i", "i", "i", "i" });

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"\nОшибка: {e.Message}");
                        }

                        break;

                    // Курсовой проект I2: Преобразование КС-грамматики в транслирующую с операционными символами.

                    // Демонстрирует алгоритм преобразования контекстно-свободной грамматики
                    // в транслирующую грамматику путем добавления операционных символов.
                    // 
                    // Транслирующая грамматика - это грамматика, которая не только описывает
                    // синтаксис языка, но и генерирует выходную цепочку (код, промежуточное представление и т.д.).
                    // 
                    // Алгоритм преобразования:
                    // 1. Берется исходная КС-грамматика
                    // 2. К каждому правилу добавляются операционные символы, которые генерируют
                    //    выходную цепочку на основе структуры входной цепочки
                    // 3. Операционные символы выполняются во время разбора, генерируя выход
                    // 
                    // Пример: грамматика арифметических выражений преобразуется в грамматику,
                    // которая генерирует постфиксную запись или код для стековой машины.
                    // 
                    // Применение: генераторы компиляторов, трансляторы, преобразователи синтаксиса.

                    case "I2":
                        var inputGrammar = new Grammar(new List<Symbol>() { "i", "+", "*", "(", ")" },
                          new List<Symbol>() { "E", "T", "P" },
                          new List<Production>(),
                          "E");

                        inputGrammar.AddRule("E", new List<Symbol> { "T", "+", "E" });
                        inputGrammar.AddRule("E", new List<Symbol> { "T" });
                        inputGrammar.AddRule("T", new List<Symbol> { "P", "*", "T" });
                        inputGrammar.AddRule("T", new List<Symbol> { "P" });
                        inputGrammar.AddRule("P", new List<Symbol> { "i" });
                        inputGrammar.AddRule("P", new List<Symbol> { "(", "E", ")" });

                        var converter = new ConverterInTransGrammar(new List<Symbol>() { "i", "+", "*", "(", ")" },
                          new List<Symbol>() { "E", "T", "P" },
                          new List<Production>(),
                          "E");

                        converter.AddRule("E", new List<Symbol> { "T", "+", "E" });
                        converter.AddRule("E", new List<Symbol> { "T" });
                        converter.AddRule("T", new List<Symbol> { "P", "*", "T" });
                        converter.AddRule("T", new List<Symbol> { "P" });
                        converter.AddRule("P", new List<Symbol> { "i" });
                        converter.AddRule("P", new List<Symbol> { "(", "E", ")" });

                        converter.Construct();
                        var tgrm = new TransGrammar();
                        tgrm = converter.ConvertInTransGrammar(inputGrammar, "i+i*i", "iii*+");

                        break;


                    // Курсовой проект I3: Преобразование грамматики в AT-грамматику (Attribute Grammar).

                    // Демонстрирует преобразование обычной контекстно-свободной грамматики
                    // в атрибутную грамматику (AT-Grammar) с добавлением атрибутов к символам.
                    // 
                    // Атрибутная грамматика расширяет КС-грамматику:
                    // - Каждый символ может иметь атрибуты (наследуемые и синтезируемые)
                    // - К правилам добавляются функции вычисления атрибутов
                    // - Атрибуты позволяют передавать информацию между узлами дерева разбора
                    // 
                    // Алгоритм преобразования:
                    // 1. Берется исходная КС-грамматика
                    // 2. К символам добавляются атрибуты (например, "p", "q", "r" для значений)
                    // 3. Определяются функции вычисления атрибутов на основе операций грамматики
                    // 4. Создается AT-грамматика, которая может вычислять значения выражений
                    // 
                    // Применение: семантический анализ, вычисление значений выражений,
                    // проверка типов, генерация кода.

                    case "I3":
                        {
                            // ATGrammar(V,T,OP,S,P)
                            var atgr = new ATGrammar(new List<Symbol>() { "P", "E", "T", "S" },
                              new List<Symbol>() { "*", "+", "(", ")", "c" },
                              new List<Symbol_Operation>(), "S");

                            //правила для грамматики
                            atgr.Addrule("S", new List<Symbol>() { "E" });
                            atgr.Addrule("E", new List<Symbol>() { "E", "+", "T" });
                            atgr.Addrule("E", new List<Symbol>() { "T" });
                            atgr.Addrule("T", new List<Symbol>() { "T", "*", "P" });
                            atgr.Addrule("T", new List<Symbol>() { "P" });
                            atgr.Addrule("P", new List<Symbol>() { "c" });
                            atgr.Addrule("P", new List<Symbol>() { "(", "E", ")" });

                            atgr.NewAT(new List<Symbol>() { "p", "q", "r" }, new List<Symbol>() { "*", "+" },
                              new List<Symbol>() { "c" });

                            atgr.Print();
                            break;
                        }

                    // Курсовой проект I4: Работа с AT-грамматикой - атрибуты и операции.

                    // Демонстрирует работу атрибутной грамматики с вычислением атрибутов
                    // и применением операционных символов для генерации результата.
                    // 
                    // В отличие от I3, здесь показывается полный цикл работы AT-грамматики:
                    // 1. Определение атрибутов для символов (наследуемые и синтезируемые)
                    // 2. Задание функций вычисления атрибутов для правил
                    // 3. Использование операционных символов для генерации выходной информации
                    // 4. Вычисление значений атрибутов при разборе входной цепочки
                    // 
                    // Пример: грамматика арифметических выражений с вычислением результата.
                    // Атрибуты хранят значения подвыражений, функции вычисляют результат операций,
                    // операционные символы выводят промежуточные и финальные результаты.
                    // 
                    // Применение: интерпретаторы, калькуляторы, системы вычисления выражений.

                    case "I4":
                        {
                            // ATGrammar(V,T,OP,S,P)
                            // S, Er    *, +, cr     {ANS}r
                            var atgr = new ATGrammar(
                              new List<Symbol>() { "S", new Symbol("E", new List<Symbol>() { "r" }) },
                              new List<Symbol>() { "*", "+", new Symbol("c", new List<Symbol>() { "r" }) },
                              new List<Symbol_Operation>() { new Symbol_Operation("{ANS}", new List<Symbol>() { "r" }) },
                              new Symbol("S"));
                            atgr.Addrule(new Symbol("S"), // LHS        LHS -> RHS  
                              new List<Symbol>()
                              {
                // RHS
                new Symbol("E", // S -> Ep {ANS}r r -> p
                  new List<Symbol>() { "p" }),
                new Symbol_Operation("{ANS}", new List<Symbol>() { "r" })
                              },
                              new List<AttrFunction>()
                              {
                new AttrFunction(new List<Symbol>() { "r" }, new List<Symbol> { "p" })
                              }
                            );

                            atgr.Addrule(new Symbol("E", new List<Symbol>() { "p" }), // Ep -> +EpEr p -> q + r
                              new List<Symbol>()
                              {
                "+", new Symbol("E", new List<Symbol>() { "p" }),
                new Symbol("E", new List<Symbol>() { "r" })
                              },
                              new List<AttrFunction>()
                              {
                new AttrFunction(new List<Symbol>()
                {
                  "p"
                }, new List<Symbol> { "q", "+", "r" })
                              });

                            atgr.Addrule(new Symbol("E", new List<Symbol>() { "p" }), // Ep -> *EpEr   p -> q * r
                              new List<Symbol>()
                                { "*", new Symbol("E", new List<Symbol>() { "p" }), new Symbol("E", new List<Symbol>() { "r" }) },
                              new List<AttrFunction>()
                              {
                new AttrFunction(new List<Symbol>()
                {
                  "p"
                }, new List<Symbol> { "q", "+", "r" })
                              });

                            atgr.Addrule(new Symbol("E", new List<Symbol>() { "p" }), // Ep -> Cr   p -> r
                              new List<Symbol>() { new Symbol("C", new List<Symbol>() { "r" }) },
                              new List<AttrFunction>()
                              {
                new AttrFunction(new List<Symbol>()
                {
                  "p"
                }, new List<Symbol> { "r" })
                              });

                            atgr.Print();

                            atgr.transform(); //преобразование в простую форму присваивания

                            Console.WriteLine("\nPress Enter to show result\n");
                            Console.ReadLine();

                            atgr.Print();
                            Console.WriteLine("\nPress Enter to end\n");
                            Console.ReadLine();
                            break;
                        }

                    // I5 - AT-грамматика для типов переменных Python.
                    // Демонстрирует использование атрибутной грамматики для обработки
                    // объявлений переменных с определением их типов в стиле Python.
                    case "I5":
                        {
                            var atgr = new ATGrammar(
                              new List<Symbol>() { "D", new Symbol("L", new List<Symbol>() { "r" }), "E", "F" },
                              new List<Symbol>()
                                { "=", ",", new Symbol("i", new List<Symbol>() { "b" }), new Symbol("n", new List<Symbol>() { "e" }) },
                              new List<Symbol_Operation>()
                              {
                new Symbol_Operation("{type}", new List<Symbol>() { "a", "c" }),
                new Symbol_Operation("{push}", new List<Symbol>() { "k" }),
                new Symbol_Operation("{pull}", new List<Symbol>() { "t" }),
                new Symbol_Operation("{stack}", new List<Symbol>() { "u" })
                              },
                              new Symbol("D"));
                            atgr.Addrule(new Symbol("D"), // LHS
                              new List<Symbol>()
                              {
                // RHS
                new Symbol("i", // D -> i_b {тип}a,c Lr   a <- b c <- r
                  new List<Symbol>() { "b" }),
                new Symbol("{type}", new List<Symbol>() { "a", "c" }),
                new Symbol("L",
                  new List<Symbol>() { "r" })
                              },

                              new List<AttrFunction>()
                              {
                new AttrFunction(new List<Symbol>() { "a" }, new List<Symbol> { "b" }),
                new AttrFunction(new List<Symbol>() { "c" }, new List<Symbol> { "r" })
                              }
                            );

                            atgr.Addrule(new Symbol("L",
                                new List<Symbol>() { new Symbol("r") }), // Lr -> n_e    r <- e
                              new List<Symbol>()
                              {
                new Symbol("n",
                  new List<Symbol>() { "e" })
                              },

                              new List<AttrFunction>()
                              {
                new AttrFunction(new List<Symbol>() { "r" }, new List<Symbol> { "e" })
                              });

                            atgr.Addrule(new Symbol("L", new List<Symbol>() { "r" }),
                              new List<Symbol>()
                              {
                new Symbol("i",
                  new List<Symbol>() { "b" }),
                new Symbol("{type}",
                  new List<Symbol>() { "a", "c" })
                              },

                              new List<AttrFunction>()
                              {
                new AttrFunction(new List<Symbol>() { "a" }, new List<Symbol> { "b" }),
                new AttrFunction(new List<Symbol>() { "r" }, new List<Symbol> { "c" })
                              });

                            atgr.Addrule(new Symbol("L", new List<Symbol>() { "r" }),
                              new List<Symbol>()
                              {
                ",",
                new Symbol("i",
                  new List<Symbol>() { "b" }),
                new Symbol("{type}",
                  new List<Symbol>() { "a", "c" }),
                new Symbol("{pull}",
                  new List<Symbol>() { "t" }),
                new Symbol("{pull}",
                  new List<Symbol>() { "p" }),
                "E"
                              },


                              new List<AttrFunction>()
                              {
                new AttrFunction(new List<Symbol>() { "a" }, new List<Symbol> { "b" }),
                new AttrFunction(new List<Symbol>() { "c" }, new List<Symbol> { "t" }),
                new AttrFunction(new List<Symbol>() { "r" }, new List<Symbol> { "p" })
                              });

                            atgr.Addrule(new Symbol("E"),
                              new List<Symbol>()
                              {
                ",",
                new Symbol("i",
                  new List<Symbol>() { "b" }),
                new Symbol("{type}",
                  new List<Symbol>() { "a", "c" }),
                new Symbol("{pull}",
                  new List<Symbol>() { "t" }),
                "E"
                              },

                              new List<AttrFunction>()
                              {
                new AttrFunction(new List<Symbol>() { "a" }, new List<Symbol> { "b" }),
                new AttrFunction(new List<Symbol>() { "c" }, new List<Symbol> { "t" })
                              });

                            atgr.Addrule(new Symbol("E"),
                              new List<Symbol>()
                              {
                "=",
                new Symbol("i",
                  new List<Symbol>() { "b" }),
                new Symbol("{type}",
                  new List<Symbol>() { "a", "c" }),
                new Symbol("{push}",
                  new List<Symbol>() { "k" }),
                "F"
                              },

                              new List<AttrFunction>()
                              {
                new AttrFunction(new List<Symbol>() { "a" }, new List<Symbol> { "" }),
                new AttrFunction(new List<Symbol>() { "k" }, new List<Symbol> { "c" })
                              });

                            atgr.Addrule(new Symbol("F"),
                              new List<Symbol>()
                              {
                ",",
                new Symbol("i",
                  new List<Symbol>() { "b" }),
                new Symbol("{type}",
                  new List<Symbol>() { "a", "c" }),
                new Symbol("{push}",
                  new List<Symbol>() { "k" }),
                "F"
                              },

                              new List<AttrFunction>()
                              {
                new AttrFunction(new List<Symbol>() { "a" }, new List<Symbol> { "b" }),
                new AttrFunction(new List<Symbol>() { "k" }, new List<Symbol> { "c" })
                              });

                            atgr.Addrule(new Symbol("F"),
                              new List<Symbol>()
                              {
                new Symbol("{stack}",
                  new List<Symbol>() { "u" })
                              },


                              new List<AttrFunction>()
                              {
                new AttrFunction(new List<Symbol>() { "u" }, new List<Symbol> { "1" })
                              });

                            atgr.Print();

                            atgr.transform();

                            Console.WriteLine("\nPress Enter to show result\n");
                            Console.ReadLine();

                            atgr.Print();
                            Console.WriteLine("\nPress Enter to end\n");
                            Console.ReadLine();
                            break;
                        }
                    case "I6":
                        {
                            var atgr = new ATGrammar(
                              new List<Symbol>() { "A", "B", "C", "D", "E", "F", "S" },
                              new List<Symbol>()
                                { "for", "+", "(", ")", "int", "=", "id", ";", "<", "const2", "const1", "const3", "{", "body", "}" },
                              new List<Symbol_Operation>()
                              {
                new Symbol_Operation("D->{ OUT1:= ' for idn1 in range(const1, '}", new List<Symbol>() { "r" }),
                new Symbol_Operation("E->{OUT2:= 'const2,'}", new List<Symbol>() { "r" }),
                new Symbol_Operation("F->{OUT1:= 'const3): '}", new List<Symbol>() { "r" }),
                new Symbol_Operation("B->{ OUT1:= ' body '}", new List<Symbol>() { "r" }),
                new Symbol_Operation("C->{ if n3 > n4 then Error}", new List<Symbol>() { "n3", "n4" })
                              },
                              new Symbol("S"));

                            atgr.Addrule("S0", new List<Symbol>() { "S", "{ OUT1 := OUT1 || OUT2 }" });
                            atgr.Addrule("S", new List<Symbol>() { "A", "B", "C" });
                            atgr.Addrule("A", new List<Symbol>() { "for", "(", "D", "E", "F" });
                            atgr.Addrule("B", new List<Symbol>() { "body", "{OUT1:= ' body'}" });
                            atgr.Addrule("C", new List<Symbol>() { "}" });
                            atgr.Addrule("D",
                              new List<Symbol>()
                              {
                "int", "id", "=", "const1", ";",
                new Symbol("{ OUT1:= ' for id_a in range(const1_n, '}", new List<Symbol>() { "n" })
                              });
                            atgr.Addrule("E",
                              new List<Symbol>()
                                { "id", "<", "const2", ";", new Symbol("{OUT2:= 'const2_p,'}", new List<Symbol>() { "p" }) });
                            atgr.Addrule("F",
                              new List<Symbol>()
                                { "id", "+", "const3", ")", "{", new Symbol("{OUT2:= 'const3_p): '}", new List<Symbol>() { "p" }) });

                            atgr.ATG_C_Py(new List<Symbol>() { "s", "a", "n", "p", "q", "r" }, new List<Symbol>() { "=", "<", "+" });

                            atgr.Print();

                            Console.WriteLine("\nPress Enter to end\n");
                            Console.ReadLine();
                            break;
                        }

                    // I7 - Пример цепочечного перевода.
                    // Демонстрирует цепочечный перевод выражений из инфиксной записи в постфиксную
                    // с использованием синтаксически управляемого перевода.
                    case "I7":
                        {
                            /* Пример "цепочечного" перевода  11 12 13
                            Грамматика транслирует выражения из инфиксной записи в постфиксную
                            Выражения состоят из i, +, * и скобок
                            i+i*i без чисел
                            */
                            var chainPostfix = new SDT.Scheme(new List<SDT.Symbol>() { "i", "+", "*", "(", ")" },
                              new List<SDT.Symbol>() { "E", "E'", "T", "T'", "F" },
                              "E");

                            chainPostfix.AddRule("E", new List<SDT.Symbol>() { "T", "E'" });
                            chainPostfix.AddRule("E'", new List<SDT.Symbol>() { "+", "T", SDT.Actions.Print("+"), "E'" });
                            chainPostfix.AddRule("E'", new List<SDT.Symbol>() { SDT.Symbol.Epsilon });
                            chainPostfix.AddRule("T", new List<SDT.Symbol>() { "F", "T'" });
                            chainPostfix.AddRule("T'", new List<SDT.Symbol>() { "*", "F", SDT.Actions.Print("*"), "T'" });
                            chainPostfix.AddRule("T'", new List<SDT.Symbol>() { SDT.Symbol.Epsilon });
                            chainPostfix.AddRule("F", new List<SDT.Symbol>() { "i", SDT.Actions.Print("i") });
                            chainPostfix.AddRule("F", new List<SDT.Symbol>() { "(", "E", ")" });

                            var chainTranslator = new SDT.LLTranslator(chainPostfix);
                            var inp_str = new SDT.SimpleLexer().Parse(Console.ReadLine());
                            if (chainTranslator.Parse(inp_str))
                            {
                                Console.WriteLine("\nУспех. Строка соответствует грамматике.");
                            }
                            else
                            {
                                Console.WriteLine("\nНе успех. Строка не соответствует грамматике.");
                            }

                            break;
                        }

                    // I8 - L-атрибутивная грамматика для вычисления арифметических выражений.
                    // Демонстрирует использование L-атрибутивной грамматики для вычисления
                    // результата арифметических выражений с целыми числами.
                    case "I8":
                        {
                            /* L-атрибутивная грамматика
                               Грамматика вычисляет результат арифметического выражения
                               Выражения состоят из целых положительных чисел, +, * и скобок
                               1+2*3 без чисел
                            */

                            SDT.Types.Attrs sAttrs = new SDT.Types.Attrs() { ["value"] = 0 };
                            SDT.Types.Attrs lAttrs = new SDT.Types.Attrs() { ["inh"] = 0, ["syn"] = 0 };
                            var lAttrSDT = new SDT.Scheme(
                              new List<SDT.Symbol>() { new SDT.Symbol("number", sAttrs), "+", "*", "(", ")" },
                              new List<SDT.Symbol>()
                              {
                "S", new SDT.Symbol("E", sAttrs), new SDT.Symbol("E'", lAttrs),
                new SDT.Symbol("T", sAttrs), new SDT.Symbol("T'", lAttrs), new SDT.Symbol("F", sAttrs)
                              },
                              "S");

                            lAttrSDT.AddRule("S",
                              new List<SDT.Symbol>() { "E", new SDT.Types.Actions((S) => Console.Write(S["E"]["value"].ToString())) });

                            lAttrSDT.AddRule("E",
                              new List<SDT.Symbol>()
                              {
                "T", new SDT.Types.Actions((S) => S["E'"]["inh"] = S["T"]["value"]), "E'",
                new SDT.Types.Actions((S) => S["E"]["value"] = S["E'"]["syn"])
                              });

                            lAttrSDT.AddRule("E'",
                              new List<SDT.Symbol>()
                              {
                "+", "T", new SDT.Types.Actions((S) => S["E'1"]["inh"] = (int)S["E'"]["inh"] + (int)S["T"]["value"]),
                "E'", new SDT.Types.Actions((S) => S["E'"]["syn"] = S["E'1"]["syn"])
                              });

                            lAttrSDT.AddRule("E'",
                              new List<SDT.Symbol>()
                                { SDT.Symbol.Epsilon, new SDT.Types.Actions((S) => S["E'"]["syn"] = S["E'"]["inh"]) });

                            lAttrSDT.AddRule("T",
                              new List<SDT.Symbol>()
                              {
                "F", new SDT.Types.Actions((S) => S["T'"]["inh"] = S["F"]["value"]), "T'",
                new SDT.Types.Actions((S) => S["T"]["value"] = S["T'"]["syn"])
                              });

                            lAttrSDT.AddRule("T'",
                              new List<SDT.Symbol>()
                              {
                "*", "F", new SDT.Types.Actions((S) => S["T'1"]["inh"] = (int)S["T'"]["inh"] * (int)S["F"]["value"]),
                "T'", new SDT.Types.Actions((S) => S["T'"]["syn"] = S["T'1"]["syn"])
                              });

                            lAttrSDT.AddRule("T'",
                              new List<SDT.Symbol>()
                                { SDT.Symbol.Epsilon, new SDT.Types.Actions((S) => S["T'"]["syn"] = S["T'"]["inh"]) });

                            lAttrSDT.AddRule("F",
                              new List<SDT.Symbol>()
                                { "number", new SDT.Types.Actions((S) => S["F"]["value"] = S["number"]["value"]) });

                            lAttrSDT.AddRule("F",
                              new List<SDT.Symbol>()
                                { "(", "E", ")", new SDT.Types.Actions((S) => S["F"]["value"] = S["E"]["value"]) });

                            SDT.LLTranslator lAttrTranslator = new SDT.LLTranslator(lAttrSDT);
                            if (lAttrTranslator.Parse(new SDT.ArithmLexer().Parse(Console.ReadLine())))
                            {
                                Console.WriteLine("\nУспех. Строка соответствует грамматике.");
                            }
                            else
                            {
                                Console.WriteLine("\nНе успех. Строка не соответствует грамматике.");
                            }

                            break;
                        }

                    // I9 - Перевод на основе дерева разбора.
                    // Демонстрирует построение дерева разбора для арифметических выражений,
                    // его визуализацию и выполнение перевода с использованием дерева.
                    case "I9":
                        {
                            // Капалин Д.С.
                            /* Дерево разбора  1*3 только для умножения
                               Грамматика вычисляет арифметические выражения состоящие из произведений целых положительных числе
                               Дерево разбора печатается на экран, конвертируется в .dot файл и выполняется
                            */

                            var atgr = new ATGrammar(
                              new List<Symbol>() { "S", new Symbol("E", new List<Symbol>() { "r" }) },
                              new List<Symbol>() { "*", new Symbol("c", new List<Symbol>() { "r" }) },
                              new List<Symbol_Operation>() { new Symbol_Operation("{ANS}") },
                              new Symbol("S"));

                            atgr.Addrule(new Symbol("S"), // LHS        LHS -> RHS  
                              new List<Symbol>()
                              {
                // RHS
                new Symbol("E", // S -> Ep {ANS}r r -> p
                  new List<Symbol>() { "p" }),
                new Symbol_Operation("{ANS}")
                              },
                              new List<AttrFunction>()
                              {
                new AttrFunction(new List<Symbol>() { "r" }, new List<Symbol> { "p" })
                              }
                            );

                            atgr.Addrule(new Symbol("E", new List<Symbol>() { "p" }), // Ep -> *EqEr   p -> q * r
                              new List<Symbol>()
                                { new Symbol("E", new List<Symbol>() { "q" }), new Symbol("E", new List<Symbol>() { "r" }), "*" },
                              new List<AttrFunction>()
                              {
                new AttrFunction(new List<Symbol>()
                {
                  "p"
                }, new List<Symbol> { "q", "*", "r" })
                              });

                            atgr.Addrule(new Symbol("E", new List<Symbol>() { "p" }), // Ep -> Cr   p -> r
                              new List<Symbol>() { new Symbol("c", new List<Symbol>() { "r" }) },
                              new List<AttrFunction>()
                              {
                new AttrFunction(new List<Symbol>()
                {
                  "p"
                }, new List<Symbol> { "r" })
                              });

                            atgr.Print();
                            Console.WriteLine("Пример вывода цепочки грамматики:");
                            Console.WriteLine(
                              "S ⇒   E{ANS} ⇒ E * E {ADD} ⇒  E*E*E{ANS} ⇒  c3 * E*E{ANS}⇒  c3*c4*E{ANS} ⇒   c3*c4*c5{ANS} ");
                            Console.WriteLine(
                              "Для построения дерева разбора необходимо будет ввести цепочку вывода в виде последовательности правил");
                            Console.WriteLine("Например: 1 2 2 3 3 3");
                            var a = new ATScheme(atgr, atgr);
                            a.BuildInputTree();
                            a.PrintTree();
                            SDT.Types.Attrs sAttrs2 = new SDT.Types.Attrs() { ["value"] = 0 };
                            SDT.Types.Attrs lAttrs2 = new SDT.Types.Attrs() { ["inh"] = 0, ["syn"] = 0 };
                            SDT.Scheme treeGrammar = new SDT.Scheme(new List<SDT.Symbol>() { new SDT.Symbol("number", sAttrs2), "*" },
                              new List<SDT.Symbol>()
                                { "S", new SDT.Symbol("T", sAttrs2), new SDT.Symbol("T'", lAttrs2), new SDT.Symbol("F", sAttrs2) },
                              "S");

                            SDT.OperationSymbol op1 =
                              new SDT.OperationSymbol(new SDT.Types.Actions((S) => Console.Write(S["T"]["value"].ToString())),
                                "print(T.value)");
                            SDT.OperationSymbol op2 = new SDT.OperationSymbol(
                              new SDT.Types.Actions((S) => S["T'"]["inh"] = S["F"]["value"]), "T'.inh = F.value",
                              new List<string>() { "T'.inh" }, new List<string>() { "F.value" });
                            SDT.OperationSymbol op3 = new SDT.OperationSymbol(
                              new SDT.Types.Actions((S) => S["T"]["value"] = S["T'"]["syn"]), "T.value = T'.syn",
                              new List<string>() { "T.value" }, new List<string>() { "T'.syn" });
                            SDT.OperationSymbol op4 = new SDT.OperationSymbol(
                              new SDT.Types.Actions((S) => S["T'1"]["inh"] = (int)S["T'"]["inh"] * (int)S["F"]["value"]),
                              "T'1.inh = T'.inh * F.value", new List<string>() { "T'1.inh" },
                              new List<string>() { "T'.inh", "F.value" });
                            SDT.OperationSymbol op5 = new SDT.OperationSymbol(
                              new SDT.Types.Actions((S) => S["T'"]["syn"] = S["T'1"]["syn"]), "T'.syn = T'1.syn",
                              new List<string>() { "T'.syn" }, new List<string>() { "T'1.syn" });
                            SDT.OperationSymbol op6 = new SDT.OperationSymbol(
                              new SDT.Types.Actions((S) => S["T'"]["syn"] = S["T'"]["inh"]), "T'.syn = T'.inh",
                              new List<string>() { "T'.syn" }, new List<string>() { "T'.inh" });
                            SDT.OperationSymbol op7 = new SDT.OperationSymbol(
                              new SDT.Types.Actions((S) => S["F"]["value"] = S["number"]["value"]), "F.value = number.value",
                              new List<string>() { "F.value" }, new List<string>() { "number.value" });

                            treeGrammar.AddRule("S", new List<SDT.Symbol>() { "T", op1 });
                            treeGrammar.AddRule("T", new List<SDT.Symbol>() { "F", op2, "T'", op3 });
                            treeGrammar.AddRule("T'", new List<SDT.Symbol>() { "*", "F", op4, "T'", op5 });
                            treeGrammar.AddRule("T'", new List<SDT.Symbol>() { SDT.Symbol.Epsilon, op6 });
                            treeGrammar.AddRule("F", new List<SDT.Symbol>() { "number", op7 });

                            SDT.ParseTreeTranslator treeTr = new SDT.ParseTreeTranslator(treeGrammar);
                            SDT.ParseTree root = treeTr.Parse(new SDT.ArithmLexer().Parse(Console.ReadLine()));
                            if (root != null)
                            {
                                root.Print();
                                root.Execute();
                                // утилиты для прорисовки дерева в файл  
                                root.PrintToFile("../../../../parse_tree.dot", true);
                                root.PrintToFile("../../../../parse_tree2.dot", false);
                            }
                            else
                            {
                                Console.WriteLine("Строка не соответствует грамматике");
                            }

                            break;
                        }


                    // I10 - SDT-схема builder: построитель правил трансляции.
                    // Демонстрирует автоматическое построение правил синтаксически управляемого
                    // перевода на основе заданных шаблонов грамматики.
                    case "I10":
                        {
                            /* Барсов А.В. , Савин А.А. , М8О-307Б-19
                             * «Перевод конечных автоматов и грамматик в код на С# на основе СУ-схемы» */
                            /* В общем виде задаётся правило вида: S →   #→$   .addRule(#, new List<Symbol> { $ });
                               - это общее правило для порождения правил грамматики.
                               Из него достаём нужные элементы и записываем их следующим образом
                               Реализация для работы с файлами не включена
                            */

                            string[] ms = "@ # $ %".Split(' '); // мета символы для подстановки
                            string grammarPattern = "# → $"; // левое правило для порождения правил грамматики 
                            string grammarRules = ".addRule(#, new List<Symbol> { $ });"; // правило грамматики для кода(правая)
                            List<string> r = new List<string>() // правила для перевода
            {
              "Å → Å1 + B",
              "Å → T",
              "Å → T * P",
              "T → T * P",
              "T → P",
              "P → ( E )",
              "P → i"
            };
                            Console.WriteLine("Заданная грамматика: S →    " + grammarPattern + " ,   " + grammarRules);
                            SDTBuilder sdtBuilder = new SDTBuilder(ms, grammarPattern, grammarRules, r);
                            sdtBuilder.BuildTable();

                            break;
                        }
                    // I11 - SetL: язык для работы с множествами и грамматиками.
                    // Демонстрирует работу интерпретатора SetL для выполнения операций
                    // над множествами, грамматиками и их пересечениями.
                    case "I11":
                        {
                            var setlFileContent = @"
                          # G(T,V,P,S)

                          # Grammar 1
                          T = {c, d, f, t, g}
                          V = {A, B, C, D, F, G, T}
                          S = A
                          P = {A -> B, B -> CD, C -> c, D -> d, D -> FG, F -> f, G -> g, G -> T, T -> t}
                          C = { a ∈ T* | S => a }

                          # Grammar 2
                          T = {c, d, f, t, g}
                          V = {A, B, C, D, F, G, T}
                          S = A
                          P = {A -> B, B -> CD, C -> c, D -> d}
                          L = { a ∈ T* | S => a }

                          # Final chains intersection
                          M = C ∩ L
                        ";
                            Console.WriteLine("\nEXECUTING SETL:");
                            var setl = new SETL(setlFileContent);
                            foreach (var variable in setl.Variables)
                            {
                                Console.WriteLine($"{variable.Value.Name} = {variable.Value.Data}");
                            }

                            Console.ReadLine();
                            break;
                        }

                    // I12 - Преобразование деревьев при помощи СУ-схемы.
                    // Демонстрирует алгоритм преобразования деревьев разбора с использованием
                    // синтаксически управляемой схемы для трансляции между входной и выходной грамматиками.
                    case "I12":
                        {
                            //Алгоритм 6.1  Преобразование деревьев при помощи СУ-схемы
                            //Белоусов М8О-207Б-20 2022

                            var V = new List<Symbol>()
            {
              new Symbol("E", new List<Symbol>() { "p" }), new Symbol("T", new List<Symbol>() { "q" })
            }; // Множество нетерминалов
                            var OP = new List<Symbol_Operation>()
              { new Symbol_Operation("Add"), new Symbol_Operation("Mul") }; // Множество операционных символов
                            var TerminalInput = new List<Symbol>()
              { "+", "*", new Symbol("i", new List<Symbol>() { "r" }) }; // Множество терминалов входной грамматики
                            var TerminalOutput = new List<Symbol>()
              { "+", "*", new Symbol("i", new List<Symbol>() { "r" }) }; // Множество терминалов выходной грамматики
                            var S = new Symbol("E", new List<Symbol>() { "p" }); // Начальный символ

                            //1. E_p -> (T_q, T_q)
                            //   p <- q
                            //2. E_p -> (E_q + T_r{Add}, E_q T_r + {Add})
                            //   p <- q + r
                            //3. E_p -> (E_q * T_r{Mul}, E_q T_r * {Mul})
                            //   p <- q * r
                            //4. T_p -> (i_q, i_q)
                            //   p <- q


                            // Задаем входную грамматику
                            var GrammarInput = new ATGrammar(V, TerminalInput, OP, S);

                            GrammarInput.Addrule(new Symbol("E", new List<Symbol>() { "p" }),
                              new List<Symbol>() { new Symbol("T", new List<Symbol>() { "q" }) },
                              new List<AttrFunction>() { new AttrFunction(new List<Symbol>() { "p" }, new List<Symbol>() { "q" }) });

                            GrammarInput.Addrule(new Symbol("E", new List<Symbol>() { "p" }),
                              new List<Symbol>()
                              {
                new Symbol("E", new List<Symbol>() { "q" }), "+", new Symbol("T", new List<Symbol>() { "r" }), "Add"
                              },
                              new List<AttrFunction>()
                                { new AttrFunction(new List<Symbol>() { "p" }, new List<Symbol>() { "q", "+", "r" }) });

                            GrammarInput.Addrule(new Symbol("E", new List<Symbol>() { "p" }),
                              new List<Symbol>()
                              {
                new Symbol("E", new List<Symbol>() { "q" }), "*", new Symbol("T", new List<Symbol>() { "r" }), "Mul"
                              },
                              new List<AttrFunction>()
                                { new AttrFunction(new List<Symbol>() { "p" }, new List<Symbol>() { "q", "*", "r" }) });

                            GrammarInput.Addrule(new Symbol("T", new List<Symbol>() { "p" }),
                              new List<Symbol>() { new Symbol("i", new List<Symbol>() { "q" }) },
                              new List<AttrFunction>() { new AttrFunction(new List<Symbol>() { "p" }, new List<Symbol>() { "q" }) });
                            Console.WriteLine("Входная грамматика:");
                            GrammarInput.Print();

                            // Задаем выходную грамматику
                            var GrammarOutput = new ATGrammar(V, TerminalOutput, OP, S);

                            GrammarOutput.Addrule(new Symbol("E", new List<Symbol>() { "p" }),
                              new List<Symbol>() { new Symbol("T", new List<Symbol>() { "q" }) },
                              new List<AttrFunction>() { new AttrFunction(new List<Symbol>() { "p" }, new List<Symbol>() { "q" }) });

                            GrammarOutput.Addrule(new Symbol("E", new List<Symbol>() { "p" }),
                              new List<Symbol>()
                              {
                new Symbol("E", new List<Symbol>() { "q" }), new Symbol("T", new List<Symbol>() { "r" }), "+", "Add"
                              },
                              new List<AttrFunction>()
                                { new AttrFunction(new List<Symbol>() { "p" }, new List<Symbol>() { "q", "+", "r" }) });

                            GrammarOutput.Addrule(new Symbol("E", new List<Symbol>() { "p" }),
                              new List<Symbol>()
                              {
                new Symbol("E", new List<Symbol>() { "q" }), new Symbol("T", new List<Symbol>() { "r" }), "*", "Mul"
                              },
                              new List<AttrFunction>()
                                { new AttrFunction(new List<Symbol>() { "p" }, new List<Symbol>() { "q", "*", "r" }) });

                            GrammarOutput.Addrule(new Symbol("T", new List<Symbol>() { "p" }),
                              new List<Symbol>() { new Symbol("i", new List<Symbol>() { "q" }) },
                              new List<AttrFunction>() { new AttrFunction(new List<Symbol>() { "p" }, new List<Symbol>() { "q" }) });

                            // Построение СУ-схемы на основе входной и выходной грамматик
                            var Scheme = new ATScheme(GrammarInput, GrammarOutput);

                            // i_7 + i_5 * i_3
                            // 2 3 1 4 4 4
                            // 7 5 3

                            // Построение дерева вывода для входной грамматики
                            Console.WriteLine(" Введите цепочку для входной грамматики, ввод через пробелы, например 2 3 1 4 4 4:");
                            Scheme.BuildInputTree();

                            // Вывод дерева
                            Scheme.PrintTree();
                            // Преобразование дерева
                            Scheme.TreeTransform();
                            // Вывод преобразованного дерева
                            Scheme.PrintTree();
                            break;
                        }
                    case "I19":
                        {
                            //Череповская Е.Ю. М8О-413Б-22
                            var pda = new PDA(new List<Symbol>() { "q0", "q", "qf" },
                              new List<Symbol>() { "p", "q", "r", "s", "(", ")", "-", "*", "|", ">" },
                              new List<Symbol>() { "#", "S", "S'", "D", "p", "q", "r", "s", "(", ")", "-", "*", "|", ">" },
                              "q0",
                              "#",
                              new List<Symbol>() { "qf" });

                            pda.addDeltaRule("q0", "ε", "#", new List<Symbol>() { "q" }, new List<Symbol>() { "S", "#" });
                            pda.addDeltaRule("q", "ε", "S", new List<Symbol>() { "q" }, new List<Symbol>() { "p", "S'" });
                            pda.addDeltaRule("q", "ε", "S", new List<Symbol>() { "q" }, new List<Symbol>() { "p" });
                            pda.addDeltaRule("q", "ε", "S", new List<Symbol>() { "q" }, new List<Symbol>() { "q", "S'" });
                            pda.addDeltaRule("q", "ε", "S", new List<Symbol>() { "q" }, new List<Symbol>() { "q" });
                            pda.addDeltaRule("q", "ε", "S", new List<Symbol>() { "q" }, new List<Symbol>() { "r", "S'" });
                            pda.addDeltaRule("q", "ε", "S", new List<Symbol>() { "q" }, new List<Symbol>() { "r" });
                            pda.addDeltaRule("q", "ε", "S", new List<Symbol>() { "q" }, new List<Symbol>() { "s", "S'" });
                            pda.addDeltaRule("q", "ε", "S", new List<Symbol>() { "q" }, new List<Symbol>() { "s" });
                            pda.addDeltaRule("q", "ε", "S", new List<Symbol>() { "q" }, new List<Symbol>() { "(", "S", ")", "S'" });
                            pda.addDeltaRule("q", "ε", "S", new List<Symbol>() { "q" }, new List<Symbol>() { "(", "S", ")" });
                            pda.addDeltaRule("q", "ε", "S", new List<Symbol>() { "q" }, new List<Symbol>() { "-", "S", "S'" });
                            pda.addDeltaRule("q", "ε", "S", new List<Symbol>() { "q" }, new List<Symbol>() { "-", "S" });

                            pda.addDeltaRule("q", "ε", "S'", new List<Symbol>() { "q" }, new List<Symbol>() { "D", "S", "S'" });
                            pda.addDeltaRule("q", "ε", "S'", new List<Symbol>() { "q" }, new List<Symbol>() { "D", "S" });
                            pda.addDeltaRule("q", "ε", "D", new List<Symbol>() { "q" }, new List<Symbol>() { "*" });
                            pda.addDeltaRule("q", "ε", "D", new List<Symbol>() { "q" }, new List<Symbol>() { "|" });
                            pda.addDeltaRule("q", "ε", "D", new List<Symbol>() { "q" }, new List<Symbol>() { ">" });


                            pda.addDeltaRule("q", "p", "p", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" });
                            pda.addDeltaRule("q", "q", "q", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" });
                            pda.addDeltaRule("q", "r", "r", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" });
                            pda.addDeltaRule("q", "s", "s", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" });
                            pda.addDeltaRule("q", "(", "(", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" });
                            pda.addDeltaRule("q", ")", ")", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" });
                            pda.addDeltaRule("q", "¬", "¬", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" });
                            pda.addDeltaRule("q", "*", "*", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" });
                            pda.addDeltaRule("q", "|", "|", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" });
                            pda.addDeltaRule("q", "→", "→", new List<Symbol>() { "q" }, new List<Symbol>() { "ε" });
                            pda.addDeltaRule("q", "ε", "#", new List<Symbol>() { "qf" }, new List<Symbol>() { "ε" });

                            pda.Debug();


                            //Console.Write("Введите выражение: ");
                            //Console.WriteLine("p∧q");
                            //string input_str = Console.ReadLine();


                            // Использование:
                            //pda.execute(input_str);

                            /*pda.execute("p∧q");
                            pda.execute("¬p∨q");
                            pda.execute("(p→q)∧r");
                            pda.execute("p");
                            pda.execute("(p)");
                            pda.execute("p∧"); // Ошибочное
                            pda.execute("(p"); // Ошибочное
                            pda.execute("(p→q)∧¬r∨(s∧¬p)");*/
                            break;
                        }
                    default:
                        Console.WriteLine("Выход из программы");
                        return;

                    case "17": // Контекстно-зависимая грамматика a^n b^n c^n на Машине Тьюринга
                        {
                            Console.WriteLine("КЗ-Грамматика: L = { a^n b^n c^n | n >= 1 }");
                            Console.WriteLine("Реализация на Машине Тьюринга (LBA).");
                            Console.WriteLine(
                              "Алгоритм: Неразрушающая проверка. Символы a,b,c меняются на A,B,C, затем восстанавливаются.");
                            Console.WriteLine("Результат 'T' (True) записывается в конец ленты.\n");

                            // Определяем машину
                            var tm = new RaGlib.Automata.TuringMachine(
                              new List<string> { "q0", "q1", "q2", "q3", "q4", "q5", "q6", "halt" },
                              new List<string> { "a", "b", "c" },
                              new List<string> { "halt" },
                              "q0"
                            );

                            // --- ФАЗА 1: Валидация и маркировка (a->A, b->B, c->C) ---

                            // q0: Поиск 'a'
                            tm.AddRule("q0", "a", "A", Direction.Right, "q1"); // Нашли a, пометили A, идем искать b
                            tm.AddRule("q0", "B", "B", Direction.Right,
                              "q4"); // a кончились (уперлись в уже помеченные B), идем проверять хвост

                            // q1: Поиск 'b' (пропускаем a и B)
                            tm.AddRule("q1", "a", "a", Direction.Right, "q1");
                            tm.AddRule("q1", "B", "B", Direction.Right, "q1");
                            tm.AddRule("q1", "b", "B", Direction.Right, "q2"); // Нашли b, пометили B, идем искать c

                            // q2: Поиск 'c' (пропускаем b и C)
                            tm.AddRule("q2", "b", "b", Direction.Right, "q2");
                            tm.AddRule("q2", "C", "C", Direction.Right, "q2");
                            tm.AddRule("q2", "c", "C", Direction.Left, "q3"); // Нашли c, пометили C, идем НАЗАД (влево)

                            // q3: Возврат к началу (ищем A)
                            tm.AddRule("q3", "a", "a", Direction.Left, "q3");
                            tm.AddRule("q3", "b", "b", Direction.Left, "q3");
                            tm.AddRule("q3", "B", "B", Direction.Left, "q3");
                            tm.AddRule("q3", "C", "C", Direction.Left, "q3");
                            tm.AddRule("q3", "A", "A", Direction.Right, "q0"); // Нашли границу A, шаг вправо, начинаем новый цикл (q0)

                            // --- ФАЗА 2: Проверка хвоста и Восстановление ---

                            // q4: Проверка, что после исчерпания 'a' не осталось лишних 'b' или 'c'
                            tm.AddRule("q4", "B", "B", Direction.Right, "q4"); // Пропускаем помеченные B
                            tm.AddRule("q4", "C", "C", Direction.Right, "q4"); // Пропускаем помеченные C
                            tm.AddRule("q4", "_", "_", Direction.Left, "q5"); // Дошли до конца успешно. Начинаем восстановление.

                            // q5: Восстановление (идем влево: C->c, B->b, A->a)
                            tm.AddRule("q5", "C", "c", Direction.Left, "q5");
                            tm.AddRule("q5", "B", "b", Direction.Left, "q5");
                            tm.AddRule("q5", "A", "a", Direction.Left, "q5");
                            tm.AddRule("q5", "a", "a", Direction.Left, "q5"); // Пропускаем уже восстановленные
                            tm.AddRule("q5", "b", "b", Direction.Left, "q5");
                            tm.AddRule("q5", "c", "c", Direction.Left, "q5");
                            // Уперлись в левый край (пустоту или начало ленты, если реализован маркер, здесь считаем что уперлись в пустоту слева)
                            // Для простоты, если мы пришли в начало, символ будет восстановлен и мы упремся в пустоту _ слева, если мы сдвигали строку, 
                            // но в данной реализации мы просто ищем конец строки символов. 
                            // Добавим правило: если видим _ слева (начало ленты), идем вправо писать результат.
                            // В данной реализации Head не уходит в -1, поэтому проверим, если мы восстановили всё.
                            // Добавим спец правило: если в q5 мы видим "_" (начало), идем в q6
                            tm.AddRule("q5", "_", "_", Direction.Right, "q6");

                            // --- ФАЗА 3: Запись результата ---

                            tm.AddRule("q6", "a", "a", Direction.Right, "q6");
                            tm.AddRule("q6", "b", "b", Direction.Right, "q6");
                            tm.AddRule("q6", "c", "c", Direction.Right, "q6");

                            // Когда встречаем первый пробел после строки:
                            // Оставляем его пробелом, идем ВПРАВО в новое состояние q7
                            tm.AddRule("q6", "_", "_", Direction.Right, "q7");

                            // q7: Пишем T и останавливаемся
                            tm.AddRule("q7", "_", "T", Direction.Stay, "halt");

                            Console.WriteLine("Введите строку для проверки (например: aabbcc):");
                            string input = Console.ReadLine();
                            tm.Execute(input);
                            Console.ReadLine();
                        }
                        break;

                    case "18": // Машина Тьюринга для языка L = { a^(2^n) }
                        {
                            Console.WriteLine("Язык: L = { a^(2^n) | n >= 0 } (Степени двойки: 1, 2, 4, 8...)");
                            Console.WriteLine("Алгоритм: Рекурсивное деление количества 'a' на 2.");

                            // Инициализация МТ
                            var tm = new RaGlib.Automata.TuringMachine(
                                new List<string> { "q0", "q1", "q2", "q3", "q4", "halt" }, // Состояния
                                new List<string> { "a" },                                  // Алфавит
                                new List<string> { "halt" },                               // Финальные состояния
                                "q0"                                                       // Начальное состояние
                            );

                            // Вспомогательные переменные для краткости
                            var R = RaGlib.Automata.Direction.Right;
                            var L = RaGlib.Automata.Direction.Left;
                            var N = RaGlib.Automata.Direction.Stay;

                            // --- q0: Инициализация ---
                            // Заменяем первую 'a' на '_', это будет маркер начала.
                            // Также это уменьшает счетчик на 1 (для n=0 (одна 'a') останется 0 символов, что корректно обработается q1)
                            tm.AddRule("q0", "a", "_", R, "q1");

                            // --- q1: Поиск 'a' для зачеркивания (нечетная позиция в текущем цикле) ---
                            tm.AddRule("q1", "x", "x", R, "q1"); // Пропускаем уже зачеркнутые
                            tm.AddRule("q1", "a", "x", R, "q2"); // Нашли 'a' -> зачеркиваем (x), идем искать пару
                            tm.AddRule("q1", "_", "_", N, "halt"); // Если 'a' кончились и мы здесь -> УСПЕХ (осталась одна исходная, которую стерли в q0)

                            // --- q2: Поиск 'a' для сохранения (четная позиция) ---
                            tm.AddRule("q2", "x", "x", R, "q2"); // Пропускаем зачеркнутые
                            tm.AddRule("q2", "a", "a", R, "q3"); // Нашли 'a' -> оставляем, идем искать следующую для зачеркивания
                            tm.AddRule("q2", "_", "_", L, "q4"); // Строка кончилась, парность соблюдена. Идем на возврат каретки.

                            // --- q3: Поиск 'a' для зачеркивания (после сохранения предыдущей) ---
                            tm.AddRule("q3", "x", "x", R, "q3");
                            tm.AddRule("q3", "a", "x", R, "q2"); // Нашли -> зачеркиваем, возвращаемся в q2 искать пару
                                                                 // Если в q3 встретили "_" (конец ленты), значит у нас было нечетное количество > 1. 
                                                                 // Правило не добавляем -> машина остановится с ошибкой (REJECT).

                            // --- q4: Возврат в начало ---
                            tm.AddRule("q4", "a", "a", L, "q4");
                            tm.AddRule("q4", "x", "x", L, "q4");
                            tm.AddRule("q4", "_", "_", R, "q1"); // Уперлись в маркер начала, сдвиг вправо, новый цикл деления

                            // Запуск
                            Console.WriteLine("Введите строку из 'a' (например: aaaa):");
                            string input = Console.ReadLine();
                            tm.Execute(input);
                            Console.ReadLine();
                        }
                        break;

                    case "19": // Контекстно-зависимая грамматика a^n b^n c^n на LBA
                        {
                            Console.WriteLine("КЗ-Грамматика: L = { a^n b^n c^n | n >= 1 }");
                            Console.WriteLine("Реализация на Линейно Ограниченном Автомате (LBA).");
                            Console.WriteLine("Границы строки: $");
                            Console.WriteLine("Алгоритм: Неразрушающая проверка с маркировкой символов A,B,C.");
                            Console.WriteLine("Результат: Останавливается в состоянии 'halt' если строка принадлежит языку.\n");

                            // Создаем LBA (TuringMachine с ограничением не выходить за границы $)
                            var lba = new RaGlib.Automata.TuringMachine(
                                new List<string> { "q0", "q1", "q2", "q3", "q4", "q5", "q6", "halt", "reject" },
                                new List<string> { "a", "b", "c", "A", "B", "C", "$" }, // Расширенный алфавит с маркерами и границами
                                new List<string> { "halt" }, // Финальное состояние
                                "q0" // Начальное состояние
                            );

                            // --- ФАЗА 1: Маркировка символов в цикле (a->A, b->B, c->C) ---

                            // q0: Ищем первый 'a' для маркировки
                            lba.AddRule("q0", "a", "A", Direction.Right, "q1"); // Нашли 'a', меняем на 'A', идем искать 'b'
                            lba.AddRule("q0", "B", "B", Direction.Right, "q4"); // Все 'a' обработаны (видим 'B'), переходим к проверке хвоста
                            lba.AddRule("q0", "C", "C", Direction.Right, "q4"); // Аналогично, если видим 'C'
                            lba.AddRule("q0", "$", "$", Direction.Stay, "reject"); // Пустая строка или только границы - отвергаем

                            // q1: Ищем 'b' (пропускаем 'a' и уже помеченные 'B')
                            lba.AddRule("q1", "a", "a", Direction.Right, "q1"); // Пропускаем непомеченные 'a'
                            lba.AddRule("q1", "B", "B", Direction.Right, "q1"); // Пропускаем помеченные 'B'
                            lba.AddRule("q1", "b", "B", Direction.Right, "q2"); // Нашли 'b', помечаем как 'B', идем искать 'c'
                            lba.AddRule("q1", "$", "$", Direction.Stay, "reject"); // Дошли до конца раньше времени - отвергаем

                            // q2: Ищем 'c' (пропускаем 'b' и уже помеченные 'C')
                            lba.AddRule("q2", "b", "b", Direction.Right, "q2"); // Пропускаем непомеченные 'b'
                            lba.AddRule("q2", "C", "C", Direction.Right, "q2"); // Пропускаем помеченные 'C'
                            lba.AddRule("q2", "c", "C", Direction.Left, "q3"); // Нашли 'c', помечаем как 'C', возвращаемся назад
                            lba.AddRule("q2", "$", "$", Direction.Stay, "reject"); // Нет 'c' - отвергаем

                            // q3: Возвращаемся к началу строки для следующей итерации
                            lba.AddRule("q3", "a", "a", Direction.Left, "q3");
                            lba.AddRule("q3", "b", "b", Direction.Left, "q3");
                            lba.AddRule("q3", "B", "B", Direction.Left, "q3");
                            lba.AddRule("q3", "C", "C", Direction.Left, "q3");
                            lba.AddRule("q3", "A", "A", Direction.Right, "q0"); // Достигли начала маркировки, продолжаем цикл
                            lba.AddRule("q3", "$", "$", Direction.Right, "q0"); // Достигли левой границы, продолжаем цикл

                            // --- ФАЗА 2: Проверка хвоста и восстановление ---

                            // q4: Проверяем, что после всех 'a' остались только помеченные 'B' и 'C'
                            lba.AddRule("q4", "B", "B", Direction.Right, "q4"); // Пропускаем 'B'
                            lba.AddRule("q4", "C", "C", Direction.Right, "q4"); // Пропускаем 'C'
                            lba.AddRule("q4", "$", "$", Direction.Left, "q5"); // Достигли правой границы - строка корректна

                            // q5: Восстанавливаем оригинальные символы (идем влево к началу)
                            lba.AddRule("q5", "C", "c", Direction.Left, "q5"); // Восстанавливаем 'c'
                            lba.AddRule("q5", "B", "b", Direction.Left, "q5"); // Восстанавливаем 'b'
                            lba.AddRule("q5", "A", "a", Direction.Left, "q5"); // Восстанавливаем 'a'
                            lba.AddRule("q5", "a", "a", Direction.Left, "q5"); // Пропускаем уже восстановленные
                            lba.AddRule("q5", "b", "b", Direction.Left, "q5");
                            lba.AddRule("q5", "c", "c", Direction.Left, "q5");
                            lba.AddRule("q5", "$", "$", Direction.Right, "q6"); // Достигли левой границы, переходим к завершению

                            // --- ФАЗА 3: Завершение работы ---
                            
                            // q6: Мы на первом символе восстановленной строки
                            lba.AddRule("q6", "a", "a", Direction.Stay, "halt"); // Принимаем строку
                            lba.AddRule("q6", "b", "b", Direction.Stay, "halt");
                            lba.AddRule("q6", "c", "c", Direction.Stay, "halt");

                            Console.WriteLine("Введите строку для проверки (например: aabbcc):");
                            string input = Console.ReadLine();
                            
                            // Запуск LBA (с ограничениями на движение за границы)
                            lba.ExecuteLBA(input);
                            Console.ReadLine();
                        }
                        break;

                        case "20": // Контекстно-зависимая грамматика a^(2^n) на LBA
                        {
                            Console.WriteLine("Язык: L = { a^(2^n) | n >= 0 } (Степени двойки: 1, 2, 4, 8...)");
                            Console.WriteLine("Реализация на Линейно Ограниченном Автомате (LBA).");
                            Console.WriteLine("Границы строки: $");
                            Console.WriteLine("Алгоритм: Рекурсивное деление количества 'a' на 2 с использованием маркера '_' и зачеркивания 'X'.\n");

                            var lba = new RaGlib.Automata.TuringMachine(
                                new List<string> { "q0", "q1", "q2", "q3", "q4", "H", "halt", "reject" },
                                new List<string> { "a", "_", "X", "$" }, // Алфавит: a - символ, _ - маркер начала, X - зачеркнутый символ
                                new List<string> { "halt" }, // Финальное состояние
                                "q0" // Начальное состояние
                            );

                            // --- ИНИЦИАЛИЗАЦИЯ: Ставим маркер начала ---
                            
                            // q0: Заменяем первую 'a' на '_' (маркер начала обработки)
                            lba.AddRule("q0", "a", "_", Direction.Right, "q1"); // Ставим маркер, переходим к обработке
                            lba.AddRule("q0", "$", "$", Direction.Stay, "halt"); // Пустая строка (0 символов) - принимаем

                            // --- ОСНОВНОЙ ЦИКЛ: Поиск и зачеркивание пар символов ---

                            // q1: Ищем 'a' для зачеркивания (первый символ пары)
                            lba.AddRule("q1", "X", "X", Direction.Right, "q1"); // Пропускаем уже зачеркнутые
                            lba.AddRule("q1", "a", "X", Direction.Right, "q2"); // Нашли 'a' -> зачеркиваем (X), ищем пару
                            lba.AddRule("q1", "$", "$", Direction.Stay, "H"); // Если дошли до конца - успех (остался 0 или 1 символ)

                            // q2: Ищем 'a' для сохранения (второй символ пары)
                            lba.AddRule("q2", "X", "X", Direction.Right, "q2"); // Пропускаем зачеркнутые
                            lba.AddRule("q2", "a", "a", Direction.Right, "q3"); // Нашли 'a' -> сохраняем, продолжаем поиск
                            lba.AddRule("q2", "$", "$", Direction.Left, "q4"); // Дошли до конца - пары сбалансированы, возвращаемся

                            // q3: Ищем следующую 'a' для зачеркивания (после сохранения)
                            lba.AddRule("q3", "X", "X", Direction.Right, "q3"); // Пропускаем зачеркнутые
                            lba.AddRule("q3", "a", "X", Direction.Right, "q2"); // Нашли -> зачеркиваем, возвращаемся к поиску пары
                            // Если в q3 встретили "$" (конец строки) - нечетное количество > 1, автомат остановится (отвергнет)

                            // --- ВОЗВРАТ К НАЧАЛУ И НОВЫЙ ЦИКЛ ---

                            // q4: Возвращаемся к маркеру начала '_'
                            lba.AddRule("q4", "a", "a", Direction.Left, "q4"); // Двигаемся влево через 'a'
                            lba.AddRule("q4", "X", "X", Direction.Left, "q4"); // Двигаемся влево через зачеркнутые
                            lba.AddRule("q4", "_", "_", Direction.Right, "q1"); // Нашли маркер, начинаем новый цикл деления
                            lba.AddRule("q4", "$", "$", Direction.Right, "q1"); // Достигли начала строки, начинаем новый цикл

                            // --- ЗАВЕРШЕНИЕ: Успешное распознавание ---

                            // H: Состояние успеха (достигнуто когда в q1 видим конец строки)
                            lba.AddRule("H", "$", "$", Direction.Stay, "halt"); // Переход в финальное состояние

                            Console.WriteLine("Введите строку из 'a' (например: aaaa):");
                            string input = Console.ReadLine();

                            // Запуск LBA
                            lba.ExecuteLBA(input);
                            Console.ReadLine();
                        }
                        break;

                } // end switch
            } // end while
    } // end void Main()

  } // end class Program

  // Класс для операционных символов, используемых в регулярных выражениях
  public static class Operations
  {
    // {a1} (\W) - проверка символа на то, что он не является символом латиницы, цифрой или нижним подчёркиванием
    public static (bool, string) OperationA(string input, int startIndex, int endIndex)
    {
      if (startIndex < 0 || startIndex >= input.Length)
        return (false, input);

      int length = endIndex > 0 ? Math.Min(endIndex, input.Length - startIndex) : 1;
      if (startIndex + length > input.Length)
        return (false, input);

      string substring = input.Substring(startIndex, length);
      bool matches = Regex.IsMatch(substring, @"\W");
      return (matches, input);
    }

    // {a2} (p) - проверка символа на соответствие символу 'p'
    public static (bool, string) OperationB(string input, int startIndex, int endIndex)
    {
      if (startIndex < 0 || startIndex >= input.Length)
        return (false, input);

      int length = endIndex > 0 ? Math.Min(endIndex, input.Length - startIndex) : 1;
      if (startIndex + length > input.Length)
        return (false, input);

      string substring = input.Substring(startIndex, length);
      bool matches = substring == "p";
      return (matches, input);
    }

    // {a3} (o) - проверка символа на соответствие символу 'o'
    public static (bool, string) OperationC(string input, int startIndex, int endIndex)
    {
      if (startIndex < 0 || startIndex >= input.Length)
        return (false, input);

      int length = endIndex > 0 ? Math.Min(endIndex, input.Length - startIndex) : 1;
      if (startIndex + length > input.Length)
        return (false, input);

      string substring = input.Substring(startIndex, length);
      bool matches = substring == "o";
      return (matches, input);
    }

    // {a4} ([#-]) - проверка символа на соответствие символу '#' или '-'
    public static (bool, string) OperationD(string input, int startIndex, int endIndex)
    {
      if (startIndex < 0 || startIndex >= input.Length)
        return (false, input);

      int length = endIndex > 0 ? Math.Min(endIndex, input.Length - startIndex) : 1;
      if (startIndex + length > input.Length)
        return (false, input);

      string substring = input.Substring(startIndex, length);
      bool matches = Regex.IsMatch(substring, @"[#-]");
      return (matches, input);
    }

    // {a5} (\s) - проверка символа на то, что он является пробельным
    public static (bool, string) OperationE(string input, int startIndex, int endIndex)
    {
      if (startIndex < 0 || startIndex >= input.Length)
        return (false, input);

      int length = endIndex > 0 ? Math.Min(endIndex, input.Length - startIndex) : 1;
      if (startIndex + length > input.Length)
        return (false, input);

      string substring = input.Substring(startIndex, length);
      bool matches = Regex.IsMatch(substring, @"\s");
      return (matches, input);
    }

    // {a6} (\d) - проверка символа на то, что он является цифрой
    public static (bool, string) OperationF(string input, int startIndex, int endIndex)
    {
      if (startIndex < 0 || startIndex >= input.Length)
        return (false, input);

      int length = endIndex > 0 ? Math.Min(endIndex, input.Length - startIndex) : 1;
      if (startIndex + length > input.Length)
        return (false, input);

      string substring = input.Substring(startIndex, length);
      bool matches = Regex.IsMatch(substring, @"\d");
      return (matches, input);
    }

    // {a7} ([\s-]) - проверка символа на то, что он является пробельным или равен '-'
    public static (bool, string) OperationG(string input, int startIndex, int endIndex)
    {
      if (startIndex < 0 || startIndex >= input.Length)
        return (false, input);

      int length = endIndex > 0 ? Math.Min(endIndex, input.Length - startIndex) : 1;
      if (startIndex + length > input.Length)
        return (false, input);

      string substring = input.Substring(startIndex, length);
      bool matches = Regex.IsMatch(substring, @"[\s-]");
      return (matches, input);
    }

    // {a8} - дополнительная операция (если нужна)
    public static (bool, string) OperationH(string input, int startIndex, int endIndex)
    {
      if (startIndex < 0 || startIndex >= input.Length)
        return (false, input);

      int length = endIndex > 0 ? Math.Min(endIndex, input.Length - startIndex) : 1;
      if (startIndex + length > input.Length)
        return (false, input);

      string substring = input.Substring(startIndex, length);
      // По умолчанию возвращаем true для совместимости
      return (true, input);
    }
  }
}
