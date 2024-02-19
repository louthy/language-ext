using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes;

/// <summary>
/// The `static Proxy` class is the `Prelude` of the Pipes system.
/// </summary>
public static class Proxy
{
    internal const MethodImplOptions mops = MethodImplOptions.AggressiveInlining;

    /// <summary>
    /// Wait for a value to flow from upstream (whilst in a `Pipe` or a `Consumer`)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<A, A> awaiting<A>() =>
        PureProxy.ConsumerAwait<A>();

    /// <summary>
    /// Send a value flowing downstream (whilst in a `Producer` or a `Pipe`)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Producer<A, Unit> yield<A>(A value) =>
        PureProxy.ProducerYield(value);

    // TODO: Decide whether I want to put these back or not
    /// <summary>
    /// Create a queue
    /// </summary>
    /// <remarks>A `Queue` is a `Producer` with an `Enqueue`, and a `Done` to cancel the operation</remarks>
    // [Pure, MethodImpl(mops)]
    // public static Queue<A, M, Unit> Queue<M, A>() 
    //     where M : Monad<M>
    // {
    //     var c = new Channel<A>();
    //     var p = Producer.yieldAll<M, A>(c);
    //     return new Queue<A, M, Unit>(p, c);
    // }

    // TODO: Decide whether I want to put these back or not
    /// <summary>
    /// Create a `Producer` from an `IEnumerable`.  This will automatically `yield` each value of the
    /// `IEnumerable` down stream
    /// </summary>
    /// <param name="xs">Items to `yield`</param>
    /// <typeparam name="X">Type of the value to `yield`</typeparam>
    /// <returns>`Producer`</returns>
    // [Pure, MethodImpl(mops)]
    // public static Producer<X, Unit> yieldAll<X>(IEnumerable<X> xs) =>
    //     from x in many(xs)
    //     from _ in PureProxy.ProducerYield<X>(x)
    //     select unit;
    //
    // /// <summary>
    // /// Create a `Producer` from an `IAsyncEnumerable`.  This will automatically `yield` each value of the
    // /// `IEnumerable` down stream
    // /// </summary>
    // /// <param name="xs">Items to `yield`</param>
    // /// <typeparam name="X">Type of the value to `yield`</typeparam>
    // /// <returns>`Producer`</returns>
    // [Pure, MethodImpl(mops)]
    // public static Producer<X, Unit> yieldAll<X>(IAsyncEnumerable<X> xs) =>
    //     from x in many(xs)
    //     from _ in PureProxy.ProducerYield<X>(x)
    //     select unit;
    //
    // /// <summary>
    // /// Create a `Producer` from an `IObservable`.  This will automatically `yield` each value of the
    // /// `IObservable` down stream
    // /// </summary>
    // /// <param name="xs">Items to `yield`</param>
    // /// <typeparam name="X">Type of the value to `yield`</typeparam>
    // /// <returns>`Producer`</returns>
    // [Pure, MethodImpl(mops)]
    // public static Producer<X, Unit> yieldAll<X>(IObservable<X> xs) =>
    //     from x in many(xs)
    //     from _ in PureProxy.ProducerYield<X>(x)
    //  select unit;

    // TODO: IMPLEMENT TAIL CALLS
    [Pure, MethodImpl(mops)]
    public static Producer<OUT, M, R> tail<OUT, M, R>(Producer<OUT, M, R> ma) 
        where M : Monad<M> =>
        ma;

    // TODO: IMPLEMENT TAIL CALLS
    [Pure, MethodImpl(mops)]
    public static Consumer<IN, M, R> tail<IN, M, R>(Consumer<IN, M, R> ma) 
        where M : Monad<M> =>
        ma;

    // TODO: IMPLEMENT TAIL CALLS
    [Pure, MethodImpl(mops)]
    public static Pipe<IN, OUT, M, R> tail<IN, OUT, M, R>(Pipe<IN, OUT, M, R> ma) 
        where M : Monad<M> =>
        ma;

