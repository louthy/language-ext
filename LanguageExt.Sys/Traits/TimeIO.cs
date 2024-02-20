using System;
using System.Threading;
using LanguageExt.Attributes;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Sys.Traits;

public interface TimeIO
{
    /// <summary>
    /// Current local date time
    /// </summary>
    IO<DateTime> Now { get; }
        
    /// <summary>
    /// Current universal date time
    /// </summary>
    IO<DateTime> UtcNow { get; }
        
    /// <summary>
    /// Today's date 
    /// </summary>
    IO<DateTime> Today { get; }
        
    /// <summary>
    /// Pause a task until a specified time
    /// </summary>
    IO<Unit> SleepUntil(DateTime dt);
        
    /// <summary>
    /// Pause a task until for a specified length of time
    /// </summary>
    IO<Unit> SleepFor(TimeSpan ts);
}
    
/// <summary>
/// Type-class giving a struct the trait of supporting time IO
/// </summary>
/// <typeparam name="RT">Runtime</typeparam>
[Trait("*")]
public interface HasTime<RT> : HasIO<RT> 
    where RT : HasTime<RT>
{
    /// <summary>
    /// Access the time synchronous effect environment
    /// </summary>
    /// <returns>Time synchronous effect environment</returns>
    IO<TimeIO> TimeIO { get; }
}
