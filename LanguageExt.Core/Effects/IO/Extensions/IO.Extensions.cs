#nullable enable
using System;
using LanguageExt.Effects.Traits;
using LanguageExt.Transducers;

namespace LanguageExt;

public static partial class IOExtensions
{
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
    /// <typeparam name="E">Error type</typeparam>
    /// <typeparam name="A">Bound value</typeparam>
    /// <returns>Flattened IO monad</returns>
    public static IO<RT, E, A> Flatten<RT, E, A>(this IO<RT, E, IO<RT, E, A>> mma)
        where RT : struct, HasIO<RT, E> =>
        new(mma.Bind(ma => ma));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Binding extensions
    //

    public static IO<RT, E, B> Bind<RT, E, A, B>(this Fold<RT, A> ma, Func<A, IO<RT, E, B>> f)
        where RT : struct, HasIO<RT, E> =>
        IO<RT, E, A>.Lift(ma.ToTransducer()).Bind(f);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  SelectMany extensions
    //

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static IO<RT, E, D> SelectMany<RT, E, A, B, C, D>(
        this (IO<RT, E, A> First, IO<RT, E, B> Second) self,
        Func<(A First, B Second), IO<RT, E, C>> bind,
        Func<(A First, B Second), C, D> project)
        where RT : struct, HasIO<RT, E> =>
        self.Zip().Bind(ab => bind(ab).Map(c => project(ab, c)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static IO<RT, E, D> SelectMany<RT, E, A, B, C, D>(
        this IO<RT, E, A> self,
        Func<A, (IO<RT, E, B> First, IO<RT, E, C> Second)> bind,
        Func<A, (B First, C Second), D> project)
        where RT : struct, HasIO<RT, E> =>
        self.Bind(a => bind(a).Zip().Map(cd => project(a, cd)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static IO<RT, Err, E> SelectMany<RT, Err, A, B, C, D, E>(
        this (IO<RT, Err, A> First, IO<RT, Err, B> Second, IO<RT, Err, C> Third) self,
        Func<(A First, B Second, C Third), IO<RT, Err, D>> bind,
        Func<(A First, B Second, C Third), D, E> project)
        where RT : struct, HasIO<RT, Err> =>
        self.Zip().Bind(ab => bind(ab).Map(c => project(ab, c)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static IO<RT, Err, E> SelectMany<RT, Err, A, B, C, D, E>(
        this IO<RT, Err, A> self,
        Func<A, (IO<RT, Err, B> First, IO<RT, Err, C> Second, IO<RT, Err, D> Third)> bind,
        Func<A, (B First, C Second, D Third), E> project)
        where RT : struct, HasIO<RT, Err> =>
        self.Bind(a => bind(a).Zip().Map(cd => project(a, cd)));

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    public static IO<RT, E, C> SelectMany<RT, E, A, B, C>(
        this Fold<RT, A> ma,
        Func<A, IO<RT, E, B>> bind,
        Func<A, B, C> project)
        where RT : struct, HasIO<RT, E> =>
        ma.Bind(x => bind(x).Map(y => project(x, y)));
    
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
    public static IO<RT, E, (A First, B Second)> Zip<RT, E, A, B>(
         this (IO<RT, E, A> First, IO<RT, E, B> Second) tuple)
         where RT : struct, HasIO<RT, E> =>
         new(Transducer.zip(tuple.First.Morphism, tuple.Second.Morphism));

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
    public static IO<RT, E, (A First, B Second, C Third)> Zip<RT, E, A, B, C>(
        this (IO<RT, E, A> First, 
              IO<RT, E, B> Second, 
              IO<RT, E, C> Third) tuple)
        where RT : struct, HasIO<RT, E> =>
        new(Transducer.zip(tuple.First.Morphism, tuple.Second.Morphism, tuple.Third.Morphism));

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
    public static IO<RT, E, (A First, B Second)> Zip<RT, E, A, B>(
        this IO<RT, E, A> First,
        IO<RT, E, B> Second)
        where RT : struct, HasIO<RT, E> =>
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
    public static IO<RT, E, (A First, B Second, C Third)> Zip<RT, E, A, B, C>(
        this IO<RT, E, A> First, 
        IO<RT, E, B> Second, 
        IO<RT, E, C> Third)
        where RT : struct, HasIO<RT, E> =>
        (First, Second, Third).Zip();
}
