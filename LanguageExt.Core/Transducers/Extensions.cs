#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using LanguageExt.HKT;
using LanguageExt.TypeClasses;

namespace LanguageExt.Transducers;

public static partial class Transducer
{
    /// <summary>
    /// Resource tracking transducer
    /// </summary>
    public static Transducer<A, B> Use<A, B>(this Transducer<A, B> transducer, Func<B, Unit> dispose) =>
        new UseTransducer1<A, B>(transducer, dispose);
    
    /// <summary>
    /// Resource tracking transducer
    /// </summary>
    public static Transducer<A, B> Use<A, B>(this Transducer<A, B> transducer) where B : IDisposable =>
        new UseTransducer2<A, B>(transducer);
    
    /// <summary>
    /// Maps every value passing through this transducer
    /// </summary>
    public static Transducer<A, C> Map<A, B, C>(this Transducer<A, B> m, Func<B, C> f) =>
        new SelectTransducer<A, B, C>(m, f);

    /// <summary>
    /// Maps every right value passing through this transducer
    /// </summary>
    public static Transducer<E, Sum<X, B>> MapRight<E, X, A, B>(this Transducer<E, Sum<X, A>> m, Func<A, B> f) =>
        mapRight(m, lift(f));

    /// <summary>
    /// Maps every right value passing through this transducer
    /// </summary>
    public static Transducer<E, Sum<X, B>> MapRight<E, X, A, B>(this Transducer<E, Sum<X, A>> m, Transducer<A, B> f) =>
        mapRight(m, f);

    /// <summary>
    /// Maps every left value passing through this transducer
    /// </summary>
    public static Transducer<E, Sum<Y, A>> MapLeft<E, X, Y, A>(this Transducer<E, Sum<X, A>> m, Func<X, Y> f) =>
        mapLeft(m, lift(f));

    /// <summary>
    /// Maps every left value passing through this transducer
    /// </summary>
    public static Transducer<E, Sum<Y, A>> MapLeft<E, X, Y, A>(this Transducer<E, Sum<X, A>> m, Transducer<X, Y> f) =>
        mapLeft(m, f);

    /// <summary>
    /// Maps every left value passing through this transducer
    /// </summary>
    public static Transducer<E, Sum<Y, B>> BiMap<E, X, Y, A, B>(
        this Transducer<E, Sum<X, A>> transducer, 
        Func<X, Y> Left,
        Func<A, B> Right) =>
        bimap(transducer, lift(Left), lift(Right));

    /// <summary>
    /// Maps every left value passing through this transducer
    /// </summary>
    public static Transducer<E, Sum<Y, B>> BiMap<E, X, Y, A, B>(
        this Transducer<E, Sum<X, A>> transducer, 
        Transducer<X, Y> Left,
        Transducer<A, B> Right) =>
        bimap(transducer, Left, Right);

    /// <summary>
    /// Maps every value passing through this transducer
    /// </summary>
    public static Transducer<A, C> Select<A, B, C>(this Transducer<A, B> m, Func<B, C> g) =>
        new SelectTransducer<A, B, C>(m, g);

    /// <summary>
    /// Projects every value into the monadic bind function provided. 
    /// </summary>
    /// <returns>Monadic bound transducer</returns>
    public static Transducer<A, C> Bind<A, B, C>(this Transducer<A, B> m, Func<B, Transducer<A, C>> g) =>
        new BindTransducer3<A, B, C>(m, g);

    /// <summary>
    /// Projects every value into the monadic bind function provided. 
    /// </summary>
    /// <returns>Monadic bound transducer</returns>
    public static Transducer<A, C> Bind<A, B, C>(this Transducer<A, B> m, Transducer<B, Transducer<A, C>> g) =>
        new BindTransducer1<A, B, C>(m, g);

    /// <summary>
    /// Projects every value into the monadic bind function provided. 
    /// </summary>
    /// <returns>Monadic bound transducer</returns>
    public static Transducer<A, C> SelectMany<A, B, C>(this Transducer<A, B> m, Func<B, Transducer<A, C>> g) =>
        new BindTransducer3<A, B, C>(m, g);
    
