using System.Diagnostics;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Pipes;
using LanguageExt.Traits;
using static LanguageExt.Pipes.ProducerT;
using static LanguageExt.Pipes.PipeT;
using static LanguageExt.Pipes.ConsumerT;
using static LanguageExt.Prelude;

/*var op = from _1 in Producer.lift<int, IO, Unit>(writeLine("pre-yield"))
         from _2 in Producer.yieldAll<IO, int>([100, 200, 300])
         from _3 in writeLine("post-yield")
         select unit;
        
var oc = from _1 in Consumer.lift<int, IO, Unit>(writeLine("pre-await"))
         from x  in Consumer.awaiting<IO, int>()
         from _2 in Consumer.lift<int, IO, Unit>(writeLine($"post-await: {x}"))
         select unit;

var oe = op | oc;

var or = oe.RunEffect().Run();

Console.WriteLine();*/

/*
var w1 = ProducerT.liftM<int, Try, Unit>(writeLine("post yield"));
var p1 = ProducerT.yield<Try, int>(100).Bind(_ => w1);
var c1 = ConsumerT.awaitIgnore<Try, int>();
var e1 = p1 | c1;
var r1 = e1.Run().Run();
*/


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
