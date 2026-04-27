using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using LanguageExt.Common;
using LanguageExt.Traits;
using LanguageExt.Traits.Domain;
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
    public void Check_ShouldReturnTrue()
    {
        const string value = "1234567890123456";

        var valueChecked = MaxLength<N16>.Check(value);

        Assert.True(valueChecked);
    }

    [Fact]
    public void Check_ShouldReturnFalse()
    {
        const string value = "12345678901234567";

        var valueChecked = MaxLength<N16>.Check(value);
        
        Assert.False(valueChecked);
    }

    [Fact]
    public void Validate_ShouldReturnSuccess_PureOverLoad()
    {
        const string value = "1234567890123456";

        var mResult = MaxLength<N16>.Validate(
            value,
            (_, _) => throw new UnreachableException());

        Assert.True(mResult.IsSucc);
    }

    [Fact]
    public void Validate_ShouldReturnSuccess_ParameterlessOverLoad()
    {
        const string value = "1234567890123456";

        var mResult = MaxLength<N16>.Validate(
            value, () => throw new UnreachableException());

        Assert.True(mResult.IsSucc);
    }

    [Fact]
    public void Validate_ShouldReturnSuccess_DirectOverLoad()
    {
        const string value = "1234567890123456";

        var mResult = MaxLength<N16>.Validate(
            value, Error.New("Que"));

        Assert.True(mResult.IsSucc);
    }

    [Fact]
    public void Validate_ShouldReturnError_DetailAssertParamsOfFailDelegate_PureOverload()
    {
        const string value = "12345678901234567";
        const string errorMsg = "Invalid value through Func<R, V, Error>";

        var mResult = MaxLength<N16>.Validate(
            value,
            (rule, fValue) =>
            {
                Assert.Equal(value, fValue);
                Assert.IsType<MaxLength<N16>>(rule);
                Assert.Equal(N16.Value, rule.Max);

                return Error.New(errorMsg);
            });

        Assert.True(mResult.IsFail);
        Assert.Equal(errorMsg, mResult.FailValue.Message);
    }

    [Fact]
    public void Validate_ShouldReturnError_DetailAssertParamsOfFailDelegate_ParameterlessOverload()
    {
        const string value = "12345678901234567";
        const string errorMsg = "Invalid value through Func<Error>";

        var mResult = MaxLength<N16>.Validate(
            value, () => Error.New(errorMsg));

        Assert.True(mResult.IsFail);
        Assert.Equal(errorMsg, mResult.FailValue.Message);
    }

    [Fact]
    public void Validate_ShouldReturnError_DetailAssertParamsOfFailDelegate_DirectValue()
    {
        const string value = "12345678901234567";
        const string errorMsg = "Invalid value through Error";

        var mResult = MaxLength<N16>.Validate(value, Error.New(errorMsg));

        Assert.True(mResult.IsFail);
        Assert.Equal(errorMsg, mResult.FailValue.Message);
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

        var mInvalid = ruleFor<string>
            .Not<MaxLength<N16>>
            .Validate(invalidValue, 
                (r, value) =>
                {
                    Assert.Equal(N16.Value, r.NegatedRule.Max);
                    return Error.New(errorMsg(value));
                });


        var mValid = ruleFor<string>
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

        var mShortInvalid = ruleFor<string>
            .All<MinLength<N2>, MaxLength<N16>>
            .Validate(invalidShortValue,
                (minRule, maxRule, value) =>
                {
                    Assert.Equal(N2.Value, minRule.Instance.Min);
                    Assert.Equal(N16.Value, maxRule.Instance.Max);
                    return Error.New(errorMsg(value));
                });
        var mLongInvalid = ruleFor<string>
            .All<MinLength<N2>, MaxLength<N16>>
            .Validate(invalidLongValue,
                (minRule, maxRule, value) =>
                {
                    Assert.Equal(N2.Value, minRule.Instance.Min);
                    Assert.Equal(N16.Value, maxRule.Instance.Max);
                    return Error.New(errorMsg(value));
                });

        var mValid = ruleFor<string>
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
        
        var mInvalid = ruleFor<string>
            .Any<MinLength<N16>, MaxLength<N2>>
            .Validate(invalidValue,
                (minRule, maxRule, value) =>
                {
                    Assert.Equal(N16.Value, minRule.Instance.Min);
                    Assert.Equal(N2.Value, maxRule.Instance.Max);

                    return Error.New(errorMsg(value));
                });

        var mLongValid = ruleFor<string>
            .Any<MinLength<N16>, MaxLength<N2>>
            .Validate(validLongValue, () => throw new UnreachableException());

        var mShortValid = ruleFor<string>
            .Any<MinLength<N16>, MaxLength<N2>>
            .Validate(validShortValue, () => throw new UnreachableException());

        Assert.Equal(validLongValue, mLongValid.SuccValue);
        Assert.Equal(validShortValue, mShortValid.SuccValue);
        Assert.Equal(expError, mInvalid.FailValue.Message);

    }

}
