#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;

namespace LanguageExt.ClassInstances
{
    public readonly struct FFin<A, B> : 
        Functor<Fin<A>, Fin<B>, A, B>,
        BiFunctor<Fin<A>, Fin<B>, A, Error, B, Error>
    {
        public static readonly FFin<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fin<B> BiMap(Fin<A> ma, Func<A, B> fa, Func<Error, Error> fb) =>
            ma switch
            {
                { IsSucc: true} a => fa(a.Value),
                { IsFail: true} a => fb(a.Error),
                _ => Errors.Bottom
            };

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Fin<B> Map(Fin<A> ma, Func<A, B> f) =>
            ma switch
            {
                { IsSucc: true} a => f(a.Value),
                { IsFail: true} a => a.Error,
                _ => Errors.Bottom
            };
    }
}
