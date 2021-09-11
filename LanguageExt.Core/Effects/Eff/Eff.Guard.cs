using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class EffGuards
    {
        public static Eff<B> SelectMany<B>(this Guard<Error> ma, Func<Unit, Eff<B>> f) =>
            ma.Flag
                ? f(default)
                : FailEff<B>(ma.OnFalse());

        public static Eff<RT, B> SelectMany<RT, B>(this Guard<Error> ma, Func<Unit, Eff<RT, B>> f) where RT : struct =>
            ma.Flag
                ? f(default)
                : FailEff<B>(ma.OnFalse());

        public static Eff<C> SelectMany<B, C>(this Guard<Error> ma, Func<Unit, Eff<B>> bind, Func<Unit, B, C> project) =>
            ma.Flag
                ? bind(default).Map(b => project(default, b))
                : FailEff<C>(ma.OnFalse());

        public static Eff<RT, C> SelectMany<RT, B, C>(this Guard<Error> ma, Func<Unit, Eff<RT, B>> bind, Func<Unit, B, C> project) where RT : struct  =>
            ma.Flag
                ? bind(default).Map(b => project(default, b))
                : FailEff<C>(ma.OnFalse());

        public static Eff<Unit> ToEff(this Guard<Error> ma) =>
            ma.Flag
                ? unitEff
                : FailEff<Unit>(ma.OnFalse());

        public static Eff<RT, Unit> ToEff<RT>(this Guard<Error> ma) where RT : struct  =>
            ma.Flag
                ? unitEff
                : FailEff<Unit>(ma.OnFalse());
        
        public static Eff<Unit> SelectMany<A>(this Eff<A> ma, Func<A, Guard<Error>> f) =>
            ma.Bind(a => f(a).ToEff());

        public static Eff<RT, Unit> SelectMany<RT, A>(this Eff<RT, A> ma, Func<A, Guard<Error>> f) where RT : struct  =>
            ma.Bind(a => f(a).ToEff());

        public static Eff<C> SelectMany<A, C>(this Eff<A> ma, Func<A, Guard<Error>> bind, Func<A, Unit, C> project) =>
            ma.Bind(a => bind(a).ToEff().Map(_ => project(a, default)));

        public static Eff<RT, C> SelectMany<RT, A, C>(this Eff<RT, A> ma, Func<A, Guard<Error>> bind, Func<A, Unit, C> project) where RT : struct  =>
            ma.Bind(a => bind(a).ToEff().Map(_ => project(a, default)));
    }
}
