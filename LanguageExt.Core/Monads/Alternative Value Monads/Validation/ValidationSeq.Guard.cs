using System;
using LanguageExt.Common;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class ValidationSeqGuardExtensions
    {
        public static Validation<FAIL, Unit> ToValidation<FAIL>(this Guard<FAIL> ma) =>
                ma.Flag
                    ? Success<FAIL, Unit>(unit)
                    : Fail<FAIL, Unit>(ma.OnFalse());
        
        public static Validation<FAIL, B> SelectMany<FAIL, B>(
            this Guard<FAIL> ma, 
            Func<Unit, Validation<FAIL, B>> f) =>
                ma.Flag
                    ? f(default)
                    : Fail<FAIL, B>(ma.OnFalse());

        public static Validation<FAIL, C> SelectMany<FAIL, B, C>(
            this Guard<FAIL> ma, 
            Func<Unit, Validation<FAIL, B>> bind, 
            Func<Unit, B, C> project) =>
                ma.Flag
                    ? bind(default).Map(b => project(default, b))
                    : Fail<FAIL, C>(ma.OnFalse());

        public static Validation<FAIL, Unit> SelectMany<FAIL, A>(
            this Validation<FAIL, A> ma, 
            Func<A, Guard<FAIL>> f) =>
                ma.Bind(a => f(a).ToValidation<FAIL>());

        public static Validation<FAIL, C> SelectMany<FAIL, A, C>(
            this Validation<FAIL, A> ma, 
            Func<A, Guard<FAIL>> bind, 
            Func<A, Unit, C> project) =>
                ma.Bind(a => bind(a).ToValidation<FAIL>().Map(_ => project(a, default)));
    }
}
