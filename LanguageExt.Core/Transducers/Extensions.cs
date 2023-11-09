#nullable enable

using System;
using LanguageExt.HKT;

namespace LanguageExt.Transducers;

public static partial class Transducer
{
    record Wrap<M, A, B>(Transducer<A, B> F) : KArr<M, A, B>
    {
        public Transducer<A, B> Morphism => F;
    }

    internal static KArr<M, A, B> Cast<M, A, B>(this Transducer<A, B> f) =>
        new Wrap<M, A, B>(f);
    
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
    public static TResult<B> Invoke1<A, B>(this Transducer<A, B> transducer, A value) =>
        transducer
            .Invoke(value, default, Invoke1Reducer<B>.Default)
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
        Reducer<S, B> reducer)
    {
        var st = new TState();

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
}
