namespace LanguageExt.Pipes;

/// <summary>
/// Pipes prelude
/// </summary>
public static class Prelude
{
    public static readonly Repeat forever = new(Schedule.Forever);
    public static Repeat repeat(Schedule schedule) => new(schedule);
}
