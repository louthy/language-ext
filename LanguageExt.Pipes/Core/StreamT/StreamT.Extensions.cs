using System;
using LanguageExt.Common;
using LanguageExt.Traits;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.Pipes;

public static class StreamTExtensions
{
    /// <summary>
    /// Run the stream
    /// </summary>
    /// <remarks>
    /// The `M` trait must support `LiftIO`.
    /// </remarks>
    /// <param name="stream">Stream to run</param>
    /// <typeparam name="M">`Monad` and `Alternative` trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Operation that represents an `Applicative.Actions` on every item in the stream.</returns>
    [Pure]
    public static K<M, Option<A>> Run<M, A>(this K<StreamT<M>, A> stream)
        where M : Monad<M>, Alternative<M> =>
        Reduce(
            stream,
            Option<A>.None,
            (ms, ma) =>
                ValueTask.FromResult(
                    Reduced.Continue(
                        ms.Bind(_ => ma.Map(Option<A>.Some)))));

    /// <summary>
    /// Run the stream
    /// </summary>
    /// <remarks>
    /// The `M` trait must support `LiftIO`.
    /// </remarks>
    /// <param name="stream">Stream to run</param>
    /// <typeparam name="M">`Monad` and `Alternative` trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Operation that represents an `Applicative.Actions` on every item in the stream.</returns>
    [Pure]
    public static K<M, A> RunUnsafe<M, A>(this K<StreamT<M>, A> stream)
        where M : Monad<M>, Alternative<M> =>
        stream.Run().Map(mx => mx.IfNone(() => Errors.SequenceEmpty.Throw<A>()));

    /// <summary>
    /// Run the stream
    /// </summary>
    /// <remarks>
    /// The `M` trait must support `LiftIO`.
    /// </remarks>
    /// <param name="stream">Stream to run</param>
    /// <typeparam name="M">`Monad` and `Alternative` trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Operation that represents an `Applicative.Actions` on every item in the stream.</returns>
    [Pure]
    public static K<M, Unit> Iter<M, A>(this K<StreamT<M>, A> stream)
        where M : Monad<M>, Alternative<M> =>
        stream.Run().Map(Prelude.unit);

    /// <summary>
    /// Run the stream
    /// </summary>
    /// <remarks>
    /// The `M` trait must support `LiftIO`.
    /// </remarks>
    /// <param name="stream">Stream to run</param>
    /// <typeparam name="M">`Monad` and `Alternative` trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Operation that represents an `Applicative.Actions` on every item in the stream.</returns>
    [Pure]
    static StreamT<M, A> Actions<M, A>(this K<StreamT<M>, A> stream)
        where M : Monad<M>, Alternative<M> =>
        from ox in StreamT.liftM(Run(stream))
        from rs in ox.IsSome ? StreamT.pure<M, A>((A)ox) : StreamT.empty<M, A>()
        select rs;
    
    public static K<M, S> Reduce<M, A, S>(this K<StreamT<M>, A> stream, S state, Reducer<K<M, A>, K<M, S>> reducer)
        where M : Monad<M>, Alternative<M> =>
        M.LiftIO(stream.As().runStreamT.Reduce(M.Pure(state), reducer)).Flatten();
    
    internal static K<M, S> Reduce1<M, A, S>(this K<StreamT<M>, A> stream, S state, Func<K<M, S>, K<M, A>, K<M, S>> reducer)
        where M : Monad<M>, Alternative<M> =>
        Reduce(stream, state, (s, a) => new ValueTask<Reduced<K<M, S>>>(Reduced.Continue(reducer(s, a))));
    
    internal static K<M, S> Reduce2<M, A, S>(this K<StreamT<M>, A> stream, S state, Func<K<M, S>, K<M, A>, Reduced<K<M, S>>> reducer)
        where M : Monad<M>, Alternative<M> =>
        Reduce(stream, state, (s, a) => new ValueTask<Reduced<K<M, S>>>(reducer(s, a)));

