#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class RwsT
    {
        /// <summary>
        /// Traverses each value in the `ta`, applies it to `f`.  The resulting monadic value is then repeatedly
        /// bound using the monad bind operation, which means the monad laws of the result-type are followed at each
        /// step.  Resulting in a monad that has an inner value of the subject. 
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <param name="f">Mapping and lifting operation</param>
        /// <returns>Mapped and lifted monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RWS<MonoidW, Env, W, S, Arr<B>> Sequence<MonoidW, Env, W, S, A, B>(this Arr<A> ta, Func<A, RWS<MonoidW, Env,  W, S, B>> f) 
            where MonoidW : struct, Monoid<W> =>
            ta.Map(f).Sequence();

        /// <summary>
        /// Traverses each value in the `ta`, applies it to `f`.  The resulting monadic value is then repeatedly
        /// bound using the monad bind operation, which means the monad laws of the result-type are followed at each
        /// step.  Resulting in a monad that has an inner value of the subject. 
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <param name="f">Mapping and lifting operation</param>
        /// <returns>Mapped and lifted monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RWS<MonoidW, Env, W, S, IEnumerable<B>> Sequence<MonoidW, Env, W, S, A, B>(this IEnumerable<A> ta, Func<A, RWS<MonoidW, Env,  W, S, B>> f)
            where MonoidW : struct, Monoid<W> =>
            ta.Map(f).Sequence();
        
        /// <summary>
        /// Traverses each value in the `ta`, applies it to `f`.  The resulting monadic value is then repeatedly
        /// bound using the monad bind operation, which means the monad laws of the result-type are followed at each
        /// step.  Resulting in a monad that has an inner value of the subject. 
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <param name="f">Mapping and lifting operation</param>
        /// <returns>Mapped and lifted monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RWS<MonoidW, Env, W, S, Set<B>> Sequence<MonoidW, Env, W, S, A, B>(this Set<A> ta, Func<A, RWS<MonoidW, Env,  W, S, B>> f)
            where MonoidW : struct, Monoid<W> =>
            ta.Map(f).Sequence();
        
        /// <summary>
        /// Traverses each value in the `ta`, applies it to `f`.  The resulting monadic value is then repeatedly
        /// bound using the monad bind operation, which means the monad laws of the result-type are followed at each
        /// step.  Resulting in a monad that has an inner value of the subject. 
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <param name="f">Mapping and lifting operation</param>
        /// <returns>Mapped and lifted monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RWS<MonoidW, Env, W, S, Seq<B>> Sequence<MonoidW, Env, W, S, A, B>(this Seq<A> ta, Func<A, RWS<MonoidW, Env,  W, S, B>> f)
            where MonoidW : struct, Monoid<W> =>
            ta.Map(f).Sequence();
        
        /// <summary>
        /// Traverses each value in the `ta`, applies it to `f`.  The resulting monadic value is then repeatedly
        /// bound using the monad bind operation, which means the monad laws of the result-type are followed at each
        /// step.  Resulting in a monad that has an inner value of the subject. 
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <param name="f">Mapping and lifting operation</param>
        /// <returns>Mapped and lifted monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RWS<MonoidW, Env, W, S, Lst<B>> Sequence<MonoidW, Env, W, S, A, B>(this Lst<A> ta, Func<A, RWS<MonoidW, Env,  W, S, B>> f)
            where MonoidW : struct, Monoid<W> =>
            ta.Map(f).Sequence();
        
        /// <summary>
        /// Traverses each value in the `ta`, applies it to `f`.  The resulting monadic value is then repeatedly
        /// bound using the monad bind operation, which means the monad laws of the result-type are followed at each
        /// step.  Resulting in a monad that has an inner value of the subject. 
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <param name="f">Mapping and lifting operation</param>
        /// <returns>Mapped and lifted monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RWS<MonoidW, Env, W, S, HashSet<B>> Sequence<MonoidW, Env, W, S, A, B>(this HashSet<A> ta, Func<A, RWS<MonoidW, Env,  W, S, B>> f)
            where MonoidW : struct, Monoid<W> =>
            ta.Map(f).Sequence();
        
        /// <summary>
        /// Traverses each value in the `ta`, applies it to `f`.  The resulting monadic value is then repeatedly
        /// bound using the monad bind operation, which means the monad laws of the result-type are followed at each
        /// step.  Resulting in a monad that has an inner value of the subject. 
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ta">The subject traversable</param>
        /// <param name="f">Mapping and lifting operation</param>
        /// <returns>Mapped and lifted monad</returns>
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RWS<MonoidW, Env, W, S, Stck<B>> Sequence<MonoidW, Env, W, S, A, B>(this Stck<A> ta, Func<A, RWS<MonoidW, Env,  W, S, B>> f)
            where MonoidW : struct, Monoid<W> =>
            ta.Map(f).Sequence();
    }
}
