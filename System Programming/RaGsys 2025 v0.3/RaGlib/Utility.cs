using System;
using System.Collections.Generic;
using System.Linq;
using RaGlib.Core;


namespace RaGlib {
    
    public static class Utility
    {
        public static string convert(List<Symbol> arrayList)
        {
           string result = "";

            foreach (Symbol s in arrayList)
            {
                result += s.ToString() + ",";
            }

           return result;
        }

        public static string convertInt(List<int> arrayList)
        {
            var strings = arrayList.Cast<Int32>().ToArray();
            var theString = string.Join(" ", strings);
            return theString;
        }

        public static bool IsSameArrayList(List<Symbol> lar, List<Symbol> rar)
        {
            if (lar.Count != rar.Count)
                return false;

            for (int i = 0; i < lar.Count; i++)
            {
                if (lar[i].symbol != rar[i].symbol)
                    return false;
            }

            return true;
        }
    }
}
