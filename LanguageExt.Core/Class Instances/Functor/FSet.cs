#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct FSet<A, B> : Functor<Set<A>, Set<B>, A, B>
    {
        public static readonly FSet<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Set<B> Map(Set<A> ma, Func<A, B> f) =>
            ma.Map(f);
    }
}