    public static StreamT<M, A> As<M, A>(this K<StreamT<M>, A> ma)
        where M : Monad<M>, Alternative<M> =>
        (StreamT<M, A>)ma;
    
    [Pure]
    public static StreamT<M, A> AsStream<M, A>(this IEnumerable<A> items)
        where M : Monad<M>, Alternative<M> =>
        StreamT.lift<M, A>(items);
    
    [Pure]
    public static StreamT<M, A> AsStream<M, A>(this IAsyncEnumerable<A> items)
        where M : Monad<M>, Alternative<M> =>
        StreamT.lift<M, A>(items);
    
    /// <summary>
    /// Access the `Some` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of optional values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> SomeStream<M, A>(this IAsyncEnumerable<OptionT<M, A>> stream)
        where M : Monad<M>, Alternative<M>  =>
        from xs in StreamT.lift<M, OptionT<M, A>>(stream)
        from ox in xs.Run()
        where ox.IsSome
        select (A)ox;

    /// <summary>
    /// Access the `Some` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of optional values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> SomeStream<M, A>(this IAsyncEnumerable<Option<A>> stream)
        where M : Monad<M>, Alternative<M>  =>
        from ox in StreamT.lift<M, Option<A>>(stream)
        where ox.IsSome
        select (A)ox;

    /// <summary>
    /// Access the `Some` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of optional values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> SomeStream<M, A>(this IEnumerable<OptionT<M, A>> stream)
        where M : Monad<M>, Alternative<M>  =>
        from xs in StreamT.lift<M, OptionT<M, A>>(stream)
        from ox in xs.Run()
        where ox.IsSome
        select (A)ox;

    /// <summary>
    /// Access the `Some` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of optional values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> SomeStream<M, A>(this IEnumerable<Option<A>> stream)
        where M : Monad<M>, Alternative<M>  =>
        from ox in StreamT.lift<M, Option<A>>(stream)
        where ox.IsSome
        select (A)ox;

    /// <summary>
    /// Access the `Right` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> RightStream<M, L, A>(this IAsyncEnumerable<EitherT<L, M, A>> stream)
        where M : Monad<M>, Alternative<M>  =>
        from xs in StreamT.lift<M, EitherT<L, M, A>>(stream)
        from ox in xs.Run()
        where ox.IsRight
        select (A)ox;

    /// <summary>
    /// Access the `Right` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> RightStream<M, L, A>(this IAsyncEnumerable<Either<L, A>> stream)
        where M : Monad<M>, Alternative<M>  =>
        from ox in StreamT.lift<M, Either<L, A>>(stream)
        where ox.IsRight
        select (A)ox;

    /// <summary>
    /// Access the `Right` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> RightStream<M, L, A>(this IEnumerable<EitherT<L, M, A>> stream)
        where M : Monad<M>, Alternative<M>  =>
        from xs in StreamT.lift<M, EitherT<L, M, A>>(stream)
        from ox in xs.Run()
        where ox.IsRight
        select (A)ox;

    /// <summary>
    /// Access the `Right` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> RightStream<M, L, A>(this IEnumerable<Either<L, A>> stream)
        where M : Monad<M>, Alternative<M>  =>
        from ox in StreamT.lift<M, Either<L, A>>(stream)
        where ox.IsRight
        select (A)ox;

    /// <summary>
    /// Access the `Left` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, L> LeftStream<M, L, A>(this IAsyncEnumerable<EitherT<L, M, A>> stream)
        where M : Monad<M>, Alternative<M>  =>
        from xs in StreamT.lift<M, EitherT<L, M, A>>(stream)
        from ox in xs.Run()
        where ox.IsLeft
        select (L)ox;

    /// <summary>
    /// Access the `Left` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, L> LeftStream<M, L, A>(this IAsyncEnumerable<Either<L, A>> stream)
        where M : Monad<M>, Alternative<M>  =>
        from ox in StreamT.lift<M, Either<L, A>>(stream)
        where ox.IsLeft
        select (L)ox;

