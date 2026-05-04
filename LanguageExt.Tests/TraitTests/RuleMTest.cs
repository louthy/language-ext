using System;
using System.Diagnostics;
using LanguageExt.Common;
using LanguageExt.Traits.Domain;
using Xunit;

namespace LanguageExt.Tests;

public sealed class InMemoryExistingEmails : Const<IO<Seq<string>>>
{
    public const string First = "h.f.alvarez.rubio@gmail.com";
    public const string Second = "h.f.alvarez.r@gmail.com";

    public static IO<Seq<string>> Value =>
        IO.pure<Seq<string>>([First, Second]);
}

public sealed class OtherInMemoryExistingEmails : Const<IO<Seq<string>>>
{
    public const string First = "other.email1@gmail.com";
    public const string Second = "h.f.alvarez.r@gmail.com";

    public static IO<Seq<string>> Value =>
        IO.pure<Seq<string>>([First, Second]);
}



public sealed class ExistingEmail<Emails>
    : RuleM<ExistingEmail<Emails>, IO, string>
    where Emails : Const<IO<Seq<string>>>
{
    public IO<Seq<string>> Max => Emails.Value;

    public static K<IO, bool> Check(string value) =>
        from existingEmails in Emails.Value
        select existingEmails.Contains(value);
}

public sealed class RuleMTest
{
    [Fact]
    public void Check_ShouldReturnTrue()
    {
        const string value = InMemoryExistingEmails.First;

        var valueChecked = ExistingEmail<InMemoryExistingEmails>.Check(value);

        Assert.True(valueChecked.Run());
    }

    [Fact]
    public void Check_ShouldReturnFalse()
    {
        const string value = "noestoy@gmail.com";

        var valueChecked = ExistingEmail<InMemoryExistingEmails>.Check(value);

        Assert.False(valueChecked.Run());
    }

    [Fact]
    public void Validate_ShouldReturnSuccess_MonadOverload()
    {
        const string value = InMemoryExistingEmails.First;

        var resultM = ExistingEmail<InMemoryExistingEmails>.ValidateM(value, K<IO, Error> (r, v) => throw new UnreachableException())
            .Run();

        Assert.True(resultM.Run().IsSucc);
    }
    [Fact]
    public void Validate_ShouldReturnSuccess_ParameterlessOverload()
    {
        const string value = InMemoryExistingEmails.First;

        var resultM = ExistingEmail<InMemoryExistingEmails>
            .ValidateM(value, K<IO, Error> () => throw new UnreachableException())
            .Run();

        Assert.True(resultM.Run().IsSucc);
    }

    [Fact]
    public void Validate_ShouldReturnSuccess_DirectOverload()
    {
        const string value = InMemoryExistingEmails.First;

        var resultM = ExistingEmail<InMemoryExistingEmails>
            .ValidateM(value, IO.pure(Error.New("Que")))
            .Run();

        Assert.True(resultM.Run().IsSucc);
    }

    [Fact]
    public void Validate_ShouldReturnError_DetailAssertParamsOfFailDelegate_MonadOverload()
    {
        const string value = "12345678901234567";
        const string errorMsg = "Invalid value through Func<R, V, K<M, Error>>";

        var mResult = ExistingEmail<InMemoryExistingEmails>
            .ValidateM(value,
                (rule, fValue) =>

                {
                    Assert.Equal(value, fValue);
                    Assert.IsType<ExistingEmail<InMemoryExistingEmails>>(rule);
                    Assert.Equal(InMemoryExistingEmails.Value.Run(), rule.Max.Run());
                     
                    return IO.pure(Error.New(errorMsg));
                })
            .Run();

        var result = mResult.Run();

        Assert.Equal(errorMsg, result.FailValue.Message);
    }

