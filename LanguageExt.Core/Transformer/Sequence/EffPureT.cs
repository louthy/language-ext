#nullable enable
using System;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt
{
    public partial class EffPureT
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
        public static Eff<Arr<B>> Sequence<A, B>(this Arr<A> ta, Func<A, Eff<B>> f) =>
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
        public static Eff<Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, Eff<B>> f) =>
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
        public static Eff<EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, Eff<B>> f) =>
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
        public static Eff<Identity<B>> Sequence<A, B>(this Identity<A> ta, Func<A, Eff<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

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
        public static Eff<IEnumerable<B>> Sequence<A, B>(this IEnumerable<A> ta, Func<A, Eff<B>> f) =>
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
        public static Eff<Lst<B>> Sequence<A, B>(this Lst<A> ta, Func<A, Eff<B>> f) =>
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
        public static Eff<Fin<B>> Sequence<A, B>(this Fin<A> ta, Func<A, Eff<B>> f) =>
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
        public static Eff<Option<B>> Sequence<A, B>(this Option<A> ta, Func<A, Eff<B>> f) =>
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
        public static Eff<OptionUnsafe<B>> Sequence<A, B>(this OptionUnsafe<A> ta, Func<A, Eff<B>> f) =>
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
        public static Eff<Seq<B>> Sequence<A, B>(this Seq<A> ta, Func<A, Eff<B>> f) =>
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
        public static Eff<Set<B>> Sequence<A, B>(this Set<A> ta, Func<A, Eff<B>> f) =>
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
        public static Eff<HashSet<B>> Sequence<A, B>(this HashSet<A> ta, Func<A, Eff<B>> f) =>
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
        public static Eff<Que<B>> Sequence<A, B>(this Que<A> ta, Func<A, Eff<B>> f) =>
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
        public static Eff<Stck<B>> Sequence<A, B>(this Stck<A> ta, Func<A, Eff<B>> f) =>
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
        public static Eff<Try<B>> Sequence<A, B>(this Try<A> ta, Func<A, Eff<B>> f) =>
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
        public static Eff<TryOption<B>> Sequence<A, B>(this TryOption<A> ta, Func<A, Eff<B>> f) =>
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
        public static Eff<Validation<FAIL, B>> Sequence<FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, Eff<B>> f) => 
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
        public static Eff<Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, Eff<B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ta.Map(f).Sequence();
    }
}
