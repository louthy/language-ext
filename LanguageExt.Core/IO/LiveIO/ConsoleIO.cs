using System;
using static LanguageExt.Prelude;

namespace LanguageExt.LiveIO
{
    public struct ConsoleIO : Interfaces.ConsoleIO
    {
        public ConsoleKeyInfo ReadKey() =>
            Console.ReadKey();
        
        public Unit Clear()
        {
            Console.Clear();
            return unit;
        }

        public Unit SetBgColor(ConsoleColor color)
        {
            Console.BackgroundColor = color;
            return unit;
        }

        public Unit SetColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
            return unit;
        }

        public ConsoleColor BgColor => 
            Console.BackgroundColor;
        
        public ConsoleColor Color => 
            Console.ForegroundColor;
        
        public int Read() =>
            Console.Read();
        
        public string ReadLine() =>
            Console.ReadLine();
        
        public Unit WriteLine()
        {
            Console.WriteLine();
            return unit;
        }

        public Unit WriteLine(string value)
        {
            Console.WriteLine(value);
            return unit;
        }

        public Unit Write(string value)
        {
            Console.WriteLine();
            return unit;
        }
    }
}