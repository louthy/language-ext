using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Test.Implementations;

/// <summary>
/// Encapsulated in-memory console
/// No public API exists for this.  Use Sys.IO.Console.* to interact with the console
/// </summary>
/// <remarks>
/// Primarily used for testing (for use with TestRuntime or your own testing runtime)
/// </remarks>
public record ConsoleIO(MemoryConsole mem) : Sys.Traits.ConsoleIO
{
    public IO<Option<ConsoleKeyInfo>> ReadKey() =>
        lift(() => mem.ReadKey());

    public IO<Unit> Clear() =>
        lift(() => mem.Clear());

    public IO<Unit> SetBgColor(ConsoleColor color) =>
        lift(() => mem.SetBgColor(color));

    public IO<Unit> SetColor(ConsoleColor color) =>
        lift(() => mem.SetColor(color));

    public IO<Unit> ResetColor() =>
        lift(() => mem.ResetColor());

    public IO<ConsoleColor> BgColor => 
        lift(() => mem.BgColor);
        
    public IO<ConsoleColor> Color => 
        lift(() => mem.Color);
        
    public IO<Option<int>> Read() =>
        lift(() => mem.Read());
        
    public IO<Option<string>> ReadLine() =>
        lift(() => mem.ReadLine());

    public IO<Unit> WriteLine() =>
        lift(() => mem.WriteLine());

    public IO<Unit> WriteLine(string value) =>
        lift(() => mem.WriteLine(value));

    public IO<Unit> Write(string value) =>
        lift(() => mem.Write(value));
}
