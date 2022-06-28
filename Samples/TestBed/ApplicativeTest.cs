namespace TestBed;

using System;
using System.Diagnostics;
using LanguageExt;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

public static class ApplicativeTest
{
    static Aff<Unit> delay(int milliseconds) =>
        Aff(async () =>
        {
            await Task.Delay(milliseconds);
            return unit;
        });

    static Aff<int> parse(string str) =>
        from x in parseInt(str).ToAff()
        from _ in delay(1000)
        select x;

    static Aff<int> add(string sx, string sy) =>
        SuccessAff((int x, int y) => x + y) 
            .Apply(parse(sx))
            .Apply(parse(sy));

    public static async Task Test()
    {
        await Report(add("100", "200"));
        await Report(add("zzz", "yyy"));
    }

    static async Task Report<A>(Aff<A> ma)
    {
        var sw = Stopwatch.StartNew();
        var r = await ma.Run();
        sw.Stop();
        Console.WriteLine($"Result: {r} in {sw.ElapsedMilliseconds}ms");
    }
}
