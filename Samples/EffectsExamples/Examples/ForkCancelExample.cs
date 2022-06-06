using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Sys.Traits;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace EffectsExamples
{
    /// <summary>
    /// Process forking and cancelling example
    /// </summary>
    /// <remarks>
    /// Forks a process that runs 10 times, summing a value each time.
    /// If you press enter before the 10 iterations then the forked process will be cancelled
    /// </remarks>
    public class ForkCancelExample<RT>
        where RT : struct,
        HasCancel<RT>,
        HasConsole<RT>,
        HasTime<RT>
    {
        public static Aff<RT, Unit> main =>
            from cancel in fork(inner)
            from key in Console<RT>.readKey
            from _1 in cancel
            from _2 in Console<RT>.writeLine("done")
            select unit;

        static Aff<RT, Unit> inner =>
            from x in sum
            from _ in Console<RT>.writeLine($"total: {x}")
            select unit;

        static Aff<RT, int> sum =>
            digit.Fold(Schedule.recurs(10) | Schedule.spaced(1 * second), 0, (s, x) => s + x);

        static Aff<RT, int> digit =>
            from one in SuccessAff<RT, int>(1)
            from _ in Console<RT>.writeLine("*")
            select one;
    }
}
