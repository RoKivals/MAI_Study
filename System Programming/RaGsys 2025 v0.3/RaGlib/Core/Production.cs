// ============================================================================
// Production.cs - Класс для представления продукции грамматики
// ============================================================================
// Продукция (правило) грамматики имеет вид: LHS -> RHS, где LHS (левая часть)
// - нетерминал, а RHS (правая часть) - цепочка символов (терминалов и нетерминалов).
// Каждая продукция имеет уникальный идентификатор для отслеживания в процессе разбора.
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaGlib.Core {
    public class Production
    {
        public Symbol LHS { set; get; } = null; ///< On the Left Hand Side
        public List<Symbol> RHS { set; get; } ///< On the Right Hand Side
        public static int Count = 0; 
        public int Id; ///< Production number

        // Конструктор продукции - создает новое правило грамматики с автоматическим присвоением ID
        public Production(Symbol LHS, List<Symbol> RHS)
        {
            Count++;
            Id = Count;
            this.LHS = LHS;
            this.RHS = RHS;
        }
    } // end Production
}
