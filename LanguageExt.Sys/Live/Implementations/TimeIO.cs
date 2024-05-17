using System;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Live.Implementations;

public readonly struct TimeIO : Sys.Traits.TimeIO
{
    public static readonly Sys.Traits.TimeIO Default =
        new TimeIO();

    /// <summary>
    /// Current date time
    /// </summary>
    public IO<DateTime> Now =>
        lift(() => DateTime.Now);
        
    /// <summary>
    /// Current date time
    /// </summary>
    public IO<DateTime> UtcNow => 
        lift(() => DateTime.UtcNow);

    /// <summary>
    /// Today's date 
    /// </summary>
    public IO<DateTime> Today => 
        lift(() => DateTime.Today);

    /// <summary>
    /// Pause a task until a specified time
    /// </summary>
    public IO<Unit> SleepUntil(DateTime dt) =>
        from now in Now
        from res in dt <= now
                        ? unitIO
                        : liftIO(async env => await Task.Delay(dt - now, env.Token).ConfigureAwait(false))
        select res;

    /// <summary>
    /// Pause a task until for a specified length of time
    /// </summary>
    public IO<Unit> SleepFor(TimeSpan ts) =>
        liftIO(async env => await Task.Delay(ts, env.Token).ConfigureAwait(false));
}
