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
    public static IO<RT, E, Arr<B>> SequenceParallel<RT, E, A, B>(this Arr<A> ta, Func<A, IO<RT, E, B>> f) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, Arr<B>> SequenceParallel<RT, E, A, B>(this Arr<A> ta, Func<A, IO<RT, E, B>> f, int windowSize) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, Arr<B>> SequenceSerial<RT, E, A, B>(this Arr<A> ta, Func<A, IO<RT, E, B>> f) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, L, Either<L, B>> Sequence<RT, L, A, B>(this Either<L, A> ta, Func<A, IO<RT, L, B>> f) 
        where RT : HasIO<RT, L> =>
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
    public static IO<RT, E, Identity<B>> Sequence<RT, E, A, B>(this Identity<A> ta, Func<A, IO<RT, E, B>> f) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, IEnumerable<B>> SequenceSerial<RT, E, A, B>(this IEnumerable<A> ta, Func<A, IO<RT, E, B>> f) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, IEnumerable<B>> SequenceParallel<RT, E, A, B>(this IEnumerable<A> ta, Func<A, IO<RT, E, B>> f) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, IEnumerable<B>> SequenceParallel<RT, E, A, B>(this IEnumerable<A> ta, Func<A, IO<RT, E, B>> f, int windowSize) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, Lst<B>> SequenceParallel<RT, E, A, B>(this Lst<A> ta, Func<A, IO<RT, E, B>> f) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, Lst<B>> SequenceParallel<RT, E, A, B>(this Lst<A> ta, Func<A, IO<RT, E, B>> f, int windowSize) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, Lst<B>> SequenceSerial<RT, E, A, B>(this Lst<A> ta, Func<A, IO<RT, E, B>> f) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, Fin<B>> Sequence<RT, E, A, B>(this Fin<A> ta, Func<A, IO<RT, E, B>> f) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, Option<B>> Sequence<RT, E, A, B>(this Option<A> ta, Func<A, IO<RT, E, B>> f) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, Seq<B>> SequenceSerial<RT, E, A, B>(this Seq<A> ta, Func<A, IO<RT, E, B>> f) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, Seq<B>> SequenceParallel<RT, E, A, B>(this Seq<A> ta, Func<A, IO<RT, E, B>> f) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, Seq<B>> SequenceParallel<RT, E, A, B>(this Seq<A> ta, Func<A, IO<RT, E, B>> f, int windowSize) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, Set<B>> SequenceSerial<RT, E, A, B>(this Set<A> ta, Func<A, IO<RT, E, B>> f) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, Set<B>> SequenceParallel<RT, E, A, B>(this Set<A> ta, Func<A, IO<RT, E, B>> f) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, Set<B>> SequenceParallel<RT, E, A, B>(this Set<A> ta, Func<A, IO<RT, E, B>> f, int windowSize) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, HashSet<B>> SequenceSerial<RT, E, A, B>(this HashSet<A> ta, Func<A, IO<RT, E, B>> f) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, HashSet<B>> SequenceParallel<RT, E, A, B>(this HashSet<A> ta, Func<A, IO<RT, E, B>> f) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, HashSet<B>> SequenceParallel<RT, E, A, B>(this HashSet<A> ta, Func<A, IO<RT, E, B>> f, int windowSize) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, Que<B>> SequenceSerial<RT, E, A, B>(this Que<A> ta, Func<A, IO<RT, E, B>> f) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, Que<B>> SequenceParallel<RT, E, A, B>(this Que<A> ta, Func<A, IO<RT, E, B>> f) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, Que<B>> SequenceParallel<RT, E, A, B>(this Que<A> ta, Func<A, IO<RT, E, B>> f, int windowSize) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, Stck<B>> SequenceSerial<RT, E, A, B>(this Stck<A> ta, Func<A, IO<RT, E, B>> f) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, Stck<B>> SequenceParallel<RT, E, A, B>(this Stck<A> ta, Func<A, IO<RT, E, B>> f) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, E, Stck<B>> SequenceParallel<RT, E, A, B>(this Stck<A> ta, Func<A, IO<RT, E, B>> f, int windowSize) 
        where RT : HasIO<RT, E> =>
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
    public static IO<RT, FAIL, Validation<FAIL, B>> Sequence<RT, FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, IO<RT, FAIL, B>> f) 
        where RT : HasIO<RT, FAIL> => 
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
    public static IO<RT, FAIL, Validation<MonoidFail, FAIL, B>> Sequence<RT, MonoidFail, FAIL, A, B>(
        this Validation<MonoidFail, FAIL, A> ta, Func<A, IO<RT, FAIL, B>> f)
        where MonoidFail : Monoid<FAIL>, Eq<FAIL> 
        where RT : HasIO<RT, FAIL> =>
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
    public static IO<RT, E, Task<B>> Sequence<RT, E, A, B>(this Task<A> ta, Func<A, IO<RT, E, B>> f) 
        where RT : HasIO<RT, E> =>
        ta.Map(f).Sequence();
}
