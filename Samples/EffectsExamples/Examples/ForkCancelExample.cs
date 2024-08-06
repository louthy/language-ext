using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Sys.Traits;
using LanguageExt.Traits;
using static LanguageExt.Prelude;
using static LanguageExt.UnitsOfMeasure;

namespace EffectsExamples;

/// <summary>
/// Process forking and cancelling example
/// </summary>
/// <remarks>
/// Forks a process that runs 10 times, summing a value each time.
/// If you press enter before the 10 iterations then the forked process will be cancelled
/// </remarks>
public class ForkCancelExample<RT>
    where RT : 
        Has<Eff<RT>, ConsoleIO>,
        Has<Eff<RT>, TimeIO>
{
    public static Eff<RT, Unit> main =>
        from frk in forkIO(inner).As()
        from key in Console<Eff<RT>, RT>.readKey
        from _1  in frk.Cancel
        from _2  in Console<Eff<RT>, RT>.writeLine("done")
        select unit;

    static Eff<RT, Unit> inner =>
        from x in sum
        from _ in Console<Eff<RT>, RT>.writeLine($"total: {x}")
        select unit;

    static Eff<RT, int> sum =>
        digit.FoldIO(Schedule.recurs(9) | Schedule.spaced(1 * second), 0, (s, x) => s + x).As();

    static Eff<RT, int> digit =>
        from one in SuccessEff<RT, int>(1)
        from _ in Console<Eff<RT>, RT>.writeLine("*")
        select one;
}
