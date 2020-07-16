using System;

namespace LanguageExt.Interfaces
{
    public interface ConsoleIO
    {
        ConsoleKeyInfo ReadKey();
        Unit Clear();
        int Read();
        string ReadLine();
        Unit WriteLine();
        Unit WriteLine(string value);
        Unit Write(string value);
        Unit SetBgColor(ConsoleColor color);
        Unit SetColor(ConsoleColor color);
        ConsoleColor BgColor { get; }
        ConsoleColor Color { get; }
    }
}
