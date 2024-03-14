using LanguageExt.Common;

namespace TestBed;

using System;
using System.Diagnostics;
using LanguageExt;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

public static class ApplicativeTest
{
    static Eff<Unit> delay(int milliseconds) =>
        liftIO(async () =>
            {
                await Task.Delay(milliseconds);
                return unit;
            });

    static Eff<int> parse(string str) =>
        from x in parseInt(str).ToEff(Error.New($"parse error: expected int, got: '{str}'"))
        from _ in delay(10000)
        select x;

    static Eff<int> add(string sa, string sb, string sc, string sd, string se, string sf) =>
        SuccessEff(curry<int, int, int, int, int, int, int>(addPure)) 
            .Apply(parse(sa))
            .Apply(parse(sb))
            .Apply(parse(sc))
            .Apply(parse(sd))
            .Apply(parse(se))
            .Apply(parse(sf));

    static int addPure(int a, int b, int c, int d, int e, int f) =>
        a + b + c + d + e + f;

    public static void Test()
    {
        report(add("1", "2", "3", "4", "5", "6"));
        report(add("a", "b", "c", "d", "e", "f"));
    }

    static void report<A>(Eff<A> ma)
    {
        var sw = Stopwatch.StartNew();
        var r = ma.Run();
        sw.Stop();
        Console.WriteLine($"Result: {r} in {sw.ElapsedMilliseconds}ms");
    }
}