    /// <summary>
    /// Projects every value into the monadic bind function provided. 
    /// </summary>
    /// <returns>Monadic bound transducer</returns>
    public static Transducer<A, D> SelectMany<A, B, C, D>(
        this Transducer<A, B> m, 
        Func<B, Transducer<A, C>> g,
        Func<B, C, D> h) =>
        new SelectManyTransducer2<A, B, C, D>(m, g, h);    

    /// <summary>
    /// Take nested transducers and flatten them
    /// </summary>
    /// <param name="ff">Nested transducers</param>
    /// <returns>Flattened transducers</returns>
    public static Transducer<A, B> Flatten<A, B>(this Transducer<A, Transducer<A, B>> ff) =>
        new FlattenTransducer1<A, B>(ff);

    /// <summary>
    /// Take nested transducers and flatten them
    /// </summary>
    /// <param name="ff">Nested transducers</param>
    /// <returns>Flattened transducers</returns>
    public static Transducer<A, B> Flatten<A, B>(this Transducer<A, Transducer<Unit, B>> ff) =>
        new FlattenTransducer2<A, B>(ff);
    
    /// <summary>
    /// Take nested transducers and flatten them
    /// </summary>
    /// <param name="ff">Nested transducers</param>
    /// <returns>Flattened transducers</returns>
    public static Transducer<Unit, Sum<X, A>> Flatten<X, A>(
        this Transducer<Unit, Sum<X, Transducer<Unit, Sum<X, A>>>> ff) =>
        new FlattenSumTransducer1<Unit, X, A>(ff);
    
    /// <summary>
    /// Take nested transducers and flatten them
    /// </summary>
    /// <param name="ff">Nested transducers</param>
    /// <returns>Flattened transducers</returns>
    public static Transducer<Env, Sum<X, A>> Flatten<Env, X, A>(
        this Transducer<Env, Sum<X, Transducer<Env, Sum<X, A>>>> ff) =>
        new FlattenSumTransducer1<Env, X, A>(ff);
    
    /// <summary>
    /// Take nested transducers and flatten them
    /// </summary>
    /// <param name="ff">Nested transducers</param>
    /// <returns>Flattened transducers</returns>
    public static Transducer<Env, Sum<X, A>> Flatten<Env, X, A>(
        this Transducer<Env, Sum<Transducer<Env, Sum<X, A>>, Transducer<Env, Sum<X, A>>>> ff) =>
        new FlattenSumTransducer2<Env, X, A>(ff);

    /// <summary>
    /// Filter the values in the transducer
    /// </summary>
    /// <param name="f">Transducer to filter</param>
    /// <param name="pred">Predicate to apply</param>
    /// <returns>Filtered transducer</returns>
    public static Transducer<A, B> Filter<A, B>(this Transducer<A, B> f, Transducer<B, bool> pred) =>
        new FilterTransducer1<A, B>(f, pred);

    /// <summary>
    /// Filter the values in the transducer
    /// </summary>
    /// <param name="f">Transducer to filter</param>
    /// <param name="pred">Predicate to apply</param>
    /// <returns>Filtered transducer</returns>
    public static Transducer<A, B> Filter<A, B>(this Transducer<A, B> f, Func<B, bool> pred) =>
        Filter(f, lift(pred));

    /// <summary>
    /// Filter the values in the transducer
    /// </summary>
    /// <param name="f">Transducer to filter</param>
    /// <param name="pred">Predicate to apply</param>
    /// <returns>Filtered transducer</returns>
    public static Transducer<A, Sum<X, B>> Filter<X, A, B>(this Transducer<A, Sum<X, B>> f, Transducer<B, bool> pred) =>
        new FilterSumTransducer1<X, A, B>(f, pred);

