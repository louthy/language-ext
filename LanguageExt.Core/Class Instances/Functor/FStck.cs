#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct FStck<A, B> : Functor<Stck<A>, Stck<B>, A, B>
    {
        public static readonly FStck<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Stck<B> Map(Stck<A> ma, Func<A, B> f) =>
            ma.Map(f);
    }
}
