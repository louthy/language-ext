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

var o = foldUntil(Time: Schedule.recurs(50), 
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

public record DbEnv;
public record Db<A>(ReaderT<DbEnv, IO, A> RunDb) : K<Db, A>
{
    public Db<B> Select<B>(Func<A, B> m) => this.Kind().Select(m).As();
    public Db<C> SelectMany<B, C>(Func<A, K<Db, B>> b, Func<A, B, C> p) => this.Kind().SelectMany(b, p).As();
    public Db<C> SelectMany<B, C>(Func<A, K<IO, B>> b, Func<A, B, C> p) => this.Kind().SelectMany(b, p).As();
}

public static class DbExtensions
{
    public static Db<A> As<A>(this K<Db, A> ma) =>
        (Db<A>)ma;
}
public class Db : Monad<Db>, Fallible<Db>, Readable<Db, DbEnv>
{
    public static K<Db, B> Bind<A, B>(K<Db, A> ma, Func<A, K<Db, B>> f) =>
        new Db<B>(ma.As().RunDb.Bind(x => f(x).As().RunDb));

    public static K<Db, B> Map<A, B>(Func<A, B> f, K<Db, A> ma) => 
        new Db<B>(ma.As().RunDb.Map(f));

    public static K<Db, A> Pure<A>(A value) => 
        new Db<A>(ReaderT.pure<DbEnv, IO, A>(value));

    public static K<Db, B> Apply<A, B>(K<Db, Func<A, B>> mf, K<Db, A> ma) => 
        new Db<B>(mf.As().RunDb.Apply(ma.As().RunDb));

    public static K<Db, A> Fail<A>(Error error) => 
        new Db<A>(ReaderT.liftIO<DbEnv, IO, A>(error));

    public static K<Db, A> Catch<A>(K<Db, A> fa, Func<Error, bool> Predicate, Func<Error, K<Db, A>> Fail) =>
        from env in Readable.ask<Db, DbEnv>()
        from res in fa.As()
                      .RunDb
                      .runReader(env)
                      .Catch(Predicate, x => Fail(x).As().RunDb.runReader(env)) 
        select res;

    public static K<Db, A> Asks<A>(Func<DbEnv, A> f) => 
        new Db<A>(ReaderT.asks<IO, A, DbEnv>(f));

    public static K<Db, A> Local<A>(Func<DbEnv, DbEnv> f, K<Db, A> ma) => 
        new Db<A>(ReaderT.local(f, ma.As().RunDb));

    public static K<Db, A> LiftIO<A>(IO<A> ma) => 
        new Db<A>(ReaderT.liftIO<DbEnv, IO, A>(ma));

    public static K<Db, B> MapIO<A, B>(K<Db, A> ma, Func<IO<A>, IO<B>> f) => 
        new Db<B>(ma.As().RunDb.MapIO(f).As());

    public static K<Db, IO<A>> ToIO<A>(K<Db, A> ma) =>
        ma.MapIO(IO.pure);
}
