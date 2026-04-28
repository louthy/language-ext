using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using LanguageExt.Common;
using LanguageExt.Traits;
using LanguageExt.Traits.Domain;
using Newtonsoft.Json.Linq;
using Xunit;

namespace LanguageExt.Tests;

public sealed class MaxSize<MAX, F, A> : RuleK<MaxSize<MAX, F, A>, F, A>
    where MAX : Const<int>
    where F : Foldable<F>
{
    public int Max => MAX.Value;

    public static bool Check(K<F, A> value) =>
        value.Count <= MAX.Value;
}

public sealed class MinSize<MIN, F, A> : RuleK<MinSize<MIN, F, A>, F, A>
    where MIN : Const<int>
    where F : Foldable<F>
{
    public int Min => MIN.Value;

    public static bool Check(K<F, A> value) =>
        value.Count >= MIN.Value;
}

public sealed class RuleKTest
{
    [Fact]
    public void Check_ShouldValidateLength()
    {
        var validValue = "1234567890123456".AsIterable().ToSeq();
        var invalidValue = "12345678901234567".AsIterable().ToSeq();

        var validResult = MaxSize<N16, Seq, char>.Check(validValue);
        var invalidResult= MaxSize<N16, Seq, char>.Check(invalidValue);

        Assert.True(validResult);
        Assert.False(invalidResult);
    }

    [Fact]
    public void Validate_ShouldReturnSuccess()
    {
        var value = "1234567890123456".AsIterable().ToSeq();

        var mResult1 = MaxSize<N16, Seq, char>.Validate(
            value,
            (_, _) => throw new UnreachableException());

        var mResult2 = MaxSize<N16, Seq, char>.Validate(
            value, () => throw new UnreachableException());

        var mResult3 = MaxSize<N16, Seq, char>.Validate(
            value, Error.New("Que"));

        Assert.Equal(value, mResult1.SuccValue);
        Assert.Equal(value, mResult2.SuccValue);
        Assert.Equal(value, mResult3.SuccValue);
    }

    [Fact]
    public void Validate_ShouldReturnError()
    {
        var value = "12345678901234567".AsIterable().ToSeq();

        const string errorMsgRv = "Invalid value through Func<R, V, Error>";
        const string errorMsgEp = "Invalid value through Func<Error>";
        const string errorMsgDv = "Invalid value through Error";

        var mResult1 = MaxSize<N16, Seq, char>.Validate(
            value,
            (rule, fValue) =>
            {
                Assert.Equal(value, fValue);
                Assert.IsType<MaxSize<N16, Seq, char>>(rule);
                Assert.Equal(N16.Value, rule.Max);

                return Error.New(errorMsgRv);
            });

        var mResult2 = MaxSize<N16, Seq, char>.Validate(
            value, () => Error.New(errorMsgEp));

        var mResult3 = MaxSize<N16, Seq, char>.Validate(value, Error.New(errorMsgDv));

        Assert.Equal(errorMsgRv, mResult1.FailValue.Message);
        Assert.Equal(errorMsgEp, mResult2.FailValue.Message);
        Assert.Equal(errorMsgDv, mResult3.FailValue.Message);
    }

    [Fact]
    public void Not_ShouldNegateMaxLength()
    {
        var validValue = "12345678901234567".AsIterable().ToSeq();
        var invalidValue = "1".AsIterable().ToSeq();

        Func<Seq<char>, string> errorMsg =
            val => $"Too few chars, expected more than {N16.Value}, " +
                   $"entered: {val.Length} in word {val}";

        var expErrorMsg = errorMsg(invalidValue);

        var mInvalid = ruleFor<Seq, char>
            .Not<MaxSize<N16, Seq, char>>
            .Validate(invalidValue.Kind(),
                (r, value) =>
                {
                    Assert.Equal(N16.Value, r.NegatedRule.Max);
                    return Error.New(errorMsg(value.As()));
                });


        var mValid = ruleFor<Seq, char>
            .Not<MaxSize<N16, Seq, char>>
            .Validate(validValue, () => throw new UnreachableException());

        Assert.Equal(validValue, mValid.SuccValue);
        Assert.Equal(expErrorMsg, mInvalid.FailValue.Message);
    }

    [Fact]
    public void All_ShouldValidateStringLength()
    {
        var validValue = "1234567890123".AsIterable().ToSeq();
        var invalidShortValue = "1".AsIterable().ToSeq();
        var invalidLongValue = "12345678901234567".AsIterable().ToSeq();

        Func<Seq<char>, string> errorMsg =
            val => $"Chars  outside range, expected between " +
                   $"{N2.Value} and {N16.Value}, entered: " +
                   $"{val.Length} in word {val}";

        var expErrorShort = errorMsg(invalidShortValue);
        var expErrorLong = errorMsg(invalidLongValue);

        var mShortInvalid = ruleFor<Seq, char>
            .All<MinSize<N2, Seq, char>, MaxSize<N16, Seq, char>>
            .Validate(invalidShortValue,
                (minRule, maxRule, value) =>
                {
                    Assert.Equal(N2.Value, minRule.Instance.Min);
                    Assert.Equal(N16.Value, maxRule.Instance.Max);

                    return Error.New(errorMsg(value.As()));
                });
        var mLongInvalid = ruleFor<Seq, char>
            .All<MinSize<N2, Seq, char>, MaxSize<N16, Seq, char>>
            .Validate(invalidLongValue,
                (minRule, maxRule, value) =>
                {
                    Assert.Equal(N2.Value, minRule.Instance.Min);
                    Assert.Equal(N16.Value, maxRule.Instance.Max);
                    
                    return Error.New(errorMsg(value.As()));
                });

        var mValid = ruleFor<Seq, char>
            .All<MinSize<N2, Seq, char>, MaxSize<N16, Seq, char>>
            .Validate(validValue, () => throw new UnreachableException());

        Assert.Equal(validValue, mValid.SuccValue);
        Assert.Equal(expErrorShort, mShortInvalid.FailValue.Message);
        Assert.Equal(expErrorLong, mLongInvalid.FailValue.Message);
    }

    [Fact]
    public void Any_ShouldValidateStringLength()
    {
        var validLongValue = "12345678901234567".AsIterable().ToSeq();
        var validShortValue = "1".AsIterable().ToSeq();
        var invalidValue = "1234".AsIterable().ToSeq();

        Func<Seq<char>, string> errorMsg =
            val => $"Chars outside range, expected any outside of " +
                   $"{N2.Value} and {N16.Value}, entered: " +
                   $"{val.Length} in word {val}";

        var expError = errorMsg(invalidValue);

        var mInvalid = ruleFor<Seq, char>
            .Any<MinSize<N16, Seq, char>, MaxSize<N2, Seq, char>>
            .Validate(invalidValue,
                (minRule, maxRule, value) =>
                {
                    Assert.Equal(N16.Value, minRule.Instance.Min);
                    Assert.Equal(N2.Value, maxRule.Instance.Max);

                    return Error.New(errorMsg(value.As()));
                });

        var mLongValid = ruleFor<Seq, char>
            .Any<MinSize<N16, Seq, char>, MaxSize<N2, Seq, char>>
            .Validate(validLongValue, () => throw new UnreachableException());

        var mShortValid = ruleFor<Seq, char>
            .Any<MinSize<N16, Seq, char>, MaxSize<N2, Seq, char>>
            .Validate(validShortValue, () => throw new UnreachableException());

        Assert.Equal(validLongValue, mLongValid.SuccValue);
        Assert.Equal(validShortValue, mShortValid.SuccValue);
        Assert.Equal(expError, mInvalid.FailValue.Message);

    }

}
