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

namespace LanguageExt.Transducers;

public static partial class Transducer
{
    /// <summary>
    /// Lift a value into the `Transducer` space
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>`Transducer` from `Unit` to `A`</returns>
    public static Transducer<Unit, A> Pure<A>(A value) =>
        constant<Unit, A>(value);

    /// <summary>
    /// Fail transducer
    /// </summary>
    /// <remarks>
    /// Consider this the `throw` of transducers.  It is the nuclear option to get out of a reduce.
    /// </remarks>
    /// <param name="error">Error to raise</param>
    /// <returns>A transducer that always fails</returns>
    public static Transducer<A, B> Fail<A, B>(Error error) =>
        new FailTransducer<A, B>(error);
    
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

    /// <summary>
    /// Identity transducer
    /// </summary>
    public static Transducer<A, A> identity<A>() =>
        IdentityTransducer<A>.Default;

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
    
    public static Transducer<A, B> lift<A, B>(Func<A, TResult<B>> f) =>
        new LiftTransducer1<A, B>(f);
    
    public static Transducer<Unit, A> lift<A>(Func<TResult<A>> f) =>
        new LiftTransducer2<A>(f);
    
    public static Transducer<A, B> lift<A, B>(Func<A, B> f) =>
        new LiftTransducer3<A, B>(f);
    
    public static Transducer<Unit, A> lift<A>(Func<A> f) =>
        new LiftTransducer4<A>(f);
    
    public static Transducer<A, B> liftIO<A, B>(Func<CancellationToken, A, Task<B>> f) =>
        new LiftIOTransducer3<A, B>(f);
    
    public static Transducer<A, B> liftIO<A, B>(Func<CancellationToken, A, Task<TResult<B>>> f) =>
        new LiftIOTransducer1<A, B>(f);
    
    public static Transducer<Unit, A> liftIO<A>(Func<CancellationToken, Task<A>> f) =>
        new LiftIOTransducer4<A>(f);
    
    public static Transducer<Unit, A> liftIO<A>(Func<CancellationToken, Task<TResult<A>>> f) =>
        new LiftIOTransducer2<A>(f);
    
    /// <summary>
    /// Transducer composition.  The output of one transducer is fed as the input to the next.
    ///
    /// Resulting im a single transducer that captures the composition
    /// </summary>
    /// <returns>Transducer that captures the composition</returns>
    public static Transducer<A, C> compose<A, B, C>(
        Transducer<A, B> f, 
        Transducer<B, C> g) =>
        new ComposeTransducer<A, B, C>(f, g);
    
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
        new ComposeTransducer<A, B, C, D>(f, g, h);
    
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
        new ComposeTransducer<A, B, C, D, E>(f, g, h, i);
    
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
        new ComposeTransducer<A, B, C, D, E, F>(f, g, h, i, j);
    
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
        new ComposeTransducer<A, B, C, D, E, F, G>(f, g, h, i, j, k);

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

    /*
    /// <summary>
    /// Take nested transducers and flatten them
    /// </summary>
    /// <param name="ff">Nested transducers</param>
    /// <returns>Flattened transducers</returns>
    public static Transducer<A, Sum<Y, B>> flatten<X, Y, A, B>(Transducer<A, Transducer<Sum<X, A>, Sum<Y, B>>> ff) =>
        new FlattenSumTransducer5<X, Y, A, B>(ff);

    /// <summary>
    /// Take nested transducers and flatten them
    /// </summary>
    /// <param name="ff">Nested transducers</param>
    /// <returns>Flattened transducers</returns>
    public static Transducer<A, Sum<Y, B>> flatten<X, Y, A, B>(Transducer<A, Transducer<Sum<X, Unit>, Sum<Y, B>>> ff) =>
        new FlattenSumTransducer6<X, Y, A, B>(ff);

    /// <summary>
    /// Take nested transducers and flatten them
    /// </summary>
    /// <param name="ff">Nested transducers</param>
    /// <returns>Flattened transducers</returns>
    public static Transducer<Sum<X, A>, Sum<Y, B>> flatten<X, Y, A, B>(Transducer<Sum<X, A>, Sum<Y, Transducer<A, B>>> ff) =>
        new FlattenSumTransducer7<X, Y, A, B>(ff);

    /// <summary>
    /// Take nested transducers and flatten them
    /// </summary>
    /// <param name="ff">Nested transducers</param>
    /// <returns>Flattened transducers</returns>
    public static Transducer<Sum<X, A>, Sum<Y, B>> flatten<X, Y, A, B>(Transducer<Sum<X, A>, Sum<Y, Transducer<Unit, B>>> ff) =>
        new FlattenSumTransducer8<X, Y, A, B>(ff);
        */

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

    /// <summary>
    /// Lifts a unit accepting transducer, ignores the input value.
    /// </summary>
    public static Transducer<A, B> ignore<A, B>(Transducer<Unit, B> m) =>
        new IgnoreTransducer<A, B>(m);

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
        new BiMap<E, X, Y, A, B>(First, Left, Right);

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
        new BiMap3<E, X, Y, A, B>(First, Left, Right);

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
        new BiMap2<X, Y, Z, A, B, C>(First, Left, Right);

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
        new BiMap4<X, Y, Z, A, B, C>(First, Left, Right);

    /// <summary>
    /// Functor bi-map map left
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Left">Left mapping transducer</param>
    /// <returns>Composition of first and left transducers</returns>
    public static Transducer<E, Sum<Y, A>> mapLeft<E, X, Y, A>(
        Transducer<E, Sum<X, A>> First,
        Transducer<X, Y> Left) =>
        new MapLeft<E, X, Y, A>(First, Left);

