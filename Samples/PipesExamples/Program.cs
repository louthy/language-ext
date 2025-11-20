using System.Diagnostics;
using System.Net.Sockets;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Pipes;
using LanguageExt.Traits;
using static LanguageExt.Pipes.ProducerT;
using static LanguageExt.Pipes.PipeT;
using static LanguageExt.Pipes.ConsumerT;
using static LanguageExt.Prelude;

var prod = yieldAll<IO, int>(Range(1, 100));

var pipe = from x in awaiting<IO, int, int>()
           from _ in x == 10
                         ? endOfStream<int, int>()
                         : yield<IO, int, int>(x)
           select unit;

var cons = from x in awaiting<IO, int>()
           from _ in writeLine(x)
           select unit;

var effect = prod | pipe | cons;

var result = effect.RunToEnd().Run();

static PipeT<IN, OUT, IO, Unit> endOfStream<IN, OUT>() =>
    error<IN, OUT, IO, Unit>(Errors.EndOfStream);

static IO<Unit> writeLine(object? value) =>
    IO.lift(() => Console.Write($"{value} "));

public record Slice<A>(int Length, int Offset, byte[] Buffer);

public static class EffectExtensions
{
    public static K<M, Unit> RunToEnd<M>(this EffectT<M, Unit> effect)
        where M : MonadIO<M>, Fallible<M> =>
        effect.Run()
      | @catch(Errors.EndOfStream, pure<M, Unit>(unit));
}


/*
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
    */
