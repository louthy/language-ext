using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
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
    public static Fin<A> Run<RT, A>(this K<Eff<RT>, A> ma, RT env) =>
        ma.As().Run(env, EnvIO.New());

    /// <summary>
    /// Invoke the effect
    /// </summary>
    /// <remarks>
    /// Returns the result value only 
    /// </remarks>
    [Pure, MethodImpl(Opt.Default)]
    public static Fin<A> Run<RT, A>(this K<Eff<RT>, A> ma, RT env, EnvIO envIO)
    {
        try
        {
            return ma.As().effect.Run(env).Run(envIO).Value;
        }
        catch(Exception e)
        {
            return Fin<A>.Fail(e);
        }
    }  
    
    /// <summary>
    /// Invoke the effect
    /// </summary>
    /// <remarks>
    /// Returns the result value and the runtime (which carries state) 
    /// </remarks>
    [Pure, MethodImpl(Opt.Default)]
    public static Fin<(A Value, RT Runtime)> RunState<RT, A>(this K<Eff<RT>, A> ma, RT env)
    {
        try
        {
            return ma.As().effect.Run(env).Run(EnvIO.New());
        }
        catch(Exception e)
        {
            return Fin<(A Value, RT Runtime)>.Fail(e);
        }
    }  
    
    /// <summary>
    /// Invoke the effect
    /// </summary>
    /// <remarks>
    /// Returns the result value and the runtime (which carries state) 
    /// </remarks>
    [Pure, MethodImpl(Opt.Default)]
    public static Fin<(A Value, RT Runtime)> RunState<RT, A>(this K<Eff<RT>, A> ma, RT env, EnvIO envIO)
    {
        try
        {
            return ma.As().effect.Run(env).Run(envIO);
        }
        catch(Exception e)
        {
            return Fin<(A Value, RT Runtime)>.Fail(e);
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
    public static (A Value, RT Runtime) RunUnsafe<RT, A>(this K<Eff<RT>, A> ma, RT env) =>
        ma.As().effect.Run(env).Run(EnvIO.New());

    /// <summary>
    /// Invoke the effect
    /// </summary>
    /// <remarks>
    /// This is labelled 'unsafe' because it can throw an exception, whereas
    /// `Run` will capture any errors and return a `Fin` type.
    /// </remarks>
    [Pure, MethodImpl(Opt.Default)]
    public static (A Value, RT Runtime) RunUnsafe<RT, A>(this K<Eff<RT>, A> ma, RT env, EnvIO envIO) =>
        ma.As().effect.Run(env).Run(envIO);

    /// <summary>
    /// Invoke the effect to leave the inner IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<(A Value, RT Runtime)> RunIO<RT, A>(this K<Eff<RT>, A> ma, RT env) =>
        ma.As().effect.Run(env).As();
    
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
    public static Eff<RT, A> Flatten<RT, A>(this Eff<RT, K<Eff<RT>, A>> mma) =>
        mma.Bind(ma => ma);
    

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  SelectMany extensions
    //

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<RT, D> SelectMany<RT, A, B, C, D>(
        this (K<Eff<RT>, A> First, K<Eff<RT>, B> Second) self,
        Func<(A First, B Second), K<Eff<RT>, C>> bind,
        Func<(A First, B Second), C, D> project) =>
        self.Zip().Bind(ab => bind(ab).Map(c => project(ab, c)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<RT, D> SelectMany<RT, A, B, C, D>(
        this K<Eff<RT>, A> self,
        Func<A, (K<Eff<RT>, B> First, K<Eff<RT>, C> Second)> bind,
        Func<A, (B First, C Second), D> project) =>
        self.As().Bind(a => bind(a).Zip().Map(cd => project(a, cd)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<RT, E> SelectMany<RT, A, B, C, D, E>(
        this (K<Eff<RT>, A> First, K<Eff<RT>, B> Second, K<Eff<RT>, C> Third) self,
        Func<(A First, B Second, C Third), K<Eff<RT>, D>> bind,
        Func<(A First, B Second, C Third), D, E> project) =>
        self.Zip().Bind(ab => bind(ab).Map(c => project(ab, c)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<RT, E> SelectMany<RT, A, B, C, D, E>(
        this K<Eff<RT>, A> self,
        Func<A, (K<Eff<RT>, B> First, K<Eff<RT>, C> Second, K<Eff<RT>, D> Third)> bind,
        Func<A, (B First, C Second, D Third), E> project) =>
        self.As().Bind(a => bind(a).Zip().Map(cd => project(a, cd)));
    
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
        this (K<Eff<RT>, A> First, K<Eff<RT>, B> Second) tuple) =>
        from e1 in tuple.First.As().Fork()
        from e2 in tuple.Second.As().Fork()
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
        this (K<Eff<RT>, A> First,
              K<Eff<RT>, B> Second,
              K<Eff<RT>, C> Third) tuple) =>
        from e1 in tuple.First.As().Fork()
        from e2 in tuple.Second.As().Fork()
        from e3 in tuple.Third.As().Fork()
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
        this (K<Eff<RT>, A> First, 
              K<Eff<RT>, B> Second, 
              K<Eff<RT>, C> Third, 
              K<Eff<RT>, D> Fourth) tuple) =>
        from e1 in tuple.First.As().Fork()
        from e2 in tuple.Second.As().Fork()
        from e3 in tuple.Third.As().Fork()
        from e4 in tuple.Fourth.As().Fork()
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
        this K<Eff<RT>, A> First,
        K<Eff<RT>, B> Second) =>
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
        this K<Eff<RT>, A> First, 
        K<Eff<RT>, B> Second, 
        K<Eff<RT>, C> Third) =>
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
        this K<Eff<RT>, A> First, 
        K<Eff<RT>, B> Second, 
        K<Eff<RT>, C> Third, 
        K<Eff<RT>, D> Fourth) =>
        (First, Second, Third, Fourth).Zip();
}