    /// <summary>
    /// Functor bi-map map left
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Left">Left mapping transducer</param>
    /// <returns>Composition of first and left transducers</returns>
    public static Transducer<E, Sum<Y, A>> mapLeft<E, X, Y, A>(
        Transducer<E, Sum<X, A>> First,
        Func<X, Y> Left) =>
        new MapLeft2<E, X, Y, A>(First, Left);

    /// <summary>
    /// Functor bi-map map left
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Left">Left mapping transducer</param>
    /// <returns>Composition of first and left</returns>
    public static Transducer<Sum<X, A>, Sum<Z, B>> mapLeft<X, Y, Z, A, B>(
        Transducer<Sum<X, A>, Sum<Y, B>> First,
        Transducer<Y, Z> Left) =>
        new MapLeft2<X, Y, Z, A, B>(First, Left);

    /// <summary>
    /// Functor bi-map map left
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Left">Left mapping transducer</param>
    /// <returns>Composition of first and left</returns>
    public static Transducer<Sum<X, A>, Sum<Z, B>> mapLeft<X, Y, Z, A, B>(
        Transducer<Sum<X, A>, Sum<Y, B>> First,
        Func<Y, Z> Left) =>
        new MapLeft3<X, Y, Z, A, B>(First, Left);

    /// <summary>
    /// Functor bi-map map right
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Right">Right mapping transducer</param>
    /// <returns>Composition of first and right transducers</returns>
    public static Transducer<E, Sum<X, B>> mapRight<E, X, A, B>(
        Transducer<E, Sum<X, A>> First,
        Transducer<A, B> Right) =>
        new MapRight<E, X, A, B>(First, Right);

    /// <summary>
    /// Functor bi-map map right
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Right">Right mapping transducer</param>
    /// <returns>Composition of first and right transducers</returns>
    public static Transducer<E, Sum<X, B>> mapRight<E, X, A, B>(
        Transducer<E, Sum<X, A>> First,
        Func<A, B> Right) =>
        new MapRight3<E, X, A, B>(First, Right);

    /// <summary>
    /// Functor bi-map map right
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Right">Right mapping transducer</param>
    /// <returns>Composition of first and right transducers</returns>
    public static Transducer<Sum<X, A>, Sum<Y, C>> mapRight<X, Y, A, B, C>(
        Transducer<Sum<X, A>, Sum<Y, B>> First,
        Transducer<B, C> Right) =>
        new MapRight2<X, Y, A, B, C>(First, Right);

    /// <summary>
    /// Functor bi-map map right
    /// </summary>
    /// <param name="First">First transducer to run</param>
    /// <param name="Right">Right mapping transducer</param>
    /// <returns>Composition of first and right transducers</returns>
    public static Transducer<Sum<X, A>, Sum<Y, C>> mapRight<X, Y, A, B, C>(
        Transducer<Sum<X, A>, Sum<Y, B>> First,
        Func<B, C> Right) =>
        new MapRight1<X, Y, A, B, C>(First, Right);

    /// <summary>
    /// Make a Sum Right from a value
    /// </summary>
    public static Transducer<A, Sum<X, A>> mkRight<X, A>() =>
        lift<A, Sum<X, A>>(a => Sum<X, A>.Right(a));

    /// <summary>
    /// Make a Sum Left from a value
    /// </summary>
    public static Transducer<X, Sum<X, A>> mkLeft<X, A>() =>
        lift<X, Sum<X, A>>(x => Sum<X, A>.Left(x));
    
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

    /// <summary>
    /// Fold 
    /// </summary>
    /// <param name="transducer">Transducer that provides the stream of values</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="folder">Fold function</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="E">Input type to the transducer</typeparam>
    /// <typeparam name="A">Value that a successful operation of the transducer will yield</typeparam>
    /// <returns>Transducer that folds the stream of values</returns>
    public static Transducer<E, S> fold<S, E, A>(Transducer<E, A> transducer, S initialState, Func<S, A, S> folder) =>
        new FoldTransducer1<S, E, A>(transducer, initialState, folder);

    /// <summary>
    /// Fold 
    /// </summary>
    /// <param name="transducer">Transducer that provides the stream of values</param>
    /// <param name="initialState">Initial state for the fold</param>
    /// <param name="folder">Fold function</param>
    /// <typeparam name="S">State type</typeparam>
    /// <typeparam name="E">Input type to the transducer</typeparam>
    /// <typeparam name="X">Alternative type for the transducer result (often the error type)</typeparam>
    /// <typeparam name="A">Value that a successful operation of the transducer will yield</typeparam>
    /// <returns>Transducer that folds the stream of values</returns>
    public static Transducer<E, Sum<X, S>> fold<S, E, X, A>(Transducer<E, Sum<X, A>> transducer, S initialState, Func<S, A, S> folder) =>
        new FoldTransducer2<S, E, X, A>(transducer, initialState, folder);

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
    /// <param name="predicate">Predicate that decides whether to continue on each failure.  If it returns
    /// `false` then the retying stops and the `Error` is yielded.</param>
    /// <returns>A transducer that retries</returns>
    public static Transducer<RT, Sum<X, A>> retryWhile<RT, X, A>(
        Schedule schedule,
        Transducer<RT, Sum<X, A>> transducer,
        Func<X, bool> predicate) 
        where RT : struct, HasFromError<RT, X> =>
        retryUntil(schedule, transducer, not(predicate));

}
