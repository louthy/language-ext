using System;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Traits;
using LanguageExt.Traits.Domain;

namespace LanguageExt;

public readonly struct RuleKTry<R>
    where R : Rule<R>, new()
{
    public R Instance { get; }

    public bool Result { get; }

    internal RuleKTry(R instance, bool result) =>
        (Instance, Result) = (instance, result);
}

internal static class RuleKTry
{
    public static RuleKTry<R> With<R, F, A>(K<F, A> value)
        where R : RuleK<R, F, A>, new() =>
        new(R.Instance, R.Check(value));
}

public delegate Error RuleKFail<R1, R2, F, A>(RuleKTry<R1> r1, RuleKTry<R2> r2, K<F, A> value)
    where R1 : RuleK<R1, F, A>, new()
    where R2 : RuleK<R2, F, A>, new();

internal static class RuleKFailExt
{
    extension<R1, R2, F, A>(RuleKFail<R1, R2, F, A> fail)
        where R1 : RuleK<R1, F, A>, new()
        where R2 : RuleK<R2, F, A>, new()
    {
        public Error From(K<F, A> value) =>
            fail(RuleKTry.With<R1, F, A>(value), RuleKTry.With<R2, F, A>(value), value);
    }
}

public static partial class RuleKExtensions
{
    extension<R1, F, A>(R1)
        where R1 : RuleK<R1, F, A>, new()
    {
        public static Fin<K<F, A>> Validate(
            K<F, A> value, Func<R1, K<F, A>, Error> Fail) =>
            R1.Validate(value, Fail);

        public static Fin<K<F, A>> Validate(K<F, A> value, Func<Error> Fail) =>
            R1.Validate(value, (_, _) => Fail());

        public static Fin<K<F, A>> Validate(K<F, A> value, Error Fail) =>
            R1.Validate(value, (_, _) => Fail);
    }
}

public static class RuleKAllExtensions
{
    extension<R1, R2, F, A>(RuleK.All<R1, R2, F, A>)
        where R1 : RuleK<R1, F, A>, new()
        where R2 : RuleK<R2, F, A>, new()
    {
        public static Fin<K<F, A>> Validate(
            K<F, A> value,
            RuleKFail<R1, R2, F, A> Fail) =>
            RuleK.All<R1, R2, F, A>
                .Validate(value, (_, v) => Fail.From(v));
    }
}

public static class RuleKAnyExtensions
{

    extension<R1, R2, F, A>(RuleK.Any<R1, R2, F, A>)
        where R1 : RuleK<R1, F, A>, new()
        where R2 : RuleK<R2, F, A>, new()
    {
        public static Fin<K<F, A>> Validate(
            K<F, A> value,
            RuleKFail<R1, R2, F, A> Fail) =>
            RuleK.Any<R1, R2, F, A>
                .Validate(value, (_, v) => Fail.From(v));
    }
}
