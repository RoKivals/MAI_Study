using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RaGlib.Core;
using RaGlib;

namespace RaGlib.Grammars {
    public class ATGrammar : Grammar
    {
        public List<Symbol_Operation> OP {set; get;} = null; ///< Список операционных символов

    // 
        ///< Список продукций c функциями для аттрибутов  
        public List<AttrProduction> Rules { set; get; } = new List<AttrProduction>();         
        public ATGrammar() {}
        /// Конструктор
        public ATGrammar(List<Symbol> V, List<Symbol> T, List<Symbol_Operation> OP /* типо правила  ,*/, Symbol S0)
        {
            this.OP = OP;
            this.V = V;
            this.T = T;
            this.S0 = S0;
        }

    public void Addrule(Symbol LeftNoTerm, List<Symbol> Right) { 
      this.Rules.Add(new AttrProduction(LeftNoTerm,Right));
    }

    /// Добавление правила
    public void Addrule(Symbol LeftNoTerm, 
            List<Symbol> Right,
            List<AttrFunction> F) {
            this.Rules.Add(new AttrProduction(LeftNoTerm, Right, F));
    }

    // Генерация атрибутов для входной грамматики Воронов
    public void NewAT(List<Symbol> A,List<Symbol> Top,List<Symbol> L) {
      int i = 0;
      foreach (var p in Rules) {
        var F = new List<AttrFunction>();
        var AF = new List<Symbol>();

        Symbol B = p.LHS;
        B.attr=new List<Symbol>() { A[i] };
        ++i;

        foreach (var x in p.RHS) {
          if (T.Contains(x)) {
            if (L.Contains(x)) {
              x.attr=new List<Symbol>() { A[i] };
              ++i;
              AF.Add(x.attr[0]);
            } else {
              if (Top.Contains(x)) {
                AF.Add(x);
              }
            }
          } else {
            x.attr=new List<Symbol>() { A[i] };
            ++i;
            AF.Add(x.attr[0]);
          }

        }
        F.Add(new AttrFunction(B.attr,AF));
        p.F=F;
        i=0;
      }

    } // end NewAT 

        //### Тимофеев перевод цикла с С в Python 10.2        
        // AT-grammar for concret example 
        public void ATG_C_Py(List<Symbol> A, List<Symbol> Sign)
        {// символы атрибутов {a,n,s ...}, знаки операций {+, =, <}
            int i = 0;
            List<Symbol> TmpList = new List<Symbol>();// для нетерминалов из (1) 
            Symbol IdVal = null;
            foreach (var p in Rules)
            {
                var F = new List<AttrFunction>(); // вводимые атрибуты 
                var AF = new List<Symbol>(); // символ которому принадлежат вводимые атрибуты 

                var B = p.LHS;
                Symbol x1 = null, x2 = null;
                B.attr = new List<Symbol>();
                bool f = false;
                foreach (var x in p.RHS)
                {
                    if (T.Contains(x))
                    { // если символ терминальный
                        if (Sign.Contains(x))
                        {// если он является знаком 
                            B.attr = new List<Symbol>() { A[i] };
                            ++i;
                            int ind = p.RHS.IndexOf(x); // то определяем его индекс в списке RHS
                            x2 = p.RHS.ElementAt(ind + 1);// берем символ справа от знака 
                            x1 = p.RHS.ElementAt(ind - 1);// и слева 
                            if (x.symbol == "=")
                            {// если равно то синтезируем атрибуты 
                                f = false;
                                IdVal = p.LHS;
                                x1.attr = new List<Symbol>() { A[i] };
                                ++i;
                                x2.attr = new List<Symbol>() { A[i] };
                                ++i;
                                AF.Add(x1.attr[0]);
                            }
                            else
                            { // иначе синтезируем атрибут у константы а у переменной пораждаем из LHS (1)
                                TmpList.Add(p.LHS);
                                B.attr = new List<Symbol>() { A[i] };
                                ++i;
                                f = true;
                                x1.attr = new List<Symbol>() { A[i] };
                                ++i;
                                AF.Add(x1.attr[0]);
                                x2.attr = new List<Symbol>() { A[i] };
                                ++i;
                            }

                        }
                    }
                    else if (V.Contains(x) && x.symbol != "B")
                    {// если это нетерминал
                        f = false;
                        B.attr = new List<Symbol>();
                        ++i;
                        x.attr = new List<Symbol>();
                        ++i;
                        AF = new List<Symbol>();
                    }
                }

                if (f)
                {// если пораждаем от LHS
                    if (B.attr.Count > 0 && x1.attr.Count > 0)
                    {
                        f = false;
                        F.Add(new AttrFunction(x1.attr, B.attr));
                    }
                }
                else
                {// если пораждаем от RHS
                    if (B.attr.Count > 0 && AF.Count > 0)
                    {
                        F.Add(new AttrFunction(B.attr, AF));
                    }
                }

                p.F = F;
                i = 0;
            }// присвоены атрибуты нетерминалам 

            foreach (var p in Rules)
            { // идем по правилам чтобы найти эл-т из списка совпадающий с эл-том правила

                foreach (var x in p.RHS)
                { // идем по эл-там правила RHS
                    if (x.symbol == IdVal.symbol)
                    {
                        ++i;
                        x.attr = new List<Symbol>() { A[4] };
                        IdVal = x;
                    }
                }
            }
            i = 0;
            foreach (var p in Rules)
            { // идем по правилам чтобы найти эл-т из списка TmpList совпадающий с эл-том правила 

                foreach (var x in p.RHS)
                {   // идем по эл-там правила RHS
                    AttrFunction F1 = null;
                    Symbol tmpX = null;
                    foreach (var item in TmpList)
                    {   // идем по списку нетерминалов которые нужно соединить с D и сравниваем с эл-тами из RHS
                        if (x.symbol == item.symbol)
                        {
                            ++i;
                            x.attr = new List<Symbol>() { A[i] };
                            tmpX = x;
                        }
                    }
                    if (tmpX != null)
                    {
                        F1 = new AttrFunction(tmpX.attr, IdVal.attr);
                        tmpX = null;
                    }
                    if (F1 != null)
                    {
                        p.F.Add(F1);
                    }
                }
            }
        } // end ATG_C_Py 

