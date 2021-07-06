using System;
using LanguageExt.Attributes;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Sys.Traits
{
    public interface ConsoleIO
    {
        Unit Clear();
        Option<ConsoleKeyInfo> ReadKey();
        Option<int> Read();
        Option<string> ReadLine();
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
        /// Access the console synchronous effect environment
        /// </summary>
        /// <returns>Console synchronous effect environment</returns>
        Eff<RT, ConsoleIO> ConsoleEff { get; }
    }
}
