using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Interfaces;
using static LanguageExt.Pipes.Proxy;

namespace LanguageExt.Pipes
{
    public static class Producer
    {
        /// <summary>
        /// Monad return / pure
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, B, R> Pure<Env, B, R>(R value) where Env : Cancellable =>
            new Pure<Env, Void, Unit, Unit, B, R>(value).ToProducer();
       
        /// <summary>
        /// Monad return / pure
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Runtime, B, R> Pure<B, R>(R value) =>
            new Pure<Runtime, Void, Unit, Unit, B, R>(value).ToProducer();
        
        /// <summary>
        /// Send a value downstream (whilst in a producer)
        /// </summary>
        /// <remarks>
        /// This is the simpler version (fewer generic arguments required) of `yield` that works
        /// for producers.  In pipes, use `yieldP`
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, A, Unit> yield<Env, A>(A value) where Env : Cancellable =>
            respond<Env, Void, Unit, Unit, A>(value).ToProducer();
        
        /// <summary>
        /// Send a value downstream (whilst in a producer)
        /// </summary>
        /// <remarks>
        /// This is the simpler version (fewer generic arguments required) of `yield` that works
        /// for producers.  In pipes, use `yieldP`
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Runtime, A, Unit> yield<A>(A value) =>
            respond<Runtime, Void, Unit, Unit, A>(value).ToProducer();
        
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
            IO<H> Acq,
            Func<H, Unit> Rel,
            Func<H, Producer<Env, B, R>> Use) where Env : Cancellable =>
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
            IO<H> Acq,
            Func<H, Producer<Env, B, R>> Use) 
            where Env : Cancellable
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
            IO<Env, H> Acq,
            Func<H, Producer<Env, B, R>> Use)
            where Env : Cancellable
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
            SIO<H> Acq,
            Func<H, Producer<Env, B, R>> Use) 
            where Env : Cancellable
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
            SIO<Env, H> Acq,
            Func<H, Producer<Env, B, R>> Use) 
            where Env : Cancellable
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
        public static Producer<Env, A, R> repeatM<Env, A, R>(IO<Env, A> ma) where Env : Cancellable =>
            liftIO<Env, A, A>(ma) | Proxy.cat<Env, A, R>();

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, A, Unit> repeatM<Env, A>(IO<Env, A> ma) where Env : Cancellable =>
            liftIO<Env, A, A>(ma) | Proxy.cat<Env, A, Unit>();

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, A, R> repeatM<Env, A, R>(SIO<Env, A> ma) where Env : Cancellable =>
            liftIO<Env, A, A>(ma) | Proxy.cat<Env, A, R>();

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, A, Unit> repeatM<Env, A>(SIO<Env, A> ma) where Env : Cancellable =>
            liftIO<Env, A, A>(ma) | Proxy.cat<Env, A, Unit>();
        
        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, A, R> repeatM<Env, A, R>(IO<A> ma) where Env : Cancellable =>
            liftIO<Env, A, A>(ma) | Proxy.cat<Env, A, R>();

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, A, Unit> repeatM<Env, A>(IO<A> ma) where Env : Cancellable =>
            liftIO<Env, A, A>(ma) | Proxy.cat<Env, A, Unit>();

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, A, R> repeatM<Env, A, R>(SIO<A> ma) where Env : Cancellable =>
            liftIO<Env, A, A>(ma) | Proxy.cat<Env, A, R>();

        /// <summary>
        /// Repeat a monadic action indefinitely, yielding each result
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, A, Unit> repeatM<Env, A>(SIO<A> ma) where Env : Cancellable =>
            liftIO<Env, A, A>(ma) | Proxy.cat<Env, A, Unit>();

        
        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, B, R> liftIO<Env, B, R>(SIO<R> ma) where Env : Cancellable =>
            liftIO<Env, Void, Unit, Unit, B, R>(ma).ToProducer();

        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, B, R> liftIO<Env, B, R>(IO<R> ma) where Env : Cancellable =>
            liftIO<Env, Void, Unit, Unit, B, R>(ma).ToProducer();

        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, B, R> liftIO<Env, B, R>(SIO<Env, R> ma) where Env : Cancellable =>
            liftIO<Env, Void, Unit, Unit, B, R>(ma).ToProducer();

        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Env, B, R> liftIO<Env, B, R>(IO<Env, R> ma) where Env : Cancellable =>
            liftIO<Env, Void, Unit, Unit, B, R>(ma).ToProducer();

               
        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Runtime, B, R> liftIO<B, R>(SIO<R> ma) =>
            liftIO<Runtime, Void, Unit, Unit, B, R>(ma).ToProducer();

        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Runtime, B, R> liftIO<B, R>(IO<R> ma) =>
            liftIO<Runtime, Void, Unit, Unit, B, R>(ma).ToProducer();

        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Runtime, B, R> liftIO<B, R>(SIO<Runtime, R> ma) =>
            liftIO<Runtime, Void, Unit, Unit, B, R>(ma).ToProducer();

        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Runtime, B, R> liftIO<B, R>(IO<Runtime, R> ma) =>
            liftIO<Runtime, Void, Unit, Unit, B, R>(ma).ToProducer();

        
        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Runtime, B, B> liftIO<B>(SIO<B> ma) =>
            liftIO<Runtime, Void, Unit, Unit, B, B>(ma).ToProducer();

        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Runtime, B, B> liftIO<B>(IO<B> ma) =>
            liftIO<Runtime, Void, Unit, Unit, B, B>(ma).ToProducer();

        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Runtime, B, B> liftIO<B>(SIO<Runtime, B> ma) =>
            liftIO<Runtime, Void, Unit, Unit, B, B>(ma).ToProducer();

        /// <summary>
        /// Lift the IO monad into the Producer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Producer<Runtime, B, B> liftIO<B>(IO<Runtime, B> ma) =>
            liftIO<Runtime, Void, Unit, Unit, B, B>(ma).ToProducer();        
    }
}
