using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using LanguageExt.Common;
using LanguageExt.Traits;
using Xunit;

namespace LanguageExt.Tests;

public sealed class MaxLength<MAX> : Rule<MaxLength<MAX>, string>
    where MAX : Const<int>
{
    public int Max => MAX.Value;

    public static bool Check(string value) =>
        value.Length <= MAX.Value;
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
}
