using LanguageExt.Common;

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
        from x in parseInt(str).ToEff(Error.New($"parse error: expected int, got: '{str}'"))
        from _ in delay(10000)
        select x;

    static Aff<int> add(string sa, string sb, string sc, string sd, string se, string sf) =>
        SuccessAff(curry<int, int, int, int, int, int, int>(addPure)) 
            .Apply(parse(sa))
            .Apply(parse(sb))
            .Apply(parse(sc))
            .Apply(parse(sd))
            .Apply(parse(se))
            .Apply(parse(sf));

    static int addPure(int a, int b, int c, int d, int e, int f) =>
        a + b + c + d + e + f;

    public static async Task Test()
    {
        await report(add("1", "2", "3", "4", "5", "6"));
        await report(add("a", "b", "c", "d", "e", "f"));
    }

    static async Task report<A>(Aff<A> ma)
    {
        var sw = Stopwatch.StartNew();
        var r = await ma.Run();
        sw.Stop();
        Console.WriteLine($"Result: {r} in {sw.ElapsedMilliseconds}ms");
    }
}
