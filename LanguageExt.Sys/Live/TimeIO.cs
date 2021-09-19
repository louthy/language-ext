using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Sys.Live
{
    public readonly struct TimeIO : Sys.Traits.TimeIO
    {
        public readonly static Sys.Traits.TimeIO Default =
            new TimeIO();
 
        /// <summary>
        /// Current date time
        /// </summary>
        public DateTime Now => DateTime.Now;
        
        /// <summary>
        /// Current date time
        /// </summary>
        public DateTime UtcNow => DateTime.UtcNow;

        /// <summary>
        /// Today's date 
        /// </summary>
        public DateTime Today => DateTime.Today;

        /// <summary>
        /// Pause a task until a specified time
        /// </summary>
        public async ValueTask<Unit> SleepUntil(DateTime dt, CancellationToken token)
        {
            if (dt <= Now) return default; 
            await Task.Delay(dt - Now, token).ConfigureAwait(false);
            return default;
        }

        /// <summary>
        /// Pause a task until for a specified length of time
        /// </summary>
        public async ValueTask<Unit> SleepFor(TimeSpan ts, CancellationToken token)        
        {
            await Task.Delay(ts).ConfigureAwait(false);
            return default;
        }
    }
}
