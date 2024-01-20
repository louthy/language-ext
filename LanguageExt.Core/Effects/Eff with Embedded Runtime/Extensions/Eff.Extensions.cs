#nullable enable
using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt;

public static partial class EffExtensions
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
    /// <typeparam name="A">Bound value</typeparam>
    /// <returns>Flattened IO monad</returns>
    public static Eff<RT, A> Flatten<RT, A>(this Eff<RT, Eff<RT, A>> mma)
        where RT : struct, HasIO<RT, Error> =>
        mma.Bind(ma => ma);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Binding extensions
    //

    public static Eff<RT, B> Bind<RT, A, B>(this Transducer<Unit, A> ma, Func<A, Eff<RT, B>> f)
        where RT : struct, HasIO<RT, Error> =>
        Eff<RT, A>.Lift(Transducer.compose(Transducer.constant<RT, Unit>(default), ma.Morphism)).Bind(f);

    public static Eff<RT, B> Bind<RT, A, B>(this Transducer<Unit, Sum<Error, A>> ma, Func<A, Eff<RT, B>> f)
        where RT : struct, HasIO<RT, Error> =>
        Eff<RT, A>.Lift(Transducer.compose(Transducer.constant<RT, Unit>(default), ma.Morphism)).Bind(f);
    
    public static Eff<RT, B> Bind<RT, A, B>(this Transducer<RT, A> ma, Func<A, Eff<RT, B>> f)
        where RT : struct, HasIO<RT, Error> =>
        Eff<RT, A>.Lift(ma.Morphism).Bind(f);
    
    public static Eff<RT, B> Bind<RT, A, B>(this Transducer<RT, Sum<Error, A>> ma, Func<A, Eff<RT, B>> f)
        where RT : struct, HasIO<RT, Error> =>
        Eff<RT, A>.Lift(ma.Morphism).Bind(f);

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
        where RT : struct, HasIO<RT, Error> =>
        self.Zip().Bind(ab => bind(ab).Map(c => project(ab, c)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<RT, D> SelectMany<RT, A, B, C, D>(
        this Eff<RT, A> self,
        Func<A, (Eff<RT, B> First, Eff<RT, C> Second)> bind,
        Func<A, (B First, C Second), D> project)
        where RT : struct, HasIO<RT, Error> =>
        self.Bind(a => bind(a).Zip().Map(cd => project(a, cd)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<RT, E> SelectMany<RT, A, B, C, D, E>(
        this (Eff<RT, A> First, Eff<RT, B> Second, Eff<RT, C> Third) self,
        Func<(A First, B Second, C Third), Eff<RT, D>> bind,
        Func<(A First, B Second, C Third), D, E> project)
        where RT : struct, HasIO<RT, Error> =>
        self.Zip().Bind(ab => bind(ab).Map(c => project(ab, c)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<RT, E> SelectMany<RT, A, B, C, D, E>(
        this Eff<RT, A> self,
        Func<A, (Eff<RT, B> First, Eff<RT, C> Second, Eff<RT, D> Third)> bind,
        Func<A, (B First, C Second, D Third), E> project)
        where RT : struct, HasIO<RT, Error> =>
        self.Bind(a => bind(a).Zip().Map(cd => project(a, cd)));

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    public static Eff<RT, C> SelectMany<RT, A, B, C>(
        this Transducer<Unit, A> ma,
        Func<A, Eff<RT, B>> bind,
        Func<A, B, C> project)
        where RT : struct, HasIO<RT, Error> =>
        Eff<RT, A>.Lift(Transducer.compose(Transducer.constant<RT, Unit>(default), ma))
                    .Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    public static Eff<RT, C> SelectMany<RT, A, B, C>(
        this Transducer<RT, A> ma,
        Func<A, Eff<RT, B>> bind,
        Func<A, B, C> project)
        where RT : struct, HasIO<RT, Error> =>
        Eff<RT, A>.Lift(ma).Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    public static Eff<RT, C> SelectMany<RT, A, B, C>(
        this Transducer<Unit, Sum<Error, A>> ma,
        Func<A, Eff<RT, B>> bind,
        Func<A, B, C> project)
        where RT : struct, HasIO<RT, Error> =>
        Eff<RT, A>.Lift(Transducer.compose(Transducer.constant<RT, Unit>(default), ma))
                    .Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    public static Eff<RT, C> SelectMany<RT, A, B, C>(
        this Transducer<RT, Sum<Error, A>> ma,
        Func<A, Eff<RT, B>> bind,
        Func<A, B, C> project)
        where RT : struct, HasIO<RT, Error> =>
        Eff<RT, A>.Lift(ma).Bind(x => bind(x).Map(y => project(x, y)));
    
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
         where RT : struct, HasIO<RT, Error> =>
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
    public static Eff<RT, (A First, B Second, C Third)> Zip<RT, A, B, C>(
        this (Eff<RT, A> First, 
              Eff<RT, B> Second, 
              Eff<RT, C> Third) tuple)
        where RT : struct, HasIO<RT, Error> =>
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
    public static Eff<RT, (A First, B Second)> Zip<RT, A, B>(
        this Eff<RT, A> First,
        Eff<RT, B> Second)
        where RT : struct, HasIO<RT, Error> =>
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
        where RT : struct, HasIO<RT, Error> =>
        (First, Second, Third).Zip();
}
