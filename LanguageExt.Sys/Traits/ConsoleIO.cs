using System;
using LanguageExt.Attributes;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Sys.Traits;

public interface ConsoleIO
{
    IO<Unit> Clear();
    IO<Option<ConsoleKeyInfo>> ReadKey();
    IO<Option<int>> Read();
    IO<Option<string>> ReadLine();
    IO<Unit> WriteLine();
    IO<Unit> WriteLine(string value);
    IO<Unit> Write(string value);
    IO<Unit> SetBgColor(ConsoleColor color);
    IO<Unit> SetColor(ConsoleColor color);

    /// <summary>
    /// Sets the foreground and background console colors to their defaults.
    /// </summary>
    IO<Unit> ResetColor();

    IO<ConsoleColor> BgColor { get; }
    IO<ConsoleColor> Color { get; }
}

/// <summary>
/// Type-class giving a struct the trait of supporting Console IO
/// </summary>
/// <typeparam name="RT">Runtime</typeparam>
[Trait("*")]
public interface HasConsole<RT> : HasIO<RT>
    where RT : HasConsole<RT>
{
    /// <summary>
    /// Access the console synchronous effect environment
    /// </summary>
    /// <returns>Console synchronous effect environment</returns>
    Eff<RT, ConsoleIO> ConsoleEff { get; }
}
