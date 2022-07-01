#nullable enable

using System;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt.ClassInstances
{
    public readonly struct FOption<A, B> : 
        Functor<Option<A>, Option<B>, A, B>
    {
        public static readonly FOption<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Option<B> Map(Option<A> ma, Func<A, B> f) =>
            ma switch
            {
                {IsSome: true, Value: not null} a => f(a.Value),
                _ => None
            };
    }
}
