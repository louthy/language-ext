using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Effects.Traits;
using LanguageExt.Pipes;
using Void = LanguageExt.Pipes.Void;

namespace LanguageExt
{
    public static class ProxyExtensions
    {
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, S> Select<RT, A1, A, B1, B, R, S>(this Proxy<RT, A1, A, B1, B, R> ma, Func<R, S> f)
            where RT : struct, HasCancel<RT> =>
            ma.Map(f);
        
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, B> Select<RT, IN, OUT, A, B>(this Pipe<RT, IN, OUT, A> ma, Func<A, B> f) where RT : struct, HasCancel<RT> =>
            ma.Map(f).ToPipe();
        
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT, B> Select<RT, OUT, A, B>(this Producer<RT, OUT, A> ma, Func<A, B> f) where RT : struct, HasCancel<RT> =>
            ma.Map(f).ToProducer();
        
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, B> Select<RT, IN, A, B>(this Consumer<RT, IN, A> ma, Func<A, B> f) where RT : struct, HasCancel<RT> =>
            ma.Map(f).ToConsumer();

        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, B, T> SelectMany<RT, B, R, S, T>(this Proxy<RT, Void, Unit, Unit, B, R> ma,
            Func<R, Producer<RT, B, S>> bind,
            Func<R, S, T> project)
            where RT : struct, HasCancel<RT> =>
            ma.Bind(x => bind(x).Value.Map(y => project(x, y))).ToProducer();

        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, T> SelectMany<RT, A, R, S, T>(this Proxy<RT, Unit, A, Unit, Void, R> ma,
            Func<R, Consumer<RT, A, S>> bind,
            Func<R, S, T> project)
            where RT : struct, HasCancel<RT> =>
            ma.Bind(x => bind(x).Value.Map(y => project(x, y))).ToConsumer();
 
        // Pipe bindings
        
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(
            this Pipe<RT, IN, OUT, A> ma,
            Func<A, Pipe<RT, IN, OUT, B>> bind,
            Func<A, B, C> project)
            where RT : struct, HasCancel<RT> =>
            ma.Bind(x => bind(x).Value.Map(y => project(x, y))).ToPipe();
 
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(
            this Pipe<RT, IN, OUT, A> ma,
            Func<A, Pipe<RT, IN, OUT, B>> bind)
            where RT : struct, HasCancel<RT> =>
                ma.Bind(x => bind(x).Value).ToPipe();
 
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(
            this Proxy<RT, Unit, IN, Unit, OUT, A> ma,
            Func<A, Pipe<RT, IN, OUT, B>> bind)
            where RT : struct, HasCancel<RT> =>
                ma.Bind(x => bind(x).Value).ToPipe();
 
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(
            this Pipe<RT, IN, OUT, A> ma,
            Func<A, Proxy<RT, Unit, IN, Unit, OUT, B>> bind)
            where RT : struct, HasCancel<RT> =>
                ma.Bind(bind).ToPipe();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, B, S> Bind<RT, B, R, S>(this Proxy<RT, Void, Unit, Unit, B, R> ma, Func<R, Producer<RT, B, S>> f) 
            where RT : struct, HasCancel<RT> =>
                ma.Bind(f).ToProducer();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, S> Bind<RT, A, R, S>(this Proxy<RT, Unit, A, Unit, Void, R> ma, Func<R, Consumer<RT, A, S>> f) 
            where RT : struct, HasCancel<RT> =>
                ma.Bind(f).ToConsumer();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, S> Bind<RT, A, B, R, S>(this Pipe<RT, A, B, R> ma, Func<R, Pipe<RT, A, B, S>> f) 
            where RT : struct, HasCancel<RT> =>
                ma.Bind(f).ToPipe();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, A, S> Bind<RT, A, R, S>(this Producer<RT, A, R> ma, Func<R, Producer<RT, A, S>> f) 
            where RT : struct, HasCancel<RT> =>
                ma.Bind(f).ToProducer();        
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, B> Bind<RT, REQ, RES, A, B>(this Server<RT, REQ, RES, A> ma, Func<A, Server<RT, REQ, RES, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ma.Bind(f).ToServer();        
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, B> SelectMany<RT, REQ, RES, A, B>(this Server<RT, REQ, RES, A> ma, Func<A, Server<RT, REQ, RES, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ma.Bind(f).ToServer();        
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, C> SelectMany<RT, REQ, RES, A, B, C>(this Server<RT, REQ, RES, A> ma, Func<A, Server<RT, REQ, RES, B>> f, Func<A, B, C> project) 
            where RT : struct, HasCancel<RT> =>
            ma.Bind(a => f(a).Select(b => project(a,b))).ToServer();      
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, B> Bind<RT, REQ, RES, A, B>(this Client<RT, REQ, RES, A> ma, Func<A, Client<RT, REQ, RES, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ma.Bind(f).ToClient();        
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, B> SelectMany<RT, REQ, RES, A, B>(this Client<RT, REQ, RES, A> ma, Func<A, Client<RT, REQ, RES, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ma.Bind(f).ToClient();        
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, C> SelectMany<RT, REQ, RES, A, B, C>(this Client<RT, REQ, RES, A> ma, Func<A, Client<RT, REQ, RES, B>> f, Func<A, B, C> project) 
            where RT : struct, HasCancel<RT> =>
            ma.Bind(a => f(a).Select(b => project(a,b))).ToClient();        }
}
