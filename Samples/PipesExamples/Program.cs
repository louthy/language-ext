using System.Diagnostics;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Pipes;
using LanguageExt.Traits;
using static LanguageExt.Pipes.ProducerT;
using static LanguageExt.Pipes.PipeT;
using static LanguageExt.Pipes.ConsumerT;
using static LanguageExt.Prelude;

var p = yieldAll<IO, int>(Range(1, 10000000));

var o = foldUntil(Time: Schedule.spaced(10) | Schedule.recurs(3), 
                  Fold: (s, v) => s + v,
                  Pred: v => v.Value % 10000 == 0,
                  Init: 0,
                  Item: awaiting<IO, int, int>());

var c = from x in awaiting<IO, int>()
        from _ in writeLine($"{x}")
        select unit;

var e = p | o | c;

var r = e.Run().Run();

Console.WriteLine("Done");

static IO<Unit> writeLine(object? value) =>
    IO.lift(() => Console.WriteLine(value));
