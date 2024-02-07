using System.Diagnostics;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Sys;
using static LanguageExt.Prelude;

namespace TestBed;

public class SequenceParallelTest
{
    public static void Run()
    {
        SequenceParallelRandomDelayTest().GetAwaiter().GetResult();
    }

    public static async Task SequenceParallelRandomDelayTest()
    {
        var sw = Stopwatch.StartNew();
        var input = Seq(1, 2, 3, 2, 5, 1, 1, 2, 3, 2, 1, 2, 4, 2, 1, 5, 6, 1, 3, 6, 2);
	
        var ma = input.Select(DoDelay).SequenceParallel();

        Debug.Assert(await ma.IsRight);
        await ma.IfRight(right => Debug.Assert(right.SequenceEqual(input)));
        
        sw.Stop();

        System.Console.WriteLine(sw.Elapsed);
    }

    static EitherAsync<string, int> DoDelay(int seconds)
    {
        return F(seconds).ToAsync();
        static async Task<Either<string, int>> F(int seconds)
        {
            await Task.Delay(seconds * 1000);
            return seconds;
        }
    }
}
