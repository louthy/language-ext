#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public readonly struct FEitherUnsafe<L, A, B> : 
        Functor<EitherUnsafe<L, A>, EitherUnsafe<L, B>, A, B>,
        BiFunctor<EitherUnsafe<L, A>, EitherUnsafe<L, B>, L, A, L, B>
    {
        public static readonly FEitherUnsafe<L, A, B> Inst = default;

        [Pure]
        public EitherUnsafe<L, B> BiMap(EitherUnsafe<L, A> ma, Func<L, L> Left, Func<A, B> Right) => 
            ma switch
            {
                {IsRight : true} r => EitherUnsafe<L, B>.Right(Right(r.RightValue)),
                {IsLeft : true} l  => EitherUnsafe<L, B>.Left(Left(l.LeftValue)),
                _                  => EitherUnsafe<L, B>.Bottom
            };

        [Pure]
        public EitherUnsafe<L, B> Map(EitherUnsafe<L, A> ma, Func<A, B> f) =>
            ma switch
            {
                {IsRight : true} r => EitherUnsafe<L, B>.Right(f(r.RightValue)),
                {IsLeft : true} l  => EitherUnsafe<L, B>.Left(l.LeftValue),
                _                  => EitherUnsafe<L, B>.Bottom
            };
    }
}
