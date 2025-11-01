﻿using System;
using static LanguageExt.Prelude;
using NSE = System.NotSupportedException;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class FinTExtensions
{
    public static FinT<M, A> As<M, A>(this K<FinT<M>, A> ma)
        where M : Monad<M> =>
        (FinT<M, A>)ma;

    /// <summary>
    /// Runs the FinT exposing the outer monad with an inner wrapped `Fin`
    /// </summary>
    public static K<M, Fin<A>> Run<M, A>(this K<FinT<M>, A> ma)
        where M : Monad<M> =>
        ma.As().runFin;
    
    /// <summary>
    /// Get the outer task and wrap it up in a new IO within the FinT IO
    /// </summary>
    public static FinT<IO, A> Flatten<A>(this Task<FinT<IO, A>> tma) =>
        FinT<IO, FinT<IO, A>>
           .Lift(IO.liftAsync(async () => await tma.ConfigureAwait(false)))
           .Flatten();

    /// <summary>
    /// Lift the task
    /// </summary>
    public static FinT<IO, A> ToIO<M, A>(this Task<Fin<A>> ma) 
        where M : Monad<M> =>
        liftIO(ma);
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static FinT<M, A> Flatten<M, A>(this FinT<M, FinT<M, A>> mma)
        where M : Monad<M> =>
        mma.Bind(identity);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`FinT`</returns>
    [Pure]
    public static FinT<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, 
        Func<A, K<FinT<M>, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M> =>
        FinT<M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`FinT`</returns>
    [Pure]
    public static FinT<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, 
        Func<A, FinT<M, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M> =>
        FinT<M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Partitions a foldable of `FinT` into two sequences.
    /// 
    /// All the `Fail` elements are extracted, in order, to the first component of the output.
    /// Similarly, the `Succ` elements are extracted to the second component of the output.
    /// </summary>
    /// <returns>A pair containing the sequences of partitioned values</returns>
    [Pure]
    public static K<M, (Seq<Error> Fails, Seq<A> Succs)> Partition<F, M, A>(this K<F, FinT<M, A>> self)
        where F : Foldable<F>
        where M : Monad<M> =>
        self.Fold(M.Pure((Fail: Seq<Error>.Empty, Succ: Seq<A>.Empty)),
                  (ms, ma) =>
                      ms.Bind(s => ma.Run().Map(a => a switch
                                                     {
                                                         Fin.Succ<A> (var r) => (s.Fail, s.Succ.Add(r)),
                                                         Fin.Fail<A> (var l) => (s.Fail.Add(l), s.Succ),
                                                         _                   => throw new NSE()
                                                     })));

    /// <summary>
    /// Partitions a foldable of `FinT` into two lists and returns the `Fail` items only.
    /// </summary>
    /// <returns>A sequence of partitioned items</returns>
    [Pure]
    public static K<M, Seq<Error>> Fails<F, M, A>(this K<F, FinT<M, A>> self)
        where F : Foldable<F>
        where M : Monad<M> =>
        self.Fold(M.Pure(Seq<Error>.Empty),
                  (ms, ma) =>
                      ms.Bind(s => ma.Run().Map(a => a switch
                                                     {
                                                         Fin.Fail<A> (var l) => s.Add(l),
                                                         _                   => throw new NSE()
                                                     })));

    /// <summary>
    /// Partitions a foldable of `FinT` into two lists and returns the `Succ` items only.
    /// </summary>
    /// <returns>A sequence of partitioned items</returns>
    [Pure]
    public static K<M, Seq<A>> Succs<F, M, A>(this K<F, FinT<M, A>> self)
        where F : Foldable<F>
        where M : Monad<M> =>
        self.Fold(M.Pure(Seq<A>.Empty),
                  (ms, ma) =>
                      ms.Bind(s => ma.Run().Map(a => a switch
                                                     {
                                                         Fin.Succ<A> (var r) => s.Add(r),
                                                         _                   => throw new NSE()
                                                     })));
}
