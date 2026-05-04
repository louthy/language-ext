using DomainTypesExamples.Invariants;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Traits.Domain;

namespace DomainTypesExamples;

public readonly record struct Time(long Timestamp) :
    DomainType<Time, long>,
    Locus<Time, TimeSpan, long>
{
    public static Fin<Time> From(long repr) =>
        NonNegative<long>
            .Validate(repr, Fail: (r, v) => 
                Error.New($"Time cannot be negative (Sent: {v}, minimal: {r.Zero})"))
            .Map(v => new Time(v));

    public long To() =>
        Timestamp;

    public int CompareTo(Time other) => 
        Timestamp.CompareTo(other.Timestamp);

    public static bool operator >(Time left, Time right) => 
        left.Timestamp > right.Timestamp;

    public static bool operator >=(Time left, Time right) =>
        left.Timestamp >= right.Timestamp;

    public static bool operator <(Time left, Time right) => 
        left.Timestamp < right.Timestamp;

    public static bool operator <=(Time left, Time right) =>
        left.Timestamp <= right.Timestamp;        

    public static Time operator -(Time value) => 
        new (-value.Timestamp);

    public static Time Origin { get; } = 
        new(0L);

    public static Time AdditiveIdentity { get; } = 
        new(0L);
    
    public static Time operator +(Time left, TimeSpan right) => 
        new (left.Timestamp + right.Step);

    public static TimeSpan operator -(Time left, Time right) => 
        new(left.Timestamp - right.Timestamp);
}
