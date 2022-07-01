#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct FQue<A, B> : 
        Functor<Que<A>, Que<B>, A, B>
    {
        public static readonly FQue<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Que<B> Map(Que<A> ma, Func<A, B> f) =>
            new (ma.Map(f));
    }
}
