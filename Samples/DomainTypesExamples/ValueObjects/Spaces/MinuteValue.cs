using LanguageExt.Traits.Domain;

namespace DomainTypesExamples.ValueObjects.Spaces;

public sealed record MinuteValue :
    DomainTypeFactory<MinuteValue, int>,
    VectorSpace<MinuteValue, int>
{
    private readonly int _value;

    private MinuteValue(int value) =>
        _value = value;

    public int To() => _value;

    public override string ToString() =>
        $"{_value}m";

    public static Fin<MinuteValue> From(int repr) =>
        new MinuteValue(repr);

    public static MinuteValue AdditiveIdentity { get; } = new(0);

    public static MinuteValue operator +(MinuteValue left, MinuteValue right) =>
        new(left._value + right._value);

    public static MinuteValue operator -(MinuteValue left, MinuteValue right) =>
        new(left._value - right._value);

    public static MinuteValue operator -(MinuteValue value) =>
        new(-value._value);

    public static MinuteValue operator *(MinuteValue left, int right) =>
        new(left._value * right);

    public static MinuteValue operator /(MinuteValue left, int right) =>
        new(left._value / right);
}
