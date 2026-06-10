// ============================================================================
// Lexer.cs — Лексические анализаторы (простые и арифметические)
// ============================================================================
// В этом файле определены:
//   • Абстрактный класс Lexer — интерфейс лексического анализатора.
//   • SimpleLexer — простой лексер, создающий по одному токену на символ.
//   • ArithmLexer — лексер для разбора арифметических выражений, который:
//        - объединяет последовательности цифр в числа,
//        - создаёт токен NAME="number" с атрибутом value,
//        - остальные символы интерпретирует как одиночные токены.
// ============================================================================

using System;
using System.Collections.Generic;
using System.Collections;

namespace SDT
{
    // Базовый абстрактный класс лексического анализатора.
    // Наследники должны реализовать метод Parse.
    public abstract class Lexer
    {

        // Разбирает входную строку на последовательность токенов.
        public abstract List<Symbol> Parse(string _);
    }

    // Простейший лексический анализатор:
    // создает отдельный токен для каждого символа строки.
    public class SimpleLexer : Lexer
    {
        public override List<Symbol> Parse(string str)
        {
            List<Symbol> result = new List<Symbol>();

            foreach (char c in str)
                result.Add(new Symbol(c.ToString()));

            return result;
        }
    }

    // Лексический анализатор для арифметических выражений.
    // Поддерживает положительные целые числа.
    //
    // • Последовательность цифр → токен NAME="number", value=число.
    // • Любой одиночный символ → обычный токен Symbol.
    public class ArithmLexer : Lexer
    {
        public override List<Symbol> Parse(string str)
        {
            List<Symbol> result = new List<Symbol>();

            for (int i = 0; i < str.Length; ++i)
            {
                // Разбор числа
                if (Char.IsDigit(str[i]))
                {
                    int value = 0;

                    while (i < str.Length && Char.IsDigit(str[i]))
                    {
                        value = value * 10 + (str[i] - '0');
                        ++i;
                    }

                    // Создаём токен-число
                    result.Add(new Dictionary<string, object>()
                    {
                        ["NAME"] = "number",
                        ["value"] = value
                    });
                }

                // Разбор одиночного символа
                if (i < str.Length)
                {
                    result.Add(new Symbol(str[i].ToString()));
                }
            }

            return result;
        }
    }
}
