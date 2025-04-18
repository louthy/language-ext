using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Channels;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static class SourceTExtensions
{
    /// <summary>
    /// Downcast
    /// </summary>
    [Pure]
    public static SourceT<M, A> As<M, A>(this K<SourceT<M>, A> ma) 
        where M : MonadIO<M>, Alternative<M> =>
        (SourceT<M, A>)ma;
    
    [Pure]
    public static SourceT<M, A> AsSourceT<M, A>(this Channel<A> items)
        where M : MonadIO<M>, Alternative<M> =>
        SourceT.lift<M, A>(items);
    
    [Pure]
    public static SourceT<M, A> AsSourceT<M, A>(this Channel<K<M, A>> items)
        where M : MonadIO<M>, Alternative<M> =>
        SourceT.liftM(items);
    
    [Pure]
    public static SourceT<M, A> AsSourceT<M, A>(this IEnumerable<A> items)
        where M : MonadIO<M>, Alternative<M> =>
        SourceT.lift<M, A>(items);
    
    [Pure]
    public static SourceT<M, A> AsSourceT<M, A>(this IEnumerable<K<M, A>> items)
        where M : MonadIO<M>, Alternative<M> =>
        SourceT.liftM(items);
    
    [Pure]
    public static SourceT<M, A> AsSourceT<M, A>(this IAsyncEnumerable<A> items)
        where M : MonadIO<M>, Alternative<M> =>
        SourceT.lift<M, A>(items);
    
    [Pure]
    public static SourceT<M, A> AsSourceT<M, A>(this IAsyncEnumerable<K<M, A>> items)
        where M : MonadIO<M>, Alternative<M> =>
        SourceT.liftM(items);

    /// <summary>
    /// Force iteration of the stream, yielding a unit `M` structure.
    /// </summary>
    /// <remarks>
    /// The expectation is that the stream uses `IO` for side effects, so this makes them to happen.
    /// </remarks>
    [Pure]
    public static K<M, Unit> Iter<M, A>(this K<SourceT<M>, A> ma)
        where M : MonadIO<M>, Alternative<M> =>
        ma.As().Reduce(unit, (_, _) => M.Pure(unit));

    /// <summary>
    /// Force iteration of the stream, yielding the last structure processed
    /// </summary>
    [Pure]
    public static K<M, A> Last<M, A>(this K<SourceT<M>, A> ma)
        where M : MonadIO<M>, Alternative<M> =>
        ma.As()
          .Reduce(Option<A>.None, (_, x) => M.Pure(Some(x)))
          .Bind(ma => ma switch
                      {
                          { IsSome: true, Case: A value } => M.Pure(value),
                          _                               => M.Empty<A>()
                      });

    /// <summary>
    /// Force iteration of the stream and collect all the values into a `Seq`.
    /// </summary>
    [Pure]
    public static K<M, Seq<A>> Collect<M, A>(this K<SourceT<M>, A> ma)
        where M : MonadIO<M>, Alternative<M> =>
        ma.As().Reduce<Seq<A>>([], (xs, x) => M.Pure(xs.Add(x)));

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static SourceT<M, B> Bind<M, A, B>(this IO<A> ma, Func<A, SourceT<M, B>> f)
        where M : MonadIO<M>, Alternative<M> =>
        SourceT.liftIO<M, A>(ma).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static SourceT<M, B> Bind<M, A, B>(this Pure<A> ma, Func<A, SourceT<M, B>> f)
        where M : MonadIO<M>, Alternative<M> =>
        SourceT.pure<M, A>(ma.Value).Bind(f);

    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static SourceT<M, B> Bind<M, A, B>(this K<M, A> ma, Func<A, SourceT<M, B>> f)
        where M : MonadIO<M>, Alternative<M> =>
        SourceT.liftM(ma).Bind(f);    
    
    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static SourceT<M, C> SelectMany<M, A, B, C>(this K<M, A> ma, Func<A, SourceT<M, B>> bind, Func<A, B, C> project) 
        where M : MonadIO<M>, Alternative<M> =>
        SourceT.liftM(ma).As().SelectMany(bind, project);
    
    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static SourceT<M, C> SelectMany<M, A, B, C>(this IO<A> ma, Func<A, SourceT<M, B>> bind, Func<A, B, C> project) 
        where M : MonadIO<M>, Alternative<M> =>
        SourceT.liftIO<M, A>(ma).As().SelectMany(bind, project);
    
    /// <summary>
    /// Monad bind
    /// </summary>
    [Pure]
    public static SourceT<M, C> SelectMany<M, A, B, C>(this Pure<A> ma, Func<A, SourceT<M, B>> bind, Func<A, B, C> project) 
        where M : MonadIO<M>, Alternative<M> =>
        bind(ma.Value).Map(y => project(ma.Value, y));
    
    /// <summary>
    /// Access the `Some` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of optional values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    [Pure]
    public static SourceT<M, A> SomeSource<M, A>(this IAsyncEnumerable<OptionT<M, A>> stream)
        where M : MonadIO<M>, Alternative<M>  =>
        from xs in SourceT.lift<M, OptionT<M, A>>(stream)
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
    [Pure]
    public static SourceT<M, A> SomeSource<M, A>(this IAsyncEnumerable<Option<A>> stream)
        where M : MonadIO<M>, Alternative<M>  =>
        from ox in SourceT.lift<M, Option<A>>(stream)
        where ox.IsSome
        select (A)ox;

    /// <summary>
    /// Access the `Some` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of optional values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    [Pure]
    public static SourceT<M, A> SomeSource<M, A>(this IEnumerable<OptionT<M, A>> stream)
        where M : MonadIO<M>, Alternative<M>  =>
        from xs in SourceT.lift<M, OptionT<M, A>>(stream)
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
    [Pure]
    public static SourceT<M, A> SomeSource<M, A>(this IEnumerable<Option<A>> stream)
        where M : MonadIO<M>, Alternative<M>  =>
        from ox in SourceT.lift<M, Option<A>>(stream)
        where ox.IsSome
        select (A)ox;

    /// <summary>
    /// Access the `Right` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    [Pure]
    public static SourceT<M, A> RightSource<M, L, A>(this IAsyncEnumerable<EitherT<L, M, A>> stream)
        where M : MonadIO<M>, Alternative<M>  =>
        from xs in SourceT.lift<M, EitherT<L, M, A>>(stream)
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
    [Pure]
    public static SourceT<M, A> RightSource<M, L, A>(this IAsyncEnumerable<Either<L, A>> stream)
        where M : MonadIO<M>, Alternative<M>  =>
        from ox in SourceT.lift<M, Either<L, A>>(stream)
        where ox.IsRight
        select (A)ox;

    /// <summary>
    /// Access the `Right` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    [Pure]
    public static SourceT<M, A> RightSource<M, L, A>(this IEnumerable<EitherT<L, M, A>> stream)
        where M : MonadIO<M>, Alternative<M>  =>
        from xs in SourceT.lift<M, EitherT<L, M, A>>(stream)
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
    [Pure]
    public static SourceT<M, A> RightSource<M, L, A>(this IEnumerable<Either<L, A>> stream)
        where M : MonadIO<M>, Alternative<M>  =>
        from ox in SourceT.lift<M, Either<L, A>>(stream)
        where ox.IsRight
        select (A)ox;

    /// <summary>
    /// Access the `Left` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    [Pure]
    public static SourceT<M, L> LeftSource<M, L, A>(this IAsyncEnumerable<EitherT<L, M, A>> stream)
        where M : MonadIO<M>, Alternative<M>  =>
        from xs in SourceT.lift<M, EitherT<L, M, A>>(stream)
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
    [Pure]
    public static SourceT<M, L> LeftSource<M, L, A>(this IAsyncEnumerable<Either<L, A>> stream)
        where M : MonadIO<M>, Alternative<M>  =>
        from ox in SourceT.lift<M, Either<L, A>>(stream)
        where ox.IsLeft
        select (L)ox;

    /// <summary>
    /// Access the `Left` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    [Pure]
    public static SourceT<M, L> LeftSource<M, L, A>(this IEnumerable<EitherT<L, M, A>> stream)
        where M : MonadIO<M>, Alternative<M>  =>
        from xs in SourceT.lift<M, EitherT<L, M, A>>(stream)
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
    [Pure]
    public static SourceT<M, L> LeftSource<M, L, A>(this IEnumerable<Either<L, A>> stream)
        where M : MonadIO<M>, Alternative<M>  =>
        from ox in SourceT.lift<M, Either<L, A>>(stream)
        where ox.IsLeft
        select (L)ox;

    /// <summary>
    /// Access the `Succ` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    [Pure]
    public static SourceT<M, A> SuccSource<M, A>(this IAsyncEnumerable<FinT<M, A>> stream)
        where M : MonadIO<M>, Alternative<M>  =>
        from xs in SourceT.lift<M, FinT<M, A>>(stream)
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
    [Pure]
    public static SourceT<M, A> SuccSource<M, A>(this IAsyncEnumerable<Fin<A>> stream)
        where M : MonadIO<M>, Alternative<M>  =>
        from ox in SourceT.lift<M, Fin<A>>(stream)
        where ox.IsSucc
        select (A)ox;

    /// <summary>
    /// Access the `Succ` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    [Pure]
    public static SourceT<M, A> SuccSource<M, A>(this IEnumerable<FinT<M, A>> stream)
        where M : MonadIO<M>, Alternative<M>  =>
        from xs in SourceT.lift<M, FinT<M, A>>(stream)
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
    [Pure]
    public static SourceT<M, A> SuccSource<M, A>(this IEnumerable<Fin<A>> stream)
        where M : MonadIO<M>, Alternative<M>  =>
        from ox in SourceT.lift<M, Fin<A>>(stream)
        where ox.IsSucc
        select (A)ox;

    /// <summary>
    /// Access the `Fail` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    [Pure]
    public static SourceT<M, Error> FailSource<M, A>(this IAsyncEnumerable<FinT<M, A>> stream)
        where M : MonadIO<M>, Alternative<M>  =>
        from xs in SourceT.lift<M, FinT<M, A>>(stream)
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
    [Pure]
    public static SourceT<M, Error> FailSource<M, A>(this IAsyncEnumerable<Fin<A>> stream)
        where M : MonadIO<M>, Alternative<M>  =>
        from ox in SourceT.lift<M, Fin<A>>(stream)
        where ox.IsFail
        select (Error)ox;

    /// <summary>
    /// Access the `Fail` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    [Pure]
    public static SourceT<M, Error> FailSource<M, A>(this IEnumerable<FinT<M, A>> stream)
        where M : MonadIO<M>, Alternative<M>  =>
        from xs in SourceT.lift<M, FinT<M, A>>(stream)
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
    [Pure]
    public static SourceT<M, Error> FailSource<M, A>(this IEnumerable<Fin<A>> stream)
        where M : MonadIO<M>, Alternative<M>  =>
        from ox in SourceT.lift<M, Fin<A>>(stream)
        where ox.IsFail
        select (Error)ox;

    /// <summary>
    /// Access the `Succ` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    [Pure]
    public static SourceT<M, A> SuccSource<M, L, A>(this IAsyncEnumerable<ValidationT<L, M, A>> stream)
        where L : Monoid<L> 
        where M : MonadIO<M>, Alternative<M>  =>
        from xs in SourceT.lift<M, ValidationT<L, M, A>>(stream)
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
    [Pure]
    public static SourceT<M, A> SuccSource<M, L, A>(this IAsyncEnumerable<Validation<L, A>> stream)
        where L : Monoid<L> 
        where M : MonadIO<M>, Alternative<M>  =>
        from ox in SourceT.lift<M, Validation<L, A>>(stream)
        where ox.IsSuccess
        select (A)ox;

    /// <summary>
    /// Access the `Succ` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    [Pure]
    public static SourceT<M, A> SuccSource<M, L, A>(this IEnumerable<ValidationT<L, M, A>> stream)
        where L : Monoid<L> 
        where M : MonadIO<M>, Alternative<M>  =>
        from xs in SourceT.lift<M, ValidationT<L, M, A>>(stream)
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
    [Pure]
    public static SourceT<M, A> SuccSource<M, L, A>(this IEnumerable<Validation<L, A>> stream)
        where L : Monoid<L> 
        where M : MonadIO<M>, Alternative<M>  =>
        from ox in SourceT.lift<M, Validation<L, A>>(stream)
        where ox.IsSuccess
        select (A)ox;

    /// <summary>
    /// Access the `Fail` values from the asynchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    [Pure]
    public static SourceT<M, L> FailSource<M, L, A>(this IAsyncEnumerable<ValidationT<L, M, A>> stream)
        where L : Monoid<L> 
        where M : MonadIO<M>, Alternative<M>  =>
        from xs in SourceT.lift<M, ValidationT<L, M, A>>(stream)
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
    [Pure]
    public static SourceT<M, L> FailsStream<M, L, A>(this IAsyncEnumerable<Validation<L, A>> stream)
        where L : Monoid<L> 
        where M : MonadIO<M>, Alternative<M>  =>
        from ox in SourceT.lift<M, Validation<L, A>>(stream)
        where ox.IsFail
        select (L)ox;

    /// <summary>
    /// Access the `Fail` values from the synchronous stream
    /// </summary>
    /// <param name="stream">Stream of values</param>
    /// <typeparam name="M">Transformer monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Stream of values</returns>
    [Pure]
    public static SourceT<M, L> FailSource<M, L, A>(this IEnumerable<ValidationT<L, M, A>> stream)
        where L : Monoid<L> 
        where M : MonadIO<M>, Alternative<M>  =>
        from xs in SourceT.lift<M, ValidationT<L, M, A>>(stream)
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
    [Pure]
    public static SourceT<M, L> FailSource<M, L, A>(this IEnumerable<Validation<L, A>> stream)
        where L : Monoid<L> 
        where M : MonadIO<M>, Alternative<M>  =>
        from ox in SourceT.lift<M, Validation<L, A>>(stream)
        where ox.IsFail
        select (L)ox;
}
