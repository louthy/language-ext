using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Effects;
using LanguageExt.Effects.Traits;
using static LanguageExt.Pipes.Proxy;

namespace LanguageExt.Pipes
{
    public static class Producer
    {
        /// <summary>
        /// Monad return / pure
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT, R> Pure<RT, OUT, R>(R value) where RT : struct, HasCancel<RT> =>
            new Pure<RT, Void, Unit, Unit, OUT, R>(value).ToProducer();
        
        /// <summary>
        /// Send a value downstream (whilst in a producer)
        /// </summary>
        /// <remarks>
        /// This is the simpler version (fewer generic arguments required) of `yield` that works
        /// for producers.  In pipes, use `yieldP`
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT, Unit> yield<RT, OUT>(OUT value) where RT : struct, HasCancel<RT> =>
            respond<RT, Void, Unit, Unit, OUT>(value).ToProducer();
        
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, X, X> enumerate<RT, X>(IEnumerable<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Enumerate<RT, Void, Unit, Unit, X, X, X>(xs, Producer.Pure<RT, X, X>).ToProducer();

        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT, X> enumerate<RT, OUT, X>(IEnumerable<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Enumerate<RT, Void, Unit, Unit, OUT, X, X>(xs, Producer.Pure<RT, OUT, X>).ToProducer();
        
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, X, X> enumerate<RT, X>(IAsyncEnumerable<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Enumerate<RT, Void, Unit, Unit, X, X, X>(xs, Producer.Pure<RT, X, X>).ToProducer();

        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT, X> enumerate<RT, OUT, X>(IAsyncEnumerable<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Enumerate<RT, Void, Unit, Unit, OUT, X, X>(xs, Producer.Pure<RT, OUT, X>).ToProducer();

        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, X, X> observe<RT, X>(IObservable<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Observer<RT, Void, Unit, Unit, X, X, X>(xs, Producer.Pure<RT, X, X>).ToProducer();

        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT, X> observe<RT, OUT, X>(IObservable<X> xs)
            where RT : struct, HasCancel<RT> =>
            new Observer<RT, Void, Unit, Unit, OUT, X, X>(xs, Producer.Pure<RT, OUT, X>).ToProducer();

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Rel">Releases the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="OUT">Value to produce</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Producer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT, R> use<RT, OUT, H, R>(
            Aff<H> Acq,
            Func<H, Unit> Rel,
            Func<H, Producer<RT, OUT, R>> Use) where RT : struct, HasCancel<RT> =>
                PipesInternal.Use(Acq, Use, Rel);

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="OUT">Value to produce</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Producer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT, R> use<RT, OUT, H, R>(
            Aff<H> Acq,
            Func<H, Producer<RT, OUT, R>> Use) 
            where RT : struct, HasCancel<RT>
            where H : IDisposable =>
                PipesInternal.Use(Acq, Use, PipesInternal.Dispose);

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="OUT">Value to produce</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Producer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT, R> use<RT, OUT, H, R>(
            Aff<RT, H> Acq,
            Func<H, Producer<RT, OUT, R>> Use)
            where RT : struct, HasCancel<RT>
            where H : IDisposable =>
                PipesInternal.Use(Acq, Use, PipesInternal.Dispose);

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="OUT">Value to produce</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Producer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT, R> use<RT, OUT, H, R>(
            Eff<H> Acq,
            Func<H, Producer<RT, OUT, R>> Use) 
            where RT : struct, HasCancel<RT>
            where H : IDisposable =>
                PipesInternal.Use(Acq.ToAff(), Use, PipesInternal.Dispose);        

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="OUT">Value to produce</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Producer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, OUT, R> use<RT, OUT, H, R>(
            Eff<RT, H> Acq,
            Func<H, Producer<RT, OUT, R>> Use) 
            where RT : struct, HasCancel<RT>
            where H : IDisposable =>
                PipesInternal.Use(Acq.ToAff(), Use, PipesInternal.Dispose); 
        
        static Unit Dispose(IDisposable d)
        {
            d?.Dispose();
            return default;
        }        
 
        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, A, R> repeatM<RT, A, R>(Aff<RT, A> ma) where RT : struct, HasCancel<RT> =>
            Proxy.compose(liftIO<RT, A, A>(ma), Proxy.cat<RT, A, R>());

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, A, Unit> repeatM<RT, A>(Aff<RT, A> ma) where RT : struct, HasCancel<RT> =>
            Proxy.compose(liftIO<RT, A, A>(ma), Proxy.cat<RT, A, Unit>());

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, A, R> repeatM<RT, A, R>(Eff<RT, A> ma) where RT : struct, HasCancel<RT> =>
            Proxy.compose(liftIO<RT, A, A>(ma), Proxy.cat<RT, A, R>());

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, A, Unit> repeatM<RT, A>(Eff<RT, A> ma) where RT : struct, HasCancel<RT> =>
            Proxy.compose(liftIO<RT, A, A>(ma), Proxy.cat<RT, A, Unit>());
        
        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, A, R> repeatM<RT, A, R>(Aff<A> ma) where RT : struct, HasCancel<RT> =>
            Proxy.compose(liftIO<RT, A, A>(ma), Proxy.cat<RT, A, R>());

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, A, Unit> repeatM<RT, A>(Aff<A> ma) where RT : struct, HasCancel<RT> =>
            Proxy.compose(liftIO<RT, A, A>(ma), Proxy.cat<RT, A, Unit>());

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, A, R> repeatM<RT, A, R>(Eff<A> ma) where RT : struct, HasCancel<RT> =>
            Proxy.compose(liftIO<RT, A, A>(ma), Proxy.cat<RT, A, R>());

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, A, Unit> repeatM<RT, A>(Eff<A> ma) where RT : struct, HasCancel<RT> =>
            Proxy.compose(liftIO<RT, A, A>(ma), Proxy.cat<RT, A, Unit>());

        
        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, B, R> liftIO<RT, B, R>(Eff<R> ma) where RT : struct, HasCancel<RT> =>
            liftIO<RT, Void, Unit, Unit, B, R>(ma).ToProducer();

        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, B, R> liftIO<RT, B, R>(Aff<R> ma) where RT : struct, HasCancel<RT> =>
            liftIO<RT, Void, Unit, Unit, B, R>(ma).ToProducer();

        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, B, R> liftIO<RT, B, R>(Eff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            liftIO<RT, Void, Unit, Unit, B, R>(ma).ToProducer();

        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<RT, B, R> liftIO<RT, B, R>(Aff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            liftIO<RT, Void, Unit, Unit, B, R>(ma).ToProducer();
    }
}
