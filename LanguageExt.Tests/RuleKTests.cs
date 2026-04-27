using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using LanguageExt.Common;
using Xunit;

namespace LanguageExt.Tests;

public sealed class InMemoryExistingEmails : Const<IO<Seq<string>>>
{
    public const string First = "h.f.alvarez.rubio@gmail.com";
    public const string Second = "h.f.alvarez.r@gmail.com";

    public static IO<Seq<string>> Value =>
        IO.pure<Seq<string>>([First, Second]);
}


public sealed class ExistingEmail<Emails> 
    : RuleK<ExistingEmail<Emails>, IO, string>
    where Emails : Const<IO<Seq<string>>>
{
    public IO<Seq<string>> Max => Emails.Value;

    public static K<IO, bool> Check(string value) => 
        from existingEmails in Emails.Value
        select existingEmails.Contains(value);
}

public sealed class RuleKTest
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

        var resultM = ExistingEmail<InMemoryExistingEmails>
            .Validate(value, K<IO, Error> (r, v) => throw new UnreachableException())
            .Run();

        Assert.True(resultM.Run().IsSucc);
    }

    [Fact]
    public void Validate_ShouldReturnSuccess_PureOverload()
    {
        const string value = InMemoryExistingEmails.First;

        var resultM = ExistingEmail<InMemoryExistingEmails>
            .Validate(value, Error (r, v) => throw new UnreachableException())
            .Run();

        Assert.True(resultM.Run().IsSucc);
    }

    [Fact]
    public void Validate_ShouldReturnSuccess_ParameterlessOverload()
    {
        const string value = InMemoryExistingEmails.First;

        var resultM = ExistingEmail<InMemoryExistingEmails>
            .Validate(value, Error () => throw new UnreachableException())
            .Run();

        Assert.True(resultM.Run().IsSucc);
    }

    [Fact]
    public void Validate_ShouldReturnSuccess_DirectOverload()
    {
        const string value = InMemoryExistingEmails.First;

        var resultM = ExistingEmail<InMemoryExistingEmails>
            .Validate(value, Error.New("Que"))
            .Run();

        Assert.True(resultM.Run().IsSucc);
    }

    [Fact]
    public void Validate_ShouldReturnError_DetailAssertParamsOfFailDelegate_MonadOverload()
    {
        const string value = "12345678901234567";
        const string errorMsg = "Invalid value through Func<R, V, K<M, Error>>";

        var mResult = ExistingEmail<InMemoryExistingEmails>
            .Validate(value,
                     (rule, fValue) =>

                     {
                         Assert.Equal(value, fValue);
                         Assert.IsType<ExistingEmail<InMemoryExistingEmails>>(rule);
                         Assert.Equal(InMemoryExistingEmails.Value.Run(), rule.Max.Run());
                     
                         return IO.pure(Error.New(errorMsg));
                     })
                     .Run();

        var result = mResult.Run();

        Assert.True(result.IsFail);
        Assert.Equal(errorMsg, result.FailSpan().ToArray()[0].Message);
    }

    [Fact]
    public void Validate_ShouldReturnError_DetailAssertParamsOfFailDelegate_PureOverload()
    {
        const string value = "12345678901234567";
        const string errorMsg = "Invalid value through Func<R, V, Error>";

        var mResult = ExistingEmail<InMemoryExistingEmails>
            .Validate(value,
                     (rule, fValue) =>
                     {
                         Assert.Equal(value, fValue);
                         Assert.IsType<ExistingEmail<InMemoryExistingEmails>>(rule);
                         Assert.Equal(InMemoryExistingEmails.Value.Run(), rule.Max.Run());

                         return Error.New(errorMsg);
                     })
                     .Run();

        var result = mResult.Run();

        Assert.True(result.IsFail);
        Assert.Equal(errorMsg, result.FailValue.Message);
    }

    [Fact]
    public void Validate_ShouldReturnError_DetailAssertParamsOfFailDelegate_ParameterlessOverload()
    {
        const string value = "12345678901234567";
        const string errorMsg = "Invalid value through Func<Error>";

        var mResult = ExistingEmail<InMemoryExistingEmails>
            .Validate(value, () => Error.New(errorMsg))
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
            .Validate(value, Error.New(errorMsg))
            .Run();

        var result = mResult.Run();

        Assert.True(result.IsFail);
        Assert.Equal(errorMsg, result.FailValue.Message);
    }
}