    /// <summary>
    /// Repeat the `Producer` indefinitely
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Producer<OUT, M, Unit> repeat<OUT, M, R>(Producer<OUT, M, R> ma) 
        where M : Monad<M> =>
        from r in ma
        from _ in tail(repeat<OUT, M, R>(ma))
        select unit;

    /// <summary>
    /// Repeat the `Consumer` indefinitely
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<IN, M, Unit> repeat<IN, M, R>(Consumer<IN, M, R> ma) 
        where M : Monad<M> =>
        from r in ma
        from _ in tail(repeat<IN, M, R>(ma))
        select unit;

    /// <summary>
    /// Repeat the `Pipe` indefinitely
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<IN, OUT, M, Unit> repeat<IN, OUT, M, R>(Pipe<IN, OUT, M, R> ma) 
        where M : Monad<M> =>
        from r in ma
        from _ in tail(repeat<IN, OUT,M, R>(ma))
        select unit;

    /// <summary>
    /// Lift a monad into the `Proxy` monad transformer
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Proxy<A1, A, B1, B, M, R> lift<A1, A, B1, B, M, R>(K<M, R> ma) 
        where M : Monad<M> =>
        new ProxyM<A1, A, B1, B, M, R>(M.Map(Pure<A1, A, B1, B, M, R>, ma));

    /// <summary>
    /// Lift an IO monad into the `Proxy` monad transformer
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Proxy<A1, A, B1, B, M, R> liftIO<A1, A, B1, B, M, R>(IO<R> ma) 
        where M : Monad<M> =>
        lift<A1, A, B1, B, M, R>(M.LiftIO(ma));

    internal static Unit dispose<A>(A d) where A : IDisposable
    {
        d.Dispose();
        return default;
    }

    internal static Unit anyDispose<A>(A x)
    {
        if (x is IDisposable d)
        {
            d.Dispose();
        }

        return default;
    }

    /// <summary>
    /// The identity `Pipe`, simply replicates its upstream value and propagates it downstream 
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<A, A, M, R> cat<A, M, R>()
        where M : Monad<M> =>
        pull<Unit, A, M, R>(default).ToPipe();

    /// <summary>
    /// Forward requests followed by responses
    ///
    ///    pull = request | respond | pull
    /// 
    /// </summary>
    /// <remarks>
    /// `pull` is the identity of the pull category.
    /// </remarks>
    [Pure, MethodImpl(mops)]
    public static Proxy<UOut, UIn, UOut, UIn, M, A> pull<UOut, UIn, M, A>(UOut a1)
        where M : Monad<M> =>
        new Request<UOut, UIn, UOut, UIn, M, A>(
            a1,
            a => new Respond<UOut, UIn, UOut, UIn, M, A>(
                a,
                pull<UOut, UIn, M, A>));

    /// <summary>
    /// `push = respond | request | push`
    /// </summary>
    /// <remarks>
    /// `push` is the identity of the push category.
    /// </remarks>
    [Pure, MethodImpl(mops)]
    public static Proxy<UOut, UIn, UOut, UIn, M, A> push<UOut, UIn, M, A>(UIn a)
        where M : Monad<M> =>
        new Respond<UOut, UIn, UOut, UIn, M, A>(
            a,
            a1 => new Request<UOut, UIn, UOut, UIn, M, A>(
                a1,
                push<UOut, UIn, M, A>));

    /// <summary>
    /// Send a value of type `DOut` downstream and block waiting for a reply of type `DIn`
    /// </summary>
    /// <remarks>
    /// `respond` is the identity of the respond category.
    /// </remarks>
    [Pure, MethodImpl(mops)]
    public static Proxy<X1, X, DIn, DOut, M, DIn> respond<X1, X, DIn, DOut, M>(DOut value) 
        where M : Monad<M> =>
        new Respond<X1, X, DIn, DOut, M, DIn>(value, r => new Pure<X1, X, DIn, DOut, M, DIn>(r));

