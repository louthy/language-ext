using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using LanguageExt.Common;
using LanguageExt.Traits;
using LanguageExt.Traits.Domain;
using Newtonsoft.Json.Linq;
using Xunit;

namespace LanguageExt.Tests;

public sealed class MaxLength<MAX> : Rule<MaxLength<MAX>, string>
    where MAX : Const<int>
{
    public int Max => MAX.Value;

    public static bool Check(string value) =>
        value.Length <= MAX.Value;
}

public sealed class MinLength<MIN> : Rule<MinLength<MIN>, string>
    where MIN : Const<int>
{
    public int Min => MIN.Value;

    public static bool Check(string value) =>
        value.Length >= MIN.Value;
}

public sealed class N2 : Const<int>
{
    public static int Value => 2;
}

public sealed class N16 : Const<int>
{
    public static int Value => 16;
}

public sealed class RuleTest
{
    [Fact]
    public void Check_ShouldValidateLength()
    {
        const string validValue = "1234567890123456";
        const string invalidValue = "12345678901234567";

        var validResult = MaxLength<N16>.Check(validValue);
        var invalidResult = MaxLength<N16>.Check(invalidValue);

        Assert.True(validResult);
        Assert.False(invalidResult);
    }

    [Fact]
    public void Validate_ShouldReturnSuccess()
    {
        const string value = "1234567890123456";

        var mResult1 = MaxLength<N16>.Validate(
            value,
            (_, _) => throw new UnreachableException());

        var mResult2 = MaxLength<N16>.Validate(
            value, (_) => throw new UnreachableException());

        var mResult3 = MaxLength<N16>.Validate(
            value, () => throw new UnreachableException());


        var mResult4 = MaxLength<N16>.Validate(
            value, Error.New("Que"));


        Assert.Equal(value, mResult1.SuccValue);
        Assert.Equal(value, mResult2.SuccValue);
        Assert.Equal(value, mResult3.SuccValue);
        Assert.Equal(value, mResult4.SuccValue);
    }

    [Fact]
    public void Validate_ShouldReturnError_DetailAssertParamsOfFailDelegate_PureOverload()
    {
        const string value = "12345678901234567";
        const string errorMsg1 = "Invalid value through Func<R, V, Error>";
        const string errorMsg2 = "Invalid value through Func<V, Error>";
        const string errorMsg3 = "Invalid value through Func<Error>";
        const string errorMsg4 = "Invalid value through Error";

        var mResult1 = MaxLength<N16>.Validate(
            value,
            (rule, fValue) =>
            {
                Assert.Equal(value, fValue);
                Assert.IsType<MaxLength<N16>>(rule);
                Assert.Equal(N16.Value, rule.Max);

                return Error.New(errorMsg1);
            });

        var mResult2 = MaxLength<N16>.Validate(
            value, (v) =>
            {
                Assert.Equal(value, v);

                return Error.New(errorMsg2);
            });

        var mResult3 = MaxLength<N16>.Validate(
            value, () => Error.New(errorMsg3));

        var mResult4 = MaxLength<N16>.Validate(
            value, Error.New(errorMsg4));

        Assert.Equal(errorMsg1, mResult1.FailValue.Message);
        Assert.Equal(errorMsg2, mResult2.FailValue.Message);
        Assert.Equal(errorMsg3, mResult3.FailValue.Message);
        Assert.Equal(errorMsg4, mResult4.FailValue.Message);
    }
    
    [Fact]
    public void Not_ShouldNegateMaxLength()
    {
        const string validValue = "12345678901234567";
        const string invalidValue = "1";

        Func<string, string> errorMsg = 
            val => $"Too few chars, expected more than {N16.Value}, " +
                   $"entered: {val.Length} in word {val}";

        var expErrorMsg = errorMsg(invalidValue);

        var mInvalid = Rule.For<string>
            .Not<MaxLength<N16>>
            .Validate(invalidValue, 
                (r, value) =>
                {
                    Assert.Equal(N16.Value, r.NegatedRule.Max);
                    return Error.New(errorMsg(value));
                });


        var mValid = Rule.For<string>
            .Not<MaxLength<N16>>
            .Validate(validValue, () => throw new UnreachableException());

        Assert.Equal(validValue, mValid.SuccValue);
        Assert.Equal(expErrorMsg, mInvalid.FailValue.Message);
    }

    [Fact]
    public void All_ShouldValidateStringLength()
    {
        const string validValue = "1234567890123";
        const string invalidShortValue = "1";
        const string invalidLongValue = "12345678901234567";

        Func<string, string> errorMsg = 
            val => $"Chars  outside range, expected between " +
                   $"{N2.Value} and {N16.Value}, entered: " +
                   $"{val.Length} in word {val}";

        var expErrorShort = errorMsg(invalidShortValue);
        var expErrorLong = errorMsg(invalidLongValue);

        var mShortInvalid = Rule.For<string>
            .All<MinLength<N2>, MaxLength<N16>>
            .Validate(invalidShortValue,
                (rule, value) =>
                {
                    var (f, s) = rule;

                    Assert.Equal(N2.Value, f.Min);
                    Assert.Equal(N16.Value, s.Max);
                    return Error.New(errorMsg(value));
                });
        var mLongInvalid = Rule.For<string>
            .All<MinLength<N2>, MaxLength<N16>>
            .Validate(invalidLongValue,
                (rule, value) =>
                {
                    var (f, s) = rule;

                    Assert.Equal(N2.Value, f.Min);
                    Assert.Equal(N16.Value, s.Max);

                    return Error.New(errorMsg(value));
                });

        var mValid = Rule.For<string>
            .All<MinLength<N2>, MaxLength<N16>>
            .Validate(validValue, () => throw new UnreachableException());

        Assert.Equal(validValue, mValid.SuccValue);
        Assert.Equal(expErrorShort, mShortInvalid.FailValue.Message);
        Assert.Equal(expErrorLong, mLongInvalid.FailValue.Message);
    }

    [Fact]
    public void Any_ShouldValidateStringLength()
    {
        const string validLongValue = "12345678901234567";
        const string validShortValue = "1";
        const string invalidValue = "1234";

        Func<string, string> errorMsg = 
            val => $"Chars outside range, expected any outside of " +
                   $"{N2.Value} and {N16.Value}, entered: " +
                   $"{val.Length} in word {val}";

        var expError = errorMsg(invalidValue);
        
        var mInvalid = Rule.For<string>
            .Any<MinLength<N16>, MaxLength<N2>>
            .Validate(invalidValue,
                (rule, value) =>
                {
                    var (f, s) = rule;

                    Assert.Equal(N16.Value, f.Min);
                    Assert.Equal(N2.Value, s.Max);

                    return Error.New(errorMsg(value));
                });

        var mLongValid = Rule.For<string>
            .Any<MinLength<N16>, MaxLength<N2>>
            .Validate(validLongValue, () => throw new UnreachableException());

        var mShortValid = Rule.For<string>
            .Any<MinLength<N16>, MaxLength<N2>>
            .Validate(validShortValue, () => throw new UnreachableException());

        Assert.Equal(validLongValue, mLongValid.SuccValue);
        Assert.Equal(validShortValue, mShortValid.SuccValue);
        Assert.Equal(expError, mInvalid.FailValue.Message);

    }

}
