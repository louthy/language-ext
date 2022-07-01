#nullable enable

using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct FMap<K, A, B> : Functor<Map<K, A>, Map<K, B>, A, B>
    {
        public static readonly FMap<K, A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Map<K, B> Map(Map<K, A> ma, Func<A, B> f) =>
            ma.Map(f);
    }

    public readonly struct FMap<OrdK, K, A, B> : Functor<Map<OrdK, K, A>, Map<OrdK, K, B>, A, B>
        where OrdK : struct, Ord<K>
    {
        public static readonly FMap<OrdK, K, A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Map<OrdK, K, B> Map(Map<OrdK, K, A> ma, Func<A, B> f) =>
            ma.Map(f);
    }
}
