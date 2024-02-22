using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EffExtensions
{
    /// <summary>
    /// Cast type to its Kind
    /// </summary>
    public static Eff<RT, A> As<RT, A>(this K<Eff<RT>, A> ma) =>
        (Eff<RT, A>)ma;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Invoking
    //

    /// <summary>
    /// Invoke the effect
    /// </summary>
    /// <remarks>
    /// Returns the result value only 
    /// </remarks>
    [Pure, MethodImpl(Opt.Default)]
    public static Fin<A> Run<RT, A>(this Eff<RT, A> ma, RT env)
        where RT : HasIO<RT> =>
        ma.RunRT(env).Map(x => x.Value);
    
    /// <summary>
    /// Invoke the effect
    /// </summary>
    /// <remarks>
    /// Returns the result value and the runtime (which carries state) 
    /// </remarks>
    [Pure, MethodImpl(Opt.Default)]
    public static Fin<(A Value, RT Runtime)> RunRT<RT, A>(this Eff<RT, A> ma, RT env)
        where RT : HasIO<RT>
    {
        try
        {
            return ma.RunUnsafe(env);
        }
        catch (ErrorException e)
        {
            return e.ToError();
        }
        catch(Exception e)
        {
            return Error.New(e);
        }
    }

    /// <summary>
    /// Invoke the effect
    /// </summary>
    /// <remarks>
    /// This is labelled 'unsafe' because it can throw an exception, whereas
    /// `Run` will capture any errors and return a `Fin` type.
    /// </remarks>
    [Pure, MethodImpl(Opt.Default)]
    public static (A Value, RT Runtime) RunUnsafe<RT, A>(this Eff<RT, A> ma, RT env) 
        where RT : HasIO<RT> =>
        ma.effect
          .Run(env).As()
          .Run().As()
          .Run(env.EnvIO);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Matching
    //

    /// <summary>
    /// Map the failure to a new IO effect
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO that encapsulates that IfFail</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> IfFailEff<RT, A>(this Eff<RT, A> ma, Func<Error, Eff<A>> Fail)
        where RT : HasIO<RT> =>
        ma.Match(Succ: Eff<RT, A>.Pure, Fail: x => Fail(x).WithRuntime<RT>()).Flatten();    
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic bind
    //

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, B> Bind<RT, A, B>(this Eff<RT, A> ma, Func<A, Eff<B>> f)
        where RT : HasIO<RT> =>
        ma.Bind(x => f(x).WithRuntime<RT>());

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, B> Bind<RT, A, B>(this Eff<RT, A> ma, Func<A, K<Eff, B>> f)
        where RT : HasIO<RT> =>
        ma.Bind(a => f(a).As());    
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic join
    //

    /// <summary>
    /// Monadic join operator 
    /// </summary>
    /// <remarks>
    /// Collapses a nested IO monad so there is no nesting.
    /// </remarks>
    /// <param name="mma">Nest IO monad to flatten</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Bound value</typeparam>
    /// <returns>Flattened IO monad</returns>
    public static Eff<RT, A> Flatten<RT, A>(this Eff<RT, Eff<RT, A>> mma) =>
        mma.Bind(ma => ma);
    

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  SelectMany extensions
    //

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, C> SelectMany<RT, A, B, C>(this Eff<RT, A> ma, Func<A, Eff<B>> bind, Func<A, B, C> project) 
        where RT : HasIO<RT> =>
        ma.Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, C> SelectMany<RT, A, B, C>(this Eff<RT, A> ma, Func<A, K<Eff, B>> bind, Func<A, B, C> project) 
        where RT : HasIO<RT> =>
        ma.SelectMany(x => bind(x).As(), project);

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<RT, D> SelectMany<RT, A, B, C, D>(
        this (Eff<RT, A> First, Eff<RT, B> Second) self,
        Func<(A First, B Second), Eff<RT, C>> bind,
        Func<(A First, B Second), C, D> project) =>
        self.Zip().Bind(ab => bind(ab).Map(c => project(ab, c)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<RT, D> SelectMany<RT, A, B, C, D>(
        this Eff<RT, A> self,
        Func<A, (Eff<RT, B> First, Eff<RT, C> Second)> bind,
        Func<A, (B First, C Second), D> project) =>
        self.Bind(a => bind(a).Zip().Map(cd => project(a, cd)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<RT, E> SelectMany<RT, A, B, C, D, E>(
        this (Eff<RT, A> First, Eff<RT, B> Second, Eff<RT, C> Third) self,
        Func<(A First, B Second, C Third), Eff<RT, D>> bind,
        Func<(A First, B Second, C Third), D, E> project) =>
        self.Zip().Bind(ab => bind(ab).Map(c => project(ab, c)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<RT, E> SelectMany<RT, A, B, C, D, E>(
        this Eff<RT, A> self,
        Func<A, (Eff<RT, B> First, Eff<RT, C> Second, Eff<RT, D> Third)> bind,
        Func<A, (B First, C Second, D Third), E> project) =>
        self.Bind(a => bind(a).Zip().Map(cd => project(a, cd)));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Zipping
    //

    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <param name="tuple">Tuple of IO monads to run</param>
    /// <typeparam name="RT">Runtime type</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    public static Eff<RT, (A First, B Second)> Zip<RT, A, B>(
        this (Eff<RT, A> First, Eff<RT, B> Second) tuple) =>
        from e1 in tuple.First.Fork()
        from e2 in tuple.Second.Fork()
        from r1 in e1.Await
        from r2 in e2.Await
        select (r1, r2);

    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <param name="tuple">Tuple of IO monads to run</param>
    /// <typeparam name="RT">Runtime type</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <typeparam name="C">Third IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    public static Eff<RT, (A First, B Second, C Third)> Zip<RT, A, B, C>(
        this (Eff<RT, A> First,
              Eff<RT, B> Second,
              Eff<RT, C> Third) tuple) =>
        from e1 in tuple.First.Fork()
        from e2 in tuple.Second.Fork()
        from e3 in tuple.Third.Fork()
        from r1 in e1.Await
        from r2 in e2.Await
        from r3 in e3.Await
        select (r1, r2, r3);

    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <param name="tuple">Tuple of IO monads to run</param>
    /// <typeparam name="RT">Runtime type</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <typeparam name="C">Third IO monad bound value type</typeparam>
    /// <typeparam name="D">Fourth IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    public static Eff<RT, (A First, B Second, C Third, D Fourth)> Zip<RT, A, B, C, D>(
        this (Eff<RT, A> First, 
              Eff<RT, B> Second, 
              Eff<RT, C> Third, 
              Eff<RT, D> Fourth) tuple) =>
        from e1 in tuple.First.Fork()
        from e2 in tuple.Second.Fork()
        from e3 in tuple.Third.Fork()
        from e4 in tuple.Fourth.Fork()
        from r1 in e1.Await
        from r2 in e2.Await
        from r3 in e3.Await
        from r4 in e4.Await
        select (r1, r2, r3, r4);
    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <param name="tuple">Tuple of IO monads to run</param>
    /// <typeparam name="RT">Runtime type</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    public static Eff<RT, (A First, B Second)> Zip<RT, A, B>(
        this Eff<RT, A> First,
        Eff<RT, B> Second) =>
        (First, Second).Zip();

    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <param name="tuple">Tuple of IO monads to run</param>
    /// <typeparam name="RT">Runtime type</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <typeparam name="C">Third IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    public static Eff<RT, (A First, B Second, C Third)> Zip<RT, A, B, C>(
        this Eff<RT, A> First, 
        Eff<RT, B> Second, 
        Eff<RT, C> Third) =>
        (First, Second, Third).Zip();
    
    /// <summary>
    /// Takes two IO monads and zips their result
    /// </summary>
    /// <remarks>
    /// Asynchronous operations will run concurrently
    /// </remarks>
    /// <param name="tuple">Tuple of IO monads to run</param>
    /// <typeparam name="RT">Runtime type</typeparam>
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">First IO monad bound value type</typeparam>
    /// <typeparam name="B">Second IO monad bound value type</typeparam>
    /// <typeparam name="C">Third IO monad bound value type</typeparam>
    /// <typeparam name="D">Fourth IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    public static Eff<RT, (A First, B Second, C Third, D Fourth)> Zip<RT, A, B, C, D>(
        this Eff<RT, A> First, 
        Eff<RT, B> Second, 
        Eff<RT, C> Third, 
        Eff<RT, D> Fourth) =>
        (First, Second, Third, Fourth).Zip();
}
