using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Live;

public record ConsoleIO : Sys.Traits.ConsoleIO
{
    public static readonly Sys.Traits.ConsoleIO Default =
        new ConsoleIO();

    public IO<Option<ConsoleKeyInfo>> ReadKey() =>
        lift(() => Optional(Console.ReadKey()));

    public IO<Unit> Clear() =>
        lift(Console.Clear);

    public IO<Unit> SetBgColor(ConsoleColor color) =>
        lift(() => { Console.BackgroundColor = color;});

    public IO<Unit> SetColor(ConsoleColor color) =>
        lift(() => { Console.ForegroundColor = color; });

    public IO<Unit> ResetColor() =>
        lift(Console.ResetColor);

    public IO<ConsoleColor> BgColor=>
        lift(() => Console.BackgroundColor);
        
    public IO<ConsoleColor> Color=>
        lift(() => Console.ForegroundColor);

    public IO<Option<int>> Read() =>
        lift(() =>
             {
                 var k = Console.Read();
                 return k == -1
                            ? None
                            : Some(k);
             });

    public IO<Option<string>> ReadLine()=>
        lift(() => Optional(Console.ReadLine()));

    public IO<Unit> WriteLine() =>
        lift(Console.WriteLine);

    public IO<Unit> WriteLine(string value) =>
        lift(() => Console.WriteLine(value));

    public IO<Unit> Write(string value) =>
        lift(() => Console.Write(value));
}