    /// <summary>
    /// Filter the values in the transducer
    /// </summary>
    /// <param name="f">Transducer to filter</param>
    /// <param name="pred">Predicate to apply</param>
    /// <returns>Filtered transducer</returns>
    public static Transducer<A, Sum<X, B>> Filter<X, A, B>(this Transducer<A, Sum<X, B>> f, Func<B, bool> pred) =>
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
    public static Transducer<E, S> Fold<S, E, A>(this Transducer<E, A> transducer, S initialState, Func<S, A, S> folder) =>
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
    public static Transducer<E, Sum<X, S>> Fold<S, E, X, A>(this Transducer<E, Sum<X, A>> transducer, S initialState, Func<S, A, S> folder) =>
        new FoldTransducer2<S, E, X, A>(transducer, initialState, folder);

    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Gets a lifted function and a lifted argument, invokes the function with the argument and re-lifts the result.
    /// </remarks>
    /// <returns>Result of applying the function to the argument</returns>
    public static Transducer<A, C> Apply<A, B, C>(
        this Transducer<A, Func<B, C>> ff,
        Transducer<A, B> fa) =>
        new ApplyTransducer<A, B, C>(ff, fa);
    
    /// <summary>
    /// Lifts a unit accepting transducer, ignores the input value.
    /// </summary>
    public static Transducer<A, B> Ignore<A, B>(this Transducer<Unit, B> m) =>
        new IgnoreTransducer<A, B>(m);

