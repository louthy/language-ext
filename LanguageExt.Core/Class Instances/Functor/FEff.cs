#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt.ClassInstances
{
    public readonly struct FEff<A, B> : 
        Functor<Eff<A>, Eff<B>, A, B>,
        BiFunctor<Eff<A>, Eff<B>, Error, A, Error, B>
    {
        public static readonly FEff<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Eff<B> Map(Eff<A> ma, Func<A, B> f) =>
            ma.Map(f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Eff<B> BiMap(Eff<A> ma, Func<Error, Error> fa, Func<A, B> fb) =>
            ma.BiMap(fb, fa);
    }
    
    public readonly struct FEff<RT, A, B> : 
        Functor<Eff<RT, A>, Eff<RT, B>, A, B>,
        BiFunctor<Eff<RT, A>, Eff<RT, B>, Error, A, Error, B>
        where RT: struct
    {
        public static readonly FEff<RT, A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Eff<RT, B> Map(Eff<RT, A> ma, Func<A, B> f) =>
            ma.Map(f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Eff<RT, B> BiMap(Eff<RT, A> ma, Func<Error, Error> fa, Func<A, B> fb) =>
            ma.BiMap(fb, fa);
    }
}
