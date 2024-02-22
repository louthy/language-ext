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
}
