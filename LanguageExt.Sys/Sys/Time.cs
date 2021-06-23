using System;
using System.IO;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Effects.Traits;
using LanguageExt.Sys.Traits;

namespace LanguageExt
{
    /// <summary>
    /// Time IO 
    /// </summary>
    public static class Time
    {
        /// <summary>
        /// Current date time
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<RT, DateTime> now<RT>()
            where RT : struct, HasTime<RT> =>
            default(RT).TimeEff.Map(e => e.Now);

        /// <summary>
        /// Today's date 
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<RT, DateTime> today<RT>()
            where RT : struct, HasTime<RT> =>
            default(RT).TimeEff.Map(e => e.Today);

        /// <summary>
        /// Pause a task until a specified time
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<RT, Unit> sleepUntil<RT>(DateTime dt)
            where RT : struct, HasTime<RT> =>
            default(RT).TimeEff.MapAsync(e => e.SleepUntil(dt));

        /// <summary>
        /// Pause a task until for a specified length of time
        /// </summary>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Aff<RT, Unit> sleepFor<RT>(TimeSpan ts)
            where RT : struct, HasTime<RT> =>
            default(RT).TimeEff.MapAsync(e => e.SleepFor(ts));
    }
}
