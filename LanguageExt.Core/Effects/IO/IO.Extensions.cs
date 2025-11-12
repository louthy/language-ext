using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IOExtensions
{
    /// <summary>
    /// Convert the kind version of the `IO` monad to an `IO` monad.
    /// </summary>
    /// <remarks>
    /// This is a simple cast operation which is just a bit more elegant
    /// than manually casting.
    /// </remarks>
    /// <param name="ma"></param>
    /// <typeparam name="A"></typeparam>
    /// <returns></returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<A> As<A>(this K<IO, A> ma) =>
        (IO<A>)ma;

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static A Run<A>(this K<IO, A> ma) =>
        ma.As().Run();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static A Run<A>(this K<IO, A> ma, EnvIO envIO) =>
        ma.As().Run(envIO);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fin<A> RunSafe<A>(this K<IO, A> ma) =>
        ma.As().Try().Run().Run();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Fin<A> RunSafe<A>(this K<IO, A> ma, EnvIO envIO) =>
        ma.As().Try().Run().Run(envIO);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<A> RunAsync<A>(this K<IO, A> ma) =>
        ma.As().RunAsync();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<A> RunAsync<A>(this K<IO, A> ma, EnvIO envIO) =>
        ma.As().RunAsync(envIO);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Fin<A>> RunSafeAsync<A>(this K<IO, A> ma) =>
        ma.As().Try().Run().RunAsync();

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<Fin<A>> RunSafeAsync<A>(this K<IO, A> ma, EnvIO envIO) =>
        ma.As().Try().Run().RunAsync(envIO);
    
    /// <summary>
    /// Get the outer task and wrap it up in a new IO within the IO
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<A> Flatten<A>(this Task<IO<A>> tma) =>
        IO.liftAsync(async () => await tma.ConfigureAwait(false))
          .Flatten();

    /// <summary>
    /// Unwrap the inner IO to flatten the structure
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<A> Flatten<A>(this IO<IO<A>> mma) =>
        mma.Bind(x => x);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<C> SelectMany<A, B, C>(this K<IO, A> ma, Func<A, K<IO, B>> bind, Func<A, B, C> project) =>
        ma.As().SelectMany(bind, project);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OptionT<M, C> SelectMany<M, A, B, C>(this K<IO, A> ma, Func<A, OptionT<M, B>> bind, Func<A, B, C> project)
        where M : MonadIO<M>, Alternative<M> =>
        OptionT.liftIO<M, A>(ma.As()).SelectMany(bind, project);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TryT<M, C> SelectMany<M, A, B, C>(this K<IO, A> ma, Func<A, TryT<M, B>> bind, Func<A, B, C> project)
        where M : MonadIO<M>, Alternative<M> =>
        TryT.liftIO<M, A>(ma.As()).SelectMany(bind, project);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EitherT<L, M, C> SelectMany<L, M, A, B, C>(this K<IO, A> ma, Func<A, EitherT<L, M, B>> bind, Func<A, B, C> project)
        where M : MonadIO<M>, Alternative<M> =>
        EitherT.liftIO<L, M, A>(ma.As()).SelectMany(bind, project);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FinT<M, C> SelectMany<M, A, B, C>(this K<IO, A> ma, Func<A, FinT<M, B>> bind, Func<A, B, C> project)
        where M : MonadIO<M>, Alternative<M> =>
        FinT.liftIO<M, A>(ma.As()).SelectMany(bind, project);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValidationT<F, M, C> SelectMany<F, M, A, B, C>(this K<IO, A> ma, Func<A, ValidationT<F, M, B>> bind, Func<A, B, C> project)
        where F : Monoid<F>
        where M : MonadIO<M>, Alternative<M> =>
        ValidationT.liftIO<F, M, A>(ma.As()).SelectMany(bind, project);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReaderT<Env, M, C> SelectMany<Env, M, A, B, C>(this K<IO, A> ma, Func<A, ReaderT<Env, M, B>> bind, Func<A, B, C> project)
        where M : MonadIO<M>, Alternative<M> =>
        ReaderT<Env, M, A>.LiftIO(ma.As()).SelectMany(bind, project);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StateT<S, M, C> SelectMany<S, M, A, B, C>(this K<IO, A> ma, Func<A, StateT<S, M, B>> bind, Func<A, B, C> project)
        where M : MonadIO<M>, Alternative<M> =>
        StateT<S, M, A>.LiftIO(ma.As()).SelectMany(bind, project);

    /// <summary>
    /// Wait for a signal
    /// </summary>
    public static IO<bool> WaitOneIO(this AutoResetEvent wait) =>
        IO.liftAsync(e => wait.WaitOneAsync(e.Token));

    /// <summary>
    /// Wait for a signal
    /// </summary>
    public static K<M, bool> WaitOneIO<M>(this AutoResetEvent wait)
        where M : Monad<M> =>
        M.LiftIOMaybe(wait.WaitOneIO());
}
