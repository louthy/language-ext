using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// StreamT extensions
/// </summary>
public static partial class StreamTExtensions
{
    public static StreamT<M, A> As<M, A>(this K<StreamT<M>, A> ma)
        where M : Monad<M> =>
        (StreamT<M, A>)ma;

    public static MList<A> As<M, A>(this K<MList, A> ma)
        where M : Monad<M> =>
        (MList<A>)ma;

    public static K<M, Option<(A Head, StreamT<M, A> Tail)>> Run<M, A>(this K<StreamT<M>, A> mma)
        where M : Monad<M> =>
        mma.As().Run();

    /// <summary>
    /// Execute the stream's inner monad `M`, combining the results using
    /// its `MonoidK<M>.Combine` operator.
    /// </summary>
    /// <param name="mma">Stream to combine</param>
    /// <returns>Result of the combined effects</returns>
    public static K<M, A> Combine<M, A>(this K<StreamT<M>, A> mma) 
        where M : Monad<M>, MonoidK<M> =>
        mma.As().runListT().Combine();

    /// <summary>
    /// Execute the stream's inner monad `M`, combining the results using
    /// its `MonoidK<M>.Combine` operator.
    /// </summary>
    /// <param name="mma">Stream to combine</param>
    /// <returns>Result of the combined effects</returns>
    static K<M, A> Combine<M, A>(this K<M, MList<A>> mma)
        where M : Monad<M>, MonoidK<M> =>
        mma.Bind(ml => ml switch
                       {
                           MNil<A> =>
                               M.Empty<A>(),

                           MCons<M, A>(var head, var tail) =>
                               M.Combine(M.Pure(head), tail().Combine()),

                           _ => throw new NotSupportedException()
                       });

    public static StreamT<M, A> Flatten<M, A>(this K<StreamT<M>, StreamT<M, A>> mma)
        where M : Monad<M> =>
        new (() => mma.As().runListT().Map(ml => ml.Map(ma => ma.runListT())).Flatten());

    public static StreamT<M, A> Flatten<M, A>(this K<StreamT<M>, K<StreamT<M>, A>> mma)
        where M : Monad<M> =>
        new (() => mma.As().runListT().Map(ml => ml.Map(ma => ma.As().runListT())).Flatten());

    public static K<M, MList<A>> Flatten<M, A>(this K<M, MList<K<M, MList<A>>>> mma)
        where M : Monad<M> =>
        mma.Bind(la => la.Flatten());

    public static K<M, MList<A>> Flatten<M, A>(this MList<K<M, MList<A>>> mma)
        where M : Monad<M> =>
        mma switch
        {
            MNil<K<M, MList<A>>>                     => M.Pure(MNil<A>.Default),
            MCons<M, K<M, MList<A>>> (var h, var t)  => h.Append(t().Flatten()),
            _                                        => throw new NotSupportedException()
        };

    public static K<M, MList<A>> Append<M, A>(this K<M, MList<A>> xs, K<M, MList<A>> ys)
        where M : Monad<M> =>
        xs.Bind(x => x.Append(ys));

    public static StreamT<M, B> Bind<M, A, B>(this Pure<A> ma, Func<A, StreamT<M, B>> f)
        where M : Monad<M> =>
        StreamT<M>.pure(ma.Value).Bind(f);

    public static StreamT<M, B> Bind<M, A, B>(this Pure<A> ma, Func<A, K<StreamT<M>, B>> f)
        where M : Monad<M> =>
        StreamT<M>.pure(ma.Value).Bind(f);

    public static StreamT<M, B> Bind<M, A, B>(this IO<A> ma, Func<A, StreamT<M, B>> f)
        where M : Monad<M> =>
        StreamT<M>.liftIO(ma).Bind(f);

    public static StreamT<M, B> Bind<M, A, B>(this IO<A> ma, Func<A, K<StreamT<M>, B>> f)
        where M : Monad<M> =>
        StreamT<M>.liftIO(ma).Bind(f);

    public static StreamT<M, C> SelectMany<M, A, B, C>(
        this Pure<A> ma,
        Func<A, StreamT<M, B>> bind,
        Func<A, B, C> project)
        where M : Monad<M> =>
        StreamT<M>.pure(ma.Value).SelectMany(bind, project);

