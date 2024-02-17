/*
using System;
using LanguageExt.Effects;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IOExtensions
{
    public static IO<E, A> As<E, A>(this K<IO<E>, A> ma) =>
        (IO<E, A>)ma;
    
    /// <summary>
    /// Lift transducer into an effect monad
    /// </summary>
    public static IO<E, A> ToIO<E, A>(this Transducer<Unit, Sum<E, A>> t) =>
        new (t);

    /// <summary>
    /// Lift transducer in an effect monad
    /// </summary>
    public static IO<E, A> ToIO<E, A>(this Transducer<Unit, A> t) =>
        new(t);
    
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
    public static IO<E, A> Flatten<E, A>(this IO<E, IO<E, A>> mma) =>
        mma.Bind(ma => ma);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Binding extensions
    //

    public static IO<E, B> Bind<E, A, B>(this Transducer<Unit, A> ma, Func<A, IO<E, B>> f) =>
        IO<E, A>.Lift(Transducer.compose(Transducer.constant<MinRT<E>, Unit>(default), ma.Morphism)).Bind(f);

    public static IO<E, B> Bind<E, A, B>(this Transducer<Unit, Sum<E, A>> ma, Func<A, IO<E, B>> f) =>
        IO<E, A>.Lift(Transducer.compose(Transducer.constant<MinRT<E>, Unit>(default), ma.Morphism)).Bind(f);

    public static IO<E, B> Bind<E, A, B>(this Transducer<MinRT<E>, A> ma, Func<A, IO<E, B>> f) =>
        IO<E, A>.Lift(ma.Morphism).Bind(f);

    public static IO<E, B> Bind<E, A, B>(this Transducer<MinRT<E>, Sum<E, A>> ma, Func<A, IO<E, B>> f) =>
        IO<E, A>.Lift(ma.Morphism).Bind(f);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  SelectMany extensions
    //

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static IO<E, D> SelectMany<E, A, B, C, D>(
        this (IO<E, A> First, IO<E, B> Second) self,
        Func<(A First, B Second), IO<E, C>> bind,
        Func<(A First, B Second), C, D> project) =>
        self.Zip().Bind(ab => bind(ab).Map(c => project(ab, c)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static IO<E, D> SelectMany<E, A, B, C, D>(
        this IO<E, A> self,
        Func<A, (IO<E, B> First, IO<E, C> Second)> bind,
        Func<A, (B First, C Second), D> project) =>
        self.Bind(a => bind(a).Zip().Map(cd => project(a, cd)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static IO<Err, E> SelectMany<Err, A, B, C, D, E>(
        this (IO<Err, A> First, IO<Err, B> Second, IO<Err, C> Third) self,
        Func<(A First, B Second, C Third), IO<Err, D>> bind,
        Func<(A First, B Second, C Third), D, E> project) =>
        self.Zip().Bind(ab => bind(ab).Map(c => project(ab, c)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static IO<Err, E> SelectMany<Err, A, B, C, D, E>(
        this IO<Err, A> self,
        Func<A, (IO<Err, B> First, IO<Err, C> Second, IO<Err, D> Third)> bind,
        Func<A, (B First, C Second, D Third), E> project) =>
        self.Bind(a => bind(a).Zip().Map(cd => project(a, cd)));

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    public static IO<E, C> SelectMany<E, A, B, C>(
        this Transducer<Unit, A> ma,
        Func<A, IO<E, B>> bind,
        Func<A, B, C> project) =>
        IO<E, A>.Lift(Transducer.compose(Transducer.constant<MinRT<E>, Unit>(default), ma))
                .Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    public static IO<E, C> SelectMany<E, A, B, C>(
        this Transducer<MinRT<E>, A> ma,
        Func<A, IO<E, B>> bind,
        Func<A, B, C> project) =>
        IO<E, A>.Lift(ma).Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    public static IO<E, C> SelectMany<E, A, B, C>(
        this Transducer<Unit, Sum<E, A>> ma,
        Func<A, IO<E, B>> bind,
        Func<A, B, C> project) =>
        IO<E, A>.Lift(Transducer.compose(Transducer.constant<MinRT<E>, Unit>(default), ma))
                .Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    public static IO<E, C> SelectMany<E, A, B, C>(
        this Transducer<MinRT<E>, Sum<E, A>> ma,
        Func<A, IO<E, B>> bind,
        Func<A, B, C> project) =>
        IO<E, A>.Lift(ma).Bind(x => bind(x).Map(y => project(x, y)));

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
    public static IO<E, (A First, B Second)> Zip<E, A, B>(
        this (IO<E, A> First, IO<E, B> Second) tuple) =>
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
    public static IO<E, (A First, B Second, C Third)> Zip<E, A, B, C>(
        this (IO<E, A> First,
            IO<E, B> Second,
            IO<E, C> Third) tuple) =>
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
    public static IO<E, (A First, B Second)> Zip<E, A, B>(
        this IO<E, A> First,
        IO<E, B> Second) =>
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
    public static IO<E, (A First, B Second, C Third)> Zip<E, A, B, C>(
        this IO<E, A> First,
        IO<E, B> Second,
        IO<E, C> Third) =>
        (First, Second, Third).Zip();
}
*/
