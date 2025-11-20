using System.Collections.Generic;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
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
    public static K<M, Seq<A>> succs<F, M, A>(
        K<F, K<M, A>> fma)
        where M : Monad<M>, Fallible<M>
        where F : Foldable<F> =>
        fma.Fold(M.Pure(LanguageExt.Seq.empty<A>()),
                 ma => ms => ms.Bind(
                           s => ma.Bind(a => M.Pure(s.Add(a)))
                                  .Catch(_ => M.Pure(s))));
    
    /// <summary>
    /// Partitions a collection of effects into successes and failures,
    /// and returns only the failures.
    /// </summary>
    /// <typeparam name="M">Fallible monadic type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fma">Collection of fallible monadic values</param>
    /// <returns>A collection of `Error` values</returns>
    public static K<M, Seq<A>> succs<M, A>(
        Seq<K<M, A>> fma)
        where M : Monad<M>, Fallible<M> =>
        fma.Kind().Succs();    
    
    /// <summary>
    /// Partitions a collection of effects into successes and failures,
    /// and returns only the failures.
    /// </summary>
    /// <typeparam name="M">Fallible monadic type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fma">Collection of fallible monadic values</param>
    /// <returns>A collection of `Error` values</returns>
    public static K<M, Seq<A>> succs<M, A>(
        Iterable<K<M, A>> fma)
        where M : Monad<M>, Fallible<M> =>
        fma.Kind().Succs();    
    
    /// <summary>
    /// Partitions a collection of effects into successes and failures,
    /// and returns only the failures.
    /// </summary>
    /// <typeparam name="M">Fallible monadic type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fma">Collection of fallible monadic values</param>
    /// <returns>A collection of `Error` values</returns>
    public static K<M, Seq<A>> succs<M, A>(
        Lst<K<M, A>> fma)
        where M : Monad<M>, Fallible<M> =>
        fma.Kind().Succs();
    
    /// <summary>
    /// Partitions a collection of effects into successes and failures,
    /// and returns only the failures.
    /// </summary>
    /// <typeparam name="M">Fallible monadic type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fma">Collection of fallible monadic values</param>
    /// <returns>A collection of `Error` values</returns>
    public static K<M, Seq<A>> succs<M, A>(
        IEnumerable<K<M, A>> fma)
        where M : Monad<M>, Fallible<M> =>
        LanguageExt.Iterable.createRange(fma).Succs();
    
    /// <summary>
    /// Partitions a collection of effects into successes and failures,
    /// and returns only the failures.
    /// </summary>
    /// <typeparam name="M">Fallible monadic type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fma">Collection of fallible monadic values</param>
    /// <returns>A collection of `Error` values</returns>
    public static K<M, Seq<A>> succs<M, A>(
        HashSet<K<M, A>> fma)
        where M : Monad<M>, Fallible<M> =>
        fma.Kind().Succs();
    
    /// <summary>
    /// Partitions a collection of effects into successes and failures,
    /// and returns only the failures.
    /// </summary>
    /// <typeparam name="M">Fallible monadic type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fma">Collection of fallible monadic values</param>
    /// <returns>A collection of `Error` values</returns>
    public static K<M, Seq<A>> succs<M, A>(
        Set<K<M, A>> fma)
        where M : Monad<M>, Fallible<M> =>
        fma.Kind().Succs();    
}
