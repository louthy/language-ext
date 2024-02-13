using System;
using LanguageExt.Common;
using LanguageExt.Effects;
using LanguageExt.HKT;

namespace LanguageExt;

public static partial class EffExtensions
{
    /// <summary>
    /// Cast type to its Kind
    /// </summary>
    public static Eff<A> As<A>(this K<Eff, A> ma) =>
        (Eff<A>)ma;

    
    /// <summary>
    /// Lift transducer into an effect monad
    /// </summary>
    public static Eff<A> ToEff<A>(this Transducer<Unit, Sum<Error, A>> t) =>
        new(t);

    /// <summary>
    /// Lift transducer into an effect monad
    /// </summary>
    public static Eff<A> ToEff<A>(this Transducer<Unit, A> t) =>
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
    /// <typeparam name="A">Bound value</typeparam>
    /// <returns>Flattened IO monad</returns>
    public static Eff<A> Flatten<A>(this Eff<Eff<A>> mma) =>
        mma.Bind(ma => ma);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Binding extensions
    //

    public static Eff<B> Bind<A, B>(this Transducer<Unit, A> ma, Func<A, Eff<B>> f) =>
        Eff<A>.Lift(Transducer.compose(Transducer.constant<MinRT, Unit>(default), ma.Morphism)).Bind(f);

    public static Eff<B> Bind<A, B>(this Transducer<Unit, Sum<Error, A>> ma, Func<A, Eff<B>> f) =>
        Eff<A>.Lift(Transducer.compose(Transducer.constant<MinRT, Unit>(default), ma.Morphism)).Bind(f);
    
    public static Eff<B> Bind<A, B>(this Transducer<MinRT, A> ma, Func<A, Eff<B>> f) =>
        Eff<A>.Lift(ma.Morphism).Bind(f);
    
    public static Eff<B> Bind<A, B>(this Transducer<MinRT, Sum<Error, A>> ma, Func<A, Eff<B>> f) =>
        Eff<A>.Lift(ma.Morphism).Bind(f);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  SelectMany extensions
    //

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<D> SelectMany<A, B, C, D>(
        this (Eff<A> First, Eff<B> Second) self,
        Func<(A First, B Second), Eff<C>> bind,
        Func<(A First, B Second), C, D> project) =>
        self.Zip().Bind(ab => bind(ab).Map(c => project(ab, c)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<D> SelectMany<A, B, C, D>(
        this Eff<A> self,
        Func<A, (Eff<B> First, Eff<C> Second)> bind,
        Func<A, (B First, C Second), D> project) =>
        self.Bind(a => bind(a).Zip().Map(cd => project(a, cd)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<E> SelectMany<A, B, C, D, E>(
        this (Eff<A> First, Eff<B> Second, Eff<C> Third) self,
        Func<(A First, B Second, C Third), Eff<D>> bind,
        Func<(A First, B Second, C Third), D, E> project) =>
        self.Zip().Bind(ab => bind(ab).Map(c => project(ab, c)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<E> SelectMany<A, B, C, D, E>(
        this Eff<A> self,
        Func<A, (Eff<B> First, Eff<C> Second, Eff<D> Third)> bind,
        Func<A, (B First, C Second, D Third), E> project) =>
        self.Bind(a => bind(a).Zip().Map(cd => project(a, cd)));

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    public static Eff<C> SelectMany<A, B, C>(
        this Transducer<Unit, A> ma,
        Func<A, Eff<B>> bind,
        Func<A, B, C> project) =>
        Eff<A>.Lift(Transducer.compose(Transducer.constant<MinRT, Unit>(default), ma))
                    .Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    public static Eff<C> SelectMany<A, B, C>(
        this Transducer<MinRT, A> ma,
        Func<A, Eff<B>> bind,
        Func<A, B, C> project) =>
        Eff<A>.Lift(ma).Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    public static Eff<C> SelectMany<A, B, C>(
        this Transducer<Unit, Sum<Error, A>> ma,
        Func<A, Eff<B>> bind,
        Func<A, B, C> project) =>
        Eff<A>.Lift(Transducer.compose(Transducer.constant<MinRT, Unit>(default), ma))
                    .Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monadic bind and project
    /// </summary>
    public static Eff<C> SelectMany<A, B, C>(
        this Transducer<MinRT, Sum<Error, A>> ma,
        Func<A, Eff<B>> bind,
        Func<A, B, C> project) =>
        Eff<A>.Lift(ma).Bind(x => bind(x).Map(y => project(x, y)));
    
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
    public static Eff<(A First, B Second)> Zip<A, B>(
         this (Eff<A> First, Eff<B> Second) tuple) =>
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
    public static Eff<(A First, B Second, C Third)> Zip<A, B, C>(
        this (Eff<A> First, 
              Eff<B> Second, 
              Eff<C> Third) tuple) =>
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
    /// <typeparam name="C">Third IO monad bound value type</typeparam>
    /// <typeparam name="D">Fourth IO monad bound value type</typeparam>
    /// <returns>IO monad</returns>
    public static Eff<(A First, B Second, C Third, D Fourth)> Zip<A, B, C, D>(
        this (Eff<A> First,
            Eff<B> Second,
            Eff<C> Third,
            Eff<D> Fourth) tuple) =>
        new(Transducer.zip(tuple.First.Morphism, tuple.Second.Morphism, tuple.Third.Morphism, tuple.Fourth.Morphism));    

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
    public static Eff<(A First, B Second)> Zip<A, B>(
        this Eff<A> First,
        Eff<B> Second) =>
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
    public static Eff<(A First, B Second, C Third)> Zip<A, B, C>(
        this Eff<A> First, 
        Eff<B> Second, 
        Eff<C> Third) =>
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
    public static Eff<(A First, B Second, C Third, D Fourth)> Zip<A, B, C, D>(
        this Eff<A> First,
        Eff<B> Second,
        Eff<C> Third,
        Eff<D> Fourth) =>
        new(Transducer.zip(First.Morphism, Second.Morphism, Third.Morphism, Fourth.Morphism)); 
}