    /// <summary>
    /// Send a value of type `UOut` upstream and block waiting for a reply of type `UIn`
    /// </summary>
    /// <remarks>
    /// `request` is the identity of the request category.
    /// </remarks>
    [Pure, MethodImpl(mops)]
    public static Proxy<UOut, UIn, Y1, Y, M, UIn> request<UOut, UIn, Y1, Y, M>(UOut value) 
        where M : Monad<M> =>
        new Request<UOut, UIn, Y1, Y, M, UIn>(value, r => new Pure<UOut, UIn, Y1, Y, M, UIn>(r));


    /// <summary>
    /// `reflect` transforms each streaming category into its dual:
    ///
    /// The request category is the dual of the respond category
    ///
    ///      reflect . respond = request
    ///      reflect . (f | g) = reflect . f | reflect . g
    ///      reflect . request = respond
    ///      reflect . (f | g) = reflect . f | reflect . g
    ///
    /// The pull category is the dual of the push category
    ///
    ///      reflect . push = pull
    ///      reflect . (f | g) = reflect . f | reflect . g
    ///      reflect . pull = push
    ///      reflect . (f | g) = reflect . f | reflect . g
    ///
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Proxy<DOut, DIn, UIn, UOut, M, R> reflect<UOut, UIn, DIn, DOut, M, R>(
        Proxy<UOut, UIn, DIn, DOut, M, R> p)
        where M : Monad<M> =>
        p.Reflect();

    /// <summary>
    /// `p.ForEach(body)` loops over the `Producer p` replacing each `yield` with `body`
    /// 
    ///     Producer b r -> (b -> Producer c ()) -> Producer c r
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Producer<OUT_B, M, A> ForEach<OUT_A, OUT_B, M, A>(
        this Producer<OUT_A, M, A> p, 
        Func<OUT_A, Producer<OUT_B, M, Unit>> body)
        where M : Monad<M> =>
        p.For(body).ToProducer();

    /// <summary>
    /// `p.ForEach(body)` loops over `Producer p` replacing each `yield` with `body`
    /// 
    ///     Producer b r -> (b -> Effect ()) -> Effect r
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Effect<M, A> ForEach<OUT, M, A>(
        this Producer<OUT, M, A> p, 
        Func<OUT, Effect<M, Unit>> fb)
        where M : Monad<M> =>
        p.For(fb).ToEffect();

    /// <summary>
    /// `p.ForEach(body)` loops over `Pipe p` replacing each `yield` with `body`
    /// 
    ///     Pipe x b r -> (b -> Consumer x ()) -> Consumer x r
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<IN, M, A> ForEach<IN, OUT, M, A>(
        this Pipe<IN, OUT, M, A> p0, 
        Func<OUT, Consumer<IN, M, Unit>> fb)
        where M : Monad<M> =>
        p0.For(fb).ToConsumer();

    /// <summary>
    /// `p.ForEach(body)` loops over `Pipe p` replacing each `yield` with `body`
    /// 
    ///     Pipe x b r -> (b -> Pipe x c ()) -> Pipe x c r
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<IN, OUT, M, R> ForEach<IN, B, OUT, M, R>(
        this Pipe<IN, B, M, R> p0, 
        Func<B, Pipe<IN, OUT, M, Unit>> fb)
        where M : Monad<M> =>
        p0.For(fb).ToPipe();

    /// <summary>
    /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Proxy<UOut, UIn, DIn, DOut, M, B> compose<UOut, UIn, DIn, DOut, A, M, B>(
        Proxy<UOut, UIn, DIn, DOut, M, A> p1, 
        Proxy<Unit, A, DIn, DOut, M, B> p2)
        where M : Monad<M> =>
        compose(_ => p1, p2);

