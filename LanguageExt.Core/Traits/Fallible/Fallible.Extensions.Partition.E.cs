using System.Collections.Generic;
using LanguageExt.Common;

namespace LanguageExt.Traits;

public static partial class FallibleExtensionsE
{
    /// <summary>
    /// Partitions a foldable of effects into two lists.
    /// All the `Fail` elements are extracted, in order, to the first
    /// component of the output.  Similarly, the `Succ` elements are extracted
    /// to the second component of the output.
    /// </summary>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="F">Foldable type</typeparam>
    /// <typeparam name="M">Fallible monadic type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fma">Foldable of fallible monadic values</param>
    /// <returns>A tuple containing an `Error` sequence and a `Succ` sequence</returns>
    public static K<M, (Seq<E> Fails, Seq<A> Succs)> Partition<E, F, M, A>(
        this K<F, K<M, A>> fma)
        where M : Monad<M>, Fallible<E, M>
        where F : Foldable<F> =>
        fma.Fold(M.Pure((Fails: Seq.empty<E>(), Succs: Seq.empty<A>())),
                 ma => ms => ms.Bind(
                           s => ma.Bind(a => M.Pure((s.Fails, s.Succs.Add(a))))
                                  .Catch((E e) => M.Pure((s.Fails.Add(e), s.Succs)))));
    
    /// <summary>
    /// Partitions a collection of effects into two lists.
    /// All the `Fail` elements are extracted, in order, to the first
    /// component of the output.  Similarly, the `Succ` elements are extracted
    /// to the second component of the output.
    /// </summary>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="M">Fallible monadic type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fma">Collection of fallible monadic values</param>
    /// <returns>A tuple containing an `Error` sequence and a `Succ` sequence</returns>
    public static K<M, (Seq<E> Fails, Seq<A> Succs)> Partition<E, M, A>(
        this Seq<K<M, A>> fma)
        where M : Monad<M>, Fallible<E, M> =>
        fma.Kind().Partition<E, Seq, M, A>();    
    
    /// <summary>
    /// Partitions a collection of effects into two lists.
    /// All the `Fail` elements are extracted, in order, to the first
    /// component of the output.  Similarly, the `Succ` elements are extracted
    /// to the second component of the output.
    /// </summary>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="M">Fallible monadic type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fma">Collection of fallible monadic values</param>
    /// <returns>A tuple containing an `Error` sequence and a `Succ` sequence</returns>
    public static K<M, (Seq<E> Fails, Seq<A> Succs)> Partition<E, M, A>(
        this Iterable<K<M, A>> fma)
        where M : Monad<M>, Fallible<E, M> =>
        fma.Kind().Partition<E, Iterable, M, A>();    
    
    /// <summary>
    /// Partitions a collection of effects into two lists.
    /// All the `Fail` elements are extracted, in order, to the first
    /// component of the output.  Similarly, the `Succ` elements are extracted
    /// to the second component of the output.
    /// </summary>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="M">Fallible monadic type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fma">Collection of fallible monadic values</param>
    /// <returns>A tuple containing an `Error` sequence and a `Succ` sequence</returns>
    public static K<M, (Seq<E> Fails, Seq<A> Succs)> Partition<E, M, A>(
        this Lst<K<M, A>> fma)
        where M : Monad<M>, Fallible<E, M> =>
        fma.Kind().Partition<E, Lst, M, A>();
    
    /// <summary>
    /// Partitions a collection of effects into two lists.
    /// All the `Fail` elements are extracted, in order, to the first
    /// component of the output.  Similarly, the `Succ` elements are extracted
    /// to the second component of the output.
    /// </summary>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="M">Fallible monadic type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fma">Collection of fallible monadic values</param>
    /// <returns>A tuple containing an `Error` sequence and a `Succ` sequence</returns>
    public static K<M, (Seq<E> Fails, Seq<A> Succs)> Partition<E, M, A>(
        this IEnumerable<K<M, A>> fma)
        where M : Monad<M>, Fallible<E, M> =>
        Iterable.createRange(fma).Partition<E, Iterable, M, A>();
    
    /// <summary>
    /// Partitions a collection of effects into two lists.
    /// All the `Fail` elements are extracted, in order, to the first
    /// component of the output.  Similarly, the `Succ` elements are extracted
    /// to the second component of the output.
    /// </summary>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="M">Fallible monadic type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fma">Collection of fallible monadic values</param>
    /// <returns>A tuple containing an `Error` sequence and a `Succ` sequence</returns>
    public static K<M, (Seq<E> Fails, Seq<A> Succs)> Partition<E, M, A>(
        this HashSet<K<M, A>> fma)
        where M : Monad<M>, Fallible<E, M> =>
        fma.Kind().Partition<E, HashSet, M, A>();
    
    /// <summary>
    /// Partitions a collection of effects into two lists.
    /// All the `Fail` elements are extracted, in order, to the first
    /// component of the output.  Similarly, the `Succ` elements are extracted
    /// to the second component of the output.
    /// </summary>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="M">Fallible monadic type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fma">Collection of fallible monadic values</param>
    /// <returns>A tuple containing an `Error` sequence and a `Succ` sequence</returns>
    public static K<M, (Seq<E> Fails, Seq<A> Succs)> Partition<E, M, A>(
        this Set<K<M, A>> fma)
        where M : Monad<M>, Fallible<E, M> =>
        fma.Kind().Partition<E, Set, M, A>();    
}
