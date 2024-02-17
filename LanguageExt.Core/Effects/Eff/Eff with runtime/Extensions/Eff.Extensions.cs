using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EffExtensions
{
    /// <summary>
    /// Cast type to its Kind
    /// </summary>
    public static Eff<RT, A> As<RT, A>(this K<Eff.Runtime<RT>, A> ma)
        where RT : HasIO<RT, Error> =>
        (Eff<RT, A>)ma;
    
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
    public static Eff<RT, A> Flatten<RT, A>(this Eff<RT, Eff<RT, A>> mma)
        where RT : HasIO<RT, Error> =>
        mma.Bind(ma => ma);
    

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  SelectMany extensions
    //

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<RT, D> SelectMany<RT, A, B, C, D>(
        this (Eff<RT, A> First, Eff<RT, B> Second) self,
        Func<(A First, B Second), Eff<RT, C>> bind,
        Func<(A First, B Second), C, D> project)
        where RT : HasIO<RT, Error> =>
        self.Zip().Bind(ab => bind(ab).Map(c => project(ab, c)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<RT, D> SelectMany<RT, A, B, C, D>(
        this Eff<RT, A> self,
        Func<A, (Eff<RT, B> First, Eff<RT, C> Second)> bind,
        Func<A, (B First, C Second), D> project)
        where RT : HasIO<RT, Error> =>
        self.Bind(a => bind(a).Zip().Map(cd => project(a, cd)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<RT, E> SelectMany<RT, A, B, C, D, E>(
        this (Eff<RT, A> First, Eff<RT, B> Second, Eff<RT, C> Third) self,
        Func<(A First, B Second, C Third), Eff<RT, D>> bind,
        Func<(A First, B Second, C Third), D, E> project)
        where RT : HasIO<RT, Error> =>
        self.Zip().Bind(ab => bind(ab).Map(c => project(ab, c)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<RT, E> SelectMany<RT, A, B, C, D, E>(
        this Eff<RT, A> self,
        Func<A, (Eff<RT, B> First, Eff<RT, C> Second, Eff<RT, D> Third)> bind,
        Func<A, (B First, C Second, D Third), E> project)
        where RT : HasIO<RT, Error> =>
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
        this (Eff<RT, A> First, Eff<RT, B> Second) tuple)
        where RT : HasIO<RT, Error> =>
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
              Eff<RT, C> Third) tuple)
        where RT : HasIO<RT, Error> =>
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
              Eff<RT, D> Fourth) tuple)
        where RT : HasIO<RT, Error> =>
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
        Eff<RT, B> Second)
        where RT : HasIO<RT, Error> =>
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
        Eff<RT, C> Third)
        where RT : HasIO<RT, Error> =>
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
        Eff<RT, D> Fourth)
        where RT : HasIO<RT, Error> =>
        (First, Second, Third, Fourth).Zip();
}
