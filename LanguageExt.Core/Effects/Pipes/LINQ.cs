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
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT, B> Bind<RT, OUT, A, B>(this Proxy<RT, Void, Unit, Unit, OUT, A> ma, Func<A, Producer<RT, OUT, B>> f) 
            where RT : struct, HasCancel<RT> =>
                ma.Bind(f).ToProducer();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, B> Bind<RT, IN, A, B>(this Proxy<RT, Unit, IN, Unit, Void, A> ma, Func<A, Consumer<RT, IN, B>> f) 
            where RT : struct, HasCancel<RT> =>
                ma.Bind(f).ToConsumer();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, B> Bind<RT, IN, OUT, A, B>(this Proxy<RT, Unit, IN, Unit, OUT, A> ma, Func<A, Pipe<RT, IN, OUT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ma.Bind(f).ToPipe();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, B> Bind<RT, REQ, RES, A, B>(this Proxy<RT, REQ, RES, Unit, Void, A> ma, Func<A, Client<RT, REQ, RES, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ma.Bind(f).ToClient();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, B> Bind<RT, REQ, RES, A, B>(this Proxy<RT, Void, Unit, REQ, RES, A> ma, Func<A, Server<RT, REQ, RES, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ma.Bind(f).ToServer();

        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT, B> SelectMany<RT, OUT, A, B>(this Proxy<RT, Void, Unit, Unit, OUT, A> ma, Func<A, Producer<RT, OUT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ma.Bind(f).ToProducer();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, B> SelectMany<RT, IN, A, B>(this Proxy<RT, Unit, IN, Unit, Void, A> ma, Func<A, Consumer<RT, IN, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ma.Bind(f).ToConsumer();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, B> SelectMany<RT, IN, OUT, A, B>(this Proxy<RT, Unit, IN, Unit, OUT, A> ma, Func<A, Pipe<RT, IN, OUT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ma.Bind(f).ToPipe();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, B> SelectMany<RT, REQ, RES, A, B>(this Proxy<RT, REQ, RES, Unit, Void, A> ma, Func<A, Client<RT, REQ, RES, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ma.Bind(f).ToClient();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, B> SelectMany<RT, REQ, RES, A, B>(this Proxy<RT, Void, Unit, REQ, RES, A> ma, Func<A, Server<RT, REQ, RES, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ma.Bind(f).ToServer();

        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT, C> SelectMany<RT, OUT, A, B, C>(this Proxy<RT, Void, Unit, Unit, OUT, A> ma, Func<A, Producer<RT, OUT, B>> f, Func<A, B, C> project) 
            where RT : struct, HasCancel<RT> =>
            ma.Bind(a => f(a).Map(b => project(a, b))).ToProducer();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, IN, C> SelectMany<RT, IN, A, B, C>(this Proxy<RT, Unit, IN, Unit, Void, A> ma, Func<A, Consumer<RT, IN, B>> f, Func<A, B, C> project) 
            where RT : struct, HasCancel<RT> =>
            ma.Bind(a => f(a).Map(b => project(a, b))).ToConsumer();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, IN, OUT, C> SelectMany<RT, IN, OUT, A, B, C>(this Proxy<RT, Unit, IN, Unit, OUT, A> ma, Func<A, Pipe<RT, IN, OUT, B>> f, Func<A, B, C> project) 
            where RT : struct, HasCancel<RT> =>
            ma.Bind(a => f(a).Map(b => project(a, b))).ToPipe();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Client<RT, REQ, RES, C> SelectMany<RT, REQ, RES, A, B, C>(this Proxy<RT, REQ, RES, Unit, Void, A> ma, Func<A, Client<RT, REQ, RES, B>> f, Func<A, B, C> project) 
            where RT : struct, HasCancel<RT> =>
            ma.Bind(a => f(a).Map(b => project(a, b))).ToClient();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Server<RT, REQ, RES, C> SelectMany<RT, REQ, RES, A, B, C>(this Proxy<RT, Void, Unit, REQ, RES, A> ma, Func<A, Server<RT, REQ, RES, B>> f, Func<A, B, C> project) 
            where RT : struct, HasCancel<RT> =>
            ma.Bind(a => f(a).Map(b => project(a, b))).ToServer();
    }
}
