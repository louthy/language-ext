using LanguageExt;
using LanguageExt.Pipes;
using LanguageExt.Pipes2;
using static LanguageExt.Prelude;

var op = from _1 in Producer.lift<int, IO, Unit>(writeLine("pre-yield"))
         from _2 in Producer.yieldAll<IO, int>([100, 200, 300])
         from _3 in writeLine("post-yield")
         select unit;
        
var oc = from _1 in Consumer.lift<int, IO, Unit>(writeLine("pre-await"))
         from x  in Consumer.awaiting<IO, int>()
         from _2 in Consumer.lift<int, IO, Unit>(writeLine($"post-await: {x}"))
         select unit;

var oe = op | oc;

var or = oe.RunEffect().Run();

Console.WriteLine();

var p = from _1 in ProducerT.liftM<int, IO, Unit>(writeLine("pre-yield"))
        from _2 in ProducerT.yieldAll<IO, int>([100, 200, 300])
        from _x in ProducerT.pure<int, IO, string>("Hello")
        from _3 in writeLine("post-yield")
        select unit;
        
var c = from _1 in ConsumerT.liftM<int, IO, Unit>(writeLine("pre-await"))
        from x  in ConsumerT.awaiting<IO, int>()
        from _2 in ConsumerT.liftM<int, IO, Unit>(writeLine($"post-await: {x}"))
        select unit;

var e = p | c;

var r = e.Run().Run();

Console.WriteLine("Done");

static IO<Unit> writeLine(object? value) =>
    IO.lift(() => Console.WriteLine(value));
