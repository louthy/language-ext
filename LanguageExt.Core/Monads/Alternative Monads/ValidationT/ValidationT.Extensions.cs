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
    public static ValidationT<F, M, A> As<F, M, A>(this K<ValidationT<F, M>, A> ma)
        where M : Monad<M> =>
        (ValidationT<F, M, A>)ma;
    
    public static ValidationT<F, M, A> As2<F, M, A>(this K<ValidationT<M>, F, A> ma)
        where M : Monad<M> =>
        (ValidationT<F, M, A>)ma;
    
    public static K<M, Validation<F, A>> Run<F, M, A>(this K<ValidationT<F, M>, A> ma)
        where M : Monad<M>
        where F : Monoid<F> =>
        ((ValidationT<F, M, A>)ma).Run(F.Instance);

    public static K<M, B> Match<F, M, A, B>(
        this K<ValidationT<F, M>, A> ma, 
        Func<F, B> Fail,
        Func<A, B> Succ) 
        where M : Monad<M> 
        where F : Monoid<F> =>
        ma.As().MatchI(Fail, Succ)(F.Instance);
    
    /// <summary>
    /// Maps the bound value
    /// </summary>
    /// <param name="f">Mapping function</param>
    /// <typeparam name="B">Target bound value type</typeparam>
    /// <returns>`ValidationT`</returns>
    public static ValidationT<F1, M, A> MapFail<F, F1, M, A>(
        this K<ValidationT<F, M>, A> ma, 
        Func<F, F1> f) 
        where M : Monad<M> 
        where F : Monoid<F> 
        where F1 : Monoid<F1> =>
        new(_ => M.Map(mx => mx.MapFail(f), ma.Run()));
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static ValidationT<F, M, A> Flatten<F, M, A>(this ValidationT<F, M, ValidationT<F, M, A>> mma)
        where M : Monad<M> 
        where F : Monoid<F> =>
        mma.Bind(identity);

    /// <summary>
    /// Get the outer task and wrap it up in a new IO within the EitherT IO
    /// </summary>
    public static ValidationT<F, IO, A> Flatten<F, A>(this Task<ValidationT<F, IO, A>> tma)
        where F : Monoid<F> =>
        ValidationT
           .lift<F, IO, ValidationT<F, IO, A>>(IO.liftAsync(async () => await tma.ConfigureAwait(false)))
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
        new(ma.runValidation(F.Instance).Map(ma => ma.ToOption()));

    public static EitherT<F, M, A> ToEither<F, M, A>(this ValidationT<F, M, A> ma)
        where F : Monoid<F>
        where M : Monad<M> =>
        new(ma.runValidation(F.Instance).Map(ma => ma.ToEither()));

    public static FinT<M, A> ToFin<M, A>(this ValidationT<Error, M, A> ma)
        where M : Monad<M> =>
        new(ma.runValidation(Monoid.instance<Error>()).Map(ma => ma.ToFin()));
    
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
        ValidationT.lift<L, M, A>(ma).SelectMany(bind, project);

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
        ValidationT.lift<L, M, A>(ma).SelectMany(bind, project);
}
