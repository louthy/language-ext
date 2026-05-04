namespace DomainTypesExamples.Capabilities;

public interface TimeIO
{
    IO<DateTimeOffset> Now { get; }

}

public static class TimeIOExtensions
{
    extension(TimeIO timeIO)
    {
        public IO<DateTimeOffset> UtcNow =>
            timeIO.Now.Map(now => now.ToUniversalTime());

        public IO<DateOnly> OnlyDate =>
            timeIO.Now.Map(dt => DateOnly.FromDateTime(dt.DateTime));

        public IO<TimeOnly> OnlyTime =>
            timeIO.Now.Map(dt => TimeOnly.FromDateTime(dt.DateTime));
    }
}

public sealed class DefaultTimeProvider(TimeProvider timeProvider) : TimeIO
{
    public IO<DateTimeOffset> Now => IO.lift(timeProvider.GetLocalNow);
}
