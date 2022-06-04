#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class EitherT
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
        public static Either<L, Arr<B>> Sequence<L, A, B>(this Arr<A> ta, Func<A, Either<L, B>> f) =>
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
        public static Either<L, Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, Either<L, B>> f) =>
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
        public static Either<L, EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, Either<L, B>> f) =>
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
        public static Either<L, Identity<B>> Sequence<L, A, B>(this Identity<A> ta, Func<A, Either<L, B>> f) =>
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
        public static Either<L, IEnumerable<B>> Sequence<L, A, B>(this IEnumerable<A> ta, Func<A, Either<L, B>> f) =>
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
        public static Either<L, Lst<B>> Sequence<L, A, B>(this Lst<A> ta, Func<A, Either<L, B>> f) =>
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
        public static Either<L, Fin<B>> Sequence<L, A, B>(this Fin<A> ta, Func<A, Either<L, B>> f) =>
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
        public static Either<L, Option<B>> Sequence<L, A, B>(this Option<A> ta, Func<A, Either<L, B>> f) =>
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
        public static Either<L, OptionUnsafe<B>> Sequence<L, A, B>(this OptionUnsafe<A> ta, Func<A, Either<L, B>> f) =>
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
        public static Either<L, Seq<B>> Sequence<L, A, B>(this Seq<A> ta, Func<A, Either<L, B>> f) =>
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
        public static Either<L, Set<B>> Sequence<L, A, B>(this Set<A> ta, Func<A, Either<L, B>> f) =>
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
        public static Either<L, HashSet<B>> Sequence<L, A, B>(this HashSet<A> ta, Func<A, Either<L, B>> f) =>
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
        public static Either<L, Try<B>> Sequence<L, A, B>(this Try<A> ta, Func<A, Either<L, B>> f) =>
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
        public static Either<L, TryOption<B>> Sequence<L, A, B>(this TryOption<A> ta, Func<A, Either<L, B>> f) =>
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
        public static Either<L, Validation<FAIL, B>> Sequence<FAIL, L, A, B>(this Validation<FAIL, A> ta, Func<A, Either<L, B>> f) => 
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
        public static Either<FAIL, Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, Either<FAIL, B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
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
        public static Either<L, Eff<B>> Sequence<L, A, B>(this Eff<A> ta, Func<A, Either<L, B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
    }
}
