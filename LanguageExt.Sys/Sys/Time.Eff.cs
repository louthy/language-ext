using System;
using LanguageExt.Traits;
using LanguageExt.Sys.Traits;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.Sys;

/// <summary>
/// DateTime IO 
/// </summary>
public static class Time<RT>
    where RT : 
        Has<Eff<RT>, TimeIO>
{
    /// <summary>
    /// Current local date time
    /// </summary>
    public static Eff<RT, DateTime> now =>
        Time<Eff<RT>, RT>.now.As();

    /// <summary>
    /// Current universal date time
    /// </summary>
    public static Eff<RT, DateTime> nowUTC =>
        Time<Eff<RT>, RT>.nowUTC.As();

    /// <summary>
    /// Today's date 
    /// </summary>
    public static Eff<RT, DateTime> today =>
        Time<Eff<RT>, RT>.today.As();

    /// <summary>
    /// Pause a task until a specified time
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Eff<RT, Unit> sleepUntil(DateTime dt) =>
        Time<Eff<RT>, RT>.sleepUntil(dt).As();

    /// <summary>
    /// Pause a task until for a specified length of time
    /// </summary>
    [Pure, MethodImpl(EffOpt.mops)]
    public static Eff<RT, Unit> sleepFor(TimeSpan ts) =>
        Time<Eff<RT>, RT>.sleepFor(ts).As();
}
