using System;
using LanguageExt.Common;
using LanguageExt.Traits.Domain;
using Xunit;

namespace LanguageExt.Tests.TraitTests.Domain;

public sealed class ComplexSpaceDomainTypeTests
{
    [Fact]
    public void Operations()
    {
        var left = ComplexSignal.From((2d, 3d)).SuccValue;

        var right = ComplexSignal.FromM((4d, -1d)).Run().As().Value.SuccValue;;

        var product = left * right;

        AssertClose(11d, product.To().Real);
        AssertClose(10d, product.To().Imaginary);
    }

    [Fact]
    public void ImaginarySquaredIsMinusOne()
    {
        var minusOne = ComplexSignal.I * ComplexSignal.I;

        AssertClose(-1d, minusOne.To().Real);
        AssertClose(0d, minusOne.To().Imaginary);
    }
    private static void AssertClose(double expected, double actual) =>
        Assert.True(Math.Abs(expected - actual) < 0.000_000_001d, $"Expected {expected}, actual {actual}");
}

file sealed class ComplexSignal :
    ComplexSpace<ComplexSignal>,
    DomainTypeFactory<ComplexSignal, (double Real, double Imaginary)>,
    DomainTypeFactoryM<ComplexSignal, Identity, (double Real, double Imaginary)>
{
    private readonly double real;
    private readonly double imaginary;

    private ComplexSignal(double real, double imaginary)
    {
        this.real = real;
        this.imaginary = imaginary;
    }

    public (double Real, double Imaginary) To() =>
        (real, imaginary);

    public static Fin<ComplexSignal> From((double Real, double Imaginary) repr) =>
        double.IsFinite(repr.Real) && double.IsFinite(repr.Imaginary)
            ? Fin.Succ(new ComplexSignal(repr.Real, repr.Imaginary))
            : Fin.Fail<ComplexSignal>(Error.New("Complex signal components must be finite."));

    public static FinT<Identity, ComplexSignal> FromM((double Real, double Imaginary) repr) =>
        From(repr).Match(
            Succ: static value => FinT.Succ<Identity, ComplexSignal>(value),
            Fail: static error => FinT.Fail<Identity, ComplexSignal>(error));

    public static ComplexSignal AdditiveIdentity { get; } =
        new(0d, 0d);

    public static ComplexSignal Origin =>
        AdditiveIdentity;

    public static ComplexSignal I { get; } =
        new(0d, 1d);

    public static ComplexSignal operator +(ComplexSignal left, ComplexSignal right) =>
        new(left.real + right.real, left.imaginary + right.imaginary);

    public static ComplexSignal operator -(ComplexSignal left, ComplexSignal right) =>
        new(left.real - right.real, left.imaginary - right.imaginary);

    public static ComplexSignal operator -(ComplexSignal value) =>
        new(-value.real, -value.imaginary);

    public static ComplexSignal operator *(ComplexSignal value, double scalar) =>
        new(value.real * scalar, value.imaginary * scalar);

    public static ComplexSignal operator /(ComplexSignal value, double scalar) =>
        new(value.real / scalar, value.imaginary / scalar);

    public static ComplexSignal operator *(ComplexSignal left, ComplexSignal right) =>
        new(
            real: left.real * right.real - left.imaginary * right.imaginary,
            imaginary: left.real * right.imaginary + left.imaginary * right.real);

    public bool Equals(ComplexSignal? other) =>
        other is not null &&
        real.Equals(other.real) &&
        imaginary.Equals(other.imaginary);

    public override bool Equals(object? obj) =>
        obj is ComplexSignal other && Equals(other);

    public override int GetHashCode() =>
        HashCode.Combine(real, imaginary);

    public static bool operator ==(ComplexSignal? left, ComplexSignal? right) =>
        left is null ? right is null : left.Equals(right);

    public static bool operator !=(ComplexSignal? left, ComplexSignal? right) =>
        !(left == right);
}
