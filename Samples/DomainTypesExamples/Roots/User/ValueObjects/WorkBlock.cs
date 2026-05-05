namespace DomainTypesExamples.Roots;

public abstract record WorkBlock
    : DomainType<WorkBlock, (string Kind, WorkMoment Start, WorkDuration Duration)>
{
    public abstract WorkBlockKind Kind { get; }

    public required WorkMoment Start { get; init; }

    public required WorkDuration Duration { get; init; }

    public WorkMoment Finish =>
        Start + Duration.ToBase();

    public (string Kind, WorkMoment Start, WorkDuration Duration) To() =>
        (Kind.To(), Start, Duration);

    public WorkBlock MoveBy(Time offset) =>
        this with
        {
            Start = Start + offset
        };

    public sealed record Effective : WorkBlock
    {
        public override WorkBlockKind Kind =>
            WorkBlockKind.Effective;

        public static Fin<Effective> From((WorkMoment Start, WorkDuration Duration) repr) =>
            new Effective
            {
                Start = repr.Start,
                Duration = repr.Duration
            };
    }

    public sealed record Lunch : WorkBlock
    {
        public override WorkBlockKind Kind =>
            WorkBlockKind.Lunch;

        public static Fin<Lunch> From(WorkMoment start) =>
            WorkDuration.From(1 * hour)
                .Map(duration => new Lunch
                {
                    Start = start,
                    Duration = duration
                });
    }

    public sealed record Rest : WorkBlock
    {
        public override WorkBlockKind Kind =>
            WorkBlockKind.Rest;

        public static Fin<Rest> From((WorkMoment Start, WorkDuration Duration) repr) =>
            new Rest
            {
                Start = repr.Start,
                Duration = repr.Duration
            };
    }
}
