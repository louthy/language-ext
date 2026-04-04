////////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                    //
//                                                                                                    //
//     NOTE: This is just my scratch pad for quickly testing stuff, not for human consumption         //
//                                                                                                    //
//                                                                                                    //
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Sys;
using static LanguageExt.Prelude;

namespace TestBed;

public class SequenceParallelTest
{
    public static void Run()
    {
        ParallelTests().GetAwaiter().GetResult();
        //SequenceParallelRandomDelayTest().GetAwaiter().GetResult();
    }

    public static async Task SequenceParallelRandomDelayTest()
    {
        var sw = Stopwatch.StartNew();
        var input = Seq(1, 2, 3, 2, 5, 1, 1, 2, 3, 2, 1, 2, 4, 2, 1, 5, 6, 1, 3, 6, 2);
	
        var eitherIO = input.Map(DoDelay).Traverse(x => x).As();
        var either = eitherIO.Run().As().Run();
        
        Debug.Assert(either.IsRight);
        either.IfRight(right => Debug.Assert(right == input));
        
        sw.Stop();

        System.Console.WriteLine(sw.Elapsed);
    }

    static EitherT<string, IO, int> DoDelay(int seconds)
    {
        return liftIO(() => F(seconds));
        static async Task<Either<string, int>> F(int seconds)
        {
            await Task.Delay(seconds * 1000);
            return seconds;
        }
    }
    
    static async Task ParallelTests()
    {
        var sum = Range(1, 10000).Sum();

        var seq = toSeq(Range(1, 10000));

        var tasks = new List<Task<int>>();
        foreach(var x in Range(1, 1000))
        {
            tasks.Add(Task.Run(() => seq.Sum()));
        }

        await Task.WhenAll(tasks.ToArray());

        var results = tasks.Select(t => t.Result).ToArray();

        seq.Iter((i, x) =>
                 {
                     if (x != i + 1)
                     {
                         System.Console.WriteLine($"Invalid value in the sequence at index {i}");
                     }
                 }); 

        foreach (var result in results)
        {
            if (result != sum)
            {
                System.Console.WriteLine($"Result is {result}, should be: {sum}");
            }
        }
        System.Console.WriteLine("Done");
    }
}
