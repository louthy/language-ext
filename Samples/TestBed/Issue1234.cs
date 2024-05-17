using System;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Sys;
using static LanguageExt.Prelude;
using LanguageExt.Sys.Live;

namespace TestBed;

public class Issue1234
{
    public static void Test() =>
        ignore(Main.Run(Runtime.New(), EnvIO.New()));

    public static Eff<Runtime, Unit> Main => 
        repeat(
            from _1 in Delay(TimeSpan.FromMilliseconds(10)).Timeout(TimeSpan.FromSeconds(5))
            from _2 in Console<Eff<Runtime>, Runtime>.writeLine($"{DateTime.Now:hh:mm:ss}")
            select unit);

    static Eff<Runtime, Unit> Delay(TimeSpan delay) =>
        liftEff<Runtime, Unit>(
            async _ =>
            {
                await Task.Delay(delay);
                return unit;
            });
}
