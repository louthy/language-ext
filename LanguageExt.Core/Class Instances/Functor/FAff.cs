#nullable enable

using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt.ClassInstances
{
    public readonly struct FAff<A, B> : 
        FunctorAsync<Aff<A>, Aff<B>, A, B>,
        BiFunctorAsync<Aff<A>, Aff<B>, Error, A, Error, B>
    {
        public static readonly FAff<A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aff<B> Map(Aff<A> ma, Func<A, B> f) =>
            ma.Map(f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aff<B> MapAsync(Aff<A> ma, Func<A, ValueTask<B>> f) =>
            ma.MapAsync(f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aff<B> BiMapAsync(Aff<A> ma, Func<Error, ValueTask<Error>> fa, Func<A, ValueTask<B>> fb) =>
            ma.BiMapAsync(fb, fa);
    }
    
    public readonly struct FAff<RT, A, B> : 
        FunctorAsync<Aff<RT, A>, Aff<RT, B>, A, B>,
        BiFunctorAsync<Aff<RT, A>, Aff<RT, B>, Error, A, Error, B>
        where RT: struct, HasCancel<RT>
    {
        public static readonly FAff<RT, A, B> Inst = default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aff<RT, B> Map(Aff<RT, A> ma, Func<A, B> f) =>
            ma.Map(f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aff<RT, B> MapAsync(Aff<RT, A> ma, Func<A, ValueTask<B>> f) =>
            ma.MapAsync(f);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Aff<RT, B> BiMapAsync(Aff<RT, A> ma, Func<Error, ValueTask<Error>> fa, Func<A, ValueTask<B>> fb) =>
            ma.BiMapAsync(fb, fa);
    }
}
