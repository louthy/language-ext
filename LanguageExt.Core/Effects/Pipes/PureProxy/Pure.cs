#nullable enable

using System;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using static LanguageExt.Pipes.Proxy;
using System.Runtime.CompilerServices;

namespace LanguageExt.Pipes
{
    /// <summary>
    /// Pure value
    /// </summary>
    /// <param name="Value">Value</param>
    /// <typeparam name="A">Value type</typeparam>
    public record struct Pure<A>(A Value)
    {
        /// <summary>
        /// Functor map
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pure<B> Map<B>(Func<A, B> f) =>
            new(f(Value));
        
        /// <summary>
        /// Functor map
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pure<B> Select<B>(Func<A, B> f) =>
            new (f(Value));
        
        /// <summary>
        /// Monad bind
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pure<B> SelectMany<B>(Func<A, Pure<B>> f) =>
            f(Value);
        
        /// <summary>
        /// Monad bind
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pure<B> Bind<B>(Func<A, Pure<B>> f) =>
            f(Value);
        
        /// <summary>
        /// Monad bind
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pure<C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
            new(project(Value, bind(Value).Value));
    }
}
