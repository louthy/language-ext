using System;
using System.IO;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Interfaces;

namespace LanguageExt
{
    /// <summary>
    /// IO prelude
    /// </summary>
    public static partial class IO
    {
        /// <summary>
        /// Time IO 
        /// </summary>
        public static class Time
        {
            /// <summary>
            /// Current date time
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static SIO<RT, DateTime> now<RT>()
                where RT : struct, HasTime<RT> =>
                default(RT).TimeSIO.Map(e => e.Now);

            /// <summary>
            /// Today's date 
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static SIO<RT, DateTime> today<RT>()
                where RT : struct, HasTime<RT> =>
                default(RT).TimeSIO.Map(e => e.Today);

            /// <summary>
            /// Pause a task until a specified time
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static IO<RT, Unit> sleepUntil<RT>(DateTime dt)
                where RT : struct, HasTime<RT> =>
                default(RT).TimeIO.MapAsync(e => e.SleepUntil(dt));

            /// <summary>
            /// Pause a task until for a specified length of time
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static IO<RT, Unit> sleepFor<RT>(TimeSpan ts)
                where RT : struct, HasTime<RT> =>
                default(RT).TimeIO.MapAsync(e => e.SleepFor(ts));
        }
    }
}
