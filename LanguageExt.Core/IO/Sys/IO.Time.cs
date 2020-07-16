using System;
using System.IO;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

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
            public static SIO<Runtime,DateTime> now
            {
                [Pure, MethodImpl(IO.mops)] 
                get => Runtime.senv.Map(e => e.Time.Now);
            }

            /// <summary>
            /// Current date time
            /// </summary>
            public static IO<Runtime,DateTime> nowAsync
            {
                [Pure, MethodImpl(IO.mops)] 
                get => now.ToAsync();
            }

            /// <summary>
            /// Today's date 
            /// </summary>
            public static SIO<Runtime,DateTime> today 
            { 
                [Pure, MethodImpl(IO.mops)]
                get => Runtime.senv.Map(e => e.Time.Today);
            }

            /// <summary>
            /// Pause a task until a specified time
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static IO<Runtime, Unit> sleepUntil(DateTime dt) =>
                Runtime.env.MapAsync(e => e.Time.SleepUntil(dt));
        
            /// <summary>
            /// Pause a task until for a specified length of time
            /// </summary>
            [Pure, MethodImpl(IO.mops)]
            public static IO<Runtime, Unit> sleepFor(TimeSpan ts) => 
                Runtime.env.MapAsync(e => e.Time.SleepFor(ts));
        }
    }
}
