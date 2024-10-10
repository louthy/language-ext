using System;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Validation monad extensions
/// </summary>
public static class ValidationTExt
{
    public static ValidationT<L, M, A> As<L, M, A>(this K<ValidationT<L, M>, A> ma)
        where M : Monad<M>
        where L : Monoid<L> =>
        (ValidationT<L, M, A>)ma;
    
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

    /// <summary>
    /// Applicative apply
    /// </summary>
    [Pure]
    public static ValidationT<L, M, B> Apply<L, M, A, B>(this ValidationT<L, M, Func<A, B>> mf, ValidationT<L, M, A> ma)
        where M : Monad<M>
        where L : Monoid<L> =>
        new(M.Bind(mf.As().runValidation,
                   ef => ef.IsSuccess switch
                         {
                             true => 
                                 M.Bind(ma.As().runValidation,
                                        ea => ea.IsSuccess switch
                                              {
                                                  true => 
                                                      M.Pure(Validation<L, B>.Success(ef.SuccessValue(ea.SuccessValue))),
                                                 
                                                  false => 
                                                      M.Pure(Validation<L, B>.Fail(ef.FailValue.Combine(ea.FailValue))),
                                              }),
                             
                             false =>
                                 M.Bind(ma.As().runValidation,
                                        ea => ea.IsSuccess switch
                                              {
                                                  true => 
                                                      M.Pure(Validation<L, B>.Fail(ef.FailValue.Combine(ea.FailValue))),
                                                 
                                                  false => 
                                                      M.Pure(Validation<L, B>.Fail(ef.FailValue))

                                              })
                         }));

    /// <summary>
    /// Applicative action
    /// </summary>
    [Pure]
    public static ValidationT<L, M, B> Action<L, M, A, B>(this ValidationT<L, M, A> ma, ValidationT<L, M, B> mb)
        where M : Monad<M>
        where L : Monoid<L> =>
        fun((A _, B b) => b).Map(ma).Apply(mb).As();
    
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static ValidationT<FAIL, M, B> Map<FAIL, M, A, B>(this Func<A, B> f, K<ValidationT<FAIL, M>, A> ma)
        where M : Monad<M> 
        where FAIL : Monoid<FAIL> =>
        ma.Map(f).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static ValidationT<FAIL, M, Func<B, C>> Map<FAIL, M, A, B, C>(
        this Func<A, B, C> f, K<ValidationT<FAIL, M>, A> ma)
        where M : Monad<M> 
        where FAIL : Monoid<FAIL> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static ValidationT<FAIL, M, Func<B, Func<C, D>>> Map<FAIL, M, A, B, C, D>(
        this Func<A, B, C, D> f, K<ValidationT<FAIL, M>, A> ma)
        where M : Monad<M> 
        where FAIL : Monoid<FAIL> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static ValidationT<FAIL, M, Func<B, Func<C, Func<D, E>>>> Map<FAIL, M, A, B, C, D, E>(
        this Func<A, B, C, D, E> f, K<ValidationT<FAIL, M>, A> ma)
        where M : Monad<M> 
        where FAIL : Monoid<FAIL> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static ValidationT<FAIL, M, Func<B, Func<C, Func<D, Func<E, F>>>>> Map<FAIL, M, A, B, C, D, E, F>(
        this Func<A, B, C, D, E, F> f, K<ValidationT<FAIL, M>, A> ma) 
        where M : Monad<M> 
        where FAIL : Monoid<FAIL> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static ValidationT<FAIL, M, Func<B, Func<C, Func<D, Func<E, Func<F, G>>>>>> Map<FAIL, M, A, B, C, D, E, F, G>(
        this Func<A, B, C, D, E, F, G> f, K<ValidationT<FAIL, M>, A> ma) 
        where M : Monad<M> 
        where FAIL : Monoid<FAIL> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static ValidationT<FAIL, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, H>>>>>>> Map<FAIL, M, A, B, C, D, E, F, G, H>(
        this Func<A, B, C, D, E, F, G, H> f, K<ValidationT<FAIL, M>, A> ma) 
        where M : Monad<M> 
        where FAIL : Monoid<FAIL> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static ValidationT<FAIL, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, I>>>>>>>> Map<FAIL, M, A, B, C, D, E, F, G, H, I>(
        this Func<A, B, C, D, E, F, G, H, I> f, K<ValidationT<FAIL, M>, A> ma) 
        where M : Monad<M> 
        where FAIL : Monoid<FAIL> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static ValidationT<FAIL, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, J>>>>>>>>> Map<FAIL, M, A, B, C, D, E, F, G, H, I, J>(
        this Func<A, B, C, D, E, F, G, H, I, J> f, K<ValidationT<FAIL, M>, A> ma)
        where M : Monad<M> 
        where FAIL : Monoid<FAIL> =>
        ma.Map(x => curry(f)(x)).As();
    
    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <remarks>
    /// Unwraps the value within the functor, passes it to the map function `f` provided, and
    /// then takes the mapped value and wraps it back up into a new functor.
    /// </remarks>
    /// <param name="ma">Functor to map</param>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped functor</returns>
    public static ValidationT<FAIL, M, Func<B, Func<C, Func<D, Func<E, Func<F, Func<G, Func<H, Func<I, Func<J, K>>>>>>>>>> Map<FAIL, M, A, B, C, D, E, F, G, H, I, J, K>(
        this Func<A, B, C, D, E, F, G, H, I, J, K> f, K<ValidationT<FAIL, M>, A> ma) 
        where M : Monad<M> 
        where FAIL : Monoid<FAIL> =>
        ma.Map(x => curry(f)(x)).As();    
}
