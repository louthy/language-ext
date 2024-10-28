using System;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// ValidationT monad transformer extensions
/// </summary>
public static partial class ValidationTExtensions
{
    public static ValidationT<L, M, A> As<L, M, A>(this K<ValidationT<L, M>, A> ma)
        where M : Monad<M>
        where L : Monoid<L> =>
        (ValidationT<L, M, A>)ma;
    
    public static K<M, Validation<L, A>> Run<L, M, A>(this K<ValidationT<L, M>, A> ma)
        where M : Monad<M>
        where L : Monoid<L> =>
        ((ValidationT<L, M, A>)ma).Run();
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static ValidationT<L, M, A> Flatten<L, M, A>(this ValidationT<L, M, ValidationT<L, M, A>> mma)
        where M : Monad<M> 
        where L : Monoid<L> =>
        mma.Bind(identity);

    /// <summary>
    /// Get the outer task and wrap it up in a new IO within the EitherT IO
    /// </summary>
    public static ValidationT<L, IO, A> Flatten<L, A>(this Task<ValidationT<L, IO, A>> tma)
        where L : Monoid<L> =>
        ValidationT<L, IO, ValidationT<L, IO, A>>
           .Lift(IO.liftAsync(async () => await tma.ConfigureAwait(false)))
           .Flatten();

    /// <summary>
    /// Lift the task
    /// </summary>
    public static ValidationT<L, IO, A> ToIO<L, A>(this Task<Validation<L, A>> ma)
        where L : Monoid<L> =>
        liftIO(ma);
    
    public static OptionT<M, A> ToOption<F, M, A>(this ValidationT<F, M, A> ma)
        where F : Monoid<F>
        where M : Monad<M> =>
        new(ma.runValidation.Map(ma => ma.ToOption()));

    public static EitherT<F, M, A> ToEither<F, M, A>(this ValidationT<F, M, A> ma)
        where F : Monoid<F>
        where M : Monad<M> =>
        new(ma.runValidation.Map(ma => ma.ToEither()));

    public static FinT<M, A> ToFin<M, A>(this ValidationT<Error, M, A> ma)
        where M : Monad<M> =>
        new(ma.runValidation.Map(ma => ma.ToFin()));
    
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    [Pure]
    public static ValidationT<L, M, C> SelectMany<L, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, K<ValidationT<L, M>, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M>
        where L : Monoid<L> =>
        ValidationT<L, M, A>.Lift(ma).SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    [Pure]
    public static ValidationT<L, M, C> SelectMany<L, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, ValidationT<L, M, B>> bind, 
        Func<A, B, C> project)
        where M : Monad<M>
        where L : Monoid<L> =>
        ValidationT<L, M, A>.Lift(ma).SelectMany(bind, project);
}
