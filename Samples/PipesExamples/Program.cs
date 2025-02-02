using System.Diagnostics;
using LanguageExt;
using LanguageExt.Pipes;
using LanguageExt.Pipes2;
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


var p = from _1 in writeLine("pre-yield")
        from _2 in ProducerT.yieldAll<IO, int>([100, 200, 300])
      //from _2 in ProducerT.yield<Try, int>(100)
        from _3 in writeLine("post-yield")
        select unit;

var o = from _1 in writeLine("pre-pipe")
        from x  in PipeT.awaiting<IO, int, int>()
        from _2 in writeLine($"piped-value: {x}")
        from _3 in PipeT.yield<IO, int, int>(x * 2)
        from _4 in writeLine("post-pipe")
        select unit;

var c = from _1 in writeLine("pre-await")
        from x  in ConsumerT.awaiting<IO, int>()
        from _2 in writeLine($"post-await: {x}")
        select unit;

var e = p | o | c;

var r = e.Run().Run();

Console.WriteLine("Done");

static IO<Unit> writeLine(object? value) =>
    IO.lift(() => Console.WriteLine(value));
