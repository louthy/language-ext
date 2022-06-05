#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class QueT
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
        public static Que<Arr<B>> Sequence<A, B>(this Arr<A> ta, Func<A, Que<B>> f) =>
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
        public static Que<Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, Que<B>> f) =>
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
        public static Que<EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, Que<B>> f) =>
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
        public static Que<Identity<B>> Sequence<A, B>(this Identity<A> ta, Func<A, Que<B>> f) =>
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
        public static Que<IEnumerable<B>> Sequence<A, B>(this IEnumerable<A> ta, Func<A, Que<B>> f) =>
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
        public static Que<Fin<B>> Sequence<A, B>(this Fin<A> ta, Func<A, Que<B>> f) =>
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
        public static Que<Option<B>> Sequence<A, B>(this Option<A> ta, Func<A, Que<B>> f) =>
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
        public static Que<OptionUnsafe<B>> Sequence<A, B>(this OptionUnsafe<A> ta, Func<A, Que<B>> f) =>
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
        public static Que<Seq<B>> Sequence<A, B>(this Seq<A> ta, Func<A, Que<B>> f) =>
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
        public static Que<Set<B>> Sequence<A, B>(this Set<A> ta, Func<A, Que<B>> f) =>
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
        public static Que<Try<B>> Sequence<A, B>(this Try<A> ta, Func<A, Que<B>> f) =>
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
        public static Que<TryOption<B>> Sequence<A, B>(this TryOption<A> ta, Func<A, Que<B>> f) =>
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
        public static Que<Validation<FAIL, B>> Sequence<FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, Que<B>> f) => 
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
        public static Que<Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, Que<B>> f)
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
        public static Que<Eff<B>> Sequence<A, B>(this Eff<A> ta, Func<A, Que<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
    }
}
