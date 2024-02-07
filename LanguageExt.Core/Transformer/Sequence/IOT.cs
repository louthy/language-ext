using System;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Effects.Traits;

namespace LanguageExt;

public static partial class IOT
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
    public static IO<E, Arr<B>> SequenceParallel<E, A, B>(this Arr<A> ta, Func<A, IO<E, B>> f) => 
        ta.Map(f).SequenceParallel();
        
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
    public static IO<E, Arr<B>> SequenceParallel<E, A, B>(this Arr<A> ta, Func<A, IO<E, B>> f, int windowSize) => 
        ta.Map(f).SequenceParallel(windowSize);
        
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
    public static IO<E, Arr<B>> SequenceSerial<E, A, B>(this Arr<A> ta, Func<A, IO<E, B>> f) =>
        ta.Map(f).SequenceSerial();

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
    public static IO<L, Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, IO<L, B>> f) =>
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
    public static IO<E, Identity<B>> Sequence<E, A, B>(this Identity<A> ta, Func<A, IO<E, B>> f) => 
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
    public static IO<E, IEnumerable<B>> SequenceSerial<E, A, B>(this IEnumerable<A> ta, Func<A, IO<E, B>> f) => 
        ta.Map(f).SequenceSerial();

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
    public static IO<E, IEnumerable<B>> SequenceParallel<E, A, B>(this IEnumerable<A> ta, Func<A, IO<E, B>> f) =>
        ta.Map(f).SequenceParallel();

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
    public static IO<E, IEnumerable<B>> SequenceParallel<E, A, B>(this IEnumerable<A> ta, Func<A, IO<E, B>> f, int windowSize) => 
        ta.Map(f).SequenceParallel(windowSize);

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
    public static IO<E, Lst<B>> SequenceParallel<E, A, B>(this Lst<A> ta, Func<A, IO<E, B>> f) => 
        ta.Map(f).SequenceParallel();
        
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
    public static IO<E, Lst<B>> SequenceParallel<E, A, B>(this Lst<A> ta, Func<A, IO<E, B>> f, int windowSize) => 
        ta.Map(f).SequenceParallel(windowSize);
        
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
    public static IO<E, Lst<B>> SequenceSerial<E, A, B>(this Lst<A> ta, Func<A, IO<E, B>> f) => 
        ta.Map(f).SequenceSerial();

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
    public static IO<E, Fin<B>> Sequence<E, A, B>(this Fin<A> ta, Func<A, IO<E, B>> f) =>
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
    public static IO<E, Option<B>> Sequence<E, A, B>(this Option<A> ta, Func<A, IO<E, B>> f) =>
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
    public static IO<E, Seq<B>> SequenceSerial<E, A, B>(this Seq<A> ta, Func<A, IO<E, B>> f) =>
        ta.Map(f).SequenceSerial();
        
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
    public static IO<E, Seq<B>> SequenceParallel<E, A, B>(this Seq<A> ta, Func<A, IO<E, B>> f) =>
        ta.Map(f).SequenceParallel();
        
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
    public static IO<E, Seq<B>> SequenceParallel<E, A, B>(this Seq<A> ta, Func<A, IO<E, B>> f, int windowSize) => 
        ta.Map(f).SequenceParallel(windowSize);
        
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
    public static IO<E, Set<B>> SequenceSerial<E, A, B>(this Set<A> ta, Func<A, IO<E, B>> f) =>
        ta.Map(f).SequenceSerial();
        
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
    public static IO<E, Set<B>> SequenceParallel<E, A, B>(this Set<A> ta, Func<A, IO<E, B>> f) =>
        ta.Map(f).SequenceParallel();
        
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
    public static IO<E, Set<B>> SequenceParallel<E, A, B>(this Set<A> ta, Func<A, IO<E, B>> f, int windowSize) => 
        ta.Map(f).SequenceParallel(windowSize);
        
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
    public static IO<E, HashSet<B>> SequenceSerial<E, A, B>(this HashSet<A> ta, Func<A, IO<E, B>> f) => 
        ta.Map(f).SequenceSerial();
        
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
    public static IO<E, HashSet<B>> SequenceParallel<E, A, B>(this HashSet<A> ta, Func<A, IO<E, B>> f) => 
        ta.Map(f).SequenceParallel();
        
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
    public static IO<E, HashSet<B>> SequenceParallel<E, A, B>(this HashSet<A> ta, Func<A, IO<E, B>> f, int windowSize) => 
        ta.Map(f).SequenceParallel(windowSize);
        
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
    public static IO<E, Que<B>> SequenceSerial<E, A, B>(this Que<A> ta, Func<A, IO<E, B>> f) => 
        ta.Map(f).SequenceSerial();
        
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
    public static IO<E, Que<B>> SequenceParallel<E, A, B>(this Que<A> ta, Func<A, IO<E, B>> f) =>
        ta.Map(f).SequenceParallel();
        
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
    public static IO<E, Que<B>> SequenceParallel<E, A, B>(this Que<A> ta, Func<A, IO<E, B>> f, int windowSize) => 
        ta.Map(f).SequenceParallel(windowSize);
        
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
    public static IO<E, Stck<B>> SequenceSerial<E, A, B>(this Stck<A> ta, Func<A, IO<E, B>> f) =>
        ta.Map(f).SequenceSerial();
        
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
    public static IO<E, Stck<B>> SequenceParallel<E, A, B>(this Stck<A> ta, Func<A, IO<E, B>> f) =>
        ta.Map(f).SequenceParallel();
        
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
    public static IO<E, Stck<B>> SequenceParallel<E, A, B>(this Stck<A> ta, Func<A, IO<E, B>> f, int windowSize) => 
        ta.Map(f).SequenceParallel(windowSize);
        
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
    public static IO<FAIL, Validation<FAIL, B>> Sequence<FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, IO<FAIL, B>> f) => 
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
    public static IO<FAIL, Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(
        this Validation<MonoidFail, FAIL, A> ta, Func<A, IO<FAIL, B>> f)
        where MonoidFail : Monoid<FAIL>, Eq<FAIL>  =>
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
    public static IO<E, Task<B>> Sequence<E, A, B>(this Task<A> ta, Func<A, IO<E, B>> f) => 
        ta.Map(f).Sequence();
}
