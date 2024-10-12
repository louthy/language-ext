using System;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// OptionT monad-transformer extensions
/// </summary>
public static partial class OptionTExtensions
{
    public static OptionT<M, A> As<M, A>(this K<OptionT<M>, A> ma)
        where M : Monad<M> =>
        (OptionT<M, A>)ma;

    public static K<M, Option<A>> Run<M, A>(this K<OptionT<M>, A> ma)
        where M : Monad<M> =>
        ((OptionT<M, A>)ma).Run();

    /// <summary>
    /// Get the outer task and wrap it up in a new IO within the OptionT IO
    /// </summary>
    public static OptionT<IO, A> Flatten<A>(this Task<OptionT<IO, A>> tma) =>
        OptionT<IO, OptionT<IO, A>>
           .Lift(IO.liftAsync(async () => await tma.ConfigureAwait(false)))
           .Flatten();

    /// <summary>
    /// Lift the task
    /// </summary>
    public static OptionT<IO, A> ToIO<A>(this Task<Option<A>> ma) =>
        liftIO(ma);
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static OptionT<M, A> Flatten<M, A>(this OptionT<M, OptionT<M, A>> mma)
        where M : Monad<M> =>
        mma.Bind(identity);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`OptionT`</returns>
    [Pure]
    public static OptionT<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, 
        Func<A, K<OptionT<M>, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M> =>
        OptionT<M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`OptionT`</returns>
    [Pure]
    public static OptionT<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma, 
        Func<A, OptionT<M, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M> =>
        OptionT<M, A>.Lift(ma).SelectMany(bind, project);
}
