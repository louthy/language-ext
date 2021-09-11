using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class FinGuardExtensions
    {
        public static Fin<Unit> ToFin(this Guard<Error> ma) =>
            ma.Flag
                ? FinSucc(unit)
                : FinFail<Unit>(ma.OnFalse());
        
        public static Fin<B> SelectMany<B>(this Guard<Error> ma, Func<Unit, Fin<B>> f) =>
            ma.Flag
                ? f(default)
                : FinFail<B>(ma.OnFalse());

        public static Fin<C> SelectMany<B, C>(this Guard<Error> ma, Func<Unit, Fin<B>> bind, Func<Unit, B, C> project) =>
            ma.Flag
                ? bind(default).Map(b => project(default, b))
                : FinFail<C>(ma.OnFalse());

        public static Fin<Unit> SelectMany<A>(this Fin<A> ma, Func<A, Guard<Error>> f) =>
            ma.Bind(a => f(a).ToFin());

        public static Fin<C> SelectMany<A, C>(this Fin<A> ma, Func<A, Guard<Error>> bind, Func<A, Unit, C> project) =>
            ma.Bind(a => bind(a).ToFin().Map(_ => project(a, default)));
    }
}
