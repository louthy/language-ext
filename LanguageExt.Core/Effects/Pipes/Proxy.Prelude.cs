using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes
{
    /// <summary>
    /// The `static Proxy` class is the `Prelude` of the Pipes system.
    /// </summary>
    public static partial class Proxy
    {
        internal const MethodImplOptions mops = MethodImplOptions.AggressiveInlining;

        /// <summary>
        /// Monad return / pure
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pure<A> Pure<A>(A value) =>
            new (value);

        /// <summary>
        /// Wait for a value to flow from upstream (whilst in a `Pipe` or a `Consumer`)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<A, A> awaiting<A>() =>
            PureProxy.ConsumerAwait<A>();

        /// <summary>
        /// Send a value flowing downstream (whilst in a `Producer` or a `Pipe`)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<A, Unit> yield<A>(A value) =>
            PureProxy.ProducerYield(value);

        /// <summary>
        /// Create a queue
        /// </summary>
        /// <remarks>A `Queue` is a `Producer` with an `Enqueue`, and a `Done` to cancel the operation</remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Queue<RT, A, Unit> Queue<RT, A>() where RT : struct, HasCancel<RT>
        {
            var c = new Channel<A>();
            var p = Producer.enumerate<RT, A>(c);
            return new Queue<RT, A, Unit>(p, c);
        }

        /// <summary>
        /// Create a `Producer` from an `IEnumerable`.  This will automatically `yield` each value of the
        /// `IEnumerable` down stream
        /// </summary>
        /// <param name="xs">Items to `yield`</param>
        /// <typeparam name="X">Type of the value to `yield`</typeparam>
        /// <returns>`Producer`</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<X, Unit> enumerate<X>(IEnumerable<X> xs) =>
            enumerate2(xs).Bind(yield);
        
        /// <summary>
        /// Create a `Producer` from an `IEnumerable`.  This will **not** automatically `yield` each value
        /// of the `IEnumerable` down stream, it will return each of the values in the `IEnumerable` as the
        /// bound value, so you can first transform it (for example), before yielding down stream.
        /// </summary>
        /// <example>
        ///
        ///     from text  in enumerate(list)
        ///     from value in parseInt(text).Case switch
        ///                   {
        ///                       int x => yield(x),
        ///                       _     => Pure(unit)
        ///                   }
        ///     select unit;
        /// 
        /// <param name="xs">Items to `yield`</param>
        /// <typeparam name="X">Type of the value to `yield`</typeparam>
        /// <returns>`Producer`</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<OUT, X> enumerate<OUT, X>(IEnumerable<X> xs) =>
            PureProxy.ProducerEnumerate<OUT, X>(xs);

        /// <summary>
        /// Create a `Producer` from an `IEnumerable`.  This will **not** automatically `yield` each value
        /// of the `IEnumerable` down stream, it will return each of the values in the `IEnumerable` as the
        /// bound value, so you can first transform it (for example), before yielding down stream.
        /// </summary>
        /// <example>
        /// <typeparam name="X">Type of the value to `yield`</typeparam>
        /// <returns>`Producer`</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<X, X> enumerate2<X>(IEnumerable<X> xs) =>
            PureProxy.ProducerEnumerate<X, X>(xs);

        /// <summary>
        /// Create a `Producer` from an `IAsyncEnumerable`.  This will automatically `yield` each value of the
        /// `IEnumerable` down stream
        /// </summary>
        /// <param name="xs">Items to `yield`</param>
        /// <typeparam name="X">Type of the value to `yield`</typeparam>
        /// <returns>`Producer`</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<X, Unit> enumerate<X>(IAsyncEnumerable<X> xs) =>
            enumerate2<X>(xs).Bind(yield);
        
        /// <summary>
        /// Create a `Producer` from an `IAsyncEnumerable`.  This will **not** automatically `yield` each value
        /// of the `IEnumerable` down stream, it will return each of the values in the `IAsyncEnumerable` as the
        /// bound value, so you can first transform it (for example), before yielding down stream.
        /// </summary>
        /// <example>
        ///
        ///     from text  in enumerate(list)
        ///     from value in parseInt(text).Case switch
        ///                   {
        ///                       int x => yield(x),
        ///                       _     => Pure(unit)
        ///                   }
        ///     select unit;
        /// 
        /// <param name="xs">Items to `yield`</param>
        /// <typeparam name="X">Type of the value to `yield`</typeparam>
        /// <returns>`Producer`</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<OUT, X> enumerate<OUT, X>(IAsyncEnumerable<X> xs) =>
            PureProxy.ProducerEnumerate<OUT, X>(xs);

        /// <summary>
        /// Create a `Producer` from an `IAsyncEnumerable`.  This will **not** automatically `yield` each value
        /// of the `IEnumerable` down stream, it will return each of the values in the `IAsyncEnumerable` as the
        /// bound value, so you can first transform it (for example), before yielding down stream.
        /// </summary>
        /// <example>
        ///
        ///     from text  in enumerate(list)
        ///     from value in parseInt(text).Case switch
        ///                   {
        ///                       int x => yield(x),
        ///                       _     => Pure(unit)
        ///                   }
        ///     select unit;
        /// 
        /// <param name="xs">Items to `yield`</param>
        /// <typeparam name="X">Type of the value to `yield`</typeparam>
        /// <returns>`Producer`</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<X, X> enumerate2<X>(IAsyncEnumerable<X> xs) =>
            PureProxy.ProducerEnumerate<X, X>(xs);

        /// <summary>
        /// Create a `Producer` from an `IObservable`.  This will automatically `yield` each value of the
        /// `IObservable` down stream
        /// </summary>
        /// <param name="xs">Items to `yield`</param>
        /// <typeparam name="X">Type of the value to `yield`</typeparam>
        /// <returns>`Producer`</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<X, Unit> observe<X>(IObservable<X> xs) =>
            observeX<X>(xs).Bind(yield);

        /// <summary>
        /// Create a `Producer` from an `IObservable`.  This will **not** automatically `yield` each value
        /// of the `IObservable` down stream, it will return each of the values in the `IObservable` as the
        /// bound value, so you can first transform it (for example), before yielding down stream.
        /// </summary>
        /// <example>
        ///
        ///     from text  in enumerate(list)
        ///     from value in parseInt(text).Case switch
        ///                   {
        ///                       int x => yield(x),
        ///                       _     => Pure(unit)
        ///                   }
        ///     select unit;
        /// 
        /// <param name="xs">Items to `yield`</param>
        /// <typeparam name="X">Type of the value to `yield`</typeparam>
        /// <returns>`Producer`</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<OUT, X> observe<OUT, X>(IObservable<X> xs) =>
            PureProxy.ProducerObserve<OUT, X>(xs);

        /// <summary>
        /// Create a `Producer` from an `IObservable`.  This will **not** automatically `yield` each value
        /// of the `IObservable` down stream, it will return each of the values in the `IObservable` as the
        /// bound value, so you can first transform it (for example), before yielding down stream.
        /// </summary>
        /// <example>
        ///
        ///     from text  in enumerate(list)
        ///     from value in parseInt(text).Case switch
        ///                   {
        ///                       int x => yield(x),
        ///                       _     => Pure(unit)
        ///                   }
        ///     select unit;
        /// 
        /// <param name="xs">Items to `yield`</param>
        /// <typeparam name="X">Type of the value to `yield`</typeparam>
        /// <returns>`Producer`</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<X, X> observeX<X>(IObservable<X> xs) =>
            PureProxy.ProducerObserve<X, X>(xs);

        /// <summary>
        /// Lift the `Eff` monad into the monad transformer 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Lift<RT, R> lift<RT, R>(Eff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            Lift.Eff<RT, R>(ma);

        /// <summary>
        /// Lift the `Aff` monad into the monad transformer 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Lift<RT, R> lift<RT, R>(Aff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            Lift.Aff<RT, R>(ma);

        /// <summary>
        /// Lift the `Eff` monad into the monad transformer 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Lift<RT, R> lift<RT, R>(Eff<R> ma) where RT : struct, HasCancel<RT> =>
            Lift.Eff<RT, R>(ma);

        /// <summary>
        /// Lift the `Aff` monad into the monad transformer 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Lift<RT, R> lift<RT, R>(Aff<R> ma) where RT : struct, HasCancel<RT> =>
            Lift.Aff<RT, R>(ma);

        /// <summary>
        /// Lift the `Eff` monad into the monad transformer 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Lift<RT, R> use<RT, R>(Eff<RT, R> ma)
            where RT : struct, HasCancel<RT>
            where R : IDisposable =>
            Lift.Eff<RT, R>(ma);

        /// <summary>
        /// Lift the `Aff` monad into the monad transformer 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Lift<RT, R> use<RT, R>(Aff<RT, R> ma)
            where RT : struct, HasCancel<RT>
            where R : IDisposable =>
            Lift.Aff<RT, R>(ma);

        /// <summary>
        /// Lift the `Eff` monad into the monad transformer 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Lift<RT, R> use<RT, R>(Eff<R> ma)
            where RT : struct, HasCancel<RT>
            where R : IDisposable =>
            Lift.Eff<RT, R>(ma);

        /// <summary>
        /// Lift the `Aff` monad into the monad transformer 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Lift<RT, R> use<RT, R>(Aff<R> ma)
            where RT : struct, HasCancel<RT>
            where R : IDisposable =>
            Lift.Aff<RT, R>(ma);

        /// <summary>
        /// Release a previously used resource
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Release<Unit> release<A>(A value) =>
            new Release<Unit>.Do<A>(value, PureProxy.ReleasePure<Unit>);

        /// <summary>
        /// Repeat the `Producer` indefinitely
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT, R> repeat<RT, OUT, R>(Producer<RT, OUT, R> ma) where RT : struct, HasCancel<RT> =>
            new Repeat<RT, Void, Unit, Unit, OUT, R>(ma).ToProducer();

        /// <summary>
        /// Repeat the `Consumer` indefinitely
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, R> repeat<RT, IN, R>(Consumer<RT, IN, R> ma) where RT : struct, HasCancel<RT> =>
            new Repeat<RT, Unit, IN, Unit, Void, R>(ma).ToConsumer();

        /// <summary>
        /// Repeat the `Pipe` indefinitely
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, R> repeat<RT, IN, OUT, R>(Pipe<RT, IN, OUT, R> ma) where RT : struct, HasCancel<RT> =>
            new Repeat<RT, Unit, IN, Unit, OUT, R>(ma).ToPipe();

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, R> lift<RT, A1, A, B1, B, R>(Aff<R> ma) where RT : struct, HasCancel<RT> =>
            Disposable<R>.IsDisposable
                ? use<RT, A1, A, B1, B, R>(ma, anyDispose)
                : new M<RT, A1, A, B1, B, R>(ma.Map(Pure<RT, A1, A, B1, B, R>).WithRuntime<RT>());

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, R> lift<RT, A1, A, B1, B, R>(Eff<R> ma) where RT : struct, HasCancel<RT> =>
            Disposable<R>.IsDisposable
                ? use<RT, A1, A, B1, B, R>(ma, anyDispose)
                : new M<RT, A1, A, B1, B, R>(ma.Map(Pure<RT, A1, A, B1, B, R>).ToAffWithRuntime<RT>());

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, R> lift<RT, A1, A, B1, B, R>(Aff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            Disposable<R>.IsDisposable
                ? use<RT, A1, A, B1, B, R>(ma, anyDispose)
                : new M<RT, A1, A, B1, B, R>(ma.Map(Pure<RT, A1, A, B1, B, R>));

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, R> lift<RT, A1, A, B1, B, R>(Eff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            Disposable<R>.IsDisposable
                ? use<RT, A1, A, B1, B, R>(ma, anyDispose)
                : new M<RT, A1, A, B1, B, R>(ma.Map(Pure<RT, A1, A, B1, B, R>).ToAff());

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, R> use<RT, A1, A, B1, B, R>(Aff<R> ma)
            where RT : struct, HasCancel<RT>
            where R : IDisposable =>
            new Use<RT, A1, A, B1, B, R, R>(ma.WithRuntime<RT>, dispose, Pure<RT, A1, A, B1, B, R>);

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, R> use<RT, A1, A, B1, B, R>(Eff<R> ma)
            where RT : struct, HasCancel<RT>
            where R : IDisposable =>
            new Use<RT, A1, A, B1, B, R, R>(ma.ToAffWithRuntime<RT>, dispose, Pure<RT, A1, A, B1, B, R>);

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, R> use<RT, A1, A, B1, B, R>(Aff<RT, R> ma)
            where RT : struct, HasCancel<RT>
            where R : IDisposable =>
            new Use<RT, A1, A, B1, B, R, R>(() => ma, dispose, Pure<RT, A1, A, B1, B, R>);

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, R> use<RT, A1, A, B1, B, R>(Eff<RT, R> ma)
            where RT : struct, HasCancel<RT>
            where R : IDisposable =>
            new Use<RT, A1, A, B1, B, R, R>(() => ma, dispose, Pure<RT, A1, A, B1, B, R>);

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, R> use<RT, A1, A, B1, B, R>(Aff<R> ma, Func<R, Unit> dispose)
            where RT : struct, HasCancel<RT> =>
            new Use<RT, A1, A, B1, B, R, R>(ma.WithRuntime<RT>, dispose, Pure<RT, A1, A, B1, B, R>);

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, R> use<RT, A1, A, B1, B, R>(Eff<R> ma, Func<R, Unit> dispose)
            where RT : struct, HasCancel<RT> =>
            new Use<RT, A1, A, B1, B, R, R>(ma.ToAffWithRuntime<RT>, dispose, Pure<RT, A1, A, B1, B, R>);

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, R> use<RT, A1, A, B1, B, R>(Aff<RT, R> ma, Func<R, Unit> dispose)
            where RT : struct, HasCancel<RT> =>
            new Use<RT, A1, A, B1, B, R, R>(() => ma, dispose, Pure<RT, A1, A, B1, B, R>);

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, R> use<RT, A1, A, B1, B, R>(Eff<RT, R> ma, Func<R, Unit> dispose)
            where RT : struct, HasCancel<RT> =>
            new Use<RT, A1, A, B1, B, R, R>(() => ma, dispose, Pure<RT, A1, A, B1, B, R>);

        /// <summary>
        /// Release a previously used resource
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, Unit> release<RT, A1, A, B1, B, R>(R dispose)
            where RT : struct, HasCancel<RT> =>
            new Release<RT, A1, A, B1, B, R, Unit>(dispose, Pure<RT, A1, A, B1, B, Unit>);

        internal static Unit dispose<A>(A d) where A : IDisposable
        {
            d?.Dispose();
            return default;
        }

        internal static Unit anyDispose<A>(A x)
        {
            if (x is IDisposable d)
            {
                d?.Dispose();
            }

            return default;
        }

        /// <summary>
        /// The identity `Pipe`, simply replicates its upstream value and propagates it downstream 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, A, R> cat<RT, A, R>() where RT : struct, HasCancel<RT> =>
            pull<RT, Unit, A, R>(default).ToPipe();

        /// <summary>
        /// Forward requests followed by responses
        ///
        ///    pull = request | respond | pull
        /// 
        /// </summary>
        /// <remarks>
        /// `pull` is the identity of the pull category.
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, UOut, UIn, UOut, UIn, A> pull<RT, UOut, UIn, A>(UOut a1) where RT : struct, HasCancel<RT> =>
            new Request<RT, UOut, UIn, UOut, UIn, A>(a1, a => new Respond<RT, UOut, UIn, UOut, UIn, A>(a, pull<RT, UOut, UIn, A>));

        /// <summary>
        /// `push = respond | request | push`
        /// </summary>
        /// <remarks>
        /// `push` is the identity of the push category.
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, UOut, UIn, UOut, UIn, A> push<RT, UOut, UIn, A>(UIn a) where RT : struct, HasCancel<RT> =>
            new Respond<RT, UOut, UIn, UOut, UIn, A>(a, a1 => new Request<RT, UOut, UIn, UOut, UIn, A>(a1, push<RT, UOut, UIn, A>));

        /// <summary>
        /// Send a value of type `DOut` downstream and block waiting for a reply of type `DIn`
        /// </summary>
        /// <remarks>
        /// `respond` is the identity of the respond category.
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, X1, X, DIn, DOut, DIn> respond<RT, X1, X, DIn, DOut>(DOut value) where RT : struct, HasCancel<RT> =>
            new Respond<RT, X1, X, DIn, DOut, DIn>(value, r => new Pure<RT, X1, X, DIn, DOut, DIn>(r));

        /// <summary>
        /// Send a value of type `UOut` upstream and block waiting for a reply of type `UIn`
        /// </summary>
        /// <remarks>
        /// `request` is the identity of the request category.
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, UOut, UIn, Y1, Y, UIn> request<RT, UOut, UIn, Y1, Y>(UOut value) where RT : struct, HasCancel<RT> =>
            new Request<RT, UOut, UIn, Y1, Y, UIn>(value, r => new Pure<RT, UOut, UIn, Y1, Y, UIn>(r));


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
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, DOut, DIn, UIn, UOut, R> reflect<RT, UOut, UIn, DIn, DOut, R>(Proxy<RT, UOut, UIn, DIn, DOut, R> p)
            where RT : struct, HasCancel<RT> =>
            p.Reflect();

        /// <summary>
        /// `p.ForEach(body)` loops over the `Producer p` replacing each `yield` with `body`
        /// 
        ///     Producer b r -> (b -> Producer c ()) -> Producer c r
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT_B, A> ForEach<RT, OUT_A, OUT_B, A>(this Producer<RT, OUT_A, A> p, Func<OUT_A, Producer<RT, OUT_B, Unit>> body)
            where RT : struct, HasCancel<RT> =>
            p.For(body).ToProducer();

        /// <summary>
        /// `p.ForEach(body)` loops over `Producer p` replacing each `yield` with `body`
        /// 
        ///     Producer b r -> (b -> Effect ()) -> Effect r
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<RT, A> ForEach<RT, OUT, A>(this Producer<RT, OUT, A> p, Func<OUT, Effect<RT, Unit>> fb)
            where RT : struct, HasCancel<RT> =>
            p.For(fb).ToEffect();

        /// <summary>
        /// `p.ForEach(body)` loops over `Pipe p` replacing each `yield` with `body`
        /// 
        ///     Pipe x b r -> (b -> Consumer x ()) -> Consumer x r
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, A> ForEach<RT, IN, OUT, A>(this Pipe<RT, IN, OUT, A> p0, Func<OUT, Consumer<RT, IN, Unit>> fb)
            where RT : struct, HasCancel<RT> =>
            p0.For(fb).ToConsumer();

        /// <summary>
        /// `p.ForEach(body)` loops over `Pie p` replacing each `yield` with `body`
        /// 
        ///     Pipe x b r -> (b -> Pipe x c ()) -> Pipe x c r
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, R> ForEach<RT, IN, B, OUT, R>(this Pipe<RT, IN, B, R> p0, Func<B, Pipe<RT, IN, OUT, Unit>> fb)
            where RT : struct, HasCancel<RT> =>
            p0.For(fb).ToPipe();

        /// <summary>
        /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, UOut, UIn, DIn, DOut, B> compose<RT, UOut, UIn, DIn, DOut, A, B>(Proxy<RT, UOut, UIn, DIn, DOut, A> p1, Proxy<RT, Unit, A, DIn, DOut, B> p2)
            where RT : struct, HasCancel<RT> =>
            compose((Unit _) => p1, p2);

        /// <summary>
        /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
        /// 
        ///     Effect b -> Consumer b c -> Effect c
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<RT, A> compose<RT, OUT, A>(Effect<RT, OUT> p1, Consumer<RT, OUT, A> p2)
            where RT : struct, HasCancel<RT> =>
            compose((Unit _) => p1, p2).ToEffect();

        /// <summary>
        /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
        /// 
        ///     Consumer a b -> Consumer b c -> Consumer a c
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, C> compose<RT, A, B, C>(Consumer<RT, A, B> p1, Consumer<RT, B, C> p2) where RT : struct, HasCancel<RT> =>
            compose((Unit _) => p1, p2).ToConsumer();

        /// <summary>
        /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
        /// 
        ///     Producer y b -> Pipe b y m c -> Producer y c
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT, C> compose<RT, OUT, IN, C>(Producer<RT, OUT, IN> p1, Pipe<RT, IN, OUT, C> p2) where RT : struct, HasCancel<RT> =>
            compose((Unit _) => p1, p2).ToProducer();

        /// <summary>
        /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
        /// 
        ///     Pipe a y b -> Pipe b y c -> Pipe a y c
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, Y, C> compose<RT, Y, A, B, C>(Pipe<RT, A, Y, B> p1,
            Pipe<RT, B, Y, C> p2) where RT : struct, HasCancel<RT> =>
            compose((Unit _) => p1, p2).ToPipe();

        // fixAwaitDual
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, Y1, Y, C> compose<RT, A1, A, Y1, Y, B, C>(Proxy<RT, Unit, B, Y1, Y, C> p2,
            Proxy<RT, A1, A, Y1, Y, B> p1) where RT : struct, HasCancel<RT> =>
            compose(p1, p2);

        /// <summary>
        /// Replaces each `request` or `respond` in `p0` with `fb1`.
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, Y1, Y, C> compose<RT, A1, A, B1, B, Y1, Y, C>(Func<B1, Proxy<RT, A1, A, Y1, Y, B>> fb1,
            Proxy<RT, B1, B, Y1, Y, C> p0) where RT : struct, HasCancel<RT> =>
            p0.ComposeRight(fb1);

        /// <summary>
        /// `compose(p, f)` pairs each `respond` in `p` with a `request` in `f`.
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, C1, C, R> compose<RT, A1, A, B1, B, C1, C, R>(Proxy<RT, A1, A, B1, B, R> p,
            Func<B, Proxy<RT, B1, B, C1, C, R>> fb)
            where RT : struct, HasCancel<RT> =>
            p.ComposeLeft(fb);

        /// <summary>
        /// `compose(f, p)` pairs each `request` in `p` with a `respond` in `f`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, C1, C, R> compose<RT, A1, A, B1, B, C1, C, R>(Func<B1, Proxy<RT, A1, A, B1, B, R>> fb1,
            Proxy<RT, B1, B, C1, C, R> p)
            where RT : struct, HasCancel<RT> =>
            p.ComposeRight(fb1);

        /// <summary>
        /// Pipe composition
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, C1, C, R> compose<RT, A1, A, B, C1, C, R>(Proxy<RT, A1, A, Unit, B, R> p1,
            Proxy<RT, Unit, B, C1, C, R> p2)
            where RT : struct, HasCancel<RT> =>
            compose((Unit _) => p1, p2);

        /// <summary>
        /// Pipe composition
        ///
        ///     Producer b r -> Consumer b r -> Effect m r
        /// 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<RT, R> compose<RT, B, R>(Producer<RT, B, R> p1,
            Consumer<RT, B, R> p2)
            where RT : struct, HasCancel<RT> =>
            compose((Unit _) => p1.ToProxy(), p2).ToEffect();

        /// <summary>
        /// Pipe composition
        ///
        ///     Producer b r -> Pipe b c r -> Producer c r
        /// 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, C, R> compose<RT, B, C, R>(Producer<RT, B, R> p1,
            Pipe<RT, B, C, R> p2)
            where RT : struct, HasCancel<RT> =>
            compose((Unit _) => p1.ToProxy(), p2).ToProducer();

        /// <summary>
        /// Pipe composition
        ///
        ///     Pipe a b r -> Consumer b r -> Consumer a r
        /// 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> compose<RT, A, B, R>(Pipe<RT, A, B, R> p1,
            Consumer<RT, B, R> p2)
            where RT : struct, HasCancel<RT> =>
            compose((Unit _) => p1.ToProxy(), p2).ToConsumer();

        /// <summary>
        /// Pipe composition
        ///
        ///     Pipe a b r -> Pipe b c r -> Pipe a c r
        /// 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, C, R> compose<RT, A, B, C, R>(Pipe<RT, A, B, R> p1,
            Pipe<RT, B, C, R> p2)
            where RT : struct, HasCancel<RT> =>
            compose((Unit _) => p1.ToProxy(), p2).ToPipe();

        /// <summary>
        /// Compose two unfolds, creating a new unfold
        /// </summary>
        /// <remarks>
        /// This is the composition operator of the respond category.
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Func<A, Proxy<RT, X1, X, C1, C, A1>> compose<RT, X1, X, A1, A, B1, B, C1, C>(Func<A, Proxy<RT, X1, X, B1, B, A1>> fa,
            Func<B, Proxy<RT, X1, X, C1, C, B1>> fb) where RT : struct, HasCancel<RT> =>
            a => compose(fa(a), fb);

        /// <summary>
        /// Compose two unfolds, creating a new unfold
        /// </summary>
        /// <remarks>
        /// This is the composition operator of the respond category.
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Func<A, Proxy<RT, X1, X, C1, C, A1>> Then<RT, X1, X, A1, A, B1, B, C1, C>(this Func<A, Proxy<RT, X1, X, B1, B, A1>> fa,
            Func<B, Proxy<RT, X1, X, C1, C, B1>> fb) where RT : struct, HasCancel<RT> =>
            a => compose(fa(a), fb);

        /// <summary>
        /// `compose(p, f)` replaces each `respond` in `p` with `f`.
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, X1, X, C1, C, A1> compose<RT, X1, X, A1, B1, C1, C, B>(Proxy<RT, X1, X, B1, B, A1> p0,
            Func<B, Proxy<RT, X1, X, C1, C, B1>> fb) where RT : struct, HasCancel<RT> =>
            p0.ComposeLeft(fb);

        /// <summary>
        /// `compose(p, f)` replaces each `respond` in `p` with `f`.
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, X1, X, C1, C, A1> Then<RT, X1, X, A1, B1, C1, C, B>(this Proxy<RT, X1, X, B1, B, A1> p0,
            Func<B, Proxy<RT, X1, X, C1, C, B1>> fb) where RT : struct, HasCancel<RT> =>
            compose(p0, fb);


        /// <summary>
        ///  Compose two folds, creating a new fold
        /// 
        ///     (f | g) x = f | g x
        /// 
        ///     | is the composition operator of the request category.
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Func<C1, Proxy<RT, A1, A, Y1, Y, C>> compose<RT, A1, A, B1, B, Y1, Y, C1, C>(Func<B1, Proxy<RT, A1, A, Y1, Y, B>> fb1,
            Func<C1, Proxy<RT, B1, B, Y1, Y, C>> fc1) where RT : struct, HasCancel<RT> =>
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
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, R> observe<RT, A1, A, B1, B, R>(Proxy<RT, A1, A, B1, B, R> p0)
            where RT : struct, HasCancel<RT> =>
            p0.Observe();

        /// <summary>
        /// `Absurd` function
        /// </summary>
        /// <param name="value">`Void` is supposed to represent `void`, nothing can be constructed from `void` and
        /// so this method just throws `ApplicationException("closed")`</param>
        [Pure, MethodImpl(Proxy.mops)]
        public static A closed<A>(Void value) =>
            throw new ApplicationException("closed");

        /// <summary>
        /// Applicative apply
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, S> apply<RT, A1, A, B1, B, R, S>(Proxy<RT, A1, A, B1, B, Func<R, S>> pf,
            Proxy<RT, A1, A, B1, B, R> px) where RT : struct, HasCancel<RT>
        {
            return Go(pf);

            Proxy<RT, A1, A, B1, B, S> Go(Proxy<RT, A1, A, B1, B, Func<R, S>> p) =>
                p.ToProxy() switch
                {
                    Request<RT, A1, A, B1, B, Func<R, S>> (var a1, var fa) => new Request<RT, A1, A, B1, B, S>(a1, a => Go(fa(a))),
                    Respond<RT, A1, A, B1, B, Func<R, S>> (var b, var fb1) => new Respond<RT, A1, A, B1, B, S>(b, b1 => Go(fb1(b1))),
                    M<RT, A1, A, B1, B, Func<R, S>> (var m)                => new M<RT, A1, A, B1, B, S>(m.Map(Go)),
                    Pure<RT, A1, A, B1, B, Func<R, S>> (var f)             => px.Map(f),
                    Repeat<RT, A1, A, B1, B, Func<R, S>> (var innr)        => new Repeat<RT, A1, A, B1, B, S>(innr.Apply(px)),
                    Enumerate<RT, A1, A, B1, B, Func<R, S>> enumer         => enumer.Bind(px.Map),
                    Use<RT, A1, A, B1, B, Func<R, S>> use                  => use.Bind(px.Map),
                    Release<RT, A1, A, B1, B, Func<R, S>> rel              => rel.Bind(px.Map),
                    _                                                      => throw new NotSupportedException()
                };
        }

        /// <summary>
        /// Applicative apply
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, S> Apply<RT, A1, A, B1, B, R, S>(this Proxy<RT, A1, A, B1, B, Func<R, S>> pf, Proxy<RT, A1, A, B1, B, R> px) where RT : struct, HasCancel<RT> =>
            apply(pf, px);

        /// <summary>
        /// Applicative action
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, S> Action<RT, A1, A, B1, B, R, S>(this Proxy<RT, A1, A, B1, B, R> l, Proxy<RT, A1, A, B1, B, S> r) where RT : struct, HasCancel<RT> =>
            l.Action(r);

        /// <summary>
        /// Monad return / pure
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, R> Pure<RT, A1, A, B1, B, R>(R value) where RT : struct, HasCancel<RT> =>
            new Pure<RT, A1, A, B1, B, R>(value);

        /// <summary>
        /// Creates a non-yielding producer that returns the result of the effects
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Lift<RT, (A, B)> collect<RT, A, B>(Effect<RT, A> ma, Effect<RT, B> mb) where RT : struct, HasCancel<RT> =>
            Proxy.lift<RT, (A, B)>((ma.RunEffect(), mb.RunEffect()).Sequence());

        /// <summary>
        /// Creates a non-yielding producer that returns the result of the effects
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Lift<RT, (A, B, C)> collect<RT, A, B, C>(Effect<RT, A> ma, Effect<RT, B> mb, Effect<RT, C> mc) where RT : struct, HasCancel<RT> =>
            Proxy.lift<RT, (A, B, C)>((ma.RunEffect(), mb.RunEffect(), mc.RunEffect()).Sequence());

        /// <summary>
        /// Creates a non-yielding producer that returns the result of the effects
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Lift<RT, (A, B, C, D)> collect<RT, A, B, C, D>(Effect<RT, A> ma, Effect<RT, B> mb, Effect<RT, C> mc, Effect<RT, D> md) where RT : struct, HasCancel<RT> =>
            Proxy.lift<RT, (A, B, C, D)>((ma.RunEffect(), mb.RunEffect(), mc.RunEffect(), md.RunEffect()).Sequence());

        /// <summary>
        /// Creates a non-yielding producer that returns the result of the effects
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Lift<RT, (A, B, C, D, E)> collect<RT, A, B, C, D, E>(Effect<RT, A> ma, Effect<RT, B> mb, Effect<RT, C> mc, Effect<RT, D> md, Effect<RT, E> me) where RT : struct, HasCancel<RT> =>
            Proxy.lift<RT, (A, B, C, D, E)>((ma.RunEffect(), mb.RunEffect(), mc.RunEffect(), md.RunEffect(), me.RunEffect()).Sequence());

        /// <summary>
        /// Creates a non-yielding producer that returns the result of the effects
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static ProducerLift<RT, (A, B), Unit> yield<RT, A, B>(Effect<RT, A> ma, Effect<RT, B> mb) where RT : struct, HasCancel<RT> =>
            from r in collect(ma, mb)
            from _ in Proxy.yield(r)
            select unit;

        /// <summary>
        /// Creates a non-yielding producer that returns the result of the effects
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static ProducerLift<RT, (A, B, C), Unit> yield<RT, A, B, C>(Effect<RT, A> ma, Effect<RT, B> mb, Effect<RT, C> mc) where RT : struct, HasCancel<RT> =>
            from r in collect(ma, mb, mc)
            from _ in Proxy.yield(r)
            select unit;

        /// <summary>
        /// Creates a non-yielding producer that returns the result of the effects
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static ProducerLift<RT, (A, B, C, D), Unit> yield<RT, A, B, C, D>(Effect<RT, A> ma, Effect<RT, B> mb, Effect<RT, C> mc, Effect<RT, D> md) where RT : struct, HasCancel<RT> =>
            from r in collect(ma, mb, mc, md)
            from _ in Proxy.yield(r)
            select unit;

        /// <summary>
        /// Creates a non-yielding producer that returns the result of the effects
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static ProducerLift<RT, (A, B, C, D, E), Unit> yield<RT, A, B, C, D, E>(Effect<RT, A> ma, Effect<RT, B> mb, Effect<RT, C> mc, Effect<RT, D> md, Effect<RT, E> me) where RT : struct, HasCancel<RT> =>
            from r in collect(ma, mb, mc, md, me)
            from _ in Proxy.yield(r)
            select unit;

        /// <summary>
        /// Only forwards values that satisfy the predicate.
        /// </summary>
        public static Pipe<A, A, Unit> filter<A>(Func<A, bool> f) =>
            from x in Proxy.awaiting<A>()
            from r in f(x) ? Proxy.yield(x) : Proxy.Pure(unit)
            select r;

        /// <summary>
        /// Map the output of the pipe (not the bound value as is usual with Map)
        /// </summary>
        public static Pipe<A, B, Unit> map<A, B>(Func<A, B> f) =>
            from x in Proxy.awaiting<A>()
            from r in Proxy.yield(f(x))
            select r;

        /// <summary>
        /// Folds values coming down-stream, when the predicate returns false the folded value is yielded 
        /// </summary>
        /// <param name="Initial">Initial state</param>
        /// <param name="Fold">Fold operation</param>
        /// <param name="WhileState">Predicate</param>
        /// <returns>A pipe that folds</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<IN, OUT, Unit> foldWhile<IN, OUT>(OUT Initial, Func<OUT, IN, OUT> Fold, Func<OUT, bool> State) => 
            foldUntil<IN, OUT>(Initial, Fold, x => !State(x));
 
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
            return Proxy.awaiting<IN>()
                       .Bind(x =>
                             {
                                 state = Fold(state, x);
                                 if (State(state))
                                 {
                                     var nstate = state;
                                     state = Initial;
                                     return Proxy.yield(nstate);
                                 }
                                 else
                                 {
                                     return Proxy.Pure(unit);
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
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<IN, OUT, Unit> foldWhile<IN, OUT>(OUT Initial, Func<OUT, IN, OUT> Fold, Func<IN, bool> Value) => 
            foldUntil<IN, OUT>(Initial, Fold, x => !Value(x));
 
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
            return Proxy.awaiting<IN>()
                       .Bind(x =>
                             {
                                 if (Value(x))
                                 {
                                     var nstate = state;
                                     state = Initial;
                                     return Proxy.yield(nstate);
                                 }
                                 else
                                 {
                                     state = Fold(state, x);
                                     return Proxy.Pure(unit);
                                 }
                             });
        }
    }
}
