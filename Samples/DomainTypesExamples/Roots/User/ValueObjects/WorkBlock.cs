using DomainTypesExamples.ValueObjects;
using DomainTypesExamples.ValueObjects.Scalars;
using DomainTypesExamples.ValueObjects.Spaces;

namespace DomainTypesExamples.Roots;

public abstract record WorkBlock
    : DomainType<WorkBlock, (string Kind, HourOnly Start, WorkDuration Duration)>,
        AffineSpace<WorkBlock, WorkDuration, HourScalar>
{
    public abstract WorkBlockKind Kind { get; }

    public required HourOnly Start { get; init; }

    public required WorkDuration Duration { get; init; }

    public HourOnly Finish => Start + Duration.ToBase();

    public (string Kind, HourOnly Start, WorkDuration Duration) To() =>
        (Kind.To(), Start, Duration);

    
    public static WorkBlock operator +(WorkBlock block, WorkDuration difference) =>
        block.TranslateTo(difference);

    public static WorkDuration operator -(WorkBlock left, WorkBlock right) =>
        WorkDuration.From(left.Start - right.Start).ThrowIfFail();

    protected abstract WorkBlock TranslateTo(WorkDuration difference);

    public sealed record Effective : WorkBlock
    {
        public override WorkBlockKind Kind =>
            WorkBlockKind.Effective;

        protected override WorkBlock TranslateTo(WorkDuration duration) =>
            this with
            {
                Start = Start,
                Duration = duration
            };



        public static Fin<Effective> From((HourOnly Start, WorkDuration Duration) repr) =>
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

        protected override WorkBlock TranslateTo(WorkDuration duration) =>
            this with
            {
                Start = Start,
                Duration = duration
            };

        public static Fin<Lunch> From(HourOnly start) =>
            WorkDuration.From(HourOnly.FromHours(1)) *
            (Lunch (duration) => new Lunch
            {
                Start = start,
                Duration = duration
            });
    }

    public sealed record Rest : WorkBlock
    {
        public override WorkBlockKind Kind =>
            WorkBlockKind.Rest;

        protected override WorkBlock TranslateTo(WorkDuration duration) =>
            this with
            {
                Start = Start,
                Duration = duration
            };

        public static Fin<Rest> From((HourOnly Start, WorkDuration Duration) repr) =>
            new Rest
            {
                Start = repr.Start,
                Duration = repr.Duration
            };
    }
}
