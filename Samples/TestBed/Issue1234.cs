using System;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Sys.Live;

namespace TestBed;

public class Issue1234
{
    public static void Test()
    {
        ignore(Main.Run(Runtime.New()).Result);
    }
    
    public static Aff<Runtime, Unit> Main => 
        repeat(
            from _1 in Delay(TimeSpan.FromMilliseconds(10)).Timeout(TimeSpan.FromSeconds(5))
            from _2 in Console<Runtime>.writeLine($"{DateTime.Now:hh:mm:ss}")
            select unit);

    private static Aff<Runtime, Unit> Delay(TimeSpan delay) => Aff<Runtime, Unit>(async _ =>
    {
        await Task.Delay(delay);
        return unit;
    });
}
