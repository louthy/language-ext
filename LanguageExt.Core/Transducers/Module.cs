#nullable enable

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class Transducer
{
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Constants
    //
    
    /// <summary>
    /// Lift a value into the `Transducer` space
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>`Transducer` from `Unit` to `A`</returns>
    public static Transducer<Unit, A> pure<A>(A value) =>
        constant<Unit, A>(value);

    /// <summary>
    /// Fail transducer
    /// </summary>
    /// <remarks>
    /// Consider this the `throw` of transducers.  It is the nuclear option to get out of a reduce.
    /// </remarks>
    /// <param name="error">Error to raise</param>
    /// <returns>A transducer that always fails</returns>
    public static Transducer<A, B> fail<A, B>(Error error) =>
        new FailTransducer<A, B>(error);

    /// <summary>
    /// Constant transducer
    /// </summary>
    /// <remarks>
    /// Takes any value, ignores it and yields the value provided.
    /// </remarks>
    /// <param name="value">Constant value to yield</param>
    /// <typeparam name="A">Input value type</typeparam>
    /// <typeparam name="B">Constant value type</typeparam>
    /// <returns>`Transducer` from `A` to `B`</returns>
    public static Transducer<A, B> constant<A, B>(B value) =>
        new ConstantTransducer<A, B>(value);

    /// <summary>
    /// Lifts a unit accepting transducer, ignores the input value.
    /// </summary>
    public static Transducer<A, B> ignore<A, B>(Transducer<Unit, B> m) =>
        new IgnoreTransducer<A, B>(m);

    /// <summary>
    /// Make a tuple from a value
    /// </summary>
    public static Transducer<A, (A, A)> mkPair<A>() =>
        lift((A a) => (a, a));

    /// <summary>
    /// Make a `Sum.Right` from a value
    /// </summary>
    public static Transducer<A, Sum<X, A>> mkRight<X, A>() =>
        lift((A a) => Sum<X, A>.Right(a));

    /// <summary>
    /// Make a `Sum.Left` from a value
    /// </summary>
    public static Transducer<X, Sum<X, A>> mkLeft<X, A>() =>
        lift((X x) => Sum<X, A>.Left(x));

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Guards
    //
    
    /// <summary>
    /// Guard transducer 
    /// </summary>
    public static Transducer<Guard<E, Unit>, Sum<E, Unit>> guard<E>() =>
        GuardTransducer<E, Unit>.Default;

    /// <summary>
    /// Guard transducer 
    /// </summary>
    public static Transducer<Guard<E, A>, Sum<E, Unit>> guard<E, A>() =>
        GuardTransducer<E, A>.Default;
    
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Resource use
    //
    
    /// <summary>
    /// Resource tracking transducer
    /// </summary>
    public static Transducer<A, B> use<A, B>(Transducer<A, B> transducer, Func<B, Unit> dispose) =>
        new UseTransducer1<A, B>(transducer, dispose);
    
    /// <summary>
    /// Resource tracking transducer
    /// </summary>
    public static Transducer<A, B> use<A, B>(Transducer<A, B> transducer) where B : IDisposable =>
        new UseTransducer2<A, B>(transducer);
    
    /// <summary>
    /// Resource releasing transducer
    /// </summary>
    public static Transducer<A, Unit> release<A>() =>
        ReleaseTransducer<A>.Default;

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Streaming
    //
    
    /// <summary>
    /// Stream the items in the enumerable through the transducer
    /// </summary>
    /// <param name="items">Items to stream</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Transducer that represents the stream</returns>
    public static Transducer<IEnumerable<A>, A> enumerable<A>() =>
        StreamEnumerableTransducer<A>.Default;

    /// <summary>
    /// Stream the items in the `Seq` through the transducer
    /// </summary>
    /// <param name="items">Items to stream</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Transducer that represents the stream</returns>
    public static Transducer<Seq<A>, A> seq<A>() =>
        StreamSeqTransducer<A>.Default;

    /// <summary>
    /// Stream the items in the `IAsyncEnumerable` through the transducer
    /// </summary>
    /// <param name="items">Items to stream</param>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>Transducer that represents the stream</returns>
    public static Transducer<IAsyncEnumerable<A>, A> asyncEnumerable<A>() =>
        StreamAsyncEnumerableTransducer<A>.Default;

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Identity
    //
    
    /// <summary>
    /// Identity transducer
    /// </summary>
    public static Transducer<A, A> identity<A>() =>
        IdentityTransducer<A>.Default;
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Composition
    //
    
    /// <summary>
    /// Transducer composition.  The output of one transducer is fed as the input to the next.
    ///
    /// Resulting im a single transducer that captures the composition
    /// </summary>
    /// <returns>Transducer that captures the composition</returns>
    public static Transducer<A, C> compose<A, B, C>(
        Transducer<A, B> f, 
        Transducer<B, C> g) =>
        f.Compose(g);
    
    /// <summary>
    /// Transducer composition.  The output of one transducer is fed as the input to the next.
    ///
    /// Resulting im a single transducer that captures the composition
    /// </summary>
    /// <returns>Transducer that captures the composition</returns>
    public static Transducer<A, D> compose<A, B, C, D>(
        Transducer<A, B> f, 
        Transducer<B, C> g, 
        Transducer<C, D> h) =>
        f.Compose(g)
         .Compose(h);
    
    /// <summary>
    /// Transducer composition.  The output of one transducer is fed as the input to the next.
    ///
    /// Resulting im a single transducer that captures the composition
    /// </summary>
    /// <returns>Transducer that captures the composition</returns>
    public static Transducer<A, E> compose<A, B, C, D, E>(
        Transducer<A, B> f, 
        Transducer<B, C> g, 
        Transducer<C, D> h,
        Transducer<D, E> i) =>
        f.Compose(g)
         .Compose(h)
         .Compose(i);
    
    /// <summary>
    /// Transducer composition.  The output of one transducer is fed as the input to the next.
    ///
    /// Resulting im a single transducer that captures the composition
    /// </summary>
    /// <returns>Transducer that captures the composition</returns>
    public static Transducer<A, F> compose<A, B, C, D, E, F>(
        Transducer<A, B> f, 
        Transducer<B, C> g, 
        Transducer<C, D> h,
        Transducer<D, E> i,
        Transducer<E, F> j) =>
        f.Compose(g)
         .Compose(h)
         .Compose(i)
         .Compose(j);
    
    /// <summary>
    /// Transducer composition.  The output of one transducer is fed as the input to the next.
    ///
    /// Resulting im a single transducer that captures the composition
    /// </summary>
    /// <returns>Transducer that captures the composition</returns>
    public static Transducer<A, G> compose<A, B, C, D, E, F, G>(
        Transducer<A, B> f, 
        Transducer<B, C> g, 
        Transducer<C, D> h,
        Transducer<D, E> i,
        Transducer<E, F> j,
        Transducer<F, G> k) =>
        f.Compose(g)
         .Compose(h)
         .Compose(i)
         .Compose(j)
         .Compose(k);
    
    /// <summary>
    /// Transducer composition.  The output of one transducer is fed as the input to the next.
    ///
    /// Resulting im a single transducer that captures the composition
    /// </summary>
    /// <returns>Transducer that captures the composition</returns>
    public static Transducer<A, H> compose<A, B, C, D, E, F, G, H>(
        Transducer<A, B> f, 
        Transducer<B, C> g, 
        Transducer<C, D> h,
        Transducer<D, E> i,
        Transducer<E, F> j,
        Transducer<F, G> k,
        Transducer<G, H> l) =>
        f.Compose(g)
         .Compose(h)
         .Compose(i)
         .Compose(j)
         .Compose(k)
         .Compose(l);
    
    /// <summary>
    /// Transducer composition.  The output of one transducer is fed as the input to the next.
    ///
    /// Resulting im a single transducer that captures the composition
    /// </summary>
    /// <returns>Transducer that captures the composition</returns>
    public static Transducer<A, I> compose<A, B, C, D, E, F, G, H, I>(
        Transducer<A, B> f, 
        Transducer<B, C> g, 
        Transducer<C, D> h,
        Transducer<D, E> i,
        Transducer<E, F> j,
        Transducer<F, G> k,
        Transducer<G, H> l,
        Transducer<H, I> m) =>
        f.Compose(g)
         .Compose(h)
         .Compose(i)
         .Compose(j)
         .Compose(k)
         .Compose(l)
         .Compose(m);
    
    /// <summary>
    /// Transducer composition.  The output of one transducer is fed as the input to the next.
    ///
    /// Resulting im a single transducer that captures the composition
    /// </summary>
    /// <returns>Transducer that captures the composition</returns>
    public static Transducer<A, J> compose<A, B, C, D, E, F, G, H, I, J>(
        Transducer<A, B> f, 
        Transducer<B, C> g, 
        Transducer<C, D> h,
        Transducer<D, E> i,
        Transducer<E, F> j,
        Transducer<F, G> k,
        Transducer<G, H> l,
        Transducer<H, I> m,
        Transducer<I, J> n) =>
        f.Compose(g)
         .Compose(h)
         .Compose(i)
         .Compose(j)
         .Compose(k)
         .Compose(l)
         .Compose(m)
         .Compose(n);
    

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Applicatives
    //
    
    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Gets a lifted function and a lifted argument, invokes the function with the argument and re-lifts the result.
    /// </remarks>
    /// <returns>Result of applying the function to the argument</returns>
    public static Transducer<E, B> apply<E, A, B>(
        Transducer<E, Func<A, B>> ff,
        Transducer<E, A> fa) =>
        new ApplyTransducer<E, A, B>(ff, fa);

    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Gets a lifted function and a lifted argument, invokes the function with the argument and re-lifts the result.
    /// </remarks>
    /// <returns>Result of applying the function to the argument</returns>
    public static Transducer<E, B> apply<E, A, B>(
        Transducer<E, Transducer<A, B>> ff,
        Transducer<E, A> fa) =>
        new ApplyTransducer2<E, A, B>(ff, fa);

    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Gets a lifted function and a lifted argument, invokes the function with the argument and re-lifts the result.
    /// </remarks>
    /// <returns>Result of applying the function to the argument</returns>
    public static Transducer<E, Sum<X, B>> apply<E, X, A, B>(
        Transducer<E, Sum<X, Func<A, B>>> ff,
        Transducer<E, Sum<X, A>> fa) =>
        new ApplySumTransducer<E, X, A, B>(ff, fa);

    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Gets a lifted function and a lifted argument, invokes the function with the argument and re-lifts the result.
    /// </remarks>
    /// <returns>Result of applying the function to the argument</returns>
    public static Transducer<E, Sum<X, B>> apply<E, X, A, B>(
        Transducer<E, Sum<X, Transducer<A, B>>> ff,
        Transducer<E, Sum<X, A>> fa) =>
        new ApplySumTransducer2<E, X, A, B>(ff, fa);

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Functor and bi-functor mapping
    //
    
    /// <summary>
    /// Functor bi-map
    /// </summary>
    /// <param name="Left">Left mapping transducer</param>
    /// <param name="Right">Right mapping transducer</param>
    /// <returns>Composition of first, left, and right transducers</returns>
    public static Transducer<Sum<X, A>, Sum<Y, B>> bimap<X, Y, A, B>(
        Transducer<X, Y> Left,
        Transducer<A, B> Right) =>
        new BiMapSum<X, Y, A, B>(Left, Right);

    /// <summary>
    /// Functor bi-map
    /// </summary>
    /// <param name="Left">Left mapping transducer</param>
    /// <param name="Right">Right mapping transducer</param>
    /// <returns>Composition of first, left, and right transducers</returns>
    public static Transducer<(X, A), (Y, B)> bimapPair<X, Y, A, B>(
        Transducer<X, Y> Left,
        Transducer<A, B> Right) =>
        new BiMapProduct<X, Y, A, B>(Left, Right);

    /// <summary>
    /// Functor bi-map
    /// </summary>
    /// <param name="Left">Left mapping transducer</param>
    /// <param name="Right">Right mapping transducer</param>
    /// <returns>Composition of first, left, and right transducers</returns>
    public static Transducer<Sum<X, A>, Sum<Y, B>> bimap<X, Y, A, B>(
        Func<X, Y> Left,
        Func<A, B> Right) =>
        new BiMapSum<X, Y, A, B>(lift(Left), lift(Right));

    /// <summary>
    /// Functor bi-map
    /// </summary>
    /// <param name="Left">Left mapping transducer</param>
    /// <param name="Right">Right mapping transducer</param>
    /// <returns>Composition of first, left, and right transducers</returns>
    public static Transducer<(X, A), (Y, B)> bimapPair<X, Y, A, B>(
        Func<X, Y> Left,
        Func<A, B> Right) =>
        new BiMapProduct<X, Y, A, B>(lift(Left), lift(Right));

    /// <summary>
    /// Functor bi-map
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Left">Left mapping transducer</param>
    /// <param name="Right">Right mapping transducer</param>
    /// <returns>Composition of first, left, and right transducers</returns>
    public static Transducer<E, Sum<Y, B>> bimap<E, X, Y, A, B>(
        Transducer<E, Sum<X, A>> First,
        Transducer<X, Y> Left,
        Transducer<A, B> Right) =>
        compose(First, bimap(Left, Right));

    /// <summary>
    /// Functor bi-map
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Left">Left mapping transducer</param>
    /// <param name="Right">Right mapping transducer</param>
    /// <returns>Composition of first, left, and right transducers</returns>
    public static Transducer<E, Sum<Y, B>> bimap<E, X, Y, A, B>(
        Transducer<E, Sum<X, A>> First,
        Func<X, Y> Left,
        Func<A, B> Right) =>
        compose(First, bimap(Left, Right));

    /// <summary>
    /// Functor bi-map
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Left">Left mapping transducer</param>
    /// <param name="Right">Right mapping transducer</param>
    /// <returns>Composition of first, left, and right transducers</returns>
    public static Transducer<Sum<X, A>, Sum<Z, C>> bimap<X, Y, Z, A, B, C>(
        Transducer<Sum<X, A>, Sum<Y, B>> First,
        Transducer<Y, Z> Left,
        Transducer<B, C> Right) =>
        compose(First, bimap(Left, Right));

    /// <summary>
    /// Functor bi-map
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Left">Left mapping transducer</param>
    /// <param name="Right">Right mapping transducer</param>
    /// <returns>Composition of first, left, and right transducers</returns>
    public static Transducer<Sum<X, A>, Sum<Z, C>> bimap<X, Y, Z, A, B, C>(
        Transducer<Sum<X, A>, Sum<Y, B>> First,
        Func<Y, Z> Left,
        Func<B, C> Right) =>
        compose(First, bimap(Left, Right));

    /// <summary>
    /// Functor bi-map map left
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Left">Left mapping transducer</param>
    /// <returns>Composition of first and left transducers</returns>
    public static Transducer<Sum<X, A>, Sum<Y, A>> mapLeft<X, Y, A>(
        Transducer<X, Y> Left) =>
        bimap(Left, identity<A>());

    /// <summary>
    /// Functor bi-map map left
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Left">Left mapping transducer</param>
    /// <returns>Composition of first and left transducers</returns>
    public static Transducer<Sum<X, A>, Sum<Y, A>> mapLeft<X, Y, A>(
        Func<X, Y> Left) =>
        bimap(lift(Left), identity<A>());
    
    /// <summary>
    /// Functor bi-map map left
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Left">Left mapping transducer</param>
    /// <returns>Composition of first and left transducers</returns>
    public static Transducer<E, Sum<Y, A>> mapLeft<E, X, Y, A>(
        Transducer<E, Sum<X, A>> First,
        Transducer<X, Y> Left) =>
        compose(First, mapLeft<X, Y, A>(Left));

    /// <summary>
    /// Functor bi-map map left
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Left">Left mapping transducer</param>
    /// <returns>Composition of first and left transducers</returns>
    public static Transducer<E, Sum<Y, A>> mapLeft<E, X, Y, A>(
        Transducer<E, Sum<X, A>> First,
        Func<X, Y> Left) =>
        compose(First, mapLeft<X, Y, A>(Left));

    /// <summary>
    /// Functor bi-map map left
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Left">Left mapping transducer</param>
    /// <returns>Composition of first and left</returns>
    public static Transducer<Sum<X, A>, Sum<Z, B>> mapLeft<X, Y, Z, A, B>(
        Transducer<Sum<X, A>, Sum<Y, B>> First,
        Transducer<Y, Z> Left) =>
        compose(First, mapLeft<Y, Z, B>(Left));

    /// <summary>
    /// Functor bi-map map left
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Left">Left mapping transducer</param>
    /// <returns>Composition of first and left</returns>
    public static Transducer<Sum<X, A>, Sum<Z, B>> mapLeft<X, Y, Z, A, B>(
        Transducer<Sum<X, A>, Sum<Y, B>> First,
        Func<Y, Z> Left) =>
        compose(First, mapLeft<Y, Z, B>(Left));

    /// <summary>
    /// Functor bi-map map right
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Right">Right mapping transducer</param>
    /// <returns>Composition of first and right transducers</returns>
    public static Transducer<Sum<X, A>, Sum<X, B>> mapRight<X, A, B>(
        Transducer<A, B> Right) =>
        bimap(identity<X>(), Right);

    /// <summary>
    /// Functor bi-map map right
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Right">Right mapping transducer</param>
    /// <returns>Composition of first and right transducers</returns>
    public static Transducer<Sum<X, A>, Sum<X, B>> mapRight<X, A, B>(
        Func<A, B> Right) =>
        bimap(identity<X>(), lift(Right));

    /// <summary>
    /// Functor bi-map map right
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Right">Right mapping transducer</param>
    /// <returns>Composition of first and right transducers</returns>
    public static Transducer<E, Sum<X, B>> mapRight<E, X, A, B>(
        Transducer<E, Sum<X, A>> First,
        Transducer<A, B> Right) =>
        compose(First, mapRight<X, A, B>(Right));

    /// <summary>
    /// Functor bi-map map right
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Right">Right mapping transducer</param>
    /// <returns>Composition of first and right transducers</returns>
    public static Transducer<E, Sum<X, B>> mapRight<E, X, A, B>(
        Transducer<E, Sum<X, A>> First,
        Func<A, B> Right) =>
        compose(First, mapRight<X, A, B>(Right));

    /// <summary>
    /// Functor bi-map map right
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Right">Right mapping transducer</param>
    /// <returns>Composition of first and right transducers</returns>
    public static Transducer<Sum<X, A>, Sum<Y, C>> mapRight<X, Y, A, B, C>(
        Transducer<Sum<X, A>, Sum<Y, B>> First,
        Transducer<B, C> Right) =>
        compose(First, mapRight<Y, B, C>(Right));

    /// <summary>
    /// Functor bi-map map right
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Right">Right mapping transducer</param>
    /// <returns>Composition of first and right transducers</returns>
    public static Transducer<Sum<X, A>, Sum<Y, C>> mapRight<X, Y, A, B, C>(
        Transducer<Sum<X, A>, Sum<Y, B>> First,
        Func<B, C> Right) =>
        compose(First, mapRight<Y, B, C>(Right));
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Partial application
    //
    
    /// <summary>
    /// Partial application
    /// </summary>
    /// <param name="f">Transducer to partially apply</param>
    /// <param name="value">Value to apply</param>
    /// <returns>Transducer with the first argument filled</returns>
    public static Transducer<B, C> partial<A, B, C>(Transducer<A, Transducer<B, C>> f, A value) =>
        new PartialTransducer<A, B, C>(value, f);

    /// <summary>
    /// Partial application
    /// </summary>
    /// <param name="f">Transducer to partially apply</param>
    /// <param name="value">Value to apply</param>
    /// <returns>Transducer with the first argument filled</returns>
    public static Transducer<B, C> partial<A, B, C>(Transducer<A, Func<B, C>> f, A value) =>
        new PartialFTransducer<A, B, C>(value, f);

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Flatten
    //
    
    /// <summary>
    /// Take nested transducers and flatten them
    /// </summary>
    /// <param name="ff">Nested transducers</param>
    /// <returns>Flattened transducers</returns>
    public static Transducer<A, B> flatten<A, B>(Transducer<A, Transducer<A, B>> ff) =>
        new FlattenTransducer1<A, B>(ff);

    /// <summary>
    /// Take nested transducers and flatten them
    /// </summary>
    /// <param name="ff">Nested transducers</param>
    /// <returns>Flattened transducers</returns>
    public static Transducer<A, B> flatten<A, B>(Transducer<A, Transducer<Unit, B>> ff) =>
        new FlattenTransducer2<A, B>(ff);
    
    /// <summary>
    /// Take nested transducers and flatten them
    /// </summary>
    /// <param name="ff">Nested transducers</param>
    /// <returns>Flattened transducers</returns>
    public static Transducer<Env, Sum<X, A>> flatten<Env, X, A>(Transducer<Env, Sum<X, Transducer<Env, Sum<X, A>>>> ff) =>
        new FlattenSumTransducer1<Env, X, A>(ff);
    
    /// <summary>
    /// Take nested transducers and flatten them
    /// </summary>
    /// <param name="ff">Nested transducers</param>
    /// <returns>Flattened transducers</returns>
    public static Transducer<Env, Sum<X, A>> flatten<Env, X, A>(
        Transducer<Env, Sum<Transducer<Env, Sum<X, A>>, Transducer<Env, Sum<X, A>>>> ff) =>
        new FlattenSumTransducer2<Env, X, A>(ff);

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Monadic binding
    //
    
    /// <summary>
    /// Monadic bind
    /// </summary>
    public static Transducer<E, B> bind<E, A, B>(
        Transducer<E, A> m,
        Transducer<A, Transducer<E, B>> f) =>
        new BindTransducer1<E, A, B>(m, f);

    /// <summary>
    /// Monadic bind
    /// </summary>
    public static Transducer<E, B> bind<E, A, B>(
        Transducer<E, A> m,
        Transducer<A, Func<E, B>> f) =>
        new BindTransducer2<E, A, B>(m, f);

    /// <summary>
    /// Monadic bind
    /// </summary>
    public static Transducer<E, B> bind<E, A, B>(
        Transducer<E, A> m,
        Func<A, Transducer<E, B>> f) =>
        new BindTransducer3<E, A, B>(m, f);

    /// <summary>
    /// Monadic bind
    /// </summary>
    public static Transducer<E, Sum<X, B>> bind<E, X, A, B>(
        Transducer<E, Sum<X, A>> m,
        Transducer<A, Transducer<E, Sum<X, B>>> f) =>
        new BindTransducerSum<X, E, A, B>(m, f);

    /// <summary>
    /// Monadic bind
    /// </summary>
    public static Transducer<E, Sum<X, B>> bind<E, X, A, B>(
        Transducer<E, Sum<X, A>> m,
        Func<A, Transducer<E, Sum<X, B>>> f) =>
        new BindTransducerSum2<X, E, A, B>(m, f);

    /// <summary>
    /// Monadic bind
    /// </summary>
    public static Transducer<E, Sum<X, C>> selectMany<E, X, A, B, C>(
        Transducer<E, Sum<X, A>> ma,
        Func<A, Transducer<E, Sum<X, B>>> bind, 
        Func<A, B, C> project) =>
        new SelectManySumTransducer1<E, X, A, B, C>(ma, bind, project);

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Filtering
    //
    
    /// <summary>
    /// Filter the values in the transducer
    /// </summary>
    /// <param name="f">Transducer to filter</param>
    /// <param name="pred">Predicate to apply</param>
    /// <returns>Filtered transducer</returns>
    public static Transducer<A, B> filter<A, B>(Transducer<A, B> f, Transducer<B, bool> pred) =>
        new FilterTransducer1<A, B>(f, pred);

    /// <summary>
    /// Filter the values in the transducer
    /// </summary>
    /// <param name="f">Transducer to filter</param>
    /// <param name="pred">Predicate to apply</param>
    /// <returns>Filtered transducer</returns>
    public static Transducer<A, B> filter<A, B>(Transducer<A, B> f, Func<B, bool> pred) =>
        Filter(f, lift(pred));

    /// <summary>
    /// Filter the values in the transducer
    /// </summary>
    /// <param name="f">Transducer to filter</param>
    /// <param name="pred">Predicate to apply</param>
    /// <returns>Filtered transducer</returns>
    public static Transducer<A, Sum<X, B>> filter<X, A, B>(Transducer<A, Sum<X, B>> f, Transducer<B, bool> pred) =>
        new FilterSumTransducer1<X, A, B>(f, pred);

    /// <summary>
    /// Filter the values in the transducer
    /// </summary>
    /// <param name="f">Transducer to filter</param>
    /// <param name="pred">Predicate to apply</param>
    /// <returns>Filtered transducer</returns>
    public static Transducer<A, Sum<X, B>> filter<X, A, B>(Transducer<A, Sum<X, B>> f, Func<B, bool> pred) =>
        Filter(f, lift(pred));

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Folding
    //
    
    /// <summary>
    /// Fold 
    /// </summary>
    /// <param name="schedule">Series of time-spans that dictate the rate of the fold and for how many iterations</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predicate">Predicate that decides if the state should be pushed down the stream</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="A">Input value to the fold operation</typeparam>
    /// <returns>Transducer that folds the stream of values</returns>
    public static Transducer<A, S> fold<S, A>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), TResult<Unit>> predicate) =>
        new FoldTransducer<S, A>(schedule, initialState, folder, predicate);

    /// <summary>
    /// Fold 
    /// </summary>
    /// <param name="schedule">Series of time-spans that dictate the rate of the fold and for how many iterations</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predicate">Predicate that decides if the state should be pushed down the stream</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="A">Input value to the fold operation</typeparam>
    /// <returns>Transducer that folds the stream of values</returns>
    public static Transducer<A, S> foldWhile<S, A>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        fold(schedule,
             initialState,
             folder,
             s => predicate(s) ? TResult.Continue(unit) : TResult.Complete(unit));

    /// <summary>
    /// Fold 
    /// </summary>
    /// <param name="schedule">Series of time-spans that dictate the rate of the fold and for how many iterations</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predicate">Predicate that decides if the state should be pushed down the stream</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="A">Input value to the fold operation</typeparam>
    /// <returns>Transducer that folds the stream of values</returns>
    public static Transducer<A, S> foldWhile<S, A>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        foldWhile(schedule, initialState, folder, s => valueIs(s.Value));

    /// <summary>
    /// Fold 
    /// </summary>
    /// <param name="schedule">Series of time-spans that dictate the rate of the fold and for how many iterations</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predicate">Predicate that decides if the state should be pushed down the stream</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="A">Input value to the fold operation</typeparam>
    /// <returns>Transducer that folds the stream of values</returns>
    public static Transducer<A, S> foldWhile<S, A>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        foldWhile(schedule, initialState, folder, s => stateIs(s.State));

    /// <summary>
    /// Fold 
    /// </summary>
    /// <param name="schedule">Series of time-spans that dictate the rate of the fold and for how many iterations</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predicate">Predicate that decides if the state should be pushed down the stream</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="A">Input value to the fold operation</typeparam>
    /// <returns>Transducer that folds the stream of values</returns>
    public static Transducer<A, S> foldWhile<S, A>(
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        fold(Schedule.Forever,
             initialState,
             folder,
             s => predicate(s) ? TResult.Continue(unit) : TResult.Complete(unit));

    /// <summary>
    /// Fold 
    /// </summary>
    /// <param name="schedule">Series of time-spans that dictate the rate of the fold and for how many iterations</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predicate">Predicate that decides if the state should be pushed down the stream</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="A">Input value to the fold operation</typeparam>
    /// <returns>Transducer that folds the stream of values</returns>
    public static Transducer<A, S> foldWhile<S, A>(
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        foldWhile(Schedule.Forever, initialState, folder, s => valueIs(s.Value));

    /// <summary>
    /// Fold 
    /// </summary>
    /// <param name="schedule">Series of time-spans that dictate the rate of the fold and for how many iterations</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predicate">Predicate that decides if the state should be pushed down the stream</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="A">Input value to the fold operation</typeparam>
    /// <returns>Transducer that folds the stream of values</returns>
    public static Transducer<A, S> foldWhile<S, A>(
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        foldWhile(Schedule.Forever, initialState, folder, s => stateIs(s.State));    

    /// <summary>
    /// Fold 
    /// </summary>
    /// <param name="schedule">Series of time-spans that dictate the rate of the fold and for how many iterations</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predicate">Predicate that decides if the state should be pushed down the stream</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="A">Input value to the fold operation</typeparam>
    /// <returns>Transducer that folds the stream of values</returns>
    public static Transducer<A, S> foldUntil<S, A>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        foldWhile(schedule, initialState, folder, not(predicate));

    /// <summary>
    /// Fold 
    /// </summary>
    /// <param name="schedule">Series of time-spans that dictate the rate of the fold and for how many iterations</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predicate">Predicate that decides if the state should be pushed down the stream</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="A">Input value to the fold operation</typeparam>
    /// <returns>Transducer that folds the stream of values</returns>
    public static Transducer<A, S> foldUntil<S, A>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        foldWhile(schedule, initialState, folder, not(valueIs));

    /// <summary>
    /// Fold 
    /// </summary>
    /// <param name="schedule">Series of time-spans that dictate the rate of the fold and for how many iterations</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predicate">Predicate that decides if the state should be pushed down the stream</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="A">Input value to the fold operation</typeparam>
    /// <returns>Transducer that folds the stream of values</returns>
    public static Transducer<A, S> foldUntil<S, A>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        foldWhile(schedule, initialState, folder, not(stateIs));

    /// <summary>
    /// Fold 
    /// </summary>
    /// <param name="schedule">Series of time-spans that dictate the rate of the fold and for how many iterations</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predicate">Predicate that decides if the state should be pushed down the stream</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="A">Input value to the fold operation</typeparam>
    /// <returns>Transducer that folds the stream of values</returns>
    public static Transducer<A, S> foldUntil<S, A>(
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        foldWhile(Schedule.Forever, initialState, folder, not(predicate));

    /// <summary>
    /// Fold 
    /// </summary>
    /// <param name="schedule">Series of time-spans that dictate the rate of the fold and for how many iterations</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predicate">Predicate that decides if the state should be pushed down the stream</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="A">Input value to the fold operation</typeparam>
    /// <returns>Transducer that folds the stream of values</returns>
    public static Transducer<A, S> foldUntil<S, A>(
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        foldWhile(Schedule.Forever, initialState, folder, not(valueIs));

    /// <summary>
    /// Fold 
    /// </summary>
    /// <param name="schedule">Series of time-spans that dictate the rate of the fold and for how many iterations</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predicate">Predicate that decides if the state should be pushed down the stream</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="A">Input value to the fold operation</typeparam>
    /// <returns>Transducer that folds the stream of values</returns>
    public static Transducer<A, S> foldUntil<S, A>(
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        foldWhile(Schedule.Forever, initialState, folder, not(stateIs));    

    /// <summary>
    /// Fold 
    /// </summary>
    /// <param name="schedule">Series of time-spans that dictate the rate of the fold and for how many iterations</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="folder">Fold function</param>
    /// <param name="predicate">Predicate that decides if the state should be pushed down the stream</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="X">Alternative type for the transducer result (often the error type)</typeparam>
    /// <typeparam name="A">Input value to the fold operation</typeparam>
    /// <returns>Transducer that folds the stream of values</returns>
    public static Transducer<Sum<X, A>, Sum<X, S>> foldSum<S, X, A>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), TResult<Unit>> predicate) =>
        mapRight<X, A, S>(fold(schedule, initialState, folder, predicate));

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Choice
    //

    /// <summary>
    /// Choice transducer
    /// </summary>
    /// <remarks>
    /// Tries a sequence of transducers until one succeeds (results in a `Sum.Right`).  If the sequence
    /// is exhausted then the transducer completes.
    /// </remarks>
    /// <param name="transducers">Sequence of transducers</param>
    /// <returns>Transducer that encapsulates the choice</returns>
    public static Transducer<E, Sum<X, B>> choice<E, X, B>(Seq<Transducer<E, Sum<X, B>>> transducers) =>
        new ChoiceTransducer<E, X, B>(transducers.FlattenChoices().ToSeq());

    /// <summary>
    /// Choice transducer
    /// </summary>
    /// <remarks>
    /// Tries a sequence of transducers until one succeeds (results in a `Sum.Right`).  If the sequence
    /// is exhausted then the transducer completes.
    /// </remarks>
    /// <param name="transducers">Sequence of transducers</param>
    /// <returns>Transducer that encapsulates the choice</returns>
    public static Transducer<E, Sum<X, B>> choice<E, X, B>(params Transducer<E, Sum<X, B>>[] transducers) =>
        new ChoiceTransducer<E, X, B>(transducers.FlattenChoices().ToSeq());

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Memoisation
    //

    /// <summary>
    /// Caches the result of the transducer computation for each value flowing through.
    /// </summary>
    /// <remarks>
    /// This works within the context of a single `Invoke` operation, so it only makes
    /// sense if you're using the transducer as part of a stream process.  This allows
    /// each item coming through to have its result cached. So you never repeat the
    /// process for each `A` value.
    /// </remarks>
    /// <param name="transducer"></param>
    /// <typeparam name="EqA"></typeparam>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <returns></returns>
    public static Transducer<A, B> memoStream<A, B>(Transducer<A, B> transducer) =>
        memoStream<EqDefault<A>, A, B>(transducer);

    /// <summary>
    /// Caches the result of the transducer computation for each value flowing through.
    /// </summary>
    /// <remarks>
    /// This works within the context of a single `Invoke` operation, so it only makes
    /// sense if you're using the transducer as part of a stream process.  This allows
    /// each item coming through to have its result cached. So you never repeat the
    /// process for each `A` value.
    /// </remarks>
    /// <param name="transducer"></param>
    /// <typeparam name="EqA"></typeparam>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    /// <returns></returns>
    public static Transducer<A, B> memoStream<EqA, A, B>(Transducer<A, B> transducer)
        where EqA : struct, Eq<A> =>
        new MemoTransducer<EqA, A, B>(transducer);

    /// <summary>
    /// Caches the result of the transducer computation for each value flowing through.
    /// </summary>
    /// <remarks>
    /// Unlike `MemoStream` -  which only caches values for the duration of the the call
    /// to `Invoke` - this caches values for the duration of the life of the `Transducer`
    /// instance.  
    /// </remarks>
    /// <remarks>
    /// NOTE: Transducers use both a _value_ and a _state_ as inputs to its transformation
    /// and reduce process.  For memoisation the state is ignored.  In a non-memoisation
    /// scenario a different state and value pair could produce different results; and
    /// so this should be considered when deciding to apply a memo to a transducer: it
    /// only checks _value_ equality.
    /// </remarks>
    /// <param name="transducer">Transducer to memoise</param>
    /// <returns>Memoised transducer</returns>
    public static Transducer<A, B> memo<A, B>(Transducer<A, B> transducer) =>
        memo<EqDefault<A>, A, B>(transducer);

    /// <summary>
    /// Caches the result of the transducer computation for each value flowing through.
    /// </summary>
    /// <remarks>
    /// Unlike `MemoStream` -  which only caches values for the duration of the the call
    /// to `Invoke` - this caches values for the duration of the life of the `Transducer`
    /// instance.  
    /// </remarks>
    /// <remarks>
    /// NOTE: Transducers use both a _value_ and a _state_ as inputs to its transformation
    /// and reduce process.  For memoisation the state is ignored.  In a non-memoisation
    /// scenario a different state and value pair could produce different results; and
    /// so this should be considered when deciding to apply a memo to a transducer: it
    /// only checks _value_ equality.
    /// </remarks>
    /// <param name="transducer">Transducer to memoise</param>
    /// <returns>Memoised transducer</returns>
    public static Transducer<A, B> memo<EqA, A, B>(Transducer<A, B> transducer)
        where EqA : struct, Eq<A> =>
        new Memo1Transducer<EqA, A, B>(transducer);

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Zipping
    //
    
    /// <summary>
    /// Zips transducers together so their results are combined.  
    /// </summary>
    /// <remarks>
    /// Asynchronous transducers will run concurrently
    /// </remarks>
    /// <param name="First">First transducer</param>
    /// <param name="Second">Second transducer</param>
    /// <returns>A transducer that contains the results of both provided</returns>
    public static Transducer<E, (A First, B Second)> zip<E, A, B>(
        Transducer<E, A> First, 
        Transducer<E, B> Second) =>
        new ZipTransducer2<E, A, B>(First, Second);

    /// <summary>
    /// Zips transducers together so their results are combined.  
    /// </summary>
    /// <remarks>
    /// Asynchronous transducers will run concurrently
    /// </remarks>
    /// <param name="First">First transducer</param>
    /// <param name="Second">Second transducer</param>
    /// <returns>A transducer that contains the results of both provided</returns>
    public static Transducer<E, Sum<X, (A First, B Second)>> zip<E, X, A, B>(
        Transducer<E, Sum<X, A>> First, 
        Transducer<E, Sum<X, B>> Second) =>
        new ZipSumTransducer2<E, X, A, B>(First, Second);

    /// <summary>
    /// Zips transducers together so their results are combined.  
    /// </summary>
    /// <remarks>
    /// Asynchronous transducers will run concurrently
    /// </remarks>
    /// <param name="First">First transducer</param>
    /// <param name="Second">Second transducer</param>
    /// <param name="Third">Third transducer</param>
    /// <returns>A transducer that contains the results of all provided</returns>
    public static Transducer<E, (A First, B Second, C Third)> zip<E, A, B, C>(
        Transducer<E, A> First, 
        Transducer<E, B> Second,
        Transducer<E, C> Third) =>
        new ZipTransducer3<E, A, B, C>(First, Second, Third);

    /// <summary>
    /// Zips transducers together so their results are combined.  
    /// </summary>
    /// <remarks>
    /// Asynchronous transducers will run concurrently
    /// </remarks>
    /// <param name="First">First transducer</param>
    /// <param name="Second">Second transducer</param>
    /// <param name="Third">Third transducer</param>
    /// <returns>A transducer that contains the results of all provided</returns>
    public static Transducer<E, Sum<X, (A First, B Second, C Third)>> zip<E, X, A, B, C>(
        Transducer<E, Sum<X, A>> First, 
        Transducer<E, Sum<X, B>> Second, 
        Transducer<E, Sum<X, C>> Third) =>
        new ZipSumTransducer3<E, X, A, B, C>(First, Second, Third);

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Forking
    //
    
    /// <summary>
    /// Create a transducer that is queued to run on the thread-pool. 
    /// </summary>
    /// <param name="transducer">Transducer to fork</param>
    /// <param name="timeout">Maximum time that the forked transducer can run for.  `None` for no timeout.</param>
    /// <returns>Returns a `TFork` data-structure that contains two transducers that can be used to either cancel the
    /// /// forked transducer or to await the result of it.</returns>
    public static Transducer<A, TFork<B>> fork<A, B>(Transducer<A, B> transducer, Option<TimeSpan> timeout = default) =>
        new ForkTransducer1<A, B>(transducer, timeout);

    /// <summary>
    /// Create a transducer that is queued to run on the thread-pool. 
    /// </summary>
    /// <param name="transducer">Transducer to fork</param>
    /// <param name="timeout">Maximum time that the forked transducer can run for.  `None` for no timeout.</param>
    /// <returns>Returns a `TFork` data-structure that contains two transducers that can be used to either cancel the
    /// /// forked transducer or to await the result of it.</returns>
    public static Transducer<A, TFork<S>> fork<S, A, B>(
        Transducer<A, B> transducer, 
        S initialState,
        Reducer<B, S> reducer,
        Option<TimeSpan> timeout = default) =>
        new ForkTransducer2<S, A, B>(transducer, initialState, reducer, timeout);

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Error catching 
    //
    
    /// <summary>
    /// Like `try` / `catch` this transducer wraps an existing transducer and catches any errors generated.
    ///
    /// The `match` function is a predicate that decides if the error is going to be dealt with by the
    /// `catch` transducer.  If it is then it can make the result safe, or indeed throw a new error if
    /// necessary.
    /// </summary>
    /// <param name="transducer">Transducer to wrap with the try</param>
    /// <param name="match">Predicate that decides if the error is going to be dealt with</param>
    /// <param name="catch">Transducer that handles the error</param>
    /// <returns>A transducer that is wrapped with a `try` / `catch`</returns>
    public static Transducer<A, B> @try<A, B>(
        Transducer<A, B> transducer,
        Func<Error, bool> match,
        Transducer<Error, B> @catch) =>
        new TryTransducer<A, B>(transducer, match, @catch);

    /// <summary>
    /// Like `try` / `catch` this transducer wraps an existing transducer and catches any errors generated.
    ///
    /// The `match` function is a predicate that decides if the error is going to be dealt with by the
    /// `catch` transducer.  If it is then it can make the result safe, or indeed throw a new error if
    /// necessary.
    /// </summary>
    /// <remarks>
    /// Because the result of the wrapped transducer is a `Sum` type where `Left` is considered the failure
    /// value.  This function will convert any exceptional `Error` values to `Left` so they can be matched and
    /// caught just like any regular `Left` value.
    /// </remarks>
    /// <param name="transducer">Transducer to wrap with the try</param>
    /// <param name="match">Predicate that decides if the error is going to be dealt with</param>
    /// <param name="catch">Transducer that handles the error</param>
    /// <returns>A transducer that is wrapped with a `try` / `catch`</returns>
    public static Transducer<RT, Sum<X, A>> @try<RT, X, A>(
        Transducer<RT, Sum<X, A>> transducer,
        Func<X, bool> match,
        Transducer<X, Sum<X, A>> @catch) 
        where RT : struct, HasFromError<RT, X> =>
        new TryTransducer<RT, X, A>(transducer, match, @catch);

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Retry
    //
    
    /// <summary>
    /// Keep retrying if the transducer fails
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <returns>A transducer that retries</returns>
    /// <param name="transducer">Transducer to keep retrying</param>
    public static Transducer<A, B> retry<A, B>(Schedule schedule, Transducer<A, B> transducer) =>
        retryUntil(schedule, transducer, _ => false);

    /// <summary>
    /// Keep retrying if the transducer fails
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep retrying</param>
    /// <param name="predicate">Predicate that decides whether to continue on each failure.  If it returns
    /// `true` then the retying stops and the `Error` is yielded.</param>
    /// <returns>A transducer that retries</returns>
    public static Transducer<A, B> retryUntil<A, B>(
        Schedule schedule, 
        Transducer<A, B> transducer, 
        Func<Error, bool> predicate) =>
        new RetryTransducer<A, B>(transducer, schedule, predicate);

    /// <summary>
    /// Keep retrying if the transducer fails
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep retrying</param>
    /// <param name="predicate">Predicate that decides whether to continue on each failure.  If it returns
    /// `false` then the retying stops and the `Error` is yielded.</param>
    /// <returns>A transducer that retries</returns>
    public static Transducer<A, B> retryWhile<A, B>(
        Schedule schedule,
        Transducer<A, B> transducer,
        Func<Error, bool> predicate) =>
        retryUntil(schedule, transducer, not(predicate));

    /// <summary>
    /// Keep retrying if the transducer fails
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <returns>A transducer that retries</returns>
    /// <param name="transducer">Transducer to keep retrying</param>
    public static Transducer<RT, Sum<X, A>> retry<RT, X, A>(
        Schedule schedule, 
        Transducer<RT, Sum<X, A>> transducer)
        where RT : struct, HasFromError<RT, X> =>
        retryUntil<RT, X, A>(schedule, transducer, _ => false);

    /// <summary>
    /// Keep retrying if the transducer fails
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep retrying</param>
    /// <param name="predicate">Predicate that decides whether to continue on each failure.  If it returns
    /// `true` then the retying stops and the `Error` is yielded.</param>
    /// <returns>A transducer that retries</returns>
    public static Transducer<RT, Sum<X, A>> retryUntil<RT, X, A>(
        Schedule schedule, 
        Transducer<RT, Sum<X, A>> transducer, 
        Func<X, bool> predicate) 
        where RT : struct, HasFromError<RT, X> =>
        new RetrySumTransducer<RT, X, A>(transducer, schedule, predicate);

    /// <summary>
    /// Keep retrying if the transducer fails
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep retrying</param>
    /// <param name="predicate">Predicate that decides whether to continue on each repeat.  If it returns
    /// `false` then the repeating stops and the latest value is yielded.</param>
    /// <returns>A transducer that retries</returns>
    public static Transducer<RT, Sum<X, A>> retryWhile<RT, X, A>(
        Schedule schedule,
        Transducer<RT, Sum<X, A>> transducer,
        Func<X, bool> predicate) 
        where RT : struct, HasFromError<RT, X> =>
        retryUntil(schedule, transducer, not(predicate));
    
    /// <summary>
    /// Keep retrying if the transducer fails
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <returns>A transducer that retries</returns>
    /// <param name="transducer">Transducer to keep retrying</param>
    public static Transducer<A, B> retry<A, B>(
        Transducer<A, B> transducer) =>
        retryUntil(Schedule.Forever, transducer, _ => false);

    /// <summary>
    /// Keep retrying if the transducer fails
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep retrying</param>
    /// <param name="predicate">Predicate that decides whether to continue on each failure.  If it returns
    /// `true` then the retying stops and the `Error` is yielded.</param>
    /// <returns>A transducer that retries</returns>
    public static Transducer<A, B> retryUntil<A, B>(
        Transducer<A, B> transducer, 
        Func<Error, bool> predicate) =>
        new RetryTransducer<A, B>(transducer, Schedule.Forever, predicate);

    /// <summary>
    /// Keep retrying if the transducer fails
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep retrying</param>
    /// <param name="predicate">Predicate that decides whether to continue on each failure.  If it returns
    /// `false` then the retying stops and the `Error` is yielded.</param>
    /// <returns>A transducer that retries</returns>
    public static Transducer<A, B> retryWhile<A, B>(
        Transducer<A, B> transducer,
        Func<Error, bool> predicate) =>
        retryUntil(Schedule.Forever, transducer, not(predicate));

    /// <summary>
    /// Keep retrying if the transducer fails
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <returns>A transducer that retries</returns>
    /// <param name="transducer">Transducer to keep retrying</param>
    public static Transducer<RT, Sum<X, A>> retry<RT, X, A>(
        Transducer<RT, Sum<X, A>> transducer)
        where RT : struct, HasFromError<RT, X> =>
        retryUntil<RT, X, A>(Schedule.Forever, transducer, _ => false);

    /// <summary>
    /// Keep retrying if the transducer fails
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep retrying</param>
    /// <param name="predicate">Predicate that decides whether to continue on each failure.  If it returns
    /// `true` then the retying stops and the `Error` is yielded.</param>
    /// <returns>A transducer that retries</returns>
    public static Transducer<RT, Sum<X, A>> retryUntil<RT, X, A>(
        Transducer<RT, Sum<X, A>> transducer, 
        Func<X, bool> predicate) 
        where RT : struct, HasFromError<RT, X> =>
        new RetrySumTransducer<RT, X, A>(transducer, Schedule.Forever, predicate);

    /// <summary>
    /// Keep retrying if the transducer fails
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep retrying</param>
    /// <param name="predicate">Predicate that decides whether to continue on each repeat.  If it returns
    /// `false` then the repeating stops and the latest value is yielded.</param>
    /// <returns>A transducer that retries</returns>
    public static Transducer<RT, Sum<X, A>> retryWhile<RT, X, A>(
        Transducer<RT, Sum<X, A>> transducer,
        Func<X, bool> predicate) 
        where RT : struct, HasFromError<RT, X> =>
        retryUntil(Schedule.Forever, transducer, not(predicate));
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Repeat
    //
    
    /// <summary>
    /// Keep repeating the transducer
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of repeats and the time-gap between each one</param>
    /// <returns>A transducer that repeats</returns>
    /// <param name="transducer">Transducer to keep repeating</param>
    /// <returns>A transducer that repeats</returns>
    public static Transducer<A, B> repeat<A, B>(Schedule schedule, Transducer<A, B> transducer) =>
        repeatUntil(schedule, transducer, _ => false);

    /// <summary>
    /// Keep repeating the transducer until a condition is met
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of repeats and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep repeating</param>
    /// <param name="predicate">Predicate that decides whether to continue on each repeat.  If it returns
    /// `true` then the repeating stops and the latest value is yielded.</param>
    /// <returns>A transducer that repeats</returns>
    public static Transducer<A, B> repeatUntil<A, B>(
        Schedule schedule, 
        Transducer<A, B> transducer, 
        Func<B, bool> predicate) =>
        new RepeatTransducer<A, B>(transducer, schedule, predicate);

    /// <summary>
    /// Keep repeating the transducer until a condition is met
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep repeating</param>
    /// <param name="predicate">Predicate that decides whether to continue on each repeat.  If it returns
    /// `false` then the repeating stops and the latest value is yielded.</param>
    /// <returns>A transducer that repeats</returns>
    public static Transducer<A, B> repeatWhile<A, B>(
        Schedule schedule,
        Transducer<A, B> transducer,
        Func<B, bool> predicate) =>
        repeatUntil(schedule, transducer, not(predicate));

    /// <summary>
    /// Keep repeating the transducer 
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <returns>A transducer that retries</returns>
    /// <returns>A transducer that repeats</returns>
    public static Transducer<RT, Sum<X, A>> repeat<RT, X, A>(
        Schedule schedule, 
        Transducer<RT, Sum<X, A>> transducer)
        where RT : struct, HasFromError<RT, X> =>
        repeatUntil<RT, X, A>(schedule, transducer, _ => false);

    /// <summary>
    /// Keep repeating the transducer until a condition is met
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep repeating</param>
    /// <param name="predicate">Predicate that decides whether to continue on each repeat.  If it returns
    /// `true` then the repeating stops and the latest value is yielded.</param>
    /// <returns>A transducer that repeats</returns>
    public static Transducer<RT, Sum<X, A>> repeatUntil<RT, X, A>(
        Schedule schedule, 
        Transducer<RT, Sum<X, A>> transducer, 
        Func<A, bool> predicate) 
        where RT : struct, HasFromError<RT, X> =>
        new RepeatSumTransducer<RT, X, A>(transducer, schedule, predicate);

    /// <summary>
    /// Keep repeating the transducer until a condition is met
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of repeats and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep repeating</param>
    /// <param name="predicate">Predicate that decides whether to continue on each repeat.  If it returns
    /// `false` then the repeating stops and the latest value is yielded.</param>
    /// <returns>A transducer that repeats</returns>
    public static Transducer<RT, Sum<X, A>> repeatWhile<RT, X, A>(
        Schedule schedule,
        Transducer<RT, Sum<X, A>> transducer,
        Func<A, bool> predicate) 
        where RT : struct, HasFromError<RT, X> =>
        repeatUntil(schedule, transducer, not(predicate));
    
    /// <summary>
    /// Keep repeating the transducer
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of repeats and the time-gap between each one</param>
    /// <returns>A transducer that repeats</returns>
    /// <param name="transducer">Transducer to keep repeating</param>
    /// <returns>A transducer that repeats</returns>
    public static Transducer<A, B> repeat<A, B>(
        Transducer<A, B> transducer) =>
        repeatUntil(Schedule.Forever, transducer, _ => false);

    /// <summary>
    /// Keep repeating the transducer until a condition is met
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of repeats and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep repeating</param>
    /// <param name="predicate">Predicate that decides whether to continue on each repeat.  If it returns
    /// `true` then the repeating stops and the latest value is yielded.</param>
    /// <returns>A transducer that repeats</returns>
    public static Transducer<A, B> repeatUntil<A, B>(
        Transducer<A, B> transducer, 
        Func<B, bool> predicate) =>
        new RepeatTransducer<A, B>(transducer, Schedule.Forever, predicate);

    /// <summary>
    /// Keep repeating the transducer until a condition is met
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep repeating</param>
    /// <param name="predicate">Predicate that decides whether to continue on each repeat.  If it returns
    /// `false` then the repeating stops and the latest value is yielded.</param>
    /// <returns>A transducer that repeats</returns>
    public static Transducer<A, B> repeatWhile<A, B>(
        Transducer<A, B> transducer,
        Func<B, bool> predicate) =>
        repeatUntil(Schedule.Forever, transducer, not(predicate));

    /// <summary>
    /// Keep repeating the transducer 
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <returns>A transducer that retries</returns>
    /// <returns>A transducer that repeats</returns>
    public static Transducer<RT, Sum<X, A>> repeat<RT, X, A>(
        Transducer<RT, Sum<X, A>> transducer)
        where RT : struct, HasFromError<RT, X> =>
        repeatUntil<RT, X, A>(Schedule.Forever, transducer, _ => false);

    /// <summary>
    /// Keep repeating the transducer until a condition is met
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep repeating</param>
    /// <param name="predicate">Predicate that decides whether to continue on each repeat.  If it returns
    /// `true` then the repeating stops and the latest value is yielded.</param>
    /// <returns>A transducer that repeats</returns>
    public static Transducer<RT, Sum<X, A>> repeatUntil<RT, X, A>(
        Transducer<RT, Sum<X, A>> transducer, 
        Func<A, bool> predicate) 
        where RT : struct, HasFromError<RT, X> =>
        new RepeatSumTransducer<RT, X, A>(transducer, Schedule.Forever, predicate);

    /// <summary>
    /// Keep repeating the transducer until a condition is met
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of repeats and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep repeating</param>
    /// <param name="predicate">Predicate that decides whether to continue on each repeat.  If it returns
    /// `false` then the repeating stops and the latest value is yielded.</param>
    /// <returns>A transducer that repeats</returns>
    public static Transducer<RT, Sum<X, A>> repeatWhile<RT, X, A>(
        Transducer<RT, Sum<X, A>> transducer,
        Func<A, bool> predicate) 
        where RT : struct, HasFromError<RT, X> =>
        repeatUntil(Schedule.Forever, transducer, not(predicate));
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Cross thread-context posting
    //
    
    /// <summary>
    /// Make a transducer run on the `SynchronizationContext` that was captured at the start
    /// of an `Invoke` call.
    /// </summary>
    /// <remarks>
    /// The transducer receives its input value from the currently running sync-context and
    /// then proceeds to run its operation in the captured `SynchronizationContext`:
    /// typically a UI context, but could be any captured context.  The result of the
    /// transducer is the received back on the currently running sync-context. 
    /// </remarks>
    /// <param name="f">Transducer</param>
    /// <returns></returns>
    public static Transducer<A, B> post<A, B>(Transducer<A, B> f) =>
        new PostTransducer<A, B>(f);
}
