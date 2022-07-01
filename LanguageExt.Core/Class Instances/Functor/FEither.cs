#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public readonly struct FEither<L, A, B> : 
        Functor<Either<L, A>, Either<L, B>, A, B>,
        BiFunctor<Either<L, A>, Either<L, B>, L, A, L, B>
    {
        public static readonly FEither<L, A, B> Inst = default;

        [Pure]
        public Either<L, B> BiMap(Either<L, A> ma, Func<L, L> Left, Func<A, B> Right) => 
            ma switch
            {
                {IsRight : true} r => Either<L, B>.Right(Right(r.RightValue)),
                {IsLeft : true} l  => Either<L, B>.Left(Left(l.LeftValue)),
                _                  => Either<L, B>.Bottom
            };

        [Pure]
        public Either<L, B> Map(Either<L, A> ma, Func<A, B> f) =>
            ma switch
            {
                {IsRight : true} r => Either<L, B>.Right(f(r.RightValue)),
                {IsLeft : true} l  => Either<L, B>.Left(l.LeftValue),
                _                  => Either<L, B>.Bottom
            };
    }
}
