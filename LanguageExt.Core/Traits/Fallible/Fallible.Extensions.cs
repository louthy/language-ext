using System;
using LanguageExt.Common;

namespace LanguageExt.Traits;

/// <summary>
/// Trait for higher-kinded structures that have a failure state `E`
/// </summary>
/// <typeparam name="F">Higher-kinded structure</typeparam>
/// <typeparam name="E">Failure type</typeparam>
public static class FallibleExtensions
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    //  Error catching by predicate
    //
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="fa">`Fallible` structure</param>
    /// <param name="Predicate">Predicate to test any failure values</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<F, A> Catch<E, F, A>(
        this K<F, A> fa,
        Func<E, bool> Predicate,
        Func<E, K<F, A>> Fail) where F : Fallible<E, F> =>
        F.Catch(fa, Predicate, Fail);
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="fa">`Fallible` structure</param>
    /// <param name="Predicate">Predicate to test any failure values</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<F, A> Catch<F, A>(
        this K<F, A> fa,
        Func<Error, bool> Predicate,
        Func<Error, K<F, A>> Fail) where F : Fallible<F> =>
        F.Catch(fa, Predicate, Fail);
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="ma">`Fallible` structure</param>
    /// <param name="Predicate">Predicate to test any failure values</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<M, A> Catch<E, M, A>(
        this K<M, A> ma,
        Func<E, bool> Predicate,
        Func<E, K<IO, A>> Fail) where M : Fallible<E, M>, Monad<M> =>
        M.Catch(ma, Predicate, e => M.LiftIO(Fail(e)));
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="ma">`Fallible` structure</param>
    /// <param name="Predicate">Predicate to test any failure values</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<M, A> Catch<M, A>(
        this K<M, A> ma,
        Func<Error, bool> Predicate,
        Func<Error, K<IO, A>> Fail) where M : Fallible<M>, Monad<M> =>
        M.Catch(ma, Predicate, e => M.LiftIO(Fail(e)));
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="fa">`Fallible` structure</param>
    /// <param name="Predicate">Predicate to test any failure values</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<F, A> Catch<E, F, A>(
        this K<F, A> fa,
        Func<E, bool> Predicate,
        Func<E, E> Fail) where F : Fallible<E, F> =>
        F.Catch(fa, Predicate, e => F.Fail<A>(Fail(e)));
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="fa">`Fallible` structure</param>
    /// <param name="Predicate">Predicate to test any failure values</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<F, A> Catch<F, A>(
        this K<F, A> fa,
        Func<Error, bool> Predicate,
        Func<Error, Error> Fail) where F : Fallible<F> =>
        F.Catch(fa, Predicate, e => F.Fail<A>(Fail(e)));
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="fa">`Fallible` structure</param>
    /// <param name="Predicate">Predicate to test any failure values</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<F, A> Catch<E, F, A>(
        this K<F, A> fa,
        Func<E, bool> Predicate,
        Func<E, A> Fail) where F : Fallible<E, F>, Applicative<F> =>
        F.Catch(fa, Predicate, e => F.Pure(Fail(e)));
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="fa">`Fallible` structure</param>
    /// <param name="Predicate">Predicate to test any failure values</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<F, A> Catch<F, A>(
        this K<F, A> fa,
        Func<Error, bool> Predicate,
        Func<Error, A> Fail) where F : Fallible<F>, Applicative<F> =>
        F.Catch(fa, Predicate, e => F.Pure(Fail(e)));

    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    //  Error catching by equality
    //
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="fa">`Fallible` structure</param>
    /// <param name="Match">Error to match to</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<F, A> Catch<E, F, A>(
        this K<F, A> fa,
        Error Match,
        Func<E, K<F, A>> Fail) where F : Fallible<E, F> =>
        F.Catch(fa, e => Match.Equals(e), Fail);
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="fa">`Fallible` structure</param>
    /// <param name="Match">Error to match to</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<F, A> Catch<F, A>(
        this K<F, A> fa,
        Error Match,
        Func<Error, K<F, A>> Fail) where F : Fallible<F> =>
        F.Catch(fa, e => e.Is(Match), Fail);
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="ma">`Fallible` structure</param>
    /// <param name="Match">Error to match to</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<M, A> Catch<E, M, A>(
        this K<M, A> ma,
        Error Match,
        Func<E, K<IO, A>> Fail) where M : Fallible<E, M>, Monad<M> =>
        M.Catch(ma, e => Match.Equals(e), e => M.LiftIO(Fail(e)));
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="ma">`Fallible` structure</param>
    /// <param name="Match">Error to match to</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<M, A> Catch<M, A>(
        this K<M, A> ma,
        Error Match,
        Func<Error, K<IO, A>> Fail) where M : Fallible<M>, Monad<M> =>
        M.Catch(ma, e => e.Is(Match), e => M.LiftIO(Fail(e)));
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="fa">`Fallible` structure</param>
    /// <param name="Match">Error to match to</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<F, A> Catch<E, F, A>(
        this K<F, A> fa,
        Error Match,
        Func<E, E> Fail) where F : Fallible<E, F> =>
        F.Catch(fa, e => Match.Equals(e), e => F.Fail<A>(Fail(e)));
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="fa">`Fallible` structure</param>
    /// <param name="Match">Error to match to</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<F, A> Catch<F, A>(
        this K<F, A> fa,
        Error Match,
        Func<Error, Error> Fail) where F : Fallible<F> =>
        F.Catch(fa, e => e.Is(Match), e => F.Fail<A>(Fail(e)));
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="fa">`Fallible` structure</param>
    /// <param name="Match">Error to match to</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<F, A> Catch<E, F, A>(
        this K<F, A> fa,
        Error Match,
        Func<E, A> Fail) where F : Fallible<E, F>, Applicative<F> =>
        F.Catch(fa, e => Match.Equals(e), e => F.Pure(Fail(e)));
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="fa">`Fallible` structure</param>
    /// <param name="Match">Error to match to</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<F, A> Catch<F, A>(
        this K<F, A> fa,
        Error Match,
        Func<Error, A> Fail) where F : Fallible<F>, Applicative<F> =>
        F.Catch(fa, e => e.Is(Match), e => F.Pure(Fail(e)));
    
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    //  Catch all errors 
    //
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="fa">`Fallible` structure</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<F, A> Catch<E, F, A>(
        this K<F, A> fa,
        Func<E, K<F, A>> Fail) where F : Fallible<E, F> =>
        F.Catch(fa, _ => true, Fail);
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="fa">`Fallible` structure</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<F, A> Catch<F, A>(
        this K<F, A> fa,
        Func<Error, K<F, A>> Fail) where F : Fallible<F> =>
        F.Catch(fa, _ => true, Fail);
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="ma">`Fallible` structure</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<M, A> Catch<E, M, A>(
        this K<M, A> ma,
        Func<E, K<IO, A>> Fail) where M : Fallible<E, M>, Monad<M> =>
        M.Catch(ma, _ => true, e => M.LiftIO(Fail(e)));
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="ma">`Fallible` structure</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<M, A> Catch<M, A>(
        this K<M, A> ma,
        Func<Error, K<IO, A>> Fail) where M : Fallible<M>, Monad<M> =>
        M.Catch(ma, _ => true, e => M.LiftIO(Fail(e)));
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="fa">`Fallible` structure</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<F, A> Catch<E, F, A>(
        this K<F, A> fa,
        Func<E, E> Fail) where F : Fallible<E, F> =>
        F.Catch(fa, _ => true, e => F.Fail<A>(Fail(e)));
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="fa">`Fallible` structure</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<F, A> Catch<F, A>(
        this K<F, A> fa,
        Func<Error, Error> Fail) where F : Fallible<F> =>
        F.Catch(fa, _ => true, e => F.Fail<A>(Fail(e)));
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="fa">`Fallible` structure</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<F, A> Catch<E, F, A>(
        this K<F, A> fa,
        Func<E, A> Fail) where F : Fallible<E, F>, Applicative<F> =>
        F.Catch(fa, _ => true, e => F.Pure(Fail(e)));
    
    /// <summary>
    /// Run the `Fallible` structure.  If in a failed state, test the failure value
    /// against the predicate.  If, it returns `true`, run the `Fail` function with
    /// the failure value.
    /// </summary>
    /// <param name="fa">`Fallible` structure</param>
    /// <param name="Fail">Handler when in failed state</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Either `fa` or the result of `Fail` if `fa` is in a failed state and the
    /// predicate returns true for the failure value</returns>
    public static K<F, A> Catch<F, A>(
        this K<F, A> fa,
        Func<Error, A> Fail) where F : Fallible<F>, Applicative<F> =>
        F.Catch(fa, _ => true, e => F.Pure(Fail(e)));    
}