    [Fact]
    public void Validate_ShouldReturnError_DetailAssertParamsOfFailDelegate_ParameterlessOverload()
    {
        const string value = "12345678901234567";
        const string errorMsg = "Invalid value through Func<Error>";

        var mResult = ExistingEmail<InMemoryExistingEmails>
            .ValidateM(value, () => IO.pure(Error.New(errorMsg)))
            .Run();

        var result = mResult.Run();

        Assert.True(result.IsFail);
        Assert.Equal(errorMsg, result.FailValue.Message);
    }

    [Fact]
    public void Validate_ShouldReturnError_DetailAssertParamsOfFailDelegate_DirectValue()
    {
        const string value = "12345678901234567";
        const string errorMsg = "Invalid value through Error";

        var mResult = ExistingEmail<InMemoryExistingEmails>
            .ValidateM(value, IO.pure(Error.New(errorMsg)))
            .Run();

        var result = mResult.Run();

        Assert.True(result.IsFail);
        Assert.Equal(errorMsg, result.FailValue.Message);
    }

    [Fact]
    public void Not_ShouldNegate()
    {
        const string notExistEmail = "1";
        const string existEmail = InMemoryExistingEmails.First;

        Func<string, string> errorMsg = val => $"The email {val} already exists";

        var expErrorMsg = errorMsg(existEmail);

        var mSuccess = RuleM<IO>.For<string>
            .Not<ExistingEmail<InMemoryExistingEmails>>
            .ValidateM(notExistEmail, K<IO, Error> (r, v) => throw new UnreachableException())
            .Run();

        var mFailure = RuleM<IO>.For<string>
            .Not<ExistingEmail<InMemoryExistingEmails>>
            .ValidateM(
                existEmail, 
                K<IO, Error> (r, v) => IO.pure(Error.New(errorMsg(v))))
            .Run();

        Assert.Equal(notExistEmail, mSuccess.Run().SuccValue);
        Assert.Equal(expErrorMsg, mFailure.Run().FailValue.Message);
    }

    [Fact]
    public void All_ShouldVerifyDuplicatedInMemory()
    {
        const string validValue = InMemoryExistingEmails.Second;
        const string invalidValue = InMemoryExistingEmails.First;

        Func<string, string> errorMsg =
            val => $"The email {val} is NOT duplicated";

        var expErrorMsg = errorMsg(invalidValue);

        var mValid = RuleM<IO>.For<string>
            .All<ExistingEmail<InMemoryExistingEmails>, ExistingEmail<OtherInMemoryExistingEmails>>
            .ValidateM(validValue, K<IO, Error> (r, v) => throw new UnreachableException())
            .Run();

        var mInvalid = RuleM<IO>.For<string>
            .All<ExistingEmail<InMemoryExistingEmails>, ExistingEmail<OtherInMemoryExistingEmails>>
            .ValidateM(invalidValue, 
                K<IO, Error> (r, v) => IO.pure(Error.New(errorMsg(v))))
            .Run();

        Assert.Equal(validValue, mValid.Run().SuccValue);
        Assert.Equal(expErrorMsg, mInvalid.Run().FailValue.Message);
    }

    [Fact]
    public void Any_ShouldVerifyIfExistsInAnyMemory()
    {
        const string validValue = InMemoryExistingEmails.First;
        const string invalidValue = "a1";

        Func<string, string> errorMsg =
            val => $"The email {val} does not exists in one of the lists";

        var expErrorMsg = errorMsg(invalidValue);

        var mValid = RuleM<IO>.For<string>
            .Any<ExistingEmail<InMemoryExistingEmails>, ExistingEmail<OtherInMemoryExistingEmails>>
            .ValidateM(validValue, K<IO, Error> (r, v) => throw new UnreachableException())
            .Run();

        var mInvalid = RuleM<IO>.For<string>
            .Any<ExistingEmail<InMemoryExistingEmails>, ExistingEmail<OtherInMemoryExistingEmails>>
            .ValidateM(invalidValue, 
                K<IO, Error> (r, v) => IO.pure(Error.New(errorMsg(v))))
            .Run();

        Assert.Equal(validValue, mValid.Run().SuccValue);
        Assert.Equal(expErrorMsg, mInvalid.Run().FailValue.Message);

    }
}
