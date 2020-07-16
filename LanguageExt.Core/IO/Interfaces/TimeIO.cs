using System;
using System.Threading.Tasks;

namespace LanguageExt.Interfaces
{
    public interface TimeIO
    {
        /// <summary>
        /// Current date time
        /// </summary>
        DateTime Now { get; }
        
        /// <summary>
        /// Today's date 
        /// </summary>
        DateTime Today { get; }
        
        /// <summary>
        /// Pause a task until a specified time
        /// </summary>
        ValueTask<Unit> SleepUntil(DateTime dt);
        
        /// <summary>
        /// Pause a task until for a specified length of time
        /// </summary>
        ValueTask<Unit> SleepFor(TimeSpan ts);
    }
}
