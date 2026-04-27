using System;
using System.Security.Cryptography;
using LanguageExt.Common;
using LanguageExt.Traits;
using LanguageExt.Traits.Domain;
using static LanguageExt.Prelude;

namespace LanguageExt;

public readonly struct RuleTry<R>
    where R : Rule<R>, new()
{
    public R Instance { get; }

    public bool Result { get; }

    internal RuleTry(R instance, bool result) => 
        (Instance, Result) = (instance, result);
}

internal static class RuleTry
{
    public static RuleTry<R> With<R, A>(A value)
        where R : Rule<R, A>, new() =>
        new(R.Instance, R.Check(value));
}

public delegate Error RuleFail<R1, R2, A>(RuleTry<R1> r1, RuleTry<R2> r2, A value)
    where R1 : Rule<R1, A>, new()
    where R2 : Rule<R2, A>, new();

internal static class RuleFailExt
{
    extension<R1, R2, A>(RuleFail<R1, R2, A> fail)
        where R1 : Rule<R1, A>, new()
        where R2 : Rule<R2, A>, new()
    {
        public Error From(A value) =>
            fail(RuleTry.With<R1, A>(value), RuleTry.With<R2, A>(value), value);
    }
}

public static partial class RuleExtensions
{
    extension<R1, A>(R1)
        where R1 : Rule<R1, A>, new()
    {
        public static Fin<A> Validate(A value, Func<R1, A, Error> Fail) =>
            R1.Validate(value, Fail);

        public static Fin<A> Validate(A value, Func<Error> Fail) =>
            R1.Validate(value, (_, _) => Fail());

        public static Fin<A> Validate(A value, Error Fail) =>
            R1.Validate(value, (_, _) => Fail);
    }
}

public static class RuleAllExtensions
{
    extension<R1, R2, A>(Rule.All<R1, R2, A>)
        where R1 : Rule<R1, A>, new()
        where R2 : Rule<R2, A>, new()
    {
        public static Fin<A> Validate(
            A value,
            RuleFail<R1, R2, A> Fail) =>
            Rule.All<R1, R2, A>
                .Validate(value, (_, v) => Fail.From(v));
    }
}

public static class RuleAnyExtensions
{

    extension<R1, R2, A>(Rule.Any<R1, R2, A>)
        where R1 : Rule<R1, A>, new()
        where R2 : Rule<R2, A>, new()
    {
        public static Fin<A> Validate(
            A value,
            RuleFail<R1, R2, A> Fail) =>
            Rule.Any<R1, R2, A>
                .Validate(value, (_, v) => Fail.From(v));
    }
}
