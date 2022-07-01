#nullable enable
using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct FValidation<FAIL, A, B> : 
        Functor<Validation<FAIL, A>, Validation<FAIL, B>, A, B>,
        BiFunctor<Validation<FAIL, A>, Validation<FAIL, B>, Seq<FAIL>, A, Seq<FAIL>, B>
    {
        public static readonly FValidation<FAIL, A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<FAIL, B> BiMap(Validation<FAIL, A> ma, Func<Seq<FAIL>, Seq<FAIL>> fa, Func<A, B> fb) =>
            ma.Match(
                Fail:    a => Validation<FAIL, B>.Fail(fa(a)),
                Succ:    b => Validation<FAIL, B>.Success(fb(b)));

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Validation<FAIL, B> Map(Validation<FAIL, A> ma, Func<A, B> f) =>
             ma.Match(
                Fail: Validation<FAIL, B>.Fail,
                Succ: b => Validation<FAIL, B>.Success(f(b)));
    }
}
