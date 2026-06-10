using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RaGlib.Core;

namespace RaGlib.Automata
{
    public class TuringMachine
    {
        private List<string> Tape;
        private int Head;
        private string CurrentState;
        private List<TMTransition> Transitions;
        private List<string> FinalStates;
        private string BlankSymbol = "_";

        public TuringMachine(List<string> states, List<string> sigma, List<string> finalStates, string q0)
        {
            Tape = new List<string>();
            Head = 0;
            CurrentState = q0;
            Transitions = new List<TMTransition>();
            FinalStates = finalStates;
        }

        public void AddRule(string fromState, string read, string write, Direction dir, string toState)
        {
            Transitions.Add(new TMTransition(fromState, read, write, dir, toState));
        }

        private TMTransition FindTransition(string state, string symbol)
        {
            return Transitions.FirstOrDefault(t => t.FromState == state && t.ReadSymbol == symbol);
        }

        public void Execute(string inputChain)
        {
            // === ИНИЦИАЛИЗАЦИЯ ЛЕНТЫ ===
            Tape.Clear();
            
            // 1. Добавляем "стенку" слева (пустой символ)
            Tape.Add(BlankSymbol); 
            
            // 2. Добавляем саму строку
            foreach (char c in inputChain) Tape.Add(c.ToString());
            
            // 3. Добавляем пустые символы в конец (хвост ленты)
            for(int k=0; k<10; k++) Tape.Add(BlankSymbol);

            // === ВАЖНО: Головка встает на первый символ СТРОКИ (индекс 1) ===
            Head = 1; 
            
            CurrentState = "q0"; 
            
            Console.WriteLine("\n--- ЗАПУСК МАШИНЫ ТЬЮРИНГА ---");
            Console.WriteLine($"Входная строка: {inputChain}");
            Console.WriteLine("Формат вывода: (Состояние, [Символ_под_головкой] Лента)");

            int step = 0;
            bool halted = false;

            while (!halted)
            {
                PrintConfiguration(step);

                if (FinalStates.Contains(CurrentState))
                {
                    Console.WriteLine("\nRESULT: ACCEPTED (Строка валидна)");
                    halted = true;
                    break;
                }

                string symbolUnderHead = Head < Tape.Count ? Tape[Head] : BlankSymbol;

                var rule = FindTransition(CurrentState, symbolUnderHead);

                if (rule == null)
                {
                    Console.WriteLine($"\nТупиковая ситуация! Нет перехода для ({CurrentState}, {symbolUnderHead})");
                    Console.WriteLine("RESULT: REJECTED (Строка невалидна)");
                    halted = true;
                    break;
                }

                // 1. Запись
                if (Head < Tape.Count) 
                {
                    Tape[Head] = rule.WriteSymbol;
                }
                else if (rule.WriteSymbol != BlankSymbol) 
                { 
                    while(Tape.Count <= Head) Tape.Add(BlankSymbol);
                    Tape[Head] = rule.WriteSymbol;
                }

                // 2. Сдвиг
                Head += (int)rule.MoveDirection;
                
                if (Head < 0) Head = 0;

                // 3. Смена состояния
                CurrentState = rule.ToState;
                step++;

                if (step > 10000)
                {
                    Console.WriteLine("Превышен лимит шагов!");
                    break;
                }
            }
        }

        private void PrintConfiguration(int step)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Step {step, -4}: ({CurrentState}, ");
            
            for (int i = 0; i < Tape.Count; i++)
            {
                if (i > Head + 2 && i >= Tape.Count - 5 && Tape[i] == BlankSymbol) continue;

                if (i == Head) sb.Append($"[{Tape[i]}]");
                else sb.Append(Tape[i]);
            }
            sb.Append(")");
            Console.WriteLine(sb.ToString());
        }

        public void ExecuteLBA(string inputChain)
        {
            Tape.Clear();
            
            // Добавляем границы
            Tape.Add("$");
            foreach (char c in inputChain) Tape.Add(c.ToString());
            Tape.Add("$");
            
            Head = 1; // Первый символ после левой границы
            CurrentState = "q0";
            
            Console.WriteLine("\n--- ЗАПУСК ЛИНЕЙНО ОГРАНИЧЕННОГО АВТОМАТА ---");
            Console.WriteLine($"Входная строка: {inputChain}");
            Console.WriteLine("Границы строки: $ (головка не должна выходить за них)");
            Console.WriteLine();

            int step = 0;
            bool halted = false;

            while (!halted)
            {
                PrintConfigurationLBA(step);

                if (CurrentState == "halt")
                {
                    Console.WriteLine("\n✓ RESULT: ACCEPTED (Строка валидна)");
                    halted = true;
                    break;
                }
                
                if (CurrentState == "reject")
                {
                    Console.WriteLine("\n✗ RESULT: REJECTED (Строка невалидна)");
                    halted = true;
                    break;
                }

                string symbolUnderHead = Tape[Head];
                var rule = FindTransition(CurrentState, symbolUnderHead);

                if (rule == null)
                {
                    Console.WriteLine($"\n✗ Нет перехода для ({CurrentState}, {symbolUnderHead})");
                    Console.WriteLine("✗ RESULT: REJECTED");
                    halted = true;
                    break;
                }

                // Запись
                Tape[Head] = rule.WriteSymbol;
                
                // Сдвиг с проверкой границ
                int newHead = Head + (int)rule.MoveDirection;
                
                // Проверяем, что не выходим за границы
                if (newHead < 0 || newHead >= Tape.Count)
                {
                    Console.WriteLine($"\n✗ Нарушение LBA: выход за границы");
                    Console.WriteLine("✗ RESULT: REJECTED");
                    halted = true;
                    break;
                }
                
                Head = newHead;
                CurrentState = rule.ToState;
                
                step++;

                if (step > 1000)
                {
                    Console.WriteLine("\n⚠ Превышен лимит шагов!");
                    halted = true;
                    break;
                }
            }
            
            Console.WriteLine("\nФинальное состояние ленты:");
            for (int i = 0; i < Tape.Count; i++)
            {
                if (i == Head) Console.Write($"[{Tape[i]}]");
                else Console.Write(Tape[i]);
            }
            Console.WriteLine();
        }
        
        private void PrintConfigurationLBA(int step)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Step {step, -4}: ({CurrentState}, ");
            
            for (int i = 0; i < Tape.Count; i++)
            {
                if (i == Head) sb.Append($"[{Tape[i]}]");
                else sb.Append(Tape[i]);
            }
            sb.Append(")");
            Console.WriteLine(sb.ToString());
        }
    }
}