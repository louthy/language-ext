////////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                    //
//                                                                                                    //
//     NOTE: This is just my scratch pad for quickly testing stuff, not for human consumption         //
//                                                                                                    //
//                                                                                                    //
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using LanguageExt;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace TestBed;

public class RangePerfTests
{
    public static void Run()
    {
        const int max   = 1000000;
        var       range = Range(1, max);
        var       sysRange = SystemRange(1, max);

        /*
        Bench([
                  ("ForeachSum", () => ForeachSum(sysRange)),
                  ("LinqSum", () => LinqSum(sysRange)),
                  ("FoldableSum", () => FoldableSum(range))
              ]);
              */

        Bench([
                  ("ForeachCount", () => ForeachCount(sysRange)),
                  ("LinqCount", () => LinqCount(sysRange)),
                  ("FoldableCount", () => FoldableCount(range))
              ]);
    }

    static IEnumerable<int> SystemRange(int start, int count)
    {
        for (var i = start; i < start + count; i++)
        {
            yield return i;
        }
    }

    static int ForeachSum(IEnumerable<int> range)
    {
        var t = 0;
        foreach (var x in range)
        {
            t += x;
        }
        return t;
    }

    static int ForeachCount(IEnumerable<int> range)
    {
        var t = 0;
        foreach (var _ in range)
        {
            t ++;
        }
        return t;
    }

    static int LinqCount(IEnumerable<int> range) =>
        range.Count();

    static int FoldableCount(Range<int> range) =>
        Foldable.count(range);

    static int FoldableSum(Range<int> range) =>
        Foldable.sum(range);
 
    static int LinqSum(IEnumerable<int> range) =>
        range.Sum();

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    static int Count<T, A, FS>(K<T, A> ta)
        where T : Foldable<T, FS>
        where FS : allows ref struct
    {
        var foldState = T.StepSetup(ta);
        var state = 0;
        while (T.Step(ta, ref foldState, out _))
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
        var foldState = T.StepSetup(ta);
        var state     = 0;
        while (T.Step(ta, ref foldState, out var x))
        {
            state += x;
        }
        return state;
    }    
    
    public static void Bench(Seq<(string Name, Action Action)> actions)
    {
        for (var warmup = 0; warmup < 1000; warmup++)
        {
            if(warmup % 100 == 0) Console.WriteLine($"Warmup #{warmup}");
            foreach (var action in actions) action.Action();
        }

        Seq<Seq<(string Name, TimeSpan Duration)>> runs = [];
        
        for(var run = 0; run < 1000; run++)
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
