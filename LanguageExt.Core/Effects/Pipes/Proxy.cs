using System;
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
            ma.Bind(a => repeat(ma)); // TODO: Remove recursion

        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, R> repeat<RT, IN, R>(Consumer<RT, IN, R> ma) where RT : struct, HasCancel<RT> =>
            ma.Bind(a => repeat(ma)); // TODO: Remove recursion
        
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, R> repeat<RT, IN, OUT, R>(Pipe<RT, IN, OUT, R> ma) where RT : struct, HasCancel<RT> =>
            ma.Bind(a => repeat(ma)); // TODO: Remove recursion

        
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
        /// Converts a `Proxy` with the correct _shape_ into an `Effect`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<RT, R> ToEffect<RT, R>(this Proxy<RT, Void, Unit, Unit, Void, R> ma) where RT : struct, HasCancel<RT> =>
            new Effect<RT, R>(ma);
        
        /// <summary>
        /// Converts a `Proxy` with the correct _shape_ into a `Producer`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, A, R> ToProducer<RT, A, R>(this Proxy<RT, Void, Unit, Unit, A, R> ma) where RT : struct, HasCancel<RT> =>
            new Producer<RT, A, R>(ma);
        
        /// <summary>
        /// Converts a `Proxy` with the correct _shape_ into a `Consumer`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> ToConsumer<RT, A, R>(this Proxy<RT, Unit, A, Unit, Void, R> ma) where RT : struct, HasCancel<RT> =>
            new Consumer<RT, A, R>(ma);
        
        /// <summary>
        /// Converts a `Proxy` with the correct _shape_ into n `Pipe`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> ToPipe<RT, A, B, R>(this Proxy<RT, Unit, A, Unit, B, R> ma) where RT : struct, HasCancel<RT> =>
            new Pipe<RT, A, B, R>(ma);
        
        /// <summary>
        /// Converts a `Proxy` with the correct _shape_ into a `Client`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, A, B, R> ToClient<RT, A, B, R>(this Proxy<RT, A, B, Unit, Unit, R> ma) where RT : struct, HasCancel<RT> =>
            new Client<RT, A, B, R>(ma);
        
        /// <summary>
        /// Converts a `Proxy` with the correct _shape_ into a `Server`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, A, B, R> ToServer<RT, A, B, R>(this Proxy<RT, Unit, Unit, A, B, R> ma) where RT : struct, HasCancel<RT> =>
            new Server<RT, A, B, R>(ma);
        
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
        public static Proxy<RT, A1, A, A1, A, R> pull<RT, A1, A, R>(A1 a1) where RT : struct, HasCancel<RT> =>
            new Request<RT, A1, A, A1, A, R>(a1, a => new Respond<RT, A1, A, A1, A, R>(a, pull<RT, A1, A, R>));

        /// <summary>
        /// `push = respond | request | push`
        /// </summary>
        /// <remarks>
        /// `push` is the identity of the push category.
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, A1, A, R> push<RT, A1, A, R>(A a) where RT : struct, HasCancel<RT> =>
            new Respond<RT, A1, A, A1, A, R>(a, a1 => new Request<RT, A1, A, A1, A, R>(a1, push<RT, A1, A, R>));
        
        /// <summary>
        /// Send a value of type `a` downstream and block waiting for a reply of type `a`
        /// </summary>
        /// <remarks>
        /// `respond` is the identity of the respond category.
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, X1, X, A1, A, A1> respond<RT, X1, X, A1, A>(A value) where RT : struct, HasCancel<RT> =>
            new Respond<RT, X1, X, A1, A, A1>(value, r => new Pure<RT, X1, X, A1, A, A1>(r));

        /// <summary>
        /// Send a value of type `a` upstream and block waiting for a reply of type `a`
        /// </summary>
        /// <remarks>
        /// `request` is the identity of the request category.
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, Y1, Y, A> request<RT, A1, A, Y1, Y>(A1 value) where RT : struct, HasCancel<RT> =>
            new Request<RT, A1, A, Y1, Y, A>(value, r => new Pure<RT, A1, A, Y1, Y, A>(r));
        
       
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
        public static Proxy<RT, B, B1, A, A1, R> reflect<RT, A1, A, B1, B, R>(Proxy<RT, A1, A, B1, B, R> p) where RT : struct, HasCancel<RT>
        {
            return Go(p);
            Proxy<RT, B, B1, A, A1, R> Go(Proxy<RT, A1, A, B1, B, R> p) =>
                p.ToProxy() switch
                {
                    Request<RT, A1, A, B1, B, R> (var a1, var fa) => new Respond<RT, B, B1, A, A1, R>(a1, a => Go(fa(a))),
                    Respond<RT, A1, A, B1, B, R> (var b, var fb1) => new Request<RT, B, B1, A, A1, R>(b, b1 => Go(fb1(b1))),
                    M<RT, A1, A, B1, B, R> (var m)                => new M<RT, B, B1, A, A1, R>(m.Map(Go)),
                    Pure<RT, A1, A, B1, B, R> (var r)             => Pure<RT, B, B1, A, A1, R>(r),                                                                                
                    _                                              => throw new NotSupportedException()
                };
        }
        
        /// <summary>
        /// `p.For(body)` loops over `p` replacing each `yield` with `body`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, X1, X, C1, C, R> For<RT, X1, X, B1, B, C1, C, R>(this
            Proxy<RT, X1, X, B1, B, R> p0,
            Func<B, Proxy<RT, X1, X, C1, C, B1>> fb) where RT : struct, HasCancel<RT>
        {
            return Go(p0);
            Proxy<RT, X1, X, C1, C, R> Go(Proxy<RT, X1, X, B1, B, R> p) =>
                p.ToProxy() switch
                {
                    Request<RT, X1, X, B1, B, R> (var x1, var fx) => new Request<RT, X1, X, C1, C, R>(x1, x => Go(fx(x))),
                    Respond<RT, X1, X, B1, B, R> (var b, var fb1) => fb(b).Bind(b1 => Go(fb1(b1))),
                    M<RT, X1, X, B1, B, R> (var m)                => new M<RT, X1, X, C1, C, R>(m.Map(Go)),
                    Pure<RT, X1, X, B1, B, R> (var a)             => Pure<RT, X1, X, C1, C, R>(a),                                                                                
                    _                                             => throw new NotSupportedException()
                };
        }

        /// <summary>
        /// `p.For(body)` loops over `p` replacing each `yield` with `body`
        /// 
        /// Producer b r -> (b -> Producer c ()) -> Producer c r
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, C, R> For<RT, B, C, R>(this
            Producer<RT, B, R> p0,  
            Func<B, Producer<RT, C, Unit>> fb) where RT : struct, HasCancel<RT> =>
            For<RT, Void, Unit, Unit, B, Unit, C, R>(p0, fb).ToProducer();

        /// <summary>
        /// `p.For(body)` loops over `p` replacing each `yield` with `body`
        /// 
        /// Producer b r -> (b -> Effect ()) -> Effect r
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<RT, R> For<RT, B, R>(this
            Producer<RT, B, R> p0,  
            Func<B, Effect<RT, Unit>> fb) where RT : struct, HasCancel<RT> =>
            For<RT, Void, Unit, Unit, B, Unit, Void, R>(p0, fb).ToEffect();

        /// <summary>
        /// `p.For(body)` loops over `p` replacing each `yield` with `body`
        /// 
        /// Pipe x b r -> (b -> Consumer x ()) -> Consumer x r
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, X, R> For<RT, X, B, R>(this
            Pipe<RT, X, B, R> p0,
            Func<B, Consumer<RT, X, Unit>> fb) where RT : struct, HasCancel<RT> =>
            For<RT, Unit, X, Unit, B, Unit, Void, R>(p0, fb).ToConsumer();

        /// <summary>
        /// `p.For(body)` loops over `p` replacing each `yield` with `body`
        /// 
        /// Pipe x b r -> (b -> Pipe x c ()) -> Pipe x c r
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, X, C, R> For<RT, X, B, C, R>(this
            Pipe<RT, X, B, R> p0,
            Func<B, Pipe<RT, X, C, Unit>> fb) where RT : struct, HasCancel<RT> =>  
            For<RT, Unit, X, Unit, B, Unit, C, R>(p0, fb).ToPipe(); 

        
        /// <summary>
        /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, Y1, Y, C> compose<RT, A1, A, Y1, Y, B, C>(
            Proxy<RT, A1, A, Y1, Y, B> p1,
            Proxy<RT, Unit, B, Y1, Y, C> p2) where RT : struct, HasCancel<RT> =>
            compose((Unit _) => p1, p2);
        
        /// <summary>
        /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
        /// 
        /// Effect b -> Consumer b c -> Effect c
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<RT, C> compose<RT, B, C>(
            Effect<RT, B> p1,
            Consumer<RT, B, C> p2) where RT : struct, HasCancel<RT> =>
            compose((Unit _) => p1, p2).ToEffect();        
        
        /// <summary>
        /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
        /// 
        /// Consumer a b -> Consumer b c -> Consumer a c
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, C> compose<RT, A, B, C>(
            Consumer<RT, A, B> p1,
            Consumer<RT, B, C> p2) where RT : struct, HasCancel<RT> =>
            compose((Unit _) => p1, p2).ToConsumer();
        
        /// <summary>
        /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
        /// 
        /// Producer y b -> Pipe b y m c -> Producer y c
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, Y, C> compose<RT, Y, B, C>(
            Producer<RT, Y, B> p1,
            Pipe<RT, B, Y, C> p2) where RT : struct, HasCancel<RT> =>
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
            Proxy<RT, B1, B, Y1, Y, C> p0) where RT : struct, HasCancel<RT> 
        {
            return Go(p0);
            Proxy<RT, A1, A, Y1, Y, C> Go(Proxy<RT, B1, B, Y1, Y, C> p) =>
                p.ToProxy() switch
                {
                    Request<RT, B1, B, Y1, Y, C> (var b1, var fb) => fb1(b1).Bind(b => Go(fb(b))),
                    Respond<RT, B1, B, Y1, Y, C> (var x, var fx1) => new Respond<RT, A1, A, Y1, Y, C>(x, x1 => Go(fx1(x1))),
                    M<RT, B1, B, Y1, Y, C> (var m)                => new M<RT, A1, A, Y1, Y, C>(m.Map(Go)),
                    Pure<RT, B1, B, Y1, Y, C> (var a)             => Pure<RT, A1, A, Y1, Y, C>(a),                                                                                
                    _                                              => throw new NotSupportedException()
                };
        }        
        
        /// <summary>
        /// `compose(p, f)` pairs each `respond` in `p` with a `request` in `f`.
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, C1, C, R> compose<RT, A1, A, B1, B, C1, C, R>(
            Proxy<RT, A1, A, B1, B, R> p,
            Func<B, Proxy<RT, B1, B, C1, C, R>> fb
            )
            where RT : struct, HasCancel<RT> => 
                p.ToProxy() switch
                {
                    Request<RT, A1, A, B1, B, R> (var a1, var fa) => new Request<RT, A1, A, C1, C, R>(a1, a => compose(fa(a), fb)),
                    Respond<RT, A1, A, B1, B, R> (var b, var fb1) => compose(fb1, fb(b)),
                    M<RT, A1, A, B1, B, R> (var m)                => new M<RT, A1, A, C1, C, R>(m.Map(p1 => compose(p1, fb))),
                    Pure<RT, A1, A, B1, B, R> (var r)             => Pure<RT, A1, A, C1, C, R>(r),                                                                                
                    _                                              => throw new NotSupportedException()
                };        
        
        /// <summary>
        /// `compose(f, p)` pairs each `request` in `p` with a `respond` in `f`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, C1, C, R> compose<RT, A1, A, B1, B, C1, C, R>(
            Func<B1, Proxy<RT, A1, A, B1, B, R>> fb1,
            Proxy<RT, B1, B, C1, C, R> p) 
            where RT : struct, HasCancel<RT> => 
                p.ToProxy() switch
                {
                    Request<RT, B1, B, C1, C, R> (var b1, var fb) => compose(fb1(b1), fb),
                    Respond<RT, B1, B, C1, C, R> (var c, var fc1) => new Respond<RT, A1, A, C1, C, R>(c, c1 => compose(fb1, fc1(c1))),
                    M<RT, B1, B, C1, C, R> (var m)                => new M<RT, A1, A, C1, C, R>(m.Map(p1 => compose(fb1, p1))),
                    Pure<RT, B1, B, C1, C, R> (var r)             => Pure<RT, A1, A, C1, C, R>(r),                      
                    _                                              => throw new NotSupportedException()
                };
        
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
            Func<B, Proxy<RT, X1, X, C1, C, B1>> fb) where RT : struct, HasCancel<RT>
        {
            return Go(p0);
            Proxy<RT, X1, X, C1, C, A1> Go(Proxy<RT, X1, X, B1, B, A1> p) =>
                p.ToProxy() switch
                {
                    Request<RT, X1, X, B1, B, A1> (var x1, var fx) => new Request<RT, X1, X, C1, C, A1>(x1, x => Go(fx(x))),
                    Respond<RT, X1, X, B1, B, A1> (var b, var fb1) => fb(b).Bind(b1 => Go(fb1(b1))),
                    M<RT, X1, X, B1, B, A1> (var m)                => new M<RT, X1, X, C1, C, A1>(m.Map(Go)),
                    Pure<RT, X1, X, B1, B, A1> (var a)             => Pure<RT, X1, X, C1, C, A1>(a),
                    _                                               => throw new NotSupportedException()
                };
        }
        
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
        public static Proxy<RT, A1, A, B1, B, R> observe<RT, A1, A, B1, B, R>(Proxy<RT, A1, A, B1, B, R> p0) where RT : struct, HasCancel<RT>
        {
            return new M<RT, A1, A, B1, B, R>(Go(p0));

            static Aff<RT, Proxy<RT, A1, A, B1, B, R>> Go(Proxy<RT, A1, A, B1, B, R> p) =>
                p.ToProxy() switch
                {
                    Request<RT, A1, A, B1, B, R> (var a1, var fa) => Aff<RT, Proxy<RT, A1, A, B1, B, R>>.Success(new Request<RT, A1, A, B1, B, R>(a1, a => observe(fa(a)))),
                    Respond<RT, A1, A, B1, B, R> (var b, var fb1) => Aff<RT, Proxy<RT, A1, A, B1, B, R>>.Success(new Respond<RT, A1, A, B1, B, R>(b, b1 => observe(fb1(b1)))),
                    M<RT, A1, A, B1, B, R> (var m1)               => m1.Bind(Go),
                    Pure<RT, A1, A, B1, B, R> (var r)             => Aff<RT, Proxy<RT, A1, A, B1, B, R>>.Success(new Pure<RT, A1, A, B1, B, R>(r)),                                                                                
                    _                                             => throw new NotSupportedException()
                };
        }

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
        public static Proxy<RT, A1, A, B1, B, S> apply<RT, A1, A, B1, B, R, S>(Proxy<RT, A1, A, B1, B, Func<R, S>> pf, Proxy<RT, A1, A, B1, B, R> px) where RT : struct, HasCancel<RT>
        {
            return Go(pf);
            Proxy<RT, A1, A, B1, B, S> Go(Proxy<RT, A1, A, B1, B, Func<R, S>> p) =>
                p.ToProxy() switch
                {
                    Request<RT, A1, A, B1, B, Func<R, S>> (var a1, var fa) => new Request<RT, A1, A, B1, B, S>(a1, a => Go(fa(a))),
                    Respond<RT, A1, A, B1, B, Func<R, S>> (var b, var fb1) => new Respond<RT, A1, A, B1, B, S>(b, b1 => Go(fb1(b1))),
                    M<RT, A1, A, B1, B, Func<R, S>> (var m)                => new M<RT, A1, A, B1, B, S>(m.Map(Go)),
                    Pure<RT, A1, A, B1, B, Func<R, S>> (var f)             => px.Map(f),                                                                                
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
        public static Proxy<RT, A1, A, B1, B, S> action<RT, A1, A, B1, B, R, S>(Proxy<RT, A1, A, B1, B, R> l, Proxy<RT, A1, A, B1, B, S> r) where RT : struct, HasCancel<RT>
        {
            return Go(l);
            Proxy<RT, A1, A, B1, B, S> Go(Proxy<RT, A1, A, B1, B, R> p) =>
                p.ToProxy() switch
                {
                    Request<RT, A1, A, B1, B, R> (var a1, var fa) => new Request<RT, A1, A, B1, B, S>(a1, a => Go(fa(a))),
                    Respond<RT, A1, A, B1, B, R> (var b, var fb1) => new Respond<RT, A1, A, B1, B, S>(b, b1 => Go(fb1(b1))),
                    M<RT, A1, A, B1, B, R> (var m)                => new M<RT, A1, A, B1, B, S>(m.Map(Go)),
                    Pure<RT, A1, A, B1, B, R> (var _)             => r,                                                                                
                    _                                             => throw new NotSupportedException()
                };
        }

        /// <summary>
        /// Applicative action
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, S> Action<RT, A1, A, B1, B, R, S>(this Proxy<RT, A1, A, B1, B, R> l, Proxy<RT, A1, A, B1, B, S> r) where RT : struct, HasCancel<RT> =>
            action(l, r);

        /// <summary>
        /// Monad return / pure
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, R> Pure<RT, A1, A, B1, B, R>(R value) where RT : struct, HasCancel<RT> =>
            new Pure<RT, A1, A, B1, B, R>(value);
    }
}
