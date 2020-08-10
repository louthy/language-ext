using System;
using System.Threading.Tasks;
using LanguageExt.Attributes;

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
    
    /// <summary>
    /// Type-class giving a struct the trait of supporting time IO
    /// </summary>
    /// <typeparam name="RT">Runtime</typeparam>
    [Typeclass("*")]
    public interface HasTime<RT> : HasCancel<RT> 
        where RT : struct, HasCancel<RT>
    {
        /// <summary>
        /// Access the time IO environment
        /// </summary>
        /// <returns>Time IO environment</returns>
        IO<RT, TimeIO> TimeIO { get; }

        /// <summary>
        /// Access the time SIO environment
        /// </summary>
        /// <returns>Time SIO environment</returns>
        SIO<RT, TimeIO> TimeSIO { get; }
    }
}
