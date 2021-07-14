using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Used by various error producing monads to have a contextual `where`
    /// </summary>
    /// <remarks>
    /// See `Prelude.guard(...)`
    /// </remarks>
    public struct Guard
    {
        public readonly bool Flag;
        public readonly Func<Error> OnFalse;

        public Guard(bool flag, Func<Error> onFalse) =>
            (Flag, OnFalse) = (flag, onFalse);

        public Guard(bool flag, Error onFalse) =>
            (Flag, OnFalse) = (flag, () => onFalse);

        public Aff<B> SelectMany<B>(Func<Unit, Aff<B>> f) =>
            Flag
                ? f(default)
                : FailAff<B>(OnFalse());

        public Aff<RT, B> SelectMany<RT, B>(Func<Unit, Aff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            Flag
                ? f(default)
                : FailAff<B>(OnFalse());

        public Aff<C> SelectMany<B, C>(Func<Unit, Aff<B>> bind, Func<Unit, B, C> project) =>
            Flag
                ? bind(default).Map(b => project(default, b))
                : FailAff<C>(OnFalse());

        public Aff<RT, C> SelectMany<RT, B, C>(Func<Unit, Aff<RT, B>> bind, Func<Unit, B, C> project) where RT : struct, HasCancel<RT> =>
            Flag
                ? bind(default).Map(b => project(default, b))
                : FailAff<C>(OnFalse());

        public Eff<B> SelectMany<B>(Func<Unit, Eff<B>> f) =>
            Flag
                ? f(default)
                : FailEff<B>(OnFalse());

        public Eff<RT, B> SelectMany<RT, B>(Func<Unit, Eff<RT, B>> f) =>
            Flag
                ? f(default)
                : FailEff<B>(OnFalse());

        public Eff<C> SelectMany<B, C>(Func<Unit, Eff<B>> bind, Func<Unit, B, C> project) =>
            Flag
                ? bind(default).Map(b => project(default, b))
                : FailEff<C>(OnFalse());

        public Eff<RT, C> SelectMany<RT, B, C>(Func<Unit, Eff<RT, B>> bind, Func<Unit, B, C> project) =>
            Flag
                ? bind(default).Map(b => project(default, b))
                : FailEff<C>(OnFalse());

        public Eff<Unit> ToEff() =>
            Flag
                ? unitEff
                : FailEff<Unit>(OnFalse());

        public Eff<RT, Unit> ToEff<RT>() =>
            Flag
                ? unitEff
                : FailEff<Unit>(OnFalse());

        public Aff<Unit> ToAff() =>
            Flag
                ? unitEff
                : FailAff<Unit>(OnFalse());

        public Aff<RT, Unit> ToAff<RT>() where RT : struct, HasCancel<RT> =>
            Flag
                ? unitEff
                : FailAff<Unit>(OnFalse());
    }

    public static class GuardExt
    {
        public static Aff<Unit> SelectMany<A>(this Aff<A> ma, Func<A, Guard> f) =>
            ma.Bind(a => f(a).ToAff());

        public static Aff<RT, Unit> SelectMany<RT, A>(this Aff<RT, A> ma, Func<A, Guard> f) where RT : struct, HasCancel<RT> =>
            ma.Bind(a => f(a).ToAff());

        public static Aff<C> SelectMany<A, C>(this Aff<A> ma, Func<A, Guard> bind, Func<A, Unit, C> project) =>
            ma.Bind(a => bind(a).ToAff().Map(_ => project(a, default)));

        public static Aff<RT, C> SelectMany<RT, A, C>(this Aff<RT, A> ma, Func<A, Guard> bind, Func<A, Unit, C> project) where RT : struct, HasCancel<RT> =>
            ma.Bind(a => bind(a).ToAff().Map(_ => project(a, default)));

        public static Eff<Unit> SelectMany<A>(this Eff<A> ma, Func<A, Guard> f) =>
            ma.Bind(a => f(a).ToEff());

        public static Eff<RT, Unit> SelectMany<RT, A>(this Eff<RT, A> ma, Func<A, Guard> f) =>
            ma.Bind(a => f(a).ToEff());

        public static Eff<C> SelectMany<A, C>(this Eff<A> ma, Func<A, Guard> bind, Func<A, Unit, C> project) =>
            ma.Bind(a => bind(a).ToEff().Map(_ => project(a, default)));

        public static Eff<RT, C> SelectMany<RT, A, C>(this Eff<RT, A> ma, Func<A, Guard> bind, Func<A, Unit, C> project) =>
            ma.Bind(a => bind(a).ToEff().Map(_ => project(a, default)));
    }
}