    public static StreamT<M, C> SelectMany<M, A, B, C>(
        this Pure<A> ma,
        Func<A, K<StreamT<M>, B>> bind,
        Func<A, B, C> project)
        where M : Monad<M> =>
        StreamT<M>.pure(ma.Value).SelectMany(bind, project);

    public static StreamT<M, C> SelectMany<M, A, B, C>(
        this IO<A> ma,
        Func<A, StreamT<M, B>> bind,
        Func<A, B, C> project)
        where M : Monad<M> =>
        StreamT<M>.liftIO(ma).SelectMany(bind, project);

    public static StreamT<M, C> SelectMany<M, A, B, C>(
        this IO<A> ma,
        Func<A, K<StreamT<M>, B>> bind,
        Func<A, B, C> project)
        where M : Monad<M> =>
        StreamT<M>.liftIO(ma).SelectMany(bind, project);

    /// <summary>
    /// Iterate the stream, ignoring any result.
    /// </summary>
    public static K<M, Unit> Iter<M, A>(this K<StreamT<M>, A> ma) 
        where M : Monad<M> =>
        ma.As().Iter();

    /// <summary>
    /// Retrieve the head of the sequence
    /// </summary>
    public static K<M, Option<A>> Head<M, A>(this K<StreamT<M>, A> ma) 
        where M : Monad<M> =>
        ma.As().Head();

    /// <summary>
    /// Retrieve the head of the sequence
    /// </summary>
    /// <exception cref="ExpectedException">Throws sequence-empty expected-exception</exception>
    public static K<M, A> HeadUnsafe<M, A>(this K<StreamT<M>, A> ma) 
        where M : Monad<M> =>
        ma.As().HeadUnsafe();

    /// <summary>
    /// Retrieve the head of the sequence
    /// </summary>
    public static StreamT<M, A> Tail<M, A>(this K<StreamT<M>, A> ma) 
        where M : Monad<M> =>
        ma.As().Tail();

    /// <summary>
    /// Fold the stream itself, yielding the latest state value when the fold function returns `None`
    /// </summary>
    /// <param name="state">Initial state of the fold</param>
    /// <param name="f">Fold operation</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, S> Fold<M, A,S>(this K<StreamT<M>, A> ma, S state, Func<S, A, Option<S>> f) 
        where M : Monad<M> =>
        ma.As().Fold(state, f);

