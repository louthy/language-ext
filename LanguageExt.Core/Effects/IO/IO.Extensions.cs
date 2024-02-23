using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
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

    /// <summary>
    /// Await a forked operation
    /// </summary>
    [Pure]
    public static IO<A> Await<A>(this K<IO, ForkIO<A>> ma) =>
        ma.As().Bind(f => f.Await);

    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    [Pure]
    [MethodImpl(Opt.Default)]
    public static K<M, ForkIO<A>> Fork<M, A>(this K<M, A> ma, Option<TimeSpan> timeout = default)
        where M : Monad<M> =>
        from mk in M.UnliftIO<A>()
        from rs in mk(ma).Fork(timeout)
        select rs;
}
