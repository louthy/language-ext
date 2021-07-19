using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Live
{
    public readonly struct ConsoleIO : Sys.Traits.ConsoleIO
    {
        public readonly static Sys.Traits.ConsoleIO Default =
            new ConsoleIO();

        public Option<ConsoleKeyInfo> ReadKey() =>
            System.Console.ReadKey();

        public Unit Clear()
        {
            System.Console.Clear();
            return unit;
        }

        public Unit SetBgColor(ConsoleColor color)
        {
            System.Console.BackgroundColor = color;
            return unit;
        }

        public Unit SetColor(ConsoleColor color)
        {
            System.Console.ForegroundColor = color;
            return unit;
        }

        public ConsoleColor BgColor => 
            System.Console.BackgroundColor;
        
        public ConsoleColor Color => 
            System.Console.ForegroundColor;

        public Option<int> Read()
        {
            var k = System.Console.Read();
            return k == -1
                       ? None
                       : k;
        }

        public Option<string> ReadLine() =>
            System.Console.ReadLine();
        
        public Unit WriteLine()
        {
            System.Console.WriteLine();
            return unit;
        }

        public Unit WriteLine(string value)
        {
            System.Console.WriteLine(value);
            return unit;
        }

        public Unit Write(string value)
        {
            System.Console.Write(value);
            return unit;
        }
    }
}
