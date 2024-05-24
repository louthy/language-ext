using System;
using System.IO;
using LanguageExt;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace TestBed;

public abstract record Op<A> : K<Op, A>;
public record ReadLinesOp<A>(string Path, Func<Seq<string>, A> Next): Op<A>;
public record WriteLinesOp<A>(string Path, Seq<string> Lines, Func<Unit, A> Next) : Op<A>;

public partial class Op : Functor<Op>
{
    public static K<Op, B> Map<A, B>(Func<A, B> f, K<Op, A> ma) =>
        ma switch
        {
            ReadLinesOp<A> (var path, var next)             => new ReadLinesOp<B>(path, x => f(next(x))),
            WriteLinesOp<A> (var path, var lines, var next) => new WriteLinesOp<B>(path, lines, x => f(next(x)))
        };
}

public partial class Op
{
    public static Free<Op, Seq<string>> readLines(string path) =>
        Free.lift(new ReadLinesOp<Seq<string>>(path, identity));

    public static Free<Op, Unit> writeLines(string path, Seq<string> lines) =>
        Free.lift(new WriteLinesOp<Unit>(path, lines, identity));
}

public static class FreeTests
{
    public static void Test()
    {
        var repr = from lines in Op.readLines("c:\\temp\\test.txt")
                   from _     in Op.writeLines("c:\\temp\\test2.txt", lines)
                   select unit;

        var comp = Interpret(repr);

        comp.Run();
    }

    static IO<A> Interpret<A>(K<Free<Op>, A> ma) =>
        ma switch
        {
            Pure<Op, A>(var value) => 
                IO.Pure(value),
            
            Bind<Op, A>(var bind) =>
                bind switch
                {
                    ReadLinesOp<Free<Op, A>> (var path, var next) => 
                        ReadLines(path).Map(next).Bind(Interpret),
                    
                    WriteLinesOp<Free<Op, A>> (var path, var lines, var next) => 
                        WriteLines(path, lines).Map(next).Bind(Interpret)
                }
        };

    static IO<Seq<string>> ReadLines(string path) =>
        IO.liftAsync(() => File.ReadAllLinesAsync(path)).Map(toSeq);

    static IO<Unit> WriteLines(string path, Seq<string> lines) =>
        IO.liftAsync(async () =>
                     {
                         await File.WriteAllLinesAsync(path, lines);
                         return unit;
                     });
}
