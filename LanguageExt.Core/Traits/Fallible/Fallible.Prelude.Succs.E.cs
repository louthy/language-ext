using System.Collections.Generic;
using LanguageExt.Common;

namespace LanguageExt.Traits;

public static partial class FallibleExtensionsE
{
    /// <summary>
    /// Partitions a foldable of effects into successes and failures,
    /// and returns only the failures.
    /// </summary>
    /// <typeparam name="F">Foldable type</typeparam>
    /// <typeparam name="M">Fallible monadic type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fma">Foldable of fallible monadic values</param>
    /// <returns>A collection of `Error` values</returns>
    public static K<M, Seq<A>> succs<E, F, M, A>(
        K<F, K<M, A>> fma)
        where M : Monad<M>, Fallible<E, M>
        where F : Foldable<F> =>
        fma.Fold(M.Pure(Seq.empty<A>()),
                 ma => ms => ms.Bind(
                           s => ma.Bind(a => M.Pure(s.Add(a)))
                                  .Catch((E _) => M.Pure(s))));
    
    /// <summary>
    /// Partitions a collection of effects into successes and failures,
    /// and returns only the failures.
    /// </summary>
    /// <typeparam name="M">Fallible monadic type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fma">Collection of fallible monadic values</param>
    /// <returns>A collection of `Error` values</returns>
    public static K<M, Seq<A>> succs<E, M, A>(
        Seq<K<M, A>> fma)
        where M : Monad<M>, Fallible<E, M> =>
        fma.Kind().Succs<E, Seq, M, A>();    
    
    /// <summary>
    /// Partitions a collection of effects into successes and failures,
    /// and returns only the failures.
    /// </summary>
    /// <typeparam name="M">Fallible monadic type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fma">Collection of fallible monadic values</param>
    /// <returns>A collection of `Error` values</returns>
    public static K<M, Seq<A>> succs<E, M, A>(
        Iterable<K<M, A>> fma)
        where M : Monad<M>, Fallible<E, M> =>
        fma.Kind().Succs<E, Iterable, M, A>();    
    
    /// <summary>
    /// Partitions a collection of effects into successes and failures,
    /// and returns only the failures.
    /// </summary>
    /// <typeparam name="M">Fallible monadic type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fma">Collection of fallible monadic values</param>
    /// <returns>A collection of `Error` values</returns>
    public static K<M, Seq<A>> succs<E, M, A>(
        Lst<K<M, A>> fma)
        where M : Monad<M>, Fallible<E, M> =>
        fma.Kind().Succs<E, Lst, M, A>();
    
    /// <summary>
    /// Partitions a collection of effects into successes and failures,
    /// and returns only the failures.
    /// </summary>
    /// <typeparam name="M">Fallible monadic type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fma">Collection of fallible monadic values</param>
    /// <returns>A collection of `Error` values</returns>
    public static K<M, Seq<A>> succs<E, M, A>(
        IEnumerable<K<M, A>> fma)
        where M : Monad<M>, Fallible<E, M> =>
        Iterable.createRange(fma).Succs<E, Iterable, M, A>();
    
    /// <summary>
    /// Partitions a collection of effects into successes and failures,
    /// and returns only the failures.
    /// </summary>
    /// <typeparam name="M">Fallible monadic type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fma">Collection of fallible monadic values</param>
    /// <returns>A collection of `Error` values</returns>
    public static K<M, Seq<A>> succs<E, M, A>(
        HashSet<K<M, A>> fma)
        where M : Monad<M>, Fallible<E, M> =>
        fma.Kind().Succs<E, HashSet, M, A>();
    
    /// <summary>
    /// Partitions a collection of effects into successes and failures,
    /// and returns only the failures.
    /// </summary>
    /// <typeparam name="M">Fallible monadic type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fma">Collection of fallible monadic values</param>
    /// <returns>A collection of `Error` values</returns>
    public static K<M, Seq<A>> succs<E, M, A>(
        Set<K<M, A>> fma)
        where M : Monad<M>, Fallible<E, M> =>
        fma.Kind().Succs<E, Set, M, A>();    
}
