using System;
using System.Threading.Tasks;
using LanguageExt.Interfaces;

namespace LanguageExt.LiveIO
{
    public struct TimeIO : Interfaces.TimeIO
    {
        public static Interfaces.TimeIO Default =
            new TimeIO();
 
        /// <summary>
        /// Current date time
        /// </summary>
        public DateTime Now => DateTime.Now;
        
        /// <summary>
        /// Today's date 
        /// </summary>
        public DateTime Today => DateTime.Today;

        /// <summary>
        /// Pause a task until a specified time
        /// </summary>
        public async ValueTask<Unit> SleepUntil(DateTime dt)
        {
            if (dt <= Now) return default; 
            await Task.Delay(dt - Now);
            return default;
        }

        /// <summary>
        /// Pause a task until for a specified length of time
        /// </summary>
        public async ValueTask<Unit> SleepFor(TimeSpan ts)        
        {
            await Task.Delay(ts);
            return default;
        }
    }
}
