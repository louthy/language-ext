using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Access the cancellation-token from the IO environment
    /// </summary>
    /// <returns>CancellationToken</returns>
    public static readonly IO<CancellationToken> cancelToken =
        IO.lift(e => e.Token);
    
    /// <summary>
    /// Request a cancellation of the IO expression
    /// </summary>
    public static readonly IO<Unit> cancel = 
        new (e =>
             {
                 e.Source.Cancel();
                 throw new TaskCanceledException();
             });
    
    /// <summary>
    /// Always yields a `Unit` value
    /// </summary>
    public static readonly IO<Unit> unitIO = 
        IO<Unit>.Pure(default);
    
    /// <summary>
    /// Yields the IO environment
    /// </summary>
    public static readonly IO<EnvIO> envIO = 
        IO<EnvIO>.Lift(e => e);

    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, ForkIO<A>> fork<M, A>(K<M, A> ma, Option<TimeSpan> timeout = default)
        where M : Monad<M> =>
        from mk in M.UnliftIO<A>()
        from rs in mk(ma).Fork(timeout)
        select rs;

    /// <summary>
    /// Timeout operation if it takes too long
    /// </summary>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static IO<A> timeout<A>(TimeSpan timeout, IO<A> ma) =>
         ma.Timeout(timeout);
}