    /// <summary>
    /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
    /// 
    ///     Effect b -> Consumer b c -> Effect c
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Effect<M, A> compose<OUT, M, A>(
        Effect<M, OUT> p1, 
        Consumer<OUT, M, A> p2)
        where M : Monad<M> =>
        compose(_ => p1, p2).ToEffect();

    /// <summary>
    /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
    /// 
    ///     Consumer a b -> Consumer b c -> Consumer a c
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<A, M, C> compose<A, B, M, C>(
        Consumer<A, M, B> p1, 
        Consumer<B, M, C> p2) 
        where M : Monad<M> =>
        compose(_ => p1, p2).ToConsumer();

    /// <summary>
    /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
    /// 
    ///     Producer y b -> Pipe b y m c -> Producer y c
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Producer<OUT, M, C> compose<OUT, IN, M, C>(
        Producer<OUT, M, IN> p1, 
        Pipe<IN, OUT, M, C> p2) 
        where M : Monad<M> =>
        compose(_ => p1, p2).ToProducer();

    /// <summary>
    /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
    /// 
    ///     Pipe a y b -> Pipe b y c -> Pipe a y c
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<A, Y, M, C> compose<Y, A, B, M, C>(
        Pipe<A, Y, M, B> p1,
        Pipe<B, Y, M, C> p2) 
        where M : Monad<M> =>
        compose(_ => p1, p2).ToPipe();

    // fixAwaitDual
    [Pure, MethodImpl(mops)]
    public static Proxy<A1, A, Y1, Y, M, C> compose<A1, A, Y1, Y, B, M, C>(
        Proxy<Unit, B, Y1, Y, M, C> p2,
        Proxy<A1, A, Y1, Y, M, B> p1) 
        where M : Monad<M> =>
        compose(p1, p2);

    /// <summary>
    /// Replaces each `request` or `respond` in `p0` with `fb1`.
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Proxy<A1, A, Y1, Y, M, C> compose<A1, A, B1, B, Y1, Y, M, C>(
        Func<B1, Proxy<A1, A, Y1, Y, M, B>> fb1,
        Proxy<B1, B, Y1, Y, M, C> p0) 
        where M : Monad<M> =>
        p0.ReplaceRequest(fb1);

    /// <summary>
    /// `compose(p, f)` pairs each `respond` in `p` with a `request` in `f`.
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Proxy<A1, A, C1, C, M, R> compose<A1, A, B1, B, C1, C, M, R>(
        Proxy<A1, A, B1, B, M, R> p,
        Func<B, Proxy<B1, B, C1, C, M, R>> fb)
        where M : Monad<M> =>
        p.PairEachRespondWithRequest(fb);

    /// <summary>
    /// `compose(f, p)` pairs each `request` in `p` with a `respond` in `f`
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Proxy<A1, A, C1, C, M, R> compose<A1, A, B1, B, C1, C, M, R>(
        Func<B1, Proxy<A1, A, B1, B, M, R>> fb1,
        Proxy<B1, B, C1, C, M, R> p)
        where M : Monad<M> =>
        p.PairEachRequestWithRespond(fb1);

    /// <summary>
    /// Pipe composition
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Proxy<A1, A, C1, C, M, R> compose<A1, A, B, C1, C, M, R>(
        Proxy<A1, A, Unit, B, M, R> p1,
        Proxy<Unit, B, C1, C, M, R> p2)
        where M : Monad<M> =>
        compose(_ => p1, p2);

    /// <summary>
    /// Pipe composition
    ///
    ///     Producer b r -> Consumer b r -> Effect m r
    /// 
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Effect<M, R> compose<B, M, R>(
        Producer<B, M, R> p1,
        Consumer<B, M, R> p2)
        where M : Monad<M> =>
        compose(p1.ToProxy(), p2).ToEffect();

