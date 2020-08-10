using System;
using LanguageExt.Attributes;

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

    /// <summary>
    /// Type-class giving a struct the trait of supporting Console IO
    /// </summary>
    /// <typeparam name="RT">Runtime</typeparam>
    [Typeclass("*")]
    public interface HasConsole<RT> : HasCancel<RT>
        where RT : struct, HasCancel<RT>
    {
        /// <summary>
        /// Access the console IO environment
        /// </summary>
        /// <returns>Console IO environment</returns>
        IO<RT, ConsoleIO> ConsoleIO { get; }

        /// <summary>
        /// Access the console SIO environment
        /// </summary>
        /// <returns>Console SIO environment</returns>
        SIO<RT, ConsoleIO> ConsoleSIO { get; }
    }
}
