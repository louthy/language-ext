using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using LanguageExt;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace TestBed;

public class FoldPerfTests
{
    public static void Run()
    {
        const int max   = 1000000;
        var       arr   = Range(1, max).ToSeq().ToArr();
        var       array = arr.ToArray();

        Bench([
                  ("ForeachSum", () => ForeachSum(arr)),
                  ("ArraySum", () => ArraySum(array)),
                  ("FoldableSum", () => FoldableSum(arr))
              ]);

        /*Bench([
                  ("ForeachCount", () => ForeachCount(arr)),
                  ("ArrayCount", () => ArrayCount(array)),
                  ("FoldableCount", () => FoldableCount(arr))
              ]);*/
    }
    
    static int FoldableCount(Arr<int> arr) =>
        Count<Arr, int, Arr.FoldState>(arr);

    static int ForeachCount(Arr<int> arr)
    {
        var t = 0;
        foreach (var _ in arr)
        {
            t++;
        }
        return t;
    }

    static int ArrayCount(int[] arr)
    {
        var t = 0;
        foreach (var _ in arr)
        {
            t++;
        }
        return t;
    }

    static int FoldableSum(Arr<int> arr) =>
        Sum<Arr, Arr.FoldState>(arr);

    static int ForeachSum(Arr<int> arr)
    {
        var t = 0;
        foreach (var x in arr)
        {
            t += x;
        }
        return t;
    }

    static int ArraySum(int[] arr)
    {
        var t = 0;
        foreach (var x in arr)
        {
            t += x;
        }
        return t;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static int Count<T, A, FS>(K<T, A> ta)
        where T : Foldable<T, FS>
        where FS : allows ref struct
    {
        FS foldState = default!;
        T.FoldStepSetup(ta, ref foldState);
        var state = 0;
        while (T.FoldStep(ta, ref foldState, out _))
        {
            state++;
        }
        return state;
    }    

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static int Sum<T, FS>(K<T, int> ta)
        where T : Foldable<T, FS>
        where FS : allows ref struct
    {
        FS foldState = default!;
        T.FoldStepSetup(ta, ref foldState);
        var state = 0;
        while (T.FoldStep(ta, ref foldState, out var x))
        {
            state += x;
        }
        return state;
    }    
    
    public static void Bench(Seq<(string Name, Action Action)> actions)
    {
        for (var warmup = 0; warmup < 2000; warmup++)
        {
            if(warmup % 100 == 0) Console.WriteLine($"Warmup #{warmup}");
            foreach (var action in actions) action.Action();
        }

        Seq<Seq<(string Name, TimeSpan Duration)>> runs = [];
        
        for(var run = 0; run < 2000; run++)
        {
            if(run % 100 == 0) Console.WriteLine($"Live run #{run}");
            var durations = Seq<(string Name, TimeSpan Duration)>();

            foreach (var action in actions)
            {
                var sw = Stopwatch.StartNew(); 
                action.Action();
                sw.Stop();
                var ts= sw.Elapsed;
                durations = durations.Add((action.Name, ts));
            }
            runs = runs.Add(durations);
        }

        var ix = 0;
        foreach (var action in actions)
        {
            var total = 0.0;
            foreach (var run in runs)
            {
                total += run[ix].Duration.TotalNanoseconds;
            }
            var average = total / runs.Count;
            Console.WriteLine($"{action.Name}: {average:N0}ns");
            ix++;
        }
    }
}