    /// <summary>
    /// Pipe composition
    ///
    ///     Producer b r -> Pipe b c r -> Producer c r
    /// 
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Producer<C, M, R> compose<B, C, M, R>(
        Producer<B, M, R> p1,
        Pipe<B, C, M, R> p2)
        where M : Monad<M> =>
        compose(p1.ToProxy(), p2).ToProducer();

    /// <summary>
    /// Pipe composition
    ///
    ///     Pipe a b r -> Consumer b r -> Consumer a r
    /// 
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Consumer<A, M, R> compose<A, B, M, R>(
        Pipe<A, B, M, R> p1,
        Consumer<B, M, R> p2)
        where M : Monad<M> =>
        compose(p1.ToProxy(), p2).ToConsumer();

    /// <summary>
    /// Pipe composition
    ///
    ///     Pipe a b r -> Pipe b c r -> Pipe a c r
    /// 
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<A, C, M, R> compose<A, B, C, M, R>(
        Pipe<A, B, M, R> p1,
        Pipe<B, C, M, R> p2)
        where M : Monad<M> =>
        compose(p1.ToProxy(), p2).ToPipe();

    /// <summary>
    /// Compose two unfolds, creating a new unfold
    /// </summary>
    /// <remarks>
    /// This is the composition operator of the respond category.
    /// </remarks>
    [Pure, MethodImpl(mops)]
    public static Func<A, Proxy<X1, X, C1, C, M, A1>> compose<X1, X, A1, A, B1, B, C1, M, C>(
        Func<A, Proxy<X1, X, B1, B, M, A1>> fa,
        Func<B, Proxy<X1, X, C1, C, M, B1>> fb) 
        where M : Monad<M> =>
        a => compose(fa(a), fb);

    /// <summary>
    /// Compose two unfolds, creating a new unfold
    /// </summary>
    /// <remarks>
    /// This is the composition operator of the respond category.
    /// </remarks>
    [Pure, MethodImpl(mops)]
    public static Func<A, Proxy<X1, X, C1, C, M, A1>> Then<X1, X, A1, A, B1, B, C1, M, C>(
        this Func<A, Proxy<X1, X, B1, B, M, A1>> fa,
        Func<B, Proxy<X1, X, C1, C, M, B1>> fb) 
        where M : Monad<M> =>
        a => compose(fa(a), fb);

    /// <summary>
    /// `compose(p, f)` replaces each `respond` in `p` with `f`.
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Proxy<X1, X, C1, C, M, A1> compose<X1, X, A1, B1, C1, C, M, B>(
        Proxy<X1, X, B1, B, M, A1> p0,
        Func<B, Proxy<X1, X, C1, C, M, B1>> fb) 
        where M : Monad<M> =>
        p0.ReplaceRespond(fb);

    /// <summary>
    /// `compose(p, f)` replaces each `respond` in `p` with `f`.
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Proxy<X1, X, C1, C, M, A1> Then<X1, X, A1, B1, C1, C, M, B>(
        this Proxy<X1, X, B1, B, M, A1> p0,
        Func<B, Proxy<X1, X, C1, C, M, B1>> fb) 
        where M : Monad<M> =>
        compose(p0, fb);


    /// <summary>
    ///  Compose two folds, creating a new fold
    /// 
    ///     (f | g) x = f | g x
    /// 
    ///     | is the composition operator of the request category.
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Func<C1, Proxy<A1, A, Y1, Y, M, C>> compose<A1, A, B1, B, Y1, Y, C1, M, C>(
        Func<B1, Proxy<A1, A, Y1, Y, M, B>> fb1,
        Func<C1, Proxy<B1, B, Y1, Y, M, C>> fc1) 
        where M : Monad<M> =>
        c1 => compose(fb1, fc1(c1));

