using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// EitherT monad-transformer extensions
/// </summary>
public static partial class EitherTExtensions
{
    public static EitherT<L, M, A> As<L, M, A>(this K<EitherT<L, M>, A> ma)
        where M : Monad<M> =>
        (EitherT<L, M, A>)ma;

    public static FinT<M, A> ToFin<M, A>(this K<EitherT<Error, M>, A> ma) 
        where M : Monad<M> =>
        new(ma.As().runEither.Map(ma => ma.ToFin()));

    /// <summary>
    /// Runs the EitherT exposing the outer monad with an inner wrapped `Either`
    /// </summary>
    public static K<M, Either<L, A>> Run<L, M, A>(this K<EitherT<L, M>, A> ma)
        where M : Monad<M> =>
        ma.As().runEither;

    /// <summary>
    /// Get the outer task and wrap it up in a new IO within the EitherT IO
    /// </summary>
    public static EitherT<L, IO, A> Flatten<L, A>(this Task<EitherT<L, IO, A>> tma) =>
        EitherT<L, IO, EitherT<L, IO, A>>
           .Lift(IO.liftAsync(async () => await tma.ConfigureAwait(false)))
           .Flatten();

    /// <summary>
    /// Lift the task
    /// </summary>
    public static EitherT<L, IO, A> ToIO<L, A>(this Task<Either<L, A>> ma) =>
        liftIO(ma);
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static EitherT<L, M, A> Flatten<L, M, A>(this EitherT<L, M, EitherT<L, M, A>> mma)
        where M : Monad<M> =>
        mma.Bind(identity);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    [Pure]
    public static EitherT<L, M, C> SelectMany<L, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, K<EitherT<L, M>, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M> =>
        EitherT<L, M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    [Pure]
    public static EitherT<L, M, C> SelectMany<L, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, EitherT<L, M, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M> =>
        EitherT<L, M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Extracts from a list of 'Either' all the 'Left' elements.
    /// All the 'Left' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static K<M, Seq<L>> Lefts<L, M, R>(this IEnumerable<EitherT<L, M, R>> self)
        where M : Monad<M>
    {
        var result = M.Pure(Seq<L>.Empty);

        foreach (var either in self)
        {
            result = result.Bind(
                acc => either.Run().Map(
                    e => e switch
                         {
                             Either.Right<L, R>        => acc,
                             Either.Left<L, R> (var l) => acc.Add(l),
                             _                         => throw new NotSupportedException()
                         }));
        }
        return result;
    }

    /// <summary>
    /// Extracts from a list of 'Either' all the 'Left' elements.
    /// All the 'Left' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static K<M, Seq<L>> Lefts<L, M, R>(this Seq<EitherT<L, M, R>> self)
        where M : Monad<M>
    {
        var result = M.Pure(Seq<L>.Empty);

        foreach (var either in self)
        {
            result = result.Bind(
                acc => either.Run().Map(
                    e => e switch
                         {
                             Either.Right<L, R>        => acc,
                             Either.Left<L, R> (var l) => acc.Add(l),
                             _                         => throw new NotSupportedException()
                         }));
        }
        return result;
    }
    
    /// <summary>
    /// Extracts from a list of 'Either' all the 'Right' elements.
    /// All the 'Right' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static K<M, Seq<R>> Rights<L, M, R>(this IEnumerable<EitherT<L, M, R>> self)
        where M : Monad<M>
    {
        var result = M.Pure(Seq<R>.Empty);

        foreach (var either in self)
        {
            result = result.Bind(
                acc => either.Run().Map(
                    e => e switch
                         {
                             Either.Right<L, R> (var r) => acc.Add(r),
                             Either.Left<L, R>          => acc,
                             _                          => throw new NotSupportedException()
                         }));
        }
        return result;
    }

    /// <summary>
    /// Extracts from a list of 'Either' all the 'Right' elements.
    /// All the 'Right' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static K<M, Seq<R>> Rights<L, M, R>(this Seq<EitherT<L, M, R>> self)
        where M : Monad<M>
    {
        var result = M.Pure(Seq<R>.Empty);

        foreach (var either in self)
        {
            result = result.Bind(
                acc => either.Run().Map(
                    e => e switch
                         {
                             Either.Right<L, R> (var r) => acc.Add(r),
                             Either.Left<L, R>          => acc,
                             _                          => throw new NotSupportedException()
                         }));
        }
        return result;
    }

    /// <summary>
    /// Partitions a list of 'Either' into two lists.
    /// All the 'Left' elements are extracted, in order, to the first
    /// component of the output.  Similarly, the 'Right' elements are extracted
    /// to the second component of the output.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>A tuple containing the enumerable of L and enumerable of R</returns>
    [Pure]
    public static K<M, (Seq<L> Lefts, Seq<R> Rights)> Partition<L, M, R>(this Seq<EitherT<L, M, R>> self)
        where M : Monad<M>
    {
        var result = M.Pure((Left: Seq<L>.Empty, Right: Seq<R>.Empty));
        
        foreach (var either in self)
        {
            result = result.Bind(
                acc => either.Run().Map(
                    e => e switch
                         {
                             Either.Right<L, R> (var r) => (acc.Left, acc.Right.Add(r)),
                             Either.Left<L, R> (var l)  => (acc.Left.Add(l), acc.Right),
                             _                          => throw new NotSupportedException()
                         }));
        }
        return result;
    }

    /// <summary>
    /// Partitions a list of 'Either' into two lists.
    /// All the 'Left' elements are extracted, in order, to the first
    /// component of the output.  Similarly, the 'Right' elements are extracted
    /// to the second component of the output.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>A tuple containing the enumerable of L and enumerable of R</returns>
    [Pure]
    public static K<M, (Seq<L> Lefts, Seq<R> Rights)> Partition<L, M, R>(this IEnumerable<EitherT<L, M, R>> self)
        where M : Monad<M>
    {
        var result = M.Pure((Left: Seq<L>.Empty, Right: Seq<R>.Empty));

        foreach (var either in self)
        {
            result = result.Bind(
                acc => either.Run().Map(
                    e => e switch
                         {
                             Either.Right<L, R> (var r) => (acc.Left, acc.Right.Add(r)),
                             Either.Left<L, R> (var l)  => (acc.Left.Add(l), acc.Right),
                             _                          => throw new NotSupportedException()
                         }));
        }

        return result;
    }
}
