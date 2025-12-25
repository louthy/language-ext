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

        var xs = insertSort(3, [1, 2, 4]);
    }

    public static Seq<int> insertSort(int value, Seq<int> sorted) =>
        sorted.IsEmpty || value < sorted[0] 
            ? value + sorted
            : sorted[0] + insertSort(value, sorted.Tail);
    
    public static void recurIsSameAsBind<M>(Func<K<M, int>, K<M, int>, bool>? equals = null)
        where M : Monad<M>
    {
        var example = toSeq(Range(1, 20000)).Strict();

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
                []          => M.Done<(int, Seq<int>), int>(pair.Total),
                var (x, xs) => M.Loop<(int, Seq<int>), int>((pair.Total + x, xs)) 
            };

        
        K<M, Next<(int Total, Seq<int> Values), int>> sumTail2((int Total, Seq<int> Values) pair) =>
            pair.Values switch
            {
                []          => M.Done<(int, Seq<int>), int>(pair.Total),
                var (x, xs) => M.Loop<(int, Seq<int>), int>((pair.Total + x, xs)) 
            };
        
        K<M, int> sumNoTail(Seq<int> values) =>
            values switch
            {
                []          => M.Pure(0),
                var (x, xs) => from t in sumNoTail(xs)
                               select t + x
            };
    }
}
