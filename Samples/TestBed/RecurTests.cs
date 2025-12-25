using System;
using System.Diagnostics;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;


namespace TestBed;

public class RecurTests
{
    public static void Run()
    {
        recurIsSameAsBind<Seq>();
        recurIsSameAsBind<Iterable>();
        recurIsSameAsBind<Either<Error>>();
        recurIsSameAsBind<Option>();
        recurIsSameAsBind<Fin>();
        recurIsSameAsBind<Try>((mx, my) => mx.Run() == my.Run()) ;
    }
    
    public static void recurIsSameAsBind<M>(Func<K<M, int>, K<M, int>, bool>? equals = null)
        where M : Monad<M>
    {
        var example = toSeq(Range(1, 40000)).Strict();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("------------------------------------------------------------------------------------------");
        Console.WriteLine($"{typeof(M).Name}");
        Console.ForegroundColor = ConsoleColor.White;
        
        Console.WriteLine("Testing Monad.recur");
        var sw1     = Stopwatch.StartNew();
        var result1 = Monad.recur((0, example), sumTail);
        sw1.Stop();
        Console.WriteLine($"{sw1.Elapsed.Nanoseconds} ns");

        Console.WriteLine();

        Console.WriteLine("Testing general recursion");
        var sw2     = Stopwatch.StartNew();
        var result2 = sumNoTail(example);
        sw2.Stop();
        Console.WriteLine($"{sw2.Elapsed.Nanoseconds} ns");

        equals ??= (fa, fb) => fa.Equals(fb);
        
        if (!equals(result1, result2))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Monad trait implementation for {typeof(M).Name}.Recur gives a different "     +
                              $"result to the equivalent recursive {typeof(M).Name}.Bind.  This suggests "   +
                              $"an implementation bug, most likely in {typeof(M).Name}.Recur, but possibly " +
                              $"in {typeof(M).Name}.Bind.");
            Console.ForegroundColor = ConsoleColor.White;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("SUCCESS");
            Console.ForegroundColor = ConsoleColor.White;
        }
        
        K<M, Next<(int Total, Seq<int> Values), int>> sumTail((int Total, Seq<int> Values) pair) =>
            pair.Values switch
            {
                []          => M.Pure(Next.Done<(int, Seq<int>), int>(pair.Total)),
                var (x, xs) => M.Pure(Next.Loop<(int, Seq<int>), int>((pair.Total + x, xs))) 
            };

        K<M, int> sumNoTail(Seq<int> values) =>
            values switch
            {
                [var x]     => M.Pure(x),
                var (x, xs) => sumNoTail(xs) * (t => x + t)
            };
    }
}
