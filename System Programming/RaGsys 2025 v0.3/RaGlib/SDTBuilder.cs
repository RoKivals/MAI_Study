using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RaGlib {
  public class SDTBuilder {
    private string[] metaSymbols;
    private List<string> rules; // file1  T -> A    ->     # -> $
    private string patternS;
    private string patternRule;
    private string patternCode;// file2 ".addRule(#, new List<Symbol> { $ });"

    private string pathFilePattern;
    private string pathFileRules;
    private string pathFileTable;

    private List<string> table;

    public SDTBuilder(string p1,string p2,string p3) {
      table=new List<string>();
      rules=new List<string>();
      patternS="";
      patternRule="";
      patternCode="";
      pathFilePattern=p1;
      pathFileRules=p2;
      pathFileTable=p3;
    }

    public SDTBuilder(string[] ms,string p1,string p2,List<string> r1) {
      metaSymbols=ms;
      table=new List<string>();
      patternS="S → ";
      rules=r1;
      patternRule=p1;
      patternCode=p2;
    }

    public void ReadPattern() // читаем паттерн с мета символами 
    {
      try {
        using (StreamReader fs = new StreamReader(pathFilePattern)) {
          metaSymbols=fs.ReadLine().Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries);
          patternS=fs.ReadLine();
          patternRule=fs.ReadLine();
          patternCode=fs.ReadLine();
        }
      } catch {
        throw new Exception("Файл не существует или пуст");
      }
    }
    public void ReadData() //читаем грамматику/автомат
    {
      try {
        using (StreamReader fs = new StreamReader(pathFileRules)) {
          rules=new List<string>();
          string temp = fs.ReadLine(); // считываем построчно правила
          while (temp!=null) {
            rules.Add(temp);
            temp=fs.ReadLine();
          }
        }
      } catch {
        throw new Exception("Файл не существует или пуст");
      }
    }

    public Dictionary<char,string> Mapping(string rule,string chain) { // если в шаблоне после метасимвола поставить пробел, то ему присвоится единичный символ, не цепочка
      const char sentinel = (char)0; // добавочый символ для общности алгоритма
      rule+=sentinel;
      chain+=sentinel;
      Dictionary<char,string> map = new Dictionary<char,string>(); // словарь соответствий
                                                                   // параллельно проходимся двумя укзателями по строкам, находя 
                                                                   // соответствующие цепочки для метасимволов
      for (int i = 0, j = 0; rule[i]!=sentinel&&chain[j]!=sentinel; i++, j++) {
        if (rule[i]!=chain[j]) {
          if (!metaSymbols.Contains(rule[i].ToString())) { // ошибка, шаблон неверный, под него нельзя подставить правила
            throw new Exception("unreal pattern!!!");
          } else {
            int from = j;
            // составление соотв. подстроки
            while (chain[j]!=rule[i+1]) j++;
            // сохранение соответствия
            map[rule[i]]=chain.Substring(from,j-from);
            j--;
          }
        }
      }
      return map;
    }

    static string Substitute(string rule,Dictionary<char,string> map,bool quoted = false) { // функция для подстановки - заполнение шаблона заменами на соотв.цепочки
      foreach (var item in map) { // quoted - вспомогательное, если символы нужны обрамлять в кавычки для вывода
        if (quoted)
          rule=rule.Replace(item.Key.ToString(),String.Join(", ",item.Value
              .Split(' ')
              .ToList()
              .Select(s => "\""+s+"\"")));
        else
          rule=rule.Replace(item.Key.ToString(),item.Value);
      }
      return rule;
    }

    public void ApplyPattern() {
      Console.OutputEncoding=System.Text.Encoding.UTF8;
      table=new List<string>(); // таблица получаемой СУ-схемы
      List<string> lhs_list = new List<string>(); // множество левых правил (LHS)
      List<string> rhs_list = new List<string>(); // множество правых правил (RHS)

      int max = -1; // для форматирования таблицы
      foreach (var chain in rules) {
        Console.WriteLine("Цепочка: "+chain);
        // нахождение соответствий для дальнейшей подстановки
        var map = Mapping(patternRule,chain);
        // подстановка соотв. цепочек на метасимволы
        var lhs = Substitute(patternRule,map);
        var rhs = Substitute(patternCode,map,true);

        Console.WriteLine("Спец.символам для подстановки соответствуют подцепочки:");
        // вывод для отладки
        foreach (var item in map) {
          Console.WriteLine(item.Key+" = "+item.Value);
        }
        Console.WriteLine("Полученный перевод:");
        Console.WriteLine(lhs+"  ,  "+rhs);
        if (lhs.Length>max)
          max=lhs.Length;
        lhs_list.Add(lhs);
        rhs_list.Add(rhs);
        Console.WriteLine();
      }
      // заполнение таблицы
      int k = 0;
      while (k<lhs_list.Count) {
        // подготовим формат для ровных столбцов в таблице
        table.Add(lhs_list[k].PadRight(lhs_list[k].Length+max-lhs_list[k].Length+3)+"|   "+rhs_list[k]);
        ++k;
      }
    }


    public void WriteTable() {
      try {
        using (StreamWriter w = new StreamWriter(pathFileTable,false,Encoding.Unicode)) {
          foreach (var tr in table) {
            w.WriteLine(tr);
          }
        }
      } catch {
        throw new Exception("Файла не существует");
      }
    }


    public void BuildTable() {
      // ReadPattern(); // читаем паттерн с мета-символами 
      // ReadData();

      //Console.WriteLine(patternCode);
      // Console.WriteLine(patternRule);
      Console.WriteLine("Рассмотрим каждое правило: ");
      ApplyPattern();
      Console.WriteLine("Итоговая таблица перевода: ");
      foreach (var t in table) {
        Console.WriteLine(t);
      }
      //WriteTable();
    }
  }
}
