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
    where M : 
        MonadIO<M>
    where RT : 
        Has<M, TimeIO>
{
    static readonly K<M, TimeIO> timeIO =
        Has<M, RT, TimeIO>.ask;

    /// <summary>
    /// Current local date time
    /// </summary>
    public static K<M, DateTime> now =>
        timeIO.Bind(e => e.Now);

    /// <summary>
    /// Current universal date time
    /// </summary>
    public static K<M, DateTime> nowUTC =>
        timeIO.Bind(e => e.UtcNow);

    /// <summary>
    /// Today's date 
    /// </summary>
    public static K<M, DateTime> today =>
        timeIO.Bind(e => e.Today);

    /// <summary>
    /// Pause a task until a specified time
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static K<M, Unit> sleepUntil(DateTime dt) =>
        timeIO.Bind(e => e.SleepUntil(dt));

    /// <summary>
    /// Pause a task until for a specified length of time
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static K<M, Unit> sleepFor(TimeSpan ts) =>
        timeIO.Bind(e => e.SleepFor(ts));
}
