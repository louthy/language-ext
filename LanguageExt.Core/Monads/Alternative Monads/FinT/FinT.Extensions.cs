using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Either monad extensions
/// </summary>
public static class FinTExt
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
    /// Get the outer task and wrap it up in a new IO within the EitherT IO
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
    /// <returns>`EitherT`</returns>
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
    /// <returns>`EitherT`</returns>
    [Pure]
    public static FinT<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, 
        Func<A, FinT<M, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M> =>
        FinT<M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Applicative apply
    /// </summary>
    [Pure]
    public static FinT<M, B> Apply<M, A, B>(this FinT<M, Func<A, B>> mf, FinT<M, A> ma) 
        where M : Monad<M> => 
        mf.As().Bind(ma.As().Map);

    /// <summary>
    /// Applicative action
    /// </summary>
    [Pure]
    public static FinT<M, B> Action<M, A, B>(this FinT<M, A> ma, FinT<M, B> mb)
        where M : Monad<M> => 
        ma.As().Bind(_ => mb);

    /// <summary>
    /// Extracts from a sequence of 'Fin' transformers all the 'Fail' values.
    /// The 'Fail' elements are extracted in order.
    /// </summary>
    /// <typeparam name="A">Success value type</typeparam>
    /// <param name="self">Sequence of Fin transformers</param>
    /// <returns>A sequence of errors</returns>
    [Pure]
    public static K<M, Seq<Error>> Fails<M, A>(this IEnumerable<FinT<M, A>> self)
        where M : Monad<M>
    {
        var result = M.Pure(Seq<Error>.Empty);

        foreach (var either in self)
        {
            result = result.Bind(
                acc => either.Run().Map(
                    e => e switch
                         {
                             Fin.Succ<A>         => acc,
                             Fin.Fail<A> (var l) => acc.Add(l),
                             _                   => throw new NotSupportedException()
                         }));
        }
        return result;
    }

    /// <summary>
    /// Extracts from a sequence of 'Fin' transformers all the 'Fail' values.
    /// The 'Fail' elements are extracted in order.
    /// </summary>
    /// <typeparam name="A">Success value type</typeparam>
    /// <param name="self">Sequence of Fin transformers</param>
    /// <returns>A sequence of errors</returns>
    [Pure]
    public static K<M, Seq<Error>> Fails<M, R>(this Seq<FinT<M, R>> self)
        where M : Monad<M>
    {
        var result = M.Pure(Seq<Error>.Empty);

        foreach (var either in self)
        {
            result = result.Bind(
                acc => either.Run().Map(
                    e => e switch
                         {
                             Fin.Succ<R>         => acc,
                             Fin.Fail<R> (var l) => acc.Add(l),
                             _                   => throw new NotSupportedException()
                         }));
        }
        return result;
    }
    
    /// <summary>
    /// Extracts from a sequence of 'Fin' transformers all the 'Succ' values.
    /// The 'Succ' elements are extracted in order.
    /// </summary>
    /// <typeparam name="A">Success value type</typeparam>
    /// <param name="self">Sequence of Fin transformers</param>
    /// <returns>A sequence of success values</returns>
    [Pure]
    public static K<M, Seq<A>> Succs<M, A>(this IEnumerable<FinT<M, A>> self)
        where M : Monad<M>
    {
        var result = M.Pure(Seq<A>.Empty);

        foreach (var either in self)
        {
            result = result.Bind(
                acc => either.Run().Map(
                    e => e switch
                         {
                             Fin.Succ<A> (var r) => acc.Add(r),
                             Fin.Fail<A>         => acc,
                             _                   => throw new NotSupportedException()
                         }));
        }
        return result;
    }

    /// <summary>
    /// Extracts from a sequence of 'Fin' transformers all the 'Succ' values.
    /// The 'Succ' elements are extracted in order.
    /// </summary>
    /// <typeparam name="A">Success value type</typeparam>
    /// <param name="self">Sequence of Fin transformers</param>
    /// <returns>A sequence of success values</returns>
    [Pure]
    public static K<M, Seq<A>> Succs<M, A>(this Seq<FinT<M, A>> self)
        where M : Monad<M>
    {
        var result = M.Pure(Seq<A>.Empty);

        foreach (var either in self)
        {
            result = result.Bind(
                acc => either.Run().Map(
                    e => e switch
                         {
                             Fin.Succ<A> (var r) => acc.Add(r),
                             Fin.Fail<A>         => acc,
                             _                   => throw new NotSupportedException()
                         }));
        }
        return result;
    }

    /// <summary>
    /// Partitions a sequence of 'FinT' transformers into two sequences.
    /// All the 'Fail' elements are extracted, in order, to the first
    /// component of the output.  Similarly, the 'Succ' elements are extracted
    /// to the second component of the output.
    /// </summary>
    /// <typeparam name="A">Success type</typeparam>
    /// <param name="self">Sequence of Fin transformers</param>
    /// <returns>A tuple containing a sequence of `Fail` and a sequence of `Succ`</returns>
    [Pure]
    public static K<M, (Seq<Error> Lefts, Seq<A> Rights)> Partition<M, A>(this Seq<FinT<M, A>> self)
        where M : Monad<M>
    {
        var result = M.Pure((Left: Seq<Error>.Empty, Right: Seq<A>.Empty));
        
        foreach (var either in self)
        {
            result = result.Bind(
                acc => either.Run().Map(
                    e => e switch
                         {
                             Fin.Succ<A> (var r) => (acc.Left, acc.Right.Add(r)),
                             Fin.Fail<A> (var l) => (acc.Left.Add(l), acc.Right),
                             _                   => throw new NotSupportedException()
                         }));
        }
        return result;
    }

    /// <summary>
    /// Partitions a sequence of 'FinT' transformers into two sequences.
    /// All the 'Fail' elements are extracted, in order, to the first
    /// component of the output.  Similarly, the 'Succ' elements are extracted
    /// to the second component of the output.
    /// </summary>
    /// <typeparam name="A">Success type</typeparam>
    /// <param name="self">Sequence of Fin transformers</param>
    /// <returns>A tuple containing a sequence of `Fail` and a sequence of `Succ`</returns>
    [Pure]
    public static K<M, (Seq<Error> Lefts, Seq<A> Rights)> Partition<M, A>(this IEnumerable<FinT<M, A>> self)
        where M : Monad<M>
    {
        var result = M.Pure((Left: Seq<Error>.Empty, Right: Seq<A>.Empty));

        foreach (var either in self)
        {
            result = result.Bind(
                acc => either.Run().Map(
                    e => e switch
                         {
                             Fin.Succ<A> (var r) => (acc.Left, acc.Right.Add(r)),
                             Fin.Fail<A> (var l) => (acc.Left.Add(l), acc.Right),
                             _                   => throw new NotSupportedException()
                         }));
        }

        return result;
    }
}
