using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Effects.Traits;
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
        public static Consumer<RT, A, R> Pure<RT, A, R>(R value) where RT : struct, HasCancel<RT> =>
            new Pure<RT, Unit, A, Unit, Void, R>(value).ToConsumer();

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Rel">Releases the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="RT">RTironment</typeparam>
        /// <typeparam name="A">Value to consume</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Consumer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> use<RT, A, H, R>(
            Aff<H> Acq,
            Func<H, Unit> Rel,
            Func<H, Consumer<RT, A, R>> Use) where RT : struct, HasCancel<RT> =>
                PipesInternal.Use(Acq, Use, Rel);

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="RT">RTironment</typeparam>
        /// <typeparam name="A">Value to consume</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Consumer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> use<RT, A, H, R>(
            Aff<H> Acq,
            Func<H, Consumer<RT, A, R>> Use) 
            where RT : struct, HasCancel<RT>
            where H : IDisposable =>
                PipesInternal.Use(Acq, Use, PipesInternal.Dispose);

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="RT">RTironment</typeparam>
        /// <typeparam name="A">Value to consume</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Consumer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> use<RT, A, H, R>(
            Eff<H> Acq,
            Func<H, Consumer<RT, A, R>> Use) 
            where RT : struct, HasCancel<RT>
            where H : IDisposable =>
                PipesInternal.Use(Acq.ToAff(), Use, PipesInternal.Dispose);
        
        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="RT">RTironment</typeparam>
        /// <typeparam name="A">Value to consume</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Consumer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> use<RT, A, H, R>(
            Aff<RT, H> Acq,
            Func<H, Consumer<RT, A, R>> Use) 
            where RT : struct, HasCancel<RT>
            where H : IDisposable =>
                PipesInternal.Use(Acq, Use, PipesInternal.Dispose);

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="RT">RTironment</typeparam>
        /// <typeparam name="A">Value to consume</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Consumer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> use<RT, A, H, R>(
            Eff<RT, H> Acq,
            Func<H, Consumer<RT, A, R>> Use) 
            where RT : struct, HasCancel<RT>
            where H : IDisposable =>
                PipesInternal.Use(Acq.ToAff(), Use, PipesInternal.Dispose);
        
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
        public static Consumer<RT, A, A> awaiting<RT, A>() where RT : struct, HasCancel<RT> =>
            request<RT, Unit, A, Unit, Void>(unit).ToConsumer();

        
        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> liftIO<RT, A, R>(Aff<R> ma) where RT : struct, HasCancel<RT> =>
            liftIO<RT, Unit, A, Unit, Void, R>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> liftIO<RT, A, R>(Eff<R> ma) where RT : struct, HasCancel<RT> =>
            liftIO<RT, Unit, A, Unit, Void, R>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> liftIO<RT, A, R>(Aff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            liftIO<RT, Unit, A, Unit, Void, R>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> liftIO<RT, A, R>(Eff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            liftIO<RT, Unit, A, Unit, Void, R>(ma).ToConsumer();


        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, Unit> liftIO<RT, A>(Aff<RT, Unit> ma) where RT : struct, HasCancel<RT> =>
            liftIO<RT, Unit, A, Unit, Void, Unit>(ma).ToConsumer();

        /// <summary>
        /// Lift the IO monad into the Consumer monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, Unit> liftIO<RT, A>(Eff<RT, Unit> ma) where RT : struct, HasCancel<RT> =>
            liftIO<RT, Unit, A, Unit, Void, Unit>(ma).ToConsumer();

        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> repeat<RT, A, R>(Consumer<RT, A, R> ma) where RT : struct, HasCancel<RT> =>
            ma.Bind(a => repeat(ma)); // TODO: Remove recursion

        
        
        
        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> mapM<RT, A, R>(Func<A, Aff<RT, Unit>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, R>()
                 .For<RT, A, A, R>(a => liftIO<RT, A>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, Unit> mapM<RT, A>(Func<A, Aff<RT, Unit>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, Unit>()
                 .For<RT, A, A, Unit>(a => liftIO<RT, A>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> mapM<RT, A, R>(Func<A, Eff<RT, Unit>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, R>()
                 .For<RT, A, A, R>(a => liftIO<RT, A>(f(a)));
        
        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, Unit> mapM<RT, A>(Func<A, Eff<RT, Unit>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, Unit>()
                 .For<RT, A, A, Unit>(a => liftIO<RT, A>(f(a)));
        
        

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> mapM<RT, A, R>(Func<A, Aff<Unit>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, R>()
                 .For<RT, A, A, R>(a => liftIO<RT, A, Unit>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, Unit> mapM<RT, A>(Func<A, Aff<Unit>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, Unit>()
                 .For<RT, A, A, Unit>(a => liftIO<RT, A, Unit>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, R> mapM<RT, A, R>(Func<A, Eff<Unit>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, R>()
                 .For<RT, A, A, R>(a => liftIO<RT, A, Unit>(f(a)));

        /// <summary>
        /// Consume all values using a monadic function
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Consumer<RT, A, Unit> mapM<RT, A>(Func<A, Eff<Unit>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, Unit>()
                 .For<RT, A, A, Unit>(a => liftIO<RT, A, Unit>(f(a)));        
    }
}
