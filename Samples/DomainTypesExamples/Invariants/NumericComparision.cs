using System.Numerics;
using DomainTypesExamples.Literals;
using DomainTypesExamples.ValueObjects;
using LanguageExt.Traits;

namespace DomainTypesExamples.Invariants;

public sealed class GreaterThan<V, T> 
    : Rule<GreaterThan<V, T>, T>
    where V : Const<T>
    where T : INumber<T>
{
    public T Value => V.Value;

    public static bool Check(T value) => 
        value > V.Value;
}

public sealed class LowerThan<V, T> 
    : Rule<LowerThan<V, T>, T>
    where V : Const<T>
    where T : INumber<T>
{
    public T Value => V.Value;

    public static bool Check(T value) => 
        value < V.Value;
}

public sealed class EqualTo<V, T> : Rule<EqualTo<V, T>, T>
    where V : Const<T>
    where T : INumber<T>
{
    public T Value => V.Value;

    public static bool Check(T value) =>
        value == V.Value;
}

public sealed class GreatOrEqualTo<V, T>
    : Rule.For<T>.Any<GreaterThan<V, T>, EqualTo<V, T>>,
      Rule<GreatOrEqualTo<V, T>, T>
    where V : Const<T>
    where T : INumber<T>
{
    public T Value => V.Value;
}

public sealed class LowerOrEqualTo<V, T>
    : Rule.For<T>.Any<LowerThan<V, T>, EqualTo<V, T>>,
        Rule<LowerOrEqualTo<V, T>, T>
    where V : Const<T>
    where T : INumber<T>
{
    public T Value => V.Value;
}

public sealed class Between<MIN, MAX, T>
    : Rule.For<T>.All<GreatOrEqualTo<MIN, T>, 
            LowerOrEqualTo<MAX, T>>,
        Rule<Between<MIN, MAX, T>, T>
    where MIN : Const<T>
    where MAX : Const<T>
    where T : INumber<T>
{
    public T Min => MIN.Value;

    public T Max => MAX.Value;
}

public sealed class BetweenAbsolute<V, T> : Rule<BetweenAbsolute<V, T>, T>
    where V : Const<T>
    where T : INumber<T>
{
    public static bool Check(T value) =>
        value >= -V.Value && value <= V.Value;
} 
