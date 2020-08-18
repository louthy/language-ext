using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using LanguageExt.Interfaces;
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
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, A1, A, B1, B, R> liftIO<Env, A1, A, B1, B, R>(AffPure<R> ma) where Env : struct, HasCancel<Env> =>
            new M<Env, A1, A, B1, B, R>(ma.Map(Pure<Env, A1, A, B1, B, R>).WithEnv<Env>());

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, A1, A, B1, B, R> liftIO<Env, A1, A, B1, B, R>(EffPure<R> ma) where Env : struct, HasCancel<Env> =>
            new M<Env, A1, A, B1, B, R>(ma.Map(Pure<Env, A1, A, B1, B, R>).ToAsyncWithEnv<Env>());

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, A1, A, B1, B, R> liftIO<Env, A1, A, B1, B, R>(Aff<Env, R> ma) where Env : struct, HasCancel<Env> =>
            new M<Env, A1, A, B1, B, R>(ma.Map(Pure<Env, A1, A, B1, B, R>));

        /// <summary>
        /// Lift am IO monad into the `Proxy` monad transformer
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, A1, A, B1, B, R> liftIO<Env, A1, A, B1, B, R>(Eff<Env, R> ma) where Env : struct, HasCancel<Env> =>
            new M<Env, A1, A, B1, B, R>(ma.Map(Pure<Env, A1, A, B1, B, R>).ToAsync());
        /// <summary>
        /// Converts a `Proxy` with the correct _shape_ into an `Effect`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<Env, R> ToEffect<Env, R>(this Proxy<Env, Void, Unit, Unit, Void, R> ma) where Env : struct, HasCancel<Env> =>
            new Effect<Env, R>(ma);
        
        /// <summary>
        /// Converts a `Proxy` with the correct _shape_ into a `Producer`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, A, R> ToProducer<Env, A, R>(this Proxy<Env, Void, Unit, Unit, A, R> ma) where Env : struct, HasCancel<Env> =>
            new Producer<Env, A, R>(ma);
        
        /// <summary>
        /// Converts a `Proxy` with the correct _shape_ into a `Consumer`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> ToConsumer<Env, A, R>(this Proxy<Env, Unit, A, Unit, Void, R> ma) where Env : struct, HasCancel<Env> =>
            new Consumer<Env, A, R>(ma);
        
        /// <summary>
        /// Converts a `Proxy` with the correct _shape_ into n `Pipe`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, B, R> ToPipe<Env, A, B, R>(this Proxy<Env, Unit, A, Unit, B, R> ma) where Env : struct, HasCancel<Env> =>
            new Pipe<Env, A, B, R>(ma);
        
        /// <summary>
        /// Converts a `Proxy` with the correct _shape_ into a `Client`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<Env, A, B, R> ToClient<Env, A, B, R>(this Proxy<Env, A, B, Unit, Unit, R> ma) where Env : struct, HasCancel<Env> =>
            new Client<Env, A, B, R>(ma);
        
        /// <summary>
        /// Converts a `Proxy` with the correct _shape_ into a `Server`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<Env, A, B, R> ToServer<Env, A, B, R>(this Proxy<Env, Unit, Unit, A, B, R> ma) where Env : struct, HasCancel<Env> =>
            new Server<Env, A, B, R>(ma);
        
        /// <summary>
        /// The identity `Pipe`, simply replicates its upstream value and propagates it downstream 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, A, R> cat<Env, A, R>() where Env : struct, HasCancel<Env> =>
            pull<Env, Unit, A, R>(default).ToPipe();
        
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
        public static Proxy<Env, A1, A, A1, A, R> pull<Env, A1, A, R>(A1 a1) where Env : struct, HasCancel<Env> =>
            new Request<Env, A1, A, A1, A, R>(a1, a => new Respond<Env, A1, A, A1, A, R>(a, pull<Env, A1, A, R>));

        /// <summary>
        /// `push = respond | request | push`
        /// </summary>
        /// <remarks>
        /// `push` is the identity of the push category.
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, A1, A, A1, A, R> push<Env, A1, A, R>(A a) where Env : struct, HasCancel<Env> =>
            new Respond<Env, A1, A, A1, A, R>(a, a1 => new Request<Env, A1, A, A1, A, R>(a1, push<Env, A1, A, R>));
        
        /// <summary>
        /// Send a value of type `a` downstream and block waiting for a reply of type `a`
        /// </summary>
        /// <remarks>
        /// `respond` is the identity of the respond category.
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, X1, X, A1, A, A1> respond<Env, X1, X, A1, A>(A value) where Env : struct, HasCancel<Env> =>
            new Respond<Env, X1, X, A1, A, A1>(value, r => new Pure<Env, X1, X, A1, A, A1>(r));

        /// <summary>
        /// Send a value of type `a` upstream and block waiting for a reply of type `a`
        /// </summary>
        /// <remarks>
        /// `request` is the identity of the request category.
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, A1, A, Y1, Y, A> request<Env, A1, A, Y1, Y>(A1 value) where Env : struct, HasCancel<Env> =>
            new Request<Env, A1, A, Y1, Y, A>(value, r => new Pure<Env, A1, A, Y1, Y, A>(r));
        
       
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
        public static Proxy<Env, B, B1, A, A1, R> reflect<Env, A1, A, B1, B, R>(Proxy<Env, A1, A, B1, B, R> p) where Env : struct, HasCancel<Env>
        {
            return Go(p);
            Proxy<Env, B, B1, A, A1, R> Go(Proxy<Env, A1, A, B1, B, R> p) =>
                p.ToProxy() switch
                {
                    Request<Env, A1, A, B1, B, R> (var a1, var fa) => new Respond<Env, B, B1, A, A1, R>(a1, a => Go(fa(a))),
                    Respond<Env, A1, A, B1, B, R> (var b, var fb1) => new Request<Env, B, B1, A, A1, R>(b, b1 => Go(fb1(b1))),
                    M<Env, A1, A, B1, B, R> (var m)                => new M<Env, B, B1, A, A1, R>(m.Map(Go)),
                    Pure<Env, A1, A, B1, B, R> (var r)             => Pure<Env, B, B1, A, A1, R>(r),                                                                                
                    _                                              => throw new NotSupportedException()
                };
        }
        
        /// <summary>
        /// `p.For(body)` loops over `p` replacing each `yield` with `body`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, X1, X, C1, C, R> For<Env, X1, X, B1, B, C1, C, R>(this
            Proxy<Env, X1, X, B1, B, R> p0,
            Func<B, Proxy<Env, X1, X, C1, C, B1>> fb) where Env : struct, HasCancel<Env>
        {
            return Go(p0);
            Proxy<Env, X1, X, C1, C, R> Go(Proxy<Env, X1, X, B1, B, R> p) =>
                p.ToProxy() switch
                {
                    Request<Env, X1, X, B1, B, R> (var x1, var fx) => new Request<Env, X1, X, C1, C, R>(x1, x => Go(fx(x))),
                    Respond<Env, X1, X, B1, B, R> (var b, var fb1) => fb(b).Bind(b1 => Go(fb1(b1))),
                    M<Env, X1, X, B1, B, R> (var m)                => new M<Env, X1, X, C1, C, R>(m.Map(Go)),
                    Pure<Env, X1, X, B1, B, R> (var a)             => Pure<Env, X1, X, C1, C, R>(a),                                                                                
                    _                                              => throw new NotSupportedException()
                };
        }

        /// <summary>
        /// `p.For(body)` loops over `p` replacing each `yield` with `body`
        /// 
        /// Producer b r -> (b -> Producer c ()) -> Producer c r
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, C, R> For<Env, B, C, R>(this
            Producer<Env, B, R> p0,  
            Func<B, Producer<Env, C, Unit>> fb) where Env : struct, HasCancel<Env> =>
            For<Env, Void, Unit, Unit, B, Unit, C, R>(p0, fb).ToProducer();

        /// <summary>
        /// `p.For(body)` loops over `p` replacing each `yield` with `body`
        /// 
        /// Producer b r -> (b -> Effect ()) -> Effect r
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<Env, R> For<Env, B, R>(this
            Producer<Env, B, R> p0,  
            Func<B, Effect<Env, Unit>> fb) where Env : struct, HasCancel<Env> =>
            For<Env, Void, Unit, Unit, B, Unit, Void, R>(p0, fb).ToEffect();

        /// <summary>
        /// `p.For(body)` loops over `p` replacing each `yield` with `body`
        /// 
        /// Pipe x b r -> (b -> Consumer x ()) -> Consumer x r
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, X, R> For<Env, X, B, R>(this
            Pipe<Env, X, B, R> p0,
            Func<B, Consumer<Env, X, Unit>> fb) where Env : struct, HasCancel<Env> =>
            For<Env, Unit, X, Unit, B, Unit, Void, R>(p0, fb).ToConsumer();

        /// <summary>
        /// `p.For(body)` loops over `p` replacing each `yield` with `body`
        /// 
        /// Pipe x b r -> (b -> Pipe x c ()) -> Pipe x c r
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, X, C, R> For<Env, X, B, C, R>(this
            Pipe<Env, X, B, R> p0,
            Func<B, Pipe<Env, X, C, Unit>> fb) where Env : struct, HasCancel<Env> =>  
            For<Env, Unit, X, Unit, B, Unit, C, R>(p0, fb).ToPipe(); 

        
        /// <summary>
        /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, A1, A, Y1, Y, C> compose<Env, A1, A, Y1, Y, B, C>(
            Proxy<Env, A1, A, Y1, Y, B> p1,
            Proxy<Env, Unit, B, Y1, Y, C> p2) where Env : struct, HasCancel<Env> =>
            compose((Unit _) => p1, p2);
        
        /// <summary>
        /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
        /// 
        /// Effect b -> Consumer b c -> Effect c
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<Env, C> compose<Env, B, C>(
            Effect<Env, B> p1,
            Consumer<Env, B, C> p2) where Env : struct, HasCancel<Env> =>
            compose((Unit _) => p1, p2).ToEffect();        
        
        /// <summary>
        /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
        /// 
        /// Consumer a b -> Consumer b c -> Consumer a c
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, C> compose<Env, A, B, C>(
            Consumer<Env, A, B> p1,
            Consumer<Env, B, C> p2) where Env : struct, HasCancel<Env> =>
            compose((Unit _) => p1, p2).ToConsumer();
        
        /// <summary>
        /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
        /// 
        /// Producer y b -> Pipe b y m c -> Producer y c
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, Y, C> compose<Env, Y, B, C>(
            Producer<Env, Y, B> p1,
            Pipe<Env, B, Y, C> p2) where Env : struct, HasCancel<Env> =>
            compose((Unit _) => p1, p2).ToProducer();         
        
        /// <summary>
        /// `compose(draw, p)` loops over `p` replacing each `await` with `draw`
        /// 
        /// Pipe a y b -> Pipe b y c -> Pipe a y c
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, Y, C> compose<Env, Y, A, B, C>(
            Pipe<Env, A, Y, B> p1,
            Pipe<Env, B, Y, C> p2) where Env : struct, HasCancel<Env> =>
            compose((Unit _) => p1, p2).ToPipe();         
        
        // fixAwaitDual
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, A1, A, Y1, Y, C> compose<Env, A1, A, Y1, Y, B, C>(
            Proxy<Env, Unit, B, Y1, Y, C> p2,
            Proxy<Env, A1, A, Y1, Y, B> p1) where Env : struct, HasCancel<Env> =>
            compose(p1, p2);

        /// <summary>
        /// Replaces each `request` or `respond` in `p0` with `fb1`.
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, A1, A, Y1, Y, C> compose<Env, A1, A, B1, B, Y1, Y, C>(
            Func<B1, Proxy<Env, A1, A, Y1, Y, B>> fb1,
            Proxy<Env, B1, B, Y1, Y, C> p0) where Env : struct, HasCancel<Env> 
        {
            return Go(p0);
            Proxy<Env, A1, A, Y1, Y, C> Go(Proxy<Env, B1, B, Y1, Y, C> p) =>
                p.ToProxy() switch
                {
                    Request<Env, B1, B, Y1, Y, C> (var b1, var fb) => fb1(b1).Bind(b => Go(fb(b))),
                    Respond<Env, B1, B, Y1, Y, C> (var x, var fx1) => new Respond<Env, A1, A, Y1, Y, C>(x, x1 => Go(fx1(x1))),
                    M<Env, B1, B, Y1, Y, C> (var m)                => new M<Env, A1, A, Y1, Y, C>(m.Map(Go)),
                    Pure<Env, B1, B, Y1, Y, C> (var a)             => Pure<Env, A1, A, Y1, Y, C>(a),                                                                                
                    _                                              => throw new NotSupportedException()
                };
        }        
        
        /// <summary>
        /// `compose(p, f)` pairs each `respond` in `p` with a `request` in `f`.
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, A1, A, C1, C, R> compose<Env, A1, A, B1, B, C1, C, R>(
            Proxy<Env, A1, A, B1, B, R> p,
            Func<B, Proxy<Env, B1, B, C1, C, R>> fb
            )
            where Env : struct, HasCancel<Env> => 
                p.ToProxy() switch
                {
                    Request<Env, A1, A, B1, B, R> (var a1, var fa) => new Request<Env, A1, A, C1, C, R>(a1, a => compose(fa(a), fb)),
                    Respond<Env, A1, A, B1, B, R> (var b, var fb1) => compose(fb1, fb(b)),
                    M<Env, A1, A, B1, B, R> (var m)                => new M<Env, A1, A, C1, C, R>(m.Map(p1 => compose(p1, fb))),
                    Pure<Env, A1, A, B1, B, R> (var r)             => Pure<Env, A1, A, C1, C, R>(r),                                                                                
                    _                                              => throw new NotSupportedException()
                };        
        
        /// <summary>
        /// `compose(f, p)` pairs each `request` in `p` with a `respond` in `f`
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, A1, A, C1, C, R> compose<Env, A1, A, B1, B, C1, C, R>(
            Func<B1, Proxy<Env, A1, A, B1, B, R>> fb1,
            Proxy<Env, B1, B, C1, C, R> p) 
            where Env : struct, HasCancel<Env> => 
                p.ToProxy() switch
                {
                    Request<Env, B1, B, C1, C, R> (var b1, var fb) => compose(fb1(b1), fb),
                    Respond<Env, B1, B, C1, C, R> (var c, var fc1) => new Respond<Env, A1, A, C1, C, R>(c, c1 => compose(fb1, fc1(c1))),
                    M<Env, B1, B, C1, C, R> (var m)                => new M<Env, A1, A, C1, C, R>(m.Map(p1 => compose(fb1, p1))),
                    Pure<Env, B1, B, C1, C, R> (var r)             => Pure<Env, A1, A, C1, C, R>(r),                      
                    _                                              => throw new NotSupportedException()
                };
        
        /// <summary>
        /// Pipe composition
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, A1, A, C1, C, R> compose<Env, A1, A, B, C1, C, R>(
            Proxy<Env, A1, A, Unit, B, R> p1,
            Proxy<Env, Unit, B, C1, C, R> p2) 
            where Env : struct, HasCancel<Env> =>
                compose((Unit _) => p1, p2);

        /// <summary>
        /// Pipe composition
        ///
        ///     Producer b r -> Consumer b r -> Effect m r
        /// 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Effect<Env, R> compose<Env, B, R>(
            Producer<Env, B, R> p1,
            Consumer<Env, B, R> p2) 
            where Env : struct, HasCancel<Env> =>
                compose((Unit _) => p1.ToProxy(), p2).ToEffect();

        /// <summary>
        /// Pipe composition
        ///
        ///     Producer b r -> Pipe b c r -> Producer c r
        /// 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, C, R> compose<Env, B, C, R>(
            Producer<Env, B, R> p1,
            Pipe<Env, B, C, R> p2) 
            where Env : struct, HasCancel<Env> =>
            compose((Unit _) => p1.ToProxy(), p2).ToProducer();

        /// <summary>
        /// Pipe composition
        ///
        ///     Pipe a b r -> Consumer b r -> Consumer a r
        /// 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> compose<Env, A, B, R>(
            Pipe<Env, A, B, R> p1,
            Consumer<Env, B, R> p2) 
            where Env : struct, HasCancel<Env> =>
            compose((Unit _) => p1.ToProxy(), p2).ToConsumer();

        /// <summary>
        /// Pipe composition
        ///
        ///     Pipe a b r -> Pipe b c r -> Pipe a c r
        /// 
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, C, R> compose<Env, A, B, C, R>(
            Pipe<Env, A, B, R> p1,
            Pipe<Env, B, C, R> p2) 
            where Env : struct, HasCancel<Env> =>
            compose((Unit _) => p1.ToProxy(), p2).ToPipe();

        /// <summary>
        /// Compose two unfolds, creating a new unfold
        /// </summary>
        /// <remarks>
        /// This is the composition operator of the respond category.
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Func<A, Proxy<Env, X1, X, C1, C, A1>> compose<Env, X1, X, A1, A, B1, B, C1, C>(
            Func<A, Proxy<Env, X1, X, B1, B, A1>> fa,
            Func<B, Proxy<Env, X1, X, C1, C, B1>> fb) where Env : struct, HasCancel<Env> =>
                a => compose(fa(a), fb);
        
        /// <summary>
        /// Compose two unfolds, creating a new unfold
        /// </summary>
        /// <remarks>
        /// This is the composition operator of the respond category.
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Func<A, Proxy<Env, X1, X, C1, C, A1>> Then<Env, X1, X, A1, A, B1, B, C1, C>(
            this Func<A, Proxy<Env, X1, X, B1, B, A1>> fa,
            Func<B, Proxy<Env, X1, X, C1, C, B1>> fb) where Env : struct, HasCancel<Env> =>
            a => compose(fa(a), fb);
        
        /// <summary>
        /// `compose(p, f)` replaces each `respond` in `p` with `f`.
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, X1, X, C1, C, A1> compose<Env, X1, X, A1, B1, C1, C, B>(
            Proxy<Env, X1, X, B1, B, A1> p0, 
            Func<B, Proxy<Env, X1, X, C1, C, B1>> fb) where Env : struct, HasCancel<Env>
        {
            return Go(p0);
            Proxy<Env, X1, X, C1, C, A1> Go(Proxy<Env, X1, X, B1, B, A1> p) =>
                p.ToProxy() switch
                {
                    Request<Env, X1, X, B1, B, A1> (var x1, var fx) => new Request<Env, X1, X, C1, C, A1>(x1, x => Go(fx(x))),
                    Respond<Env, X1, X, B1, B, A1> (var b, var fb1) => fb(b).Bind(b1 => Go(fb1(b1))),
                    M<Env, X1, X, B1, B, A1> (var m)                => new M<Env, X1, X, C1, C, A1>(m.Map(Go)),
                    Pure<Env, X1, X, B1, B, A1> (var a)             => Pure<Env, X1, X, C1, C, A1>(a),
                    _                                               => throw new NotSupportedException()
                };
        }
        
        /// <summary>
        /// `compose(p, f)` replaces each `respond` in `p` with `f`.
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, X1, X, C1, C, A1> Then<Env, X1, X, A1, B1, C1, C, B>(
            this Proxy<Env, X1, X, B1, B, A1> p0, 
            Func<B, Proxy<Env, X1, X, C1, C, B1>> fb) where Env : struct, HasCancel<Env> =>
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
        public static Func<C1, Proxy<Env, A1, A, Y1, Y, C>> compose<Env, A1, A, B1, B, Y1, Y, C1, C>(
            Func<B1, Proxy<Env, A1, A, Y1, Y, B>> fb1, 
            Func<C1, Proxy<Env, B1, B, Y1, Y, C>> fc1) where Env : struct, HasCancel<Env> =>
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
        public static Proxy<Env, A1, A, B1, B, R> observe<Env, A1, A, B1, B, R>(Proxy<Env, A1, A, B1, B, R> p0) where Env : struct, HasCancel<Env>
        {
            return new M<Env, A1, A, B1, B, R>(Go(p0));
            Aff<Env, Proxy<Env, A1, A, B1, B, R>> Go(Proxy<Env, A1, A, B1, B, R> p) =>
                p.ToProxy() switch
                {
                    Request<Env, A1, A, B1, B, R> (var a1, var fa) => Aff<Env, Proxy<Env, A1, A, B1, B, R>>.Success(new Request<Env, A1, A, B1, B, R>(a1, a => observe(fa(a)))),
                    Respond<Env, A1, A, B1, B, R> (var b, var fb1) => Aff<Env, Proxy<Env, A1, A, B1, B, R>>.Success(new Respond<Env, A1, A, B1, B, R>(b, b1 => observe(fb1(b1)))),
                    M<Env, A1, A, B1, B, R> (var m1)               => m1.Bind(Go),
                    Pure<Env, A1, A, B1, B, R> (var r)             => Aff<Env, Proxy<Env, A1, A, B1, B, R>>.Success(new Pure<Env, A1, A, B1, B, R>(r)),                                                                                
                    _                                              => throw new NotSupportedException()
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
        public static Proxy<Env, A1, A, B1, B, S> apply<Env, A1, A, B1, B, R, S>(Proxy<Env, A1, A, B1, B, Func<R, S>> pf, Proxy<Env, A1, A, B1, B, R> px) where Env : struct, HasCancel<Env>
        {
            return Go(pf);
            Proxy<Env, A1, A, B1, B, S> Go(Proxy<Env, A1, A, B1, B, Func<R, S>> p) =>
                p.ToProxy() switch
                {
                    Request<Env, A1, A, B1, B, Func<R, S>> (var a1, var fa) => new Request<Env, A1, A, B1, B, S>(a1, a => Go(fa(a))),
                    Respond<Env, A1, A, B1, B, Func<R, S>> (var b, var fb1) => new Respond<Env, A1, A, B1, B, S>(b, b1 => Go(fb1(b1))),
                    M<Env, A1, A, B1, B, Func<R, S>> (var m)                => new M<Env, A1, A, B1, B, S>(m.Map(Go)),
                    Pure<Env, A1, A, B1, B, Func<R, S>> (var f)             => px.Map(f),                                                                                
                    _                                                       => throw new NotSupportedException()
                };
        }

        /// <summary>
        /// Applicative apply
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, A1, A, B1, B, S> Apply<Env, A1, A, B1, B, R, S>(this Proxy<Env, A1, A, B1, B, Func<R, S>> pf, Proxy<Env, A1, A, B1, B, R> px) where Env : struct, HasCancel<Env> =>
            apply(pf, px);

        /// <summary>
        /// Applicative action
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, A1, A, B1, B, S> action<Env, A1, A, B1, B, R, S>(Proxy<Env, A1, A, B1, B, R> l, Proxy<Env, A1, A, B1, B, S> r) where Env : struct, HasCancel<Env>
        {
            return Go(l);
            Proxy<Env, A1, A, B1, B, S> Go(Proxy<Env, A1, A, B1, B, R> p) =>
                p.ToProxy() switch
                {
                    Request<Env, A1, A, B1, B, R> (var a1, var fa) => new Request<Env, A1, A, B1, B, S>(a1, a => Go(fa(a))),
                    Respond<Env, A1, A, B1, B, R> (var b, var fb1) => new Respond<Env, A1, A, B1, B, S>(b, b1 => Go(fb1(b1))),
                    M<Env, A1, A, B1, B, R> (var m)                => new M<Env, A1, A, B1, B, S>(m.Map(Go)),
                    Pure<Env, A1, A, B1, B, R> (var _)             => r,                                                                                
                    _                                              => throw new NotSupportedException()
                };
        }

        /// <summary>
        /// Applicative action
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, A1, A, B1, B, S> Action<Env, A1, A, B1, B, R, S>(this Proxy<Env, A1, A, B1, B, R> l, Proxy<Env, A1, A, B1, B, S> r) where Env : struct, HasCancel<Env> =>
            action(l, r);

        /// <summary>
        /// Monad return / pure
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, A1, A, B1, B, R> Pure<Env, A1, A, B1, B, R>(R value) where Env : struct, HasCancel<Env> =>
            new Pure<Env, A1, A, B1, B, R>(value);
    }
}
