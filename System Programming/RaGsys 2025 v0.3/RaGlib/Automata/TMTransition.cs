using System;
using RaGlib.Core;

namespace RaGlib.Automata
{
    // Направление движения головки
    public enum Direction
    {
        Left = -1,
        Stay = 0,
        Right = 1
    }

    // Правило перехода Машины Тьюринга
    public class TMTransition
    {
        public string FromState { get; set; }      // Текущее состояние (q)
        public string ReadSymbol { get; set; }     // Считываемый символ (a)
        public string ToState { get; set; }        // Новое состояние (q')
        public string WriteSymbol { get; set; }    // Записываемый символ (b)
        public Direction MoveDirection { get; set; }// Направление сдвига (L/R/N)

        public TMTransition(string fromState, string readSymbol, string writeSymbol, Direction direction, string toState)
        {
            FromState = fromState;
            ReadSymbol = readSymbol;
            WriteSymbol = writeSymbol;
            MoveDirection = direction;
            ToState = toState;
        }

        public void Debug()
        {
            string dirStr = MoveDirection == Direction.Right ? "R" : (MoveDirection == Direction.Left ? "L" : "N");
            Console.WriteLine($"δ({FromState}, {ReadSymbol}) -> ({ToState}, {WriteSymbol}, {dirStr})");
        }
    }
}