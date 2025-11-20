using LanguageExt;
using LanguageExt.Traits.Domain;

namespace DomainTypesExamples;

public readonly record struct TimeSpan(long Step) : 
    DomainType<TimeSpan, long>,
    Amount<TimeSpan, long>
{
    static Fin<TimeSpan> DomainType<TimeSpan, long>.From(long repr) => 
        new TimeSpan(repr);

    public static TimeSpan From(long repr) => 
        new (repr);

    public long To() =>
        Step;
    
    public static TimeSpan operator -(TimeSpan value) => 
        new (-value.Step);

    public static TimeSpan operator +(TimeSpan left, TimeSpan right) => 
        new (left.Step + right.Step);

    public static TimeSpan operator -(TimeSpan left, TimeSpan right) => 
        new (left.Step - right.Step);

    public static TimeSpan operator *(TimeSpan left, long right) => 
        new (left.Step * right);

    public static TimeSpan operator /(TimeSpan left, long right) => 
        new (left.Step / right);

    public int CompareTo(TimeSpan other) => 
        Step.CompareTo(other.Step);

    public static bool operator >(TimeSpan left, TimeSpan right) => 
        left.Step > right.Step;

    public static bool operator >=(TimeSpan left, TimeSpan right) => 
        left.Step >= right.Step;

    public static bool operator <(TimeSpan left, TimeSpan right) => 
        left.Step < right.Step;

    public static bool operator <=(TimeSpan left, TimeSpan right) => 
        left.Step <= right.Step;
}