    /// <summary>
    /// 
    ///     observe(lift (Pure(r))) = observe(Pure(r))
    ///     observe(lift (m.Bind(f))) = observe(lift(m.Bind(x => lift(f(x)))))
    /// 
    /// This correctness comes at a small cost to performance, so use this function sparingly.
    /// This function is a convenience for low-level pipes implementers.  You do not need to
    /// use observe if you stick to the safe API.        
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Proxy<A1, A, B1, B, M, R> observe<A1, A, B1, B, M, R>(
        Proxy<A1, A, B1, B, M, R> p0)
        where M : Monad<M> =>
        p0.Observe();

    /// <summary>
    /// `Absurd` function
    /// </summary>
    /// <param name="value">`Void` is supposed to represent `void`, nothing can be constructed from `void` and
    /// so this method just throws `ApplicationException("closed")`</param>
    [Pure, MethodImpl(mops)]
    public static A closed<A>(Void value) =>
        throw new ApplicationException("closed");

    /// <summary>
    /// Applicative apply
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Proxy<A1, A, B1, B, M, S> apply<A1, A, B1, B, R, M, S>(
        Proxy<A1, A, B1, B, M, Func<R, S>> pf,
        Proxy<A1, A, B1, B, M, R> px) where M : Monad<M>
    {
        return Go(pf);

        Proxy<A1, A, B1, B, M, S> Go(Proxy<A1, A, B1, B, M, Func<R, S>> p) =>
            p.ToProxy() switch
            {
                Request<A1, A, B1, B, M, Func<R, S>> (var a1, var fa) => new Request<A1, A, B1, B, M, S>(a1, a => Go(fa(a))),
                Respond<A1, A, B1, B, M, Func<R, S>> (var b, var fb1) => new Respond<A1, A, B1, B, M, S>(b, b1 => Go(fb1(b1))),
                ProxyM<A1, A, B1, B, M, Func<R, S>> (var m)           => new ProxyM<A1, A, B1, B, M, S>(M.Map(Go, m)),
                Pure<A1, A, B1, B, M, Func<R, S>> (var f)             => px.Map(f),
                _                                                     => throw new NotSupportedException()
            };
    }

    /// <summary>
    /// Applicative apply
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Proxy<A1, A, B1, B, M, S> Apply<A1, A, B1, B, R, M, S>(
        this Proxy<A1, A, B1, B, M, Func<R, S>> pf, 
        Proxy<A1, A, B1, B, M, R> px) 
        where M : Monad<M> =>
        apply(pf, px);

    /// <summary>
    /// Applicative action
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Proxy<A1, A, B1, B, M, S> Action<A1, A, B1, B, R, M, S>(
        this Proxy<A1, A, B1, B, M, R> l, 
        Proxy<A1, A, B1, B, M, S> r) 
        where M : Monad<M> =>
        l.Action(r);

    /// <summary>
    /// Monad return / pure
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Proxy<A1, A, B1, B, M, R> Pure<A1, A, B1, B, M, R>(R value) 
        where M : Monad<M> =>
        new Pure<A1, A, B1, B, M, R>(value);

    /// <summary>
    /// Creates a non-yielding producer that returns the result of the effects
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static K<M, (A, B)> collect<M, A, B>(Effect<M, A> ma, Effect<M, B> mb)
        where M : Monad<M> =>
        fun((A x, B y) => (x, y)).Map(ma.RunEffect()).Apply(mb.RunEffect());
 
    /// <summary>
    /// Creates a non-yielding producer that returns the result of the effects
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static K<M, (A, B, C)> collect<M, A, B, C>(Effect<M, A> ma, Effect<M, B> mb, Effect<M, C> mc) 
        where M : Monad<M> =>
        fun((A x, B y, C z) => (x, y, z)).Map(ma.RunEffect()).Apply(mb.RunEffect()).Apply(mc.RunEffect());

    /// <summary>
    /// Creates a non-yielding producer that returns the result of the effects
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Producer<(A, B), M, Unit> yield<M, A, B>(Effect<M, A> ma, Effect<M, B> mb) 
        where M : Monad<M> =>
        from r in collect(ma, mb)
        from _ in yield(r)
        select unit;