        //##
        
        /// Печать грамматики
        public void Print()
        {
            Console.Write("\nAT-Grammar G = (V, T, OP, P, S)");
            Console.Write("\nV = { "); //нетерминальные символы
            for (int i = 0; i < V.Count; ++i)
            {
                V[i].print();
                if (i != V.Count - 1)
                    Console.Write(", ");
            }
            Console.Write(" },");
            Console.Write("\nT = { "); //терминальные
            for (int i = 0; i < T.Count; ++i)
            {
                T[i].print();
                if (i != T.Count - 1)
                    Console.Write(", ");
            }
            Console.Write(" },");

            var opf = new List<Symbol_Operation>(); //счётчик операционных символов, у которых есть атрибуты
            Console.Write("\nOP = { "); //операционные
            foreach (var op in OP)
            {
                op.print();
                if (op.function != null)
                    opf.Add(op);
                Console.Write(", ");
            }
            Console.Write(" },");

            Console.Write("\nS = ");
            S0.print();

            //печать правил атрибутов операционных символов
            if (opf.Count != 0)
            {
                Console.Write("\nOperation Symbols Rules:\n");
                foreach (var op in opf)
                {
                    op.print();
                    Console.Write("\n");
                }
            }
            //печать правил грамматики
            if (Rules.Count != 0)
            {
                Console.Write("\nGrammar Rules:\n");
                for (int i = 0; i < Rules.Count; ++i)
                {
                    Console.Write("\n");
                    Rules[i].print();
                    Console.Write("\n");
                }
            }
        }

