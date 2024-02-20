using System;
using LanguageExt.Traits;
using LanguageExt.Sys.Traits;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.Sys;

/// <summary>
/// DateTime IO 
/// </summary>
public static class Time<M, RT>
    where M : Reader<M, RT>, Monad<M>
    where RT : HasTime<RT>
{
    static readonly K<M, TimeIO> trait = 
        Reader.asks<M, RT, IO<TimeIO>>(e => e.TimeIO).Bind(M.LiftIO);

    /// <summary>
    /// Current local date time
    /// </summary>
    public static K<M, DateTime> now =>
        trait.Bind(e => e.Now);

    /// <summary>
    /// Current universal date time
    /// </summary>
    public static K<M, DateTime> nowUTC =>
        trait.Bind(e => e.UtcNow);

    /// <summary>
    /// Today's date 
    /// </summary>
    public static K<M, DateTime> today =>
        trait.Bind(e => e.Today);

    /// <summary>
    /// Pause a task until a specified time
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static K<M, Unit> sleepUntil(DateTime dt) =>
        trait.Bind(e => e.SleepUntil(dt));

    /// <summary>
    /// Pause a task until for a specified length of time
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static K<M, Unit> sleepFor(TimeSpan ts) =>
        trait.Bind(e => e.SleepFor(ts));
}
