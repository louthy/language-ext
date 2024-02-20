using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;

namespace LanguageExt.Sys.Test.Implementations;

public class TimeIO : Sys.Traits.TimeIO, IDisposable
{
    readonly Atom<DateTime> now;
    readonly IEnumerator<Duration> ticks;

    public TimeIO(TestTimeSpec spec)
    {
        now = Atom(spec.Start);
        ticks = spec.Schedule.Run().GetEnumerator();
    }

    public void Dispose() =>
        ticks.Dispose();

    void Tick() =>
        now.Swap(n =>
                 {
                     if (!ticks.MoveNext()) throw new TimeoutException("We've reached the heat death of the universe");
                     return n.AddMilliseconds(ticks.Current);
                 });

    /// <summary>
    /// Current UTC date time
    /// </summary>
    public IO<DateTime> UtcNow =>
        lift(() =>
             {
                 Tick();
                 return now.Value;
             });

    /// <summary>
    /// Current local date time
    /// </summary>
    public IO<DateTime> Now =>
        UtcNow.Map(t => t.ToLocalTime());

    /// <summary>
    /// Today's date 
    /// </summary>
    public IO<DateTime> Today => 
        Now.Map(t => t.Date);

    /// <summary>
    /// Pause a task until a specified time
    /// </summary>
    public IO<Unit> SleepUntil(DateTime dt) =>
        Live.TimeIO.Default.SleepUntil(dt);

    /// <summary>
    /// Pause a task until for a specified length of time
    /// </summary>
    public IO<Unit> SleepFor(TimeSpan ts) =>
        Live.TimeIO.Default.SleepFor(ts);
}
