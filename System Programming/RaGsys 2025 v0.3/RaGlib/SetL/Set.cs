using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaGlib.Core;

namespace RaGlib.SetL {
    public static class Set {

        /// Распечатать (вывести) множество A
        public static void Print(List<Symbol> A) {
            Console.Write("{ ");
            foreach (Symbol s in A) {
                Console.Write(s);
                if (s != A.Last()) {
                    Console.Write(", ");
                }
            }
            Console.Write(" }");
        } // end print


        /// Принадлежит ли element множеству A
        public static bool Belongs(Symbol element, List<Symbol> A) {
            return A.Contains(element);
        } // end belongs


        /// Объединение множеств A or B
        public static List<Symbol> Union(List<Symbol> A, List<Symbol> B) {
            return A.AsEnumerable().Union(B.AsEnumerable()).ToList();
        } // end union


        /// Пересечение множеств A and B
        public static List<Symbol> Intersect(List<Symbol> A, List<Symbol> B) {
            return A.AsEnumerable().Intersect(B.AsEnumerable()).ToList();
        } // end intersect
    }
}
