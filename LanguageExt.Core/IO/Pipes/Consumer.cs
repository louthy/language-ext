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
        public static Consumer<Env, A, R> Pure<Env, A, R>(R value) where Env : struct, HasCancel<Env> =>
            new Pure<Env, Unit, A, Unit, Void, R>(value).ToConsumer();

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
            AffPure<H> Acq,
            Func<H, Unit> Rel,
            Func<H, Consumer<Env, A, R>> Use) where Env : struct, HasCancel<Env> =>
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
            AffPure<H> Acq,
            Func<H, Consumer<Env, A, R>> Use) 
            where Env : struct, HasCancel<Env>
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
            EffPure<H> Acq,
            Func<H, Consumer<Env, A, R>> Use) 
            where Env : struct, HasCancel<Env>
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
            Aff<Env, H> Acq,
            Func<H, Consumer<Env, A, R>> Use) 
            where Env : struct, HasCancel<Env>
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
            Eff<Env, H> Acq,
            Func<H, Consumer<Env, A, R>> Use) 
            where Env : struct, HasCancel<Env>
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
        public static Consumer<Env, A, A> awaiting<Env, A>() where Env : struct, HasCancel<Env> =>
            request<Env, Unit, A, Unit, Void>(unit).ToConsumer();

        
        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> liftIO<Env, A, R>(AffPure<R> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, Unit, A, Unit, Void, R>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> liftIO<Env, A, R>(EffPure<R> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, Unit, A, Unit, Void, R>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> liftIO<Env, A, R>(Aff<Env, R> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, Unit, A, Unit, Void, R>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> liftIO<Env, A, R>(Eff<Env, R> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, Unit, A, Unit, Void, R>(ma).ToConsumer();


        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, Unit> liftIO<Env, A>(Aff<Env, Unit> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, Unit, A, Unit, Void, Unit>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, Unit> liftIO<Env, A>(Eff<Env, Unit> ma) where Env : struct, HasCancel<Env> =>
            liftIO<Env, Unit, A, Unit, Void, Unit>(ma).ToConsumer();

        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> repeat<Env, A, R>(Consumer<Env, A, R> ma) where Env : struct, HasCancel<Env> =>
            ma.Bind(a => repeat(ma)); // TODO: Remove recursion

        
        
        
        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> mapM<Env, A, R>(Func<A, Aff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Proxy.cat<Env, A, R>()
                 .For<Env, A, A, R>(a => liftIO<Env, A>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, Unit> mapM<Env, A>(Func<A, Aff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Proxy.cat<Env, A, Unit>()
                 .For<Env, A, A, Unit>(a => liftIO<Env, A>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> mapM<Env, A, R>(Func<A, Eff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Proxy.cat<Env, A, R>()
                 .For<Env, A, A, R>(a => liftIO<Env, A>(f(a)));
        
        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, Unit> mapM<Env, A>(Func<A, Eff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Proxy.cat<Env, A, Unit>()
                 .For<Env, A, A, Unit>(a => liftIO<Env, A>(f(a)));
        
        

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> mapM<Env, A, R>(Func<A, AffPure<Unit>> f) where Env : struct, HasCancel<Env> =>
            Proxy.cat<Env, A, R>()
                 .For<Env, A, A, R>(a => liftIO<Env, A, Unit>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, Unit> mapM<Env, A>(Func<A, AffPure<Unit>> f) where Env : struct, HasCancel<Env> =>
            Proxy.cat<Env, A, Unit>()
                 .For<Env, A, A, Unit>(a => liftIO<Env, A, Unit>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, R> mapM<Env, A, R>(Func<A, EffPure<Unit>> f) where Env : struct, HasCancel<Env> =>
            Proxy.cat<Env, A, R>()
                 .For<Env, A, A, R>(a => liftIO<Env, A, Unit>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<Env, A, Unit> mapM<Env, A>(Func<A, EffPure<Unit>> f) where Env : struct, HasCancel<Env> =>
            Proxy.cat<Env, A, Unit>()
                 .For<Env, A, A, Unit>(a => liftIO<Env, A, Unit>(f(a)));        
    }
}
