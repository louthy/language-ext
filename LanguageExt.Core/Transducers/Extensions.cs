using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.HKT;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class Transducer
{
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Recursion
    //
    
    /// <summary>
    /// Wrap this around a tail recursive call to mark it as the end of a recursive expression.
    /// </summary>
    public static Transducer<A, B> Tail<A, B>(this Transducer<A, B> recursive) =>
        new TailTransducer<A, B>(recursive);
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Resource use
    //
    
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
    public static Transducer<E, B> Apply<E, A, B>(
        this Transducer<E, Func<A, B>> ff,
        Transducer<E, A> fa) =>
        new ApplyTransducer<E, A, B>(ff, fa);

    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Gets a lifted function and a lifted argument, invokes the function with the argument and re-lifts the result.
    /// </remarks>
    /// <returns>Result of applying the function to the argument</returns>
    public static Transducer<E, B> Apply<E, A, B>(
        this Transducer<E, Transducer<A, B>> ff,
        Transducer<E, A> fa) =>
        new ApplyTransducer2<E, A, B>(ff, fa);    

    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Gets a lifted function and a lifted argument, invokes the function with the argument and re-lifts the result.
    /// </remarks>
    /// <returns>Result of applying the function to the argument</returns>
    public static Transducer<E, Sum<X, B>> Action<E, X, A, B>(
        this Transducer<E, Sum<X, A>> fa,
        Transducer<E, Sum<X, B>> fb) =>
        new ActionSumTransducer<E, X, A, B>(fa, fb);
    
    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Gets a lifted function and a lifted argument, invokes the function with the argument and re-lifts the result.
    /// </remarks>
    /// <returns>Result of applying the function to the argument</returns>
    public static Transducer<E, Sum<X, B>> Apply<E, X, A, B>(
        this Transducer<E, Sum<X, Func<A, B>>> ff,
        Transducer<E, Sum<X, A>> fa) =>
        new ApplySumTransducer<E, X, A, B>(ff, fa);

    /// <summary>
    /// Applicative apply
    /// </summary>
    /// <remarks>
    /// Gets a lifted function and a lifted argument, invokes the function with the argument and re-lifts the result.
    /// </remarks>
    /// <returns>Result of applying the function to the argument</returns>
    public static Transducer<E, Sum<X, B>> Apply<E, X, A, B>(
        this Transducer<E, Sum<X, Transducer<A, B>>> ff,
        Transducer<E, Sum<X, A>> fa) =>
        new ApplySumTransducer2<E, X, A, B>(ff, fa);
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Partial application
    //
    
    /// <summary>
    /// Partial application
    /// </summary>
    /// <param name="f">Transducer to partially apply</param>
    /// <param name="value">Value to apply</param>
    /// <returns>Constant transducer with the first argument filled</returns>
    public static Transducer<Unit, B> Invoke<A, B>(this Transducer<A, B> f, A value) =>
        compose(pure(value), f);

    /// <summary>
    /// Partial application
    /// </summary>
    /// <param name="f">Transducer to partially apply</param>
    /// <param name="value">Value to apply</param>
    /// <returns>Transducer with the first argument filled</returns>
    public static Transducer<B, C> Invoke<A, B, C>(this Transducer<A, Transducer<B, C>> f, A value) =>
        new PartialTransducer<A, B, C>(value, f);
    
    /// <summary>
    /// Partial application
    /// </summary>
    /// <param name="f">Transducer to partially apply</param>
    /// <param name="value">Value to apply</param>
    /// <returns>Transducer with the first argument filled</returns>
    public static Transducer<B, C> Invoke<A, B, C>(this Transducer<A, Func<B, C>> f, A value) =>
        new PartialFTransducer<A, B, C>(value, f);
        
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Map (compose)
    //
    
    /// <summary>
    /// Maps every value passing through this transducer
    /// </summary>
    public static Transducer<A, C> Map<A, B, C>(this Transducer<A, B> m, Func<B, C> f) =>
        new MapTransducer<A, B, C>(m, f);

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Map right
    //
    
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

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Map left
    //
    
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

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Bi-mapping
    //
    
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

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Select
    //
    
    /// <summary>
    /// Maps every value passing through this transducer
    /// </summary>
    public static Transducer<A, C> Select<A, B, C>(this Transducer<A, B> m, Func<B, C> g) =>
        new MapTransducer<A, B, C>(m, g);

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Flatten
    //
    
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
        this Transducer<Env, Sum<X, Transducer<Unit, Sum<X, A>>>> ff) =>
        new FlattenSumTransducer3<Env, X, A>(ff);
    
    /// <summary>
    /// Take nested transducers and flatten them
    /// </summary>
    /// <param name="ff">Nested transducers</param>
    /// <returns>Flattened transducers</returns>
    public static Transducer<Env, Sum<X, A>> Flatten<Env, X, A>(
        this Transducer<Env, Sum<Transducer<Env, Sum<X, A>>, Transducer<Env, Sum<X, A>>>> ff) =>
        new FlattenSumTransducer2<Env, X, A>(ff);

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Binding
    //
    
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
    public static Transducer<A, C> Bind<A, B, C>(this Transducer<Unit, B> m, Func<B, Transducer<A, C>> g) =>
        new BindTransducer3<A, B, C>(compose(constant<A, Unit>(default), m), g);

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
    public static Transducer<A, C> Bind<A, B, C>(this Transducer<Unit, B> m, Transducer<B, Transducer<A, C>> g) =>
        new BindTransducer1<A, B, C>(compose(constant<A, Unit>(default), m), g);

    /// <summary>
    /// Projects every value into the monadic bind function provided. 
    /// </summary>
    /// <returns>Monadic bound transducer</returns>
    public static Transducer<E, Sum<X, Unit>> Bind<E, X, A>(this Transducer<E, A> ma, Transducer<A, Guard<X, Unit>> f) =>
        ma.Bind(f.Map(g => compose(constant<E, Guard<X, Unit>>(g), guard<X>())));

    /// <summary>
    /// Projects every value into the monadic bind function provided. 
    /// </summary>
    /// <returns>Monadic bound transducer</returns>
    public static Transducer<E, Sum<X, Unit>> Bind<E, X, A>(this Transducer<E, A> ma, Func<A, Guard<X, Unit>> f) =>
        ma.Bind(lift(f));

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Binding (Sum types)
    //
    
    /// <summary>
    /// Projects every value into the monadic bind function provided. 
    /// </summary>
    /// <returns>Monadic bound transducer</returns>
    public static Transducer<E, Sum<X, B>> BindSum<E, X, A, B>(this Transducer<E, Sum<X, A>> ma, Transducer<A, Transducer<E, Sum<X, B>>> f) =>
        new BindTransducerSum<X, E, A, B>(ma, f);
    
    /// <summary>
    /// Projects every value into the monadic bind function provided. 
    /// </summary>
    /// <returns>Monadic bound transducer</returns>
    public static Transducer<E, Sum<X, B>> BindSum<E, X, A, B>(this Transducer<Unit, Sum<X, A>> ma, Transducer<A, Transducer<E, Sum<X, B>>> f) =>
        BindSum(compose(constant<E, Unit>(default), ma), f);

    /// <summary>
    /// Projects every value into the monadic bind function provided. 
    /// </summary>
    /// <returns>Monadic bound transducer</returns>
    public static Transducer<E, Sum<X, B>> BindSum<E, X, A, B>(this Transducer<E, Sum<X, A>> ma, Func<A, Transducer<E, Sum<X, B>>> f) =>
        new BindTransducerSum2<X, E, A, B>(ma, f);

    /// <summary>
    /// Projects every value into the monadic bind function provided. 
    /// </summary>
    /// <returns>Monadic bound transducer</returns>
    public static Transducer<E, Sum<X, B>> BindSum<E, X, A, B>(this Transducer<Unit, Sum<X, A>> ma, Func<A, Transducer<E, Sum<X, B>>> f) =>
        new BindTransducerSum2<X, E, A, B>(compose(constant<E, Unit>(default), ma), f);

    /// <summary>
    /// Projects every value into the monadic bind function provided. 
    /// </summary>
    /// <returns>Monadic bound transducer</returns>
    public static Transducer<E, Sum<X, B>> BindSum<E, X, A, B>(this Transducer<E, Sum<X, A>> ma, Transducer<A, Transducer<E, B>> f) =>
        BindSum(ma, lift<A, Transducer<E, Sum<X, B>>>(x => compose(partial(f, x), mkRight<X, B>())));
    
    /// <summary>
    /// Projects every value into the monadic bind function provided. 
    /// </summary>
    /// <returns>Monadic bound transducer</returns>
    public static Transducer<E, Sum<X, B>> BindSum<E, X, A, B>(this Transducer<Unit, Sum<X, A>> ma, Transducer<A, Transducer<E, B>> f) =>
        BindSum(compose(constant<E, Unit>(default), ma), f);

    /// <summary>
    /// Projects every value into the monadic bind function provided. 
    /// </summary>
    /// <returns>Monadic bound transducer</returns>
    public static Transducer<E, Sum<X, B>> BindSum<E, X, A, B>(this Transducer<E, Sum<X, A>> ma, Func<A, Transducer<E, B>> f) =>
        new BindTransducerSum2<X, E, A, B>(ma, a => compose(f(a), mkRight<X, B>()));

    /// <summary>
    /// Projects every value into the monadic bind function provided. 
    /// </summary>
    /// <returns>Monadic bound transducer</returns>
    public static Transducer<E, Sum<X, B>> BindSum<E, X, A, B>(this Transducer<Unit, Sum<X, A>> ma, Func<A, Transducer<E, B>> f) =>
        new BindTransducerSum2<X, E, A, B>(compose(constant<E, Unit>(default), ma), a => compose(f(a), mkRight<X, B>()));
    
    /// <summary>
    /// Projects every value into the monadic bind function provided. 
    /// </summary>
    /// <returns>Monadic bound transducer</returns>
    public static Transducer<E, Sum<X, Unit>> BindSum<E, X, A>(this Transducer<E, Sum<X, A>> ma, Transducer<A, Guard<X, Unit>> f) =>
        ma.BindSum(f.Map(g => compose(constant<E, Guard<X, Unit>>(g), guard<X>())));

    /// <summary>
    /// Projects every value into the monadic bind function provided. 
    /// </summary>
    /// <returns>Monadic bound transducer</returns>
    public static Transducer<Unit, Sum<X, Unit>> BindSum<X, A>(this Transducer<Unit, Sum<X, A>> ma, Transducer<A, Guard<X, Unit>> f) =>
        ma.BindSum(f.Map(g => compose(constant<Unit, Guard<X, Unit>>(g), guard<X>())));

    /// <summary>
    /// Projects every value into the monadic bind function provided. 
    /// </summary>
    /// <returns>Monadic bound transducer</returns>
    public static Transducer<E, Sum<X, Unit>> BindSum<E, X, A>(this Transducer<E, Sum<X, A>> ma, Func<A, Guard<X, Unit>> f) =>
        ma.BindSum(lift(f));

    /// <summary>
    /// Projects every value into the monadic bind function provided. 
    /// </summary>
    /// <returns>Monadic bound transducer</returns>
    public static Transducer<Unit, Sum<X, Unit>> BindSum<X, A>(this Transducer<Unit, Sum<X, A>> ma, Func<A, Guard<X, Unit>> f) =>
        ma.BindSum(lift(f));
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // SelectMany
    //
    
    /// <summary>
    /// Projects every value into the monadic bind function provided. 
    /// </summary>
    /// <returns>Monadic bound transducer</returns>
    public static Transducer<A, D> SelectMany<A, B, C, D>(
        this Transducer<A, B> ma, 
        Func<B, Transducer<A, C>> bind,
        Func<B, C, D> project) =>
        new SelectManyTransducer1<A, B, C, D>(ma, bind, project);

    /// <summary>
    /// Projects every value into the monadic bind function provided. 
    /// </summary>
    /// <returns>Monadic bound transducer</returns>
    public static Transducer<A, Sum<X, C>> SelectMany<X, A, B, C>(
        this Transducer<A, B> ma,
        Func<B, Guard<X, Unit>> bind,
        Func<B, Unit, C> project) =>
        compose(ma, mkRight<X, B>())
           .BindSum(b => compose(
                     constant<A, Unit>(default), 
                     bind(b).ToTransducer())
                    .MapRight(_ => project(b, default)));    
    
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
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Ignoring and memoisation
    //
    
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
        where EqA : Eq<A> =>
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
        where EqA : Eq<A> =>
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
    public static Transducer<A, TFork<B>> Fork<A, B>(
        this Transducer<A, B> transducer, 
        Option<TimeSpan> timeout = default) =>
        new ForkTransducer1<A, B>(transducer, timeout);

    /// <summary>
    /// Create a transducer that is queued to run on the thread-pool. 
    /// </summary>
    /// <param name="transducer">Transducer to fork</param>
    /// <param name="timeout">Maximum time that the forked transducer can run for.  `None` for no timeout.</param>
    /// <returns>Returns a `TFork` data-structure that contains two transducers that can be used to either cancel the
    /// /// forked transducer or to await the result of it.</returns>
    public static Transducer<A, TFork<S>> Fork<S, A, B>(
        this Transducer<A, B> transducer, 
        S initialState,
        Reducer<B, S> reducer,
        Option<TimeSpan> timeout = default) =>
        new ForkTransducer2<S, A, B>(transducer, initialState, reducer, timeout);

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
    public static Transducer<A, B> Retry<A, B>(
        this Transducer<A, B> transducer, 
        Schedule schedule) =>
        retryUntil(schedule, transducer, _ => false);

    /// <summary>
    /// Keep retrying if the transducer fails
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep retrying</param>
    /// <param name="predicate">Predicate that decides whether to continue on each failure.  If it returns
    /// `true` then the retying stops and the `Error` is yielded.</param>
    /// <returns>A transducer that retries</returns>
    public static Transducer<A, B> RetryUntil<A, B>(
        this Transducer<A, B> transducer, 
        Schedule schedule, 
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
    public static Transducer<A, B> RetryWhile<A, B>(
        this Transducer<A, B> transducer,
        Schedule schedule,
        Func<Error, bool> predicate) =>
        retryUntil(schedule, transducer, not(predicate));

    /// <summary>
    /// Keep retrying if the transducer fails
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <returns>A transducer that retries</returns>
    /// <param name="transducer">Transducer to keep retrying</param>
    public static Transducer<RT, Sum<X, A>> Retry<RT, X, A>(
        this Transducer<RT, Sum<X, A>> transducer,
        Schedule schedule) 
        where RT : HasFromError<RT, X> =>
        retryUntil<RT, X, A>(schedule, transducer, _ => false);

    /// <summary>
    /// Keep retrying if the transducer fails
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep retrying</param>
    /// <param name="predicate">Predicate that decides whether to continue on each failure.  If it returns
    /// `true` then the retying stops and the `Error` is yielded.</param>
    /// <returns>A transducer that retries</returns>
    public static Transducer<RT, Sum<X, A>> RetryUntil<RT, X, A>(
        this Transducer<RT, Sum<X, A>> transducer, 
        Schedule schedule, 
        Func<X, bool> predicate) 
        where RT : HasFromError<RT, X> =>
        new RetrySumTransducer<RT, X, A>(transducer, schedule, predicate);

    /// <summary>
    /// Keep retrying if the transducer fails
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep retrying</param>
    /// <param name="predicate">Predicate that decides whether to continue on each repeat.  If it returns
    /// `false` then the repeating stops and the latest value is yielded.</param>
    /// <returns>A transducer that retries</returns>
    public static Transducer<RT, Sum<X, A>> RetryWhile<RT, X, A>(
        this Transducer<RT, Sum<X, A>> transducer,
        Schedule schedule,
        Func<X, bool> predicate) 
        where RT : HasFromError<RT, X> =>
        retryUntil(schedule, transducer, not(predicate));
    
    /// <summary>
    /// Keep retrying if the transducer fails
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <returns>A transducer that retries</returns>
    /// <param name="transducer">Transducer to keep retrying</param>
    public static Transducer<A, B> Retry<A, B>(
        this Transducer<A, B> transducer) =>
        retryUntil(Schedule.Forever, transducer, _ => false);

    /// <summary>
    /// Keep retrying if the transducer fails
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep retrying</param>
    /// <param name="predicate">Predicate that decides whether to continue on each failure.  If it returns
    /// `true` then the retying stops and the `Error` is yielded.</param>
    /// <returns>A transducer that retries</returns>
    public static Transducer<A, B> RetryUntil<A, B>(
        this Transducer<A, B> transducer, 
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
    public static Transducer<A, B> RetryWhile<A, B>(
        this Transducer<A, B> transducer,
        Func<Error, bool> predicate) =>
        retryUntil(Schedule.Forever, transducer, not(predicate));

    /// <summary>
    /// Keep retrying if the transducer fails
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <returns>A transducer that retries</returns>
    /// <param name="transducer">Transducer to keep retrying</param>
    public static Transducer<RT, Sum<X, A>> Retry<RT, X, A>(
        this Transducer<RT, Sum<X, A>> transducer) 
        where RT : HasFromError<RT, X> =>
        retryUntil<RT, X, A>(Schedule.Forever, transducer, _ => false);

    /// <summary>
    /// Keep retrying if the transducer fails
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep retrying</param>
    /// <param name="predicate">Predicate that decides whether to continue on each failure.  If it returns
    /// `true` then the retying stops and the `Error` is yielded.</param>
    /// <returns>A transducer that retries</returns>
    public static Transducer<RT, Sum<X, A>> RetryUntil<RT, X, A>(
        this Transducer<RT, Sum<X, A>> transducer, 
        Func<X, bool> predicate) 
        where RT : HasFromError<RT, X> =>
        new RetrySumTransducer<RT, X, A>(transducer, Schedule.Forever, predicate);

    /// <summary>
    /// Keep retrying if the transducer fails
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep retrying</param>
    /// <param name="predicate">Predicate that decides whether to continue on each repeat.  If it returns
    /// `false` then the repeating stops and the latest value is yielded.</param>
    /// <returns>A transducer that retries</returns>
    public static Transducer<RT, Sum<X, A>> RetryWhile<RT, X, A>(
        this Transducer<RT, Sum<X, A>> transducer,
        Func<X, bool> predicate) 
        where RT : HasFromError<RT, X> =>
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
    public static Transducer<A, B> Repeat<A, B>(
        this Transducer<A, B> transducer,
        Schedule schedule) =>
        repeatUntil(schedule, transducer, _ => false);

    /// <summary>
    /// Keep repeating the transducer until a condition is met
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of repeats and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep repeating</param>
    /// <param name="predicate">Predicate that decides whether to continue on each repeat.  If it returns
    /// `true` then the repeating stops and the latest value is yielded.</param>
    /// <returns>A transducer that repeats</returns>
    public static Transducer<A, B> RepeatUntil<A, B>(
        this Transducer<A, B> transducer, 
        Schedule schedule, 
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
    public static Transducer<A, B> RepeatWhile<A, B>(
        this Transducer<A, B> transducer,
        Schedule schedule,
        Func<B, bool> predicate) =>
        repeatUntil(schedule, transducer, not(predicate));

    /// <summary>
    /// Keep repeating the transducer 
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <returns>A transducer that retries</returns>
    /// <returns>A transducer that repeats</returns>
    public static Transducer<RT, Sum<X, A>> Repeat<RT, X, A>(
        this Transducer<RT, Sum<X, A>> transducer,
        Schedule schedule)
        where RT : HasFromError<RT, X> =>
        repeatUntil<RT, X, A>(schedule, transducer, _ => false);

    /// <summary>
    /// Keep repeating the transducer until a condition is met
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep repeating</param>
    /// <param name="predicate">Predicate that decides whether to continue on each repeat.  If it returns
    /// `true` then the repeating stops and the latest value is yielded.</param>
    /// <returns>A transducer that repeats</returns>
    public static Transducer<RT, Sum<X, A>> RepeatUntil<RT, X, A>(
        this Transducer<RT, Sum<X, A>> transducer, 
        Schedule schedule, 
        Func<A, bool> predicate) 
        where RT : HasFromError<RT, X> =>
        new RepeatSumTransducer<RT, X, A>(transducer, schedule, predicate);

    /// <summary>
    /// Keep repeating the transducer until a condition is met
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of repeats and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep repeating</param>
    /// <param name="predicate">Predicate that decides whether to continue on each repeat.  If it returns
    /// `false` then the repeating stops and the latest value is yielded.</param>
    /// <returns>A transducer that repeats</returns>
    public static Transducer<RT, Sum<X, A>> RepeatWhile<RT, X, A>(
        this Transducer<RT, Sum<X, A>> transducer,
        Schedule schedule,
        Func<A, bool> predicate) 
        where RT : HasFromError<RT, X> =>
        repeatUntil(schedule, transducer, not(predicate));    
    
    /// <summary>
    /// Keep repeating the transducer
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of repeats and the time-gap between each one</param>
    /// <returns>A transducer that repeats</returns>
    /// <param name="transducer">Transducer to keep repeating</param>
    /// <returns>A transducer that repeats</returns>
    public static Transducer<A, B> Repeat<A, B>(
        this Transducer<A, B> transducer) =>
        repeatUntil(Schedule.Forever, transducer, _ => false);

    /// <summary>
    /// Keep repeating the transducer until a condition is met
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of repeats and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep repeating</param>
    /// <param name="predicate">Predicate that decides whether to continue on each repeat.  If it returns
    /// `true` then the repeating stops and the latest value is yielded.</param>
    /// <returns>A transducer that repeats</returns>
    public static Transducer<A, B> RepeatUntil<A, B>(
        this Transducer<A, B> transducer, 
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
    public static Transducer<A, B> RepeatWhile<A, B>(
        this Transducer<A, B> transducer,
        Func<B, bool> predicate) =>
        repeatUntil(Schedule.Forever, transducer, not(predicate));

    /// <summary>
    /// Keep repeating the transducer 
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <returns>A transducer that retries</returns>
    /// <returns>A transducer that repeats</returns>
    public static Transducer<RT, Sum<X, A>> Repeat<RT, X, A>(
        this Transducer<RT, Sum<X, A>> transducer)
        where RT : HasFromError<RT, X> =>
        repeatUntil<RT, X, A>(Schedule.Forever, transducer, _ => false);

    /// <summary>
    /// Keep repeating the transducer until a condition is met
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of retries and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep repeating</param>
    /// <param name="predicate">Predicate that decides whether to continue on each repeat.  If it returns
    /// `true` then the repeating stops and the latest value is yielded.</param>
    /// <returns>A transducer that repeats</returns>
    public static Transducer<RT, Sum<X, A>> RepeatUntil<RT, X, A>(
        this Transducer<RT, Sum<X, A>> transducer, 
        Func<A, bool> predicate) 
        where RT : HasFromError<RT, X> =>
        new RepeatSumTransducer<RT, X, A>(transducer, Schedule.Forever, predicate);

    /// <summary>
    /// Keep repeating the transducer until a condition is met
    /// </summary>
    /// <param name="schedule">Schedule that dictates the number of repeats and the time-gap between each one</param>
    /// <param name="transducer">Transducer to keep repeating</param>
    /// <param name="predicate">Predicate that decides whether to continue on each repeat.  If it returns
    /// `false` then the repeating stops and the latest value is yielded.</param>
    /// <returns>A transducer that repeats</returns>
    public static Transducer<RT, Sum<X, A>> RepeatWhile<RT, X, A>(
        this Transducer<RT, Sum<X, A>> transducer,
        Func<A, bool> predicate) 
        where RT : HasFromError<RT, X> =>
        repeatUntil(Schedule.Forever, transducer, not(predicate));        
    
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Invoking
    //
    
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
    public static TResult<S> Run<S, A, B>(
        this Transducer<A, B> transducer, 
        A value, 
        S initialState, 
        Reducer<B, S> reducer,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null)
    {
        var st = new TState(syncContext ?? SynchronizationContext.Current, token);

        try
        {
            var s = initialState;
            var tf = transducer.Transform(reducer);
            var tr = tf.Run(st, s, value);

            while (!st.Token.IsCancellationRequested)
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
            return TResult.Cancel<S>();
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
    public static TResult<B> Run1<A, B>(
        this Transducer<A, B> transducer, 
        A value, 
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        transducer
           .Run(value, default, Invoke1Reducer<B>.Default, token, syncContext)
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
    public static Task<TResult<S>> RunAsync<S, A, B>(
        this Transducer<A, B> transducer,
        A value,
        S initialState,
        Reducer<B, S> reducer,
        Action? @finally,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        TaskAsync<A>.RunAsync<S>((t, v) => Run(transducer, v, initialState, reducer, t, syncContext), value, @finally, token);
    
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
    public static Task<TResult<B>> Run1Async<A, B>(
        this Transducer<A, B> transducer, 
        A value, 
        Action? @finally,
        CancellationToken token = default,
        SynchronizationContext? syncContext = null) =>
        TaskAsync<A>.RunAsync<B>((t, v) => Run1(transducer, v, t, syncContext), value, @finally, token);

    internal static IEnumerable<Transducer<A, Sum<E, B>>> FlattenChoices<A, E, B>(this IEnumerable<Transducer<A, Sum<E, B>>> items)
    {
        foreach (var item in items)
        {
            if (item is ChoiceTransducer<A, E, B> choice)
            {
                foreach (var citem in choice.Transducers)
                {
                    yield return citem;
                }
            }
            else
            {
                yield return item;
            }
        }
    }
}
