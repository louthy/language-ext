using System;
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
        public static Producer<Env, B, R> Pure<Env, B, R>(R value) where Env : struct, HasCancel<Env> =>
            new Pure<Env, Void, Unit, Unit, B, R>(value).ToProducer();
        
        /// <summary>
        /// Send a value downstream (whilst in a producer)
        /// </summary>
        /// <remarks>
        /// This is the simpler version (fewer generic arguments required) of `yield` that works
        /// for producers.  In pipes, use `yieldP`
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, A, Unit> yield<Env, A>(A value) where Env : struct, HasCancel<Env> =>
            respond<Env, Void, Unit, Unit, A>(value).ToProducer();
        
        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Rel">Releases the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="B">Value to produce</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Producer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, B, R> use<Env, B, H, R>(
            Aff<H> Acq,
            Func<H, Unit> Rel,
            Func<H, Producer<Env, B, R>> Use) where Env : struct, HasCancel<Env> =>
                PipesInternal.Use(Acq, Use, Rel);

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="B">Value to produce</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Producer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, B, R> use<Env, B, H, R>(
            Aff<H> Acq,
            Func<H, Producer<Env, B, R>> Use) 
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
                PipesInternal.Use(Acq, Use, PipesInternal.Dispose);

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="B">Value to produce</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Producer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, B, R> use<Env, B, H, R>(
            Aff<Env, H> Acq,
            Func<H, Producer<Env, B, R>> Use)
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
                PipesInternal.Use(Acq, Use, PipesInternal.Dispose);

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="B">Value to produce</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Producer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, B, R> use<Env, B, H, R>(
            Eff<H> Acq,
            Func<H, Producer<Env, B, R>> Use) 
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
                PipesInternal.Use(Acq.ToAsync(), Use, PipesInternal.Dispose);        

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="B">Value to produce</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Producer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, B, R> use<Env, B, H, R>(
            Eff<Env, H> Acq,
            Func<H, Producer<Env, B, R>> Use) 
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
                PipesInternal.Use(Acq.ToAsync(), Use, PipesInternal.Dispose); 
        
        static Unit Dispose(IDisposable d)
        {
            d?.Dispose();
            return default;
        }        
 
        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, A, R> repeatM<Env, A, R>(Aff<Env, A> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, A, A>(ma) | Proxy.cat<Env, A, R>();

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, A, Unit> repeatM<Env, A>(Aff<Env, A> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, A, A>(ma) | Proxy.cat<Env, A, Unit>();

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, A, R> repeatM<Env, A, R>(Eff<Env, A> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, A, A>(ma) | Proxy.cat<Env, A, R>();

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, A, Unit> repeatM<Env, A>(Eff<Env, A> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, A, A>(ma) | Proxy.cat<Env, A, Unit>();
        
        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, A, R> repeatM<Env, A, R>(Aff<A> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, A, A>(ma) | Proxy.cat<Env, A, R>();

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, A, Unit> repeatM<Env, A>(Aff<A> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, A, A>(ma) | Proxy.cat<Env, A, Unit>();

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, A, R> repeatM<Env, A, R>(Eff<A> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, A, A>(ma) | Proxy.cat<Env, A, R>();

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, A, Unit> repeatM<Env, A>(Eff<A> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, A, A>(ma) | Proxy.cat<Env, A, Unit>();

        
        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, B, R> liftIO<Env, B, R>(Eff<R> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, Void, Unit, Unit, B, R>(ma).ToProducer();

        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, B, R> liftIO<Env, B, R>(Aff<R> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, Void, Unit, Unit, B, R>(ma).ToProducer();

        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, B, R> liftIO<Env, B, R>(Eff<Env, R> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, Void, Unit, Unit, B, R>(ma).ToProducer();

        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, B, R> liftIO<Env, B, R>(Aff<Env, R> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, Void, Unit, Unit, B, R>(ma).ToProducer();

    }
}
