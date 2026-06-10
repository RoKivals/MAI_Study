// ============================================================================
// Symbol_Operation.cs - Класс для представления операционных символов
// ============================================================================
// Операционные символы используются в грамматиках для выполнения семантических
// действий во время разбора (например, сохранение данных, загрузка, вычисления).
// Наследуется от Symbol и добавляет функциональность выполнения операций.
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaGlib.Core {

    public class Symbol_Operation : Symbol
    {   
        public OPSymbolOperatrion function { set; get; } = null;   
        // Конструктор с полным набором параметров (символ, атрибуты, левая и правая части)
        public Symbol_Operation(string s,List<Symbol> a,List<Symbol> L, List<Symbol> R): 
            base(s,a) {
            function = new OPSymbolOperatrion(this, this.attr);
        }

        // Конструктор с символом и атрибутами
        public Symbol_Operation(string s, List<Symbol> a) : base(s, a) {}

        // Конструктор только с именем символа
        public Symbol_Operation(string s) : base(s) { }

        // Вывод операционного символа в консоль
        public override void print() {      
            Console.Write(this.symbol + "\n");    
        }
    }

}
