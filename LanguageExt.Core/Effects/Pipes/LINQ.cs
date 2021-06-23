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
        public static Proxy<Env, A1, A, B1, B, S> Map<Env, A1, A, B1, B, R, S>(this Proxy<Env, A1, A, B1, B, R> ma, Func<R, S> f) 
            where Env : struct, HasCancel<Env>
        {
            return Go(ma);

            Proxy<Env, A1, A, B1, B, S> Go(Proxy<Env, A1, A, B1, B, R> p) =>
                p.ToProxy() switch
                {
                    Request<Env, A1, A, B1, B, R> (var a1, var fa) => new Request<Env, A1, A, B1, B, S>(a1, a => Go(fa(a))),
                    Respond<Env, A1, A, B1, B, R> (var b, var fb1) => new Respond<Env, A1, A, B1, B, S>(b, b1 => Go(fb1(b1))),
                    M<Env, A1, A, B1, B, R> (var m)                => new M<Env, A1, A, B1, B, S>(m.Map(Go)),
                    Pure<Env, A1, A, B1, B, R> (var r)             => new Pure<Env, A1, A, B1, B, S>(f(r)),                                                                                
                    _                                              => throw new NotSupportedException()
                };
        } 
        
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, A1, A, B1, B, S> Select<Env, A1, A, B1, B, R, S>(this Proxy<Env, A1, A, B1, B, R> ma, Func<R, S> f)
            where Env : struct, HasCancel<Env> =>
            ma.Map(f);
        
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, A1, A, B1, B, S> SelectMany<Env, A1, A, B1, B, R, S>(this Proxy<Env, A1, A, B1, B, R> ma, Func<R, Proxy<Env, A1, A, B1, B, S>> f)
            where Env : struct, HasCancel<Env> =>
            ma.Bind(f);

        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, A1, A, B1, B, T> SelectMany<Env, A1, A, B1, B, R, S, T>(this Proxy<Env, A1, A, B1, B, R> ma,
            Func<R, Proxy<Env, A1, A, B1, B, S>> bind,
            Func<R, S, T> project)
            where Env : struct, HasCancel<Env> =>
            ma.Bind(x => bind(x).Map(y => project(x, y)));

        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, B, T> SelectMany<Env, B, R, S, T>(this Proxy<Env, Void, Unit, Unit, B, R> ma,
            Func<R, Producer<Env, B, S>> bind,
            Func<R, S, T> project)
            where Env : struct, HasCancel<Env> =>
            ma.Bind(x => bind(x).Value.Map(y => project(x, y))).ToProducer();

        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, T> SelectMany<Env, A, R, S, T>(this Proxy<Env, Unit, A, Unit, Void, R> ma,
            Func<R, Consumer<Env, A, S>> bind,
            Func<R, S, T> project)
            where Env : struct, HasCancel<Env> =>
            ma.Bind(x => bind(x).Value.Map(y => project(x, y))).ToConsumer();
 
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, A, T> SelectMany<Env, A, R, S, T>(this Pipe<Env, A, A, R> ma,
            Func<R, Pipe<Env, A, A, S>> bind,
            Func<R, S, T> project)
            where Env : struct, HasCancel<Env> =>
            ma.Bind(x => bind(x).Value.Map(y => project(x, y))).ToPipe();

        /// <summary>
        /// Monad bind
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        static Proxy<Env, A1, A, B1, B, S> BindInternal<Env, A1, A, B1, B, R, S>(
            this Proxy<Env, A1, A, B1, B, R> ma, 
            Func<R, Proxy<Env, A1, A, B1, B, S>> f) where Env : struct, HasCancel<Env>
        {
           
            return Go(ma);

            Proxy<Env, A1, A, B1, B, S> Go(Proxy<Env, A1, A, B1, B, R> p) =>
                p.ToProxy() switch
                {
                    Request<Env, A1, A, B1, B, R> (var a1, var fa) => new Request<Env, A1, A, B1, B, S>(a1, a => Go(fa(a))),
                    Respond<Env, A1, A, B1, B, R> (var b, var fb1) => new Respond<Env, A1, A, B1, B, S>(b, b1 => Go(fb1(b1))),
                    M<Env, A1, A, B1, B, R> (var m)                => new M<Env, A1, A, B1, B, S>(m.Map(Go)),
                    Pure<Env, A1, A, B1, B, R> (var r)             => f(r),                                                                                
                    _                                              => throw new NotSupportedException()
                };
        }
        
        /// <summary>
        /// Monad bind
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<Env, A1, A, B1, B, S> Bind<Env, A1, A, B1, B, R, S>(this Proxy<Env, A1, A, B1, B, R> ma, Func<R, Proxy<Env, A1, A, B1, B, S>> f) 
            where Env : struct, HasCancel<Env> =>
                BindInternal(ma, f);
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, B, S> Bind<Env, B, R, S>(this Proxy<Env, Void, Unit, Unit, B, R> ma, Func<R, Producer<Env, B, S>> f) 
            where Env : struct, HasCancel<Env> =>
                BindInternal(ma, f).ToProducer();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, S> Bind<Env, A, R, S>(this Proxy<Env, Unit, A, Unit, Void, R> ma, Func<R, Consumer<Env, A, S>> f) 
            where Env : struct, HasCancel<Env> =>
                BindInternal(ma, f).ToConsumer();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<Env, A, A, S> Bind<Env, A, R, S>(this Pipe<Env, A, A, R> ma, Func<R, Pipe<Env, A, A, S>> f) 
            where Env : struct, HasCancel<Env> =>
                BindInternal(ma, f).ToPipe();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, A, S> Bind<Env, A, R, S>(this Producer<Env, A, R> ma, Func<R, Producer<Env, A, S>> f) 
            where Env : struct, HasCancel<Env> =>
                BindInternal(ma, f).ToProducer();        
    }
}
