#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public readonly struct FTryOption<A, B> :
        Functor<TryOption<A>, TryOption<B>, A, B>,
        BiFunctor<TryOption<A>, TryOption<B>, Error, A, Error, B>
    {
        public static readonly FTryOption<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryOption<B> Map(TryOption<A> ma, Func<A, B> f) => 
            ma.Map(f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TryOption<B> BiMap(TryOption<A> ma, Func<Error, Error> fa, Func<A, B> fb) => 
            ma.BiMap(fb, fa);
    }
}
