using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.Effects.Traits;
using LanguageExt.Pipes;
using static LanguageExt.Pipes.Proxy;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static class Pipe
    {
        /// <summary>
        /// Monad return / pure
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> Pure<RT, A, B, R>(R value) where RT : struct, HasCancel<RT> =>
            new Pure<RT, Unit, A, Unit, B, R>(value).ToPipe();
        
        /// <summary>
        /// Wait for a value from upstream (whilst in a pipe)
        /// </summary>
        /// <remarks>
        /// This is the version of `await` that works for pipes.  In consumers, use `Consumer.await`
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, Y, A> await<RT, A, Y>() where RT : struct, HasCancel<RT> =>
            request<RT, Unit, A, Unit, Y>(unit).ToPipe();
        
        /// <summary>
        /// Send a value downstream (whilst in a pipe)
        /// </summary>
        /// <remarks>
        /// This is the version of `yield` that works for pipes.  In producers, use `Producer.yield`
        /// </remarks>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, X, A, Unit> yield<RT, X, A>(A value) where RT : struct, HasCancel<RT> =>
            respond<RT, Unit, X, Unit, A>(value).ToPipe();

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Rel">Releases the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="B">Value to produce</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Producer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> use<RT, A, B, H, R>(
            Aff<H> Acq,
            Func<H, Unit> Rel,
            Func<H, Pipe<RT, A, B, R>> Use) where RT : struct, HasCancel<RT> =>
                PipesInternal.Use(Acq, Use, Rel);

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="B">Value to produce</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Producer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> use<RT, A, B, H, R>(
            Aff<H> Acq,
            Func<H, Pipe<RT, A, B, R>> Use) 
            where RT : struct, HasCancel<RT>
            where H : IDisposable =>
                PipesInternal.Use(Acq, Use, PipesInternal.Dispose);

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="B">Value to produce</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Producer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> use<RT, A, B, H, R>(
            Eff<H> Acq,
            Func<H, Pipe<RT, A, B, R>> Use) 
            where RT : struct, HasCancel<RT>
            where H : IDisposable =>
                PipesInternal.Use(Acq.ToAff(), Use, PipesInternal.Dispose);
        
        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="B">Value to produce</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Producer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> use<RT, A, B, H, R>(
            Aff<RT, H> Acq,
            Func<H, Pipe<RT, A, B, R>> Use) 
            where RT : struct, HasCancel<RT>
            where H : IDisposable =>
                PipesInternal.Use(Acq, Use, PipesInternal.Dispose);

        /// <summary>
        /// Resource management 
        /// </summary>
        /// <param name="Acq">Acquires the resource</param>
        /// <param name="Use">Uses the resource</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="B">Value to produce</typeparam>
        /// <typeparam name="H">Type of resource to acquire</typeparam>
        /// <typeparam name="R">Return value of the Use operation</typeparam>
        /// <returns>Producer</returns>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> use<RT, A, B, H, R>(
            Eff<RT, H> Acq,
            Func<H, Pipe<RT, A, B, R>> Use) 
            where RT : struct, HasCancel<RT>
            where H : IDisposable =>
                PipesInternal.Use(Acq.ToAff(), Use, PipesInternal.Dispose);

        /// <summary>
        /// Only forwards values that satisfy the predicate.
        /// </summary>
        public static Pipe<RT, A, A, Unit> filter<RT, A>(Func<A, bool> f)  where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, Unit>().For(a => f(a)
                    ? Pipe.yield<RT, A, A>(a)
                    : Pipe.Pure<RT, A, A, Unit>(default))
                .ToPipe();

        /// <summary>
        /// Map the output of the pipe (not the bound value as is usual with Map)
        /// </summary>
        public static Pipe<RT, A, B, R> map<RT, A, B, R>(Func<A, B> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, R>().For(a => Pipe.yield<RT, A, B>(f(a))).ToPipe();

        /// <summary>
        /// Map the output of the pipe (not the bound value as is usual with Map)
        /// </summary>
        public static Pipe<RT, A, B, Unit> map<RT, A, B>(Func<A, B> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, Unit>().For(a => Pipe.yield<RT, A, B>(f(a))).ToPipe();

        /// <summary>
        /// Map the output of the pipe (not the bound value as is usual with Map)
        /// </summary>
        public static Pipe<A, B, Unit> map<A, B>(Func<A, B> f) =>
            new Pipe<A, B, Unit>.Await(x => new Pipe<A, B, Unit>.Yield(f(x), PureProxy.PipePure<A, B, Unit>));

        /// <summary>
        /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> mapM<RT, A, B, R>(Func<A, Aff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, R>()
                 .For<RT, A, A, B, R>(a => Pipe.liftIO<RT, A, B, B>(f(a))
                                                .Bind(Pipe.yield<RT, A, B>)
                 .ToPipe());

        /// <summary>
        /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, A, Unit> mapM<RT, A>(Func<A, Aff<RT, A>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, Unit>()
                .For<RT, A, A, A, Unit>(a => Pipe.liftIO<RT, A, A, A>(f(a))
                    .Bind(Pipe.yield<RT, A, A>)
                    .ToPipe());

        /// <summary>
        /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> mapM<RT, A, B, R>(Func<A, Aff<B>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, R>()
                 .For<RT, A, A, B, R>(a => Pipe.liftIO<RT, A, B, B>(f(a))
                                                .Bind(Pipe.yield<RT, A, B>)
                 .ToPipe());
        
        /// <summary>
        /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> mapM<RT, A, B, R>(Func<A, Eff<RT, B>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, R>()
                .For<RT, A, A, B, R>(a => Pipe.liftIO<RT, A, B, B>(f(a))
                    .Bind(Pipe.yield<RT, A, B>)
                    .ToPipe());
        
        /// <summary>
        /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, A, Unit> mapM<RT, A>(Func<A, Eff<RT, A>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, Unit>()
                .For<RT, A, A, A, Unit>(a => Pipe.liftIO<RT, A, A, A>(f(a))
                    .Bind(Pipe.yield<RT, A, A>)
                    .ToPipe());

        /// <summary>
        /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> mapM<RT, A, B, R>(Func<A, Eff<B>> f) where RT : struct, HasCancel<RT> =>
            Proxy.cat<RT, A, R>()
                .For<RT, A, A, B, R>(a => Pipe.liftIO<RT, A, B, B>(f(a))
                    .Bind(Pipe.yield<RT, A, B>)
                    .ToPipe());

        /// <summary>
        /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> liftIO<RT, A, B, R>(Aff<R> ma) where RT : struct, HasCancel<RT> =>
            liftIO<RT, Unit, A, Unit, B, R>(ma).ToPipe(); 
 
        /// <summary>
        /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> liftIO<RT, A, B, R>(Eff<R> ma) where RT : struct, HasCancel<RT> =>
            liftIO<RT, Unit, A, Unit, B, R>(ma).ToPipe(); 
 
        /// <summary>
        /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> liftIO<RT, A, B, R>(Aff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            liftIO<RT, Unit, A, Unit, B, R>(ma).ToPipe(); 
 
        /// <summary>
        /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
        /// </summary>
        [Pure, MethodImpl(Proxy.mops)]
        public static Pipe<RT, A, B, R> liftIO<RT, A, B, R>(Eff<RT, R> ma) where RT : struct, HasCancel<RT> =>
            liftIO<RT, Unit, A, Unit, B, R>(ma).ToPipe(); 
    }
}
