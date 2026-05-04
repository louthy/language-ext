using LanguageExt.Traits.Domain;

namespace DomainTypesExamples.ValueObjects.Spaces;

public sealed record HourValue : 
    DomainTypeFactory<HourValue, int>,
    VectorSpace<HourValue, int>
{
    private readonly int _value;

    private HourValue(int value) =>
        _value = value;

    public int To() => _value;

    public override string ToString() =>
        $"{_value}h";

    public static Fin<HourValue> From(int repr) =>
        new HourValue(repr);

    public static HourValue AdditiveIdentity { get; } = new(0);

    public static HourValue operator +(HourValue left, HourValue right) =>
        new(left._value + right._value);

    public static HourValue operator -(HourValue left, HourValue right) =>
        new(left._value - right._value);

    public static HourValue operator -(HourValue value) =>
        new(-value._value);

    public static HourValue operator *(HourValue left, int right) =>
        new(left._value * right);

    public static HourValue operator /(HourValue left, int right) =>
        new(left._value / right);
}
