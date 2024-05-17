/*
using System;

namespace LanguageExt
{
    public static class ValidationSeqGuardExtensions
    {
        public static Validation<FAIL, Unit> SelectMany<FAIL, A>(
            this Validation<FAIL, A> ma, 
            Func<A, Guard<FAIL, Unit>> f) =>
                ma.Bind(a => f(a).ToValidation());

        public static Validation<FAIL, C> SelectMany<FAIL, A, C>(
            this Validation<FAIL, A> ma, 
            Func<A, Guard<FAIL, Unit>> bind, 
            Func<A, Unit, C> project) =>
                ma.Bind(a => bind(a).ToValidation().Map(_ => project(a, default)));
    }
}
*/
