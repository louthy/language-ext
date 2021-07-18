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
        public static Proxy<RT, A1, A, B1, B, S> Map<RT, A1, A, B1, B, R, S>(this Proxy<RT, A1, A, B1, B, R> ma, Func<R, S> f) 
            where RT : struct, HasCancel<RT>
        {
            return Go(ma);

            Proxy<RT, A1, A, B1, B, S> Go(Proxy<RT, A1, A, B1, B, R> p) =>
                p.ToProxy() switch
                {
                    Request<RT, A1, A, B1, B, R> (var a1, var fa) => new Request<RT, A1, A, B1, B, S>(a1, a => Go(fa(a))),
                    Respond<RT, A1, A, B1, B, R> (var b, var fb1) => new Respond<RT, A1, A, B1, B, S>(b, b1 => Go(fb1(b1))),
                    M<RT, A1, A, B1, B, R> (var m)                => new M<RT, A1, A, B1, B, S>(m.Map(Go)),
                    Pure<RT, A1, A, B1, B, R> (var r)             => new Pure<RT, A1, A, B1, B, S>(f(r)),                                                                                
                    _                                              => throw new NotSupportedException()
                };
        } 
        
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, S> Select<RT, A1, A, B1, B, R, S>(this Proxy<RT, A1, A, B1, B, R> ma, Func<R, S> f)
            where RT : struct, HasCancel<RT> =>
            ma.Map(f);
        
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, S> SelectMany<RT, A1, A, B1, B, R, S>(this Proxy<RT, A1, A, B1, B, R> ma, Func<R, Proxy<RT, A1, A, B1, B, S>> f)
            where RT : struct, HasCancel<RT> =>
            ma.Bind(f);

        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, T> SelectMany<RT, A1, A, B1, B, R, S, T>(this Proxy<RT, A1, A, B1, B, R> ma,
            Func<R, Proxy<RT, A1, A, B1, B, S>> bind,
            Func<R, S, T> project)
            where RT : struct, HasCancel<RT> =>
            ma.Bind(x => bind(x).Map(y => project(x, y)));

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
 
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, A, T> SelectMany<RT, A, R, S, T>(this Pipe<RT, A, A, R> ma,
            Func<R, Pipe<RT, A, A, S>> bind,
            Func<R, S, T> project)
            where RT : struct, HasCancel<RT> =>
            ma.Bind(x => bind(x).Value.Map(y => project(x, y))).ToPipe();

        /// <summary>
        /// Monad bind
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        static Proxy<RT, A1, A, B1, B, S> BindInternal<RT, A1, A, B1, B, R, S>(
            this Proxy<RT, A1, A, B1, B, R> ma, 
            Func<R, Proxy<RT, A1, A, B1, B, S>> f) where RT : struct, HasCancel<RT>
        {
           
            return Go(ma);

            Proxy<RT, A1, A, B1, B, S> Go(Proxy<RT, A1, A, B1, B, R> p) =>
                p.ToProxy() switch
                {
                    Request<RT, A1, A, B1, B, R> (var a1, var fa) => new Request<RT, A1, A, B1, B, S>(a1, a => Go(fa(a))),
                    Respond<RT, A1, A, B1, B, R> (var b, var fb1) => new Respond<RT, A1, A, B1, B, S>(b, b1 => Go(fb1(b1))),
                    M<RT, A1, A, B1, B, R> (var m)                => new M<RT, A1, A, B1, B, S>(m.Map(Go)),
                    Pure<RT, A1, A, B1, B, R> (var r)             => f(r),                                                                                
                    _                                              => throw new NotSupportedException()
                };
        }
        
        /// <summary>
        /// Monad bind
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Proxy<RT, A1, A, B1, B, S> Bind<RT, A1, A, B1, B, R, S>(this Proxy<RT, A1, A, B1, B, R> ma, Func<R, Proxy<RT, A1, A, B1, B, S>> f) 
            where RT : struct, HasCancel<RT> =>
                BindInternal(ma, f);
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, B, S> Bind<RT, B, R, S>(this Proxy<RT, Void, Unit, Unit, B, R> ma, Func<R, Producer<RT, B, S>> f) 
            where RT : struct, HasCancel<RT> =>
                BindInternal(ma, f).ToProducer();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, S> Bind<RT, A, R, S>(this Proxy<RT, Unit, A, Unit, Void, R> ma, Func<R, Consumer<RT, A, S>> f) 
            where RT : struct, HasCancel<RT> =>
                BindInternal(ma, f).ToConsumer();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, A, S> Bind<RT, A, R, S>(this Pipe<RT, A, A, R> ma, Func<R, Pipe<RT, A, A, S>> f) 
            where RT : struct, HasCancel<RT> =>
                BindInternal(ma, f).ToPipe();
        
        /// <summary>
        /// Monad bind (specialised)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, A, S> Bind<RT, A, R, S>(this Producer<RT, A, R> ma, Func<R, Producer<RT, A, S>> f) 
            where RT : struct, HasCancel<RT> =>
                BindInternal(ma, f).ToProducer();        
    }
}
