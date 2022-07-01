#nullable enable

using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct FValidation<MonoidFail, FAIL, A, B> : 
        Functor<Validation<MonoidFail, FAIL, A>, Validation<MonoidFail, FAIL, B>, A, B>,
        BiFunctor<Validation<MonoidFail, FAIL, A>, Validation<MonoidFail, FAIL, B>, FAIL, A, FAIL, B>
        where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL>
    {
        public static readonly FValidation<MonoidFail, FAIL, A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<MonoidFail, FAIL, B> BiMap(Validation<MonoidFail, FAIL, A> ma, Func<FAIL, FAIL> fa, Func<A, B> fb) =>
            ma.Match(
                Fail:    a => Validation<MonoidFail, FAIL, B>.Fail(fa(a)),
                Succ:    b => Validation<MonoidFail, FAIL, B>.Success(fb(b)));

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<MonoidFail, FAIL, B> Map(Validation<MonoidFail, FAIL, A> ma, Func<A, B> f) =>
             ma.Match(
                Fail: Validation<MonoidFail, FAIL, B>.Fail,
                Succ: b => Validation<MonoidFail, FAIL, B>.Success(f(b)));
    }
}