    /// <summary>
    /// Access the `Left` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, L> LeftStream<M, L, A>(this IEnumerable<EitherT<L, M, A>> stream)
        where M : Monad<M>, Alternative<M>  =>
        from xs in StreamT.lift<M, EitherT<L, M, A>>(stream)
        from ox in xs.Run()
        where ox.IsLeft
        select (L)ox;

    /// <summary>
    /// Access the `Succ` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, L> LeftStream<M, L, A>(this IEnumerable<Either<L, A>> stream)
        where M : Monad<M>, Alternative<M>  =>
        from ox in StreamT.lift<M, Either<L, A>>(stream)
        where ox.IsLeft
        select (L)ox;

    /// <summary>
    /// Access the `Succ` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> SuccStream<M, A>(this IAsyncEnumerable<FinT<M, A>> stream)
        where M : Monad<M>, Alternative<M>  =>
        from xs in StreamT.lift<M, FinT<M, A>>(stream)
        from ox in xs.Run()
        where ox.IsSucc
        select (A)ox;

    /// <summary>
    /// Access the `Succ` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> SuccStream<M, A>(this IAsyncEnumerable<Fin<A>> stream)
        where M : Monad<M>, Alternative<M>  =>
        from ox in StreamT.lift<M, Fin<A>>(stream)
        where ox.IsSucc
        select (A)ox;

    /// <summary>
    /// Access the `Succ` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> SuccStream<M, A>(this IEnumerable<FinT<M, A>> stream)
        where M : Monad<M>, Alternative<M>  =>
        from xs in StreamT.lift<M, FinT<M, A>>(stream)
        from ox in xs.Run()
        where ox.IsSucc
        select (A)ox;

    /// <summary>
    /// Access the `Succ` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> SuccStream<M, A>(this IEnumerable<Fin<A>> stream)
        where M : Monad<M>, Alternative<M>  =>
        from ox in StreamT.lift<M, Fin<A>>(stream)
        where ox.IsSucc
        select (A)ox;

    /// <summary>
    /// Access the `Fail` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, Error> FailStream<M, A>(this IAsyncEnumerable<FinT<M, A>> stream)
        where M : Monad<M>, Alternative<M>  =>
        from xs in StreamT.lift<M, FinT<M, A>>(stream)
        from ox in xs.Run()
        where ox.IsFail
        select (Error)ox;

    /// <summary>
    /// Access the `Fail` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, Error> FailStream<M, A>(this IAsyncEnumerable<Fin<A>> stream)
        where M : Monad<M>, Alternative<M>  =>
        from ox in StreamT.lift<M, Fin<A>>(stream)
        where ox.IsFail
        select (Error)ox;

    /// <summary>
    /// Access the `Fail` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, Error> FailStream<M, A>(this IEnumerable<FinT<M, A>> stream)
        where M : Monad<M>, Alternative<M>  =>
        from xs in StreamT.lift<M, FinT<M, A>>(stream)
        from ox in xs.Run()
        where ox.IsFail
        select (Error)ox;

    /// <summary>
    /// Access the `Fail` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, Error> FailStream<M, A>(this IEnumerable<Fin<A>> stream)
        where M : Monad<M>, Alternative<M>  =>
        from ox in StreamT.lift<M, Fin<A>>(stream)
        where ox.IsFail
        select (Error)ox;

    /// <summary>
    /// Access the `Succ` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> SuccStream<M, L, A>(this IAsyncEnumerable<ValidationT<L, M, A>> stream)
        where L : Monoid<L> 
        where M : Monad<M>, Alternative<M>  =>
        from xs in StreamT.lift<M, ValidationT<L, M, A>>(stream)
        from ox in xs.Run()
        where ox.IsSuccess
        select (A)ox;

    /// <summary>
    /// Access the `Succ` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> SuccStream<M, L, A>(this IAsyncEnumerable<Validation<L, A>> stream)
        where L : Monoid<L> 
        where M : Monad<M>, Alternative<M>  =>
        from ox in StreamT.lift<M, Validation<L, A>>(stream)
        where ox.IsSuccess
        select (A)ox;

