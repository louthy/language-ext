using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Interfaces;
using static LanguageExt.Pipes.Proxy;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes
{
    public static class Consumer
    {
        /// <summary>
        /// Monad return / pure
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> Pure<Env, A, R>(R value) where Env : Cancellable =>
            new Pure<Env, Unit, A, Unit, Void, R>(value).ToConsumer();
       
        /// <summary>
        /// Monad return / pure
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Runtime, A, R> Pure<A, R>(R value) =>
            new Pure<Runtime, Unit, A, Unit, Void, R>(value).ToConsumer();

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Rel">Releases the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Value to consume</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Consumer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> use<Env, A, H, R>(
            IO<H> Acq,
            Func<H, Unit> Rel,
            Func<H, Consumer<Env, A, R>> Use) where Env : Cancellable =>
                PipesInternal.Use(Acq, Use, Rel);

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Value to consume</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Consumer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> use<Env, A, H, R>(
            IO<H> Acq,
            Func<H, Consumer<Env, A, R>> Use) 
            where Env : Cancellable
            where H : IDisposable =>
                PipesInternal.Use(Acq, Use, PipesInternal.Dispose);

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Value to consume</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Consumer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> use<Env, A, H, R>(
            SIO<H> Acq,
            Func<H, Consumer<Env, A, R>> Use) 
            where Env : Cancellable
            where H : IDisposable =>
                PipesInternal.Use(Acq.ToAsync(), Use, PipesInternal.Dispose);
        
        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Value to consume</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Consumer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> use<Env, A, H, R>(
            IO<Env, H> Acq,
            Func<H, Consumer<Env, A, R>> Use) 
            where Env : Cancellable
            where H : IDisposable =>
                PipesInternal.Use(Acq, Use, PipesInternal.Dispose);

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Value to consume</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Consumer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> use<Env, A, H, R>(
            SIO<Env, H> Acq,
            Func<H, Consumer<Env, A, R>> Use) 
            where Env : Cancellable
            where H : IDisposable =>
                PipesInternal.Use(Acq.ToAsync(), Use, PipesInternal.Dispose);
        
        static Unit Dispose(IDisposable d)
        {
            d?.Dispose();
            return default;
        }

        /// <summary>
        /// Wait for a value from upstream (whilst in a consumer)
        /// </summary>
        /// <remarks>
        /// This is the simpler version (fewer generic arguments required) of `await` that works
        /// for consumers.  In pipes, use `awaitingP`
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, A> awaiting<Env, A>() where Env : Cancellable =>
            request<Env, Unit, A, Unit, Void>(unit).ToConsumer();

        /// <summary>
        /// Wait for a value from upstream (whilst in a consumer)
        /// </summary>
        /// <remarks>
        /// This is the simpler version (fewer generic arguments required) of `await` that works
        /// for consumers.  In pipes, use `awaitingP`
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Runtime, A, A> awaiting<A>() =>
            request<Runtime, Unit, A, Unit, Void>(unit).ToConsumer();


        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> liftIO<Env, A, R>(IO<R> ma) where Env : Cancellable =>
            liftIO<Env, Unit, A, Unit, Void, R>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> liftIO<Env, A, R>(SIO<R> ma) where Env : Cancellable =>
            liftIO<Env, Unit, A, Unit, Void, R>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> liftIO<Env, A, R>(IO<Env, R> ma) where Env : Cancellable =>
            liftIO<Env, Unit, A, Unit, Void, R>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> liftIO<Env, A, R>(SIO<Env, R> ma) where Env : Cancellable =>
            liftIO<Env, Unit, A, Unit, Void, R>(ma).ToConsumer();


        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Runtime, A, R> liftIO<A, R>(IO<R> ma) =>
            liftIO<Runtime, Unit, A, Unit, Void, R>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Runtime, A, R> liftIO<A, R>(SIO<R> ma) =>
            liftIO<Runtime, Unit, A, Unit, Void, R>(ma).ToConsumer();

        
        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Runtime, A, R> liftIO<A, R>(IO<Runtime, R> ma) =>
            liftIO<Runtime, Unit, A, Unit, Void, R>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Runtime, A, R> liftIO<A, R>(SIO<Runtime, R> ma) =>
            liftIO<Runtime, Unit, A, Unit, Void, R>(ma).ToConsumer();


        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Runtime, A, Unit> liftIO<A>(IO<Unit> ma) =>
            liftIO<Runtime, Unit, A, Unit, Void, Unit>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Runtime, A, Unit> liftIO<A>(SIO<Unit> ma) =>
            liftIO<Runtime, Unit, A, Unit, Void, Unit>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, Unit> liftIO<Env, A>(IO<Env, Unit> ma) where Env : Cancellable =>
            liftIO<Env, Unit, A, Unit, Void, Unit>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, Unit> liftIO<Env, A>(SIO<Env, Unit> ma) where Env : Cancellable =>
            liftIO<Env, Unit, A, Unit, Void, Unit>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Runtime, A, Unit> liftIO<A>(IO<Runtime, Unit> ma) =>
            liftIO<Runtime, Unit, A, Unit, Void, Unit>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Runtime, A, Unit> liftIO<A>(SIO<Runtime, Unit> ma) =>
            liftIO<Runtime, Unit, A, Unit, Void, Unit>(ma).ToConsumer();

        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> repeat<Env, A, R>(Consumer<Env, A, R> ma) where Env : Cancellable =>
            ma.Bind(a => repeat(ma)); // TODO: Remove recursion

        
        
        
        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> mapM<Env, A, R>(Func<A, IO<Env, Unit>> f) where Env : Cancellable =>
            Proxy.cat<Env, A, R>()
                 .For<Env, A, A, R>(a => liftIO<Env, A>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, Unit> mapM<Env, A>(Func<A, IO<Env, Unit>> f) where Env : Cancellable =>
            Proxy.cat<Env, A, Unit>()
                 .For<Env, A, A, Unit>(a => liftIO<Env, A>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> mapM<Env, A, R>(Func<A, SIO<Env, Unit>> f) where Env : Cancellable =>
            Proxy.cat<Env, A, R>()
                 .For<Env, A, A, R>(a => liftIO<Env, A>(f(a)));
        
        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, Unit> mapM<Env, A>(Func<A, SIO<Env, Unit>> f) where Env : Cancellable =>
            Proxy.cat<Env, A, Unit>()
                 .For<Env, A, A, Unit>(a => liftIO<Env, A>(f(a)));
        
        

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> mapM<Env, A, R>(Func<A, IO<Unit>> f) where Env : Cancellable =>
            Proxy.cat<Env, A, R>()
                 .For<Env, A, A, R>(a => liftIO<Env, A, Unit>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, Unit> mapM<Env, A>(Func<A, IO<Unit>> f) where Env : Cancellable =>
            Proxy.cat<Env, A, Unit>()
                 .For<Env, A, A, Unit>(a => liftIO<Env, A, Unit>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> mapM<Env, A, R>(Func<A, SIO<Unit>> f) where Env : Cancellable =>
            Proxy.cat<Env, A, R>()
                 .For<Env, A, A, R>(a => liftIO<Env, A, Unit>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, Unit> mapM<Env, A>(Func<A, SIO<Unit>> f) where Env : Cancellable =>
            Proxy.cat<Env, A, Unit>()
                 .For<Env, A, A, Unit>(a => liftIO<Env, A, Unit>(f(a)));        
        
        

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Runtime, A, R> mapM<A, R>(Func<A, IO<Unit>> f) =>
            Proxy.cat<Runtime, A, R>()
                 .For<Runtime, A, A, R>(a => liftIO<Runtime, A, Unit>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Runtime, A, Unit> mapM<A>(Func<A, IO<Unit>> f) =>
            Proxy.cat<Runtime, A, Unit>()
                 .For<Runtime, A, A, Unit>(a => liftIO<Runtime, A, Unit>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Runtime, A, R> mapM<A, R>(Func<A, SIO<Unit>> f) =>
            Proxy.cat<Runtime, A, R>()
                 .For<Runtime, A, A, R>(a => liftIO<Runtime, A, Unit>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Runtime, A, Unit> mapM<A>(Func<A, SIO<Unit>> f) =>
            Proxy.cat<Runtime, A, Unit>()
                 .For<Runtime, A, A, Unit>(a => liftIO<Runtime, A, Unit>(f(a)));    }
}
