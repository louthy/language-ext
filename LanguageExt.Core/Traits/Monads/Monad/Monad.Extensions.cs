using System;
using LanguageExt.Traits;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class MonadExtensions
{
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Monadic bind function</param>
    /// <typeparam name="A">Initial bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <returns>M〈B〉</returns>
    [Pure]
    public static K<M, B> Bind<M, A, B>(
        this K<M, A> ma,
        Func<A, K<M, B>> f)
        where M : Monad<M> =>
        M.Bind(ma, f);
    
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="f">Monadic bind function</param>
    /// <typeparam name="A">Initial bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <returns>M〈B〉</returns>
    public static K<M, B> Bind<M, A, B>(
        this K<M, A> ma,
        Func<A, Pure<B>> f)
        where M : Functor<M> =>
        M.Map(x => f(x).Value, ma);
    
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="A">Initial bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>M〈C〉</returns>
    public static K<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma,
        Func<A, K<M, B>> bind,
        Func<A, B, C> project)
        where M : Monad<M> =>
        M.Bind(ma, a => M.Map(b => project(a, b) , bind(a)));
    
    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="A">Initial bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>M〈C〉</returns>
    public static K<M, C> SelectMany<M, A, B, C>(
        this K<M, A> ma,
        Func<A, Pure<B>> bind,
        Func<A, B, C> project)
        where M : Functor<M> =>
        M.Map(a => project(a, bind(a).Value), ma);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="M">Monad trait</typeparam>
    /// <typeparam name="A">Initial bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>M〈C〉</returns>
    public static K<M, C> SelectMany<E, M, A, C>(
        this K<M, A> ma,
        Func<A, Guard<E, Unit>> bind,
        Func<A, Unit, C> project)
        where M : Monad<M>, Fallible<E, M> =>
        M.Bind(ma, a => bind(a) switch
                        {
                            { Flag: true } => M.Pure(project(a, default)),
                            var guard      => M.Fail<C>(guard.OnFalse())
                        });

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="M">Monad trait</typeparam>
    /// <typeparam name="A">Initial bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>M〈C〉</returns>
    public static K<M, C> SelectMany<E, M, A, C>(
        this K<M, A> ma,
        Func<A, Guard<Fail<E>, Unit>> bind,
        Func<A, Unit, C> project)
        where M : Monad<M>, Fallible<E, M> =>
        M.Bind(ma, a => bind(a) switch
                        {
                            { Flag: true } => M.Pure(project(a, default)),
                            var guard      => M.Fail<C>(guard.OnFalse().Value)
                        });
    
    /// <summary>
    /// Monadic join operation
    /// </summary>
    /// <param name="mma"></param>
    /// <typeparam name="M">Monad trait</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Joined monad</returns>
    public static K<M, A> Flatten<M, A>(this K<M, K<M, A>> mma)
        where M : Monad<M> =>
        M.Bind(mma, Prelude.identity);

    /// <param name="ma">Computation to recursively run</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    extension<M, A>(K<M, A> ma) 
        where M : Monad<M>
    {
        /// <summary>
        /// This is equivalent to the infinite loop below without the stack-overflow issues:
        /// 
        ///     K〈M, A〉go =>
        ///         ma.Bind(_ => go);
        /// 
        /// </summary>
        /// <typeparam name="B">'Result' type, there will never be a result of `B`, but the monad rules may exit the loop
        /// with an alternative value; and so `B` is still valid</typeparam>
        /// <returns>A looped computation</returns>
        [Pure]
        public K<M, A> Forever() =>
            Monad.forever<M, A, A>(ma);

        /// <summary>
        /// This is equivalent to the infinite loop below without the stack-overflow issues:
        /// 
        ///     K〈M, A〉go =>
        ///         ma.Bind(_ => go);
        /// 
        /// </summary>
        /// <typeparam name="B">'Result' type, there will never be a result of `B`, but the monad rules may exit the loop
        /// with an alternative value; and so `B` is still valid</typeparam>
        /// <returns>A looped computation</returns>
        [Pure]
        public K<M, B> Forever<B>() =>
            Monad.forever<M, A, B>(ma);

        /// <summary>
        /// Running the monadic computation `ma` a fixed number of times (`count`) collecting the results
        /// </summary>
        /// <param name="count">Number of times to replicate monadic computation</param>
        /// <returns>A lifted iterable of values collected</returns>
        [Pure]
        public K<M, Iterable<A>> Replicate(int count) =>
            Monad.replicate(ma, count);

        /// <summary>
        /// Keep running the monadic computation `ma` collecting the result values until a result value
        /// yielded triggers a `true` value when passed to the `f` predicate
        /// </summary>
        /// <param name="f">Predicate</param>
        /// <returns>A lifted iterable of values collected</returns>
        [Pure]
        public K<M, Iterable<A>> AccumUntil(Func<A, bool> f) =>
            Monad.accumWhile(ma, f);

        /// <summary>
        /// Keep running the monadic computation `ma` collecting the result values until a result value
        /// yielded triggers a `true` value when passed to the `f` predicate
        /// </summary>
        /// <param name="f">Predicate</param>
        /// <returns>A lifted iterable of values collected</returns>
        [Pure]
        public K<M, Iterable<A>> AccumUntilM(Func<A, K<M, bool>> f) =>
            Monad.accumUntilM(ma, f);

        /// <summary>
        /// Keep running the monadic computation `ma` collecting the result values until a result value
        /// yielded triggers a `true` value when passed to the `f` predicate
        /// </summary>
        /// <param name="f">Predicate</param>
        /// <returns>A lifted iterable of values collected</returns>
        [Pure]
        public K<M, Iterable<A>> AccumWhile(Func<A, bool> f) =>
            Monad.accumWhile(ma, f);

        /// <summary>
        /// Keep running the monadic computation `ma` collecting the result values until a result value
        /// yielded triggers a `true` value when passed to the `f` predicate
        /// </summary>
        /// <param name="f">Predicate</param>
        /// <returns>A lifted iterable of values collected</returns>
        [Pure]
        public K<M, Iterable<A>> AccumWhileM(Func<A, K<M, bool>> f) =>
            Monad.accumWhileM(ma, f);
    }
}
