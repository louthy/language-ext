using System.Collections.Generic;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
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
    public static K<M, (Seq<E> Fails, Seq<A> Succs)> partition<E, F, M, A>(
        K<F, K<M, A>> fma)
        where M : Monad<M>, Fallible<E, M>
        where F : Foldable<F> =>
        fma.Fold(M.Pure((Fails: LanguageExt.Seq.empty<E>(), Succs: LanguageExt.Seq.empty<A>())),
                 ma => ms => from s in ms 
                             from r in ma.Map(a => (s.Fails, s.Succs.Add(a)))
                                         .Catch((E e) => M.Pure((s.Fails.Add(e), s.Succs)))
                             select r);
    
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
    public static K<M, (Seq<E> Fails, Seq<A> Succs)> partition<E, M, A>(
        Seq<K<M, A>> fma)
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
    public static K<M, (Seq<E> Fails, Seq<A> Succs)> partition<E, M, A>(
        Iterable<K<M, A>> fma)
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
    public static K<M, (Seq<E> Fails, Seq<A> Succs)> partition<E, M, A>(
        Lst<K<M, A>> fma)
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
    public static K<M, (Seq<E> Fails, Seq<A> Succs)> partition<E, M, A>(
        IEnumerable<K<M, A>> fma)
        where M : Monad<M>, Fallible<E, M> =>
        LanguageExt.Iterable.createRange(fma).Partition<E, Iterable, M, A>();
    
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
    public static K<M, (Seq<E> Fails, Seq<A> Succs)> partition<E, M, A>(
        HashSet<K<M, A>> fma)
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
    public static K<M, (Seq<E> Fails, Seq<A> Succs)> partition<E, M, A>(
        Set<K<M, A>> fma)
        where M : Monad<M>, Fallible<E, M> =>
        fma.Kind().Partition<E, Set, M, A>();    
}