    /// <summary>
    /// Creates a non-yielding producer that returns the result of the effects
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Producer<(A, B, C), M, Unit> yield<M, A, B, C>(Effect<M, A> ma, Effect<M, B> mb, Effect<M, C> mc) 
        where M : Monad<M> =>
        from r in collect(ma, mb, mc)
        from _ in yield(r)
        select unit;

    /// <summary>
    /// Only forwards values that satisfy the predicate.
    /// </summary>
    public static Pipe<A, A, Unit> filter<A>(Func<A, bool> f) =>
        from x in awaiting<A>()
        from r in f(x) ? yield(x) : Prelude.Pure(unit)
        select r;

    /// <summary>
    /// Map the output of the pipe (not the bound value as is usual with Map)
    /// </summary>
    public static Pipe<A, B, Unit> map<A, B>(Func<A, B> f) =>
        from x in awaiting<A>()
        from r in yield(f(x))
        select r;

    /// <summary>
    /// Folds values coming down-stream, when the predicate returns false the folded value is yielded 
    /// </summary>
    /// <param name="Initial">Initial state</param>
    /// <param name="Fold">Fold operation</param>
    /// <param name="WhileState">Predicate</param>
    /// <returns>A pipe that folds</returns>
    [Pure, MethodImpl(mops)]
    public static Pipe<IN, OUT, Unit> foldWhile<IN, OUT>(OUT Initial, Func<OUT, IN, OUT> Fold, Func<OUT, bool> State) => 
        foldUntil(Initial, Fold, x => !State(x));
 
    /// <summary>
    /// Folds values coming down-stream, when the predicate returns true the folded value is yielded 
    /// </summary>
    /// <param name="Initial">Initial state</param>
    /// <param name="Fold">Fold operation</param>
    /// <param name="UntilState">Predicate</param>
    /// <returns>A pipe that folds</returns>
    public static Pipe<IN, OUT, Unit> foldUntil<IN, OUT>(OUT Initial, Func<OUT, IN, OUT> Fold, Func<OUT, bool> State)
    {
        var state = Initial;
        return awaiting<IN>()
           .Bind(x =>
                 {
                     state = Fold(state, x);
                     if (State(state))
                     {
                         var nstate = state;
                         state = Initial;
                         return yield(nstate);
                     }
                     else
                     {
                         return Prelude.Pure(unit);
                     }
                 });
    }        
        
    /// <summary>
    /// Folds values coming down-stream, when the predicate returns false the folded value is yielded 
    /// </summary>
    /// <param name="Initial">Initial state</param>
    /// <param name="Fold">Fold operation</param>
    /// <param name="WhileValue">Predicate</param>
    /// <returns>A pipe that folds</returns>
    [Pure, MethodImpl(mops)]
    public static Pipe<IN, OUT, Unit> foldWhile<IN, OUT>(OUT Initial, Func<OUT, IN, OUT> Fold, Func<IN, bool> Value) => 
        foldUntil(Initial, Fold, x => !Value(x));
 
    /// <summary>
    /// Folds values coming down-stream, when the predicate returns true the folded value is yielded 
    /// </summary>
    /// <param name="Initial">Initial state</param>
    /// <param name="Fold">Fold operation</param>
    /// <param name="UntilValue">Predicate</param>
    /// <returns>A pipe that folds</returns>
    public static Pipe<IN, OUT, Unit> foldUntil<IN, OUT>(OUT Initial, Func<OUT, IN, OUT> Fold, Func<IN, bool> Value)
    {
        var state = Initial;
        return awaiting<IN>()
           .Bind(x =>
                 {
                     if (Value(x))
                     {
                         var nstate = state;
                         state = Initial;
                         return yield(nstate);
                     }
                     else
                     {
                         state = Fold(state, x);
                         return Prelude.Pure(unit);
                     }
                 });
    }
}
