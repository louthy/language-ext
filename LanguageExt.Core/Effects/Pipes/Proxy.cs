using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes
{
    /// <summary>
    /// Proxy prelude
    /// </summary>
    public static partial class Proxy
    {
        internal const MethodImplOptions mops = MethodImplOptions.AggressiveInlining;

        /// <summary>
        /// Monad return / pure
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pure<A> Pure<A>(A value) =>
            new Pure<A>(value);

        /// <summary>
        /// Wait for a value from upstream (whilst in a pipe)
        /// </summary>
        /// <remarks>
        /// This is the version of `await` that works for pipes.  In consumers, use `Consumer.await`
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<A, A> awaiting<A>() =>
            PureProxy.ConsumerAwait<A>();
        
        /// <summary>
        /// Send a value downstream (whilst in a pipe)
        /// </summary>
        /// <remarks>
        /// This is the version of `yield` that works for pipes.  In producers, use `Producer.yield`
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<A, Unit> yield<A>(A value) =>
            PureProxy.ProducerYield(value);

        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<OUT, X> enumerate<OUT, X>(IEnumerable<X> xs) =>
            PureProxy.ProducerEnumerate<OUT, X>(xs);

        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<X, X> enumerate<X>(IEnumerable<X> xs) =>
            PureProxy.ProducerEnumerate<X, X>(xs);

        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<OUT, X> enumerate<OUT, X>(IAsyncEnumerable<X> xs) =>
            PureProxy.ProducerEnumerate<OUT, X>(xs);

        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<X, X> enumerate<X>(IAsyncEnumerable<X> xs) =>
            PureProxy.ProducerEnumerate<X, X>(xs);

        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<OUT, X> observe<OUT, X>(IObservable<X> xs) =>
            PureProxy.ProducerObserve<OUT, X>(xs);

        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<X, X> observe<X>(IObservable<X> xs) =>
            PureProxy.ProducerObserve<X, X>(xs);

        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<X, Unit> observe2<X>(IObservable<X> xs) =>
            observe<X>(xs).Bind(yield);

        /// <summary>
        /// Lift the IO monad into the monad transformer 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Lift<RT, R> liftIO<RT, R>(Eff<RT, R> ma) where RT : struct, HasCancel<RT>  =>
            Lift.Eff<RT, R>(ma);

        /// <summary>
        /// Lift the IO monad into the monad transformer 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Lift<RT, R> liftIO<RT, R>(Aff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            Lift.Aff<RT, R>(ma);

        /// <summary>
        /// Lift the IO monad into the monad transformer 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Lift<RT, R> liftIO<RT, R>(Eff<R> ma) where RT : struct, HasCancel<RT> =>
            Lift.Eff<RT, R>(ma);

        /// <summary>
        /// Lift the IO monad into the monad transformer 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Lift<RT, R> liftIO<RT, R>(Aff<R> ma) where RT : struct, HasCancel<RT> =>
            Lift.Aff<RT, R>(ma);

        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT, R> repeat<RT, OUT, R>(Producer<RT, OUT, R> ma) where RT : struct, HasCancel<RT> =>
            new Repeat<RT, Void, Unit, Unit, OUT, R>(ma).ToProducer();

        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, R> repeat<RT, IN, R>(Consumer<RT, IN, R> ma) where RT : struct, HasCancel<RT> =>
            new Repeat<RT, Unit, IN, Unit, Void, R>(ma).ToConsumer();
        
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, R> repeat<RT, IN, OUT, R>(Pipe<RT, IN, OUT, R> ma) where RT : struct, HasCancel<RT> =>
            new Repeat<RT, Unit, IN, Unit, OUT, R>(ma).ToPipe();
        
        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, R> liftIO<RT, A1, A, B1, B, R>(Aff<R> ma) where RT : struct, HasCancel<RT> =>
            new M<RT, A1, A, B1, B, R>(ma.Map(Pure<RT, A1, A, B1, B, R>).WithRuntime<RT>());

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, R> liftIO<RT, A1, A, B1, B, R>(Eff<R> ma) where RT : struct, HasCancel<RT> =>
            new M<RT, A1, A, B1, B, R>(ma.Map(Pure<RT, A1, A, B1, B, R>).ToAsyncWithRT<RT>());

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, R> liftIO<RT, A1, A, B1, B, R>(Aff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            new M<RT, A1, A, B1, B, R>(ma.Map(Pure<RT, A1, A, B1, B, R>));

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, R> liftIO<RT, A1, A, B1, B, R>(Eff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            new M<RT, A1, A, B1, B, R>(ma.Map(Pure<RT, A1, A, B1, B, R>).ToAff());
        
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
        /// `p.@for(body)` loops over `p` replacing each `yield` with `body`
        /// 
        /// Producer b r -> (b -> Producer c ()) -> Producer c r
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT_B, A> @for<RT, OUT_A, OUT_B, A>(this Producer<RT, OUT_A, A> p, Func<OUT_A, Producer<RT, OUT_B, Unit>> body) 
            where RT : struct, HasCancel<RT> =>
                p.For(body).ToProducer();

        /// <summary>
        /// `p.@for(body)` loops over `p` replacing each `yield` with `body`
        /// 
        /// Producer b r -> (b -> Effect ()) -> Effect r
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<RT, A> @for<RT, OUT, A>(this Producer<RT, OUT, A> p, Func<OUT, Effect<RT, Unit>> fb) 
            where RT : struct, HasCancel<RT> =>
                p.For(fb).ToEffect();

        /// <summary>
        /// `p.@for(body)` loops over `p` replacing each `yield` with `body`
        /// 
        /// Pipe x b r -> (b -> Consumer x ()) -> Consumer x r
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, A> @for<RT, IN, OUT, A>(this Pipe<RT, IN, OUT, A> p0, Func<OUT, Consumer<RT, IN, Unit>> fb) 
            where RT : struct, HasCancel<RT> =>
                p0.For(fb).ToConsumer();

        /// <summary>
        /// `p.@for(body)` loops over `p` replacing each `yield` with `body`
        /// 
        /// Pipe x b r -> (b -> Pipe x c ()) -> Pipe x c r
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, R> @for<RT, IN, B, OUT, R>(this Pipe<RT, IN, B, R> p0, Func<B, Pipe<RT, IN, OUT, Unit>> fb) 
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
        /// Effect b -> Consumer b c -> Effect c
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<RT, A> compose<RT, OUT, A>(Effect<RT, OUT> p1, Consumer<RT, OUT, A> p2) 
            where RT : struct, HasCancel<RT> =>
                compose((Unit _) => p1, p2).ToEffect();        
        
        /// <summary>
        /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
        /// 
        /// Consumer a b -> Consumer b c -> Consumer a c
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, C> compose<RT, A, B, C>(Consumer<RT, A, B> p1, Consumer<RT, B, C> p2) where RT : struct, HasCancel<RT> =>
            compose((Unit _) => p1, p2).ToConsumer();
        
        /// <summary>
        /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
        /// 
        /// Producer y b -> Pipe b y m c -> Producer y c
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT, C> compose<RT, OUT, IN, C>(Producer<RT, OUT, IN> p1, Pipe<RT, IN, OUT, C> p2) where RT : struct, HasCancel<RT> =>
            compose((Unit _) => p1, p2).ToProducer();         
        
        /// <summary>
        /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
        /// 
        /// Pipe a y b -> Pipe b y c -> Pipe a y c
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, Y, C> compose<RT, Y, A, B, C>(
            Pipe<RT, A, Y, B> p1,
            Pipe<RT, B, Y, C> p2) where RT : struct, HasCancel<RT> =>
            compose((Unit _) => p1, p2).ToPipe();         
        
        // fixAwaitDual
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, Y1, Y, C> compose<RT, A1, A, Y1, Y, B, C>(
            Proxy<RT, Unit, B, Y1, Y, C> p2,
            Proxy<RT, A1, A, Y1, Y, B> p1) where RT : struct, HasCancel<RT> =>
            compose(p1, p2);

        /// <summary>
        /// Replaces each `request` or `respond` in `p0` with `fb1`.
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, Y1, Y, C> compose<RT, A1, A, B1, B, Y1, Y, C>(
            Func<B1, Proxy<RT, A1, A, Y1, Y, B>> fb1,
            Proxy<RT, B1, B, Y1, Y, C> p0) where RT : struct, HasCancel<RT> =>
            p0.ComposeRight(fb1);
        
        /// <summary>
        /// `compose(p, f)` pairs each `respond` in `p` with a `request` in `f`.
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, C1, C, R> compose<RT, A1, A, B1, B, C1, C, R>(
            Proxy<RT, A1, A, B1, B, R> p,
            Func<B, Proxy<RT, B1, B, C1, C, R>> fb)
            where RT : struct, HasCancel<RT> => 
                p.ComposeLeft(fb);        
        
        /// <summary>
        /// `compose(f, p)` pairs each `request` in `p` with a `respond` in `f`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, C1, C, R> compose<RT, A1, A, B1, B, C1, C, R>(
            Func<B1, Proxy<RT, A1, A, B1, B, R>> fb1,
            Proxy<RT, B1, B, C1, C, R> p) 
            where RT : struct, HasCancel<RT> =>
                p.ComposeRight(fb1);
        
        /// <summary>
        /// Pipe composition
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, C1, C, R> compose<RT, A1, A, B, C1, C, R>(
            Proxy<RT, A1, A, Unit, B, R> p1,
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
        public static Effect<RT, R> compose<RT, B, R>(
            Producer<RT, B, R> p1,
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
        public static Producer<RT, C, R> compose<RT, B, C, R>(
            Producer<RT, B, R> p1,
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
        public static Consumer<RT, A, R> compose<RT, A, B, R>(
            Pipe<RT, A, B, R> p1,
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
        public static Pipe<RT, A, C, R> compose<RT, A, B, C, R>(
            Pipe<RT, A, B, R> p1,
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
        public static Func<A, Proxy<RT, X1, X, C1, C, A1>> compose<RT, X1, X, A1, A, B1, B, C1, C>(
            Func<A, Proxy<RT, X1, X, B1, B, A1>> fa,
            Func<B, Proxy<RT, X1, X, C1, C, B1>> fb) where RT : struct, HasCancel<RT> =>
                a => compose(fa(a), fb);
        
        /// <summary>
        /// Compose two unfolds, creating a new unfold
        /// </summary>
        /// <remarks>
        /// This is the composition operator of the respond category.
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Func<A, Proxy<RT, X1, X, C1, C, A1>> Then<RT, X1, X, A1, A, B1, B, C1, C>(
            this Func<A, Proxy<RT, X1, X, B1, B, A1>> fa,
            Func<B, Proxy<RT, X1, X, C1, C, B1>> fb) where RT : struct, HasCancel<RT> =>
            a => compose(fa(a), fb);
        
        /// <summary>
        /// `compose(p, f)` replaces each `respond` in `p` with `f`.
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, X1, X, C1, C, A1> compose<RT, X1, X, A1, B1, C1, C, B>(
            Proxy<RT, X1, X, B1, B, A1> p0, 
            Func<B, Proxy<RT, X1, X, C1, C, B1>> fb) where RT : struct, HasCancel<RT> =>
            p0.ComposeLeft(fb);
        
        /// <summary>
        /// `compose(p, f)` replaces each `respond` in `p` with `f`.
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, X1, X, C1, C, A1> Then<RT, X1, X, A1, B1, C1, C, B>(
            this Proxy<RT, X1, X, B1, B, A1> p0, 
            Func<B, Proxy<RT, X1, X, C1, C, B1>> fb) where RT : struct, HasCancel<RT> =>
                compose(p0, fb);
        
        
        /// {-| Compose two folds, creating a new fold
        ///         @
        ///     (f '\>\' g) x = f '>\\' g x
        ///         @
        /// 
        ///     ('\>\') is the composition operator of the request category.
        ///     -}
        /// (\>\)
        /// :: Functor m
        /// => (b' -> Proxy a' a y' y m b)
        /// -- ^
        /// -> (c' -> Proxy b' b y' y m c)
        /// -- ^
        /// -> (c' -> Proxy a' a y' y m c)
        /// -- ^
        /// (fb' \>\ fc') c' = fb' >\\ fc' c'
        [Pure, MethodImpl(Proxy.mops)]
        public static Func<C1, Proxy<RT, A1, A, Y1, Y, C>> compose<RT, A1, A, B1, B, Y1, Y, C1, C>(
            Func<B1, Proxy<RT, A1, A, Y1, Y, B>> fb1, 
            Func<C1, Proxy<RT, B1, B, Y1, Y, C>> fc1) where RT : struct, HasCancel<RT> =>
            c1 => compose(fb1, fc1(c1));

        /// <summary>
        /// observe (lift (return r)) = observe (return r)
        /// observe (lift (m >>= f)) = observe (lift m >>= lift . f)
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
        /// 'Absurd' function
        /// `VoidX` is supposed to represent `void`, nothing can be constructed from `void` and
        /// so this method just throws `ApplicationException("closed")`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static A closed<A>(Void _) =>
            throw new ApplicationException("closed");
        
        /// <summary>
        /// Applicative apply
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, S> apply<RT, A1, A, B1, B, R, S>(
            Proxy<RT, A1, A, B1, B, Func<R, S>> pf, 
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
                    Observer<RT, A1, A, B1, B, Func<R, S>> obs              => obs.Bind(px.Map),
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
    }
}
