using System;

namespace LanguageExt.Sys.Test.Implementations;

public record TestTimeSpec(Schedule Schedule, DateTime Start)
{
    /// <summary>
    /// Time never passes, it has a constant value of `start`
    /// </summary>
    public static TestTimeSpec FixedFromSpecified(DateTime start) =>
        new (Schedule.Forever, start);

    /// <summary>
    /// Time never passes, it has a constant value of `DateTime.Now` (as it was set when you call this method)
    /// </summary>
    public static TestTimeSpec FixedFromNow(DateTime start) =>
        FixedFromSpecified(DateTime.UtcNow);
    
    /// <summary>
    /// Time passes at 1 millisecond per tick starting from `start`
    /// </summary>
    public static TestTimeSpec RunningFromSpecified(DateTime start) =>
        new (Schedule.spaced(1), start);

    /// <summary>
    /// Time passes at 1 millisecond per tick starting from `DateTime.Now`
    /// </summary>
    public static TestTimeSpec RunningFromNow() =>
        RunningFromSpecified(DateTime.UtcNow);
}
