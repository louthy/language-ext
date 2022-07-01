#nullable enable

using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct FOptionUnsafe<A, B> : Functor<OptionUnsafe<A>, OptionUnsafe<B>, A, B>
    {
        public static readonly FOptionUnsafe<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public OptionUnsafe<B> Map(OptionUnsafe<A> ma, Func<A, B> f) =>
            ma switch
            {
                {IsSome: true, Value: not null} a => f(a.Value),
                _ => None
            };
    }
}
