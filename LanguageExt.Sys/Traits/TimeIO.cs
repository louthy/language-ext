using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Attributes;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Sys.Traits
{
    public interface TimeIO
    {
        /// <summary>
        /// Current local date time
        /// </summary>
        DateTime Now { get; }
        
        /// <summary>
        /// Current universal date time
        /// </summary>
        DateTime UtcNow { get; }
        
        /// <summary>
        /// Today's date 
        /// </summary>
        DateTime Today { get; }
        
        /// <summary>
        /// Pause a task until a specified time
        /// </summary>
        ValueTask<Unit> SleepUntil(DateTime dt, CancellationToken token);
        
        /// <summary>
        /// Pause a task until for a specified length of time
        /// </summary>
        ValueTask<Unit> SleepFor(TimeSpan ts, CancellationToken token);
    }
    
    /// <summary>
    /// Type-class giving a struct the trait of supporting time IO
    /// </summary>
    /// <typeparam name="RT">Runtime</typeparam>
    [Typeclass("*")]
    public interface HasTime<RT> : HasCancel<RT> 
        where RT : struct, HasCancel<RT>
    {
        /// <summary>
        /// Access the time synchronous effect environment
        /// </summary>
        /// <returns>Time synchronous effect environment</returns>
        Eff<RT, TimeIO> TimeEff { get; }
    }
}
