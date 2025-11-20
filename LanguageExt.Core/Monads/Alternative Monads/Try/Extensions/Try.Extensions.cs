using System;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Try monad extensions
/// </summary>
public static partial class TryExtensions
{
    public static Try<A> As<A>(this K<Try, A> ma) =>
        (Try<A>)ma;

    /// <summary>
    /// Run the `Try`
    /// </summary>
    public static Fin<A> Run<A>(this K<Try, A> ma)
    {
        try
        {
            return ma.As().runTry();
        }
        catch (Exception e)
        {
            return Fin.Fail<A>(e);
        }
    }

    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Try<A> Flatten<A>(this K<Try, Try<A>> mma) =>
        new(() =>
                mma.As().Run() switch
                {
                    Fin<Try<A>>.Succ (var succ) => succ.Run(),
                    Fin<Try<A>>.Fail (var fail) => Fin.Fail<A>(fail),
                    _                           => throw new NotSupportedException()
                });

    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Try<A> Flatten<A>(this K<Try, K<Try, A>> mma) =>
        new(() =>
                mma.Run() switch
                {
                    Fin<K<Try, A>>.Succ (var succ) => succ.Run(),
                    Fin<K<Try, A>>.Fail (var fail) => Fin.Fail<A>(fail),
                    _                              => throw new NotSupportedException()
                });
}
