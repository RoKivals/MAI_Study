// ============================================================================
// myHTable.cs — Таблица соответствия символов (гомоморфизм)
// ============================================================================
// Класс myHTable реализует простую таблицу для отображения символов входного
// алфавита в символы выходного алфавита. Используется для трансляции или
// преобразования цепочек символов.
// ============================================================================

using System;
using System.Collections;
using System.Collections.Generic;

using RaGlib.Core;

namespace RaGlib
{
    public class myHTable
    {
        // Таблица: первая строка — входные символы, вторая строка — соответствующие выходные
        public List<List<Symbol>> table = new List<List<Symbol>>() { new List<Symbol>(), new List<Symbol>() };

        // Конструктор таблицы. Проверяет, что входной и выходной алфавиты имеют одинаковую длину
        public myHTable(List<Symbol> InputSigma, List<Symbol> OutputSigma)
        {
            if (InputSigma.Count != OutputSigma.Count)
            {
                Console.WriteLine("Неправильно введена таблица (размеры не совпадают)");
                throw new RankException();
            }

            table = new List<List<Symbol>>() { InputSigma, OutputSigma };
        }

        // Отладочный вывод таблицы в консоль
        public void debugHTable()
        {
            var a = table[0];
            var h_a = table[1];
            for (int i = 0; i < a.Count; i++)
            {
                Console.Write(a[i]);
                Console.Write(" --- ");
                Console.WriteLine(h_a[i]);
            }
        }

        // Применяет отображение таблицы к последовательности символов
        public string h(List<Symbol> input)
        {
            string output = "";

            if (input.Count == 1 && input[0].symbol == "")
                return "";

            foreach (Symbol symbol in input) // для каждого символа входной последовательности
            {
                var s = table[0];
                int ss = s.IndexOf(symbol);

                if (ss == -1)
                {
                    Console.WriteLine("Символ не из алфавита\nВходная цепочка не была разобрана");
                    return "";
                }

                // Добавляем соответствующий выходной символ
                output += (table[1])[ss].symbol;
            }

            return output;
        }
    }
}