    /// <summary>
    /// Caches the result of the transducer computation for each value flowing through.
    /// </summary>
    /// <remarks>
    /// This works within the context of a single `Invoke` operation, so it only makes
    /// sense if you're using the transducer as part of a stream process.  This allows
    /// each item coming through to have its result cached. So you never repeat the
    /// process for each `A` value.
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
    public static Transducer<A, B> MemoStream<A, B>(this Transducer<A, B> transducer) =>
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
    /// <remarks>
    /// NOTE: Transducers use both a _value_ and a _state_ as inputs to its transformation
    /// and reduce process.  For memoisation the state is ignored.  In a non-memoisation
    /// scenario a different state and value pair could produce different results; and
    /// so this should be considered when deciding to apply a memo to a transducer: it
    /// only checks _value_ equality.
    /// </remarks>
    /// <param name="transducer">Transducer to memoise</param>
    /// <returns>Memoised transducer</returns>
    public static Transducer<A, B> MemoStream<EqA, A, B>(this Transducer<A, B> transducer)
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
    public static Transducer<A, B> Memo<A, B>(this Transducer<A, B> transducer) =>
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
    public static Transducer<A, B> Memo<EqA, A, B>(this Transducer<A, B> transducer)
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
    public static Transducer<E, (A First, B Second)> Zip<E, A, B>(
        this Transducer<E, A> First, 
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
    public static Transducer<E, Sum<X, (A First, B Second)>> Zip<E, X, A, B>(
        this Transducer<E, Sum<X, A>> First, 
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
    public static Transducer<E, (A First, B Second, C Third)> Zip<E, A, B, C>(
        this Transducer<E, A> First, 
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
    public static Transducer<E, Sum<X, (A First, B Second, C Third)>> Zip<E, X, A, B, C>(
        this Transducer<E, Sum<X, A>> First, 
        Transducer<E, Sum<X, B>> Second, 
        Transducer<E, Sum<X, C>> Third) =>
        new ZipSumTransducer3<E, X, A, B, C>(First, Second, Third);    

    
    
    /// <summary>
    /// Invoke the transducer, reducing to a single value only
    /// </summary>
    /// <param name="transducer">Transducer to invoke</param>
    /// <param name="value">Value to use as the argument to the transducer</param>
    /// <returns>
    /// If the transducer yields multiple values then it will return the last value in a `TResult.Complete`.
    /// If the transducer yields zero values then it will return `TResult.None`. 
    /// If the transducer throws an exception or yields an `Error`, then it will return `TResult.Fail`.
    /// If the transducer is cancelled, then it will return `TResult.Cancelled`. 
    /// </returns>
    public static TResult<B> Invoke1<A, B>(
        this Transducer<A, B> transducer, 
        A value, 
        CancellationToken token) =>
        transducer
            .Invoke(value, default, Invoke1Reducer<B>.Default, token)
            .Bind(static b => b is null ? TResult.None<B>() : TResult.Complete<B>(b));
    
    /// <summary>
    /// Invoke the transducer, transforming the input value and finally reducing the output  with
    /// the `Reducer` provided
    /// </summary>
    /// <param name="transducer">Transducer to invoke</param>
    /// <param name="value">Value to use as the argument to the transducer</param>
    /// <param name="initialState">Starting state</param>
    /// <param name="reducer">Value to use as the argument to the transducer</param>
    /// <returns>
    /// If the transducer yields multiple values then it will return the last value in a `TResult.Complete`.
    /// If the transducer yields zero values then it will return `TResult.None`. 
    /// If the transducer throws an exception or yields an `Error`, then it will return `TResult.Fail`.
    /// If the transducer is cancelled, then it will return `TResult.Cancelled`. 
    /// </returns>
    public static TResult<S> Invoke<S, A, B>(
        this Transducer<A, B> transducer, 
        A value, 
        S initialState, 
        Reducer<B, S> reducer, 
        CancellationToken token)
    {
        var st = new TState(token);

        try
        {
            var s = initialState;
            var tf = transducer.Transform(reducer);
            var tr = tf.Run(st, s, value);

            while (true)
            {
                switch (tr)
                {
                    case TRecursive<S> r:
                        tr = r.Run();
                        break;

                    case TContinue<S> {Value: not null} r:
                        return TResult.Complete<S>(r.Value);

                    case TComplete<S> {Value: not null} r:
                        return TResult.Complete<S>(r.Value);

                    case TCancelled<S>:
                        return TResult.Cancel<S>();

                    case TFail<S> r:
                        return TResult.Fail<S>(r.Error);

                    default:
                        return TResult.None<S>();
                }
            }
        }
        catch (Exception e)
        {
            return TResult.Fail<S>(e);
        }
        finally
        {
            st.Dispose();
        }
    }


    /// <summary>
    /// Invoke the transducer, transforming the input value and finally reducing the output  with
    /// the `Reducer` provided
    /// </summary>
    /// <param name="transducer">Transducer to invoke</param>
    /// <param name="value">Value to use as the argument to the transducer</param>
    /// <param name="initialState">Starting state</param>
    /// <param name="reducer">Value to use as the argument to the transducer</param>
    /// <returns>
    /// If the transducer yields multiple values then it will return the last value in a `TResult.Complete`.
    /// If the transducer yields zero values then it will return `TResult.None`. 
    /// If the transducer throws an exception or yields an `Error`, then it will return `TResult.Fail`.
    /// If the transducer is cancelled, then it will return `TResult.Cancelled`. 
    /// </returns>
    public static Task<TResult<S>> InvokeAsync<S, A, B>(
        this Transducer<A, B> transducer,
        A value,
        S initialState,
        Reducer<B, S> reducer,
        CancellationToken token) =>
        TaskAsync<A>.RunAsync<S>((t, v) => Invoke(transducer, v, initialState, reducer, t), value, token);
    
    /// <summary>
    /// Invoke the transducer, reducing to a single value only
    /// </summary>
    /// <param name="transducer">Transducer to invoke</param>
    /// <param name="value">Value to use as the argument to the transducer</param>
    /// <returns>
    /// If the transducer yields multiple values then it will return the last value in a `TResult.Complete`.
    /// If the transducer yields zero values then it will return `TResult.None`. 
    /// If the transducer throws an exception or yields an `Error`, then it will return `TResult.Fail`.
    /// If the transducer is cancelled, then it will return `TResult.Cancelled`. 
    /// </returns>
    public static Task<TResult<B>> Invoke1Async<A, B>(
        this Transducer<A, B> transducer, 
        A value, 
        CancellationToken token) =>
        TaskAsync<A>.RunAsync<B>((t, v) => Invoke1(transducer, v, t), value, token);
}
