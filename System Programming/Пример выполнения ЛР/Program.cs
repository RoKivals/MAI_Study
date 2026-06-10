using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RegularExpression
{
  internal class Program
  {
    static void Main(string[] args)
    {
      //Console.WriteLine("Hi");
      /* 7. Действительный номер мобильного телефона. 
        Лаб 1.
        1. Составть паттерн для правильных номеров телефонов
         1.1. Выписать цепочки правильных символов номеров телефонов         

              1  \+ \  91  \-  96789   67101
              2                96789   67101
              4   +    91   -  96789 - 67101        ...
              7   +    91      96789   67101
                ^\+?\d{0,2}\-?\d{4,5}\-?\d{5,6}
          1.2. Составить 3 паттерн на языке
          
          1.3. Language - L язык 
          L1 = ^\+?\d{0,2}\-?\d{4,5}\-?\d{5,6} 
          L1 = {+91-9678967101, ...,x} 

          1.4. Спроектировать автоматную грамматику  G
          L1 = L(G) 
          L(KA) = L(G)  
          1.5 ПРоверить регулярность. 
        Лаб.2.
        2.1. Алгоритм преобразования G в КА (для 3 КА)  
           Составить общий автомат.
        2.2. Преобразовать НДКА в ДКА
      
        Лаб 3.  Реадлизация  GRFL                   
      */
      string pattern_1 = @"^\+?\d{0,2}\-?\d{4,5}\-?\d{5,6}";
      string pattern_2 = @"^[A-Z][a-zAZ]*$";
      string pattern_3 =  "";
      
      var reg = new Regex(pattern_1);

      // какой-то текст   
      string[] str = {"+91-9678967101", "9678967101", "+96-78967101",
                      "+91-9678-967101", "+91-96789-67101", "abcd+91-96789-67101",
                      "+919678967101","Hello world"};
      //Введите строки для совпадения с действительным номером
      // мобильного телефона.
      foreach (var s in str) {
        Console.WriteLine(" {0} {1} a valid mobile number.",s, 
            reg.IsMatch(s) ? "is" : " is not");
        //The IsMatch method is used to validate a string or
        //to ensure that a string conforms to a particular part
      } // foreach

      Console.ReadKey();
      
    }
  }
}