        private bool IsOper(string s) {
            return s=="+"||s=="-"||s=="*"||s=="/";
        }
        public void transform()
        {
            Console.WriteLine("\nPress Enter to start\n");
            Console.ReadLine();
            for (int i = 0; i < Rules.Count; ++i)
            {
                for (int j = 0; j < Rules[i].F.Count; ++j)
                { //обработка j-го атрибутного правила i-го правила грамматики
                    string NewOpS = "";
                    var atrs = new List<Symbol>();
                    var atrs_l = new List<Symbol>();
                    for (int k = 0; k < Rules[i].F[j].RH.Count; ++k)
                    { //проверка наличия функции в правой чаcnи правила
                        if (IsOper(Rules[i].F[j].RH[k].symbol))
                        {
                            NewOpS += Rules[i].F[j].RH[k]; //создание имени для нового оперционного символа
                        }
                        else
                        {
                            atrs.Add(new Symbol(Rules[i].F[j].RH[k] + "'")); //создание дублирующих символов для правил A <- a, но в формате a' <- a.
                            atrs_l.Add(Rules[i].F[j].RH[k]); //список атрибутов, входящих в функцию
                        }
                    }
                    if ((NewOpS.Count()) == 0) // проверка, что нет функций в правй части правила
                        continue;
                    NewOpS += i.ToString() + j.ToString(); //создание уникального имени операционного символа
                    atrs.Add(new Symbol(atrs[0] + "_ans")); // добавление атрибута для результата функции

                    this.OP.Add(new Symbol_Operation("{" + NewOpS + "}", atrs, new List<Symbol>() { new Symbol(atrs[0] + "_ans") },
                        Rules[i].F[j].RH)); // добавление операционного символа с атрибутами и атрибутным правилом
//  Console.WriteLine("####### before ##########");
                    for (int k = 0; k < atrs.Count - 1; ++k)
                    { //добавление копирующих правил a' <- a  !
                      //Было: Rules[i].F.Add(new AttrFunction(new List<Symbol>() { atrs[k] }, new List<Symbol>() { atrs_l[k] }));
                      //Стало:          
                      Rules[i].F.Add(new AttrFunction(new List<Symbol>() { atrs_l[k] }, new List<Symbol>() { atrs[k] }));
                      // BUG при создании копирующих правил для вычисления атрибутов левая и правая часть при записи менялись местами,
                      //из-за чего возникала неправильная печать и добавлялись некоректные правила.
                    }
                    Rules[i].F.Add(new AttrFunction(new List<Symbol>(Rules[i].F[j].LH), new List<Symbol>() { new Symbol(atrs[0] + "_ans") }));
                    //добавление правила z1, ... , zm <- p, где p - результат функции операционного символа
                    Rules[i].F.RemoveAt(j); //удаление правила с функцией в правой части
                    j -= 1;
                    for (int k = Rules[i].RHS.Count - 1; k >= 0; --k)
                    {
                        //поиск самой левой позииции для вставки операционного символа,
                        //начиная с самой правой позиции
                        int k1;
                        if (Rules[i].RHS[k].attr == null) //проверка того, что есть атрибуты у к-го символа правой
                                                          //части правила грамматики
                            continue;
                        for (k1 = 0; k1 < Rules[i].RHS[k].attr.Count; ++k1)
                        { //проверка, что у к-го символа нет атрибута, который есть у операционного символа,
                          //если он есть, то дальше мы не двигаемся и вставляем операционный символ перед ним, инче идём дальше до конца
                            if (atrs_l.Contains(Rules[i].RHS[k].attr[k1]))
                                break;
                        }
                        if (k1 < Rules[i].RHS[k].attr.Count)
                        { //нашли такой символ, справа от которого вставляем операционный
                            Rules[i].RHS.Insert(k + 1, new Symbol("{" + NewOpS + "}", atrs));
                            break;
                        }
                        if (k == 0)
                        { //дошли до конца правила и не нашли символа с хотя бы одним атрибутом, совпадающим с атрибутами операционного символа. Такого быть не должно, т.к. это означает, что атрибутные правила содержат атрибуты, отсутствующие у правила грамматики
                            Rules[i].RHS.Insert(k, new Symbol("{" + NewOpS + "}", atrs));
                            break;
                        }
                    }
                }
                //поиск лишних атрибутов в правилах типа
                // a1, ... , am <- k
                // b1, ..., k, ..., bn <- g
                // и замена на b1, ..., a1, ... , am, ..., bn <- g с удалением правила a1, ... , am <- k
                for (int r = 0; r < Rules[i].F.Count; ++r)
                {
                    bool deleted = false;
                    for (int l = r + 1; l < Rules[i].F.Count; ++l)
                    {
                        if (Rules[i].F[l].LH.Contains(Rules[i].F[r].RH[0]))
                        {
                            Rules[i].F[l].LH.Remove(Rules[i].F[r].RH[0]);
                            deleted = true;
                            foreach (var s in (Rules[i].F[r].LH))
                                Rules[i].F[l].LH.Add(s);
                        }
                    }
                    if (deleted)
                    {
                        Rules[i].F.RemoveAt(r);
                        r -= 1;
                    }
                }
                Console.WriteLine("\nChange for " + (i + 1).ToString() + "th rule\n");
                Rules[i].print();
                Console.ReadLine();
            }
        }

    } // and AGrammar

}
