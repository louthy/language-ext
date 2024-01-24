using System;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Sys.Traits;

namespace LanguageExt.Sys
{
    /// <summary>
    /// DateTime IO 
    /// </summary>
    public static class Time<RT>
        where RT : HasTime<RT>
    {
        /// <summary>
        /// Current local date time
        /// </summary>
        public static Eff<RT, DateTime> now
        {
            [Pure, MethodImpl(AffOpt.mops)]
            get => default(RT).TimeEff.Map(static e => e.Now);
        }

        /// <summary>
        /// Current universal date time
        /// </summary>
        public static Eff<RT, DateTime> nowUTC
        {
            [Pure, MethodImpl(AffOpt.mops)] 
            get => default(RT).TimeEff.Map(static e => e.UtcNow);
        }

        /// <summary>
        /// Today's date 
        /// </summary>
        public static Eff<RT, DateTime> today
        {
            [Pure, MethodImpl(AffOpt.mops)]
            get => default(RT).TimeEff.Map(static e => e.Today);
        }

        /// <summary>
        /// Pause a task until a specified time
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<RT, Unit> sleepUntil(DateTime dt) =>
            cancelToken<RT>().Bind(t =>
                default(RT).TimeEff.Map(liftIO(async (TimeIO e) => await e.SleepUntil(dt, t))));

        /// <summary>
        /// Pause a task until for a specified length of time
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<RT, Unit> sleepFor(TimeSpan ts) =>
            cancelToken<RT>().Bind(t =>
                default(RT).TimeEff.Map(liftIO(async (TimeIO e) => await e.SleepFor(ts, t))));
    }
}
