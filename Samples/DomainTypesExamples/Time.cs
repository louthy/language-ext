using LanguageExt;
using LanguageExt.Traits.Domain;

namespace DomainTypesExamples;

public readonly record struct Time(long Timestamp) :
    DomainType<Time, long>,
    Locus<Time, TimeSpan, long>
{
    static Fin<Time> DomainType<Time, long>.From(long repr) => 
        new Time(repr);

    public static Time From(long repr) => 
        new (repr);

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