    /// <summary>
    /// Access the `Succ` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> SuccStream<M, L, A>(this IEnumerable<ValidationT<L, M, A>> stream)
        where L : Monoid<L> 
        where M : Monad<M>, Alternative<M>  =>
        from xs in StreamT.lift<M, ValidationT<L, M, A>>(stream)
        from ox in xs.Run()
        where ox.IsSuccess
        select (A)ox;

    /// <summary>
    /// Access the `Succ` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, A> SuccStream<M, L, A>(this IEnumerable<Validation<L, A>> stream)
        where L : Monoid<L> 
        where M : Monad<M>, Alternative<M>  =>
        from ox in StreamT.lift<M, Validation<L, A>>(stream)
        where ox.IsSuccess
        select (A)ox;

    /// <summary>
    /// Access the `Fail` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, L> FailStream<M, L, A>(this IAsyncEnumerable<ValidationT<L, M, A>> stream)
        where L : Monoid<L> 
        where M : Monad<M>, Alternative<M>  =>
        from xs in StreamT.lift<M, ValidationT<L, M, A>>(stream)
        from ox in xs.Run()
        where ox.IsFail
        select (L)ox;

    /// <summary>
    /// Access the `Fail` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, L> FailsStream<M, L, A>(this IAsyncEnumerable<Validation<L, A>> stream)
        where L : Monoid<L> 
        where M : Monad<M>, Alternative<M>  =>
        from ox in StreamT.lift<M, Validation<L, A>>(stream)
        where ox.IsFail
        select (L)ox;

    /// <summary>
    /// Access the `Fail` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, L> FailStream<M, L, A>(this IEnumerable<ValidationT<L, M, A>> stream)
        where L : Monoid<L> 
        where M : Monad<M>, Alternative<M>  =>
        from xs in StreamT.lift<M, ValidationT<L, M, A>>(stream)
        from ox in xs.Run()
        where ox.IsFail
        select (L)ox;

    /// <summary>
    /// Access the `Fail` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    public static StreamT<M, L> FailStream<M, L, A>(this IEnumerable<Validation<L, A>> stream)
        where L : Monoid<L> 
        where M : Monad<M>, Alternative<M>  =>
        from ox in StreamT.lift<M, Validation<L, A>>(stream)
        where ox.IsFail
        select (L)ox;

    public static StreamT<M, B> Bind<M, A, B>(this IO<A> ma, Func<A, StreamT<M, B>> f)
        where M : Monad<M>, Alternative<M> =>
        StreamT.liftIO<M, A>(ma).Bind(f);

    public static StreamT<M, B> Bind<M, A, B>(this Pure<A> ma, Func<A, StreamT<M, B>> f)
        where M : Monad<M>, Alternative<M> =>
        StreamT.pure<M, A>(ma.Value).Bind(f);

    public static StreamT<M, B> Bind<M, A, B>(this K<M, A> ma, Func<A, StreamT<M, B>> f)
        where M : Monad<M>, Alternative<M> =>
        StreamT.liftM(ma).Bind(f);    
    
    public static StreamT<M, C> SelectMany<M, A, B, C>(this IO<A> ma, Func<A, StreamT<M, B>> bind, Func<A, B, C> project)
        where M : Monad<M>, Alternative<M> =>
        StreamT.liftIO<M, A>(ma).SelectMany(bind, project);

    public static StreamT<M, C> SelectMany<M, A, B, C>(this Pure<A> ma, Func<A, StreamT<M, B>> bind, Func<A, B, C> project)
        where M : Monad<M>, Alternative<M> =>
        StreamT.pure<M, A>(ma.Value).SelectMany(bind, project);

    public static StreamT<M, C> SelectMany<M, A, B, C>(this K<M, A> ma, Func<A, StreamT<M, B>> bind, Func<A, B, C> project)
        where M : Monad<M>, Alternative<M> =>
        StreamT.liftM(ma).SelectMany(bind, project);
}
