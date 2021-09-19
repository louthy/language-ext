using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Test
{
    public readonly struct TimeIO : Sys.Traits.TimeIO
    {
        readonly TestTimeSpec spec;

        public TimeIO(TestTimeSpec spec) =>
            this.spec = spec;

        /// <summary>
        /// Current UTC date time
        /// </summary>
        public DateTime UtcNow =>
            spec.Type == TestTimeSpecType.Fixed
                ? spec.Start
                : spec.Specified.Add(DateTime.UtcNow - spec.Start);

        /// <summary>
        /// Current local date time
        /// </summary>
        public DateTime Now =>
            UtcNow.ToLocalTime();

        /// <summary>
        /// Today's date 
        /// </summary>
        public DateTime Today => 
            Now.Date;

        /// <summary>
        /// Pause a task until a specified time
        /// </summary>
        public ValueTask<Unit> SleepUntil(DateTime dt, CancellationToken token) =>
            Live.TimeIO.Default.SleepUntil(dt, token);

        /// <summary>
        /// Pause a task until for a specified length of time
        /// </summary>
        public ValueTask<Unit> SleepFor(TimeSpan ts, CancellationToken token) =>
            Live.TimeIO.Default.SleepFor(ts, token);
    }
}
