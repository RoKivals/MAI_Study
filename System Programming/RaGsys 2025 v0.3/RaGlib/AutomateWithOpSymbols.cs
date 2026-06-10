// ============================================================================
// AutomateWithOpSymbols.cs - Конечный автомат с операционными символами
// ============================================================================
// Расширение конечного автомата, поддерживающее выполнение операций (семантических
// действий) во время разбора. Операционные символы позволяют выполнять действия
// при прохождении определенных переходов (сохранение в файл, загрузка из файла,
// разделение строки и т.д.). Используется для создания трансляторов и интерпретаторов.
// ============================================================================

using System;
using System.Collections.Generic;
using RaGlib.Core;
using RaGlib.Automata;
using System.IO;

namespace RaGlib {
    public class FSAutomateWithOpSymbols : FSAutomate
    {
        // Делегат для операций, выполняемых на входной строке
        private delegate string operation(string inStr, int indxInStr);
        // Словарь операционных символов и соответствующих им функций
        private Dictionary<Symbol_Operation, operation> OpDictionary = new Dictionary<Symbol_Operation, operation>();
        // Множество операционных символов (алфавит операций)
        public List<Symbol_Operation> SigmaOP { set; get; } = null;

        // Конструктор автомата с операционными символами
        public FSAutomateWithOpSymbols(List<Symbol> Q, List<Symbol> Sigma, List<Symbol_Operation> SigmaOP, List<Symbol> F, Symbol q0)
        {
            this.Q = Q;
            this.Sigma = Sigma;
            this.SigmaOP = SigmaOP;
            this.Q0 = q0;
            this.F = F;
            this.Delta = new List<DeltaQSigma>();
            // Инициализация стандартных операций
            operation OP = new operation(Save);
            OpDictionary.Add(new Symbol_Operation("{save}"), OP);
            OP = new operation(Load);
            OpDictionary.Add(new Symbol_Operation("{load}"), OP);
            OP = new operation(Split);
            OpDictionary.Add(new Symbol_Operation("{split}"), OP);
        }
        
        // Операция сохранения части строки в файл
        private static string Save(string inStr, int indxInStr)
        {
            Console.WriteLine("Введите путь и имя файла для сохранения в него строки");
            Console.WriteLine("Пример C:\\Users\\mokra\\Documents\\t.txt");
            string path = Console.ReadLine();
            if (!File.Exists(path))
            {
                File.WriteAllText(path, inStr.Substring(0, indxInStr + 1) + Environment.NewLine);
            }
            return inStr;
        }
        
        // Операция загрузки строки из файла и вставки в текущую позицию
        private static string Load(string inStr, int indxInStr)
        {
            Console.WriteLine("Введите путь и имя файла для чтения из него");
            string path = Console.ReadLine();
            string filetext = File.ReadAllText(path);
            filetext = filetext.Substring(0, filetext.Length); 
            string readSubStrInStr = inStr.Substring(0, indxInStr + 1);
            string nonreadSubStrInStr = inStr.Substring(indxInStr + 1);
            inStr = readSubStrInStr + filetext + nonreadSubStrInStr;
            return inStr;
        }
        
        // Операция разделения строки (вставка переноса строки)
        private static string Split(string inStr, int indxInStr)
        {
            string readSubStrInStr = inStr.Substring(0, indxInStr + 1);
            string nonreadSubStrInStr = inStr.Substring(indxInStr + 1);
            inStr = readSubStrInStr + Environment.NewLine + nonreadSubStrInStr;
            return inStr;
        }
        
        // Добавление правила перехода с операционным символом
        public void AddRule(string state, string term, string opSymbol, string nextState)
        {
            this.Delta.Add(new DeltaQSigma(state, term, new List<Symbol> { new Symbol(nextState), new Symbol_Operation(opSymbol) }));
        }

        // Выполнение автомата с выполнением операций при прохождении переходов
        public new void Execute(string chineSymbol)
        {
            int InStrLenght = chineSymbol.Length; // для отслеживания изменения длины строки
            var currState = this.Q0;
            int flag = 0;
            int i = 0;
            for (; i < chineSymbol.Length; i++)
            {
                flag = 0;
                foreach (var d in this.Delta)
                {
                    if (d.LHSQ == currState && d.LHSS == chineSymbol.Substring(i, 1))
                    {
                        currState = d.RHSQ[0].symbol; // Для детерминированного К автомата
                        if (d.RHSQ.Count > 1)// говорит о том что есть оп символ
                        {
                            operation OP = OpDictionary[new Symbol_Operation(d.RHSQ[1].symbol)];
                            //chineSymbol - входная строка, i - для получения последнего обраотанного символа
                            chineSymbol = OP(chineSymbol, i);
                            // длина строки может измениться из-за операции, которая что-то туда записала
                            // i изменится на длину вставки
                            i = i + chineSymbol.Length - InStrLenght;
                            InStrLenght = chineSymbol.Length;
                        }
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0) break;
            } // end for

            Console.WriteLine("Length: " + chineSymbol.Length);
            Console.WriteLine(" i :" + i.ToString());
            Debug("curr", currState.symbol);
            if (this.F.Contains(currState) && i == chineSymbol.Length)
                Console.WriteLine("chineSymbol belongs to language");
            else
                Console.WriteLine("chineSymbol doesn't belong to language");
        } // end Execute
    }
}