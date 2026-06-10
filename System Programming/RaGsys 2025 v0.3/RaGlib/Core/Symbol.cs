// ============================================================================
// Symbol.cs - Базовый класс для представления символов в грамматиках и автоматах
// ============================================================================
// Класс Symbol представляет символ грамматики или автомата (терминал или нетерминал).
// Поддерживает атрибуты символов, используется во всех структурах данных библиотеки.
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaGlib.Core {
  public class Symbol { //: ICloneable  {
      public string symbol; ///< Строковое значение/имя символа
      public List<Symbol> attr { set; get;} = null; ///< Множество атрибутов символа
        // $$$

        //&&&

      public int production { set; get;} = 0; // for grammar occur  
      public int symbolPosition { set; get; } = 0; // for grammar occur

      // Конструктор по умолчанию - создает пустой символ
      public Symbol() {}
      
      // Конструктор с указанием позиции в продукции грамматики
      public Symbol(string s, int production, int symbolPosition)
      {
            this.symbol = s;
            this.production = production;
            this.symbolPosition = symbolPosition;
      }
      
      // Конструктор с атрибутами символа
      public Symbol(string s, List<Symbol> attr)
      {
          this.symbol = s;
          this.attr = new List<Symbol>(attr);
          this.production = 0;
          this.symbolPosition = 0;
      }

      // Конструктор из строки - создает символ с заданным именем
      public Symbol(string value)
      {
          this.symbol = value;
          this.attr = null;
          this.production = 0;
          this.symbolPosition = 0;
      }

      // Неявное преобразование строки в Symbol для удобства использования
      public static implicit operator Symbol(string str) => new Symbol(str);
      
      // Проверка равенства символов по имени (для Dictionary и HashSet)
      public override bool Equals(object other)
      {
          return (other is Symbol) && (this.symbol == ((Symbol)other).symbol);
      }

      // Строгое сравнение с учетом позиции в грамматике (production и symbolPosition)
      public bool SEquals(object other) {
      return (other is Symbol)&&(this.symbol==((Symbol)other).symbol)&&
               (((Symbol)other).production==this.production)&&
               (((Symbol)other).symbolPosition==this.symbolPosition);
      }

      // Проверка, является ли символ пустым (эпсилон)
      public bool IsEpsilon() => string.IsNullOrEmpty(this.symbol)||this.symbol[0]=='\0'||this.symbol=="ε";
      
      // Хеш-функция для использования в Dictionary и HashSet
      public override int GetHashCode()
      {
          return (this.symbol + this.production.ToString() + this.symbolPosition.ToString()).GetHashCode();
      }
      
      // Оператор равенства символов
      public static bool operator == (Symbol a, Symbol b)
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
          return a.symbol== b.symbol;
      }
      
      // Оператор неравенства символов
      public static bool operator != (Symbol symbol1,Symbol symbol2) {
          return !(symbol1 == symbol2);
      }
      
      // Вывод символа и его атрибутов в консоль
      public virtual void print()
      {
          Console.Write(this.symbol);
          Console.Write(" ");
          if (attr == null)
              return;
          foreach (var a in attr)
              Console.Write("_" + a.symbol + " ");
      }
      
      // Преобразование символа в строку (эпсилон отображается как "e")
      public override string ToString() => this != Epsilon ? this.symbol : "e";
      public static readonly Symbol Epsilon = new Symbol(""); ///< Пустой символ
      public static readonly Symbol Sentinel = new Symbol("$"); ///< Cимвол конца строки / Символ дна стека
  }
}
