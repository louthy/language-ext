using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude; 
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Effects.Traits;

namespace LanguageExt;

public static partial class IOT
{
    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Arr<A>> SequenceParallel<E, A>(this Arr<IO<E, A>> ta) =>
        TraverseParallel(ta, identity, SysInfo.DefaultAsyncSequenceParallelism);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Arr<A>> SequenceParallel<E, A>(this Arr<IO<E, A>> ta, int windowSize) =>
        TraverseParallel(ta, identity, windowSize);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Arr<A>> SequenceSerial<E, A>(this Arr<IO<E, A>> ta) =>
        TraverseSerial(ta, identity);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<L, Either<L, A>> Sequence<L, A>(this Either<L, IO<L, A>> ta) =>
        ta.Traverse(identity);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Identity<A>> Sequence<E, A>(this Identity<IO<E, A>> ta) =>
        ta.Traverse(identity);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, IEnumerable<A>> SequenceParallel<E, A>(this IEnumerable<IO<E, A>> ta) =>
        TraverseParallel(ta, identity, SysInfo.DefaultAsyncSequenceParallelism);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, IEnumerable<A>> SequenceParallel<E, A>(this IEnumerable<IO<E, A>> ta, int windowSize) =>
        TraverseParallel(ta, identity, windowSize);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, IEnumerable<A>> SequenceSerial<E, A>(this IEnumerable<IO<E, A>> ta) =>
        TraverseSerial(ta, identity);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Lst<A>> SequenceParallel<E, A>(this Lst<IO<E, A>> ta) =>
        TraverseParallel(ta, identity, SysInfo.DefaultAsyncSequenceParallelism);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Lst<A>> SequenceParallel<E, A>(this Lst<IO<E, A>> ta, int windowSize) =>
        TraverseParallel(ta, identity, windowSize);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Lst<A>> SequenceSerial<E, A>(this Lst<IO<E, A>> ta) =>
        TraverseSerial(ta, identity);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Fin<A>> Sequence<E, A>(this Fin<IO<E, A>> ta) =>
        ta.Traverse(identity);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Option<A>> Sequence<E, A>(this Option<IO<E, A>> ta) =>
        ta.Traverse(identity);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Seq<A>> SequenceParallel<E, A>(this Seq<IO<E, A>> ta) =>
        TraverseParallel(ta, identity, SysInfo.DefaultAsyncSequenceParallelism);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Seq<A>> SequenceParallel<E, A>(this Seq<IO<E, A>> ta, int windowSize) =>
        TraverseParallel(ta, identity, windowSize);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Seq<A>> SequenceSerial<E, A>(this Seq<IO<E, A>> ta) =>
        TraverseSerial(ta, identity);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Set<A>> SequenceParallel<E, A>(this Set<IO<E, A>> ta) =>
        TraverseParallel(ta, identity, SysInfo.DefaultAsyncSequenceParallelism);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Set<A>> SequenceParallel<E, A>(this Set<IO<E, A>> ta, int windowSize) =>
        TraverseParallel(ta, identity, windowSize);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Set<A>> SequenceSerial<E, A>(this Set<IO<E, A>> ta) =>
        TraverseSerial(ta, identity);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, HashSet<A>> SequenceParallel<E, A>(this HashSet<IO<E, A>> ta) =>
        TraverseParallel(ta, identity, SysInfo.DefaultAsyncSequenceParallelism);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, HashSet<A>> SequenceParallel<E, A>(this HashSet<IO<E, A>> ta, int windowSize) =>
        TraverseParallel(ta, identity, windowSize);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, HashSet<A>> SequenceSerial<E, A>(this HashSet<IO<E, A>> ta) =>
        TraverseSerial(ta, identity);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Que<A>> SequenceParallel<E, A>(this Que<IO<E, A>> ta) =>
        TraverseParallel(ta, identity, SysInfo.DefaultAsyncSequenceParallelism);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Que<A>> SequenceParallel<E, A>(this Que<IO<E, A>> ta, int windowSize) =>
        TraverseParallel(ta, identity, windowSize);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Que<A>> SequenceSerial<E, A>(this Que<IO<E, A>> ta) =>
        TraverseSerial(ta, identity);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Stck<A>> SequenceSerial<E, A>(this Stck<IO<E, A>> ta) =>
        TraverseSerial(ta, identity);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Stck<A>> SequenceParallel<E, A>(this Stck<IO<E, A>> ta) =>
        TraverseParallel(ta, identity, SysInfo.DefaultAsyncSequenceParallelism);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Stck<A>> SequenceParallel<E, A>(this Stck<IO<E, A>> ta, int windowSize) =>
        TraverseParallel(ta, identity, windowSize);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<FAIL, Validation<FAIL, A>> Sequence<FAIL, A>(
        this Validation<FAIL, IO<FAIL, A>> ta) =>
        ta.Traverse(identity);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<FAIL, Validation<MonoidFail, FAIL, A>> Sequence<MonoidFail, FAIL, A>(
        this Validation<MonoidFail, FAIL, IO<FAIL, A>> ta)
        where MonoidFail : Monoid<FAIL>, Eq<FAIL> =>
        ta.Traverse(identity);

    /// <summary>
    /// Traverses each value in the `ta` nested monad,  Then applies the monadic rules of the return type
    /// (which is the input nested monad, flipped: so `M<N<A>>` becomes `N<M<A>>`).   
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ta">The subject traversable</param>
    /// <returns>Mapped monad</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<E, Task<A>> Sequence<E, A>(this Task<IO<E, A>> ta) =>
        ta.Traverse(identity);
}