    /// <summary>
    /// Fold the stream itself, yielding values when the `until` predicate is `true`
    /// </summary>
    /// <param name="state">Initial state of the fold</param>
    /// <param name="f">Fold operation</param>
    /// <param name="until">Predicate</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, S> FoldUntil<M, A, S>(
        this K<StreamT<M>, A> ma, 
        S state, 
        Func<S, A, S> f, 
        Func<S, A, bool> until) 
        where M : Monad<M> =>
        ma.As().FoldUntil(state, f, until);

    /// <summary>
    /// Fold the stream itself, yielding values when the `until` predicate is `true`
    /// </summary>
    /// <param name="state">Initial state of the fold</param>
    /// <param name="f">Fold operation</param>
    /// <param name="until">Predicate</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, S> FoldWhile<M, A,S>(
        this K<StreamT<M>, A> ma,
        S state, 
        Func<S, A, S> f, 
        Func<S, A, bool> @while) 
        where M : Monad<M> =>
        ma.As().FoldWhile(state, f, @while);

    /// <summary>
    /// Left fold
    /// </summary>
    /// <param name="state">Initial state</param>
    /// <param name="f">Folding function</param>
    /// <typeparam name="S">State type</typeparam>
    /// <returns>Accumulate state wrapped in the StreamT inner monad</returns>
    public static K<M, S> FoldM<M, A, S>(
        this K<StreamT<M>, A> ma,
        S state, 
        Func<S, A, K<M, S>> f)
        where M : Monad<M> =>
        ma.As().FoldM(state, f);

    /// <summary>
    /// Concatenate streams
    /// </summary>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> Combine<M, A>(
        this K<StreamT<M>, A> first,
        StreamT<M, A> second) 
        where M : Monad<M> =>
        first.As().Combine(second);

    /// <summary>
    /// Concatenate streams
    /// </summary>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> Combine<M, A>(
        this K<StreamT<M>, A> first,
        K<StreamT<M>, A> second)
        where M : Monad<M> =>
        first.As().Combine(second);

    /// <summary>
    /// Interleave the items of many streams
    /// </summary>
    /// <param name="first">First stream to merge with</param>
    /// <param name="second">Second stream to merge with</param>
    /// <param name="rest">N streams to merge</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> Merge<M, A>(
        this K<StreamT<M>, A> first,
        K<StreamT<M>, A> second)
        where M : Monad<M> =>
        first.As().Merge(second);

    /// <summary>
    /// Interleave the items of two streams
    /// </summary>
    /// <param name="first">First stream to merge with</param>
    /// <param name="second">Second stream to merge with</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, A> Merge<M, A>(
        this K<StreamT<M>, A> first,
        K<StreamT<M>, A> second, 
        params K<StreamT<M>, A>[] rest)
        where M : Monad<M> =>
        first.As().Merge(second, rest);
 
    /// <summary>
    /// Merge the items of two streams into pairs
    /// </summary>
    /// <param name="first">First stream to merge with</param>
    /// <param name="second">Other stream to merge with</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, (A First, B Second)> Zip<M, A, B>(
        this K<StreamT<M>, A> first,
        K<StreamT<M>, B> second)
        where M : Monad<M> =>
        first.As().Zip(second);

    /// <summary>
    /// Merge the items of two streams into 3-tuples
    /// </summary>
    /// <param name="first">First stream to merge with</param>
    /// <param name="second">Second stream to merge with</param>
    /// <param name="third">Third stream to merge with</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, (A First, B Second, C Third)> Zip<M, A, B, C>(
        this K<StreamT<M>, A> first,
        K<StreamT<M>, B> second,
        K<StreamT<M>, C> third) 
        where M : Monad<M> =>
        first.As().Zip(second, third);

    /// <summary>
    /// Merge the items of two streams into 4-tuples
    /// </summary>
    /// <param name="first">First stream to merge with</param>
    /// <param name="second">Second stream to merge with</param>
    /// <param name="third">Third stream to merge with</param>
    /// <param name="fourth">Fourth stream to merge with</param>
    /// <returns>Stream transformer</returns>
    public static StreamT<M, (A First, B Second, C Third, D Fourth)> Zip<M, A, B, C, D>(
        this K<StreamT<M>, A> first,
        K<StreamT<M>, B> second,
        K<StreamT<M>, C> third,
        K<StreamT<M>, D> fourth)
        where M : Monad<M> =>
        first.As().Zip(second, third, fourth);

    /// <summary>
    /// Access the `Some` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of optional values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> Somes<M, A>(this IAsyncEnumerable<OptionT<M, A>> stream)
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, A>>.Lift(stream.Map(mx => mx.ToStream()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Some` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of optional values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> SomesStream<M, A>(this IAsyncEnumerable<Option<A>> stream)
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, A>>.Lift(stream.Map(mx => mx.ToStream<M>()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Some` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of optional values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> Somes<M, A>(this IEnumerable<OptionT<M, A>> stream)
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, A>>.Lift(stream.Select(mx => mx.ToStream()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Some` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of optional values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> SomesStream<M, A>(this IEnumerable<Option<A>> stream)
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, A>>.Lift(stream.Select(mx => mx.ToStream<M>()))
        from x in xs
        select x;    

    /// <summary>
    /// Access the `Right` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> Rights<M, L, A>(this IAsyncEnumerable<EitherT<L, M, A>> stream)
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, A>>.Lift(stream.Map(mx => mx.ToStream()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Right` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> RightsStream<M, L, A>(this IAsyncEnumerable<Either<L, A>> stream)
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, A>>.Lift(stream.Map(mx => mx.ToStream<M>()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Right` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> Rights<M, L, A>(this IEnumerable<EitherT<L, M, A>> stream)
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, A>>.Lift(stream.Select(mx => mx.ToStream()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Right` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> RightsStream<M, L, A>(this IEnumerable<Either<L, A>> stream)
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, A>>.Lift(stream.Select(mx => mx.ToStream<M>()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Left` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, L> Lefts<M, L, A>(this IAsyncEnumerable<EitherT<L, M, A>> stream)
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, L>>.Lift(stream.Map(mx => mx.LeftToStream()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Left` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, L> LeftsStream<M, L, A>(this IAsyncEnumerable<Either<L, A>> stream)
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, L>>.Lift(stream.Map(mx => mx.LeftToStream<M>()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Left` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, L> Lefts<M, L, A>(this IEnumerable<EitherT<L, M, A>> stream)
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, L>>.Lift(stream.Select(mx => mx.LeftToStream()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Succ` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, L> LeftsStream<M, L, A>(this IEnumerable<Either<L, A>> stream)
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, L>>.Lift(stream.Select(mx => mx.LeftToStream<M>()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Succ` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> Succs<M, A>(this IAsyncEnumerable<FinT<M, A>> stream)
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, A>>.Lift(stream.Map(mx => mx.ToStream()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Succ` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> SuccsStream<M, A>(this IAsyncEnumerable<Fin<A>> stream)
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, A>>.Lift(stream.Map(mx => mx.ToStream<M>()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Succ` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> Succs<M, A>(this IEnumerable<FinT<M, A>> stream)
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, A>>.Lift(stream.Select(mx => mx.ToStream()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Succ` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> SuccsStream<M, A>(this IEnumerable<Fin<A>> stream)
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, A>>.Lift(stream.Select(mx => mx.ToStream<M>()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Fail` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, Error> Fails<M, A>(this IAsyncEnumerable<FinT<M, A>> stream)
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, Error>>.Lift(stream.Map(mx => mx.FailToStream()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Fail` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, Error> FailsStream<M, A>(this IAsyncEnumerable<Fin<A>> stream)
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, Error>>.Lift(stream.Map(mx => mx.FailToStream<M>()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Fail` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, Error> Fails<M, A>(this IEnumerable<FinT<M, A>> stream)
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, Error>>.Lift(stream.Select(mx => mx.FailToStream()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Fail` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, Error> FailsStream<M, A>(this IEnumerable<Fin<A>> stream)
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, Error>>.Lift(stream.Select(mx => mx.FailToStream<M>()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Right` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> Succs<M, L, A>(this IAsyncEnumerable<ValidationT<L, M, A>> stream)
        where L : Monoid<L> 
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, A>>.Lift(stream.Map(mx => mx.ToStream()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Right` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> SuccsStream<M, L, A>(this IAsyncEnumerable<Validation<L, A>> stream)
        where L : Monoid<L> 
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, A>>.Lift(stream.Map(mx => mx.ToStream<M>()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Right` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> Succs<M, L, A>(this IEnumerable<ValidationT<L, M, A>> stream)
        where L : Monoid<L> 
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, A>>.Lift(stream.Select(mx => mx.ToStream()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Right` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> SuccsStream<M, L, A>(this IEnumerable<Validation<L, A>> stream)
        where L : Monoid<L> 
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, A>>.Lift(stream.Select(mx => mx.ToStream<M>()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Left` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, L> Fails<M, L, A>(this IAsyncEnumerable<ValidationT<L, M, A>> stream)
        where L : Monoid<L> 
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, L>>.Lift(stream.Map(mx => mx.FailToStream()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Left` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, L> FailsStream<M, L, A>(this IAsyncEnumerable<Validation<L, A>> stream)
        where L : Monoid<L> 
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, L>>.Lift(stream.Map(mx => mx.FailToStream<M>()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Left` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, L> Fails<M, L, A>(this IEnumerable<ValidationT<L, M, A>> stream)
        where L : Monoid<L> 
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, L>>.Lift(stream.Select(mx => mx.FailToStream()))
        from x in xs
        select x;

    /// <summary>
    /// Access the `Succ` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, L> FailsStream<M, L, A>(this IEnumerable<Validation<L, A>> stream)
        where L : Monoid<L> 
        where M : Monad<M> =>
        from xs in StreamT<M, StreamT<M, L>>.Lift(stream.Select(mx => mx.FailToStream<M>()))
        from x in xs
        select x;
    
}
